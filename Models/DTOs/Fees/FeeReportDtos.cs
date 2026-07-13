namespace SchoolManagementSystem.Models.DTOs.Fees;

public class StudentLedgerReportDto
{
    public int Id { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
    public string Description { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
}

public class DailyCollectionReportDto
{
    public int Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string ReferenceNo { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public int TotalRecords { get; set; }
}

public class MonthlyCollectionReportDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalCollected { get; set; }
    public int TransactionCount { get; set; }
    public int TotalRecords { get; set; }
}

public class DueReportDto
{
    public int Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public int DaysOverdue { get; set; }
    public int TotalRecords { get; set; }
}

public class DiscountReportDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string FeeCategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class WaiverReportDto
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string InvoiceNo { get; set; } = string.Empty;
    public decimal WaiverAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
}

public class RefundReportDto
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string InvoiceNo { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime RefundDate { get; set; }
    public int TotalRecords { get; set; }
}

public class ClassCollectionSummaryDto
{
    public string ClassName { get; set; } = string.Empty;
    public decimal TotalAssigned { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal TotalDue { get; set; }
    public decimal CollectionRate { get; set; }
    public int StudentCount { get; set; }
    public int TotalRecords { get; set; }
}
