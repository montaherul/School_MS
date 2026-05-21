using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.ViewModels.Attendance;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Attendance;

[Authorize]
public class AttendanceRecordController : Controller
{
    private readonly IAttendanceRecordService  _service;
    private readonly IStudentService           _studentService;
    private readonly ITeacherService           _teacherService;
    private readonly ITeacherAssignmentService _teacherAssignmentService;
    private readonly ISchoolClassService       _classService;
    private readonly ISectionService           _sectionService;

    public AttendanceRecordController(
        IAttendanceRecordService  service,
        IStudentService           studentService,
        ITeacherService           teacherService,
        ITeacherAssignmentService teacherAssignmentService,
        ISchoolClassService       classService,
        ISectionService           sectionService)
    {
        _service                  = service;
        _studentService           = studentService;
        _teacherService           = teacherService;
        _teacherAssignmentService = teacherAssignmentService;
        _classService             = classService;
        _sectionService           = sectionService;
    }

    [RequirePermission("Attendance.View")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewBag.Classes = await _classService.GetAllAsync(ct);
        return View();
    }

    // AJAX: sections for a class (used by dropdown cascade)
    [HttpGet]
    [RequirePermission("Attendance.View")]
    public async Task<IActionResult> GetSectionsByClass(int classId, CancellationToken ct)
    {
        var sections = await _sectionService.GetByClassIdAsync(classId, ct);
        return Json(sections.Select(s => new { id = s.Id, name = s.Name }));
    }

    // AJAX: Tabulator data endpoint
    [HttpGet]
    [RequirePermission("Attendance.View")]
    public async Task<IActionResult> GetList(
        int     page            = 1,
        int     size            = 10,
        string? search          = null,
        int?    classId         = null,
        int?    sectionId       = null,
        string? attendanceDate  = null,
        CancellationToken ct    = default)
    {
        // Student users may only see their own records
        int? studentId = null;
        if (User.IsInRole("Student"))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var uid))
                studentId = await _studentService.GetStudentIdByUserIdAsync(uid, ct);

            if (studentId == null)
                return Json(new { data = Array.Empty<object>(), last_page = 1, total_records = 0 });
        }

        DateOnly? dateFilter = null;
        if (!string.IsNullOrWhiteSpace(attendanceDate) &&
            DateOnly.TryParse(attendanceDate, out var parsed))
        {
            dateFilter = parsed;
        }

        var result = await _service.GetPagedAsync(
            page, size, search,
            studentId,
            classId,
            sectionId,
            dateFilter,
            ct);

        return Json(new
        {
            data          = result.Items,
            last_page     = (int)Math.Ceiling((double)result.TotalItems / result.PageSize),
            total_records = result.TotalItems
        });
    }

    [HttpGet]
    [RequirePermission("Attendance.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("Attendance.Create")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("Attendance.Create")]
    public async Task<IActionResult> CreateEdit(int? id, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return RedirectToAction("Index", "Home");

        bool isStaff = User.IsInRole("Super Admin") || User.IsInRole("Principal") || User.IsInRole("Assistant Head");

        if (!isStaff)
        {
            var teacher = await _teacherService.GetByUserIdAsync(userId, ct);
            if (teacher != null)
                ViewBag.AssignedClasses = await _teacherAssignmentService.GetClassesByTeacherIdAsync(teacher.Id, ct);
        }

        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value, ct);
            if (dto == null) return NotFound();
            return View(new AttendanceRecordViewModel
            {
                Id            = dto.Id,
                StudentId     = dto.StudentId,
                SchoolClassId = dto.SchoolClassId,
                SectionId     = dto.SectionId,
                Status        = dto.Status,
                Remarks       = dto.Remarks
            });
        }

        return View(new AttendanceRecordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Attendance.Create")]
    public async Task<IActionResult> CreateEdit(AttendanceRecordViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        if (vm.IsEditMode)
        {
            await _service.UpdateAsync(vm, userId, ct);
            TempData["SuccessMessage"] = "Attendance record updated successfully.";
        }
        else
        {
            await _service.CreateAsync(vm, userId, ct);
            TempData["SuccessMessage"] = "Attendance record created successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Attendance.Create")]
    public Task<IActionResult> Save(AttendanceRecordViewModel vm, CancellationToken ct)
        => CreateEdit(vm, ct);

    [HttpGet]
    [RequirePermission("Attendance.View")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var dto = await _service.GetForEditAsync(id, ct);
        if (dto == null) return NotFound();

        if (User.IsInRole("Student"))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var uid))
            {
                var sid = await _studentService.GetStudentIdByUserIdAsync(uid, ct);
                if (dto.StudentId != sid) return Forbid();
            }
        }

        return View(new AttendanceRecordViewModel
        {
            Id            = dto.Id,
            StudentId     = dto.StudentId,
            SchoolClassId = dto.SchoolClassId,
            SectionId     = dto.SectionId,
            Status        = dto.Status,
            Remarks       = dto.Remarks
        });
    }

    [HttpGet]
    [RequirePermission("Attendance.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var dto = await _service.GetForEditAsync(id, ct);
        if (dto == null) return NotFound();

        return View(new AttendanceRecordViewModel
        {
            Id            = dto.Id,
            StudentId     = dto.StudentId,
            SchoolClassId = dto.SchoolClassId,
            SectionId     = dto.SectionId,
            Status        = dto.Status,
            Remarks       = dto.Remarks
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Attendance.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId, ct);
        TempData["SuccessMessage"] = "Attendance record deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
