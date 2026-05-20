using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.Entities.Attendance
{
    public class EmployeeAttendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee.Employee? Employee { get; set; }

        [Required]
        public DateTime AttendanceDate { get; set; }

        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }

        [Required]
        public SchoolManagementSystem.Models.Enums.AttendanceStatus Status { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [Required]
        public string RecordedBy { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
