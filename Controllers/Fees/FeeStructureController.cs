using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

using SchoolManagementSystem.Constants;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize(Roles = Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.Principal + "," + Roles.Accountant + "," + Roles.Staff)]
public class FeeStructureController : Controller
{
    private readonly IFeeStructureService _service;
    public FeeStructureController(IFeeStructureService service) { _service = service; }

    public IActionResult Index() { return View(); }

    [HttpGet]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, size, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            var vm = new FeeStructureViewModel { Id = dto.Id,SchoolClassId = dto.SchoolClassId,FeeName = dto.FeeName,Amount = dto.Amount,IsRecurring = dto.IsRecurring,            };
            return View(vm);
        }
        return View(new FeeStructureViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FeeStructureViewModel vm)
    {
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
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();

        return View(new FeeStructureViewModel
        {
            Id = dto.Id,
            SchoolClassId = dto.SchoolClassId,
            FeeName = dto.FeeName,
            Amount = dto.Amount,
            IsRecurring = dto.IsRecurring
        });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();

        return View(new FeeStructureViewModel
        {
            Id = dto.Id,
            SchoolClassId = dto.SchoolClassId,
            FeeName = dto.FeeName,
            Amount = dto.Amount,
            IsRecurring = dto.IsRecurring
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "FeeStructure deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}

