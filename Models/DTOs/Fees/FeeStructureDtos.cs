using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FeeStructureListItemDto
{
    public int Id { get; set; }
    public int SchoolClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string FeeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsRecurring { get; set; }
    public int TotalRecords { get; set; }
}

public class FeeStructureUpsertDto
{
    public int Id { get; set; }
    [Required]
    public int SchoolClassId { get; set; }
    [Required]
    [StringLength(100)]
    public string FeeName { get; set; } = string.Empty;
    [Required]
    public decimal Amount { get; set; }
    [Required]
    public bool IsRecurring { get; set; }
}

