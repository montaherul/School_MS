using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FeePaymentListItemDto
{
    public int Id { get; set; }
    public int FeeInvoiceId { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal LateFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public int Method { get; set; }
    public string? ReferenceNo { get; set; }
    public DateTime PaidAt { get; set; }
    public string? Remarks { get; set; }
    public int TotalRecords { get; set; }
}

public class FeePaymentUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int FeeInvoiceId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public decimal LateFee { get; set; }
    public decimal DiscountAmount { get; set; }

    [Required]
    public int Method { get; set; }

    [StringLength(80)]
    public string? ReferenceNo { get; set; }

    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? Remarks { get; set; }
}
