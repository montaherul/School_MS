using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

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
