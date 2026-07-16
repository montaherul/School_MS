using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[Authorize]
[Route("SchoolPay/Analytics")]
public class SchoolPayAnalyticsController : Controller
{
    private readonly IAnalyticsService _analyticsService;

    public SchoolPayAnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [RequirePermission("SchoolPay.ViewTransactions")]
    public async Task<IActionResult> Index(int days = 30, CancellationToken ct = default)
    {
        ViewBag.Days = days;
        var data = await _analyticsService.GetAnalyticsAsync(days, ct);
        return View(data);
    }
}
