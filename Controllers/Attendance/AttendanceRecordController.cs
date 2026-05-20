using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.ViewModels.Attendance;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using System.Security.Claims;
using SchoolManagementSystem.Constants;

namespace SchoolManagementSystem.Controllers.Attendance;

[Authorize]
public class AttendanceRecordController : Controller
{
<<<<<<< HEAD
    private readonly IAttendanceRecordService _service;
    private readonly IStudentService _studentService;
    private readonly ITeacherService _teacherService;
    private readonly ITeacherAssignmentService _teacherAssignmentService;

    public AttendanceRecordController(
        IAttendanceRecordService service,
        IStudentService studentService,
        ITeacherService teacherService,
        ITeacherAssignmentService teacherAssignmentService)
    {
        _service = service;
        _studentService = studentService;
        _teacherService = teacherService;
        _teacherAssignmentService = teacherAssignmentService;
    }

    [RequirePermission("Attendance.View")]
    public IActionResult Index() { return View(); }

    [HttpGet]
    [RequirePermission("Attendance.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("Attendance.Create")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("Attendance.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, CancellationToken ct = default)
=======
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
>>>>>>> d8b24e6 (attendece and website curtomize)
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
        if (User.IsInRole(Roles.Student))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
<<<<<<< HEAD
            if (int.TryParse(userIdStr, out var userId))
            {
                studentId = await _studentService.GetStudentIdByUserIdAsync(userId, ct);
            }
            if (studentId == null) return Json(new { data = new List<object>(), last_page = 0 });
        }

        var result = await _service.GetPagedAsync(page, size, search, studentId, ct);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
=======
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
>>>>>>> d8b24e6 (attendece and website curtomize)
    }

    [HttpGet]
    [RequirePermission("Attendance.Create")]
<<<<<<< HEAD
=======
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("Attendance.Create")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("Attendance.Create")]
>>>>>>> d8b24e6 (attendece and website curtomize)
    public async Task<IActionResult> CreateEdit(int? id, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return RedirectToAction("Index", "Home");

<<<<<<< HEAD
        bool isStaff = User.IsInRole(Roles.SuperAdmin) || User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Principal);
=======
        bool isStaff = User.IsInRole("Super Admin") || User.IsInRole("Principal") || User.IsInRole("Assistant Head");
>>>>>>> d8b24e6 (attendece and website curtomize)

        if (!isStaff)
        {
            var teacher = await _teacherService.GetByUserIdAsync(userId, ct);
            if (teacher != null)
<<<<<<< HEAD
            {
                ViewBag.AssignedClasses = await _teacherAssignmentService.GetClassesByTeacherIdAsync(teacher.Id, ct);
            }
=======
                ViewBag.AssignedClasses = await _teacherAssignmentService.GetClassesByTeacherIdAsync(teacher.Id, ct);
>>>>>>> d8b24e6 (attendece and website curtomize)
        }

        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value, ct);
            if (dto == null) return NotFound();
<<<<<<< HEAD
            var vm = new AttendanceRecordViewModel { Id = dto.Id, StudentId = dto.StudentId, SchoolClassId = dto.SchoolClassId, SectionId = dto.SectionId, Status = dto.Status, Remarks = dto.Remarks };
            return View(vm);
=======
            return View(new AttendanceRecordViewModel
            {
                Id            = dto.Id,
                StudentId     = dto.StudentId,
                SchoolClassId = dto.SchoolClassId,
                SectionId     = dto.SectionId,
                Status        = dto.Status,
                Remarks       = dto.Remarks
            });
>>>>>>> d8b24e6 (attendece and website curtomize)
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
<<<<<<< HEAD
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId, ct); TempData["SuccessMessage"] = "AttendanceRecord updated successfully."; }
        else { await _service.CreateAsync(vm, userId, ct); TempData["SuccessMessage"] = "AttendanceRecord created successfully."; }
=======

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

>>>>>>> d8b24e6 (attendece and website curtomize)
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Attendance.Create")]
<<<<<<< HEAD
    public Task<IActionResult> Save(AttendanceRecordViewModel vm, CancellationToken ct) => CreateEdit(vm, ct);
=======
    public Task<IActionResult> Save(AttendanceRecordViewModel vm, CancellationToken ct)
        => CreateEdit(vm, ct);
>>>>>>> d8b24e6 (attendece and website curtomize)

    [HttpGet]
    [RequirePermission("Attendance.View")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var dto = await _service.GetForEditAsync(id, ct);
        if (dto == null) return NotFound();

        if (User.IsInRole(Roles.Student))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
<<<<<<< HEAD
            if (int.TryParse(userIdStr, out var userId))
            {
                var studentId = await _studentService.GetStudentIdByUserIdAsync(userId, ct);
                if (dto.StudentId != studentId) return Forbid();
=======
            if (int.TryParse(userIdStr, out var uid))
            {
                var sid = await _studentService.GetStudentIdByUserIdAsync(uid, ct);
                if (dto.StudentId != sid) return Forbid();
>>>>>>> d8b24e6 (attendece and website curtomize)
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
<<<<<<< HEAD
        TempData["SuccessMessage"] = "AttendanceRecord deleted successfully.";
=======
        TempData["SuccessMessage"] = "Attendance record deleted successfully.";
>>>>>>> d8b24e6 (attendece and website curtomize)
        return RedirectToAction(nameof(Index));
    }
}
