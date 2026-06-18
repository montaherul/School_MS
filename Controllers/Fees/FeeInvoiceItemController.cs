using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeInvoiceItemController : Controller
{
    private readonly IFeeInvoiceItemService _service;
    private readonly IFeeSecurityService _security;
    public FeeInvoiceItemController(IFeeInvoiceItemService service, IFeeSecurityService security) { _service = service; _security = security; }

    [RequirePermission("FeeInvoiceItems.Read")]
    public IActionResult Index() { return View(); }

    [HttpGet]
    [RequirePermission("FeeInvoiceItems.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FeeInvoiceItems.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FeeInvoiceItems.Read")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, int? feeInvoiceId = null)
    {
        var result = await _service.GetPagedAsync(page, size, search, feeInvoiceId);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "FeeInvoiceItems.Update" : "FeeInvoiceItems.Create"))
            return Forbid();
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            return View(new FeeInvoiceItemViewModel { Id = dto.Id, FeeInvoiceId = dto.FeeInvoiceId, FeeStructureId = dto.FeeStructureId, FeeCategoryId = dto.FeeCategoryId, Description = dto.Description, Amount = dto.Amount, DiscountAmount = dto.DiscountAmount, NetAmount = dto.NetAmount });
        }
        return View(new FeeInvoiceItemViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FeeInvoiceItemViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FeeInvoiceItems.Update" : "FeeInvoiceItems.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Item updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Item created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(FeeInvoiceItemViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("FeeInvoiceItems.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View(new FeeInvoiceItemViewModel { Id = dto.Id, FeeInvoiceId = dto.FeeInvoiceId, FeeStructureId = dto.FeeStructureId, FeeCategoryId = dto.FeeCategoryId, Description = dto.Description, Amount = dto.Amount, DiscountAmount = dto.DiscountAmount, NetAmount = dto.NetAmount });
    }

    [HttpGet]
    [RequirePermission("FeeInvoiceItems.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View(new FeeInvoiceItemViewModel { Id = dto.Id, FeeInvoiceId = dto.FeeInvoiceId, FeeStructureId = dto.FeeStructureId, FeeCategoryId = dto.FeeCategoryId, Description = dto.Description, Amount = dto.Amount, DiscountAmount = dto.DiscountAmount, NetAmount = dto.NetAmount });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeInvoiceItems.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Item deleted.";
        return RedirectToAction(nameof(Index));
    }

}
