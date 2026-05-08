using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Attendance;

public class AttendanceRecord : BaseEntity
{
    public int StudentId { get; set; }
    public int SchoolClassId { get; set; }
    public int SectionId { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public int? PeriodNo { get; set; }
    public AttendanceStatus Status { get; set; }

    [MaxLength(240)]
    public string? Remarks { get; set; }

    public int? CreatedByUserId { get; set; }
    public int? UpdatedByUserId { get; set; }
}

public class LeaveApplication : BaseEntity
{
    public int StudentId { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }

    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public int? ApprovedByUserId { get; set; }
}
