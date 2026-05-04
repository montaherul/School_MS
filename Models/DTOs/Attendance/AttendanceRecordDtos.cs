using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Attendance;

public class AttendanceRecordListItemDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int SchoolClassId { get; set; }
    public int SectionId { get; set; }
    public SchoolManagementSystem.Models.Enums.AttendanceStatus Status { get; set; }
    public string Remarks { get; set; } = string.Empty;
}

public class AttendanceRecordUpsertDto
{
    public int Id { get; set; }
    [Required]
    public int StudentId { get; set; }
    [Required]
    public int SchoolClassId { get; set; }
    [Required]
    public int SectionId { get; set; }
    [Required]
    public SchoolManagementSystem.Models.Enums.AttendanceStatus Status { get; set; }
    [Required]
    [StringLength(240)]
    public string Remarks { get; set; } = string.Empty;
}

