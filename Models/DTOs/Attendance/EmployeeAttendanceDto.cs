using System;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Attendance
{
    public class EmployeeAttendanceDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string EmployeeType { get; set; } = string.Empty;
        public bool IsTeachingStaff { get; set; }
        public DateTime AttendanceDate { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public AttendanceStatus Status { get; set; }
        public string StatusName { get; set; } = AttendanceStatus.Present.ToString();
        public string? Remarks { get; set; }
    }

    public class EmployeeAttendanceBulkDto
    {
        public DateTime AttendanceDate { get; set; }
        public List<EmployeeAttendanceItemDto> Attendances { get; set; } = new List<EmployeeAttendanceItemDto>();
    }

    public class EmployeeAttendanceItemDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public AttendanceStatus Status { get; set; }
        public string? StatusName { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public string? Remarks { get; set; }
    }

    public class EmployeeAttendanceFilterDto
    {
        public DateTime AttendanceDate { get; set; } = DateTime.Today;
        public int? EmployeeId { get; set; }
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public string? EmployeeType { get; set; }
        public bool? IsTeachingStaff { get; set; }
        public string? SearchTerm { get; set; }
        public int? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class EmployeeAttendanceSummaryDto
    {
        public int TotalEmployees { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public int Leave { get; set; }
    }

    public class EmployeeAttendanceMonthlySummaryDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public int RecordedDays { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public int Leave { get; set; }
        public double AttendancePercentage { get; set; }
    }
}
