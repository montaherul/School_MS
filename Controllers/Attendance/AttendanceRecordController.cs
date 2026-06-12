using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Enums;
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
    private readonly IStudentAttendanceService  _studentAttendanceService;
    private readonly ITeacherScopeService       _teacherScopeService;
    private readonly IAttendanceAuthorizationService _attendanceAuthorizationService;
    private readonly SchoolManagementSystem.UnitOfWork.Interfaces.IUnitOfWork _unitOfWork;
    private readonly ILogger<AttendanceRecordController> _logger;
    private readonly SchoolManagementSystem.Repositories.Interfaces.Attendance.IAttendanceLogRepository _attendanceLogRepository;

    public AttendanceRecordController(
        IAttendanceRecordService  service,
        IStudentService           studentService,
        ITeacherService           teacherService,
        ITeacherAssignmentService teacherAssignmentService,
        ISchoolClassService       classService,
        ISectionService           sectionService,
        IStudentAttendanceService  studentAttendanceService,
        ITeacherScopeService       teacherScopeService,
        IAttendanceAuthorizationService attendanceAuthorizationService,
        SchoolManagementSystem.UnitOfWork.Interfaces.IUnitOfWork unitOfWork,
        ILogger<AttendanceRecordController> logger,
        SchoolManagementSystem.Repositories.Interfaces.Attendance.IAttendanceLogRepository attendanceLogRepository)
    {
        _service                  = service;
        _studentService           = studentService;
        _teacherService           = teacherService;
        _teacherAssignmentService = teacherAssignmentService;
        _classService             = classService;
        _sectionService           = sectionService;
        _studentAttendanceService = studentAttendanceService;
        _teacherScopeService      = teacherScopeService;
        _attendanceAuthorizationService = attendanceAuthorizationService;
        _unitOfWork               = unitOfWork;
        _logger                   = logger;
        _attendanceLogRepository  = attendanceLogRepository;
    }

    private async Task<bool> HasPermissionAsync(string permissionCode, CancellationToken ct)
    {
        if (User.IsInRole("Super Admin")) return true;

        var roles = User.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .ToArray();

        var db = HttpContext.RequestServices.GetRequiredService<SchoolManagementSystem.Data.SchoolDbContext>();
        return await db.RolePermissions
            .AnyAsync(rp => rp.Permission != null && rp.Role != null && rp.Permission.Code == permissionCode && roles.Contains(rp.Role.Name), ct);
    }

    private async Task LogAttendanceActionAsync(string action, string entityName, int entityId, CancellationToken ct)
    {
        var username = User.Identity?.Name ?? "Anonymous";
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        
        try
        {
            await _attendanceLogRepository.AddAsync(new AttendanceLog
            {
                UserId = username,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                IPAddress = ip,
                Timestamp = DateTime.UtcNow
            }, ct);
            
            var uow = HttpContext.RequestServices.GetRequiredService<SchoolManagementSystem.UnitOfWork.Interfaces.IUnitOfWork>();
            await uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write attendance audit log.");
        }
    }

    private bool IsAdminOrPrincipal()
    {
        return User.IsInRole("Super Admin") ||
               User.IsInRole("Admin") ||
               User.IsInRole("Principal") ||
               User.IsInRole("Assistant Head");
    }

    private async Task<bool> CanManageStudentAttendanceAsync(
        int studentId,
        int classId,
        int sectionId,
        CancellationToken ct)
    {
        if (IsAdminOrPrincipal()) return true;

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return false;

        var teacher = await _teacherService.GetByUserIdAsync(userId, ct);
        if (teacher == null) return false;

        var student = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
            .Query()
            .AsNoTracking()
            .Where(s => s.Id == studentId && !s.IsDeleted)
            .Select(s => new { s.StudentGroupId })
            .FirstOrDefaultAsync(ct);

        if (student == null) return false;

        return await _attendanceAuthorizationService.IsAuthorizedToMarkAttendanceAsync(
            teacher.Id,
            classId,
            sectionId,
            student.StudentGroupId,
            0,
            ct);
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (User.IsInRole("Student"))
        {
            await LogAttendanceActionAsync("Access Student Attendance Dashboard", "StudentDashboard", 0, ct);
            return RedirectToAction(nameof(MyAttendance));
        }

        if (!await HasPermissionAsync("Attendance.View", ct))
        {
            _logger.LogWarning("Unauthorized attempt to access Attendance Index by user {Username}", User.Identity?.Name);
            await LogAttendanceActionAsync("Unauthorized Access Attempt - Index", "AttendanceRecord", 0, ct);
            return Forbid();
        }

        ViewBag.Classes = await _classService.GetAllAsync(ct);
        return View();
    }

    // AJAX: sections for a class (used by dropdown cascade)
    [HttpGet]
    public async Task<IActionResult> GetSectionsByClass(int classId, CancellationToken ct)
    {
        if (!await HasPermissionAsync("Attendance.View", ct))
        {
            _logger.LogWarning("Unauthorized attempt to access GetSectionsByClass by user {Username}", User.Identity?.Name);
            await LogAttendanceActionAsync("Unauthorized Access Attempt - GetSectionsByClass", "AttendanceRecord", 0, ct);
            return Forbid();
        }
        var sections = await _sectionService.GetByClassIdAsync(classId, null, ct);
        return Json(sections.Select(s => new { id = s.Id, name = s.Name }));
    }

    // AJAX: Tabulator data endpoint
    [HttpGet]
    public async Task<IActionResult> GetList(
        int     page            = 1,
        int     size            = 10,
        string? search          = null,
        int?    classId         = null,
        int?    sectionId       = null,
        int?    studentGroupId  = null,
        string? attendanceDate  = null,
        CancellationToken ct    = default)
    {
        int? studentId = null;

        if (User.IsInRole("Student"))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var uid))
                studentId = await _studentService.GetStudentIdByUserIdAsync(uid, ct);

            if (studentId == null)
            {
                _logger.LogWarning("Invalid student access attempt to GetList by user {Username}", User.Identity?.Name);
                await LogAttendanceActionAsync("Invalid Student GetList Access Attempt", "AttendanceRecord", 0, ct);
                return Json(new { data = Array.Empty<object>(), last_page = 1, total_records = 0 });
            }
        }
        else
        {
            if (!await HasPermissionAsync("Attendance.View", ct))
            {
                _logger.LogWarning("Unauthorized attempt to access GetList by user {Username}", User.Identity?.Name);
                await LogAttendanceActionAsync("Unauthorized Access Attempt - GetList", "AttendanceRecord", 0, ct);
                return Json(new { data = Array.Empty<object>(), last_page = 1, total_records = 0 });
            }
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
            studentGroupId,
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

        bool isStaff = IsAdminOrPrincipal();

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

        bool isStaff = IsAdminOrPrincipal();
        if (!isStaff)
        {
            var hasAccess = await CanManageStudentAttendanceAsync(vm.StudentId, vm.SchoolClassId, vm.SectionId, ct);
            if (!hasAccess) return Forbid();
        }

        try
        {
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
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Attendance.Create")]
    public Task<IActionResult> Save(AttendanceRecordViewModel vm, CancellationToken ct)
        => CreateEdit(vm, ct);

    [HttpGet]
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
                if (dto.StudentId != sid)
                {
                    _logger.LogWarning("Student {StudentId} tried to view details of student {TargetStudentId} for record {RecordId}", sid, dto.StudentId, id);
                    await LogAttendanceActionAsync($"Unauthorized Record Access Attempt: Target student {dto.StudentId}", "AttendanceRecord", id, ct);
                    return Forbid();
                }
            }
            else
            {
                return Forbid();
            }
        }
        else
        {
            if (!await HasPermissionAsync("Attendance.View", ct))
            {
                _logger.LogWarning("Unauthorized attempt to access Details by user {Username} for record {RecordId}", User.Identity?.Name, id);
                await LogAttendanceActionAsync("Unauthorized Access Attempt - Details", "AttendanceRecord", id, ct);
                return Forbid();
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

        if (!await CanManageStudentAttendanceAsync(dto.StudentId, dto.SchoolClassId, dto.SectionId, ct)) return Forbid();

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
        var dto = await _service.GetForEditAsync(id, ct);
        if (dto == null) return NotFound();
        if (!await CanManageStudentAttendanceAsync(dto.StudentId, dto.SchoolClassId, dto.SectionId, ct)) return Forbid();

        await _service.DeleteAsync(id, userId, ct);
        TempData["SuccessMessage"] = "Attendance record deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<int?> GetLoggedInStudentIdAsync(CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdStr, out var userId))
        {
            return await _studentService.GetStudentIdByUserIdAsync(userId, ct);
        }
        return null;
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MyAttendance(CancellationToken ct)
    {
        var studentId = await GetLoggedInStudentIdAsync(ct);
        if (studentId == null)
        {
            _logger.LogWarning("Student profile not found for user {Username}", User.Identity?.Name);
            return NotFound("Student profile not found.");
        }

        var today = DateTime.Today;
        var summary = await _studentAttendanceService.GetMonthlySummaryAsync(studentId.Value, today.Year, today.Month, ct);
        
        await LogAttendanceActionAsync("View Student Attendance Dashboard", "StudentDashboard", studentId.Value, ct);

        return View(summary);
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyAttendanceData(int? year = null, int? month = null, CancellationToken ct = default)
    {
        var studentId = await GetLoggedInStudentIdAsync(ct);
        if (studentId == null)
        {
            return Json(new { success = false, message = "Student profile not found." });
        }

        var today = DateTime.Today;
        var y = year ?? today.Year;
        var m = month ?? today.Month;

        var history = await _studentAttendanceService.GetAttendanceHistoryAsync(studentId.Value, y, m, ct);
        var summary = await _studentAttendanceService.GetMonthlySummaryAsync(studentId.Value, y, m, ct);

        // Check today's status
        var todayRecord = history.FirstOrDefault(h => DateOnly.FromDateTime(h.AttendanceDate) == DateOnly.FromDateTime(today));
        var todayStatus = todayRecord?.StatusName ?? "Not Marked";

        return Json(new {
            success = true,
            summary,
            history = history.Select(h => new {
                date = h.AttendanceDate.ToString("yyyy-MM-dd"),
                formattedDate = h.AttendanceDate.ToString("dd MMM yyyy"),
                status = h.Status,
                statusName = h.StatusName,
                remarks = h.Remarks
            }),
            todayStatus
        });
    }
}
