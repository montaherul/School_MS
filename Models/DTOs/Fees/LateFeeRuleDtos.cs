using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class LateFeeRuleListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GraceDays { get; set; }
    public int FeeType { get; set; }
    public decimal FeeValue { get; set; }
    public decimal MaxFee { get; set; }
    public int? SchoolClassId { get; set; }
    public string? ClassName { get; set; }
    public int? FeeCategoryId { get; set; }
    public string? FeeCategoryName { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class LateFeeRuleUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public int GraceDays { get; set; }

    [Required]
    public int FeeType { get; set; }

    [Required]
    public decimal FeeValue { get; set; }

    public decimal MaxFee { get; set; }
    public int? SchoolClassId { get; set; }
    public int? FeeCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}
