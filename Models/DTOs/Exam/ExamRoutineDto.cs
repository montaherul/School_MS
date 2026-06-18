namespace SchoolManagementSystem.Models.DTOs.Exam;

public class ExamRoutineDto
{
    public int ScheduleId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public DateOnly ExamDate { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
    public string RoomNo { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? GroupName { get; set; }
    public string? SectionName { get; set; }
}

public class ExamRoutineViewModel
{
    public string ExamName { get; set; } = string.Empty;
    public string ExamTerm { get; set; } = string.Empty;
    public DateOnly? ExamStartsOn { get; set; }
    public DateOnly? ExamEndsOn { get; set; }
    public string? StudentName { get; set; }
    public string? StudentNo { get; set; }
    public string? ClassName { get; set; }
    public string? GroupName { get; set; }
    public int? SelectedStudentId { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string SchoolAddress { get; set; } = string.Empty;
    public string? SchoolLogo { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public List<ExamRoutineDto> Schedules { get; set; } = new();
}
