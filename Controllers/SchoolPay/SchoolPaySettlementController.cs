using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[Authorize]
[Route("SchoolPay/Settlement")]
public class SchoolPaySettlementController : Controller
{
    private readonly ISettlementService _settlementService;
    private readonly ILogger<SchoolPaySettlementController> _logger;

    public SchoolPaySettlementController(ISettlementService settlementService, ILogger<SchoolPaySettlementController> logger)
    {
        _settlementService = settlementService;
        _logger = logger;
    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var settlements = await _settlementService.GetSettlementsAsync(ct);
        return View("~/Views/SchoolPay/Settlement/Index.cshtml", settlements);
    }

    [HttpPost("MarkSettled/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkSettled(int id, string? providerSettlementId, CancellationToken ct)
    {
        var success = await _settlementService.MarkAsSettledAsync(id, providerSettlementId, ct);
        return Json(new { success });
    }
}
