using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Result;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class PromotioSessionController : Controller
{
    private readonly IPromotioSessionService _sessionService;
    private readonly IAcademicYearService _academicYearService;
    private readonly ISchoolClassService _schoolClassService;

    public PromotioSessionController(
        IPromotioSessionService sessionService,
        IAcademicYearService academicYearService,
        ISchoolClassService schoolClassService)
    {
        _sessionService = sessionService;
        _academicYearService = academicYearService;
        _schoolClassService = schoolClassService;
    }

    [HttpGet]
    [RequirePermission("Promotion.View")]
    public async Task<IActionResult> Index(int? academicYearId, CancellationToken ct = default)
    {
        var years = await _academicYearService.GetAllYearsAsync(ct);
        ViewBag.AcademicYears = years;
        ViewBag.SelectedYearId = academicYearId ?? years.FirstOrDefault(y => y.IsActive)?.Id ?? 0;
        return View();
    }

    [HttpGet]
    [RequirePermission("Promotion.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 20, string? search = null, string? status = null, CancellationToken ct = default)
    {
        var result = await _sessionService.GetPagedAsync(page, size, search, status, ct);
        return Json(new { items = result.Items, total = result.TotalItems, page, size });
    }

    [HttpGet]
    [RequirePermission("Promotion.Manage")]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        var years = await _academicYearService.GetAllYearsAsync(ct);
        ViewBag.AcademicYears = years.Where(y => !y.IsDeleted && y.IsActive).ToList();
        return View("CreateEdit", new PromotioSessionUpsertDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Manage")]
    public async Task<IActionResult> Create(PromotioSessionUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AcademicYears = (await _academicYearService.GetAllYearsAsync(ct)).Where(y => !y.IsDeleted && y.IsActive).ToList();
            return View("CreateEdit", dto);
        }
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var id = await _sessionService.CreateAsync(dto, userId, ct);
            TempData["Success"] = "Promotion session created successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
            ViewBag.AcademicYears = (await _academicYearService.GetAllYearsAsync(ct)).Where(y => !y.IsDeleted && y.IsActive).ToList();
            return View("CreateEdit", dto);
        }
    }

    [HttpGet]
    [RequirePermission("Promotion.Manage")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
    {
        var dto = await _sessionService.GetForEditAsync(id, ct);
        if (dto == null) return NotFound();
        ViewBag.AcademicYears = (await _academicYearService.GetAllYearsAsync(ct)).Where(y => !y.IsDeleted && y.IsActive).ToList();
        return View("CreateEdit", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Manage")]
    public async Task<IActionResult> Edit(PromotioSessionUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AcademicYears = (await _academicYearService.GetAllYearsAsync(ct)).Where(y => !y.IsDeleted && y.IsActive).ToList();
            return View("CreateEdit", dto);
        }
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _sessionService.UpdateAsync(dto, userId, ct);
            TempData["Success"] = "Promotion session updated.";
            return RedirectToAction(nameof(Details), new { id = dto.Id });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
            ViewBag.AcademicYears = (await _academicYearService.GetAllYearsAsync(ct)).Where(y => !y.IsDeleted && y.IsActive).ToList();
            return View("CreateEdit", dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _sessionService.DeleteAsync(id, userId, ct);
            TempData["Success"] = "Session deleted.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Promotion.View")]
    public async Task<IActionResult> Details(int id, CancellationToken ct = default)
    {
        var session = await _sessionService.GetSessionWithAcademicYearAsync(id, ct);
        if (session == null) return NotFound();

        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);
        ViewBag.Classes = classes.Where(c => !c.IsDeleted && c.IsActive).OrderBy(c => c.SortOrder).ToList();
        ViewBag.Session = session;

        var dashboard = await _sessionService.GetDashboardAsync(session.AcademicYearId, ct);
        ViewBag.Dashboard = dashboard;

        return View();
    }

    [HttpGet]
    [RequirePermission("Promotion.View")]
    public async Task<IActionResult> GetCandidates(int classId, int academicYearId, string? search, CancellationToken ct = default)
    {
        var candidates = await _sessionService.GetCandidatesAsync(classId, academicYearId, search, ct);
        return Json(new { items = candidates, total = candidates.Count > 0 ? candidates[0].TotalRecords : 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Execute")]
    public async Task<IActionResult> BulkPromote(int sessionId, int fromClassId, int toClassId, int academicYearId, CancellationToken ct = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var result = await _sessionService.BulkPromoteAsync(sessionId, fromClassId, toClassId, academicYearId, userId, ct);
            TempData["Success"] = $"Promoted {result.PromotedCount} students successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }
        return RedirectToAction(nameof(Details), new { id = sessionId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Reverse")]
    public async Task<IActionResult> Rollback(int sessionId, CancellationToken ct = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _sessionService.RollbackSessionAsync(sessionId, userId, ct);
            TempData["Success"] = "Session rolled back successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }
        return RedirectToAction(nameof(Details), new { id = sessionId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Execute")]
    public async Task<IActionResult> Approve(int sessionId, CancellationToken ct = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _sessionService.ApproveSessionAsync(sessionId, userId, ct);
            TempData["Success"] = "Session approved.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }
        return RedirectToAction(nameof(Details), new { id = sessionId });
    }

    [HttpGet]
    [RequirePermission("Promotion.View")]
    public async Task<IActionResult> ProgressionRules(CancellationToken ct = default)
    {
        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);
        ViewBag.Classes = classes.Where(c => !c.IsDeleted && c.IsActive).OrderBy(c => c.SortOrder).ToList();
        var rules = await _sessionService.GetProgressionRulesAsync(ct);
        return View(rules);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Manage")]
    public async Task<IActionResult> ProgressionRules(List<ClassProgressionRuleUpsertDto> rules, CancellationToken ct = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _sessionService.UpdateProgressionRulesAsync(rules, userId, ct);
            TempData["Success"] = "Progression rules updated.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }
        return RedirectToAction(nameof(ProgressionRules));
    }

    [HttpGet]
    [RequirePermission("Reports.View")]
    public async Task<IActionResult> Reports(int? academicYearId, CancellationToken ct = default)
    {
        var years = await _academicYearService.GetAllYearsAsync(ct);
        ViewBag.AcademicYears = years;
        ViewBag.SelectedYearId = academicYearId ?? years.FirstOrDefault(y => y.IsActive)?.Id ?? 0;
        return View();
    }

    [HttpGet]
    [RequirePermission("Reports.View")]
    public async Task<IActionResult> Register(int sessionId, CancellationToken ct = default)
    {
        var register = await _sessionService.GetPromotioRegisterAsync(sessionId, ct);
        return View(register);
    }

    [HttpGet]
    [RequirePermission("Reports.View")]
    public async Task<IActionResult> FailedStudents(int academicYearId, int? classId, CancellationToken ct = default)
    {
        var data = await _sessionService.GetFailedStudentsAsync(academicYearId, classId, ct);
        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);
        ViewBag.Classes = classes.Where(c => !c.IsDeleted && c.IsActive).OrderBy(c => c.SortOrder).ToList();
        ViewBag.AcademicYearId = academicYearId;
        ViewBag.SelectedClassId = classId;
        return View(data);
    }

    [HttpGet]
    [RequirePermission("Promotion.View")]
    public async Task<IActionResult> Dashboard(int? academicYearId, CancellationToken ct = default)
    {
        var years = await _academicYearService.GetAllYearsAsync(ct);
        var activeYear = years.FirstOrDefault(y => y.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;
        var dashboard = yearId > 0 ? await _sessionService.GetDashboardAsync(yearId, ct) : new PromotioDashboardDto();
        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);

        ViewBag.AcademicYears = years;
        ViewBag.SelectedYearId = yearId;
        ViewBag.Dashboard = dashboard;
        ViewBag.Classes = classes.Where(c => !c.IsDeleted && c.IsActive).OrderBy(c => c.SortOrder).ToList();
        return View();
    }
}
