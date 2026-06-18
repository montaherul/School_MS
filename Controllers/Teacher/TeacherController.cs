using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Helpers.Files;
using SchoolManagementSystem.Models.DTOs.Teacher;
using SchoolManagementSystem.Models.Enums;
using TeacherEntity = SchoolManagementSystem.Models.Entities.Teachers.Teacher;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Teacher;

[Authorize]
public class TeacherController : Controller
{
    private readonly ITeacherService _service;
    private readonly IFileStorageService _fileStorage;
    private readonly IUnitOfWork _uow;

    public TeacherController(ITeacherService service, IFileStorageService fileStorage, IUnitOfWork uow)
    {
        _service = service;
        _fileStorage = fileStorage;
        _uow = uow;
    }

    [RequirePermission("Teachers.View")]
    public async Task<IActionResult> Index(int page = 1, int size = 10, string? search = null, string? department = null, string? status = null, CancellationToken ct = default)
    {
        var result = await _service.GetPagedAsync(page, size, search, department, status, ct);
        
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { data = result.Items, last_page = result.TotalPages, total_records = result.TotalItems });
        }

        return View(result);
    }

    [RequirePermission("Teachers.Create")]
    public IActionResult Create()
    {
        return View("CreateEdit", new TeacherUpsertDto());
    }

    [RequirePermission("Teachers.Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
    {
        var dto = await _service.GetForEditAsync(id, ct);
        if (dto == null) return NotFound();

        var teacherEnt = await _uow.Repository<TeacherEntity>().Query()
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);
        if (teacherEnt != null) ViewBag.EmployeeId = teacherEnt.EmployeeId;

        return View("CreateEdit", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(TeacherUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return View("CreateEdit", dto);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        // Handle profile picture upload
        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            dto.ProfilePicturePath = await _fileStorage.SaveAsync(dto.ProfilePicture, "teachers", ct);
        }

        try
        {
            if (dto.Id == 0)
            {
                await _service.CreateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Teacher created and user account generated successfully.";
            }
            else
            {
                await _service.UpdateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Teacher updated successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            ModelState.AddModelError("", "Error: " + message);
            return View("CreateEdit", dto);
        }
    }

    [Route("Teacher/Details/{id?}")]
    public async Task<IActionResult> Details(int? id, CancellationToken ct = default)
    {
        int targetId;
        bool isOwnProfile = false;

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int.TryParse(userIdStr, out var currentUserId);
        
        var currentTeacherDto = await _service.GetByUserIdAsync(currentUserId, ct);

        if (id == null || id == 0)
        {
            if (currentTeacherDto == null) return NotFound("Teacher profile not found.");
            targetId = currentTeacherDto.Id;
            isOwnProfile = true;
        }
        else
        {
            targetId = id.Value;
            if (currentTeacherDto != null && currentTeacherDto.Id == targetId)
            {
                isOwnProfile = true;
            }
        }

        // Security Check
        if (!isOwnProfile && !User.HasClaim("Permission", "Teachers.View") && !User.IsInRole("Super Admin"))
        {
            return Forbid();
        }

        var dto = await _service.GetForEditAsync(targetId, ct);
        if (dto == null) return NotFound("Teacher not found.");

        var teacherEnt = await _uow.Repository<TeacherEntity>().Query()
            .FirstOrDefaultAsync(t => t.Id == targetId && !t.IsDeleted, ct);
        if (teacherEnt != null) ViewBag.EmployeeId = teacherEnt.EmployeeId;

        return View(dto);
    }

    [HttpPost]
    [RequirePermission("Teachers.Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId, ct);
        TempData["SuccessMessage"] = "Teacher deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [RequirePermission("Teachers.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeactivateAsync(id, userId, ct);
        TempData["SuccessMessage"] = "Teacher account deactivated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [RequirePermission("Teachers.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.ActivateAsync(id, userId, ct);
        TempData["SuccessMessage"] = "Teacher account activated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments(int id, CancellationToken ct)
    {
        var teacherEnt = await _uow.Repository<TeacherEntity>()
            .Query()
            .Include(t => t.Employee)
                .ThenInclude(e => e.Documents.Where(d => !d.IsDeleted))
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);

        if (teacherEnt?.Employee == null)
            return Json(Array.Empty<object>());

        var docs = teacherEnt.Employee.Documents.Select(d => new
        {
            d.Id,
            d.DocumentType,
            d.DocumentName,
            d.FilePath,
            d.ExpiryDate,
            UploadedDate = d.CreatedAt,
            d.Remarks
        });

        return Json(docs);
    }
}

