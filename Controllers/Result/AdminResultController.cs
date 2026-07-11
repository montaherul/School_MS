using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SchoolManagementSystem.Filters;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.ViewModels.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Data;
using System.Security.Claims;
using System.Text.Json;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class AdminResultController : Controller
{
    private readonly IResultAnalyticsService _analyticsService;
    private readonly IResultPublicationService _publicationService;
    private readonly IResultCalculationService _resultCalculationService;
    private readonly IExamService _examService;
    private readonly IMeritCalculationService _meritCalculationService;
    private readonly ISubjectService _subjectService;
    private readonly IResultPublicationRepository _publicationRepository;
    private readonly IStudentExamResultRepository _studentExamResultRepository;
    private readonly IUnitOfWork _uow;

    public AdminResultController(
        IResultAnalyticsService analyticsService,
        IResultPublicationService publicationService,
        IResultCalculationService resultCalculationService,
        IExamService examService,
        IMeritCalculationService meritCalculationService,
        ISubjectService subjectService,
        IResultPublicationRepository publicationRepository,
        IStudentExamResultRepository studentExamResultRepository,
        IUnitOfWork uow)
    {
        _analyticsService = analyticsService;
        _publicationService = publicationService;
        _resultCalculationService = resultCalculationService;
        _examService = examService;
        _meritCalculationService = meritCalculationService;
        _subjectService = subjectService;
        _publicationRepository = publicationRepository;
        _studentExamResultRepository = studentExamResultRepository;
        _uow = uow;
    }

    [HttpGet]
    [RequirePermission("Result.Dashboard")]
    public async Task<IActionResult> Dashboard(
        int? academicYearId,
        int? examId,
        int? classId,
        int? sectionId,
        int? groupId,
        CancellationToken ct = default)
    {
        var academicYears = await _uow.Repository<AcademicYear>().ListAsync(x => !x.IsDeleted, ct);
        var activeYear = academicYears.FirstOrDefault(x => x.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        var dashboardDto = await _analyticsService.GetAdminDashboardAsync();
        if (yearId > 0 && activeYear?.Id != yearId)
        {
            var year = academicYears.FirstOrDefault(y => y.Id == yearId);
            if (year != null)
            {
                dashboardDto.ActiveYear = year;
                dashboardDto.Exams = await _uow.Repository<ExamEntity>().Query()
                    .Where(e => e.AcademicYearId == yearId && !e.IsDeleted)
                    .ToListAsync(ct);
            }
        }

        // Use repository (which calls sp_GetResultSummary) for server-side aggregations
        List<dynamic> classPerf = new();
        List<dynamic> sectionPerf = new();
        List<dynamic> gradeDist = new();
        List<dynamic> passTrend = new();
        List<dynamic> topStudents = new();
        int passCount = 0, failCount = 0;

        if (examId.HasValue)
        {
            var classWise = await _studentExamResultRepository.GetClassWiseResultsAsync(examId.Value, ct);
            classPerf = classWise.Select(c => (dynamic)new { Label = c.ClassName, PassRate = c.TotalStudents > 0 ? Math.Round(100m * c.PassedCount / c.TotalStudents, 1) : 0m }).ToList();

            var gradeDistData = await _studentExamResultRepository.GetGradeDistributionAsync(examId.Value, ct);
            gradeDist = gradeDistData.Select(g => (dynamic)new { Grade = g.Grade, Count = g.Count }).ToList();

            var top = await _studentExamResultRepository.GetTopStudentsAsync(examId.Value, ct);
            topStudents = top.Select(t => (dynamic)new { Name = t.StudentName, Gpa = t.Gpa }).ToList();

            var summary = await _studentExamResultRepository.GetResultSummaryStatsAsync(examId.Value, ct);
            if (summary != null)
            {
                passCount = summary.PassedCount;
                failCount = summary.FailedCount;
            }
        }

        // Section + group level aggregations via EF (SP has group data but section-level is per-exam/filter)
        var resultsQuery = _uow.Repository<StudentExamResult>().Query()
            .Include(r => r.Student).ThenInclude(s => s.Class)
            .Include(r => r.Student).ThenInclude(s => s.Section)
            .Include(r => r.Student).ThenInclude(s => s.StudentGroup)
            .Include(r => r.Exam)
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.Exam.AcademicYearId == yearId);

        if (examId.HasValue) resultsQuery = resultsQuery.Where(r => r.ExamId == examId.Value);
        if (classId.HasValue) resultsQuery = resultsQuery.Where(r => r.Student.ClassId == classId.Value);
        if (sectionId.HasValue) resultsQuery = resultsQuery.Where(r => r.Student.SectionId == sectionId.Value);
        if (groupId.HasValue) resultsQuery = resultsQuery.Where(r => r.Student.StudentGroupId == groupId.Value);

        var results = await resultsQuery.ToListAsync(ct);

        if (!classPerf.Any())
        {
            classPerf = results.GroupBy(r => r.Student.Class?.Name ?? "Unknown")
                .Select(g => (dynamic)new { Label = g.Key, PassRate = g.Any() ? Math.Round(100m * g.Count(x => x.IsPassed) / g.Count(), 1) : 0m })
                .OrderBy(x => x.Label).ToList();
        }

        sectionPerf = results.GroupBy(r => r.Student.Section?.Name ?? "Unknown")
            .Select(g => (dynamic)new { Label = g.Key, PassRate = g.Any() ? Math.Round(100m * g.Count(x => x.IsPassed) / g.Count(), 1) : 0m })
            .OrderBy(x => x.Label).ToList();

        if (!gradeDist.Any())
        {
            gradeDist = results.GroupBy(r => string.IsNullOrEmpty(r.Grade) ? "N/A" : r.Grade)
                .Select(g => (dynamic)new { Grade = g.Key, Count = g.Count() })
                .OrderBy(x => x.Grade).ToList();
        }

        passTrend = results.GroupBy(r => r.Exam?.Name ?? "Exam")
            .Select(g => (dynamic)new { Label = g.Key, PassRate = g.Any() ? Math.Round(100m * g.Count(x => x.IsPassed) / g.Count(), 1) : 0m })
            .OrderBy(x => x.Label).ToList();

        if (!topStudents.Any())
        {
            topStudents = results.OrderByDescending(r => r.Gpa).Take(10)
                .Select(r => (dynamic)new { Name = r.Student.FullName, Gpa = r.Gpa }).ToList();
        }

        if (!examId.HasValue || passCount == 0 && failCount == 0)
        {
            passCount = results.Count(r => r.IsPassed);
            failCount = results.Count(r => !r.IsPassed);
        }

        var groupPerf = results.Where(r => r.Student.StudentGroup != null)
            .GroupBy(r => r.Student.StudentGroup!.Name)
            .Select(g => (dynamic)new { Label = g.Key, PassRate = g.Any() ? Math.Round(100m * g.Count(x => x.IsPassed) / g.Count(), 1) : 0m })
            .OrderBy(x => x.Label).ToList();

        var subjectPerf = new List<dynamic>();

        var groups = await _uow.Repository<StudentGroup>().ListAsync(g => !g.IsDeleted, ct);
        var chartDataJson = JsonSerializer.Serialize(new
        {
            classPerf,
            sectionPerf,
            groupPerf,
            subjectPerf,
            gradeDist,
            passTrend,
            topStudents,
            passCount,
            failCount
        });

        var vm = new ResultDashboardViewModel
        {
            ActiveYear = dashboardDto.ActiveYear,
            Exams = dashboardDto.Exams,
            ResultStats = dashboardDto.ResultStats,
            AcademicYears = academicYears.ToList(),
            FilterExams = (await _examService.GetExamsAsync(yearId)).ToList(),
            Groups = groups.ToList(),
            SelectedAcademicYearId = yearId,
            SelectedExamId = examId,
            SelectedGroupId = groupId,
            ChartDataJson = chartDataJson
        };

        return View(vm);
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> AllSubjects(CancellationToken ct)
    {
        var groupedSubjects = await _subjectService.GetGroupedSubjectsAsync(ct);
        var dict = new Dictionary<string, List<SchoolManagementSystem.Models.DTOs.Academic.SubjectListItemDto>>();
        foreach (var kvp in groupedSubjects)
            dict[kvp.Key] = kvp.Value.ToList();
        var allSubjects = dict.SelectMany(g => g.Value).ToList();
        var model = new AllSubjectsPageViewModel
        {
            Subjects = allSubjects,
            GroupedSubjects = dict
        };
        return View(model);
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> AllResults(int? examId, int? classId, string? status, CancellationToken ct)
    {
        var rawClasses = await _examService.GetClassesAsync(ct);
        var classes = rawClasses.Select(c => new IdNamePairDto
        {
            Id = (int)c.GetType().GetProperty("Id")!.GetValue(c)!,
            Name = (string)c.GetType().GetProperty("Name")!.GetValue(c)!
        }).ToList();
        var model = new AllResultsPageViewModel
        {
            Exams = await _examService.GetExamsAsync(0),
            Classes = classes,
            SelectedExamId = examId,
            SelectedClassId = classId,
            SelectedStatus = status
        };
        return View(model);
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> GetResultListJson(
        int page = 1,
        int size = 20,
        int? examId = null,
        int? classId = null,
        int? sectionId = null,
        int? studentGroupId = null,
        int? status = null,
        string? searchTerm = null,
        int? academicYearId = null,
        CancellationToken ct = default)
    {
        var (items, totalCount) = await _studentExamResultRepository.GetResultListAsync(
            examId, classId, sectionId, studentGroupId, status,
            searchTerm, page, size, ct, academicYearId);

        return Json(new
        {
            data = items,
            last_page = size > 0 ? (int)Math.Ceiling((double)totalCount / size) : 1,
            total_records = totalCount
        });
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> TabulationSheet(int examId, int? classId, int? sectionId, CancellationToken ct)
    {
        var tabulationSheet = await _analyticsService.GetTabulationSheetAsync(examId, classId, sectionId);
        if (tabulationSheet == null) return NotFound("Exam not found");

        ViewBag.Classes = await _examService.GetClassesAsync(ct);
        ViewBag.Sections = await _examService.GetSectionsAsync(classId, ct);

        return View(tabulationSheet);
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public IActionResult MeritLists(int examId)
    {
        return RedirectToAction("Index", "MeritList", new { examId });
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> SubjectAnalysis(int examId, CancellationToken ct)
    {
        var exam = await _examService.GetExamByIdAsync(examId, ct) as ExamEntity;
        if (exam == null) return NotFound();
        var subjectAnalysis = await _analyticsService.GetSubjectAnalysisAsync(examId);
        var model = new SubjectAnalysisPageViewModel
        {
            Subjects = subjectAnalysis,
            Exam = exam
        };
        return View(model);
    }

    [HttpGet]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> ResultPublishing(int? academicYearId, CancellationToken ct)
    {
        var academicYears = await _uow.Repository<AcademicYear>().ListAsync(x => !x.IsDeleted, ct);
        var activeYear = academicYears.FirstOrDefault(x => x.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        PublicationDashboardSummaryDto? summary = null;
        List<PublicationDashboardExamDto> exams;
        List<PublicationHistoryEntryDto> history = new();

        if (yearId > 0)
        {
            var (examsData, summaryData) = await _publicationRepository.GetPublicationDashboardAsync(yearId, ct);
            exams = examsData.ToList();
            summary = summaryData;

            var rawHistory = await _publicationRepository.Query()
                .Include(p => p.Exam)
                .Where(p => !p.IsDeleted && p.Exam.AcademicYearId == yearId)
                .OrderByDescending(p => p.PublishedAt ?? p.UpdatedAt ?? p.CreatedAt)
                .Take(50)
                .Select(p => new PublicationHistoryEntryDto
                {
                    Timestamp = (p.PublishedAt ?? p.UpdatedAt ?? p.CreatedAt).ToString("dd MMM yyyy HH:mm"),
                    Action = p.Status.ToString(),
                    PerformedBy = p.UpdatedBy ?? p.CreatedBy ?? "System",
                    Notes = p.IsLocked ? "Results locked" : ""
                })
                .ToListAsync(ct);
            history = rawHistory;
        }
        else
        {
            exams = new List<PublicationDashboardExamDto>();
        }

        var model = new ResultPublishingPageViewModel
        {
            Exams = exams,
            Summary = summary,
            History = history,
            AcademicYears = academicYears.ToList(),
            SelectedYearId = yearId,
            ActiveYear = yearId > 0 ? academicYears.FirstOrDefault(y => y.Id == yearId) : activeYear
        };

        return View(model);
    }

    [HttpGet]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> ReviewResults(int examId, int? classId, int? sectionId, int? groupId, CancellationToken ct)
    {
        var exam = await _uow.Repository<ExamEntity>().Query()
            .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted, ct);

        if (exam == null) return NotFound();

        var academicYear = await _uow.Repository<AcademicYear>().GetByIdAsync(exam.AcademicYearId, ct);

        var (results, _) = await _studentExamResultRepository.GetResultListAsync(
            examId, classId, sectionId, groupId, (int)ResultWorkflowStatus.Submitted, null, 1, 2000, ct);

        var model = new ReviewResultsPageViewModel
        {
            ExamName = exam.Name,
            ExamId = examId,
            AcademicYearName = academicYear?.Name ?? "",
            ClassId = classId,
            SectionId = sectionId,
            GroupId = groupId,
            Results = results
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> PublishResults([FromBody] PublishRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var rows = await _uow.ExecuteSqlRawAsync(
                "EXEC sp_PublishResults @ExamId, @AcademicYearId, @PublishedByUserId, @LockResults, @Remarks",
                new SqlParameter("@ExamId", request.ExamId),
                new SqlParameter("@AcademicYearId", await GetAcademicYearIdForExam(request.ExamId)),
                new SqlParameter("@PublishedByUserId", userId),
                new SqlParameter("@LockResults", request.LockResults ? 1 : 0),
                new SqlParameter("@Remarks", (object?)request.Remarks ?? DBNull.Value));

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> UnpublishResults([FromBody] PublishRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var rows = await _uow.ExecuteSqlRawAsync(
                "EXEC sp_UnpublishResults @ExamId, @UnpublishedByUserId, @Reason",
                new SqlParameter("@ExamId", request.ExamId),
                new SqlParameter("@UnpublishedByUserId", userId),
                new SqlParameter("@Reason", (object?)request.Remarks ?? "Unpublish requested"));

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private async Task<int> GetAcademicYearIdForExam(int examId)
    {
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId);
        return exam?.AcademicYearId ?? 0;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> RepublishResults([FromBody] PublishRequest request)
    {
        try
        {
            await _publicationService.RepublishResultsAsync(request.ExamId);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Approve")]
    public async Task<IActionResult> ApproveResults([FromBody] PublishRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _publicationService.ApproveReviewedResultsAsync(request.ExamId, userId);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> RejectResults([FromBody] RejectRequest request)
    {
        try
        {
            var rows = await _uow.ExecuteSqlRawAsync(
                "UPDATE StudentExamResults SET Status = @Status, UpdatedAt = GETUTCDATE(), UpdatedBy = @UpdatedBy WHERE ExamId = @ExamId AND IsDeleted = 0",
                new SqlParameter("@Status", (int)ResultWorkflowStatus.Draft),
                new SqlParameter("@UpdatedBy", User.Identity?.Name ?? "admin"),
                new SqlParameter("@ExamId", request.ExamId));
            return Json(new { success = true, affected = rows });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    public class PublishRequest
    {
        public int ExamId { get; set; }
        public bool LockResults { get; set; } = true;
        public string? Remarks { get; set; }
    }

    public class RejectRequest
    {
        public int ExamId { get; set; }
        public List<int>? StudentIds { get; set; }
        public string? Notes { get; set; }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Recalculate")]
    public async Task<IActionResult> RecalculateResults(int examId)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var academicYearId = await GetAcademicYearIdForExam(examId);
            await _uow.ExecuteSqlRawAsync(
                "EXEC sp_RecalculateResults @ExamId, @AcademicYearId, @RecalculatedByUserId, @Reason",
                new SqlParameter("@ExamId", examId),
                new SqlParameter("@AcademicYearId", academicYearId),
                new SqlParameter("@RecalculatedByUserId", userId),
                new SqlParameter("@Reason", "Recalculation triggered from admin dashboard"));
            return Json(new { success = true, message = "Results recalculated successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Recalculate")]
    public async Task<IActionResult> RecalculateMeritPositions(int examId)
    {
        try
        {
            await _uow.ExecuteSqlRawAsync(
                "EXEC sp_CalculateMerit @ExamGroupKey",
                new SqlParameter("@ExamGroupKey", await GetExamName(examId)));
            TempData["Success"] = "Merit positions recalculated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error recalculating merit positions: {ex.Message}";
        }
        return RedirectToAction("Dashboard");
    }

    private async Task<string> GetExamName(int examId)
    {
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId);
        return exam?.Name ?? "";
    }
}
