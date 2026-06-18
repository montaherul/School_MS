using SchoolManagementSystem.Models.DTOs.Calendar;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.ViewModels.Dashboard;

public class StudentDashboardViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public decimal AttendancePercentage { get; set; }
    public decimal TotalDue { get; set; }
    public string StudentStatus { get; set; } = string.Empty;
    public List<RecentActivityItem> RecentNotices { get; set; } = new();
    public List<AssignmentDashboardItem> UpcomingAssignments { get; set; } = new();
    public List<AssignmentDashboardItem> Assignments { get; set; } = new();
    public List<StudentResultViewModel> Results { get; set; } = new();
    public List<AttendanceCalendarDto> AttendanceCalendar { get; set; } = new();

    // Calendar Widgets
    public List<UpcomingHolidayDto> UpcomingHolidays { get; set; } = new();
    public List<UpcomingExamDto> UpcomingExams { get; set; } = new();
    public bool IsResultBlocked { get; set; }
}

public class AttendanceCalendarDto
{
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
}

public record AssignmentDashboardItem(string Subject, string Title, DateTime Deadline);
