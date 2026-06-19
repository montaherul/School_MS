using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Controllers.Dashboard;

[Authorize]
[Route("StudentFinance")]
public class StudentFinanceController : Controller
{
    private readonly IStudentFinanceService _financeService;
    private readonly IFeeReceiptService _receiptService;
    private readonly IFeeSecurityService _security;

    public StudentFinanceController(
        IStudentFinanceService financeService,
        IFeeReceiptService receiptService,
        IFeeSecurityService security)
    {
        _financeService = financeService;
        _receiptService = receiptService;
        _security = security;
    }

    [HttpGet("GetInvoices")]
    public async Task<IActionResult> GetInvoices(int page = 1, int size = 10, string? search = null, int? status = null)
    {
        var studentId = _security.GetCurrentStudentId(User);
        if (!_security.HasStudentRole(User) || studentId is null)
            return Json(new { data = Array.Empty<object>(), total = 0, page, lastPage = 0 });

        var result = await _financeService.GetInvoicesPagedAsync(studentId.Value, page, size, search, status, HttpContext.RequestAborted);
        return Json(new
        {
            data = result.Items,
            total = result.TotalItems,
            page = result.Page,
            last_page = result.TotalPages
        });
    }

    [HttpGet("GetPayments")]
    public async Task<IActionResult> GetPayments(int page = 1, int size = 10, string? search = null)
    {
        var studentId = _security.GetCurrentStudentId(User);
        if (!_security.HasStudentRole(User) || studentId is null)
            return Json(new { data = Array.Empty<object>(), total = 0, page, last_page = 0 });

        var result = await _financeService.GetPaymentsPagedAsync(studentId.Value, page, size, search, HttpContext.RequestAborted);
        return Json(new
        {
            data = result.Items,
            total = result.TotalItems,
            page = result.Page,
            last_page = result.TotalPages
        });
    }

    [HttpGet("GetLedger")]
    public async Task<IActionResult> GetLedger(int page = 1, int size = 10, string? search = null)
    {
        var studentId = _security.GetCurrentStudentId(User);
        if (!_security.HasStudentRole(User) || studentId is null)
            return Json(new { data = Array.Empty<object>(), total = 0, page, last_page = 0 });

        var result = await _financeService.GetLedgerPagedAsync(studentId.Value, page, size, search, HttpContext.RequestAborted);
        return Json(new
        {
            data = result.Items,
            total = result.TotalItems,
            page = result.Page,
            last_page = result.TotalPages
        });
    }

    [HttpGet("GetFinanceSummary")]
    public async Task<IActionResult> GetFinanceSummary()
    {
        var studentId = _security.GetCurrentStudentId(User);
        if (!_security.HasStudentRole(User) || studentId is null)
            return Json(new { totalInvoiced = 0m, totalPaid = 0m, totalDue = 0m, pendingInvoices = 0, lastPayment = (object?)null });

        var summary = await _financeService.GetFinanceSummaryAsync(studentId.Value, HttpContext.RequestAborted);
        var lastPayment = await _financeService.GetLastPaymentAsync(studentId.Value, HttpContext.RequestAborted);

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
    public async Task<IActionResult> DownloadReceipt(int paymentId)
    {
        if (!_security.HasStudentRole(User))
            return Forbid();

        var canAccess = await _security.CanAccessPaymentAsync(User, paymentId, HttpContext.RequestAborted);
        if (!canAccess)
            return Forbid();

        var pdf = await _receiptService.GenerateReceiptPdfAsync(paymentId, HttpContext.RequestAborted);
        if (pdf.Length == 0)
            return NotFound();

        return File(pdf, "application/pdf", $"Receipt_{paymentId}.pdf");
    }
}