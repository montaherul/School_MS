namespace SchoolManagementSystem.Models.DTOs.Exam;

public class AutoScheduleResultDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public int TotalSubjects { get; set; }
    public int Scheduled { get; set; }
    public int Skipped { get; set; }
    public List<AutoScheduleItemDto> Items { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}

public class AutoScheduleItemDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public bool IsScheduled { get; set; }
    public DateOnly? ExamDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string? RoomNo { get; set; }
    public string? Reason { get; set; }
}
