using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Attendance;

public class AttendanceRecordListItemDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int SchoolClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public SchoolManagementSystem.Models.Enums.AttendanceStatus Status { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public DateOnly AttendanceDate { get; set; }
    public int TotalRecords { get; set; }
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

    public DateOnly AttendanceDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
}

public class AttendanceCalendarDto
{
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
}

