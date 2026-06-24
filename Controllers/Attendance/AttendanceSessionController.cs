using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Controllers.Attendance
{
    using SchoolManagementSystem.Filters;

    [RequirePermission("Attendance.Manage")]
    public class AttendanceSessionController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IStudentAttendanceService _attendanceService;

        public AttendanceSessionController(
            IUnitOfWork uow,
            IStudentAttendanceService attendanceService)
        {
            _uow = uow;
            _attendanceService = attendanceService;
        }

        private bool IsAdminOrPrincipal()
        {
            return User.IsInRole("Super Admin") || User.IsInRole("Admin") || User.IsInRole("Principal") || User.IsInRole("Assistant Head");
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var classes = await _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.SchoolClass>().Query()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name })
                .ToListAsync(ct);

            ViewBag.Classes = classes;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSessions(int page = 1, int size = 25, DateTime? date = null, int? classId = null, int? sectionId = null, int? groupId = null, int? status = null, CancellationToken ct = default)
        {
            page = Math.Max(1, page);
            size = Math.Clamp(size, 10, 200);
            var q = _uow.Repository<AttendanceSession>().Query().Where(s => !s.IsDeleted);

            if (date.HasValue)
            {
                var d = DateOnly.FromDateTime(date.Value.Date);
                q = q.Where(s => s.AttendanceDate == d);
            }

            if (classId.HasValue) q = q.Where(s => s.SchoolClassId == classId.Value);
            if (sectionId.HasValue) q = q.Where(s => s.SectionId == sectionId.Value);
            if (groupId.HasValue) q = q.Where(s => s.StudentGroupId == groupId.Value);
            if (status.HasValue) q = q.Where(s => (int)s.Status == status.Value);

            var total = await q.CountAsync(ct);
            var items = await q.OrderByDescending(s => s.AttendanceDate)
                .Skip((page - 1) * size).Take(size)
                .ToListAsync(ct);

            // Batch load names for all sessions in one query each
            var classIds = items.Select(s => s.SchoolClassId).Distinct().ToList();
            var sectionIds = items.Select(s => s.SectionId).Distinct().ToList();
            var groupIds = items.Select(s => s.StudentGroupId).Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
            var dates = items.Select(s => s.AttendanceDate).Distinct().ToList();

            var classDict = await _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.SchoolClass>().Query()
                .Where(c => classIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.Name, ct);

            var sectionDict = await _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.Section>().Query()
                .Where(s => sectionIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.Name, ct);

            var groupDict = groupIds.Count > 0
                ? await _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.StudentGroup>().Query()
                    .Where(g => groupIds.Contains(g.Id))
                    .ToDictionaryAsync(g => g.Id, g => g.Name, ct)
                : new Dictionary<int, string>();

            // Single batch query for attendance records
            var attendanceRecords = await _uow.Repository<AttendanceRecord>().Query()
                .Where(a => dates.Contains(a.AttendanceDate)
                         && classIds.Contains(a.SchoolClassId)
                         && sectionIds.Contains(a.SectionId)
                         && !a.IsDeleted)
                .Include(a => a.Student)
                .ToListAsync(ct);

            var recordsByKey = attendanceRecords
                .GroupBy(a => (a.AttendanceDate, a.SchoolClassId, a.SectionId))
                .ToDictionary(g => g.Key, g => g.ToList());

            var rows = items.Select(s =>
            {
                var key = (s.AttendanceDate, s.SchoolClassId, s.SectionId);
                var records = recordsByKey.TryGetValue(key, out var recs)
                    ? (IEnumerable<AttendanceRecord>)recs
                    : Enumerable.Empty<AttendanceRecord>();

                if (s.StudentGroupId.HasValue)
                    records = records.Where(a => a.Student != null && a.Student.StudentGroupId == s.StudentGroupId.Value);

                var totalStudents = records.Count();
                var present = records.Count(a => a.Status == AttendanceStatus.Present);
                var absent = records.Count(a => a.Status == AttendanceStatus.Absent);

                var className = classDict.GetValueOrDefault(s.SchoolClassId, string.Empty);
                var sectionName = sectionDict.GetValueOrDefault(s.SectionId, string.Empty);
                var groupName = s.StudentGroupId.HasValue ? groupDict.GetValueOrDefault(s.StudentGroupId.Value, string.Empty) : string.Empty;

                return new
                {
                    s.Id,
                    attendanceDate = s.AttendanceDate.ToDateTime(TimeOnly.MinValue),
                    classId = s.SchoolClassId,
                    className,
                    sectionId = s.SectionId,
                    sectionName,
                    studentGroupId = s.StudentGroupId,
                    studentGroupName = groupName,
                    status = s.Status.ToString(),
                    statusValue = (int)s.Status,
                    submittedBy = s.SubmittedBy ?? s.CreatedBy,
                    submittedAt = s.SubmittedAt ?? s.CreatedAt,
                    lockedBy = s.LockedBy,
                    lockedAt = s.LockedAt,
                    updatedAt = s.UpdatedAt,
                    totalStudents,
                    present,
                    absent
                };
            }).ToList();

            var lastPage = Math.Max(1, (int)Math.Ceiling((double)total / size));
            return Json(new { data = rows, totalRecords = total, last_page = lastPage, total_records = total, page, pageSize = size });
        }

        [HttpGet]
        public async Task<IActionResult> GetSessionStatus(int classId, int sectionId, DateTime date, int? groupId = null, CancellationToken ct = default)
        {
            var d = DateOnly.FromDateTime(date.Date);
            var query = _uow.Repository<AttendanceSession>().Query()
                .Where(s => s.SchoolClassId == classId && s.SectionId == sectionId && s.AttendanceDate == d && !s.IsDeleted);

            query = groupId.HasValue
                ? query.Where(s => s.StudentGroupId == groupId)
                : query.Where(s => s.StudentGroupId == null);

            var session = await query.FirstOrDefaultAsync(ct);
            if (session == null) return Json(new { exists = false });
            return Json(new { exists = true, status = session.Status.ToString(), submittedBy = session.SubmittedBy ?? session.CreatedBy, submittedAt = session.SubmittedAt ?? session.CreatedAt, lockedBy = session.LockedBy, lockedAt = session.LockedAt });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Attendance.Submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitRequest dto, CancellationToken ct)
        {
            if (dto == null) return BadRequest();
            try
            {
                await _attendanceService.SubmitAttendanceSessionAsync(
                    dto.ClassId, dto.SectionId, dto.StudentGroupId, dto.AttendanceDate,
                    User.Identity?.Name ?? "system", dto.Notes, ct);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Attendance.Lock")]
        public async Task<IActionResult> Lock([FromBody] LockRequest dto, CancellationToken ct)
        {
            if (dto == null) return BadRequest();
            try
            {
                await _attendanceService.LockAttendanceSessionAsync(
                    dto.ClassId, dto.SectionId, dto.StudentGroupId, dto.AttendanceDate,
                    User.Identity?.Name ?? "system", dto.Notes, ct);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Attendance.Lock")]
        public async Task<IActionResult> Unlock([FromBody] UnlockRequest dto, CancellationToken ct)
        {
            if (dto == null) return BadRequest();
            await _attendanceService.UnlockAttendanceSessionAsync(dto.ClassId, dto.SectionId, dto.StudentGroupId, dto.AttendanceDate, User.Identity?.Name ?? "system", dto.Reason, ct);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Attendance.Manage")]
        public async Task<IActionResult> Revise([FromBody] ReviseRequest dto, CancellationToken ct)
        {
            if (dto == null) return BadRequest();
            await _attendanceService.ReviseAttendanceSessionAsync(dto.ClassId, dto.SectionId, dto.StudentGroupId, dto.AttendanceDate, User.Identity?.Name ?? "system", dto.Notes, ct);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Attendance.Manage")]
        public async Task<IActionResult> Approve([FromBody] ApproveRequest dto, CancellationToken ct)
        {
            if (dto == null) return BadRequest();
            await _attendanceService.ApproveAttendanceSessionAsync(dto.ClassId, dto.SectionId, dto.StudentGroupId, dto.AttendanceDate, User.Identity?.Name ?? "system", ct);
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetRevisions(int classId, int sectionId, DateTime date, int? groupId = null, CancellationToken ct = default)
        {
            if (!IsAdminOrPrincipal()) return Forbid();

            var d = DateOnly.FromDateTime(date.Date);
            var studentIds = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
                .Where(s => s.ClassId == classId && s.SectionId == sectionId && !s.IsDeleted)
                .Where(s => !groupId.HasValue || s.StudentGroupId == groupId)
                .Select(s => s.Id)
                .ToListAsync(ct);

            var revs = await _uow.Repository<AttendanceRevision>().Query()
                .Where(r => r.AttendanceDate == d && !r.IsDeleted && studentIds.Contains(r.StudentId))
                .OrderByDescending(r => r.ChangedAt)
                .ToListAsync(ct);

            return Json(new { data = revs });
        }

        public class UnlockRequest
        {
            public int ClassId { get; set; }
            public int SectionId { get; set; }
            public int? StudentGroupId { get; set; }
            public DateTime AttendanceDate { get; set; }
            public string Reason { get; set; } = string.Empty;
        }

        public class SubmitRequest
        {
            public int ClassId { get; set; }
            public int SectionId { get; set; }
            public int? StudentGroupId { get; set; }
            public DateTime AttendanceDate { get; set; }
            public string? Notes { get; set; }
        }

        public class LockRequest
        {
            public int ClassId { get; set; }
            public int SectionId { get; set; }
            public int? StudentGroupId { get; set; }
            public DateTime AttendanceDate { get; set; }
            public string? Notes { get; set; }
        }

        public class ReviseRequest
        {
            public int ClassId { get; set; }
            public int SectionId { get; set; }
            public int? StudentGroupId { get; set; }
            public DateTime AttendanceDate { get; set; }
            public string? Notes { get; set; }
        }

        public class ApproveRequest
        {
            public int ClassId { get; set; }
            public int SectionId { get; set; }
            public int? StudentGroupId { get; set; }
            public DateTime AttendanceDate { get; set; }
        }
    }
}
