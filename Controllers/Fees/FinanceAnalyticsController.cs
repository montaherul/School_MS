using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
[RequirePermission("FinanceReports.Read")]
public class FinanceAnalyticsController : Controller
{
    private const string ViewPath = "~/Views/Fees/FinanceAnalytics";
    private readonly IFinanceAnalyticsService _service;

    public FinanceAnalyticsController(IFinanceAnalyticsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var data = await _service.GetDashboardAsync(ct);
        return View($"{ViewPath}/Index.cshtml", data);
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardData(CancellationToken ct = default)
    {
        var data = await _service.GetDashboardAsync(ct);
        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetMonthlySummaries(int months = 12, CancellationToken ct = default)
    {
        var data = await _service.GetMonthlySummariesAsync(months, ct);
        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetDefaulterSegments(CancellationToken ct = default)
    {
        var data = await _service.GetDefaulterSegmentsAsync(ct);
        return Json(data);
    }
}
