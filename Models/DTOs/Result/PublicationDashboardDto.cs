namespace SchoolManagementSystem.Models.DTOs.Result;

public class PublicationDashboardExamDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public DateOnly StartsOn { get; set; }
    public DateOnly EndsOn { get; set; }
    public int Status { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockedAt { get; set; }
    public int? LockedByUserId { get; set; }
    public int TotalResults { get; set; }
    public int PublishedResults { get; set; }
    public int ApprovedResults { get; set; }
    public int ReviewedResults { get; set; }
    public int SubmittedResults { get; set; }
    public int DraftResults { get; set; }
    public DateTime? LockedDateTime { get; set; }
}

public class PublicationDashboardSummaryDto
{
    public int TotalExams { get; set; }
    public int PublishedExams { get; set; }
    public int ApprovedExams { get; set; }
    public int ReviewedExams { get; set; }
    public int SubmittedExams { get; set; }
    public int DraftExams { get; set; }
    public int TotalStudentResults { get; set; }
    public int TotalPublishedResults { get; set; }
}