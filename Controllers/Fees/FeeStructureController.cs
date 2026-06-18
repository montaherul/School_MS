using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeStructureController : Controller
{
    private readonly IFeeStructureService _service;
    private readonly IFeeSecurityService _security;
    public FeeStructureController(IFeeStructureService service, IFeeSecurityService security) { _service = service; _security = security; }

    [RequirePermission("FeeStructures.Read")]
    public IActionResult Index() { return View(); }

    [HttpGet]
    [RequirePermission("FeeStructures.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FeeStructures.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FeeStructures.Read")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, int? schoolClassId = null, int? feeCategoryId = null)
    {
        var result = await _service.GetPagedAsync(page, size, search, schoolClassId, feeCategoryId);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "FeeStructures.Update" : "FeeStructures.Create"))
        {
            return Forbid();
        }

        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            var vm = new FeeStructureViewModel
            {
                Id = dto.Id, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId,
                AcademicYearId = dto.AcademicYearId, FeeName = dto.FeeName, Description = dto.Description,
                Amount = dto.Amount, IsRecurring = dto.IsRecurring, Frequency = dto.Frequency,
                DueDay = dto.DueDay, IsActive = dto.IsActive
            };
            return View(vm);
        }
        return View(new FeeStructureViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FeeStructureViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FeeStructures.Update" : "FeeStructures.Create"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "FeeStructure updated successfully."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "FeeStructure created successfully."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(FeeStructureViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("FeeStructures.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View(new FeeStructureViewModel
        {
            Id = dto.Id, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId,
            AcademicYearId = dto.AcademicYearId, FeeName = dto.FeeName, Description = dto.Description,
            Amount = dto.Amount, IsRecurring = dto.IsRecurring, Frequency = dto.Frequency,
            DueDay = dto.DueDay, IsActive = dto.IsActive
        });
    }

    [HttpGet]
    [RequirePermission("FeeStructures.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View(new FeeStructureViewModel
        {
            Id = dto.Id, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId,
            AcademicYearId = dto.AcademicYearId, FeeName = dto.FeeName, Description = dto.Description,
            Amount = dto.Amount, IsRecurring = dto.IsRecurring, Frequency = dto.Frequency,
            DueDay = dto.DueDay, IsActive = dto.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeStructures.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "FeeStructure deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeStructures.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Fee structure restored successfully.";
        return RedirectToAction(nameof(Index));
    }

}
