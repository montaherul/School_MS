using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Reports;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class PaymentAllocationController : Controller
{
    private const string ViewPath = "~/Views/Fee/PaymentAllocation";
    private readonly IPaymentAllocationService _service;
    private readonly IFeeSecurityService _security;
    private readonly IPdfGenerator _pdfGenerator;
    public PaymentAllocationController(IPaymentAllocationService service, IFeeSecurityService security, IPdfGenerator pdfGenerator) { _service = service; _security = security; _pdfGenerator = pdfGenerator; }

    [RequirePermission("PaymentAllocations.Read")]
    public IActionResult Index() { return View($"{ViewPath}/Index.cshtml"); }

    [HttpGet]
    [RequirePermission("PaymentAllocations.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("PaymentAllocations.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("PaymentAllocations.Read")]
    public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null, int? paymentId = null, int? feeInvoiceId = null)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search, paymentId, feeInvoiceId);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "PaymentAllocations.Update" : "PaymentAllocations.Create"))
            return Forbid();

        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            return View($"{ViewPath}/CreateEdit.cshtml", new PaymentAllocationViewModel { Id = dto.Id, PaymentId = dto.PaymentId, FeeInvoiceId = dto.FeeInvoiceId, AllocatedAmount = dto.AllocatedAmount, Remarks = dto.Remarks });
        }
        return View($"{ViewPath}/CreateEdit.cshtml", new PaymentAllocationViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(PaymentAllocationViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "PaymentAllocations.Update" : "PaymentAllocations.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View($"{ViewPath}/CreateEdit.cshtml", vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Payment allocation updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Payment allocation created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(PaymentAllocationViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("PaymentAllocations.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Details.cshtml", new PaymentAllocationViewModel { Id = dto.Id, PaymentId = dto.PaymentId, FeeInvoiceId = dto.FeeInvoiceId, AllocatedAmount = dto.AllocatedAmount, Remarks = dto.Remarks });
    }

    [HttpGet]
    [RequirePermission("PaymentAllocations.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Delete.cshtml", new PaymentAllocationViewModel { Id = dto.Id, PaymentId = dto.PaymentId, FeeInvoiceId = dto.FeeInvoiceId, AllocatedAmount = dto.AllocatedAmount, Remarks = dto.Remarks });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("PaymentAllocations.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Payment allocation deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("PaymentAllocations.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Payment allocation restored successfully.";
        return RedirectToAction(nameof(Index));
    }
}
