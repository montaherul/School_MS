using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.ViewModels.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Attendance;

[Authorize]
public class AttendanceRecordController : Controller
{
    private readonly IAttendanceRecordService _service;
    private readonly SchoolDbContext _db;
    public AttendanceRecordController(IAttendanceRecordService service, SchoolDbContext db) 
    { 
        _service = service; 
        _db = db;
    }

    public IActionResult Index() { return View(); }

    [HttpGet]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        int? studentId = null;
        if (User.IsInRole("Student"))
        {
            studentId = GetStudentIdSync();
            if (studentId == null) return Json(new { data = new List<object>(), last_page = 0 });
        }

        var result = await _service.GetPagedAsync(page, size, search, studentId);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    [Authorize(Roles = "Super Admin,Principal,Assistant Head,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return RedirectToAction("Index", "Home");

        bool isStaff = User.IsInRole("Super Admin") || User.IsInRole("Principal") || User.IsInRole("Assistant Head");
        
        if (!isStaff)
        {
            // For Lecturers/Senior Lecturers, load only assigned classes/sections
            var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);
            if (teacher != null)
            {
                ViewBag.AssignedClasses = await _db.TeacherClassAssignments
                    .Include(a => a.Class)
                    .Where(a => a.TeacherId == teacher.Id && !a.IsDeleted)
                    .Select(a => new { a.ClassId, ClassName = a.Class.Name })
                    .Distinct()
                    .ToListAsync();
            }
        }

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

