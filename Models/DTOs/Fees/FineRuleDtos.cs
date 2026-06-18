using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FineRuleListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GraceDays { get; set; }
    public decimal FinePerDay { get; set; }
    public int TotalRecords { get; set; }
}

public class FineRuleUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public int GraceDays { get; set; }

    [Required]
    public decimal FinePerDay { get; set; }
}
