using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class SchoolSessionListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public bool IsCurrent { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class SchoolSessionUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int AcademicYearId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public bool IsCurrent { get; set; }
    public bool IsActive { get; set; }
}
