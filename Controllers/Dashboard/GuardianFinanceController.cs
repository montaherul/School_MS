using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Guardian;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Dashboard;

[Authorize(Roles = "Guardian")]
[Route("GuardianFinance")]
public class GuardianFinanceController : Controller
{
    private readonly IStudentFinanceService _financeService;
    private readonly IFeeReceiptService _receiptService;
    private readonly IGuardianService _guardianService;
    private readonly ISchoolSettingRepository _settingRepo;

    public GuardianFinanceController(
        IStudentFinanceService financeService,
        IFeeReceiptService receiptService,
        IGuardianService guardianService,
        ISchoolSettingRepository settingRepo)
    {
        _financeService = financeService;
        _receiptService = receiptService;
        _guardianService = guardianService;
        _settingRepo = settingRepo;
    }

    private async Task<bool> IsGuardianPortalEnabledAsync()
    {
        var settings = await _settingRepo.GetCurrentSettingsAsync();
        return settings?.EnableGuardianPortal == true;
    }

    private int? GetCurrentUserId()
    {
        if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return userId;
        return null;
    }

    private async Task<bool> CanAccessStudentAsync(int studentId)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return false;
        return await _guardianService.UserHasAccessToStudentAsync(userId.Value, studentId);
    }

    [HttpGet("GetInvoices")]
    public async Task<IActionResult> GetInvoices(int studentId, int page = 1, int size = 10, string? search = null, int? status = null)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return Json(new { data = Array.Empty<object>(), total = 0, page, last_page = 0 });
        if (!await CanAccessStudentAsync(studentId))
            return Json(new { data = Array.Empty<object>(), total = 0, page, last_page = 0 });

        var result = await _financeService.GetInvoicesPagedAsync(studentId, page, size, search, status, HttpContext.RequestAborted);
        return Json(new { data = result.Items, total = result.TotalItems, page = result.Page, last_page = result.TotalPages });
    }

    [HttpGet("GetPayments")]
    public async Task<IActionResult> GetPayments(int studentId, int page = 1, int size = 10, string? search = null)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return Json(new { data = Array.Empty<object>(), total = 0, page, last_page = 0 });
        if (!await CanAccessStudentAsync(studentId))
            return Json(new { data = Array.Empty<object>(), total = 0, page, last_page = 0 });

        var result = await _financeService.GetPaymentsPagedAsync(studentId, page, size, search, HttpContext.RequestAborted);
        return Json(new { data = result.Items, total = result.TotalItems, page = result.Page, last_page = result.TotalPages });
    }

    [HttpGet("GetLedger")]
    public async Task<IActionResult> GetLedger(int studentId, int page = 1, int size = 10, string? search = null)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return Json(new { data = Array.Empty<object>(), total = 0, page, last_page = 0 });
        if (!await CanAccessStudentAsync(studentId))
            return Json(new { data = Array.Empty<object>(), total = 0, page, last_page = 0 });

        var result = await _financeService.GetLedgerPagedAsync(studentId, page, size, search, HttpContext.RequestAborted);
        return Json(new { data = result.Items, total = result.TotalItems, page = result.Page, last_page = result.TotalPages });
    }

    [HttpGet("GetFinanceSummary")]
    public async Task<IActionResult> GetFinanceSummary(int studentId)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return Json(new { totalInvoiced = 0m, totalPaid = 0m, totalDue = 0m, pendingInvoices = 0, lastPayment = (object?)null });
        if (!await CanAccessStudentAsync(studentId))
            return Json(new { totalInvoiced = 0m, totalPaid = 0m, totalDue = 0m, pendingInvoices = 0, lastPayment = (object?)null });

        var summary = await _financeService.GetFinanceSummaryAsync(studentId, HttpContext.RequestAborted);
        var lastPayment = await _financeService.GetLastPaymentAsync(studentId, HttpContext.RequestAborted);

        return Json(new
        {
            totalInvoiced = summary.TotalInvoiced,
            totalPaid = summary.TotalPaid,
            totalDue = summary.TotalDue,
            lastPayment = lastPayment is null ? null : new
            {
                lastPayment.Id,
                lastPayment.PaymentDate,
                lastPayment.Amount
            }
        });
    }

    [HttpGet("DownloadReceipt/{paymentId}")]
    public async Task<IActionResult> DownloadReceipt(int paymentId, int studentId)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return Forbid();
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Forbid();

        if (!await _guardianService.UserHasAccessToStudentAsync(userId, studentId))
            return Forbid();

        var receiptData = await _receiptService.GetReceiptDataAsync(paymentId, HttpContext.RequestAborted);
        if (receiptData is null || receiptData.StudentId != studentId)
            return Forbid();

        var pdf = await _receiptService.GenerateReceiptPdfAsync(paymentId, HttpContext.RequestAborted);
        if (pdf.Length == 0)
            return NotFound();

        return File(pdf, "application/pdf", $"Receipt_{paymentId}.pdf");
    }
}
