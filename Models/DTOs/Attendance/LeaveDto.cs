using System;

namespace SchoolManagementSystem.Models.DTOs.Attendance
{
    public class LeaveTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MaxDays { get; set; }
        public bool IsPaid { get; set; }
        public bool IsActive { get; set; }
    }

    public class LeaveApplicationDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalDays { get; set; }
        public string? Reason { get; set; }
        public string? AttachmentPath { get; set; }
        public string ApprovalStatus { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
