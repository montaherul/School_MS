using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
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

    [RequirePermission("Subjects.View")]
    public IActionResult Index() { return View(); }
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [RequirePermission("Subjects.View")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var dto = await _service.GetForEditAsync(id, ct);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpGet, RequirePermission("Subjects.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, string? group = null, string? status = null)
    {
        var result = await _service.GetPagedAsync(page, size, search, group, status);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet, RequirePermission("Subjects.Create")]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            var vm = new SubjectViewModel
            {
                Id = dto.Id, Code = dto.Code, Name = dto.Name, NameBn = dto.NameBn,
                ShortName = dto.ShortName, Category = dto.Category, SubjectGroup = dto.SubjectGroup,
                IsReligionSubject = dto.IsReligionSubject, ReligionType = dto.ReligionType,
                IsOptional = dto.IsOptional, IsPractical = dto.IsPractical,
                DefaultFullMarks = dto.DefaultFullMarks, DefaultPassMarks = dto.DefaultPassMarks,
                DisplayOrder = dto.DisplayOrder, IsActive = dto.IsActive
            };
            return View(vm);
        }
        return View(new SubjectViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Subjects.Create")]
    public async Task<IActionResult> CreateEdit(SubjectViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        var dto = new SubjectUpsertDto
        {
            Id = vm.Id, Code = vm.Code, Name = vm.Name, NameBn = vm.NameBn,
            ShortName = vm.ShortName, Category = vm.Category, SubjectGroup = vm.SubjectGroup,
            IsReligionSubject = vm.IsReligionSubject, ReligionType = vm.ReligionType,
            IsOptional = vm.IsOptional, IsPractical = vm.IsPractical,
            DefaultFullMarks = vm.DefaultFullMarks, DefaultPassMarks = vm.DefaultPassMarks,
            DisplayOrder = vm.DisplayOrder, IsActive = vm.IsActive
        };
        if (vm.IsEditMode) { await _service.UpdateAsync(dto, userId); TempData["SuccessMessage"] = "Subject updated successfully."; }
        else { await _service.CreateAsync(dto, userId); TempData["SuccessMessage"] = "Subject created successfully."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Subjects.Create")]
    public Task<IActionResult> Save(SubjectViewModel vm) => CreateEdit(vm);

    [HttpGet, RequirePermission("Subjects.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var dto = await _service.GetForEditAsync(id, ct);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Subjects.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId, ct);
        TempData["SuccessMessage"] = "Subject deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, RequirePermission("Subjects.Delete")]
    public async Task<IActionResult> DeleteAjax(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId, ct);
        return Json(new { success = true });
    }

    [HttpPost, RequirePermission("Subjects.Create")]
    public async Task<IActionResult> ToggleActive(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.ToggleActiveAsync(id, userId, ct);
        return Json(new { success = true });
    }

    [HttpPost, RequirePermission("Subjects.Create")]
    public async Task<IActionResult> BulkActivate([FromBody] List<int> ids, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.BulkActivateAsync(ids, userId, ct);
        return Json(new { success = true });
    }

    [HttpPost, RequirePermission("Subjects.Create")]
    public async Task<IActionResult> BulkDeactivate([FromBody] List<int> ids, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.BulkDeactivateAsync(ids, userId, ct);
        return Json(new { success = true });
    }

    [HttpPost, RequirePermission("Subjects.Create")]
    public async Task<IActionResult> BulkImport([FromBody] List<SubjectUpsertDto> dtos, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.BulkImportAsync(dtos, userId, ct);
        return Json(new { success = true, message = "Subjects imported successfully." });
    }

    [HttpGet, RequirePermission("Subjects.View")]
    public async Task<IActionResult> BulkExport(CancellationToken ct)
    {
        var items = await _service.BulkExportAsync(ct);
        return Json(new { success = true, data = items });
    }
}
