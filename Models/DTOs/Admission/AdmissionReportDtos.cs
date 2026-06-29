namespace SchoolManagementSystem.Models.DTOs.Admission;

public class AdmissionReportRequest
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int? ClassId { get; set; }
    public int? Status { get; set; }
    public string? Gender { get; set; }
    public string? Religion { get; set; }
    public string? District { get; set; }
    public string? GroupBy { get; set; } = "Month";
}

public class AdmissionRegisterReportDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public List<AdmissionRegisterRow> Rows { get; set; } = new();
    public int TotalRecords => Rows.Count;
}

public class AdmissionRegisterRow
{
    public int SerialNo { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string? NameBangla { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string MotherName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string Religion { get; set; } = string.Empty;
    public string AppliedClass { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
}

public class CollectionReportDto
{
    public decimal TotalCollected { get; set; }
    public int TotalPayments { get; set; }
    public List<CollectionRow> Details { get; set; } = new();
}

public class CollectionRow
{
    public DateTime Date { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

public class ConversionFunnelDto
{
    public int TotalApplications { get; set; }
    public int DocumentVerified { get; set; }
    public int InterviewCompleted { get; set; }
    public int FeePaid { get; set; }
    public int Approved { get; set; }
    public int Converted { get; set; }
    public double ConversionRate { get; set; }
}

public class TrendAnalysisDto
{
    public string PeriodLabel { get; set; } = string.Empty;
    public int PeriodYear { get; set; }
    public int PeriodMonth { get; set; }
    public int TotalApplications { get; set; }
    public int PendingCount { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int ConvertedCount { get; set; }
    public double ConversionRate { get; set; }
}

public class ClassDemandDto
{
    public string ClassName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int TotalApplications { get; set; }
    public int ConvertedCount { get; set; }
    public int PendingCount { get; set; }
    public int ApprovedCount { get; set; }
    public double ConversionRate { get; set; }
    public int GenderCount { get; set; }
    public int ReligionDiversity { get; set; }
    public List<NameCountDto> GenderBreakdown { get; set; } = new();
}

public class RevenueReportDto
{
    public decimal TotalInvoiceAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal TotalDueAmount { get; set; }
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int PendingInvoices { get; set; }
    public double CollectionRate { get; set; }
    public List<RevenueByClassDto> ByClass { get; set; } = new();
    public List<RevenueTrendDto> MonthlyTrend { get; set; } = new();
    public WaiverSummaryDto WaiverSummary { get; set; } = new();
}

public class RevenueByClassDto
{
    public string ClassName { get; set; } = string.Empty;
    public decimal TotalInvoiceAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public int InvoiceCount { get; set; }
    public int PaidCount { get; set; }
}

public class RevenueTrendDto
{
    public string PeriodLabel { get; set; } = string.Empty;
    public int PeriodYear { get; set; }
    public int PeriodMonth { get; set; }
    public decimal TotalInvoiceAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public int InvoiceCount { get; set; }
}

public class WaiverSummaryDto
{
    public int TotalWaivers { get; set; }
    public decimal TotalWaiverAmount { get; set; }
    public double AvgWaiverPercentage { get; set; }
}
