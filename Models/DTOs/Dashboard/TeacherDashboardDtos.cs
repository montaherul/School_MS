namespace SchoolManagementSystem.Models.DTOs.Dashboard;

public class TeacherScheduleItemDto
{
    public string SubjectName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? RoomNo { get; set; }
}

public class TeacherMarkEntryStatusDto
{
    public string SubjectName { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int MarksEntered { get; set; }
    public int PendingCount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class TeacherLeaveStatusDto
{
    public int TotalLeaves { get; set; }
    public int ApprovedLeaves { get; set; }
    public int PendingLeaves { get; set; }
    public int RejectedLeaves { get; set; }
}

public class TeacherNotificationItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public bool IsRead { get; set; }
    public DateTime? SentAt { get; set; }
}
