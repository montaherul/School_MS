using System;
using System.Collections.Generic;
using SchoolManagementSystem.Models.DTOs.Guardian;
using SchoolManagementSystem.Models.DTOs.Attendance;

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
