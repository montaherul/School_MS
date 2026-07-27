using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class ScholarshipListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = "Percentage";
    public decimal Value { get; set; }
    public int? SchoolClassId { get; set; }
    public string? ClassName { get; set; }
    public int? FeeCategoryId { get; set; }
    public string? FeeCategoryName { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class ScholarshipUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public FeeDiscountType DiscountType { get; set; } = FeeDiscountType.Percentage;
    public decimal Value { get; set; }

    public int? SchoolClassId { get; set; }
    public int? FeeCategoryId { get; set; }
    public int? FeeTypeId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
}
