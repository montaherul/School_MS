using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Guardian;
using SchoolManagementSystem.Models.DTOs.Routine;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Assignment;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Models.Entities.Guardian;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using GuardianEntity = SchoolManagementSystem.Models.Entities.Guardian.Guardian;
using StudentEntity = SchoolManagementSystem.Models.Entities.Student.Student;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Assignment;
using SchoolManagementSystem.Services.Interfaces.Exam;
using SchoolManagementSystem.Services.Interfaces.Guardian;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Routine;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Guardian;

[Authorize(Roles = "Guardian")]
[Route("Guardian/Portal")]
public class GuardianPortalPagesController : Controller
{
    private readonly IGuardianService _guardianService;
    private readonly ISchoolSettingRepository _settingRepo;
    private readonly IUnitOfWork _uow;
    private readonly IStudentExamResultRepository _studentExamResultRepository;
    private readonly ITranscriptService _transcriptService;
    private readonly IAssignmentService _assignmentService;
    private readonly IAdmitCardService _admitCardService;
    private readonly IRoutineEntryService _routineEntryService;
    private readonly IRoutinePeriodService _routinePeriodService;
    private readonly ILogger<GuardianPortalPagesController> _logger;
    private readonly IOnlinePaymentService _onlinePaymentService;

    public GuardianPortalPagesController(
        IGuardianService guardianService,
        ISchoolSettingRepository settingRepo,
        IUnitOfWork uow,
        IStudentExamResultRepository studentExamResultRepository,
        ITranscriptService transcriptService,
        IAssignmentService assignmentService,
        IAdmitCardService admitCardService,
        IRoutineEntryService routineEntryService,
        IRoutinePeriodService routinePeriodService,
        ILogger<GuardianPortalPagesController> logger,
        IOnlinePaymentService onlinePaymentService)
    {
        _guardianService = guardianService;
        _settingRepo = settingRepo;
        _uow = uow;
        _studentExamResultRepository = studentExamResultRepository;
        _transcriptService = transcriptService;
        _assignmentService = assignmentService;
        _admitCardService = admitCardService;
        _routineEntryService = routineEntryService;
        _routinePeriodService = routinePeriodService;
        _logger = logger;
        _onlinePaymentService = onlinePaymentService;
    }

    private async Task<bool> IsGuardianPortalEnabledAsync()
    {
        var settings = await _settingRepo.GetCurrentSettingsAsync();
        return settings?.EnableGuardianPortal == true;
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
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> Attendance(int? studentId, DateTime? from, DateTime? to, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
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
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> Results(int? studentId, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
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
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> Fees(int? studentId, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
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

        var onlineRequests = await _onlinePaymentService.GetByStudentAsync(sid, ct);

        ViewBag.StudentId = sid;
        ViewBag.StudentName = (await _guardianService.GetChildDetailAsync(userId, sid, ct))?.FullName;
        ViewBag.Payments = payments;
        ViewBag.TotalDue = invoices.Where(i => (int)i.Status != 3).Sum(i => i.TotalAmount - i.PaidAmount);
        ViewBag.TotalPaid = invoices.Sum(i => i.PaidAmount);
        ViewBag.OnlinePaymentRequests = onlineRequests;
        return View("~/Views/GuardianPortal/Fees.cshtml", invoices);
    }

    [HttpGet("Leaves")]
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> Leaves(int? studentId, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
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
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> ApplyLeave(int studentId, int leaveTypeId, DateTime fromDate, DateTime toDate, string reason, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
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
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> Notices(CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var notices = await _uow.Repository<Notice>().Query().AsNoTracking()
            .Where(n => !n.IsDeleted && n.IsPublished
                && (n.AudienceRole == "All" || n.AudienceRole == "Guardian" || n.AudienceRole == "Guardians" || n.AudienceRole == "Parent" || n.AudienceRole == "Parents"))
            .OrderByDescending(n => n.PublishAt)
            .Take(50)
            .ToListAsync(ct);
        return View("~/Views/GuardianPortal/Notices.cshtml", notices);
    }

    [HttpGet("Calendar")]
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> Calendar(CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var events = await _uow.Repository<AcademicCalendarEvent>().Query().AsNoTracking()
            .Include(e => e.AcademicCalendar)
            .Where(e => !e.IsDeleted && e.IsActive)
            .OrderBy(e => e.StartDate)
            .ToListAsync(ct);
        return View("~/Views/GuardianPortal/Calendar.cshtml", events);
    }

    [HttpGet("Notifications")]
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> Notifications(CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
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
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> MarkRead(int id, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
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
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> Profile(CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
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
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> ReportCard(int? studentId, int examId, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
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
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> Transcript(int? studentId, int? academicYearId, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
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
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> ExamComparison(int? studentId, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
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
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> UpdateProfile([FromForm] GuardianProfileUpdateDto dto, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
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

    [HttpGet("Timetable")]
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> Timetable(int? studentId, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await ResolveOrFirstChildAsync(studentId, ct);
        if (sid == 0) { return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account."); }
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct)) return Forbid();

        var student = await _uow.Repository<StudentEntity>().Query()
            .AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.Section)
            .Include(s => s.StudentGroup)
            .FirstOrDefaultAsync(s => s.Id == sid, ct);
        if (student == null) return NotFound();

        var currentYear = await _uow.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct);
        var academicYearId = currentYear?.Id ?? 0;

        var entries = await _routineEntryService.GetGridAsync(
            academicYearId, student.ClassId, student.SectionId, student.StudentGroupId, null, null, 1, 500, ct);
        var periods = await _routinePeriodService.GetActivePeriodsAsync(ct);
        var dayNames = new[] { "sat", "sun", "mon", "tue", "wed", "thu", "fri" };

        var grid = periods.Select(p => new Dictionary<string, object?>
        {
            ["periodName"] = p.Name,
            ["sat"] = (string?)null,
            ["sun"] = (string?)null,
            ["mon"] = (string?)null,
            ["tue"] = (string?)null,
            ["wed"] = (string?)null,
            ["thu"] = (string?)null,
            ["fri"] = (string?)null
        }).ToList();

        foreach (var entry in entries.Items)
        {
            var row = grid.FirstOrDefault(r => (string?)r["periodName"] == entry.PeriodName);
            if (row != null && entry.DayNumber >= 1 && entry.DayNumber <= 7)
            {
                row[dayNames[entry.DayNumber - 1]] = $"{entry.SubjectName}<br><small>{entry.TeacherName}<br>{entry.RoomNo}</small>";
            }
        }

        var todayDayNumber = ((int)DateTime.Today.DayOfWeek + 1) % 7 + 1;
        var todayEntries = entries.Items.Where(e => e.DayNumber == todayDayNumber).ToList();

        var model = new RoutineStudentViewModel
        {
            ClassName = student.Class?.Name ?? string.Empty,
            SectionName = student.Section?.Name,
            GroupName = student.StudentGroup?.Name,
            WeeklyGrid = grid.Cast<object>().ToList(),
            Statistics = new List<StatisticItem>
            {
                new() { IconClass = "primary", Icon = "book", Value = entries.Items.Select(e => e.SubjectName).Distinct().Count(), Label = "Subjects" },
                new() { IconClass = "info", Icon = "clock", Value = entries.Items.Count, Label = "Total Periods" },
                new() { IconClass = "success", Icon = "calendar-day", Value = todayEntries.Count, Label = "Today's Classes" }
            },
            TodayClasses = todayEntries.Select(e => new TodayStudentClassDto
            {
                PeriodName = e.PeriodName,
                SubjectName = e.SubjectName,
                TeacherName = e.TeacherName,
                RoomNo = e.RoomNo,
                StartTime = periods.FirstOrDefault(p => p.Name == e.PeriodName)?.StartTime ?? string.Empty,
                EndTime = periods.FirstOrDefault(p => p.Name == e.PeriodName)?.EndTime ?? string.Empty
            }).ToList()
        };

        ViewBag.StudentId = sid;
        ViewBag.StudentName = (await _guardianService.GetChildDetailAsync(userId, sid, ct))?.FullName;
        return View("~/Views/GuardianPortalPages/Timetable.cshtml", model);
    }

    [HttpGet("Assignments")]
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> Assignments(int? studentId, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await ResolveOrFirstChildAsync(studentId, ct);
        if (sid == 0) { return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account."); }
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct)) return Forbid();

        var student = await _uow.Repository<StudentEntity>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sid, ct);
        if (student == null) return NotFound();

        var query = _assignmentService.Query()
            .AsNoTracking()
            .Where(a => a.SchoolClassId == student.ClassId && a.SectionId == student.SectionId && !a.IsDeleted)
            .OrderByDescending(a => a.Deadline);

        var assignments = await query.ToListAsync(ct);

        ViewBag.StudentId = sid;
        ViewBag.StudentName = (await _guardianService.GetChildDetailAsync(userId, sid, ct))?.FullName;
        return View("~/Views/GuardianPortalPages/Assignments.cshtml", assignments);
    }

    [HttpGet("AdmitCard")]
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> AdmitCard(int? studentId, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await ResolveOrFirstChildAsync(studentId, ct);
        if (sid == 0) { return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account."); }
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct)) return Forbid();

        var exams = await _uow.Repository<Models.Entities.Exam.Exam>().Query()
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.Id)
            .ToListAsync(ct);

        ViewBag.StudentId = sid;
        ViewBag.StudentName = (await _guardianService.GetChildDetailAsync(userId, sid, ct))?.FullName;
        return View("~/Views/GuardianPortalPages/AdmitCard.cshtml", exams);
    }

    [HttpGet("AdmitCard/View/{examId:int}")]
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> AdmitCardView(int examId, int? studentId, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await ResolveOrFirstChildAsync(studentId, ct);
        if (sid == 0) { return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account."); }
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct)) return Forbid();

        return RedirectToAction("View", "AdmitCard", new { examId, studentId = sid });
    }

    [HttpGet("PayFee/{invoiceId:int}")]
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> PayFee(int invoiceId, int? studentId, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await ResolveOrFirstChildAsync(studentId, ct);
        if (sid == 0) { return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account."); }
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct)) return Forbid();

        var invoice = await _uow.Repository<FeeInvoice>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == invoiceId && i.StudentId == sid && !i.IsDeleted, ct);
        if (invoice == null) return NotFound();

        ViewBag.StudentId = sid;
        ViewBag.StudentName = (await _guardianService.GetChildDetailAsync(userId, sid, ct))?.FullName;
        return View("~/Views/GuardianPortal/PayFee.cshtml", invoice);
    }

    [HttpPost("PayFee/{invoiceId:int}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Guardian.View")]
    public async Task<IActionResult> PayFee(int invoiceId, int? studentId, [FromForm] Models.DTOs.Fees.OnlinePaymentSubmitDto dto, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");
        var userId = CurrentUserId();
        var sid = await ResolveOrFirstChildAsync(studentId, ct);
        if (sid == 0) { return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account."); }
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct)) return Forbid();

        if (dto.Amount <= 0)
        {
            TempData["ErrorMessage"] = "Amount must be greater than zero.";
            return RedirectToAction(nameof(PayFee), new { invoiceId, studentId = sid });
        }

        try
        {
            var request = await _onlinePaymentService.CreateAsync(sid, dto, User.Identity?.Name ?? "guardian", ct);
            TempData["SuccessMessage"] = $"Payment request submitted successfully. Reference: {request.ReferenceNo ?? request.Id.ToString()}";
            return RedirectToAction(nameof(Fees), new { studentId = sid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit online payment for invoice {InvoiceId}", invoiceId);
            TempData["ErrorMessage"] = "Failed to submit payment request. Please try again.";
            return RedirectToAction(nameof(PayFee), new { invoiceId, studentId = sid });
        }
    }
}
