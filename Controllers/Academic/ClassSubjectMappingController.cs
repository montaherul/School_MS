using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.ViewModels.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class ClassSubjectMappingController : Controller
{
    private readonly IClassSubjectMappingService _mappingService;
    private readonly ISchoolClassService _classService;
    private readonly ISubjectService _subjectService;

    public ClassSubjectMappingController(
        IClassSubjectMappingService mappingService,
        ISchoolClassService classService,
        ISubjectService subjectService)
    {
        _mappingService = mappingService;
        _classService = classService;
        _subjectService = subjectService;
    }

    [RequirePermission("ClassSubjectMappings.View")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewBag.Classes = await _classService.GetAllAsync(ct);
        return View();
    }

    [HttpGet]
    [RequirePermission("ClassSubjectMappings.View")]
    public async Task<IActionResult> GetList(
        int page = 1, 
        int size = 10, 
        int? classId = null, 
        string? groupName = null, 
        string? search = null, 
        CancellationToken ct = default)
    {
        var result = await _mappingService.GetPagedAsync(page, size, classId, groupName, search, ct);
        return Json(new 
        { 
            data = result.Items, 
            last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) 
        });
    }

    [HttpGet]
    [RequirePermission("ClassSubjectMappings.Create")]
    public async Task<IActionResult> Assign(CancellationToken ct)
    {
        ViewBag.Classes = await _classService.GetAllAsync(ct);
        return View(new ClassSubjectAssignmentDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("ClassSubjectMappings.Create")]
    public async Task<IActionResult> Assign(ClassSubjectAssignmentDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Classes = await _classService.GetAllAsync(ct);
            return View(dto);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _mappingService.SaveAssignmentsAsync(dto, userId, ct);
        TempData["SuccessMessage"] = "Subjects successfully mapped to the class.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("ClassSubjectMappings.Update")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var dto = await _mappingService.GetForEditAsync(id, ct);
        if (dto == null) return NotFound();

        ViewBag.Classes = await _classService.GetAllAsync(ct);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("ClassSubjectMappings.Update")]
    public async Task<IActionResult> Edit(ClassSubjectUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Classes = await _classService.GetAllAsync(ct);
            return View(dto);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _mappingService.CreateOrUpdateAsync(dto, userId, ct);
        TempData["SuccessMessage"] = "Class subject configuration updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("ClassSubjectMappings.Delete")]
    public async Task<IActionResult> DeleteAjax([FromBody] DeleteRequest request, CancellationToken ct)
    {
        if (request == null || request.Id <= 0)
        {
            return Json(new { success = false, message = "Invalid request payload." });
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _mappingService.DeleteAsync(request.Id, userId, ct);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [RequirePermission("ClassSubjectMappings.View")]
    public async Task<IActionResult> GetUnmappedSubjects(int classId, string? groupName, CancellationToken ct)
    {
        var subjects = await _mappingService.GetUnmappedSubjectsAsync(classId, groupName, ct);
        return Json(subjects);
    }

    public class DeleteRequest
    {
        public int Id { get; set; }
    }
}
