namespace SchoolManagementSystem.Models.DTOs.Calendar;

public class CalendarWidgetDto
{
    public List<UpcomingHolidayDto> UpcomingHolidays { get; set; } = new();
    public List<UpcomingExamDto> UpcomingExams { get; set; } = new();
    public List<UpcomingEventDto> UpcomingEvents { get; set; } = new();
    public MonthSummaryDto MonthSummary { get; set; } = new();
}

public class UpcomingHolidayDto
{
    public DateOnly Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public string HolidayType { get; set; } = string.Empty;
    public string DayOfWeek => Date.DayOfWeek.ToString();
}

public class UpcomingExamDto
{
    public DateOnly Date { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string DayOfWeek => Date.DayOfWeek.ToString();
}

public class UpcomingEventDto
{
    public DateOnly Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DayOfWeek => Date.DayOfWeek.ToString();
}

public class MonthSummaryDto
{
    public int TotalDays { get; set; }
    public int WorkingDays { get; set; }
    public int HolidayCount { get; set; }
    public int ExamDayCount { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int Year { get; set; }
}
