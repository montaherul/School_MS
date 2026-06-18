using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FeeStructureListItemDto
{
    public int Id { get; set; }
    public int SchoolClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int? FeeCategoryId { get; set; }
    public string? FeeCategoryName { get; set; }
    public int? AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }
    public string FeeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public bool IsRecurring { get; set; }
    public int Frequency { get; set; }
    public int? DueDay { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class FeeStructureUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int SchoolClassId { get; set; }

    public int? FeeCategoryId { get; set; }
    public int? AcademicYearId { get; set; }

    [Required]
    [StringLength(100)]
    public string FeeName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public bool IsRecurring { get; set; }
    public int Frequency { get; set; } = 1;
    public int? DueDay { get; set; }
    public bool IsActive { get; set; } = true;
}
