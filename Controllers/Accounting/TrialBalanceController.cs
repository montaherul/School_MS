using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Accounting;

namespace SchoolManagementSystem.Controllers.Accounting;

[Authorize]
[Route("Accounting/[controller]")]
[RequirePermission("Accounting.View")]
public class TrialBalanceController : Controller
{
    private readonly ILedgerService _service;
    private readonly IFinancialPeriodService _periodService;

    public TrialBalanceController(ILedgerService service, IFinancialPeriodService periodService)
    {
        _service = service;
        _periodService = periodService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(DateTime? asOfDate, int? periodId)
    {
        ViewBag.Periods = await _periodService.GetPeriodSelectListAsync(activeOnly: false);
        var result = await _service.GetTrialBalanceAsync(asOfDate ?? DateTime.Today, periodId);
        ViewBag.AsOfDate = asOfDate ?? DateTime.Today;
        return View("~/Views/Accounting/TrialBalance/Index.cshtml", result);
    }
}
