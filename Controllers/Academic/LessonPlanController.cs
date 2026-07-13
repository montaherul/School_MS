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
using TeacherEntity = SchoolManagementSystem.Models.Entities.Teachers.Teacher;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class LessonPlanController : Controller
{
    private readonly ILessonPlanService _service;
    private readonly IAcademicYearService _academicYearService;
    private readonly ISchoolClassService _classService;
    private readonly IUnitOfWork _uow;

    public LessonPlanController(
        ILessonPlanService service,
        IAcademicYearService academicYearService,
        ISchoolClassService classService,
        IUnitOfWork uow)
    {
        _service = service;
        _academicYearService = academicYearService;
        _classService = classService;
        _uow = uow;
    }

    [RequirePermission("LessonPlan.View")]
    public IActionResult Index() { return View(); }

    [HttpGet, RequirePermission("LessonPlan.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, size, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet, RequirePermission("LessonPlan.Create")]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        await PopulateDropdownsAsync(ct);
        ViewBag.Action = "Create";
        return View(new LessonPlanUpsertDto());
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("LessonPlan.Create")]
    public async Task<IActionResult> Create(LessonPlanUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) { await PopulateDropdownsAsync(ct); ViewBag.Action = "Create"; return View(dto); }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.CreateAsync(dto, userId);
        TempData["SuccessMessage"] = "Lesson Plan created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet, RequirePermission("LessonPlan.Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto is null) return NotFound();
        await PopulateDropdownsAsync(ct);
        ViewBag.Action = "Edit";
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken, RequirePermission("LessonPlan.Edit")]
    public async Task<IActionResult> Edit(LessonPlanUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) { await PopulateDropdownsAsync(ct); ViewBag.Action = "Edit"; return View(dto); }
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

    private async Task PopulateDropdownsAsync(CancellationToken ct)
    {
        var years = await _academicYearService.GetAllYearsAsync(ct);
        ViewBag.AcademicYears = new SelectList(years, "Id", "Name");

        var classes = await _classService.GetAllSchoolClassesAsync(ct);
        ViewBag.SchoolClasses = new SelectList(classes, "Id", "Name");

        var subjects = await _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.Subject>().QueryNoTracking()
            .Where(s => !s.IsDeleted && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(ct);
        ViewBag.Subjects = new SelectList(subjects, "Id", "Name");

        var teacherRepo = _uow.Repository<TeacherEntity>();
        var teachers = await teacherRepo.QueryNoTracking()
            .Include(t => t.Employee)
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.Employee!.FullName)
            .Select(t => new { t.Id, t.Employee!.FullName })
            .ToListAsync(ct);
        ViewBag.Teachers = new SelectList(teachers, "Id", "FullName");
    }
}
