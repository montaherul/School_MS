namespace SchoolManagementSystem.Models.DTOs.Fees;

public class CashierCollectionSearchDto
{
    public string? SearchTerm { get; set; }
    public List<StudentSearchResultDto> Results { get; set; } = [];
}

public class StudentSearchResultDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public string StudentCode { get; set; } = "";
    public string ClassName { get; set; } = "";
}

public class CashierCollectionDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public string StudentCode { get; set; } = "";
    public string ClassName { get; set; } = "";
    public List<CashierInvoiceItemDto> Invoices { get; set; } = [];
    public CashierPaymentDto Payment { get; set; } = new();
}

public class CashierInvoiceItemDto
{
    public int InvoiceId { get; set; }
    public string InvoiceNo { get; set; } = "";
    public DateOnly DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal LateFee { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = "";
    public bool IsSelected { get; set; }
}

public class CashierPaymentDto
{
    public decimal Amount { get; set; }
    public decimal LateFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public int Method { get; set; } = 1;
    public string? ReferenceNo { get; set; }
    public string? Remarks { get; set; }
}

public class CashierPaymentResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int PaymentId { get; set; }
    public string? ReceiptUrl { get; set; }
}
