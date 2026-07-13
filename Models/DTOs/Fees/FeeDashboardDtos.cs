namespace SchoolManagementSystem.Models.DTOs.Fees;

public class EnhancedFeeDashboardDto
{
    public decimal TotalAssigned { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal TotalOutstanding { get; set; }
    public decimal TotalDiscounted { get; set; }
    public decimal TotalWaived { get; set; }
    public decimal TotalLateFeeCollected { get; set; }
    public int TotalInvoices { get; set; }
    public int TotalPayments { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal CollectionRate { get; set; }

    public decimal TodayCollection { get; set; }
    public int TodayPaymentCount { get; set; }
    public decimal LateFeeOutstanding { get; set; }
    public int DueStudentCount { get; set; }
    public decimal CashBalance { get; set; }
    public decimal ScholarshipAmount { get; set; }
    public int ScholarshipCount { get; set; }
    public int PendingInvoiceCount { get; set; }

    public List<ClassCollectionSummary> ClassCollections { get; set; } = [];
    public List<DueSoonInvoiceDto> DueSoonInvoices { get; set; } = [];
    public List<MonthlyCollectionDto> MonthlyTrend { get; set; } = [];
    public List<MonthlyCollectionDto> MonthlyCollections { get; set; } = [];
    public List<PaymentMethodBreakdownDto> PaymentMethodBreakdown { get; set; } = [];
}

public class ClassCollectionSummary
{
    public string ClassName { get; set; } = "";
    public decimal Assigned { get; set; }
    public decimal Collected { get; set; }
    public decimal Due { get; set; }
    public decimal Rate { get; set; }
}

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
