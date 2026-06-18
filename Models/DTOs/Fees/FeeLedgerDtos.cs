namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FeeLedgerListItemDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int? FeeInvoiceId { get; set; }
    public string? InvoiceNo { get; set; }
    public int? FeePaymentId { get; set; }
    public int TransactionType { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
    public string? Description { get; set; }
    public DateTime TransactionDate { get; set; }
    public int TotalRecords { get; set; }
}
