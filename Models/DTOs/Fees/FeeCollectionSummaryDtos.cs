using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FeeCollectionSummaryListItemDto
{
    public int Id { get; set; }
    public DateOnly CollectionDate { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal TotalDiscounted { get; set; }
    public decimal TotalRefunded { get; set; }
    public int TotalTransactions { get; set; }
    public int? PaymentMethod { get; set; }
    public bool IsDailySummary { get; set; }
    public int TotalRecords { get; set; }
}

public class FeeCollectionSummaryUpsertDto
{
    public int Id { get; set; }

    [Required]
    public DateOnly CollectionDate { get; set; }

    public decimal TotalCollected { get; set; }
    public decimal TotalDiscounted { get; set; }
    public decimal TotalRefunded { get; set; }
    public int TotalTransactions { get; set; }
    public int? PaymentMethod { get; set; }
    public bool IsDailySummary { get; set; } = true;
}
