using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class SyllabusController : Controller
{
    private readonly ISyllabusService _service;

    public SyllabusController(ISyllabusService service) { _service = service; }

    [RequirePermission("Syllabus.View")]
    public IActionResult Index() { return View(); }

    [HttpGet, RequirePermission("Syllabus.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, size, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet, RequirePermission("Syllabus.Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Action = "Create";
        return View(new SyllabusUpsertDto());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Syllabus.Create")]
    public async Task<IActionResult> Create(SyllabusUpsertDto dto, IFormFile? file)
    {
        if (!ModelState.IsValid) { ViewBag.Action = "Create"; return View(dto); }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.CreateAsync(dto, file, userId);
        TempData["SuccessMessage"] = "Syllabus created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet, RequirePermission("Syllabus.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto is null) return NotFound();
        ViewBag.Action = "Edit";
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Syllabus.Edit")]
    public async Task<IActionResult> Edit(SyllabusUpsertDto dto, IFormFile? file)
    {
        if (!ModelState.IsValid) { ViewBag.Action = "Edit"; return View(dto); }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.UpdateAsync(dto, file, userId);
        TempData["SuccessMessage"] = "Syllabus updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Syllabus.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        return Json(new { success = true, message = "Syllabus deleted successfully." });
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("Syllabus.Edit")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.ToggleActiveAsync(id, userId);
        return Json(new { success = true });
    }

    [HttpGet, RequirePermission("Syllabus.View")]
    public async Task<IActionResult> Download(int id)
    {
        var path = await _service.GetFilePathAsync(id);
        if (string.IsNullOrEmpty(path)) return NotFound();
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path.TrimStart('/'));
        if (!System.IO.File.Exists(fullPath)) return NotFound();
        var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
        var dto = await _service.GetForEditAsync(id);
        var fileName = dto?.ExistingFileName ?? "syllabus.pdf";
        return File(bytes, "application/octet-stream", fileName);
    }

    [HttpGet, RequirePermission("Syllabus.View")]
    public async Task<IActionResult> ExportPdf(int id)
    {
        var pdf = await _service.ExportPdfAsync(id);
        if (pdf is null) return NotFound();
        return File(pdf, "application/pdf", $"syllabus-{id}.pdf");
    }
}
