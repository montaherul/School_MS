using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class SyllabusListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string AcademicYearName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long? FileSize { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class SyllabusUpsertDto
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public int SchoolClassId { get; set; }

    [Required]
    public int SubjectId { get; set; }

    [Required]
    public int AcademicYearId { get; set; }

    public bool IsActive { get; set; } = true;

    public string? ExistingFileName { get; set; }
}
