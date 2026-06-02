using System.Collections.Generic;
using SchoolManagementSystem.Models.DTOs.Attendance;

namespace SchoolManagementSystem.Models.ViewModels.Dashboard
{
    public class GuardianDashboardViewModel
    {
        public int GuardianId { get; set; }
        public string GuardianName { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNo { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public double AttendancePercentage { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int LeaveCount { get; set; }
        public List<StudentAttendanceDto> AttendanceHistory { get; set; } = new();
        public List<StudentAttendanceDto> AbsentHistory { get; set; } = new();
        public List<StudentAttendanceDto> LateHistory { get; set; } = new();
        public List<string> Alerts { get; set; } = new();
        public StudentAttendanceMonthlySummaryDto MonthlySummary { get; set; } = new();
    }
}
