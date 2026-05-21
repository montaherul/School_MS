using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.Entities.Attendance
{
    public class LeaveApplication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee.Employee? Employee { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }
        public LeaveType? LeaveType { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        public int TotalDays { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        [MaxLength(260)]
        public string? AttachmentPath { get; set; }

        public enum ApprovalStatusEnum { Pending, Approved, Rejected }
        
        public ApprovalStatusEnum ApprovalStatus { get; set; } = ApprovalStatusEnum.Pending;

        [MaxLength(100)]
        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
