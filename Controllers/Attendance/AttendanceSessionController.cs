using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.Models.Entities.Attendance;

namespace SchoolManagementSystem.Controllers.Attendance
{
    [Authorize]
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

            var attendanceRepo = _uow.Repository<AttendanceRecord>();

            var rows = items.Select(s =>
            {
                var dateOnly = s.AttendanceDate;
                var classIdVal = s.SchoolClassId;
                var sectionIdVal = s.SectionId;
                var groupIdVal = s.StudentGroupId;

                var records = attendanceRepo.Query().Where(a => a.AttendanceDate == dateOnly && a.SchoolClassId == classIdVal && a.SectionId == sectionIdVal && !a.IsDeleted);
                if (groupIdVal.HasValue)
                    records = records.Where(a => a.Student != null && a.Student.StudentGroupId == groupIdVal.Value);

                var totalStudents = records.Count();
                var present = records.Count(a => a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Present);
                var absent = records.Count(a => a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Absent);

                var className = _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.SchoolClass>().Query().Where(c => c.Id == classIdVal).Select(c => c.Name).FirstOrDefault();
                var sectionName = _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.Section>().Query().Where(sx => sx.Id == sectionIdVal).Select(x => x.Name).FirstOrDefault();
                var groupName = groupIdVal.HasValue ? _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.StudentGroup>().Query().Where(g => g.Id == groupIdVal.Value).Select(g => g.Name).FirstOrDefault() : string.Empty;

                return new
                {
                    s.Id,
                    AttendanceDate = s.AttendanceDate.ToDateTime(TimeOnly.MinValue),
                    ClassId = s.SchoolClassId,
                    ClassName = className ?? string.Empty,
                    SectionId = s.SectionId,
                    SectionName = sectionName ?? string.Empty,
                    StudentGroupId = s.StudentGroupId,
                    StudentGroupName = groupName ?? string.Empty,
                    Status = s.Status.ToString(),
                    StatusValue = (int)s.Status,
                    SubmittedBy = s.CreatedBy,
                    SubmittedAt = s.CreatedAt,
                    LockedBy = s.LockedBy,
                    LockedAt = s.LockedAt,
                    UpdatedAt = s.UpdatedAt,
                    TotalStudents = totalStudents,
                    Present = present,
                    Absent = absent
                };
            }).ToList();

            return Json(new { data = rows, totalRecords = total, page, pageSize = size });
        }

        [HttpGet]
        public async Task<IActionResult> GetSessionStatus(int classId, int sectionId, DateTime date, int? groupId = null, CancellationToken ct = default)
        {
            var d = DateOnly.FromDateTime(date.Date);
            var session = await _uow.Repository<AttendanceSession>().Query().FirstOrDefaultAsync(s => s.SchoolClassId == classId && s.SectionId == sectionId && s.AttendanceDate == d && s.StudentGroupId == groupId && !s.IsDeleted, ct);
            if (session == null) return Json(new { exists = false });
            return Json(new { exists = true, status = session.Status.ToString(), submittedBy = session.CreatedBy, submittedAt = session.CreatedAt, lockedBy = session.LockedBy, lockedAt = session.LockedAt });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Super Admin")]
        public async Task<IActionResult> Unlock([FromBody] UnlockRequest dto, CancellationToken ct)
        {
            if (dto == null) return BadRequest();
            var dt = dto.AttendanceDate;
            var d = DateOnly.FromDateTime(dto.AttendanceDate.Date);
            await _attendanceService.UnlockAttendanceSessionAsync(dto.ClassId, dto.SectionId, dt, User.Identity?.Name ?? "system", dto.Reason, ct);

            var revisionRepo = _uow.Repository<AttendanceRevision>();
            var rev = new AttendanceRevision
            {
                AttendanceRecordId = 0,
                StudentId = 0,
                AttendanceDate = d,
                OldStatus = "Locked",
                NewStatus = "Revised",
                Reason = dto.Reason,
                ChangedBy = User.Identity?.Name ?? "system",
                ChangedAt = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "system",
                CreatedAt = DateTime.UtcNow
            };
            await revisionRepo.AddAsync(rev, ct);
            await _uow.SaveChangesAsync(ct);

            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Super Admin")]
        public async Task<IActionResult> Revise([FromBody] ReviseRequest dto, CancellationToken ct)
        {
            if (dto == null) return BadRequest();
            var dtRev = dto.AttendanceDate;
            var dRev = DateOnly.FromDateTime(dto.AttendanceDate.Date);
            await _attendanceService.ReviseAttendanceSessionAsync(dto.ClassId, dto.SectionId, dtRev, User.Identity?.Name ?? "system", dto.Notes, ct);
            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin,Admin")]
        public async Task<IActionResult> Approve([FromBody] ApproveRequest dto, CancellationToken ct)
        {
            if (dto == null) return BadRequest();
            var dt = dto.AttendanceDate;
            await _attendanceService.ApproveAttendanceSessionAsync(dto.ClassId, dto.SectionId, dt, User.Identity?.Name ?? "system", ct);
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetRevisions(int classId, int sectionId, DateTime date, CancellationToken ct)
        {
            var d = DateOnly.FromDateTime(date.Date);
            var revs = await _uow.Repository<AttendanceRevision>().Query()
                .Where(r => r.AttendanceDate == d && !r.IsDeleted)
                .OrderByDescending(r => r.ChangedAt)
                .ToListAsync(ct);

            return Json(new { data = revs });
        }

        public class UnlockRequest
        {
            public int ClassId { get; set; }
            public int SectionId { get; set; }
            public DateTime AttendanceDate { get; set; }
            public string Reason { get; set; } = string.Empty;
        }

        public class ReviseRequest
        {
            public int ClassId { get; set; }
            public int SectionId { get; set; }
            public DateTime AttendanceDate { get; set; }
            public string? Notes { get; set; }
        }

        public class ApproveRequest
        {
            public int ClassId { get; set; }
            public int SectionId { get; set; }
            public DateTime AttendanceDate { get; set; }
        }
    }
}
