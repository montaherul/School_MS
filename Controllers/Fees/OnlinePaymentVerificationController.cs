using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Enums;
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
    public IActionResult Index()
    {
        return View("~/Views/Fees/OnlinePaymentVerification/Index.cshtml");
    }

    [HttpGet("All")]
    [RequirePermission("OnlinePayments.View")]
    public IActionResult AllTransactions()
    {
        return View("~/Views/Fees/OnlinePaymentVerification/AllTransactions.cshtml");
    }

    [HttpGet("List")]
    [RequirePermission("OnlinePayments.View")]
    public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null, int? statusFilter = null)
    {
        var result = await _onlinePaymentService.GetPagedAsync(page, pageSize, search, statusFilter);
        return Json(new
        {
            data = result.Items,
            last_page = Math.Ceiling((double)result.TotalItems / result.PageSize)
        });
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
