using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.ViewModels.Result;

public class ExamIndexViewModel
{
    public List<ExamUpsertDto> Exams { get; set; } = new();
    public int CurrentAcademicYearId { get; set; }
}

public class MarkEntryViewModel
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public List<StudentMarkViewModel> Students { get; set; } = new();
}

public class StudentMarkViewModel
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public decimal? MarksObtained { get; set; }
    public string? Grade { get; set; }
    public bool IsLocked { get; set; }
}

public class ResultReviewViewModel
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public ResultWorkflowStatus Status { get; set; }
    public List<ClassResultSummaryViewModel> ClassSummaries { get; set; } = new();
}

public class ClassResultSummaryViewModel
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int SubmittedStudents { get; set; }
    public decimal AverageGpa { get; set; }
    public ResultWorkflowStatus Status { get; set; }
}

public class StudentPortalResultViewModel
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public List<StudentExamResultDto> ExamResults { get; set; } = new();
    public FinalResultDto? FinalResult { get; set; }
}

public class ReEvaluationDashboardViewModel
{
    public List<ReEvaluationRequestViewModel> PendingRequests { get; set; } = new();
    public List<ReEvaluationRequestViewModel> CompletedRequests { get; set; } = new();
}

public class ReEvaluationRequestViewModel
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public decimal OldMarks { get; set; }
    public decimal? NewMarks { get; set; }
    public ReEvaluationStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
