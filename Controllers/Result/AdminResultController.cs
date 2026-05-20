using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Academic;

using SchoolManagementSystem.Constants;



namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class AdminResultController : Controller
{
    private readonly IResultAnalyticsService _analyticsService;
    private readonly IResultPublicationService _publicationService;
    private readonly IExamService _examService;
    private readonly IMeritCalculationService _meritCalculationService;
    private readonly ISubjectService _subjectService;

    public AdminResultController(
        IResultAnalyticsService analyticsService,
        IResultPublicationService publicationService,
        IExamService examService,
        IMeritCalculationService meritCalculationService,
        ISubjectService subjectService)
    {
        _analyticsService = analyticsService;
        _publicationService = publicationService;
        _examService = examService;
        _meritCalculationService = meritCalculationService;
        _subjectService = subjectService;
    }

    [HttpGet]

    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]

    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]

    public async Task<IActionResult> Dashboard()
    {
        var dashboardData = await _analyticsService.GetAdminDashboardAsync();
        if (dashboardData.ActiveYear == null)
        {
            ViewBag.Message = "No active academic year found.";
            return View();
        }

        ViewBag.ActiveYear = dashboardData.ActiveYear;
        ViewBag.Exams = dashboardData.Exams;
        ViewBag.ResultStats = dashboardData.ResultStats;

        return View();
    }

    [HttpGet]

    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]

    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]

    public async Task<IActionResult> AllSubjects(CancellationToken ct)
    {
        var groupedSubjects = await _subjectService.GetGroupedSubjectsAsync(ct);
        ViewBag.GroupedSubjects = groupedSubjects;
        return View();
    }

    [HttpGet]

    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]

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

    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]

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

    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]

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

    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]

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

    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]

    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]

    public async Task<IActionResult> ResultPublishing()
    {
        var resultPublications = await _publicationService.GetResultPublicationsAsync();
        return View(resultPublications);
    }

    [HttpPost]

    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]

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

    [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin + "," + Roles.Principal)]

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


