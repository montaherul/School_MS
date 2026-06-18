using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.DTOs.Calendar;
using SchoolManagementSystem.Models.DTOs.Guardian;
using SchoolManagementSystem.Services.Interfaces.Guardian;

namespace SchoolManagementSystem.Models.ViewModels.Dashboard
{
    public class GuardianDashboardViewModel
    {
        public int GuardianId { get; set; }
        public string GuardianCode { get; set; } = string.Empty;
        public string GuardianName { get; set; } = string.Empty;
        public decimal TotalOutstandingFees { get; set; }
        public int UnreadNotifications { get; set; }
        
        public List<GuardianChildSummaryViewModel> Children { get; set; } = new();
        public List<GuardianRecentNoticeDto> RecentNotices { get; set; } = new();

        // Detailed child fields for compiling GuardianIndex.cshtml
        public string StudentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public List<string> Alerts { get; set; } = new();
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int LeaveCount { get; set; }
        public double AttendancePercentage { get; set; }
        public List<StudentAttendanceDto> AttendanceHistory { get; set; } = new();
        public List<AttendanceCalendarDto> AttendanceCalendar { get; set; } = new();

        // Multi-child switcher
        public int? SelectedChildId { get; set; }
        public List<GuardianChildCardDto> ChildSwitcher { get; set; } = new();

        // Widget data
        public decimal SelectedChildOutstandingFees { get; set; }
        public decimal SelectedChildTotalPaid { get; set; }
        public int SelectedChildInvoiceCount { get; set; }
        public decimal? SelectedChildLatestGPA { get; set; }
        public string SelectedChildLatestGrade { get; set; } = string.Empty;
        public bool SelectedChildLatestPassed { get; set; }
        public int SelectedChildLeaveCount { get; set; }
        public int SelectedChildPendingLeaveCount { get; set; }

        // Calendar Widgets
        public List<UpcomingHolidayDto> UpcomingHolidays { get; set; } = new();
        public List<UpcomingExamDto> UpcomingExams { get; set; } = new();
        public bool IsResultBlocked { get; set; }
    }

    public class GuardianChildSummaryViewModel
{
    public int StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int TotalDays { get; set; }
    public double AttendancePercentage { get; set; }
    }
}
