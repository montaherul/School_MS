using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize(Roles = "Admin,SuperAdmin,Accountant")]
[Route("Fees/OnlinePaymentVerification")]
public class OnlinePaymentVerificationController : Controller
{
    private readonly IOnlinePaymentService _onlinePaymentService;

    public OnlinePaymentVerificationController(
        IOnlinePaymentService onlinePaymentService)
    {
        _onlinePaymentService = onlinePaymentService;
    }

    [HttpGet]
    [RequirePermission("OnlinePayments.View")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var requests = await _onlinePaymentService.GetPendingAsync(ct);
        return View("~/Views/Fees/OnlinePaymentVerification/Index.cshtml", requests);
    }

    [HttpPost("Verify/{id:int}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("OnlinePayments.View")]
    public async Task<IActionResult> Verify(int id, string? adminNotes, CancellationToken ct)
    {
        var success = await _onlinePaymentService.VerifyAsync(id, User.Identity?.Name ?? "system", adminNotes, ct);
        if (!success)
        {
            TempData["ErrorMessage"] = "Payment request not found or already processed.";
        }
        else
        {
            TempData["SuccessMessage"] = "Payment request verified successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Reject/{id:int}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("OnlinePayments.View")]
    public async Task<IActionResult> Reject(int id, string? adminNotes, CancellationToken ct)
    {
        var success = await _onlinePaymentService.RejectAsync(id, User.Identity?.Name ?? "system", adminNotes, ct);
        if (!success)
        {
            TempData["ErrorMessage"] = "Payment request not found or already processed.";
        }
        else
        {
            TempData["ErrorMessage"] = "Payment request rejected.";
        }
        return RedirectToAction(nameof(Index));
    }
}
