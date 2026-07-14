using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class OnlinePaymentRequestDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public int FeeInvoiceId { get; set; }
    public string? InvoiceNo { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Status { get; set; }
    public string? Remarks { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
}

public class OnlinePaymentSubmitDto
{
    public int FeeInvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    [MaxLength(80)]
    public string? ReferenceNo { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }
}

public class OnlinePaymentVerifyDto
{
    public int Id { get; set; }

    [MaxLength(500)]
    public string? AdminNotes { get; set; }
}
