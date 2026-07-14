using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Accounting;

[Authorize]
[Route("Accounting/[controller]")]
public class BankBookController : Controller
{
    private readonly IBankService _service;
    private readonly IChartOfAccountService _accountService;
    private readonly IFinancialPeriodService _periodService;

    public BankBookController(IBankService service, IChartOfAccountService accountService,
        IFinancialPeriodService periodService)
    {
        _service = service;
        _accountService = accountService;
        _periodService = periodService;
    }

    [HttpGet("")]
    [RequirePermission("Accounting.View")]
    public IActionResult Index() => View("~/Views/Accounting/BankBook/Index.cshtml");

    [HttpGet("GetList")]
    [RequirePermission("Accounting.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, int? accountId = null, int? bankType = null, DateTime? from = null, DateTime? to = null, int? periodId = null)
    {
        var result = await _service.GetBankBookAsync(accountId, bankType, from, to, periodId, page, size);
        return Json(new { data = result.Items, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet("GetSummary")]
    [RequirePermission("Accounting.View")]
    public async Task<IActionResult> GetSummary(int? accountId, int? bankType, DateTime? from, DateTime? to, int? periodId)
    {
        var summary = await _service.GetSummaryAsync(accountId, bankType, from, to, periodId);
        return Json(summary);
    }

    [HttpGet("Create")]
    [RequirePermission("Accounting.Post")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Accounts = await _accountService.GetActiveAccountSelectListAsync();
        ViewBag.BankTypes = Enum.GetValues<BankAccountType>()
            .Select(e => new SelectListItem { Value = ((int)e).ToString(), Text = e.ToString() });
        ViewBag.TxTypes = Enum.GetValues<BankTransactionType>()
            .Select(e => new SelectListItem { Value = ((int)e).ToString(), Text = e.ToString() });
        return View("~/Views/Accounting/BankBook/Create.cshtml", new BankTransactionDto { TransactionDate = DateTime.Today });
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Accounting.Post")]
    public async Task<IActionResult> Create(BankTransactionDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Accounts = await _accountService.GetActiveAccountSelectListAsync();
            ViewBag.BankTypes = Enum.GetValues<BankAccountType>()
                .Select(e => new SelectListItem { Value = ((int)e).ToString(), Text = e.ToString() });
            ViewBag.TxTypes = Enum.GetValues<BankTransactionType>()
                .Select(e => new SelectListItem { Value = ((int)e).ToString(), Text = e.ToString() });
            return View("~/Views/Accounting/BankBook/Create.cshtml", dto);
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.CreateTransactionAsync(dto, userId);
        TempData["SuccessMessage"] = "Bank transaction recorded.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Reconciliation")]
    [RequirePermission("Accounting.Reconcile")]
    public async Task<IActionResult> Reconciliation(int? accountId)
    {
        ViewBag.Accounts = await _accountService.GetActiveAccountSelectListAsync();
        var items = await _service.GetUnreconciledAsync(accountId);
        return View("~/Views/Accounting/BankBook/Reconciliation.cshtml", items);
    }

    [HttpPost("Reconcile")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Accounting.Reconcile")]
    public async Task<IActionResult> Reconcile(string transactionIds)
    {
        if (string.IsNullOrEmpty(transactionIds))
        {
            TempData["ErrorMessage"] = "No transactions selected.";
            return RedirectToAction(nameof(Reconciliation));
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.ReconcileAsync(transactionIds, userId);
        TempData["SuccessMessage"] = "Transactions reconciled.";
        return RedirectToAction(nameof(Reconciliation));
    }
}
