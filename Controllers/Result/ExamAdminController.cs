using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;
<<<<<<< HEAD
using SchoolManagementSystem.Constants;
=======
>>>>>>> d8b24e6 (attendece and website curtomize)

namespace SchoolManagementSystem.Controllers.Result;

/// <summary>
/// ExamAdminController: Admin-only exam management
/// Controls exam creation, editing, grading rules, and exam configuration
/// </summary>
<<<<<<< HEAD
[Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]
=======
[Authorize(Roles = "Admin,Super Admin,Principal")]
>>>>>>> d8b24e6 (attendece and website curtomize)
[Route("api/[controller]")]
[ApiController]
public class ExamAdminController : ControllerBase
{
    private readonly IExamService _examService;
    private readonly IResultPublicationService _publicationService;
    private readonly IMeritCalculationService _meritCalculationService;
    private readonly IAcademicYearService _academicYearService;
    private readonly ILogger<ExamAdminController> _logger;

    public ExamAdminController(
        IExamService examService,
        IResultPublicationService publicationService,
        IMeritCalculationService meritCalculationService,
        IAcademicYearService academicYearService,
        ILogger<ExamAdminController> logger)
    {
        _examService = examService;
        _publicationService = publicationService;
        _meritCalculationService = meritCalculationService;
        _academicYearService = academicYearService;
        _logger = logger;
    }

    /// <summary>
    /// Get all exams with optional filtering by academic year
    /// </summary>
    [HttpGet("exams")]
<<<<<<< HEAD
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]
=======
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
>>>>>>> d8b24e6 (attendece and website curtomize)
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
                return BadRequest(ModelState);

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
                return BadRequest(ModelState);

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
<<<<<<< HEAD
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]
=======
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
>>>>>>> d8b24e6 (attendece and website curtomize)
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
                return BadRequest(ModelState);

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
<<<<<<< HEAD
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]
=======
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
>>>>>>> d8b24e6 (attendece and website curtomize)
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
    /// Publish exam results (Admin only)
    /// </summary>
    [HttpPost("publish-results/{examId}")]
    public async Task<IActionResult> PublishResults(int examId, [FromBody] ResultPublishDto dto, CancellationToken ct = default)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Set exam ID and user ID in dto before publishing
            dto.ExamId = examId;
            await _publicationService.PublishResultsAsync(dto);
            _logger.LogInformation($"Results published for exam: {examId}");
            return Ok(new { success = true, message = "Results published successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing results");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get subjects for exam configuration
    /// </summary>
    [HttpGet("subjects")]
<<<<<<< HEAD
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]
=======
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
>>>>>>> d8b24e6 (attendece and website curtomize)
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
<<<<<<< HEAD
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]
=======
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
>>>>>>> d8b24e6 (attendece and website curtomize)
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
<<<<<<< HEAD
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]
=======
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
>>>>>>> d8b24e6 (attendece and website curtomize)
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
    /// Get academic years for exam filtering
    /// </summary>
    [HttpGet("academic-years")]
<<<<<<< HEAD
    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]
=======
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
>>>>>>> d8b24e6 (attendece and website curtomize)
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
