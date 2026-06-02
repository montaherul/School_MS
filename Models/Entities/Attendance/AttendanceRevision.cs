using System;
using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Attendance;

public class AttendanceRevision : BaseEntity
{
    public int AttendanceRecordId { get; set; }
    public int StudentId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;

    [MaxLength(512)]
    public string? Reason { get; set; }

    [MaxLength(128)]
    public string ChangedBy { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
