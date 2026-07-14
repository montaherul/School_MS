using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Accounting;

namespace SchoolManagementSystem.Controllers.Accounting;

[Authorize]
[Route("Accounting/[controller]")]
[RequirePermission("Accounting.View")]
public class FinancialStatementController : Controller
{
    private readonly ILedgerService _service;
    private readonly IFinancialPeriodService _periodService;

    public FinancialStatementController(ILedgerService service, IFinancialPeriodService periodService)
    {
        _service = service;
        _periodService = periodService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        ViewBag.Periods = await _periodService.GetPeriodSelectListAsync(activeOnly: false);
        return View("~/Views/Accounting/FinancialStatement/Index.cshtml");
    }

    [HttpGet("IncomeStatement")]
    public async Task<IActionResult> IncomeStatement(DateTime? from, DateTime? to, int? periodId)
    {
        var f = from ?? new DateTime(DateTime.Today.Year, 1, 1);
        var t = to ?? DateTime.Today;
        var result = await _service.GetIncomeStatementAsync(f, t, periodId);
        ViewBag.FromDate = f;
        ViewBag.ToDate = t;
        return View("~/Views/Accounting/FinancialStatement/IncomeStatement.cshtml", result);
    }

    [HttpGet("BalanceSheet")]
    public async Task<IActionResult> BalanceSheet(DateTime? asOfDate, int? periodId)
    {
        var result = await _service.GetBalanceSheetAsync(asOfDate ?? DateTime.Today, periodId);
        ViewBag.AsOfDate = asOfDate ?? DateTime.Today;
        return View("~/Views/Accounting/FinancialStatement/BalanceSheet.cshtml", result);
    }

    [HttpGet("MonthlySummary")]
    public async Task<IActionResult> MonthlySummary(int year, int? periodId)
    {
        if (year == 0) year = DateTime.Today.Year;
        var result = await _service.GetMonthlyIncomeSummaryAsync(year, periodId);
        ViewBag.Year = year;
        return View("~/Views/Accounting/FinancialStatement/MonthlySummary.cshtml", result);
    }
}
