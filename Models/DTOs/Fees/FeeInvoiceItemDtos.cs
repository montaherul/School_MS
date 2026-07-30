using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FeeInvoiceItemListItemDto
{
    public int Id { get; set; }
    public int FeeInvoiceId { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public int? FeeStructureId { get; set; }
    public string? FeeStructureName { get; set; }
    public int? FeeCategoryId { get; set; }
    public string? FeeCategoryName { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public int TotalRecords { get; set; }
}

public class FeeInvoiceItemUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int FeeInvoiceId { get; set; }

    public int? FeeStructureId { get; set; }
    public int? FeeCategoryId { get; set; }

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public decimal Amount { get; set; }

    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
}

public class BillingItemDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
