namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FeeReceiptDto
{
    public string ReceiptNo { get; set; } = string.Empty;
    public string InvoiceNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string StudentIdNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal LateFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string ReferenceNo { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string SchoolAddress { get; set; } = string.Empty;
    public string SchoolPhone { get; set; } = string.Empty;
    public string SchoolEmail { get; set; } = string.Empty;
    public string QrVerificationCode { get; set; } = string.Empty;
}
