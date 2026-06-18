using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeDiscountController : Controller
{
    private readonly IFeeDiscountService _service;
    private readonly IFeeSecurityService _security;
    public FeeDiscountController(IFeeDiscountService service, IFeeSecurityService security) { _service = service; _security = security; }

    [RequirePermission("FeeDiscounts.Read")]
    public IActionResult Index() { return View(); }

    [HttpGet]
    [RequirePermission("FeeDiscounts.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FeeDiscounts.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FeeDiscounts.Read")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, size, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "FeeDiscounts.Update" : "FeeDiscounts.Create"))
            return Forbid();
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            return View(new FeeDiscountViewModel { Id = dto.Id, Name = dto.Name, Description = dto.Description, DiscountType = dto.DiscountType, Value = dto.Value, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId, FeeStructureId = dto.FeeStructureId, IsActive = dto.IsActive, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo });
        }
        return View(new FeeDiscountViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FeeDiscountViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FeeDiscounts.Update" : "FeeDiscounts.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Discount updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Discount created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(FeeDiscountViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("FeeDiscounts.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View(new FeeDiscountViewModel { Id = dto.Id, Name = dto.Name, Description = dto.Description, DiscountType = dto.DiscountType, Value = dto.Value, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId, FeeStructureId = dto.FeeStructureId, IsActive = dto.IsActive, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo });
    }

    [HttpGet]
    [RequirePermission("FeeDiscounts.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View(new FeeDiscountViewModel { Id = dto.Id, Name = dto.Name, Description = dto.Description, DiscountType = dto.DiscountType, Value = dto.Value, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId, FeeStructureId = dto.FeeStructureId, IsActive = dto.IsActive, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeDiscounts.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Discount deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeDiscounts.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Discount restored successfully.";
        return RedirectToAction(nameof(Index));
    }

}
