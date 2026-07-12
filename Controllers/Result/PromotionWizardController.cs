using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Result;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
[RequirePermission("Promotion.Manage")]
public class PromotionWizardController : Controller
{
    private readonly IPromotionWizardService _wizardService;
    private readonly IAcademicYearService _academicYearService;
    private readonly ISchoolClassService _schoolClassService;

    public PromotionWizardController(
        IPromotionWizardService wizardService,
        IAcademicYearService academicYearService,
        ISchoolClassService schoolClassService)
    {
        _wizardService = wizardService;
        _academicYearService = academicYearService;
        _schoolClassService = schoolClassService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var years = await _academicYearService.GetAllYearsAsync(ct);
        var activeYear = years.FirstOrDefault(y => y.IsActive);
        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);
        var sortedClasses = classes.Where(c => !c.IsDeleted && c.IsActive).OrderBy(c => c.SortOrder).ToList();

        ViewBag.AcademicYears = years;
        ViewBag.ActiveYear = activeYear;
        ViewBag.Classes = sortedClasses;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetPreview([FromBody] PromotionWizardExecuteRequest request, CancellationToken ct = default)
    {
        try
        {
            var preview = await _wizardService.GetPreviewAsync(
                request.FromAcademicYearId, request.FromClassId, request.ToClassId, request.ExamId, ct);
            return Json(new { success = true, data = preview });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Execute([FromBody] PromotionWizardExecuteRequest request, CancellationToken ct = default)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var result = await _wizardService.ExecuteAsync(request, userId, ct);
            return Json(new { success = result.Success, data = result });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
