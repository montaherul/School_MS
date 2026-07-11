using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class LessonPlanController : Controller
{
    private readonly ILessonPlanService _service;

    public LessonPlanController(ILessonPlanService service) { _service = service; }

    [RequirePermission("LessonPlan.View")]
    public IActionResult Index() { return View(); }

    [HttpGet, RequirePermission("LessonPlan.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, size, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet, RequirePermission("LessonPlan.Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Action = "Create";
        return View(new LessonPlanUpsertDto());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("LessonPlan.Create")]
    public async Task<IActionResult> Create(LessonPlanUpsertDto dto)
    {
        if (!ModelState.IsValid) { ViewBag.Action = "Create"; return View(dto); }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.CreateAsync(dto, userId);
        TempData["SuccessMessage"] = "Lesson Plan created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet, RequirePermission("LessonPlan.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto is null) return NotFound();
        ViewBag.Action = "Edit";
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("LessonPlan.Edit")]
    public async Task<IActionResult> Edit(LessonPlanUpsertDto dto)
    {
        if (!ModelState.IsValid) { ViewBag.Action = "Edit"; return View(dto); }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.UpdateAsync(dto, userId);
        TempData["SuccessMessage"] = "Lesson Plan updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("LessonPlan.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        return Json(new { success = true, message = "Lesson Plan deleted successfully." });
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("LessonPlan.Edit")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.ToggleActiveAsync(id, userId);
        return Json(new { success = true });
    }

    [HttpGet, RequirePermission("LessonPlan.View")]
    public async Task<IActionResult> ExportPdf(int id)
    {
        var pdf = await _service.ExportPdfAsync(id);
        if (pdf is null) return NotFound();
        return File(pdf, "application/pdf", $"lessonplan-{id}.pdf");
    }
}
