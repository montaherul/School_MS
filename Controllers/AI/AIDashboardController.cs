using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.ViewModels.AI;
using SchoolManagementSystem.Services.Interfaces.AI;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.AI;

[Authorize(Roles = "Admin,Principal,Accountant")]
[Route("AI/[controller]")]
public class AIDashboardController : Controller
{
    private readonly IAIAdminService _adminService;
    private readonly ILogger<AIDashboardController> _logger;

    public AIDashboardController(IAIAdminService adminService, ILogger<AIDashboardController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

    [HttpGet("")]
    [RequirePermission("AI.View")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var statsResult = await _adminService.GetDashboardStatsAsync(ct);
        if (statsResult.IsFailure)
        {
            TempData["ErrorMessage"] = statsResult.ErrorMessage;
            return View("~/Views/AI/Dashboard/Index.cshtml", new AIDashboardViewModel());
        }

        var chartsResult = await _adminService.GetDashboardChartsAsync(ct);
        if (chartsResult.IsFailure)
        {
            TempData["ErrorMessage"] = chartsResult.ErrorMessage;
            return View("~/Views/AI/Dashboard/Index.cshtml", new AIDashboardViewModel { Stats = statsResult.Data! });
        }

        var (requestsPerHour, dailyCost, topSubjects) = chartsResult.Data!;
        return View("~/Views/AI/Dashboard/Index.cshtml", new AIDashboardViewModel
        {
            Stats = statsResult.Data!,
            RequestsPerHour = requestsPerHour,
            DailyCost = dailyCost,
            TopSubjects = topSubjects
        });
    }

    [HttpGet("GetChartData")]
    [RequirePermission("AI.View")]
    public async Task<IActionResult> GetChartData(CancellationToken ct)
    {
        var result = await _adminService.GetDashboardChartsAsync(ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });

        var (requestsPerHour, dailyCost, topSubjects) = result.Data!;
        return Json(new { success = true, requestsPerHour, dailyCost, topSubjects });
    }
}
