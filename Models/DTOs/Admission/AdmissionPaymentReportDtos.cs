namespace SchoolManagementSystem.Models.DTOs.Admission;

public class AdmissionReceiptDto
{
    public int Id { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public int AdmissionApplicationId { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? ApplicantName { get; set; }
    public DateTime ReceiptDate { get; set; }
    public bool IsRefunded { get; set; }
    public decimal? RefundAmount { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? RefundedBy { get; set; }
    public string? RefundReason { get; set; }
    public int TotalRecords { get; set; }
}

public class AdmissionDailyCollectionDto
{
    public int Id { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public int AdmissionApplicationId { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? ApplicantName { get; set; }
    public DateTime ReceiptDate { get; set; }
    public int TotalRecords { get; set; }
}

public class AdmissionMonthlyCollectionDto
{
    public int Id { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public int AdmissionApplicationId { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? ApplicantName { get; set; }
    public DateTime ReceiptDate { get; set; }
}

public class AdmissionMonthlySummaryDto
{
    public int TotalCount { get; set; }
    public decimal TotalCollected { get; set; }
}

public class AdmissionRevenueReportDto
{
    public decimal TotalCollected { get; set; }
    public decimal TotalRefunded { get; set; }
    public int TotalTransactions { get; set; }
    public int TotalRefunds { get; set; }
    public List<AdmissionRevenueDailyDto> DailyBreakdown { get; set; } = new();
}

public class AdmissionRevenueDailyDto
{
    public DateTime CollectionDate { get; set; }
    public decimal DailyTotal { get; set; }
    public int DailyCount { get; set; }
}

public class AdmissionRefundReportDto
{
    public int Id { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public int AdmissionApplicationId { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public decimal? RefundAmount { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? RefundReason { get; set; }
    public string? RefundedBy { get; set; }
    public string? ApplicantName { get; set; }
    public string? PaymentMethod { get; set; }
    public int TotalRecords { get; set; }
}

public class AdmissionPaymentRegisterDto
{
    public int Id { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public int AdmissionApplicationId { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? ApplicantName { get; set; }
    public DateTime ReceiptDate { get; set; }
    public bool IsRefunded { get; set; }
    public decimal? RefundAmount { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? RefundReason { get; set; }
    public int TotalRecords { get; set; }
}

public class AdmissionPaymentDashboardDto
{
    public decimal TodayTotal { get; set; }
    public int TodayCount { get; set; }
    public decimal MonthTotal { get; set; }
    public int MonthCount { get; set; }
    public decimal YearTotal { get; set; }
    public int YearCount { get; set; }
    public decimal PendingTotal { get; set; }
    public int PendingCount { get; set; }
    public decimal RefundedTotal { get; set; }
    public int RefundCount { get; set; }

    public decimal TodayAdmissionFees { get; set; }
    public decimal MonthlyAdmissionFees { get; set; }
    public decimal AdmissionRevenue { get; set; }
    public int PendingAdmissionPayments { get; set; }
    public decimal RefundedAdmissionFees { get; set; }
}
