using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
[RequirePermission("FeeSettings.Read")]
public class FeeSettingsController : Controller
{
    private readonly ILateFeeRuleService _lateFeeService;
    private readonly IFeeInvoiceService _invoiceService;

    public FeeSettingsController(ILateFeeRuleService lateFeeService, IFeeInvoiceService invoiceService)
    {
        _lateFeeService = lateFeeService;
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var rules = await _lateFeeService.GetPagedAsync(1, 100, null, ct);
        ViewBag.LateFeeRules = rules.Items;
        return View("~/Views/Fee/FeeSettings/Index.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLateFee(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? "System";
        var isActive = await _lateFeeService.ToggleActiveAsync(id, userId, ct);
        return Json(new { success = true, isActive });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyLateFees(CancellationToken ct)
    {
        var result = await _invoiceService.ApplyLateFeesAsync(ct);
        TempData["SuccessMessage"] = $"Late fees applied: {result.InvoicesProcessed} invoice(s) processed, total ৳{result.TotalLateFeeApplied:N2}.";
        if (result.Errors.Count > 0)
            TempData["ErrorMessage"] = string.Join(" | ", result.Errors);
        return RedirectToAction(nameof(Index));
    }
}
