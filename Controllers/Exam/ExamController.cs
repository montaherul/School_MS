using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.ViewModels.Exam;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Base;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Models.Enums;
using System.Text.Json;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Controllers.Exam;

[Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
public class ExamController : GenericCrudController<ExamEntity>
{
    private readonly IExamRepository _examRepository;
    private readonly IExamService _examService;
    private readonly IUnitOfWork _uow;

    public ExamController(
        IBaseService<ExamEntity> service,
        IExamRepository examRepository,
        IExamService examService,
        IUnitOfWork uow) : base(service, "Exam")
    {
        _examRepository = examRepository;
        _examService = examService;
        _uow = uow;
    }

    public override async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        int? academicYearId = int.TryParse(Request.Query["academicYearId"], out var parsedYearId) ? parsedYearId : null;

        var academicYears = await _uow.Repository<AcademicYear>().ListAsync(x => !x.IsDeleted, cancellationToken);
        var activeYear = academicYears.FirstOrDefault(ay => ay.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        var exams = yearId > 0
            ? (await _examService.GetExamsAsync(yearId, cancellationToken)).ToList()
            : (await _examService.GetExamsAsync(0, cancellationToken)).ToList();

        var model = new ExamListViewModel
        {
            Exams = exams,
            SelectedAcademicYearId = yearId,
            SelectedAcademicYearName = academicYears.FirstOrDefault(y => y.Id == yearId)?.Name
                ?? activeYear?.Name ?? string.Empty,
            AcademicYears = academicYears.Select(ExamViewModelMapper.ToOption).ToList()
        };

        return View(model);
    }

    public override IActionResult Create()
        => View("Create", new ExamCreateEditViewModel());

    public override async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var exam = await _examService.GetExamForEditAsync(id, cancellationToken);
        if (exam == null) return NotFound();

        return View("Create", ExamViewModelMapper.ToCreateEditViewModel(exam, isEdit: true));
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard(
        int? academicYearId,
        int? examId,
        int? classId,
        int? sectionId,
        int? groupId,
        CancellationToken ct)
    {
        var academicYears = await _uow.Repository<AcademicYear>().ListAsync(x => !x.IsDeleted, ct);
        var activeYear = academicYears.FirstOrDefault(ay => ay.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        var stats = await _examRepository.GetDashboardDataAsync(yearId, ct);
        var statusDistribution = await _examRepository.GetStatusDistributionAsync(yearId, ct);
        var passRates = await _examRepository.GetExamPassRatesAsync(yearId, ct);
        var yearExams = (await _examRepository.GetExamsForAdminAsync(yearId, ct)).ToList();

        var recentExams = yearExams
            .OrderByDescending(e => e.CreatedAt)
            .Take(10)
            .ToList();

        var groups = await _uow.Repository<StudentGroup>().ListAsync(g => !g.IsDeleted, ct);
        var classes = await _uow.Repository<SchoolClass>().ListAsync(c => !c.IsDeleted, ct);
        var sections = await _uow.Repository<Section>().ListAsync(s => !s.IsDeleted, ct);

        var resultsQuery = _uow.Repository<StudentExamResult>().Query()
            .Include(r => r.Student).ThenInclude(s => s.StudentGroup)
            .Include(r => r.Exam)
            .Where(r => !r.IsDeleted && r.Exam.AcademicYearId == yearId);

        if (examId.HasValue) resultsQuery = resultsQuery.Where(r => r.ExamId == examId.Value);
        if (classId.HasValue) resultsQuery = resultsQuery.Where(r => r.Student.ClassId == classId.Value);
        if (sectionId.HasValue) resultsQuery = resultsQuery.Where(r => r.Student.SectionId == sectionId.Value);
        if (groupId.HasValue) resultsQuery = resultsQuery.Where(r => r.Student.StudentGroupId == groupId.Value);

        var groupPerformance = await resultsQuery
            .Where(r => r.Student.StudentGroupId != null)
            .GroupBy(r => r.Student.StudentGroup!.Name)
            .Select(g => new ExamGroupPerformanceViewModel
            {
                Label = g.Key,
                Value = g.Any() ? Math.Round(g.Average(x => x.Gpa), 2) : 0m
            })
            .OrderBy(x => x.Label)
            .ToListAsync(ct);

        var model = new ExamDashboardViewModel
        {
            Stats = stats,
            SelectedAcademicYearId = yearId,
            ActiveAcademicYearName = academicYears.FirstOrDefault(y => y.Id == yearId)?.Name ?? string.Empty,
            AcademicYears = academicYears.Select(ExamViewModelMapper.ToOption).ToList(),
            YearExams = yearExams,
            RecentExams = recentExams,
            SelectedExamId = examId,
            SelectedClassId = classId,
            SelectedSectionId = sectionId,
            SelectedGroupId = groupId,
            Groups = groups.Select(ExamViewModelMapper.ToFilterOption).ToList(),
            Classes = classes.Select(ExamViewModelMapper.ToClassFilterOption).ToList(),
            Sections = sections.Select(ExamViewModelMapper.ToSectionFilterOption).ToList(),
            StatusDistribution = statusDistribution,
            PassRates = passRates,
            GroupPerformance = groupPerformance,
            StatusDistributionJson = JsonSerializer.Serialize(statusDistribution),
            PassRateLabelsJson = JsonSerializer.Serialize(passRates.Select(x => x.ExamName)),
            PassRateDataJson = JsonSerializer.Serialize(passRates.Select(x => x.PassPercentage)),
            GroupPerformanceLabelsJson = JsonSerializer.Serialize(groupPerformance.Select(x => x.Label)),
            GroupPerformanceDataJson = JsonSerializer.Serialize(groupPerformance.Select(x => x.Value))
        };

        return View(model);
    }

    public override async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
    {
        var dto = await _examService.GetExamDetailsAsync(id, cancellationToken);
        if (dto == null) return NotFound();

        return View(new ExamDetailsViewModel { Exam = dto });
    }

    public override async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var dto = await _examService.GetExamDetailsAsync(id, cancellationToken);
        if (dto == null) return NotFound();

        return View(new ExamDetailsViewModel { Exam = dto });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _examService.DeleteExamAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Exam deleted successfully.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}
