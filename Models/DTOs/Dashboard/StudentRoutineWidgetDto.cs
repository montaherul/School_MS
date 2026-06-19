namespace SchoolManagementSystem.Models.DTOs.Dashboard;

public class StudentRoutineWidgetDto
{
    public List<RoutineClassDto> TodayClasses { get; set; } = new();
    public List<RoutineClassDto> ThisWeekClasses { get; set; } = new();
    public RoutineClassDto? NextClass { get; set; }
}

public class RoutineClassDto
{
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? RoomNo { get; set; }
}