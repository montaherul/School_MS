using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Controllers.Attendance
{
    [Authorize(Roles = "Super Admin,Admin,Principal,Assistant Head,Senior Lecturer,Lecturer,Teacher")]
    public class StudentAttendanceController : Controller
    {
        private readonly IStudentAttendanceService _service;
        private readonly ISchoolClassService _classService;
        private readonly ITeacherScopeService _teacherScopeService;
        private readonly SchoolManagementSystem.Services.Interfaces.Attendance.IAttendanceAuthorizationService _attendanceAuthService;
        private readonly IAttendanceReportService _attendanceReportService;
        private readonly ITeacherService _teacherService;
        private readonly IUnitOfWork _uow;

        public StudentAttendanceController(
            IStudentAttendanceService service,
            ISchoolClassService classService,
            ITeacherScopeService teacherScopeService,
            SchoolManagementSystem.Services.Interfaces.Attendance.IAttendanceAuthorizationService attendanceAuthService,
            IAttendanceReportService attendanceReportService,
            ITeacherService teacherService,
            IUnitOfWork uow)
        {
            _service = service;
            _classService = classService;
            _teacherScopeService = teacherScopeService;
            _attendanceAuthService = attendanceAuthService;
            _attendanceReportService = attendanceReportService;
            _teacherService = teacherService;
            _uow = uow;
        }

        private bool IsAdminOrPrincipal()
        {
            return User.IsInRole("Super Admin") || User.IsInRole("Admin") || User.IsInRole("Principal") || User.IsInRole("Assistant Head");
        }

        private async Task<bool> CanViewStudentAttendanceAsync(int studentId, CancellationToken ct)
        {
            if (IsAdminOrPrincipal()) return true;

            if (User.IsInRole("Student"))
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out var userId))
                {
                    var loggedInStudent = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
                        .Query()
                        .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);
                    return loggedInStudent?.Id == studentId;
                }
                return false;
            }

            var teacherUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(teacherUserIdStr, out var teacherUserId)) return false;

            var teacher = await _teacherService.GetByUserIdAsync(teacherUserId, ct);
            if (teacher == null) return false;

            var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
                .Query()
                .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted, ct);
            if (student == null) return false;

            return await _attendanceAuthService.IsAuthorizedToMarkAttendanceAsync(
                teacher.Id, student.ClassId, student.SectionId, student.StudentGroupId, 0, ct);
        }

        private async Task<bool> TeacherCanManageAttendanceAsync(int classId, int sectionId, int? studentGroupId, CancellationToken ct)
        {
            var teacher = await GetCurrentTeacherAsync(ct);
            if (teacher == null) return false;

            return await _attendanceAuthService.IsAuthorizedToMarkAttendanceAsync(
                teacher.Id,
                classId,
                sectionId,
                studentGroupId,
                0,
                ct);
        }

        private async Task<SchoolManagementSystem.Models.DTOs.Teacher.TeacherUpsertDto?> GetCurrentTeacherAsync(CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId)) return null;
            return await _teacherService.GetByUserIdAsync(userId, ct);
        }

        private async Task<bool> IsTeacherLockedOutAsync(DateTime attendanceDate, CancellationToken ct)
        {
            var settings = await _uow.Repository<SchoolManagementSystem.Models.Entities.Attendance.AttendanceSetting>()
                .Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            var lockAfterHours = settings?.AttendanceLockAfterHours ?? 24;
            var lockAt = attendanceDate.Date.AddHours(lockAfterHours);
            return DateTime.Now > lockAt;
        }

        private int TryParseClassNumber(string className)
        {
            // Try to extract the numeric part from class name like "Class 10", "10", "X", etc.
            if (string.IsNullOrEmpty(className)) return 0;

            // Remove common prefix
            var trimmed = className.Replace("Class ", "").Trim();

            // Use regex to find first number
            var match = System.Text.RegularExpressions.Regex.Match(trimmed, "\\d+");
            if (match.Success && int.TryParse(match.Value, out var num))
                return num;

            // Handle Roman numerals
            return trimmed.ToUpper() switch
            {
                "IX" => 9,
                "X" => 10,
                _ => 0
            };
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var classes = await _classService.GetAllAsync(ct);
            int? defaultClassId = null;
            int? defaultSectionId = null;

            if (!IsAdminOrPrincipal())
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out var userId))
                {
                    var assignedClassIds = await _teacherScopeService.GetAssignedClassIdsAsync(userId, ct);
                    classes = classes.Where(c => assignedClassIds.Contains(c.Id)).ToList();

                    if (classes.Any())
                    {
                        defaultClassId = classes.First().Id;
                        var assignedSectionIds = await _teacherScopeService.GetAssignedSectionIdsAsync(userId, defaultClassId.Value, ct);
                        if (assignedSectionIds.Any())
                        {
                            defaultSectionId = assignedSectionIds.First();
                        }
                    }
                }
                else
                {
                    classes = new List<SchoolClassListItemDto>();
                }
            }

            ViewBag.Classes = new SelectList(classes, "Id", "Name", defaultClassId);
            ViewBag.DefaultClassId = defaultClassId;
            ViewBag.DefaultSectionId = defaultSectionId;

            // Determine default group ID for class 9-10 based on curriculum rules
            int? defaultGroupId = null;
            if (defaultClassId.HasValue)
            {
                // Find class name from the list
                var selectedClass = classes.FirstOrDefault(c => c.Id == defaultClassId.Value);
                if (selectedClass != null)
                {
                    var classNumber = TryParseClassNumber(selectedClass.Name);
                    if (classNumber >= 9 && classNumber <= 10)
                    {
                        var group = await _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.StudentGroup>()
                            .Query()
                            .Where(g => g.IsActive && !g.IsDeleted && classNumber >= g.MinClass && classNumber <= g.MaxClass)
                            .OrderBy(g => g.DisplayOrder)
                            .FirstOrDefaultAsync();
                        if (group != null)
                            defaultGroupId = group.Id;
                    }
                }
            }
            ViewBag.DefaultGroupId = defaultGroupId;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedData(int page = 1, int size = 10, int? classId = null, int? sectionId = null, int? studentGroupId = null, string? date = null, CancellationToken ct = default)
        {
            if (!IsAdminOrPrincipal())
            {
                if (!classId.HasValue || !sectionId.HasValue)
                {
                    return Json(new { data = new List<object>(), last_page = 1, total_records = 0 });
                }

                if (!await TeacherCanManageAttendanceAsync(classId.Value, sectionId.Value, studentGroupId, ct)) return Forbid();
            }

            DateTime? parsedDate = null;
            if (DateTime.TryParse(date, out var d)) parsedDate = d;

            var result = await _service.GetPagedAsync(page, size, classId, sectionId, studentGroupId, parsedDate, ct);
            return Json(new { data = result.Data, last_page = Math.Ceiling((double)result.TotalRecords / size), total_records = result.TotalRecords });
        }

        /// <summary>
        /// AJAX endpoint: Load attendance with filter and summary - matches EmployeeAttendanceController pattern
        /// </summary>
        [HttpGet]
        [Route("StudentAttendance/LoadAttendance")]
        public async Task<IActionResult> LoadAttendance(
     int page = 1,
     int size = 25,
     DateTime? attendanceDate = null,
     int? classId = null,
     int? sectionId = null,
     int? studentGroupId = null,
     CancellationToken ct = default)
        {
            try
            {
                // Authorization check
                if (!IsAdminOrPrincipal())
                {
                    if (!classId.HasValue || !sectionId.HasValue)
                    {
                        return Json(new
                        {
                            success = false,
                            data = new List<object>(),
                            last_page = 1,
                            total_records = 0,
                            summary = new StudentAttendanceSummaryDto()
                        });
                    }

                    if (!await TeacherCanManageAttendanceAsync(classId.Value, sectionId.Value, studentGroupId, ct)) return Forbid();
                }

                var filter = new StudentAttendanceFilterDto
                {
                    AttendanceDate = (attendanceDate ?? DateTime.Today).Date,
                    ClassId = classId,
                    SectionId = sectionId,
                    StudentGroupId = studentGroupId
                };

                var result =
                    await _service.LoadAttendanceAsync(
                        filter,
                        page,
                        size,
                        ct);

                return Json(new
                {
                    success = true,
                    data = result.Data,
                    last_page = Math.Max(
                        1,
                        (int)Math.Ceiling(
                            (double)result.TotalRecords / size)),
                    total_records = result.TotalRecords,
                    summary = result.Summary
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Mark([FromBody] StudentAttendanceItemDto dto, int classId, int sectionId, string date, CancellationToken ct)
        {
            if (dto == null)
            {
                return Json(new { success = false, message = "DTO is null" });
            }

            if (!IsAdminOrPrincipal())
            {
                if (DateTime.TryParse(date, out var parsedLockDate) &&
                    await IsTeacherLockedOutAsync(parsedLockDate.Date, ct))
                {
                    return Json(new { success = false, message = "Attendance cannot be marked or edited after the configured lock time." });
                }

                if (DateTime.TryParse(date, out var parsedPastDate) && parsedPastDate.Date < DateTime.Today)
                {
                    return Json(new { success = false, message = "Teachers cannot edit attendance for past dates." });
                }

                var studentGroupId = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
                    .Query()
                    .AsNoTracking()
                    .Where(s => s.Id == dto.StudentId && !s.IsDeleted)
                    .Select(s => s.StudentGroupId)
                    .FirstOrDefaultAsync(ct);

                if (!await TeacherCanManageAttendanceAsync(classId, sectionId, studentGroupId, ct)) return Forbid();
            }

            try
            {
                if (!DateTime.TryParse(date, out var parsedDate)) return BadRequest("Invalid date");
                var userName = User.Identity?.Name ?? "Unknown";
                await _service.MarkAttendanceAsync(dto, classId, sectionId, parsedDate, userName, ct);
                return Json(new { success = true, message = "Attendance marked successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkMark([FromBody] StudentAttendanceBulkDto dto, CancellationToken ct)
        {
            if (dto == null)
            {
                return Json(new { success = false, message = "DTO is null" });
            }

            if (!IsAdminOrPrincipal())
            {
                if (await IsTeacherLockedOutAsync(dto.AttendanceDate.Date, ct))
                {
                    return Json(new { success = false, message = "Attendance cannot be marked or edited after the configured lock time." });
                }

                // Prevent editing past dates
                if (dto.AttendanceDate.Date < DateTime.Today)
                {
                    return Json(new { success = false, message = "Teachers cannot edit attendance for past dates." });
                }

                if (!await TeacherCanManageAttendanceAsync(dto.ClassId, dto.SectionId, dto.StudentGroupId, ct)) return Forbid();
            }

            try
            {
                var userName = User.Identity?.Name ?? "Unknown";
                await _service.BulkMarkAsync(dto, userName, ct);
                return Json(new { success = true, message = "Bulk attendance saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// AJAX endpoint: Load students for attendance grid
        /// </summary>
        [HttpGet]
        [Route("StudentAttendance/LoadStudents")]
        public async Task<IActionResult> LoadStudents(
       int classId,
       int sectionId,
       int? studentGroupId,
       DateTime attendanceDate,
       int page = 1,
       int pageSize = 50,
       CancellationToken ct = default)
        {
            try
            {
                // Authorization check
                if (!IsAdminOrPrincipal())
                {
                    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (int.TryParse(userIdStr, out var userId))
                    {
                        var teacher =
                            await _teacherService.GetByUserIdAsync(userId, ct);

                        if (teacher == null)
                            return Forbid();

                        if (!await TeacherCanManageAttendanceAsync(classId, sectionId, studentGroupId, ct))
                            return Forbid();
                    }
                    else
                    {
                        return Forbid();
                    }
                }

                var (students, total) =
                    await _service.GetStudentsForAttendanceAsync(
                        classId,
                        sectionId,
                        studentGroupId,
                        attendanceDate,
                        page,
                        pageSize,
                        ct);

                return Json(new
                {
                    success = true,
                    data = students,
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalRecords = total,
                        totalPages =
                            (int)Math.Ceiling(
                                (double)total / pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// AJAX endpoint: Save bulk attendance with automatic notifications
        /// </summary>


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAttendance(
            [FromBody] StudentAttendanceBulkDto dto,
            CancellationToken ct)
        {
            if (dto == null)
            {
                return Json(new
                {
                    success = false,
                    message = "DTO is null"
                });
            }

            if (!IsAdminOrPrincipal())
            {
                if (await IsTeacherLockedOutAsync(dto.AttendanceDate.Date, ct))
                {
                    return Json(new { success = false, message = "Attendance cannot be marked or edited after the configured lock time." });
                }

                // Prevent editing past dates
                if (dto.AttendanceDate.Date < DateTime.Today)
                {
                    return Json(new { success = false, message = "Teachers cannot edit attendance for past dates." });
                }

                if (!await TeacherCanManageAttendanceAsync(dto.ClassId, dto.SectionId, dto.StudentGroupId, ct)) return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            try
            {
                var userName = User.Identity?.Name ?? "Unknown";

                var response =
                    await _service.SaveAttendanceAsync(
                        dto,
                        userName,
                        ct);

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// AJAX endpoint: Get attendance summary cards
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSummary(int classId, int sectionId, int? studentGroupId, DateTime attendanceDate, CancellationToken ct = default)
        {
            try
            {
                if (!IsAdminOrPrincipal())
                {
                    if (!await TeacherCanManageAttendanceAsync(classId, sectionId, studentGroupId, ct)) return Forbid();
                }

                var filter = new StudentAttendanceFilterDto
                {
                    AttendanceDate = attendanceDate,
                    ClassId = classId,
                    SectionId = sectionId,
                    StudentGroupId = studentGroupId
                };

                var result = await _service.LoadAttendanceAsync(filter, 1, 1, ct);
                var summary = new
                {
                    totalStudents = result.Summary.TotalStudents,
                    present = result.Summary.Present,
                    absent = result.Summary.Absent,
                    late = result.Summary.Late,
                    leave = result.Summary.Leave
                };

                return Json(new { success = true, summary });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get attendance history for a student - matches EmployeeAttendanceController pattern
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AttendanceHistory(int studentId, int? year = null, int? month = null, CancellationToken ct = default)
        {
            try
            {
                if (!await CanViewStudentAttendanceAsync(studentId, ct)) return Forbid();

                var today = DateTime.Today;
                var y = year ?? today.Year;
                var m = month ?? today.Month;
                var rows = await _service.GetAttendanceHistoryAsync(studentId, y, m, ct);
                var summary = await _service.GetMonthlySummaryAsync(studentId, y, m, ct);
                return Json(new { data = rows, summary });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get monthly summary for a student - matches EmployeeAttendanceController pattern
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MonthlySummary(int studentId, int? year = null, int? month = null, CancellationToken ct = default)
        {
            try
            {
                if (!await CanViewStudentAttendanceAsync(studentId, ct)) return Forbid();

                var today = DateTime.Today;
                var summary = await _service.GetMonthlySummaryAsync(studentId, year ?? today.Year, month ?? today.Month, ct);
                return Json(summary);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// AJAX endpoint: Get student's attendance history (legacy - kept for compatibility)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAttendanceHistory(int studentId, int year, int month, CancellationToken ct = default)
        {
            try
            {
                if (!await CanViewStudentAttendanceAsync(studentId, ct)) return Forbid();

                var rows = await _service.GetAttendanceHistoryAsync(studentId, year, month, ct);
                var summary = await _service.GetMonthlySummaryAsync(studentId, year, month, ct);

                return Json(new { success = true, data = rows, summary });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get sections for a class - AJAX support
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSections(int classId, int? groupId = null, CancellationToken ct = default)
        {
            try
            {
                if (classId <= 0)
                    return Json(new { data = new List<object>() });

                if (!IsAdminOrPrincipal())
                {
                    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (int.TryParse(userIdStr, out var userId))
                    {
                        var assignedClassIds = await _teacherScopeService.GetAssignedClassIdsAsync(userId, ct);
                        if (!assignedClassIds.Contains(classId)) return Forbid();
                    }
                    else return Forbid();
                }

                // Get sections that belong to this class and optionally match the group
                var query = _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.Section>().Query()
                    .Where(s => s.SchoolClassId == classId && !s.IsDeleted);

                if (groupId.HasValue)
                    query = query.Where(s => s.StudentGroupId == groupId.Value);

                if (!IsAdminOrPrincipal())
                {
                    var teacher = await GetCurrentTeacherAsync(ct);
                    if (teacher == null) return Forbid();

                    var assignmentQuery = _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.TeacherClassAssignment>().Query()
                        .Where(a => a.TeacherId == teacher.Id
                            && a.ClassId == classId
                            && a.IsActive
                            && !a.IsDeleted);

                    assignmentQuery = groupId.HasValue
                        ? assignmentQuery.Where(a => a.GroupId == groupId.Value)
                        : assignmentQuery.Where(a => a.GroupId == null);

                    var assignedSectionIds = await assignmentQuery
                        .Select(a => a.SectionId)
                        .Distinct()
                        .ToListAsync(ct);

                    query = query.Where(s => assignedSectionIds.Contains(s.Id));
                }

                var sections = await query
                    .OrderBy(s => s.Name)
                    .Select(s => new { id = s.Id, name = s.Name })
                    .ToListAsync(ct);

                return Json(new { data = sections });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get student groups - note: groups are not tied to a specific class, they are global
        /// But we filter by the class MinClass/MaxClass range if it matches the current class
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetGroups(int classId, CancellationToken ct = default)
        {
            try
            {
                if (classId <= 0)
                    return Json(new { data = new List<object>() });

                if (!IsAdminOrPrincipal())
                {
                    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (int.TryParse(userIdStr, out var userId))
                    {
                        var assignedClassIds = await _teacherScopeService.GetAssignedClassIdsAsync(userId, ct);
                        if (!assignedClassIds.Contains(classId)) return Forbid();
                    }
                    else return Forbid();
                }

                // Get the class to determine if groups apply to it
                var schoolClass = await _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.SchoolClass>().Query()
                    .FirstOrDefaultAsync(c => c.Id == classId && !c.IsDeleted, ct);

                if (schoolClass == null)
                    return Json(new { data = new List<object>() });

                // Get all active student groups where this class falls within MinClass-MaxClass range
                var classNumber = TryParseClassNumber(schoolClass.Name);
                var groups = await _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.StudentGroup>().Query()
                    .Where(g => g.IsActive && !g.IsDeleted && classNumber >= g.MinClass && classNumber <= g.MaxClass)
                    .OrderBy(g => g.DisplayOrder)
                    .ToListAsync(ct);

                if (!IsAdminOrPrincipal())
                {
                    var teacher = await GetCurrentTeacherAsync(ct);
                    if (teacher == null) return Forbid();

                    var assignedGroupIds = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.TeacherClassAssignment>().Query()
                        .Where(a => a.TeacherId == teacher.Id
                            && a.ClassId == classId
                            && a.GroupId.HasValue
                            && a.IsActive
                            && !a.IsDeleted)
                        .Select(a => a.GroupId!.Value)
                        .Distinct()
                        .ToListAsync(ct);

                    groups = groups.Where(g => assignedGroupIds.Contains(g.Id)).ToList();
                }

                return Json(new { data = groups.Select(g => new { id = g.Id, name = g.Name }) });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Export attendance to CSV
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExportAttendanceCSV(int classId, int sectionId, int? studentGroupId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
        {
            try
            {
                // Authorization check
                if (!IsAdminOrPrincipal())
                {
                    if (!await TeacherCanManageAttendanceAsync(classId, sectionId, studentGroupId, ct)) return Forbid();
                }

                var (_, totalStudents) = await _service.GetStudentsForAttendanceAsync(classId, sectionId, studentGroupId, fromDate, 1, 1, ct);
                var (students, _) = await _service.GetStudentsForAttendanceAsync(classId, sectionId, studentGroupId, fromDate, 1, Math.Max(totalStudents, 1), ct);

                // Build CSV
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Roll,StudentId,StudentName,Class,Section,AttendanceDate,Status,Remarks");

                foreach (var student in students)
                {
                    csv.AppendLine($"{student.RollNumber},\"{student.StudentId}\",\"{student.StudentName}\",\"{student.ClassName}\",\"{student.SectionName}\"," +
                        $"{student.AttendanceDate:yyyy-MM-dd},{student.StatusName},\"{student.Remarks}\"");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", $"attendance_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Export attendance to PDF
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExportAttendancePDF(int classId, int sectionId, int? studentGroupId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
        {
            try
            {
                // Authorization check
                if (!IsAdminOrPrincipal())
                {
                    if (!await TeacherCanManageAttendanceAsync(classId, sectionId, studentGroupId, ct)) return Forbid();
                }

                var pdf = await _attendanceReportService.GenerateStudentAttendancePdfAsync(
                    classId,
                    sectionId,
                    fromDate,
                    toDate,
                    ct,
                    studentGroupId);

                return File(pdf, "application/pdf", $"attendance_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }

        }
    }
}