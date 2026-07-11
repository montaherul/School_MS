using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.ViewModels.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class SectionController : Controller
{
    private readonly ISectionService _service;
    private readonly ISchoolClassService _classService;
    private readonly ITeacherScopeService _teacherScopeService;

    public SectionController(ISectionService service, ISchoolClassService classService, ITeacherScopeService teacherScopeService)
    {
        _service = service;
        _classService = classService;
        _teacherScopeService = teacherScopeService;
    }

    [RequirePermission("Sections.View")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken ct = default)
    {
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            var paged = await _service.GetPagedAsync(page, pageSize, search, ct);
            return Json(new { data = paged.Items, last_page = Math.Ceiling((double)paged.TotalItems / pageSize), total_records = paged.TotalItems });
        }
        return View();
    }

    [HttpGet]
    [RequirePermission("Sections.View")]
    public async Task<IActionResult> GetGroupsForClass(int classId, CancellationToken ct)
    {
        var groups = await _service.GetGroupsByClassIdAsync(classId, ct);
        return Json(groups);
    }

    [HttpGet]
    [RequirePermission("Sections.View")]
    public async Task<IActionResult> GetStudentGroupsForClass(int classId, CancellationToken ct)
    {
        var groups = await _service.GetStudentGroupsByClassIdAsync(classId, ct);
        return Json(groups);
    }

    [HttpGet]
    [RequirePermission("Sections.View")]
    public async Task<IActionResult> GetSectionsByClass(int classId, int? studentGroupId = null, CancellationToken ct = default)
    {
        bool isStaff = User.IsInRole("Super Admin") || User.IsInRole("Admin") || User.IsInRole("Principal") || User.IsInRole("Assistant Head");
        List<int>? assignedSectionIds = null;

        if (!isStaff)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                assignedSectionIds = (await _teacherScopeService.GetAssignedSectionIdsAsync(userId, classId, ct)).ToList();
            }
            else
            {
                assignedSectionIds = [];
            }
        }

        var sections = await _service.GetSectionsByClassWithFilterAsync(classId, isStaff, assignedSectionIds, studentGroupId, ct);
        return Json(sections);
    }

    [HttpGet]
    [RequirePermission("Sections.Create")]
    public async Task<IActionResult> CreateEdit(int? id, CancellationToken ct)
    {
        ViewBag.Classes = await _classService.GetAllAsync(ct);
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value, ct);
            if (dto == null) return NotFound();
            var vm = new SectionViewModel
            {
                Id = dto.Id,
                SchoolClassId = dto.SchoolClassId,
                Name = dto.Name,
                ParentSectionId = dto.ParentSectionId
            };
            return View(vm);
        }
        return View(new SectionViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Sections.Create")]
    public async Task<IActionResult> CreateEdit(SectionViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Classes = await _classService.GetAllAsync(ct);
            return View(vm);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        var dto = new SectionUpsertDto
        {
            Id = vm.Id,
            SchoolClassId = vm.SchoolClassId,
            Name = vm.Name,
            ParentSectionId = vm.ParentSectionId
        };

        if (vm.IsEditMode)
        {
            await _service.UpdateAsync(dto, userId, ct);
            TempData["SuccessMessage"] = "Section updated successfully.";
        }
        else
        {
            await _service.CreateAsync(dto, userId, ct);
            TempData["SuccessMessage"] = "Section created successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Sections.View")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var dto = await _service.GetForEditAsync(id, ct);
        return dto == null ? NotFound() : View(dto);
    }

    [HttpGet]
    [RequirePermission("Sections.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var dto = await _service.GetForEditAsync(id, ct);
        return dto == null ? NotFound() : View(dto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Sections.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId, ct);
        TempData["SuccessMessage"] = "Section deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}

