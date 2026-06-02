using System;
using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Attendance;

public class EmployeeAttendance : BaseEntity
{
    [Required]
    public int EmployeeId { get; set; }
    public Employee.Employee? Employee { get; set; }

    [Required]
    public DateTime AttendanceDate { get; set; }

    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }

    [Required]
    public AttendanceStatus Status { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }
}
