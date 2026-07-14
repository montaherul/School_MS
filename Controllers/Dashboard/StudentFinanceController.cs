using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Dashboard;

[Authorize(Roles = "Student")]
[Route("StudentFinance")]
public class StudentFinanceController : Controller
{
    private readonly IStudentFinanceService _financeService;
    private readonly IFeeReceiptService _receiptService;
    private readonly IUnitOfWork _uow;

    public StudentFinanceController(
        IStudentFinanceService financeService,
        IFeeReceiptService receiptService,
        IUnitOfWork uow)
    {
        _financeService = financeService;
        _receiptService = receiptService;
        _uow = uow;
    }

    private int? GetCurrentUserId()
    {
        if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return userId;
        return null;
    }

    private async Task<int> GetStudentIdAsync()
    {
        var userId = GetCurrentUserId();
        if (userId is null) return 0;
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId.Value && !s.IsDeleted);
        return student?.Id ?? 0;
    }

    [HttpGet("GetInvoices")]
    public async Task<IActionResult> GetInvoices(int page = 1, int size = 10, string? search = null, int? status = null)
    {
        var studentId = await GetStudentIdAsync();
        if (studentId == 0)
            return Json(new { data = Array.Empty<object>(), total = 0, page, last_page = 0 });

        var result = await _financeService.GetInvoicesPagedAsync(studentId, page, size, search, status, HttpContext.RequestAborted);
        return Json(new { data = result.Items, total = result.TotalItems, page = result.Page, last_page = result.TotalPages });
    }

    [HttpGet("GetPayments")]
    public async Task<IActionResult> GetPayments(int page = 1, int size = 10, string? search = null)
    {
        var studentId = await GetStudentIdAsync();
        if (studentId == 0)
            return Json(new { data = Array.Empty<object>(), total = 0, page, last_page = 0 });

        var result = await _financeService.GetPaymentsPagedAsync(studentId, page, size, search, HttpContext.RequestAborted);
        return Json(new { data = result.Items, total = result.TotalItems, page = result.Page, last_page = result.TotalPages });
    }

    [HttpGet("GetLedger")]
    public async Task<IActionResult> GetLedger(int page = 1, int size = 10, string? search = null)
    {
        var studentId = await GetStudentIdAsync();
        if (studentId == 0)
            return Json(new { data = Array.Empty<object>(), total = 0, page, last_page = 0 });

        var result = await _financeService.GetLedgerPagedAsync(studentId, page, size, search, HttpContext.RequestAborted);
        return Json(new { data = result.Items, total = result.TotalItems, page = result.Page, last_page = result.TotalPages });
    }

    [HttpGet("GetFinanceSummary")]
    public async Task<IActionResult> GetFinanceSummary()
    {
        var studentId = await GetStudentIdAsync();
        if (studentId == 0)
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
    public async Task<IActionResult> DownloadReceipt(int paymentId)
    {
        var studentId = await GetStudentIdAsync();
        if (studentId == 0) return Forbid();

        var receiptData = await _receiptService.GetReceiptDataAsync(paymentId, HttpContext.RequestAborted);
        if (receiptData is null || receiptData.StudentId != studentId)
            return Forbid();

        var pdf = await _receiptService.GenerateReceiptPdfAsync(paymentId, HttpContext.RequestAborted);
        if (pdf.Length == 0)
            return NotFound();

        return File(pdf, "application/pdf", $"Receipt_{paymentId}.pdf");
    }
}
