using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FeeDiscountListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DiscountType { get; set; }
    public decimal Value { get; set; }
    public int? SchoolClassId { get; set; }
    public string? ClassName { get; set; }
    public int? FeeCategoryId { get; set; }
    public string? FeeCategoryName { get; set; }
    public int? FeeStructureId { get; set; }
    public string? FeeStructureName { get; set; }
    public bool IsActive { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
    public int TotalRecords { get; set; }
}

public class FeeDiscountUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public int DiscountType { get; set; }

    [Required]
    public decimal Value { get; set; }

    public int? SchoolClassId { get; set; }
    public int? FeeCategoryId { get; set; }
    public int? FeeStructureId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
}
