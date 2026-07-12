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
public class PromotionController : Controller
{
    private readonly IPromotionService _promotionService;
    private readonly IPromotionPolicyService _promotionPolicyService;
    private readonly IRollGenerationService _rollGenerationService;
    private readonly IAcademicYearService _academicYearService;
    private readonly ISchoolClassService _schoolClassService;

    public PromotionController(
        IPromotionService promotionService,
        IPromotionPolicyService promotionPolicyService,
        IRollGenerationService rollGenerationService,
        IAcademicYearService academicYearService,
        ISchoolClassService schoolClassService)
    {
        _promotionService = promotionService;
        _promotionPolicyService = promotionPolicyService;
        _rollGenerationService = rollGenerationService;
        _academicYearService = academicYearService;
        _schoolClassService = schoolClassService;
    }

    [HttpGet]
    [RequirePermission("Promotion.View")]
    public async Task<IActionResult> Index(int? academicYearId, CancellationToken ct = default)
    {
        var academicYears = await _academicYearService.GetAllYearsAsync(ct);
        var activeYear = academicYears.FirstOrDefault(x => x.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);
        var classesSorted = classes.Where(c => !c.IsDeleted && c.IsActive).OrderBy(c => c.SortOrder).ToList();

        var promotions = new List<object>();
        foreach (var cls in classesSorted)
        {
            var policies = await _promotionPolicyService.GetAllPromotionPoliciesAsync(yearId, ct);
            var classPolicy = policies.FirstOrDefault(p => p.SchoolClassId == cls.Id);

            var execution = yearId > 0 ? await _promotionPolicyService.GetPromotionExecutionAsync(yearId, cls.Id, ct) : null;

            promotions.Add(new
            {
                ClassId = cls.Id,
                ClassName = cls.Name,
                TotalStudents = execution?.TotalStudents ?? 0,
                PromotedCount = execution?.PromotedCount ?? 0,
                RepeatCount = execution?.RepeatCount ?? 0,
                FailedCount = execution?.FailedCount ?? 0,
                HasPolicy = classPolicy != null,
                PolicyName = classPolicy?.Name ?? "",
                HasExecution = execution != null,
                ExecutionDate = execution?.ExecutedAt.ToString("dd MMM yyyy") ?? ""
            });
        }

        ViewBag.AcademicYears = academicYears;
        ViewBag.SelectedYearId = yearId;
        ViewBag.ActiveYear = activeYear;
        ViewBag.Promotions = promotions;
        ViewBag.Classes = classesSorted;

        return View();
    }

    [HttpGet]
    [RequirePermission("Promotion.View")]
    public async Task<IActionResult> Evaluate(int classId, int academicYearId, CancellationToken ct = default)
    {
        var results = await _promotionPolicyService.EvaluateClassPromotionAsync(classId, academicYearId, ct);
        var cls = (await _schoolClassService.GetAllSchoolClassesAsync(ct)).FirstOrDefault(c => c.Id == classId);
        var year = (await _academicYearService.GetAllYearsAsync(ct)).FirstOrDefault(y => y.Id == academicYearId);
        var policy = await _promotionPolicyService.GetPromotionPolicyAsync(academicYearId, classId, ct);

        ViewBag.Class = cls;
        ViewBag.AcademicYear = year;
        ViewBag.Policy = policy;
        ViewBag.ClassId = classId;
        ViewBag.AcademicYearId = academicYearId;

        return View(results);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Execute")]
    public async Task<IActionResult> Execute(int classId, int academicYearId, CancellationToken ct = default)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var result = await _promotionPolicyService.ExecutePromotionAsync(classId, academicYearId, userId, ct);
            TempData["Success"] = $"Promotion executed successfully. Promoted: {result.PromotedCount}, Repeat: {result.RepeatCount}, Failed: {result.FailedCount}";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error executing promotion: {ex.Message}";
        }
        return RedirectToAction(nameof(Index), new { academicYearId });
    }

    [HttpGet]
    [RequirePermission("Promotion.View")]
    public async Task<IActionResult> History(int? studentId, int? classId, int? academicYearId, CancellationToken ct = default)
    {
        var academicYears = await _academicYearService.GetAllYearsAsync(ct);
        var activeYear = academicYears.FirstOrDefault(x => x.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);

        var history = await _promotionService.GetPromotionHistoryAsync(studentId, classId, yearId > 0 ? yearId : null, ct);

        ViewBag.AcademicYears = academicYears;
        ViewBag.SelectedYearId = yearId;
        ViewBag.Classes = classes;
        ViewBag.SelectedClassId = classId;
        ViewBag.SelectedStudentId = studentId;

        return View(history);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Reverse")]
    public async Task<IActionResult> Reverse(int promotionHistoryId, string reason, int? academicYearId, CancellationToken ct = default)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _promotionService.ReversePromotionAsync(promotionHistoryId, userId, reason);
            TempData["Success"] = "Promotion reversed successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error reversing promotion: {ex.Message}";
        }
        return RedirectToAction(nameof(History), new { academicYearId });
    }

    [HttpGet]
    [RequirePermission("Promotion.View")]
    public async Task<IActionResult> Policies(int? academicYearId, CancellationToken ct = default)
    {
        var academicYears = await _academicYearService.GetAllYearsAsync(ct);
        var activeYear = academicYears.FirstOrDefault(x => x.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        var policies = yearId > 0
            ? await _promotionPolicyService.GetAllPromotionPoliciesAsync(yearId, ct)
            : new List<SchoolManagementSystem.Models.Entities.Result.PromotionPolicy>();

        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);

        ViewBag.AcademicYears = academicYears;
        ViewBag.SelectedYearId = yearId;
        ViewBag.Classes = classes;

        return View(policies);
    }

    [HttpGet]
    [RequirePermission("Promotion.Manage")]
    public async Task<IActionResult> PolicyCreate(int? academicYearId, CancellationToken ct = default)
    {
        var academicYears = await _academicYearService.GetAllYearsAsync(ct);
        var activeYear = academicYears.FirstOrDefault(x => x.IsActive);
        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);

        ViewBag.AcademicYears = academicYears;
        ViewBag.SelectedYearId = academicYearId ?? activeYear?.Id ?? 0;
        ViewBag.Classes = classes;

        return View("PolicyForm");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Manage")]
    public async Task<IActionResult> PolicyCreate(SchoolManagementSystem.Models.Entities.Result.PromotionPolicy policy, string? criticalSubjects, CancellationToken ct = default)
    {
        try
        {
            policy.CriticalSubjectsJson = !string.IsNullOrEmpty(criticalSubjects)
                ? System.Text.Json.JsonSerializer.Serialize(criticalSubjects.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList())
                : null;

            await _promotionPolicyService.CreatePromotionPolicyAsync(policy, [], ct);
            TempData["Success"] = "Promotion policy created successfully.";
            return RedirectToAction(nameof(Policies), new { academicYearId = policy.AcademicYearId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error creating policy: {ex.Message}";
            var academicYears = await _academicYearService.GetAllYearsAsync(ct);
            var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);
            ViewBag.AcademicYears = academicYears;
            ViewBag.SelectedYearId = policy.AcademicYearId;
            ViewBag.Classes = classes;
            return View("PolicyForm", policy);
        }
    }

    [HttpGet]
    [RequirePermission("Promotion.Manage")]
    public async Task<IActionResult> PolicyEdit(int id, CancellationToken ct = default)
    {
        var policy = await _promotionPolicyService.GetPolicyByIdWithRulesAsync(id, ct);
        if (policy == null) return NotFound();

        var academicYears = await _academicYearService.GetAllYearsAsync(ct);
        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);

        ViewBag.AcademicYears = academicYears;
        ViewBag.SelectedYearId = policy.AcademicYearId;
        ViewBag.Classes = classes;
        ViewBag.IsEdit = true;

        return View("PolicyForm", policy);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Manage")]
    public async Task<IActionResult> PolicyEdit(SchoolManagementSystem.Models.Entities.Result.PromotionPolicy policy, string? criticalSubjects, CancellationToken ct = default)
    {
        try
        {
            policy.CriticalSubjectsJson = !string.IsNullOrEmpty(criticalSubjects)
                ? System.Text.Json.JsonSerializer.Serialize(criticalSubjects.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList())
                : null;

            await _promotionPolicyService.UpdatePromotionPolicyAsync(policy, [], ct);
            TempData["Success"] = "Promotion policy updated successfully.";
            return RedirectToAction(nameof(Policies), new { academicYearId = policy.AcademicYearId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error updating policy: {ex.Message}";
            var academicYears = await _academicYearService.GetAllYearsAsync(ct);
            var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);
            ViewBag.AcademicYears = academicYears;
            ViewBag.SelectedYearId = policy.AcademicYearId;
            ViewBag.Classes = classes;
            ViewBag.IsEdit = true;
            return View("PolicyForm", policy);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Manage")]
    public async Task<IActionResult> PolicyDelete(int id, int academicYearId, CancellationToken ct = default)
    {
        try
        {
            await _promotionPolicyService.DeletePromotionPolicyAsync(id, ct);
            TempData["Success"] = "Promotion policy deleted.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error deleting policy: {ex.Message}";
        }
        return RedirectToAction(nameof(Policies), new { academicYearId });
    }

    [HttpGet]
    [RequirePermission("Promotion.View")]
    public async Task<IActionResult> RollGeneration(int? academicYearId, int? classId, CancellationToken ct = default)
    {
        var academicYears = await _academicYearService.GetAllYearsAsync(ct);
        var activeYear = academicYears.FirstOrDefault(x => x.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);

        ViewBag.AcademicYears = academicYears;
        ViewBag.SelectedYearId = yearId;
        ViewBag.Classes = classes;
        ViewBag.SelectedClassId = classId;
        ViewBag.Results = new List<RollGenerationResult>();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Execute")]
    public async Task<IActionResult> RollGeneration(int academicYearId, int classId, SchoolManagementSystem.Models.Enums.RollGenerationStrategy strategy, CancellationToken ct = default)
    {
        try
        {
            await _rollGenerationService.SaveConfigAsync(academicYearId, classId, strategy, ct);
            var results = await _rollGenerationService.GenerateRollsAsync(academicYearId, classId, ct);

            var academicYears = await _academicYearService.GetAllYearsAsync(ct);
            var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);

            ViewBag.AcademicYears = academicYears;
            ViewBag.SelectedYearId = academicYearId;
            ViewBag.Classes = classes;
            ViewBag.SelectedClassId = classId;
            ViewBag.Results = results;
            TempData["Success"] = $"Roll numbers generated successfully for {results.Count} students.";

            return View();
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error generating rolls: {ex.Message}";
            return RedirectToAction(nameof(RollGeneration), new { academicYearId, classId });
        }
    }

    [HttpGet]
    [RequirePermission("Promotion.View")]
    public async Task<IActionResult> GroupAssignment(int? fromClassId, int? toClassId, int? academicYearId, CancellationToken ct = default)
    {
        var academicYears = await _academicYearService.GetAllYearsAsync(ct);
        var activeYear = academicYears.FirstOrDefault(x => x.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);

        ViewBag.AcademicYears = academicYears;
        ViewBag.SelectedYearId = yearId;
        ViewBag.Classes = classes;
        ViewBag.SelectedFromClassId = fromClassId;
        ViewBag.SelectedToClassId = toClassId;
        ViewBag.Results = new List<GroupAssignmentResult>();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Promotion.Execute")]
    public async Task<IActionResult> GroupAssignment(int fromClassId, int toClassId, int academicYearId, SchoolManagementSystem.Models.Enums.GroupAssignmentMethod method, CancellationToken ct = default)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var results = await _promotionPolicyService.AssignGroupsAsync(fromClassId, toClassId, academicYearId, userId, ct);

            var academicYears = await _academicYearService.GetAllYearsAsync(ct);
            var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);

            ViewBag.AcademicYears = academicYears;
            ViewBag.SelectedYearId = academicYearId;
            ViewBag.Classes = classes;
            ViewBag.SelectedFromClassId = fromClassId;
            ViewBag.SelectedToClassId = toClassId;
            ViewBag.Results = results;
            TempData["Success"] = $"Groups assigned for {results.Count} students.";

            return View();
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error assigning groups: {ex.Message}";
            return RedirectToAction(nameof(GroupAssignment), new { fromClassId, toClassId, academicYearId });
        }
    }

    [HttpGet]
    [RequirePermission("Promotion.View")]
    public async Task<IActionResult> GetClassStudentsJson(int classId, int academicYearId, CancellationToken ct)
    {
        var students = await _promotionService.GetClassStudentsJsonAsync(classId, ct);
        return Json(new { data = students });
    }
}
