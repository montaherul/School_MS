using SchoolManagementSystem.Models.Entities.Academic;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Teacher;

namespace SchoolManagementSystem.Models.ViewModels.Result;

public class ExamIndexViewModel
{
    public List<ExamListDto> Exams { get; set; } = new();
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
    // Component-wise marks (optional)
    public decimal? WrittenMarks { get; set; }
    public decimal? MCQMarks { get; set; }
    public decimal? CQMarks { get; set; }
    public decimal? PracticalMarks { get; set; }
    public decimal? VivaMarks { get; set; }
    public decimal? LabMarks { get; set; }
    public decimal? OralMarks { get; set; }
    public decimal? AssignmentMarks { get; set; }
    public decimal? ContinuousAssessmentMarks { get; set; }

    public decimal? CompetencyMarks { get; set; }
    public decimal? BehaviourMarks { get; set; }
    public decimal? ParticipationMarks { get; set; }

    // Dynamic component values for components not mapped to standard fields
    public Dictionary<string, decimal?> ComponentValues { get; set; } = new();

    // Teacher who entered the marks
    public int? EnteredByTeacherId { get; set; }
    public string? EnteredByTeacherName { get; set; }
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
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public List<StudentExamResultDto> ExamResults { get; set; } = new();
    public FinalResultDto? FinalResult { get; set; }
    public StudentTranscriptDto? Transcript { get; set; }
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

public class MarksIndexViewModel
{
    public List<SchoolManagementSystem.Models.Entities.Exam.Exam> Exams { get; set; } = new();
    public List<SchoolManagementSystem.Models.Entities.Academic.SchoolClass> Classes { get; set; } = new();
    public bool IsTeacher { get; set; }
    public int TeacherId { get; set; }
    public List<SchoolManagementSystem.Models.Entities.Teachers.TeacherClassAssignment>? Assignments { get; set; }
}

public class MarksAuditLogViewModel
{
    public IEnumerable<SchoolManagementSystem.Models.Entities.Result.ResultAuditLog> Logs { get; set; } = Enumerable.Empty<SchoolManagementSystem.Models.Entities.Result.ResultAuditLog>();
    public List<SchoolManagementSystem.Models.Entities.Exam.Exam> Exams { get; set; } = new();
    public int? SelectedExamId { get; set; }
}

public class ResultDashboardViewModel
{
    public AcademicYear? ActiveYear { get; set; }
    public List<ExamEntity> Exams { get; set; } = new();
    public ResultSummaryDto ResultStats { get; set; } = new();
    public List<AcademicYear> AcademicYears { get; set; } = new();
    public List<ExamListDto> FilterExams { get; set; } = new();
    public List<StudentGroup> Groups { get; set; } = new();
    public int SelectedAcademicYearId { get; set; }
    public int? SelectedExamId { get; set; }
    public int? SelectedGroupId { get; set; }
    public string ChartDataJson { get; set; } = "{}";
}

public class TeacherEntryViewModel
{
    public int TeacherId { get; set; }
    public List<TeacherClassAssignmentDto> Assignments { get; set; } = new();
    public List<ExamListDto> Exams { get; set; } = new();
}

public class AllResultsPageViewModel
{
    public IEnumerable<StudentExamResultDto> Results { get; set; } = Enumerable.Empty<StudentExamResultDto>();
    public IEnumerable<ExamListDto> Exams { get; set; } = Enumerable.Empty<ExamListDto>();
    public List<IdNamePairDto> Classes { get; set; } = new();
    public int? SelectedExamId { get; set; }
    public int? SelectedClassId { get; set; }
    public string? SelectedStatus { get; set; }
}

public class ResultPublishingPageViewModel
{
    public List<PublicationDashboardExamDto> Exams { get; set; } = new();
    public PublicationDashboardSummaryDto? Summary { get; set; }
    public List<PublicationHistoryEntryDto> History { get; set; } = new();
    public List<AcademicYear> AcademicYears { get; set; } = new();
    public int SelectedYearId { get; set; }
    public AcademicYear? ActiveYear { get; set; }
}

public class ReviewResultsPageViewModel
{
    public string ExamName { get; set; } = "";
    public int ExamId { get; set; }
    public string AcademicYearName { get; set; } = "";
    public int? ClassId { get; set; }
    public int? SectionId { get; set; }
    public int? GroupId { get; set; }
    public IEnumerable<ResultListItemDto> Results { get; set; } = Enumerable.Empty<ResultListItemDto>();
}

public class SubjectAnalysisPageViewModel
{
    public IEnumerable<SubjectPerformanceDto> Subjects { get; set; } = Enumerable.Empty<SubjectPerformanceDto>();
    public ExamEntity? Exam { get; set; }
}

public class AllSubjectsPageViewModel
{
    public List<SchoolManagementSystem.Models.DTOs.Academic.SubjectListItemDto> Subjects { get; set; } = new();
    public Dictionary<string, List<SchoolManagementSystem.Models.DTOs.Academic.SubjectListItemDto>> GroupedSubjects { get; set; } = new();
}

public class TeacherMarksEntryDataViewModel
{
    public bool Authorized { get; set; }
    public List<TeacherMarksEntryStudentDto> Students { get; set; } = new();
    public Dictionary<int, MarksEntryExistingDto> ExistingMarks { get; set; } = new();
}

public class TeacherResultSummaryViewModel
{
    public int TotalStudents { get; set; }
    public int MarksEntered { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public decimal AvgMarks { get; set; }
    public decimal HighestMarks { get; set; }
    public decimal LowestMarks { get; set; }
    public List<GradeDistributionItemDto> GradeDistribution { get; set; } = new();
    public string ChartDataJson { get; set; } = "{}";
}

public class TeacherMarksDashboardViewModel
{
    public int AssignedExams { get; set; }
    public int AssignedSubjects { get; set; }
    public int PendingEntries { get; set; }
    public int SubmittedEntries { get; set; }
    public int TotalEntries { get; set; }
    public List<string> ExamNames { get; set; } = new();
    public List<int> CompletionPercentages { get; set; } = new();
}
