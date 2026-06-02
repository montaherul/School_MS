using System;
using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Attendance;

public class AttendanceSession : BaseEntity
{
    [Required]
    public int SchoolClassId { get; set; }

    [Required]
    public int SectionId { get; set; }

    public int? StudentGroupId { get; set; }

    [Required]
    public DateOnly AttendanceDate { get; set; }

    public AttendanceSessionStatus Status { get; set; } = AttendanceSessionStatus.Draft;

    // Draft → Submitted Transition
    [MaxLength(256)]
    public string? SubmittedBy { get; set; }

    public DateTime? SubmittedAt { get; set; }

    // Submitted → Locked Transition
    [MaxLength(256)]
    public string? LockedBy { get; set; }

    public DateTime? LockedAt { get; set; }

    // Locked → Revised Transition
    [MaxLength(256)]
    public string? RevisedBy { get; set; }

    public DateTime? RevisedAt { get; set; }

    // Revised → Approved Transition (or Submitted → Approved for non-revised sessions)
    [MaxLength(256)]
    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    // Workflow notes/reason for state changes
    [MaxLength(512)]
    public string? Notes { get; set; }
}