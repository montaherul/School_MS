using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class StudentGroupController : Controller
{
    private readonly IStudentGroupService _service;

    public StudentGroupController(IStudentGroupService service)
    {
        _service = service;
    }

    [RequirePermission("Academics.View")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [RequirePermission("Academics.View")]
    public async Task<IActionResult> GetList(int page = 1, int pageSize = 20, string? search = null, CancellationToken ct = default)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search, ct);
        return Json(new { rows = result, total = result.FirstOrDefault()?.TotalRecords ?? 0 });
    }

    [HttpGet]
    [RequirePermission("Academics.Create")]
    public IActionResult Create()
    {
        return View(new StudentGroupUpsertDto { MinClass = 9, MaxClass = 10, DisplayOrder = 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Academics.Create")]
    public async Task<IActionResult> Create(StudentGroupUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);

        if (!string.IsNullOrWhiteSpace(dto.Code) && !await _service.IsCodeUniqueAsync(dto.Code, null, ct))
        {
            ModelState.AddModelError("Code", "Code already exists.");
            return View(dto);
        }

        await _service.CreateAsync(dto, User.Identity?.Name ?? "system", ct);
        TempData["Success"] = "Student group created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Academics.Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Academics.Edit")]
    public async Task<IActionResult> Edit(StudentGroupUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);

        if (!string.IsNullOrWhiteSpace(dto.Code) && !await _service.IsCodeUniqueAsync(dto.Code, dto.Id, ct))
        {
            ModelState.AddModelError("Code", "Code already exists.");
            return View(dto);
        }

        await _service.UpdateAsync(dto, User.Identity?.Name ?? "system", ct);
        TempData["Success"] = "Student group updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Academics.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, User.Identity?.Name ?? "system", ct);
        return Json(new { success = true, message = "Student group deleted successfully." });
    }
}
