using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Exam;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

/// <summary>
/// ExamAdminController: Admin-only exam management
/// Controls exam creation, editing, grading rules, and exam configuration
/// </summary>
[Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
[Route("api/[controller]")]
[ApiController]
public class ExamAdminController : ControllerBase
{
    private readonly IExamService _examService;
    private readonly IResultPublicationService _publicationService;
    private readonly IMeritCalculationService _meritCalculationService;
    private readonly IAcademicYearService _academicYearService;
    private readonly IExamValidationService _examValidationService;
    private readonly ISubjectMarkStructureService _markStructureService;
    private readonly IResultCalculationService _resultCalculationService;
    private readonly IExamSubjectService _examSubjectService;
    private readonly ILogger<ExamAdminController> _logger;

    public ExamAdminController(
        IExamService examService,
        IResultPublicationService publicationService,
        IMeritCalculationService meritCalculationService,
        IAcademicYearService academicYearService,
        IExamValidationService examValidationService,
        ISubjectMarkStructureService markStructureService,
        IResultCalculationService resultCalculationService,
        IExamSubjectService examSubjectService,
        ILogger<ExamAdminController> logger)
    {
        _examService = examService;
        _publicationService = publicationService;
        _meritCalculationService = meritCalculationService;
        _academicYearService = academicYearService;
        _examValidationService = examValidationService;
        _markStructureService = markStructureService;
        _resultCalculationService = resultCalculationService;
        _examSubjectService = examSubjectService;
        _logger = logger;
    }

    /// <summary>
    /// Get all exams with optional filtering by academic year
    /// </summary>
    [HttpGet("exams")]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> GetAllExams(int academicYearId = 0, CancellationToken ct = default)
    {
        try
        {
            var exams = await _examService.GetExamsAsync(academicYearId);
            return Ok(new { success = true, data = exams });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching exams");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Create a new exam (Admin only)
    /// </summary>
    [HttpPost("create-exam")]
    public async Task<IActionResult> CreateExam([FromBody] ExamUpsertDto dto, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Validation failed: {errors}" });
            }

            if (dto.Subjects != null && dto.Subjects.Count > 0)
                await _examValidationService.ThrowIfSubjectMarkStructureMissingAsync(dto.Subjects.Select(s => s.SubjectId).ToList(), ct);

            var hasMultipleClasses = dto.SelectedClassIds != null && dto.SelectedClassIds.Count > 1;

            if (hasMultipleClasses)
            {
                var results = await _examService.CreateExamsBulkAsync(dto, ct);
                _logger.LogInformation("Bulk exam creation completed: {Count} exams", results.Count);
                return Ok(new { success = true, message = $"{results.Count} exams created successfully", data = results });
            }

            var exam = await _examService.CreateExamAsync(dto, ct);
            _logger.LogInformation("Exam created successfully");
            return Ok(new { success = true, message = "Exam created successfully", data = exam });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating exam");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Update exam details (Admin only)
    /// </summary>
    [HttpPut("update-exam/{id}")]
    public async Task<IActionResult> UpdateExam(int id, [FromBody] ExamUpsertDto dto, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Validation failed: {errors}" });
            }

            if (dto.Subjects != null && dto.Subjects.Count > 0)
                await _examValidationService.ThrowIfSubjectMarkStructureMissingAsync(dto.Subjects.Select(s => s.SubjectId).ToList(), ct);

            var exam = await _examService.UpdateExamAsync(id, dto, ct);
            _logger.LogInformation($"Exam updated: {id}");
            return Ok(new { success = true, message = "Exam updated successfully", data = exam });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating exam");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Delete exam (Admin only)
    /// </summary>
    [HttpDelete("delete-exam/{id}")]
    public async Task<IActionResult> DeleteExam(int id, CancellationToken ct = default)
    {
        try
        {
            await _examService.DeleteExamAsync(id, ct);
            _logger.LogInformation($"Exam deleted: {id}");
            return Ok(new { success = true, message = "Exam deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting exam");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get all grading rules
    /// </summary>
    [HttpGet("grading-rules")]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> GetGradingRules(CancellationToken ct = default)
    {
        try
        {
            var rules = await _examService.GetGradingRulesAsync(ct);
            return Ok(new { success = true, data = rules });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching grading rules");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Create or update grading rules (Admin only)
    /// </summary>
    [HttpPost("upsert-grading-rule")]
    public async Task<IActionResult> UpsertGradingRule([FromBody] GradingRuleUpsertDto dto, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Validation failed: {errors}" });
            }

            var rule = await _examService.UpsertGradingRuleAsync(dto, ct);
            _logger.LogInformation("Grading rule saved successfully");
            return Ok(new { success = true, message = "Grading rule saved successfully", data = rule });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving grading rule");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Delete grading rule (Admin only)
    /// </summary>
    [HttpDelete("delete-grading-rule/{id}")]
    public async Task<IActionResult> DeleteGradingRule(int id, CancellationToken ct = default)
    {
        try
        {
            await _examService.DeleteGradingRuleAsync(id, ct);
            _logger.LogInformation($"Grading rule deleted: {id}");
            return Ok(new { success = true, message = "Grading rule deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting grading rule");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Copy subject structure from one exam to sibling exams
    /// </summary>
    [HttpPost("copy-subjects/{sourceExamId}")]
    public async Task<IActionResult> CopySubjects(int sourceExamId, [FromBody] List<int> targetExamIds, CancellationToken ct = default)
    {
        try
        {
            var copied = await _examSubjectService.CopySubjectStructureAsync(sourceExamId, targetExamIds);
            _logger.LogInformation("Subject structure copied: {Count} subjects from exam {SourceId}", copied, sourceExamId);
            return Ok(new { success = true, message = $"{copied} subjects copied successfully", data = new { copied } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying subject structure");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Lock exam from further modifications (Admin only)
    /// </summary>
    [HttpPost("lock-exam/{examId}")]
    public async Task<IActionResult> LockExam(int examId, [FromBody] string? reason = null, CancellationToken ct = default)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _examService.LockExamAsync(examId, userId, reason, ct);
            _logger.LogInformation($"Exam locked: {examId} by user {userId}");
            return Ok(new { success = true, message = "Exam locked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking exam");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Unlock exam (Admin only)
    /// </summary>
    [HttpPost("unlock-exam/{examId}")]
    public async Task<IActionResult> UnlockExam(int examId, [FromBody] string? reason = null, CancellationToken ct = default)
    {
        try
        {
            await _examService.UnlockExamAsync(examId, reason, ct);
            _logger.LogInformation($"Exam unlocked: {examId}");
            return Ok(new { success = true, message = "Exam unlocked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking exam");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get exam status including result publication status
    /// </summary>
    [HttpGet("exam-status/{examId}")]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> GetExamStatus(int examId, CancellationToken ct = default)
    {
        try
        {
            var status = await _examService.GetExamStatusAsync(examId, ct);
            return Ok(new { success = true, data = status });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching exam status");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Calculate and publish merit positions for exam (Admin only)
    /// </summary>
    [HttpPost("calculate-merit/{examId}")]
    public async Task<IActionResult> CalculateMerit(int examId, CancellationToken ct = default)
    {
        try
        {
            await _meritCalculationService.RecalculateMeritPositionsAsync(examId);
            _logger.LogInformation($"Merit positions calculated for exam: {examId}");
            return Ok(new { success = true, message = "Merit positions calculated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating merit positions");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Publish exam results — delegated to AdminResultController
    /// </summary>
    [HttpPost("publish-results/{examId}")]
    public async Task<IActionResult> PublishResults(int examId, [FromBody] ResultPublishDto dto)
    {
        try
        {
            dto.ExamId = examId;
            await _publicationService.PublishResultsAsync(dto);
            _logger.LogInformation($"Results published for exam: {examId}");
            return Ok(new { success = true, message = "Results published successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing results");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("review-results/{examId}")]
    public async Task<IActionResult> ReviewResults(int examId)
    {
        try
        {
            await _publicationService.ReviewExamResultsAsync(examId, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"));
            _logger.LogInformation($"Exam reviewed: {examId}");
            return Ok(new { success = true, message = "Exam results reviewed." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reviewing results");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("approve-results/{examId}")]
    public async Task<IActionResult> ApproveResults(int examId)
    {
        try
        {
            await _publicationService.ApproveReviewedResultsAsync(examId, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"));
            _logger.LogInformation($"Exam approved: {examId}");
            return Ok(new { success = true, message = "Exam results approved." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving results");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("unpublish-results/{examId}")]
    public async Task<IActionResult> UnpublishResults(int examId)
    {
        try
        {
            await _publicationService.UnpublishResultsAsync(examId);
            _logger.LogInformation($"Exam unpublished: {examId}");
            return Ok(new { success = true, message = "Exam results unpublished." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpublishing results");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("republish-results/{examId}")]
    public async Task<IActionResult> RepublishResults(int examId)
    {
        try
        {
            await _publicationService.RepublishResultsAsync(examId);
            _logger.LogInformation($"Exam republished: {examId}");
            return Ok(new { success = true, message = "Exam results republished." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error republishing results");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get subjects for exam configuration
    /// </summary>
    [HttpGet("subjects")]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> GetSubjects(CancellationToken ct = default)
    {
        try
        {
            var subjects = await _examService.GetSubjectsAsync(ct);
            return Ok(new { success = true, data = subjects });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching subjects");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get classes for exam setup
    /// </summary>
    [HttpGet("classes")]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> GetClasses(CancellationToken ct = default)
    {
        try
        {
            var classes = await _examService.GetClassesAsync(ct);
            return Ok(new { success = true, data = classes });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching classes");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get sections for a class
    /// </summary>
    [HttpGet("sections/{classId}")]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> GetSections(int classId, CancellationToken ct = default)
    {
        try
        {
            var sections = await _examService.GetSectionsAsync(classId, ct);
            return Ok(new { success = true, data = sections });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sections");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get component preview for selected subjects (read-only display for exam wizard)
    /// </summary>
    [HttpPost("component-preview")]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> GetComponentPreview([FromBody] List<int> subjectIds, CancellationToken ct = default)
    {
        try
        {
            var previews = await _markStructureService.GetComponentPreviewsAsync(subjectIds, ct);
            return Ok(new { success = true, data = previews });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching component preview");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Generate FinalResult records for all students in an academic year
    /// </summary>
    [HttpPost("generate-final-results/{academicYearId}")]
    public async Task<IActionResult> GenerateFinalResults(int academicYearId, CancellationToken ct = default)
    {
        try
        {
            var result = await _resultCalculationService.GenerateFinalResultsAsync(academicYearId);
            _logger.LogInformation($"FinalResults generated for academic year {academicYearId}: {result.GeneratedCount} generated, {result.UpdatedCount} updated");
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating final results");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get academic years for exam filtering
    /// </summary>
    [HttpGet("academic-years")]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> GetAcademicYears(CancellationToken ct = default)
    {
        try
        {
            var result = await _academicYearService.GetPagedAsync(1, 100, null, ct);
            return Ok(new { success = true, data = result.Items });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching academic years");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
