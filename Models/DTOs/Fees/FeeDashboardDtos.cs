namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FeeDashboardDto
{
    public decimal TotalAssigned { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal TotalOutstanding { get; set; }
    public decimal TotalDiscounted { get; set; }
    public int TotalInvoices { get; set; }
    public int TotalPayments { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal CollectionRate { get; set; }

    public List<MonthlyCollectionDto> MonthlyCollections { get; set; } = [];
    public List<PaymentMethodBreakdownDto> PaymentMethodBreakdown { get; set; } = [];
    public List<DueSoonInvoiceDto> DueSoonInvoices { get; set; } = [];
}

public class MonthlyCollectionDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Collected { get; set; }
    public int TransactionCount { get; set; }
}

public class PaymentMethodBreakdownDto
{
    public int Method { get; set; }
    public int Count { get; set; }
    public decimal Total { get; set; }
}

public class DueSoonInvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public int DaysRemaining { get; set; }
}
