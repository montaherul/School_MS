using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[Authorize]
[Route("SchoolPay/Monitoring")]
public class MonitoringController : Controller
{
    private readonly IMonitoringService _monitoringService;

    public MonitoringController(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    [RequirePermission("SchoolPay.ViewTransactions")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var data = await _monitoringService.GetMonitoringDataAsync(ct);
        return View(data);
    }
}
