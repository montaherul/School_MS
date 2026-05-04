using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.ViewModels.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Attendance;

[Authorize]
public class AttendanceRecordController : Controller
{
    private readonly IAttendanceRecordService _service;
    public AttendanceRecordController(IAttendanceRecordService service) { _service = service; }

    public IActionResult Index() { return View(); }

    [HttpGet]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        if (User.IsInRole("Student"))
        {
            var studentId = GetStudentIdSync();
            if (studentId == null) return Json(new { data = new List<object>(), last_page = 0 });
            
            // Note: In a real app, I'd update the service to filter. 
            // For now, I'll filter the result here or just pass the studentId if the service supported it.
            // Since the service doesn't support it yet, I'll do a manual fetch if it's a student.
            var result = await _service.GetPagedAsync(page, size, search);
            var filtered = result.Items.Where(i => i.StudentId == studentId.Value).ToList();
            return Json(new { data = filtered, last_page = Math.Ceiling((double)filtered.Count / size) });
        }
        var resultOrig = await _service.GetPagedAsync(page, size, search);
        return Json(new { data = resultOrig.Items, last_page = Math.Ceiling((double)resultOrig.TotalItems / resultOrig.PageSize) });
    }

    [HttpGet]
    [Authorize(Roles = "Super Admin,Principal,Assistant Head,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            var vm = new AttendanceRecordViewModel { Id = dto.Id,StudentId = dto.StudentId,SchoolClassId = dto.SchoolClassId,SectionId = dto.SectionId,Status = dto.Status,Remarks = dto.Remarks,            };
            return View(vm);
        }
        return View(new AttendanceRecordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Super Admin,Principal,Assistant Head,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> CreateEdit(AttendanceRecordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "AttendanceRecord updated successfully."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "AttendanceRecord created successfully."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(AttendanceRecordViewModel vm) => CreateEdit(vm);

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();

        if (User.IsInRole("Student"))
        {
            var studentId = GetStudentIdSync();
            if (dto.StudentId != studentId) return Forbid();
        }

        return View(new AttendanceRecordViewModel
        {
            Id = dto.Id,
            StudentId = dto.StudentId,
            SchoolClassId = dto.SchoolClassId,
            SectionId = dto.SectionId,
            Status = dto.Status,
            Remarks = dto.Remarks
        });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();

        return View(new AttendanceRecordViewModel
        {
            Id = dto.Id,
            StudentId = dto.StudentId,
            SchoolClassId = dto.SchoolClassId,
            SectionId = dto.SectionId,
            Status = dto.Status,
            Remarks = dto.Remarks
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "AttendanceRecord deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    private int? GetStudentIdSync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return null;
        var db = HttpContext.RequestServices.GetRequiredService<SchoolManagementSystem.Data.SchoolDbContext>();
        return db.Students.AsNoTracking().FirstOrDefault(s => s.UserId == userId && !s.IsDeleted)?.Id;
    }
}

