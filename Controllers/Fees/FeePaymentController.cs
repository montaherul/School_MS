using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeePaymentController : Controller
{
    private readonly IFeePaymentService _service;
    private readonly IFeeSecurityService _security;
    public FeePaymentController(IFeePaymentService service, IFeeSecurityService security) { _service = service; _security = security; }

    [RequirePermission("FeePayments.Read")]
    public IActionResult Index() { return View(); }

    [HttpGet]
    [RequirePermission("FeePayments.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FeePayments.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FeePayments.Read")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, int? feeInvoiceId = null, int? paymentMethod = null)
    {
        var result = await _service.GetPagedAsync(page, size, search, feeInvoiceId, paymentMethod);
        if (_security.HasStudentRole(User))
        {
            var myId = _security.GetCurrentStudentId(User);
            if (myId.HasValue)
                result = new PagedResult<FeePaymentListItemDto> { Items = result.Items.Where(i => i.StudentId == myId.Value).ToList(), Page = result.Page, PageSize = result.PageSize, TotalItems = result.TotalItems };
        }
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
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
            return View(new FeePaymentViewModel { Id = dto.Id, FeeInvoiceId = dto.FeeInvoiceId, Amount = dto.Amount, LateFee = dto.LateFee, DiscountAmount = dto.DiscountAmount, Method = dto.Method, ReferenceNo = dto.ReferenceNo, PaidAt = dto.PaidAt, Remarks = dto.Remarks });
        }
        return View(new FeePaymentViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FeePaymentViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FeePayments.Update" : "FeePayments.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Payment updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Payment recorded."; }
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
        return View(new FeePaymentViewModel { Id = dto.Id, FeeInvoiceId = dto.FeeInvoiceId, Amount = dto.Amount, LateFee = dto.LateFee, DiscountAmount = dto.DiscountAmount, Method = dto.Method, ReferenceNo = dto.ReferenceNo, PaidAt = dto.PaidAt, Remarks = dto.Remarks });
    }

    [HttpGet]
    [RequirePermission("FeePayments.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (_security.HasStudentRole(User) && !await _security.CanAccessPaymentAsync(User, id))
            return Forbid();
        return View(new FeePaymentViewModel { Id = dto.Id, FeeInvoiceId = dto.FeeInvoiceId, Amount = dto.Amount, LateFee = dto.LateFee, DiscountAmount = dto.DiscountAmount, Method = dto.Method, ReferenceNo = dto.ReferenceNo, PaidAt = dto.PaidAt, Remarks = dto.Remarks });
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
    public async Task<IActionResult> Receipt(int id)
    {
        var receiptService = HttpContext.RequestServices.GetRequiredService<IFeeReceiptService>();
        var data = await receiptService.GetReceiptDataAsync(id);
        if (data is null) return NotFound();
        return View(data);
    }

    [HttpGet]
    [RequirePermission("FeeReceipts.Read")]
    public async Task<IActionResult> DownloadReceipt(int id)
    {
        var receiptService = HttpContext.RequestServices.GetRequiredService<IFeeReceiptService>();
        var pdf = await receiptService.GenerateReceiptPdfAsync(id);
        if (pdf.Length == 0) return NotFound();
        return File(pdf, "application/pdf", $"receipt-{id:D6}.pdf");
    }

    [HttpGet]
    [RequirePermission("FeeReceipts.Read")]
    public async Task<IActionResult> VerifyReceipt(string code)
    {
        if (string.IsNullOrEmpty(code) || code.Length != 12)
            return Json(new { valid = false, message = "Invalid verification code." });
        return Json(new { valid = true, message = "Receipt verified." });
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
