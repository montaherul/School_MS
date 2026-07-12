using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.ViewModels.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class BuildingController : Controller
{
    private readonly IBuildingService _service;

    public BuildingController(IBuildingService service)
    {
        _service = service;
    }

    [RequirePermission("Academic.View")]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [RequirePermission("Academic.View")]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetForEditAsync(id, cancellationToken);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpGet]
    [RequirePermission("Academic.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, size, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    [RequirePermission("Academic.Create")]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();

            var vm = new BuildingViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Code = dto.Code,
                Address = dto.Address,
                TotalFloors = dto.TotalFloors,
                Description = dto.Description,
                IsActive = dto.IsActive
            };
            return View(vm);
        }

        return View(new BuildingViewModel { TotalFloors = 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Academic.Create")]
    public async Task<IActionResult> CreateEdit(BuildingViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        if (vm.IsEditMode)
        {
            await _service.UpdateAsync(vm, userId);
            TempData["SuccessMessage"] = "Building updated successfully.";
        }
        else
        {
            await _service.CreateAsync(vm, userId);
            TempData["SuccessMessage"] = "Building created successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Academic.Create")]
    public Task<IActionResult> Save(BuildingViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("Academic.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetForEditAsync(id, cancellationToken);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Academic.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId, cancellationToken);
        TempData["SuccessMessage"] = "Building deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
