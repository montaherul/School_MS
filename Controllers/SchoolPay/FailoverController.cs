using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[Authorize]
[Route("SchoolPay/Failover")]
public class FailoverController : Controller
{
    private readonly IFailoverService _failoverService;

    public FailoverController(IFailoverService failoverService)
    {
        _failoverService = failoverService;
    }

    [RequirePermission("SchoolPay.ViewTransactions")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var statuses = await _failoverService.GetFailoverStatusAsync(ct);
        return View(statuses);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("SchoolPay.Manage")]
    public async Task<IActionResult> TestFailover(decimal amount, string? feeType, CancellationToken ct)
    {
        var providerId = await _failoverService.ResolveWithFailoverAsync(amount, feeType, ct);
        if (providerId.HasValue)
        {
            TempData["Success"] = $"Failover resolved to provider ID {providerId.Value}";
        }
        else
        {
            TempData["Warning"] = "No available provider found for failover";
        }
        return RedirectToAction(nameof(Index));
    }
}
