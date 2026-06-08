using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Guardian;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Models.Entities.Guardian;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using GuardianEntity = SchoolManagementSystem.Models.Entities.Guardian.Guardian;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Guardian;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Guardian;

[Authorize(Roles = "Guardian")]
[Route("Guardian/Portal")]
public class GuardianPortalPagesController : Controller
{
    private readonly IGuardianService _guardianService;
    private readonly IUnitOfWork _uow;
    private readonly IStudentExamResultRepository _studentExamResultRepository;
    private readonly ITranscriptService _transcriptService;
    private readonly ILogger<GuardianPortalPagesController> _logger;

    public GuardianPortalPagesController(
        IGuardianService guardianService,
        IUnitOfWork uow,
        IStudentExamResultRepository studentExamResultRepository,
        ITranscriptService transcriptService,
        ILogger<GuardianPortalPagesController> logger)
    {
        _guardianService = guardianService;
        _uow = uow;
        _studentExamResultRepository = studentExamResultRepository;
        _transcriptService = transcriptService;
        _logger = logger;
    }

    private int CurrentUserId()
    {
        var raw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(raw, out var id) ? id : 0;
    }

    private async Task<int> ResolveOrFirstChildAsync(int? studentId, CancellationToken ct)
    {
        var userId = CurrentUserId();
        var children = await _guardianService.GetChildrenByUserIdAsync(userId, ct);
        if (!children.Any()) return 0;
        if (studentId.HasValue && children.Any(c => c.StudentId == studentId.Value))
            return studentId.Value;
        return children.First(c => c.IsPrimaryGuardian || true).StudentId;
    }

    [HttpGet("Attendance")]
    public async Task<IActionResult> Attendance(int? studentId, DateTime? from, DateTime? to, CancellationToken ct)
    {
        var userId = CurrentUserId();
        var sid = await ResolveOrFirstChildAsync(studentId, ct);
        if (sid == 0) { return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account."); }

        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct)) return Forbid();

        var fromDate = (from ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)).Date;
        var toDate = (to ?? DateTime.Today).Date;

        var records = await _guardianService.GetChildAttendanceAsync(userId, sid, fromDate, toDate, ct);
        var child = await _guardianService.GetChildDetailAsync(userId, sid, ct);

        ViewBag.StudentName = child?.FullName;
        ViewBag.StudentId = sid;
        ViewBag.From = fromDate.ToString("yyyy-MM-dd");
        ViewBag.To = toDate.ToString("yyyy-MM-dd");
        return View("~/Views/GuardianPortal/Attendance.cshtml", records);
    }

    [HttpGet("Results")]
    public async Task<IActionResult> Results(int? studentId, CancellationToken ct)
    {
        var userId = CurrentUserId();
        var sid = await ResolveOrFirstChildAsync(studentId, ct);
        if (sid == 0) { return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account."); }
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct)) return Forbid();

        var results = await _uow.Repository<StudentExamResult>().Query()
            .AsNoTracking()
            .Include(r => r.Exam)
            .Where(r => r.StudentId == sid && !r.IsDeleted && (r.Status == ResultWorkflowStatus.Published || r.Status == ResultWorkflowStatus.Locked))
            .OrderByDescending(r => r.ExamId)
            .ToListAsync(ct);

        var subjects = await _uow.Repository<StudentSubjectResult>().Query()
            .AsNoTracking()
            .Include(s => s.Subject)
            .Where(s => s.StudentId == sid && !s.IsDeleted)
            .ToListAsync(ct);

        ViewBag.StudentId = sid;
        ViewBag.StudentName = (await _guardianService.GetChildDetailAsync(userId, sid, ct))?.FullName;
        ViewBag.SubjectResults = subjects;
        return View("~/Views/GuardianPortalPages/Results.cshtml", results);
    }

    [HttpGet("Fees")]
    public async Task<IActionResult> Fees(int? studentId, CancellationToken ct)
    {
        var userId = CurrentUserId();
        var sid = await ResolveOrFirstChildAsync(studentId, ct);
        if (sid == 0) { return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account."); }
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct)) return Forbid();

        var invoices = await _uow.Repository<FeeInvoice>().Query()
            .AsNoTracking()
            .Where(i => i.StudentId == sid && !i.IsDeleted)
            .OrderByDescending(i => i.Id)
            .ToListAsync(ct);

        var invoiceIds = invoices.Select(i => i.Id).ToList();
        var payments = await _uow.Repository<Payment>().Query()
            .AsNoTracking()
            .Where(p => invoiceIds.Contains(p.FeeInvoiceId) && !p.IsDeleted)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync(ct);

        ViewBag.StudentId = sid;
        ViewBag.StudentName = (await _guardianService.GetChildDetailAsync(userId, sid, ct))?.FullName;
        ViewBag.Payments = payments;
        ViewBag.TotalDue = invoices.Where(i => (int)i.Status != 3).Sum(i => i.TotalAmount - i.PaidAmount);
        ViewBag.TotalPaid = invoices.Sum(i => i.PaidAmount);
        return View("~/Views/GuardianPortal/Fees.cshtml", invoices);
    }

    [HttpGet("Leaves")]
    public async Task<IActionResult> Leaves(int? studentId, CancellationToken ct)
    {
        var userId = CurrentUserId();
        var sid = await ResolveOrFirstChildAsync(studentId, ct);
        if (sid == 0) { return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account."); }
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct)) return Forbid();

        var leaves = await _uow.Repository<StudentLeaveApplication>().Query()
            .AsNoTracking()
            .Include(l => l.LeaveType)
            .Where(l => l.StudentId == sid)
            .OrderByDescending(l => l.Id)
            .ToListAsync(ct);

        ViewBag.StudentId = sid;
        ViewBag.StudentName = (await _guardianService.GetChildDetailAsync(userId, sid, ct))?.FullName;
        ViewBag.LeaveTypes = await _uow.Repository<LeaveType>().Query().AsNoTracking().Where(t => t.IsActive).ToListAsync(ct);
        return View("~/Views/GuardianPortal/Leaves.cshtml", leaves);
    }

    [HttpPost("Leaves/Apply")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyLeave(int studentId, int leaveTypeId, DateTime fromDate, DateTime toDate, string reason, CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, studentId, ct)) return Forbid();

        if (toDate < fromDate)
        {
            TempData["ErrorMessage"] = "To date cannot be earlier than From date.";
            return RedirectToAction(nameof(Leaves), new { studentId });
        }

        // Find the Guardian entity to satisfy the required GuardianId
        var guardian = await _uow.Repository<GuardianEntity>().Query().AsNoTracking().FirstOrDefaultAsync(g => g.UserId == userId && !g.IsDeleted, ct);
        if (guardian == null)
        {
            TempData["ErrorMessage"] = "No guardian profile found.";
            return RedirectToAction(nameof(Leaves), new { studentId });
        }

        var entity = new StudentLeaveApplication
        {
            StudentId = studentId,
            GuardianId = guardian.Id,
            LeaveTypeId = leaveTypeId,
            FromDate = fromDate,
            ToDate = toDate,
            Reason = reason,
            ApprovalStatus = StudentLeaveApplication.ApprovalStatusEnum.Pending,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.Repository<StudentLeaveApplication>().AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "Leave application submitted successfully.";
        return RedirectToAction(nameof(Leaves), new { studentId });
    }

    [HttpGet("Notices")]
    public async Task<IActionResult> Notices(CancellationToken ct)
    {
        var notices = await _uow.Repository<Notice>().Query().AsNoTracking()
            .Where(n => !n.IsDeleted && n.IsPublished
                && (n.AudienceRole == "All" || n.AudienceRole == "Guardian" || n.AudienceRole == "Guardians" || n.AudienceRole == "Parent" || n.AudienceRole == "Parents"))
            .OrderByDescending(n => n.PublishAt)
            .Take(50)
            .ToListAsync(ct);
        return View("~/Views/GuardianPortal/Notices.cshtml", notices);
    }

    [HttpGet("Calendar")]
    public async Task<IActionResult> Calendar(CancellationToken ct)
    {
        var events = await _uow.Repository<AcademicCalendarEvent>().Query().AsNoTracking()
            .Include(e => e.AcademicCalendar)
            .Where(e => !e.IsDeleted && e.IsActive)
            .OrderBy(e => e.StartDate)
            .ToListAsync(ct);
        return View("~/Views/GuardianPortal/Calendar.cshtml", events);
    }

    [HttpGet("Notifications")]
    public async Task<IActionResult> Notifications(CancellationToken ct)
    {
        var userId = CurrentUserId();
        var guardian = await _uow.Repository<GuardianEntity>().Query().AsNoTracking().FirstOrDefaultAsync(g => g.UserId == userId && !g.IsDeleted, ct);
        if (guardian == null) { return View("~/Views/GuardianPortal/Empty.cshtml", "No guardian profile."); }

        var notifications = await _uow.Repository<GuardianNotification>().Query().AsNoTracking()
            .Where(n => n.GuardianId == guardian.Id && !n.IsDeleted)
            .OrderByDescending(n => n.Id)
            .Take(100)
            .ToListAsync(ct);
        return View("~/Views/GuardianPortal/Notifications.cshtml", notifications);
    }

    [HttpPost("Notifications/MarkRead/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id, CancellationToken ct)
    {
        var userId = CurrentUserId();
        var guardian = await _uow.Repository<GuardianEntity>().Query().AsNoTracking().FirstOrDefaultAsync(g => g.UserId == userId && !g.IsDeleted, ct);
        if (guardian == null) return Forbid();

        var n = await _uow.Repository<GuardianNotification>().Query().FirstOrDefaultAsync(x => x.Id == id && x.GuardianId == guardian.Id && !x.IsDeleted, ct);
        if (n == null) return NotFound();
        n.IsRead = true;
        n.ReadAt = DateTime.UtcNow;
        n.UpdatedAt = DateTime.UtcNow;
        n.UpdatedBy = User.Identity?.Name ?? "guardian";
        _uow.Repository<GuardianNotification>().Update(n);
        await _uow.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Notifications));
    }

    [HttpGet("Profile")]
    public async Task<IActionResult> Profile(CancellationToken ct)
    {
        var userId = CurrentUserId();
        var guardian = await _uow.Repository<GuardianEntity>().Query().AsNoTracking()
            .FirstOrDefaultAsync(g => g.UserId == userId && !g.IsDeleted, ct);
        if (guardian == null) { return View("~/Views/GuardianPortal/Empty.cshtml", "No guardian profile linked to this account."); }

        var childrenCount = await _uow.Repository<StudentGuardian>().Query().AsNoTracking()
            .CountAsync(sg => sg.GuardianId == guardian.Id && !sg.IsDeleted, ct);

        var model = new SchoolManagementSystem.Models.ViewModels.Guardian.GuardianProfileViewModel
        {
            Guardian = guardian,
            ChildrenCount = childrenCount
        };
        return View("~/Views/GuardianPortal/Profile.cshtml", model);
    }

    [HttpGet("ReportCard")]
    public async Task<IActionResult> ReportCard(int? studentId, int examId, CancellationToken ct)
    {
        var userId = CurrentUserId();
        var sid = await ResolveOrFirstChildAsync(studentId, ct);
        if (sid == 0) { return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account."); }
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct)) return Forbid();
        if (examId <= 0) return RedirectToAction(nameof(Results), new { studentId = sid });

        var dto = await _studentExamResultRepository.GetReportCardAsync(examId, sid, ct);
        if (dto == null) return NotFound("Report card not available or not yet published.");

        return View("~/Views/GuardianPortalPages/ReportCard.cshtml", dto);
    }

    [HttpGet("Transcript")]
    public async Task<IActionResult> Transcript(int? studentId, int? academicYearId, CancellationToken ct)
    {
        var userId = CurrentUserId();
        var sid = await ResolveOrFirstChildAsync(studentId, ct);
        if (sid == 0) { return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account."); }
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct)) return Forbid();

        var activeYear = await _uow.Repository<AcademicYear>().Query().AsNoTracking().FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;
        if (yearId == 0) return RedirectToAction(nameof(Results), new { studentId = sid });

        var transcript = await _transcriptService.GetStudentTranscriptAsync(sid, yearId);
        if (transcript == null) return NotFound("Transcript not found.");

        ViewBag.StudentId = sid;
        return View("~/Views/GuardianPortalPages/Transcript.cshtml", transcript);
    }

    [HttpGet("ExamComparison")]
    public async Task<IActionResult> ExamComparison(int? studentId, CancellationToken ct)
    {
        var userId = CurrentUserId();
        var sid = await ResolveOrFirstChildAsync(studentId, ct);
        if (sid == 0) { return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account."); }
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct)) return Forbid();

        var results = await _uow.Repository<StudentExamResult>().Query()
            .AsNoTracking()
            .Include(r => r.Exam)
            .Where(r => r.StudentId == sid && !r.IsDeleted && (r.Status == ResultWorkflowStatus.Published || r.Status == ResultWorkflowStatus.Locked))
            .OrderBy(r => r.ExamId)
            .ToListAsync(ct);

        var subjects = await _uow.Repository<StudentSubjectResult>().Query()
            .AsNoTracking()
            .Include(s => s.Subject)
            .Where(s => s.StudentId == sid && !s.IsDeleted)
            .ToListAsync(ct);

        ViewBag.StudentId = sid;
        ViewBag.StudentName = (await _guardianService.GetChildDetailAsync(userId, sid, ct))?.FullName;
        ViewBag.SubjectResults = subjects;
        return View("~/Views/GuardianPortalPages/ExamComparison.cshtml", results);
    }

    [HttpPost("Profile/Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile([FromForm] GuardianProfileUpdateDto dto, CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (userId == 0) return RedirectToAction("Login", "Auth");

        try
        {
            await _guardianService.UpdateGuardianProfileAsync(userId, dto, ct);
            TempData["SuccessMessage"] = "Profile updated successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Profile));
    }
}
