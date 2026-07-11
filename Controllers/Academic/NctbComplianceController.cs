using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Services.Interfaces;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
[Route("Academic/[controller]")]
public class NctbComplianceController : Controller
{
    private readonly INctbComplianceService _service;
    private readonly IAcademicYearService _academicYearService;

    public NctbComplianceController(INctbComplianceService service, IAcademicYearService academicYearService)
    {
        _service = service;
        _academicYearService = academicYearService;
    }

    [RequirePermission("Curriculum.View")]
    public async Task<IActionResult> Index(int academicYearId = 0, CancellationToken ct = default)
    {
        var versions = await _service.GetCurriculumVersionsAsync(ct);
        if (versions.Count == 0) return View(new NctbComplianceReportDto { AcademicYearName = "N/A" });

        var firstYearId = academicYearId > 0 ? academicYearId : versions.First().AcademicYearId;
        ViewBag.CurriculumVersions = versions;
        var report = await _service.GetComplianceReportAsync(firstYearId, ct);
        return View(report);
    }

    [RequirePermission("Curriculum.Create")]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        var years = await _academicYearService.GetAllYearsAsync(ct);
        ViewBag.AcademicYears = new SelectList(years, "Id", "Name");
        return View(new CurriculumVersionUpsertDto { EffectiveFrom = DateOnly.FromDateTime(DateTime.Today) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Curriculum.Create")]
    public async Task<IActionResult> Create(CurriculumVersionUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            var years = await _academicYearService.GetAllYearsAsync(ct);
            ViewBag.AcademicYears = new SelectList(years, "Id", "Name");
            return View(dto);
        }

        await _service.CreateCurriculumVersionAsync(dto, ct);
        TempData["Success"] = "Curriculum version created successfully";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("Curriculum.Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
    {
        var version = await _service.GetCurriculumVersionByIdAsync(id, ct);
        if (version is null) return NotFound();

        var years = await _academicYearService.GetAllYearsAsync(ct);
        ViewBag.AcademicYears = new SelectList(years, "Id", "Name", version.AcademicYearId);

        return View(new CurriculumVersionUpsertDto
        {
            VersionName = version.VersionName,
            AcademicYearId = version.AcademicYearId,
            EffectiveFrom = version.EffectiveFrom,
            IsCurrent = version.IsCurrent,
            Description = version.Description
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Curriculum.Edit")]
    public async Task<IActionResult> Edit(int id, CurriculumVersionUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            var years = await _academicYearService.GetAllYearsAsync(ct);
            ViewBag.AcademicYears = new SelectList(years, "Id", "Name");
            return View(dto);
        }

        try
        {
            await _service.UpdateCurriculumVersionAsync(id, dto, ct);
            TempData["Success"] = "Curriculum version updated successfully";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Curriculum.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var deleted = await _service.DeleteCurriculumVersionAsync(id, ct);
        if (!deleted) return NotFound();
        TempData["Success"] = "Curriculum version deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}
