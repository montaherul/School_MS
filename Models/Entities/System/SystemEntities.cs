using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Models.Entities.System;

public class SchoolProfile : BaseEntity
{
    [MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Allows teachers to customize component MaxMarks/PassMarks for their assigned exam subjects.
    /// When false, only admins can modify component marks via ExamSubjectComponent management.
    /// </summary>
    public bool AllowTeacherComponentCustomization { get; set; } = false;
}

public class SystemLog : BaseEntity
{
    [MaxLength(40)]
    public string Level { get; set; } = "Information";

    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;
}

public class BackupRecord : BaseEntity
{
    [MaxLength(260)]
    public string FilePath { get; set; } = string.Empty;

    public DateTime BackupAt { get; set; } = DateTime.UtcNow;
    public bool Restored { get; set; }
}

public class ActivityLog : BaseEntity
{
    public int? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Module { get; set; } = string.Empty;

    public int? RecordId { get; set; }

    public string? OldValues { get; set; }
    public string? NewValues { get; set; }

    [MaxLength(64)]
    public string? IpAddress { get; set; }
}
