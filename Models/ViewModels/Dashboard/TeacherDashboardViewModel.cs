using SchoolManagementSystem.Models.DTOs.Calendar;
using SchoolManagementSystem.Models.DTOs.Dashboard;

namespace SchoolManagementSystem.Models.ViewModels.Dashboard;

public class TeacherDashboardViewModel
{
    public int TeacherId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string TeacherNo { get; set; } = string.Empty;
    
    // Role Indicators
    public bool IsPrincipal { get; set; }
    public bool IsSeniorLecturer { get; set; }
    
    // Stats
    public int MyClassesCount { get; set; }
    public int MySubjectsCount { get; set; }
    public decimal AttendanceRate { get; set; }
    public int PendingResultEntries { get; set; }
    
    // Lists
    public List<string> MyClasses { get; set; } = new();
    public List<string> MySubjects { get; set; } = new();
    public List<AssignmentDashboardItem> UpcomingAssignments { get; set; } = new();
    public List<RecentActivityItem> RecentNotices { get; set; } = new();
    
    // Principal/Admin Specific
    public PrincipalStats? PrincipalStats { get; set; }

    // Calendar Widgets
    public List<UpcomingHolidayDto> UpcomingHolidays { get; set; } = new();
    public List<UpcomingExamDto> UpcomingExams { get; set; } = new();
    public List<UpcomingEventDto> UpcomingEvents { get; set; } = new();

    // ── Teacher Widgets ───────────────────────────────────────────────────

    // Schedule Widget
    public List<TeacherScheduleItemDto> TodaySchedule { get; set; } = new();
    public List<TeacherScheduleItemDto> WeeklySchedule { get; set; } = new();

    // Mark Entry Widget
    public List<TeacherMarkEntryStatusDto> MarkEntryStatus { get; set; } = new();

    // Assignment Widget
    public List<StudentAssignmentDto> RecentAssignments { get; set; } = new();
    public int TotalAssignments { get; set; }

    // Leave Status
    public TeacherLeaveStatusDto LeaveStatus { get; set; } = new();

    // Notifications
    public int TeacherUnreadNotificationCount { get; set; }
    public List<TeacherNotificationItemDto> TeacherRecentNotifications { get; set; } = new();

    // Summary
    public int TotalStudentsTaught { get; set; }
}

public class PrincipalStats
{
    public int TotalStaff { get; set; }
    public int TotalStudents { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public List<ChartPoint> DepartmentPerformance { get; set; } = new();
}
