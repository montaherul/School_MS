using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[Authorize]
[Route("SchoolPay/Transaction")]
public class SchoolPayTransactionController : Controller
{
    private readonly ISchoolPayService _schoolPayService;
    private readonly IRefundService _refundService;
    private readonly ILogger<SchoolPayTransactionController> _logger;

    public SchoolPayTransactionController(
        ISchoolPayService schoolPayService,
        IRefundService refundService,
        ILogger<SchoolPayTransactionController> logger)
    {
        _schoolPayService = schoolPayService;
        _refundService = refundService;
        _logger = logger;
    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index(string? status, string? providerCode, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var transactions = await _schoolPayService.GetTransactionsPagedAsync(page, pageSize, status, providerCode, ct);
        var total = await _schoolPayService.GetTransactionCountAsync(status, providerCode, ct);

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
        ViewBag.StatusFilter = status;
        ViewBag.ProviderCode = providerCode;

        return View("~/Views/SchoolPay/Transaction/Index.cshtml", transactions);
    }

    [HttpGet("Refund/{transactionId:int}")]
    public async Task<IActionResult> Refund(int transactionId, decimal amount, string? reason, CancellationToken ct)
    {
        var user = User.Identity?.Name ?? "System";
        var result = await _refundService.ProcessRefundAsync(transactionId, amount, reason, user, ct);
        if (result == null)
        {
            TempData["ErrorMessage"] = "Refund failed. The transaction may not be eligible for refund.";
        }
        else
        {
            TempData["SuccessMessage"] = $"Refund #{result.RefundReference} processed successfully.";
        }
        return RedirectToAction(nameof(Index));
    }
}
