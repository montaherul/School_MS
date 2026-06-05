using System;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Attendance
{
    /// <summary>
    /// Main DTO for displaying student attendance records - organized to match EmployeeAttendanceDto pattern
    /// </summary>
    public class StudentAttendanceDto
    {
        // Identity fields
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;

        // Hierarchy fields (student-specific)
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int? StudentGroupId { get; set; }
        public string StudentGroupName { get; set; } = string.Empty;

        // Attendance fields
        public DateTime AttendanceDate { get; set; }
        public AttendanceStatus Status { get; set; }
        public string StatusName { get; set; } = AttendanceStatus.Present.ToString();
        public string? Remarks { get; set; }

        // Notification fields (student-specific)
        public string? GuardianEmail { get; set; }
        public bool NotificationSent { get; set; } = false;
    }

    /// <summary>
    /// DTO for bulk attendance save operations - matches EmployeeAttendanceBulkDto pattern
    /// </summary>
    public class StudentAttendanceBulkDto
    {
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int? StudentGroupId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public List<StudentAttendanceItemDto> Attendances { get; set; } = new List<StudentAttendanceItemDto>();

        /// <summary>
        /// If true, automatically send guardian email notifications for absent students
        /// </summary>
        public bool SendNotifications { get; set; } = true;
    }

    /// <summary>
    /// DTO for individual attendance item in bulk operations - matches EmployeeAttendanceItemDto pattern
    /// </summary>
    public class StudentAttendanceItemDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public AttendanceStatus Status { get; set; }
        public string? StatusName { get; set; }
        public string? Remarks { get; set; }
        public AttendanceStatus? PreviousStatus { get; set; }
    }

    /// <summary>
    /// DTO for filtering student attendance - matches EmployeeAttendanceFilterDto pattern
    /// </summary>
    public class StudentAttendanceFilterDto
    {
        public DateTime AttendanceDate { get; set; } = DateTime.Today;
        public int? StudentId { get; set; }
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }
        public int? StudentGroupId { get; set; }
        public string? SearchTerm { get; set; }
        public int? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    /// <summary>
    /// Response DTO for attendance summary - matches EmployeeAttendanceSummaryDto pattern
    /// </summary>
    public class StudentAttendanceSummaryDto
    {
        public int TotalStudents { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public int Leave { get; set; }
    }

    /// <summary>
    /// DTO for monthly attendance summary - matches EmployeeAttendanceMonthlySummaryDto pattern
    /// </summary>
    public class StudentAttendanceMonthlySummaryDto
    {
        // Identity fields
        public int StudentId { get; set; }
        public string StudentNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;

        // Period fields
        public int Year { get; set; }
        public int Month { get; set; }

        // Summary counts
        public int TotalDays { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int LeaveCount { get; set; }

        // Calculated fields
        public double AttendancePercentage { get; set; }
    }

    /// <summary>
    /// Response DTO for bulk attendance save operations - consistent naming with Employee Attendance
    /// </summary>
    public class BulkAttendanceSaveResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int RecordsSaved { get; set; }
        public int NotificationsSent { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Response DTO for paginated load operations - consistent with Employee Attendance response
    /// </summary>
    public class StudentAttendanceLoadResponse
    {
        public List<StudentAttendanceDto> Data { get; set; } = new List<StudentAttendanceDto>();
        public int TotalRecords { get; set; }
        public StudentAttendanceSummaryDto Summary { get; set; } = new StudentAttendanceSummaryDto();
    }
}
