using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FineRuleController : Controller
{
    private readonly IFineRuleService _service;
    private readonly IFeeSecurityService _security;
    public FineRuleController(IFineRuleService service, IFeeSecurityService security) { _service = service; _security = security; }

    [RequirePermission("FineRules.Read")]
    public IActionResult Index() { return View(); }

    [HttpGet]
    [RequirePermission("FineRules.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FineRules.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FineRules.Read")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, size, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "FineRules.Update" : "FineRules.Create"))
            return Forbid();
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            return View(new FineRuleViewModel { Id = dto.Id, Name = dto.Name, GraceDays = dto.GraceDays, FinePerDay = dto.FinePerDay });
        }
        return View(new FineRuleViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FineRuleViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FineRules.Update" : "FineRules.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Fine rule updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Fine rule created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(FineRuleViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("FineRules.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View(new FineRuleViewModel { Id = dto.Id, Name = dto.Name, GraceDays = dto.GraceDays, FinePerDay = dto.FinePerDay });
    }

    [HttpGet]
    [RequirePermission("FineRules.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View(new FineRuleViewModel { Id = dto.Id, Name = dto.Name, GraceDays = dto.GraceDays, FinePerDay = dto.FinePerDay });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FineRules.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Fine rule deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FineRules.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Fine rule restored successfully.";
        return RedirectToAction(nameof(Index));
    }

}
