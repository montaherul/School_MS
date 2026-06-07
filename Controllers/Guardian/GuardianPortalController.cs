using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.ViewModels.Dashboard;
using SchoolManagementSystem.Repositories.Guardian;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using SchoolManagementSystem.Services.Guardian;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Guardian;

[Authorize(Roles = "Guardian")]
[Route("Guardian/Portal")]
public class GuardianPortalController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly IGuardianService _guardianService;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<GuardianPortalController> _logger;

    public GuardianPortalController(
        IDashboardService dashboardService,
        IGuardianService guardianService,
        IUnitOfWork uow,
        ILogger<GuardianPortalController> logger)
    {
        _dashboardService = dashboardService;
        _guardianService = guardianService;
        _uow = uow;
        _logger = logger;
    }

    private int CurrentUserId()
    {
        var raw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(raw, out var id) ? id : 0;
    }

    [HttpGet("Dashboard")]
    [HttpGet("Dashboard/{studentId:int?}")]
    public async Task<IActionResult> Dashboard(int? studentId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = CurrentUserId();
            if (userId == 0) return RedirectToAction("Login", "Auth");

            var model = await _dashboardService.GetGuardianDashboardAsync(userId, cancellationToken);

            // If a specific child was selected, refresh that child's data
            if (studentId.HasValue && studentId.Value > 0)
            {
                if (!await _guardianService.UserHasAccessToStudentAsync(userId, studentId.Value, cancellationToken))
                    return Forbid();
                await HydrateChildDetailAsync(model, userId, studentId.Value, cancellationToken);
                model.SelectedChildId = studentId.Value;
            }

            // Always populate child switcher list
            model.ChildSwitcher = await _guardianService.GetChildrenByUserIdAsync(userId, cancellationToken);

            return View("~/Views/Dashboard/GuardianIndex.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading guardian dashboard");
            return View("Error");
        }
    }
    [HttpGet("SwitchChild/{studentId:int}")]
    public async Task<IActionResult> SwitchChild(int studentId, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId();
        if (userId == 0) return RedirectToAction("Login", "Auth");
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, studentId, cancellationToken))
            return Forbid();
        return RedirectToAction(nameof(Dashboard), new { studentId });
    }

    private async Task HydrateChildDetailAsync(GuardianDashboardViewModel model, int userId, int studentId, CancellationToken ct)
    {
        var detail = await _guardianService.GetChildDetailAsync(userId, studentId, ct);
        if (detail == null) return;

        model.StudentName = detail.FullName;
        model.ClassName = detail.ClassName;
        model.SectionName = detail.SectionName;
        model.RollNumber = detail.RollNumber.ToString();
        model.AttendancePercentage = detail.AttendancePercentage;
        model.PresentCount = detail.PresentCount;
        model.AbsentCount = detail.AbsentCount;
        model.LateCount = detail.LateCount;
        model.LeaveCount = detail.LeaveCount;

        // Refresh attendance history
        var from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var to = from.AddMonths(1).AddDays(-1);
        var records = await _guardianService.GetChildAttendanceAsync(userId, studentId, from, to, ct);
        model.AttendanceHistory = records.Take(15).ToList();
        model.AttendanceCalendar = records.Select(a => new AttendanceCalendarDto
        {
            Date = a.AttendanceDate,
            Status = a.StatusName,
            StatusColor = a.StatusName switch
            {
                "Present" => "#22c55e",
                "Absent" => "#ef4444",
                "Late" => "#eab308",
                "Leave" => "#8b5cf6",
                _ => "#94a3b8"
            }
        }).ToList();

        model.Alerts ??= new List<string>();
        if (detail.AttendancePercentage > 0 && detail.AttendancePercentage < 75.0)
        {
            model.Alerts.Add($"Low Attendance Alert: {detail.FullName}'s attendance is {detail.AttendancePercentage}%, below the 75% minimum.");
        }
        if (detail.OutstandingFees > 0)
        {
            model.Alerts.Add($"Outstanding Fees: {detail.FullName} has an outstanding balance of ৳{detail.OutstandingFees:N2}.");
        }
    }
}
