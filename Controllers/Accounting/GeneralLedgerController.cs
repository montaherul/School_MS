using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Accounting;

namespace SchoolManagementSystem.Controllers.Accounting;

[Authorize]
[Route("Accounting/[controller]")]
[RequirePermission("Accounting.View")]
public class GeneralLedgerController : Controller
{
    private readonly ILedgerService _service;
    private readonly IChartOfAccountService _accountService;
    private readonly IFinancialPeriodService _periodService;

    public GeneralLedgerController(ILedgerService service, IChartOfAccountService accountService,
        IFinancialPeriodService periodService)
    {
        _service = service;
        _accountService = accountService;
        _periodService = periodService;
    }

    [HttpGet("")]
    public IActionResult Index() => View("~/Views/Accounting/GeneralLedger/Index.cshtml");

    [HttpGet("GetList")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, int? accountId = null, DateTime? from = null, DateTime? to = null, int? periodId = null)
    {
        var result = await _service.GetGeneralLedgerAsync(accountId, from, to, periodId, page, size);
        return Json(new { data = result.Items, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet("FilterOptions")]
    public async Task<IActionResult> FilterOptions()
    {
        var accounts = await _accountService.GetPagedAsync(1, int.MaxValue, null, null);
        var periods = await _periodService.GetPagedAsync(1, int.MaxValue, null);

        return Json(new
        {
            accounts = accounts.Items.Select(a => new { a.Id, a.AccountCode, a.AccountName }),
            periods = periods.Items.Select(p => new { p.Id, p.Name })
        });
    }
}
