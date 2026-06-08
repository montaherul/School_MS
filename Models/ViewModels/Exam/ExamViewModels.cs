using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Models.ViewModels.Exam;

public class AcademicYearOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class ExamFilterOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ExamGroupPerformanceViewModel
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class ExamListViewModel
{
    public IReadOnlyList<ExamListDto> Exams { get; set; } = [];
    public int SelectedAcademicYearId { get; set; }
    public string SelectedAcademicYearName { get; set; } = string.Empty;
    public IReadOnlyList<AcademicYearOptionViewModel> AcademicYears { get; set; } = [];
}

public class ExamDashboardViewModel
{
    public ExamDashboardDto Stats { get; set; } = new();
    public int SelectedAcademicYearId { get; set; }
    public string ActiveAcademicYearName { get; set; } = string.Empty;
    public IReadOnlyList<AcademicYearOptionViewModel> AcademicYears { get; set; } = [];
    public IReadOnlyList<ExamListDto> YearExams { get; set; } = [];
    public IReadOnlyList<ExamListDto> RecentExams { get; set; } = [];
    public int? SelectedExamId { get; set; }
    public int? SelectedClassId { get; set; }
    public int? SelectedSectionId { get; set; }
    public int? SelectedGroupId { get; set; }
    public IReadOnlyList<ExamFilterOptionViewModel> Groups { get; set; } = [];
    public IReadOnlyList<ExamFilterOptionViewModel> Classes { get; set; } = [];
    public IReadOnlyList<ExamFilterOptionViewModel> Sections { get; set; } = [];
    public IReadOnlyList<ExamStatusDistributionDto> StatusDistribution { get; set; } = [];
    public IReadOnlyList<ExamPassRateDto> PassRates { get; set; } = [];
    public IReadOnlyList<ExamGroupPerformanceViewModel> GroupPerformance { get; set; } = [];
    public string StatusDistributionJson { get; set; } = "[]";
    public string PassRateLabelsJson { get; set; } = "[]";
    public string PassRateDataJson { get; set; } = "[]";
    public string GroupPerformanceLabelsJson { get; set; } = "[]";
    public string GroupPerformanceDataJson { get; set; } = "[]";
}

public class ExamCreateEditViewModel
{
    public bool IsEdit { get; set; }
    public int? ExamId { get; set; }
    public ExamUpsertDto Exam { get; set; } = new();
    public string ExamDataJson { get; set; } = "null";
}

public class ExamDetailsViewModel
{
    public ExamDetailsDto Exam { get; set; } = new();
}
