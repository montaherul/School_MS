using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class AcademicAnalyticsController : Controller
{
    private readonly IResultAnalyticsService _analyticsService;
    private readonly IUnitOfWork _uow;

    public AcademicAnalyticsController(IResultAnalyticsService analyticsService, IUnitOfWork uow)
    {
        _analyticsService = analyticsService;
        _uow = uow;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var dashboardData = await _analyticsService.GetAdminDashboardAsync();
        ViewBag.ActiveYear = dashboardData.ActiveYear;
        ViewBag.Exams = dashboardData.Exams;
        ViewBag.ResultStats = dashboardData.ResultStats;

        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> SubjectPerformance(int examId, CancellationToken ct)
    {
        var exam = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().GetByIdAsync(examId);
        if (exam == null) return NotFound("Exam not found");

        var subjectAnalysis = await _analyticsService.GetSubjectAnalysisAsync(examId);
        ViewBag.Exam = exam;

        return View(subjectAnalysis);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> TabulationSheet(int? examId, int? classId, int? sectionId, CancellationToken ct)
    {
        var exams = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => !x.IsDeleted);
        var classes = await _uow.Repository<SchoolClass>().ListAsync(x => !x.IsDeleted);

        ViewBag.Exams = exams;
        ViewBag.Classes = classes;
        ViewBag.SelectedExamId = examId;
        ViewBag.SelectedClassId = classId;
        ViewBag.SelectedSectionId = sectionId;

        if (classId.HasValue)
        {
            var sections = await _uow.Repository<Section>().ListAsync(x => x.SchoolClassId == classId.Value && !x.IsDeleted);
            ViewBag.Sections = sections;
        }

        if (examId.HasValue && examId > 0)
        {
            var tabulationSheet = await _analyticsService.GetTabulationSheetAsync(examId.Value, classId, sectionId);
            return View(tabulationSheet);
        }

        return View(new SchoolManagementSystem.Models.DTOs.Result.TabulationSheetDto());
    }
}
