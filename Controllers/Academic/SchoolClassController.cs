using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.ViewModels.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class SchoolClassController : Controller
{
    private readonly ISchoolClassService _service;
    public SchoolClassController(ISchoolClassService service) { _service = service; }

    public IActionResult Index() { return View(); }
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [RequirePermission("Classes.View")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var dto = await _service.GetForEditAsync(id, ct);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpGet, RequirePermission("Classes.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, size, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet, RequirePermission("Classes.Create")]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            var vm = new SchoolClassViewModel
            {
                Id = dto.Id, Name = dto.Name, NameBn = dto.NameBn, Code = dto.Code,
                SortOrder = dto.SortOrder, Capacity = dto.Capacity, Description = dto.Description,
                IsGroupBased = dto.IsGroupBased, IsActive = dto.IsActive
            };
            return View(vm);
        }
        return View(new SchoolClassViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Classes.Create")]
    public async Task<IActionResult> CreateEdit(SchoolClassViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        try
        {
            if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Class updated successfully."; }
            else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Class created successfully."; }
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(vm);
        }
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Classes.Create")]
    public Task<IActionResult> Save(SchoolClassViewModel vm) => CreateEdit(vm);

    [HttpGet, RequirePermission("Classes.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var dto = await _service.GetForEditAsync(id, ct);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Classes.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        try
        {
            await _service.DeleteAsync(id, userId, ct);
            TempData["SuccessMessage"] = "Class deleted successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, RequirePermission("Classes.Create")]
    public async Task<IActionResult> Clone(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        var cloned = await _service.CloneAsync(id, userId, ct);
        TempData["SuccessMessage"] = "Class cloned successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, RequirePermission("Classes.Create")]
    public async Task<IActionResult> ToggleActive(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.ToggleActiveAsync(id, userId, ct);
        return Json(new { success = true });
    }

    [HttpPost, RequirePermission("Classes.Create")]
    public async Task<IActionResult> Archive(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.ArchiveAsync(id, userId, ct);
        TempData["SuccessMessage"] = "Class archived.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, RequirePermission("Classes.Create")]
    public async Task<IActionResult> Restore(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId, ct);
        TempData["SuccessMessage"] = "Class restored.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet, RequirePermission("Classes.View")]
    public async Task<IActionResult> CheckName(string name, int? excludeId, CancellationToken ct)
    {
        var unique = await _service.IsNameUniqueAsync(name, excludeId, ct);
        return Json(new { valid = unique });
    }

    [HttpGet, RequirePermission("Classes.View")]
    public async Task<IActionResult> CheckCode(string code, int? excludeId, CancellationToken ct)
    {
        var unique = await _service.IsCodeUniqueAsync(code, excludeId, ct);
        return Json(new { valid = unique });
    }

    [HttpGet, RequirePermission("Classes.View")]
    public async Task<IActionResult> CanDelete(int id, CancellationToken ct)
    {
        var can = await _service.CanDeleteAsync(id, ct);
        return Json(new { canDelete = can });
    }
}
