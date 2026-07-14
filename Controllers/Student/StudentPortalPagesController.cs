using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Models.Entities.Notification;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Student;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Student;

[Authorize(Roles = "Student")]
[Route("Student/Portal")]
public class StudentPortalPagesController : Controller
{
    private readonly IStudentPortalService _studentPortalService;
    private readonly ISchoolSettingRepository _settingRepo;
    private readonly IUnitOfWork _uow;
    private readonly IStudentExamResultRepository _studentExamResultRepository;
    private readonly ITranscriptService _transcriptService;
    private readonly ILogger<StudentPortalPagesController> _logger;
    private readonly IOnlinePaymentService _onlinePaymentService;

    public StudentPortalPagesController(
        IStudentPortalService studentPortalService,
        ISchoolSettingRepository settingRepo,
        IUnitOfWork uow,
        IStudentExamResultRepository studentExamResultRepository,
        ITranscriptService transcriptService,
        ILogger<StudentPortalPagesController> logger,
        IOnlinePaymentService onlinePaymentService)
    {
        _studentPortalService = studentPortalService;
        _settingRepo = settingRepo;
        _uow = uow;
        _studentExamResultRepository = studentExamResultRepository;
        _transcriptService = transcriptService;
        _logger = logger;
        _onlinePaymentService = onlinePaymentService;
    }

    private async Task<bool> IsStudentPortalEnabledAsync()
    {
        var settings = await _settingRepo.GetCurrentSettingsAsync();
        return settings?.EnableStudentPortal == true;
    }

    private int CurrentUserId()
    {
        var raw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(raw, out var id) ? id : 0;
    }

    private async Task<int> GetStudentIdAsync(CancellationToken ct)
    {
        var userId = CurrentUserId();
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);
        return student?.Id ?? 0;
    }

    [HttpGet("Attendance")]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> Attendance(DateTime? from, DateTime? to, CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var fromDate = (from ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)).Date;
        var toDate = (to ?? DateTime.Today).Date;

        var records = await _studentPortalService.GetAttendanceAsync(userId, fromDate, toDate, ct);
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking().FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);

        ViewBag.StudentName = student?.FullName;
        ViewBag.StudentId = student?.Id;
        ViewBag.From = fromDate.ToString("yyyy-MM-dd");
        ViewBag.To = toDate.ToString("yyyy-MM-dd");
        return View("~/Views/StudentPortal/Attendance.cshtml", records);
    }

    [HttpGet("Results")]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> Results(CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await GetStudentIdAsync(ct);
        if (sid == 0) { return View("~/Views/StudentPortal/Empty.cshtml", "No student profile linked to your account."); }

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

        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking().FirstOrDefaultAsync(s => s.Id == sid, ct);
        ViewBag.StudentId = sid;
        ViewBag.StudentName = student?.FullName;
        ViewBag.SubjectResults = subjects;
        return View("~/Views/StudentPortalPages/Results.cshtml", results);
    }

    [HttpGet("Fees")]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> Fees(CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await GetStudentIdAsync(ct);
        if (sid == 0) { return View("~/Views/StudentPortal/Empty.cshtml", "No student profile linked to your account."); }

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

        var onlineRequests = await _onlinePaymentService.GetByStudentAsync(sid, ct);

        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking().FirstOrDefaultAsync(s => s.Id == sid, ct);
        ViewBag.StudentId = sid;
        ViewBag.StudentName = student?.FullName;
        ViewBag.Payments = payments;
        ViewBag.TotalDue = invoices.Where(i => (int)i.Status != 3).Sum(i => i.TotalAmount - i.PaidAmount);
        ViewBag.TotalPaid = invoices.Sum(i => i.PaidAmount);
        ViewBag.OnlinePaymentRequests = onlineRequests;
        return View("~/Views/StudentPortal/Fees.cshtml", invoices);
    }

    [HttpGet("Leaves")]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> Leaves(CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await GetStudentIdAsync(ct);
        if (sid == 0) { return View("~/Views/StudentPortal/Empty.cshtml", "No student profile linked to your account."); }

        var leaves = await _uow.Repository<StudentLeaveApplication>().Query()
            .AsNoTracking()
            .Include(l => l.LeaveType)
            .Where(l => l.StudentId == sid)
            .OrderByDescending(l => l.Id)
            .ToListAsync(ct);

        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking().FirstOrDefaultAsync(s => s.Id == sid, ct);
        ViewBag.StudentId = sid;
        ViewBag.StudentName = student?.FullName;
        ViewBag.LeaveTypes = await _uow.Repository<LeaveType>().Query().AsNoTracking().Where(t => t.IsActive).ToListAsync(ct);
        return View("~/Views/StudentPortal/Leaves.cshtml", leaves);
    }

    [HttpPost("Leaves/Apply")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> ApplyLeave(int studentId, int leaveTypeId, DateTime fromDate, DateTime toDate, string reason, CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();

        if (toDate < fromDate)
        {
            TempData["ErrorMessage"] = "To date cannot be earlier than From date.";
            return RedirectToAction(nameof(Leaves));
        }

        var entity = new StudentLeaveApplication
        {
            StudentId = studentId,
            GuardianId = 0,
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
        return RedirectToAction(nameof(Leaves));
    }

    [HttpGet("Notices")]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> Notices(CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var notices = await _uow.Repository<Notice>().Query().AsNoTracking()
            .Where(n => !n.IsDeleted && n.IsPublished
                && (n.AudienceRole == "All" || n.AudienceRole == "Student" || n.AudienceRole == "Students"))
            .OrderByDescending(n => n.PublishAt)
            .Take(50)
            .ToListAsync(ct);
        return View("~/Views/StudentPortal/Notices.cshtml", notices);
    }

    [HttpGet("Calendar")]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> Calendar(CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var events = await _uow.Repository<AcademicCalendarEvent>().Query().AsNoTracking()
            .Include(e => e.AcademicCalendar)
            .Where(e => !e.IsDeleted && e.IsActive)
            .OrderBy(e => e.StartDate)
            .ToListAsync(ct);
        return View("~/Views/StudentPortal/Calendar.cshtml", events);
    }

    [HttpGet("Notifications")]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> Notifications(CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var notifications = await _studentPortalService.GetNotificationsAsync(userId, ct);
        return View("~/Views/StudentPortal/Notifications.cshtml", notifications);
    }

    [HttpPost("Notifications/MarkRead/{id:int}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> MarkRead(int id, CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        await _studentPortalService.MarkNotificationReadAsync(userId, id, ct);
        return RedirectToAction(nameof(Notifications));
    }

    [HttpGet("Profile")]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> Profile(CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);
        if (student == null) { return View("~/Views/StudentPortal/Empty.cshtml", "No student profile linked to this account."); }

        var model = new SchoolManagementSystem.Models.ViewModels.Student.StudentProfileViewModel
        {
            Student = student,
            ClassName = student.Class?.Name ?? "",
            SectionName = student.Section?.Name ?? ""
        };
        return View("~/Views/StudentPortal/Profile.cshtml", model);
    }

    [HttpGet("ReportCard")]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> ReportCard(int examId, CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await GetStudentIdAsync(ct);
        if (sid == 0) { return View("~/Views/StudentPortal/Empty.cshtml", "No student profile linked to your account."); }
        if (examId <= 0) return RedirectToAction(nameof(Results));

        var dto = await _studentExamResultRepository.GetReportCardAsync(examId, sid, ct);
        if (dto == null) return NotFound("Report card not available or not yet published.");

        return View("~/Views/StudentPortalPages/ReportCard.cshtml", dto);
    }

    [HttpGet("Transcript")]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> Transcript(int? academicYearId, CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await GetStudentIdAsync(ct);
        if (sid == 0) { return View("~/Views/StudentPortal/Empty.cshtml", "No student profile linked to your account."); }

        var activeYear = await _uow.Repository<AcademicYear>().Query().AsNoTracking().FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;
        if (yearId == 0) return RedirectToAction(nameof(Results));

        var transcript = await _transcriptService.GetStudentTranscriptAsync(sid, yearId);
        if (transcript == null) return NotFound("Transcript not found.");

        ViewBag.StudentId = sid;
        return View("~/Views/StudentPortalPages/Transcript.cshtml", transcript);
    }

    [HttpGet("ExamComparison")]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> ExamComparison(CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await GetStudentIdAsync(ct);
        if (sid == 0) { return View("~/Views/StudentPortal/Empty.cshtml", "No student profile linked to your account."); }

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

        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking().FirstOrDefaultAsync(s => s.Id == sid, ct);
        ViewBag.StudentId = sid;
        ViewBag.StudentName = student?.FullName;
        ViewBag.SubjectResults = subjects;
        return View("~/Views/StudentPortalPages/ExamComparison.cshtml", results);
    }

    [HttpPost("Profile/Update")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> UpdateProfile([FromForm] StudentProfileUpdateDto dto, CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        if (userId == 0) return RedirectToAction("Login", "Auth");

        try
        {
            await _studentPortalService.UpdateProfileAsync(userId, dto, ct);
            TempData["SuccessMessage"] = "Profile updated successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Profile));
    }

    [HttpGet("PayFee/{invoiceId:int}")]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> PayFee(int invoiceId, CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await GetStudentIdAsync(ct);
        if (sid == 0) { return View("~/Views/StudentPortal/Empty.cshtml", "No student profile linked to your account."); }

        var invoice = await _uow.Repository<FeeInvoice>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == invoiceId && i.StudentId == sid && !i.IsDeleted, ct);
        if (invoice == null) return NotFound();

        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking().FirstOrDefaultAsync(s => s.Id == sid, ct);
        ViewBag.StudentId = sid;
        ViewBag.StudentName = student?.FullName;
        return View("~/Views/StudentPortal/PayFee.cshtml", invoice);
    }

    [HttpPost("PayFee/{invoiceId:int}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> PayFee(int invoiceId, [FromForm] Models.DTOs.Fees.OnlinePaymentSubmitDto dto, CancellationToken ct)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await GetStudentIdAsync(ct);
        if (sid == 0) { return View("~/Views/StudentPortal/Empty.cshtml", "No student profile linked to your account."); }

        if (dto.Amount <= 0)
        {
            TempData["ErrorMessage"] = "Amount must be greater than zero.";
            return RedirectToAction(nameof(PayFee), new { invoiceId });
        }

        try
        {
            var request = await _onlinePaymentService.CreateAsync(sid, dto, User.Identity?.Name ?? "student", ct);
            TempData["SuccessMessage"] = $"Payment request submitted successfully. Reference: {request.ReferenceNo ?? request.Id.ToString()}";
            return RedirectToAction(nameof(Fees));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit online payment for invoice {InvoiceId}", invoiceId);
            TempData["ErrorMessage"] = "Failed to submit payment request. Please try again.";
            return RedirectToAction(nameof(PayFee), new { invoiceId });
        }
    }
}
