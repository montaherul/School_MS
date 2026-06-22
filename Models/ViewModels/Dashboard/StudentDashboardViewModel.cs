using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.DTOs.Calendar;
using SchoolManagementSystem.Models.DTOs.Dashboard;
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
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public int LeaveCount { get; set; }
    public decimal TotalDue { get; set; }
    public string StudentStatus { get; set; } = string.Empty;
    public List<RecentActivityItem> RecentNotices { get; set; } = new();
    public List<AssignmentDashboardItem> UpcomingAssignments { get; set; } = new();
    public List<AssignmentDashboardItem> Assignments { get; set; } = new();
    public List<StudentResultViewModel> Results { get; set; } = new();
    public List<AttendanceCalendarDto> AttendanceCalendar { get; set; } = new();
    public List<StudentAttendanceDto> AttendanceHistory { get; set; } = new();

    // Calendar Widgets
    public List<UpcomingHolidayDto> UpcomingHolidays { get; set; } = new();
    public List<UpcomingExamDto> UpcomingExams { get; set; } = new();
    public bool IsResultBlocked { get; set; }

    // Routine Widget
    public StudentRoutineWidgetDto RoutineWidget { get; set; } = new();

    // Assignment Widget
    public int PendingAssignmentCount { get; set; }
    public int SubmittedAssignmentCount { get; set; }
    public int OverdueAssignmentCount { get; set; }
    public List<StudentAssignmentDto> RecentAssignments { get; set; } = new();

    // Library Widget
    public List<StudentLibraryBookDto> IssuedBooks { get; set; } = new();
    public int TotalIssuedBooks { get; set; }

    // Notification Center
    public int UnreadNotificationCount { get; set; }
    public List<StudentNotificationItemDto> RecentNotifications { get; set; } = new();

    // Guardian info
    public string GuardianName { get; set; } = string.Empty;
    public string GuardianCode { get; set; } = string.Empty;

    // Alerts
    public List<string> Alerts { get; set; } = new();

    // Leave Applications
    public int LeaveApplicationCount { get; set; }
    public int PendingLeaveCount { get; set; }

    // Finance Summary (loaded async via AJAX)
    public decimal FinanceTotalInvoiced { get; set; }
    public decimal FinanceTotalPaid { get; set; }
    public decimal FinanceTotalDue { get; set; }
}

public class AttendanceCalendarDto
{
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
}

public record AssignmentDashboardItem(string Subject, string Title, DateTime Deadline);
