using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.DTOs.Calendar;
using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Models.DTOs.Student;

namespace SchoolManagementSystem.Models.ViewModels.Dashboard;

public class StudentDashboardViewModel
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public string? StudentStatus { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianCode { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
    public string? ProfilePicturePath { get; set; }
    public List<string> Alerts { get; set; } = new();

    // Attendance
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public int LeaveCount { get; set; }
    public double AttendancePercentage { get; set; }
    public List<StudentAttendanceDto> AttendanceHistory { get; set; } = new();
    public List<AttendanceCalendarDto> AttendanceCalendar { get; set; } = new();

    // Finance
    public decimal OutstandingFees { get; set; }
    public decimal TotalPaid { get; set; }
    public int InvoiceCount { get; set; }
    public decimal TotalInvoiced { get; set; }
    public decimal TotalDue { get; set; }

    // Results
    public decimal? LatestGPA { get; set; }
    public string LatestGrade { get; set; } = string.Empty;
    public bool LatestPassed { get; set; }
    public bool IsResultBlocked { get; set; }
    public List<StudentResultViewModel> Results { get; set; } = new();

    // Leave
    public int LeaveApplicationCount { get; set; }
    public int PendingLeaveCount { get; set; }

    // Notifications
    public int UnreadNotificationCount { get; set; }
    public List<StudentNotificationItemDto> RecentNotifications { get; set; } = new();

    // Routine
    public StudentRoutineWidgetDto RoutineWidget { get; set; } = new();

    // Assignments
    public int PendingAssignmentCount { get; set; }
    public int SubmittedAssignmentCount { get; set; }
    public int OverdueAssignmentCount { get; set; }
    public List<StudentAssignmentDto> RecentAssignments { get; set; } = new();
    public List<AssignmentDashboardItem> UpcomingAssignments { get; set; } = new();

    // Library
    public List<StudentLibraryBookDto> IssuedBooks { get; set; } = new();
    public int TotalIssuedBooks { get; set; }

    // Notices
    public List<StudentNoticeDto> RecentNotices { get; set; } = new();

    // Calendar
    public List<UpcomingHolidayDto> UpcomingHolidays { get; set; } = new();
    public List<UpcomingExamDto> UpcomingExams { get; set; } = new();
}

public class StudentNoticeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Excerpt { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
