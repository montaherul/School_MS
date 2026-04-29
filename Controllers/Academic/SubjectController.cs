using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.ViewModels.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class SubjectController : Controller
{
    private readonly ISubjectService _service;
    public SubjectController(ISubjectService service) { _service = service; }

    public IActionResult Index() { return View(); }
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetForEditAsync(id, cancellationToken);
        return dto is null ? NotFound() : View(dto);
    }

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
            var vm = new SubjectViewModel { Id = dto.Id,Code = dto.Code,Name = dto.Name,            };
            return View(vm);
        }
        return View(new SubjectViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(SubjectViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Subject updated successfully."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Subject created successfully."; }
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(SubjectViewModel vm) => CreateEdit(vm);

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetForEditAsync(id, cancellationToken);
        return dto is null ? NotFound() : View(dto);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId, cancellationToken);
        TempData["SuccessMessage"] = "Subject deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}

