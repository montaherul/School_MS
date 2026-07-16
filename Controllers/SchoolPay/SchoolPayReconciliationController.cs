using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[Authorize]
[Route("SchoolPay/Reconciliation")]
public class SchoolPayReconciliationController : Controller
{
    private readonly IReconciliationService _reconciliationService;
    private readonly ISettlementService _settlementService;

    public SchoolPayReconciliationController(
        IReconciliationService reconciliationService,
        ISettlementService settlementService)
    {
        _reconciliationService = reconciliationService;
        _settlementService = settlementService;
    }

    [RequirePermission("SchoolPay.ViewSettlements")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var settlements = await _settlementService.GetSettlementsAsync(ct);
        return View(settlements);
    }

    [RequirePermission("SchoolPay.ViewSettlements")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var result = await _reconciliationService.GetReconciliationForSettlementAsync(id, ct);
        var data = result.FirstOrDefault();
        if (data == null) return NotFound();
        return View(data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("SchoolPay.Reconcile")]
    public async Task<IActionResult> Reconcile(int id, CancellationToken ct)
    {
        var result = await _reconciliationService.RunReconciliationAsync(id, ct);
        if (result == null)
        {
            TempData["Error"] = "Settlement not found";
        }
        else if (Math.Abs(result.Difference) < 0.01m)
        {
            TempData["Success"] = $"Settlement {result.SettlementReference} reconciled successfully. Difference: {result.Difference:C}";
        }
        else
        {
            TempData["Warning"] = $"Settlement {result.SettlementReference} has a difference of {result.Difference:C}. Marked as disputed.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("SchoolPay.Reconcile")]
    public async Task<IActionResult> BulkReconcile(CancellationToken ct)
    {
        await _reconciliationService.RunBulkReconciliationAsync(ct);
        TempData["Success"] = "Bulk reconciliation completed";
        return RedirectToAction(nameof(Index));
    }
}
