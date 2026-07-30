using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[Authorize]
[Route("SchoolPay/Transaction")]
public class SchoolPayTransactionController : Controller
{
    private readonly ISchoolPayService _schoolPayService;
    private readonly ILogger<SchoolPayTransactionController> _logger;

    public SchoolPayTransactionController(
        ISchoolPayService schoolPayService,
        ILogger<SchoolPayTransactionController> logger)
    {
        _schoolPayService = schoolPayService;
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
}
