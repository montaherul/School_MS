namespace SchoolManagementSystem.Models.DTOs.Exam;

public class ExamDashboardDto
{
    public int TotalExams { get; set; }
    public int DraftExams { get; set; }
    public int SubmittedExams { get; set; }
    public int ReviewedExams { get; set; }
    public int ApprovedExams { get; set; }
    public int PublishedExams { get; set; }
    public int LockedExams { get; set; }
    public int UnpublishedExams { get; set; }
    public int StudentsAppeared { get; set; }
}

public class ExamStatusDistributionDto
{
    public int Status { get; set; }
    public int Count { get; set; }
}

public class ExamPassRateDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
    public decimal PassPercentage { get; set; }
}