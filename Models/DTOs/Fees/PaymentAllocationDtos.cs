using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class PaymentAllocationListItemDto
{
    public int Id { get; set; }
    public int PaymentId { get; set; }
    public string? PaymentReference { get; set; }
    public int FeeInvoiceId { get; set; }
    public string? InvoiceNo { get; set; }
    public decimal AllocatedAmount { get; set; }
    public string? Remarks { get; set; }
    public int TotalRecords { get; set; }
}

public class PaymentAllocationUpsertDto
{
    public int Id { get; set; }
    public int PaymentId { get; set; }
    public int FeeInvoiceId { get; set; }
    public decimal AllocatedAmount { get; set; }
    [StringLength(500)]
    public string? Remarks { get; set; }
}
