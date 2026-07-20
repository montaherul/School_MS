using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.ViewModels.Dashboard;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Notification;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Student;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Services.Interfaces.AI;

namespace SchoolManagementSystem.Controllers.Student;

[Authorize(Roles = "Student")]
[Route("Student/Portal")]
public class StudentPortalController : Controller
{
    private readonly IStudentPortalService _studentPortalService;
    private readonly ISchoolSettingRepository _settingRepo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<StudentPortalController> _logger;
    private readonly IAIFeatureService _aiFeature;

    public StudentPortalController(
        IStudentPortalService studentPortalService,
        ISchoolSettingRepository settingRepo,
        IUnitOfWork uow,
        ILogger<StudentPortalController> logger,
        IAIFeatureService aiFeature)
    {
        _studentPortalService = studentPortalService;
        _settingRepo = settingRepo;
        _uow = uow;
        _logger = logger;
        _aiFeature = aiFeature;
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

    [HttpGet("Dashboard")]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        if (!await IsStudentPortalEnabledAsync())
            return RedirectToAction("Index", "Dashboard");

        try
        {
            var userId = CurrentUserId();
            if (userId == 0) return RedirectToAction("Login", "Auth");

            ViewBag.AIChatEnabled = await _aiFeature.IsFeatureEnabledAsync("AI.Feature.Chat");
            var dto = await _studentPortalService.GetDashboardAsync(userId, cancellationToken);
            var model = await HydrateDashboardViewModelAsync(userId, cancellationToken);
            model.StudentName = dto.StudentName;
            model.ClassName = dto.ClassName;
            model.SectionName = dto.SectionName;
            model.RollNumber = dto.RollNumber.ToString();
            model.ProfilePicturePath = dto.ProfilePicturePath;
            model.PresentCount = dto.PresentCount;
            model.AbsentCount = dto.AbsentCount;
            model.LateCount = dto.LateCount;
            model.LeaveCount = dto.LeaveCount;
            model.AttendancePercentage = dto.AttendancePercentage;
            model.OutstandingFees = dto.OutstandingFees;
            model.TotalPaid = dto.TotalPaid;
            model.InvoiceCount = dto.InvoiceCount;
            model.TotalInvoiced = dto.TotalInvoiced;
            model.TotalDue = dto.TotalDue;
            model.LatestGPA = dto.LatestGPA;
            model.LatestGrade = dto.LatestGrade;
            model.LatestPassed = dto.LatestPassed;
            model.IsResultBlocked = dto.IsResultBlocked;
            model.PendingLeaveCount = dto.PendingLeaveCount;
            model.LeaveApplicationCount = dto.LeaveApplicationCount;
            model.UnreadNotificationCount = dto.UnreadNotificationCount;

            return View("~/Views/Dashboard/StudentIndex.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading student dashboard");
            return View("Error");
        }
    }

    private async Task<StudentDashboardViewModel> HydrateDashboardViewModelAsync(int userId, CancellationToken ct)
    {
        var model = new StudentDashboardViewModel();
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .Include(s => s.Class)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);

        if (student == null) return model;

        model.StudentId = student.Id;

        // Attendance history
        var from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var to = from.AddMonths(1).AddDays(-1);
        var records = await _uow.Repository<StudentAttendance>().Query()
            .AsNoTracking()
            .Include(a => a.Class)
            .Include(a => a.Section)
            .Where(a => a.StudentId == student.Id && a.AttendanceDate >= from && a.AttendanceDate <= to)
            .OrderByDescending(a => a.AttendanceDate)
            .Take(15)
            .ToListAsync(ct);

        model.AttendanceHistory = records.Select(a => new StudentAttendanceDto
        {
            AttendanceDate = a.AttendanceDate,
            StatusName = a.Status.ToString(),
            ClassName = a.Class?.Name ?? "",
            SectionName = a.Section?.Name ?? "",
            Remarks = a.Remarks
        }).ToList();

        model.AttendanceCalendar = records.Select(a => new AttendanceCalendarDto
        {
            Date = a.AttendanceDate,
            Status = a.Status.ToString(),
            StatusColor = a.Status switch
            {
                AttendanceStatus.Present => "#22c55e",
                AttendanceStatus.Absent => "#ef4444",
                AttendanceStatus.Late => "#eab308",
                AttendanceStatus.Leave => "#8b5cf6",
                _ => "#94a3b8"
            }
        }).ToList();

        // Alerts
        if (model.AttendancePercentage > 0 && model.AttendancePercentage < 75.0)
            model.Alerts.Add($"Low Attendance Alert: Your attendance is {model.AttendancePercentage:F1}%, below the 75% minimum.");

        if (model.OutstandingFees > 0)
            model.Alerts.Add($"Outstanding Fees: You have an outstanding balance of ৳{model.OutstandingFees:N2}.");

        // Recent notices
        model.RecentNotices = await _uow.Repository<Notice>().Query()
            .AsNoTracking()
            .Where(n => !n.IsDeleted && n.IsPublished
                && (n.AudienceRole == "All" || n.AudienceRole == "Student" || n.AudienceRole == "Students"))
            .OrderByDescending(n => n.PublishAt)
            .Take(5)
            .Select(n => new StudentNoticeDto
            {
                Id = n.Id,
                Title = n.Title,
                Date = n.PublishAt,
                Excerpt = n.Body.Length > 100 ? n.Body.Substring(0, 100) + "..." : n.Body,
                Category = n.AudienceRole ?? ""
            })
            .ToListAsync(ct);

        return model;
    }
}
