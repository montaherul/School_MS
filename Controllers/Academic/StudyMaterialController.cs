using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class StudyMaterialController : Controller
{
    private readonly IStudyMaterialService _service;
    private readonly IAcademicYearService _academicYearService;
    private readonly ISchoolClassService _classService;
    private readonly IUnitOfWork _uow;

    public StudyMaterialController(
        IStudyMaterialService service,
        IAcademicYearService academicYearService,
        ISchoolClassService classService,
        IUnitOfWork uow)
    {
        _service = service;
        _academicYearService = academicYearService;
        _classService = classService;
        _uow = uow;
    }

    [RequirePermission("StudyMaterial.View")]
    public IActionResult Index() { return View(); }

    [HttpGet, RequirePermission("StudyMaterial.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, size, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet, RequirePermission("StudyMaterial.Create")]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        await PopulateDropdownsAsync(ct);
        ViewBag.Action = "Create";
        return View(new StudyMaterialUpsertDto());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("StudyMaterial.Create")]
    public async Task<IActionResult> Create(StudyMaterialUpsertDto dto, IFormFile? file, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) { await PopulateDropdownsAsync(ct); ViewBag.Action = "Create"; return View(dto); }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.CreateAsync(dto, file, userId);
        TempData["SuccessMessage"] = "Study Material created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet, RequirePermission("StudyMaterial.Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto is null) return NotFound();
        await PopulateDropdownsAsync(ct);
        ViewBag.Action = "Edit";
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("StudyMaterial.Edit")]
    public async Task<IActionResult> Edit(StudyMaterialUpsertDto dto, IFormFile? file, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) { await PopulateDropdownsAsync(ct); ViewBag.Action = "Edit"; return View(dto); }
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

    private async Task PopulateDropdownsAsync(CancellationToken ct)
    {
        var years = await _academicYearService.GetAllYearsAsync(ct);
        ViewBag.AcademicYears = new SelectList(years, "Id", "Name");

        var classes = await _classService.GetAllSchoolClassesAsync(ct);
        ViewBag.SchoolClasses = new SelectList(classes, "Id", "Name");

        var subjects = await _uow.Repository<Subject>().QueryNoTracking()
            .Where(s => !s.IsDeleted && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(ct);
        ViewBag.Subjects = new SelectList(subjects, "Id", "Name");
    }
}
