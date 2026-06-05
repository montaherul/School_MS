using System;
using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Guardian;
using SchoolManagementSystem.Models.Entities.Student;

namespace SchoolManagementSystem.Models.Entities.Attendance
{
    public class StudentLeaveApplication
    {
        [Key]
        public int Id { get; set; }

        // Reference to the student applying for leave
        [Required]
        public int StudentId { get; set; }

        // Reference to the guardian submitting the leave request
        [Required]
        public int GuardianId { get; set; }
        public Guardian.Guardian? Guardian { get; set; }
        public Student.Student? Student { get; set; }

        // Type of leave (e.g., sick, emergency, etc.)
        [Required]
        public int LeaveTypeId { get; set; }
        public LeaveType? LeaveType { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        // Calculated total days of leave
        public int TotalDays { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        // Optional attachment (e.g., medical certificate)
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
