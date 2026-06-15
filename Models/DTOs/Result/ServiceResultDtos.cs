using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Enums;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Models.DTOs.Result;

public class MarkEntryDataDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public List<StudentMarkDataDto> Students { get; set; } = new();
}

public class StudentMarkDataDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public decimal? MarksObtained { get; set; }
    public string? Grade { get; set; }
    public bool IsLocked { get; set; }
    public ComponentMarksDto ComponentMarks { get; set; } = new();
    public int? EnteredByTeacherId { get; set; }
    public string? EnteredByTeacherName { get; set; }
}

public class StudentPortalResultDto
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

public class ReEvaluationDashboardDto
{
    public List<ReEvaluationRequestItemDto> PendingRequests { get; set; } = new();
    public List<ReEvaluationRequestItemDto> CompletedRequests { get; set; } = new();
}

public class ReEvaluationRequestItemDto
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

public class EntryStatusSummaryDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public List<EntryStatusClassDto> Classes { get; set; } = new();
}

public class EntryStatusClassDto
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public List<EntryStatusSubjectDto> Subjects { get; set; } = new();
}

public class EntryStatusSubjectDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int EnteredCount { get; set; }
    public int LockedCount { get; set; }
    public bool IsLocked { get; set; }
    public decimal EntryPercentage { get; set; }
}

public class AdminDashboardDto
{
    public AcademicYear? ActiveYear { get; set; }
    public List<ExamEntity> Exams { get; set; } = new();
    public ResultSummaryDto ResultStats { get; set; } = new();
}
