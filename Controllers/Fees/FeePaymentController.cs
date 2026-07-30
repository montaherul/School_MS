using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Reports;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeePaymentController : Controller
{
    private const string ViewPath = "~/Views/Fee/FeePayment";
    private readonly IFeePaymentService _service;
    private readonly IFeeSecurityService _security;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IFinancePostingService _postingService;
    private readonly IFeeInvoiceService _invoiceService;
    private readonly IFeeReceiptService _receiptService;
    public FeePaymentController(IFeePaymentService service, IFeeSecurityService security, IPdfGenerator pdfGenerator, IFinancePostingService postingService, IFeeInvoiceService invoiceService, IFeeReceiptService receiptService) { _service = service; _security = security; _pdfGenerator = pdfGenerator; _postingService = postingService; _invoiceService = invoiceService; _receiptService = receiptService; }

    [RequirePermission("FeePayments.Read")]
    public IActionResult Index() { return View($"{ViewPath}/Index.cshtml"); }

    [HttpGet]
    [RequirePermission("FeePayments.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FeePayments.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FeePayments.Read")]
    public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null, int? feeInvoiceId = null, int? paymentMethod = null)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search, feeInvoiceId, paymentMethod);
        if (_security.HasStudentRole(User))
        {
            var myId = _security.GetCurrentStudentId(User);
            if (myId.HasValue)
                result = new PagedResult<FeePaymentListItemDto> { Items = result.Items.Where(i => i.StudentId == myId.Value).ToList(), Page = result.Page, PageSize = result.PageSize, TotalItems = result.TotalItems };
        }
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    [RequirePermission("FeePayments.Read")]
    public async Task<IActionResult> ExportExcel(string? search = null, int? feeInvoiceId = null, int? paymentMethod = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search, feeInvoiceId, paymentMethod);
        var bytes = FeeListExporter.ExportToExcel(result.Items.ToList(), "Fee Payments");
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "fee-payments.xlsx");
    }

    [HttpGet]
    [RequirePermission("FeePayments.Read")]
    public async Task<IActionResult> ExportPdf(string? search = null, int? feeInvoiceId = null, int? paymentMethod = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search, feeInvoiceId, paymentMethod);
        var html = FeeListExporter.BuildExportHtml(result.Items.ToList(), "Fee Payments");
        var bytes = _pdfGenerator.GenerateFromHtml(html);
        return File(bytes, "application/pdf", "fee-payments.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "FeePayments.Update" : "FeePayments.Create"))
            return Forbid();
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            if (_security.HasStudentRole(User) && !await _security.CanAccessInvoiceAsync(User, dto.FeeInvoiceId))
                return Forbid();
            return View($"{ViewPath}/CreateEdit.cshtml", new FeePaymentViewModel { Id = dto.Id, FeeInvoiceId = dto.FeeInvoiceId, Amount = dto.Amount, LateFee = dto.LateFee, DiscountAmount = dto.DiscountAmount, Method = dto.Method, ReferenceNo = dto.ReferenceNo, PaidAt = dto.PaidAt, Remarks = dto.Remarks });
        }
        return View($"{ViewPath}/CreateEdit.cshtml", new FeePaymentViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FeePaymentViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FeePayments.Update" : "FeePayments.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View($"{ViewPath}/CreateEdit.cshtml", vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Payment updated."; }
        else
        {
            var ct = HttpContext.RequestAborted;
            var paymentDto = new CashierPaymentDto
            {
                Amount = vm.Amount,
                LateFee = vm.LateFee,
                DiscountAmount = vm.DiscountAmount,
                Method = vm.Method,
                ReferenceNo = vm.ReferenceNo,
                Remarks = vm.Remarks
            };
            var invoice = await _invoiceService.GetByIdAsync(vm.FeeInvoiceId, ct);
            if (invoice == null)
            {
                TempData["ErrorMessage"] = "Invoice not found.";
                return RedirectToAction(nameof(Index));
            }
            var paymentId = await _postingService.PostFeeCollectionFullAsync(invoice.StudentId, new List<int> { vm.FeeInvoiceId }, paymentDto, userId, ct);
            TempData["SuccessMessage"] = "Payment recorded and posted to General Ledger.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(FeePaymentViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("FeePayments.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (_security.HasStudentRole(User) && !await _security.CanAccessPaymentAsync(User, id))
            return Forbid();
        return View($"{ViewPath}/Details.cshtml", new FeePaymentViewModel { Id = dto.Id, FeeInvoiceId = dto.FeeInvoiceId, Amount = dto.Amount, LateFee = dto.LateFee, DiscountAmount = dto.DiscountAmount, Method = dto.Method, ReferenceNo = dto.ReferenceNo, PaidAt = dto.PaidAt, Remarks = dto.Remarks });
    }

    [HttpGet]
    [RequirePermission("FeePayments.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (_security.HasStudentRole(User) && !await _security.CanAccessPaymentAsync(User, id))
            return Forbid();
        return View($"{ViewPath}/Delete.cshtml", new FeePaymentViewModel { Id = dto.Id, FeeInvoiceId = dto.FeeInvoiceId, Amount = dto.Amount, LateFee = dto.LateFee, DiscountAmount = dto.DiscountAmount, Method = dto.Method, ReferenceNo = dto.ReferenceNo, PaidAt = dto.PaidAt, Remarks = dto.Remarks });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeePayments.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Payment deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("FeeReceipts.Read")]
    public async Task<IActionResult> Receipt(int id, CancellationToken ct)
    {
        var data = await _receiptService.GetReceiptDataAsync(id, ct);
        if (data is null) return NotFound();
        return View($"{ViewPath}/Receipt.cshtml", data);
    }

    [HttpGet]
    [RequirePermission("FeeReceipts.Read")]
    public async Task<IActionResult> DownloadReceipt(int id, CancellationToken ct)
    {
        var pdf = await _receiptService.GenerateReceiptPdfAsync(id, ct);
        if (pdf.Length == 0) return NotFound();
        return File(pdf, "application/pdf", $"receipt-{id:D6}.pdf");
    }

    [HttpGet]
    [RequirePermission("FeeReceipts.Read")]
    public async Task<IActionResult> VerifyReceipt(string code, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(code) || code.Length != 12)
            return Json(new { valid = false, message = "Invalid verification code." });
        var (paymentId, paidAt) = await _service.VerifyReceiptCodeAsync(code, ct);
        if (paymentId <= 0)
            return Json(new { valid = false, message = "Invalid or expired receipt verification code." });
        var expectedCode = _receiptService.GenerateVerificationCode(paymentId, paidAt);
        var isValid = string.Equals(code, expectedCode, StringComparison.OrdinalIgnoreCase);
        return Json(new { valid = isValid, message = isValid ? "Receipt verified." : "Verification code mismatch." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeePayments.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (_security.HasStudentRole(User) && !await _security.CanAccessPaymentAsync(User, id))
            return Forbid();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Payment restored successfully.";
        return RedirectToAction(nameof(Index));
    }
}
