using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;
using System.Text.Json;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class AdminResultController : Controller
{
    private readonly IResultAnalyticsService _analyticsService;
    private readonly IResultPublicationService _publicationService;
    private readonly IExamService _examService;
    private readonly IMeritCalculationService _meritCalculationService;
    private readonly ISubjectService _subjectService;
    private readonly IResultPublicationRepository _publicationRepository;
    private readonly IStudentExamResultRepository _studentExamResultRepository;
    private readonly IUnitOfWork _uow;

    public AdminResultController(
        IResultAnalyticsService analyticsService,
        IResultPublicationService publicationService,
        IExamService examService,
        IMeritCalculationService meritCalculationService,
        ISubjectService subjectService,
        IResultPublicationRepository publicationRepository,
        IStudentExamResultRepository studentExamResultRepository,
        IUnitOfWork uow)
    {
        _analyticsService = analyticsService;
        _publicationService = publicationService;
        _examService = examService;
        _meritCalculationService = meritCalculationService;
        _subjectService = subjectService;
        _publicationRepository = publicationRepository;
        _studentExamResultRepository = studentExamResultRepository;
        _uow = uow;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
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

        var dashboardData = await _analyticsService.GetAdminDashboardAsync();
        if (yearId > 0 && activeYear?.Id != yearId)
        {
            var year = academicYears.FirstOrDefault(y => y.Id == yearId);
            if (year != null)
            {
                dashboardData.ActiveYear = year;
                dashboardData.Exams = await _uow.Repository<ExamEntity>().Query()
                    .Where(e => e.AcademicYearId == yearId && !e.IsDeleted)
                    .ToListAsync(ct);
            }
        }

        var resultsQuery = _uow.Repository<StudentExamResult>().Query()
            .Include(r => r.Student).ThenInclude(s => s.Class)
            .Include(r => r.Student).ThenInclude(s => s.Section)
            .Include(r => r.Student).ThenInclude(s => s.StudentGroup)
            .Include(r => r.Exam)
            .Where(r => !r.IsDeleted && r.Exam.AcademicYearId == yearId);

        if (examId.HasValue) resultsQuery = resultsQuery.Where(r => r.ExamId == examId.Value);
        if (classId.HasValue) resultsQuery = resultsQuery.Where(r => r.Student.ClassId == classId.Value);
        if (sectionId.HasValue) resultsQuery = resultsQuery.Where(r => r.Student.SectionId == sectionId.Value);
        if (groupId.HasValue) resultsQuery = resultsQuery.Where(r => r.Student.StudentGroupId == groupId.Value);

        var results = await resultsQuery.ToListAsync(ct);

        var classPerf = results.GroupBy(r => r.Student.Class?.Name ?? "Unknown")
            .Select(g => new { Label = g.Key, PassRate = g.Any() ? Math.Round(100m * g.Count(x => x.IsPassed) / g.Count(), 1) : 0m })
            .OrderBy(x => x.Label).ToList();

        var sectionPerf = results.GroupBy(r => r.Student.Section?.Name ?? "Unknown")
            .Select(g => new { Label = g.Key, PassRate = g.Any() ? Math.Round(100m * g.Count(x => x.IsPassed) / g.Count(), 1) : 0m })
            .OrderBy(x => x.Label).ToList();

        var groupPerf = results.Where(r => r.Student.StudentGroup != null)
            .GroupBy(r => r.Student.StudentGroup!.Name)
            .Select(g => new { Label = g.Key, PassRate = g.Any() ? Math.Round(100m * g.Count(x => x.IsPassed) / g.Count(), 1) : 0m })
            .OrderBy(x => x.Label).ToList();

        var subjectResults = await _uow.Repository<StudentSubjectResult>().Query()
            .Include(s => s.Subject)
            .Include(s => s.Exam)
            .Where(s => !s.IsDeleted && s.Exam.AcademicYearId == yearId)
            .Where(s => !examId.HasValue || s.ExamId == examId.Value)
            .ToListAsync(ct);

        var subjectPerf = subjectResults.GroupBy(s => s.Subject?.Name ?? "Unknown")
            .Select(g => new { Label = g.Key, PassRate = g.Any() ? Math.Round(100m * g.Count(x => x.IsPassed) / g.Count(), 1) : 0m })
            .OrderByDescending(x => x.PassRate).Take(12).ToList();

        var gradeDist = results.GroupBy(r => string.IsNullOrEmpty(r.Grade) ? "N/A" : r.Grade)
            .Select(g => new { Grade = g.Key, Count = g.Count() })
            .OrderBy(x => x.Grade).ToList();

        var passTrend = results.GroupBy(r => r.Exam?.Name ?? "Exam")
            .Select(g => new { Label = g.Key, PassRate = g.Any() ? Math.Round(100m * g.Count(x => x.IsPassed) / g.Count(), 1) : 0m })
            .OrderBy(x => x.Label).ToList();

        var topStudents = results.OrderByDescending(r => r.Gpa).Take(10)
            .Select(r => new { Name = r.Student.FullName, Gpa = r.Gpa }).ToList();

        ViewBag.ActiveYear = dashboardData.ActiveYear;
        ViewBag.Exams = dashboardData.Exams;
        ViewBag.ResultStats = dashboardData.ResultStats;
        ViewBag.AcademicYears = academicYears;
        ViewBag.SelectedYearId = yearId;
        ViewBag.SelectedExamId = examId;
        ViewBag.SelectedClassId = classId;
        ViewBag.SelectedSectionId = sectionId;
        ViewBag.SelectedGroupId = groupId;
        ViewBag.FilterExams = await _examService.GetExamsAsync(yearId);
        ViewBag.Classes = await _examService.GetClassesAsync(ct);
        ViewBag.Sections = classId.HasValue ? await _examService.GetSectionsAsync(classId, ct) : Enumerable.Empty<object>();
        ViewBag.Groups = await _uow.Repository<StudentGroup>().ListAsync(g => !g.IsDeleted, ct);
        ViewBag.ChartData = JsonSerializer.Serialize(new
        {
            classPerf,
            sectionPerf,
            groupPerf,
            subjectPerf,
            gradeDist,
            passTrend,
            topStudents,
            passCount = results.Count(r => r.IsPassed),
            failCount = results.Count(r => !r.IsPassed)
        });

        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> AllSubjects(CancellationToken ct)
    {
        var groupedSubjects = await _subjectService.GetGroupedSubjectsAsync(ct);
        ViewBag.GroupedSubjects = groupedSubjects;
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> AllResults(int? examId, int? classId, string? status, CancellationToken ct)
    {
        var results = await _publicationService.GetAllResultsAsync(examId, classId, status);
        ViewBag.Exams = await _examService.GetExamsAsync(0);
        ViewBag.Classes = await _examService.GetClassesAsync(ct);
        ViewBag.SelectedExamId = examId;
        ViewBag.SelectedClassId = classId;
        ViewBag.SelectedStatus = status;
        return View(results);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> TabulationSheet(int examId, int? classId, int? sectionId, CancellationToken ct)
    {
        var tabulationSheet = await _analyticsService.GetTabulationSheetAsync(examId, classId, sectionId);
        if (tabulationSheet == null) return NotFound("Exam not found");

        ViewBag.Classes = await _examService.GetClassesAsync(ct);
        ViewBag.Sections = await _examService.GetSectionsAsync(classId, ct);

        ViewBag.SelectedClassId = classId;
        ViewBag.SelectedSectionId = sectionId;
        return View(tabulationSheet);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> MeritLists(int examId, CancellationToken ct)
    {
        var exam = await _examService.GetExamByIdAsync(examId, ct);
        if (exam == null) return NotFound();

        ViewBag.Exam = exam;
        ViewBag.ClassMerit = (await _meritCalculationService.GetMeritListAsync(examId, MeritCategory.Class)).Take(50).ToList();
        ViewBag.SectionMerit = (await _meritCalculationService.GetMeritListAsync(examId, MeritCategory.Section)).Take(50).ToList();
        ViewBag.GroupMerit = (await _meritCalculationService.GetMeritListAsync(examId, MeritCategory.Group)).Take(50).ToList();
        ViewBag.SchoolMerit = (await _meritCalculationService.GetMeritListAsync(examId, MeritCategory.School)).Take(50).ToList();

        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> SubjectAnalysis(int examId, CancellationToken ct)
    {
        var exam = await _examService.GetExamByIdAsync(examId, ct);
        if (exam == null) return NotFound();
        var subjectAnalysis = await _analyticsService.GetSubjectAnalysisAsync(examId);
        ViewBag.Exam = exam;
        return View(subjectAnalysis);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> ResultPublishing(int? academicYearId, CancellationToken ct)
    {
        var academicYears = await _uow.Repository<AcademicYear>().ListAsync(x => !x.IsDeleted, ct);
        var activeYear = academicYears.FirstOrDefault(x => x.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        if (yearId == 0)
        {
            ViewBag.AcademicYears = academicYears;
            return View(Enumerable.Empty<PublicationDashboardExamDto>());
        }

        var (exams, summary) = await _publicationRepository.GetPublicationDashboardAsync(yearId, ct);

        var history = await _publicationRepository.Query()
            .Include(p => p.Exam)
            .Where(p => !p.IsDeleted && p.Exam.AcademicYearId == yearId)
            .OrderByDescending(p => p.PublishedAt ?? p.UpdatedAt ?? p.CreatedAt)
            .Take(50)
            .Select(p => new
            {
                Timestamp = (p.PublishedAt ?? p.UpdatedAt ?? p.CreatedAt).ToString("dd MMM yyyy HH:mm"),
                Action = p.Status.ToString(),
                PerformedBy = p.UpdatedBy ?? p.CreatedBy ?? "System",
                Notes = p.IsLocked ? "Results locked" : ""
            })
            .ToListAsync(ct);

        ViewBag.Summary = summary;
        ViewBag.PublicationHistory = history;
        ViewBag.AcademicYears = academicYears;
        ViewBag.SelectedYearId = yearId;
        ViewBag.ActiveYear = academicYears.FirstOrDefault(y => y.Id == yearId);

        return View(exams);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> ReviewResults(int examId, int? classId, int? sectionId, int? groupId, CancellationToken ct)
    {
        var exam = await _uow.Repository<ExamEntity>().Query()
            .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted, ct);

        if (exam == null) return NotFound();

        var academicYear = await _uow.Repository<AcademicYear>().GetByIdAsync(exam.AcademicYearId, ct);

        ViewBag.ExamName = exam.Name;
        ViewBag.ExamId = examId;
        ViewBag.AcademicYearName = academicYear?.Name ?? "";
        ViewBag.ClassId = classId;
        ViewBag.SectionId = sectionId;
        ViewBag.GroupId = groupId;

        var (results, _) = await _studentExamResultRepository.GetResultListAsync(
            examId, classId, sectionId, groupId, (int)ResultWorkflowStatus.Submitted, null, 1, 2000, ct);

        return View(results);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> PublishResults([FromBody] PublishRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var dto = new ResultPublishDto
            {
                ExamId = request.ExamId,
                LockResults = request.LockResults,
                ApprovedByUserId = userId
            };
            await _publicationService.PublishResultsAsync(dto);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> UnpublishResults([FromBody] PublishRequest request)
    {
        try
        {
            await _publicationService.UnpublishResultsAsync(request.ExamId);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
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
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    [IgnoreAntiforgeryToken]
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
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> RejectResults([FromBody] RejectRequest request)
    {
        try
        {
            var query = _uow.Repository<StudentExamResult>().Query()
                .Where(r => r.ExamId == request.ExamId && !r.IsDeleted);

            if (request.StudentIds?.Any() == true)
                query = query.Where(r => request.StudentIds.Contains(r.StudentId));

            var results = await query.ToListAsync();
            foreach (var r in results)
            {
                r.Status = ResultWorkflowStatus.Draft;
                r.UpdatedAt = DateTime.UtcNow;
                r.UpdatedBy = User.Identity?.Name ?? "admin";
                _uow.Repository<StudentExamResult>().Update(r);
            }
            await _uow.SaveChangesAsync();
            return Json(new { success = true });
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
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    public async Task<IActionResult> RecalculateResults(int examId)
    {
        try
        {
            await _publicationService.RecalculateResultsAsync(examId);
            TempData["Success"] = "Results recalculated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error recalculating results: {ex.Message}";
        }
        return RedirectToAction("Dashboard");
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    public async Task<IActionResult> RecalculateMeritPositions(int examId)
    {
        try
        {
            await _publicationService.RecalculateMeritPositionsAsync(examId);
            TempData["Success"] = "Merit positions recalculated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error recalculating merit positions: {ex.Message}";
        }
        return RedirectToAction("Dashboard");
    }
}
