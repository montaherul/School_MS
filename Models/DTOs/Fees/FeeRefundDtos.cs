using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FeeRefundListItemDto
{
    public int Id { get; set; }
    public int FeePaymentId { get; set; }
    public int FeeInvoiceId { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public int RefundMethod { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Reason { get; set; }
    public bool IsApproved { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime RefundDate { get; set; }
    public int TotalRecords { get; set; }
}

public class FeeRefundUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int FeePaymentId { get; set; }

    [Required]
    public decimal RefundAmount { get; set; }

    [Required]
    public int RefundMethod { get; set; }

    [StringLength(200)]
    public string? ReferenceNo { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    public bool IsApproved { get; set; }
    public DateTime RefundDate { get; set; } = DateTime.UtcNow;
}
