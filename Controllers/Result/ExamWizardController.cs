using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Result;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
[RequirePermission("Exam.Manage")]
public class ExamWizardController : Controller
{
    private readonly IExamWizardService _wizardService;
    private readonly IAcademicYearService _academicYearService;
    private readonly ISchoolClassService _classService;
    private readonly IExamService _examService;
    private readonly ILogger<ExamWizardController> _logger;

    public ExamWizardController(
        IExamWizardService wizardService,
        IAcademicYearService academicYearService,
        ISchoolClassService classService,
        IExamService examService,
        ILogger<ExamWizardController> logger)
    {
        _wizardService = wizardService;
        _academicYearService = academicYearService;
        _classService = classService;
        _examService = examService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        ViewBag.AcademicYears = await _academicYearService.GetAllYearsAsync(ct);
        ViewBag.Classes = await _classService.GetAllSchoolClassesAsync(ct);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoadSubjects([FromBody] ExamWizardStep1Request request, CancellationToken ct = default)
    {
        if (request.SelectedClassIds == null || request.SelectedClassIds.Count == 0)
            return Json(new { success = false, message = "Please select at least one class." });

        try
        {
            var subjects = await _wizardService.LoadSubjectsAsync(
                request.AcademicYearId, request.SelectedClassIds, request.Term, ct);

            return Json(new { success = true, subjects, subjectCount = subjects.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load subjects for exam wizard");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetExamCreationPreview([FromBody] ExamWizardPreviewRequest request, CancellationToken ct = default)
    {
        if (request.SelectedClassIds == null || request.SelectedClassIds.Count == 0)
            return Json(new { success = false, message = "Please select at least one class." });

        try
        {
            var preview = await _wizardService.GetExamCreationPreviewAsync(
                request.AcademicYearId, request.SelectedClassIds, ct);

            return Json(new { success = true, preview });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get exam creation preview");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ValidateExamCreation([FromBody] ExamValidationRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.ExamName))
            return Json(new { success = false, message = "Exam name is required." });

        if (request.SelectedClassIds == null || request.SelectedClassIds.Count == 0)
            return Json(new { success = false, message = "Please select at least one class." });

        try
        {
            var validation = await _wizardService.ValidateExamCreationAsync(
                request.AcademicYearId, request.ExamName, request.Term,
                request.SelectedClassIds, request.StartDate, request.EndDate, ct);

            return Json(new { success = true, validation });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate exam creation");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExamHierarchy([FromBody] ExamCreateHierarchyRequest request, CancellationToken ct = default)
    {
        if (request.ClassIds == null || request.ClassIds.Count == 0)
            return Json(new { success = false, message = "No classes selected." });

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            request.UserId = userId;
            var result = await _wizardService.CreateExamHierarchyAsync(request, userId, ct);
            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exam hierarchy creation failed");
            return Json(new ExamCreateResultDto
            {
                Success = false,
                Message = $"Failed to create exam hierarchy: {ex.Message}"
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckPublishReadiness([FromBody] CheckPublishReadinessRequest request, CancellationToken ct = default)
    {
        try
        {
            var readiness = await _wizardService.CheckExamPublishReadinessAsync(request.ExamId, ct);
            return Json(new { success = true, readiness });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check publish readiness");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateSchedule([FromBody] GenerateScheduleRequest request, CancellationToken ct = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var result = await _wizardService.GenerateExamScheduleAsync(request.ExamId, request.StartDate, request.EndDate, userId, ct);
            return Json(new { success = true, result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate schedule");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetConflicts([FromBody] GetConflictsRequest request, CancellationToken ct = default)
    {
        try
        {
            var conflicts = await _wizardService.GetExamConflictsAsync(request.ExamId, ct);
            return Json(new { success = true, conflicts });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get conflicts");
            return Json(new { success = false, message = ex.Message });
        }
    }

    // Fix Issues endpoints
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignTeacher([FromBody] AssignTeacherRequest request, CancellationToken ct = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var result = await _wizardService.AssignTeacherToExamSubjectAsync(
                request.AcademicYearId, request.SubjectId, request.ClassId,
                request.SectionId, request.StudentGroupId, request.TeacherId, userId, ct);
            return Json(new { success = result.Success, message = result.Message, assignmentId = result.AssignmentId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign teacher");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfigureComponents([FromBody] ConfigureComponentsRequest request, CancellationToken ct = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var result = await _wizardService.ConfigureExamSubjectComponentsAsync(request.ExamSubjectId, request.ComponentsJson, userId, ct);
            return Json(new { success = result.Success, message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to configure components");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSections([FromBody] AddSectionsRequest request, CancellationToken ct = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var result = await _wizardService.AddSectionsToClassAsync(request.ClassId, request.SectionNamesJson, request.StudentGroupId, userId, ct);
            return Json(new { success = result.Success, message = result.Message, createdCount = result.CreatedCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add sections");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MapSubject([FromBody] MapSubjectRequest request, CancellationToken ct = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var result = await _wizardService.MapSubjectToClassAsync(
                request.SubjectId, request.ClassId, request.StudentGroupId,
                request.FullMarks, request.PassMarks, request.IsOptional,
                request.DisplayOrder, userId, ct);
            return Json(new { success = result.Success, message = result.Message, createdCount = result.CreatedCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to map subject");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfigureMarkStructure([FromBody] ConfigureMarkStructureRequest request, CancellationToken ct = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var result = await _wizardService.ConfigureSubjectMarkStructureAsync(
                request.SubjectId, request.ClassId, request.StudentGroupId,
                request.ComponentsJson, userId, ct);
            return Json(new { success = result.Success, message = result.Message, createdCount = result.CreatedCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to configure mark structure");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoadPreviousTemplate([FromBody] ExamWizardTemplateRequest request, CancellationToken ct = default)
    {
        try
        {
            var template = await _wizardService.LoadPreviousExamTemplateAsync(request.AcademicYearId, request.Term, ct);
            if (template == null)
                return Json(new { success = false, message = "No previous exam found for this term." });

            return Json(new { success = true, template });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load previous exam template");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] ExamWizardCreateRequest request, CancellationToken ct = default)
    {
        if (request.SelectedClassIds == null || request.SelectedClassIds.Count == 0)
            return Json(new { success = false, message = "No classes selected." });

        if (request.Subjects == null || request.Subjects.Count == 0)
            return Json(new { success = false, message = "No subjects configured." });

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var result = await _wizardService.CreateExamsFromWizardAsync(request, userId, ct);
            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exam wizard creation failed");
            return Json(new ExamWizardResultDto
            {
                Success = false,
                Message = $"Failed to create exam(s): {ex.Message}"
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetExamSources([FromBody] ExamWizardSourceRequest request, CancellationToken ct = default)
    {
        try
        {
            var exams = await _examService.GetExamsAsync(request.AcademicYearId, ct);
            var filtered = exams
                .Where(e => e.Term == request.Term)
                .Select(e => new { e.Id, e.Name, e.Term, e.Status, AcademicYearId = request.AcademicYearId })
                .ToList();

            return Json(new { success = true, exams = filtered });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get exam sources");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoadNctbTemplate([FromBody] LoadNctbTemplateRequest request, CancellationToken ct = default)
    {
        try
        {
            var template = await _wizardService.LoadNctbTemplateAsync(
                request.AcademicYearId, request.ClassId, request.Term, ct);

            if (template == null)
                return Json(new { success = false, message = "No NCTB template available for the selected class and group." });

            return Json(new { success = true, template });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load NCTB template");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveTemplate([FromBody] SchoolManagementSystem.Models.DTOs.Result.SaveTemplateRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Json(new { success = false, message = "Template name is required." });

        if (request.Subjects == null || request.Subjects.Count == 0)
            return Json(new { success = false, message = "No subjects to save." });

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var result = await _wizardService.SaveTemplateAsync(request, userId, ct);
            return Json(new { success = true, template = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save exam template");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoadTemplate([FromBody] TemplateLoadRequest request, CancellationToken ct = default)
    {
        try
        {
            var template = await _wizardService.LoadTemplateAsync(request.TemplateId, ct);
            if (template == null)
                return Json(new { success = false, message = "Template not found." });

            return Json(new { success = true, template });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load template");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ListTemplates([FromBody] TemplateListRequest request, CancellationToken ct = default)
    {
        try
        {
            var templates = await _wizardService.ListTemplatesAsync(request.AcademicYearId, request.Term, ct);
            return Json(new { success = true, templates });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list templates");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTemplate([FromBody] TemplateDeleteRequest request, CancellationToken ct = default)
    {
        try
        {
            var success = await _wizardService.DeleteTemplateAsync(request.TemplateId, ct);
            return Json(new { success, message = success ? "Template deleted." : "Template not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete template");
            return Json(new { success = false, message = ex.Message });
        }
    }
}