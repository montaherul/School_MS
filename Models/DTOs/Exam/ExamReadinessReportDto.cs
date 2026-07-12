namespace SchoolManagementSystem.Models.DTOs.Exam;

public class ExamReadinessReportDto
{
    public int TotalExams { get; set; }
    public int DraftExams { get; set; }
    public int ReadyExams { get; set; }
    public int ClassesWithExams { get; set; }
    public int TotalActiveClasses { get; set; }
    public List<ExamReadinessIssueDto> ExamsWithoutSubjects { get; set; } = [];
    public List<ExamReadinessIssueDto> ExamsWithoutSchedule { get; set; } = [];
    public List<ExamReadinessIssueDto> ExamsWithoutGradingRules { get; set; } = [];
}

public class ExamReadinessIssueDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int? SubjectCount { get; set; }
    public int? ScheduledCount { get; set; }
}

public class AttendanceForPromotionDto
{
    public int StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int TotalSchoolDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public int LeaveDays { get; set; }
    public decimal AttendancePercentage { get; set; }
    public string EligibilityStatus { get; set; } = string.Empty;
}
