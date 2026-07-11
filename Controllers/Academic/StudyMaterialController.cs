using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class StudyMaterialController : Controller
{
    private readonly IStudyMaterialService _service;

    public StudyMaterialController(IStudyMaterialService service) { _service = service; }

    [RequirePermission("StudyMaterial.View")]
    public IActionResult Index() { return View(); }

    [HttpGet, RequirePermission("StudyMaterial.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, size, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet, RequirePermission("StudyMaterial.Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Action = "Create";
        return View(new StudyMaterialUpsertDto());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("StudyMaterial.Create")]
    public async Task<IActionResult> Create(StudyMaterialUpsertDto dto, IFormFile? file)
    {
        if (!ModelState.IsValid) { ViewBag.Action = "Create"; return View(dto); }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.CreateAsync(dto, file, userId);
        TempData["SuccessMessage"] = "Study Material created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet, RequirePermission("StudyMaterial.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto is null) return NotFound();
        ViewBag.Action = "Edit";
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("StudyMaterial.Edit")]
    public async Task<IActionResult> Edit(StudyMaterialUpsertDto dto, IFormFile? file)
    {
        if (!ModelState.IsValid) { ViewBag.Action = "Edit"; return View(dto); }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.UpdateAsync(dto, file, userId);
        TempData["SuccessMessage"] = "Study Material updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("StudyMaterial.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        return Json(new { success = true, message = "Study Material deleted successfully." });
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("StudyMaterial.Edit")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.ToggleActiveAsync(id, userId);
        return Json(new { success = true });
    }

    [HttpGet, RequirePermission("StudyMaterial.View")]
    public async Task<IActionResult> Download(int id)
    {
        var path = await _service.GetFilePathAsync(id);
        if (string.IsNullOrEmpty(path)) return NotFound();
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path.TrimStart('/'));
        if (!System.IO.File.Exists(fullPath)) return NotFound();
        var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
        var dto = await _service.GetForEditAsync(id);
        var fileName = dto?.ExistingFileName ?? "material.pdf";
        return File(bytes, "application/octet-stream", fileName);
    }

    [HttpGet, RequirePermission("StudyMaterial.View")]
    public async Task<IActionResult> ExportPdf(int id)
    {
        var pdf = await _service.ExportPdfAsync(id);
        if (pdf is null) return NotFound();
        return File(pdf, "application/pdf", $"studymaterial-{id}.pdf");
    }
}
