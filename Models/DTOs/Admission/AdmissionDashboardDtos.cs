namespace SchoolManagementSystem.Models.DTOs.Admission;

public class AdmissionDashboardDto
{
    public int TodayApplications { get; set; }
    public int WeekApplications { get; set; }
    public int MonthApplications { get; set; }
    
    public int PendingVerification { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Converted { get; set; }
    
    public List<MonthlyTrendDto> MonthlyTrend { get; set; } = new();
    
    public List<NameCountDto> ClassDistribution { get; set; } = new();
    public List<NameCountDto> GenderDistribution { get; set; } = new();
    public List<NameCountDto> ReligionDistribution { get; set; } = new();
    public List<NameCountDto> DistrictDistribution { get; set; } = new();
    
    public int TotalApplications { get; set; }
    public int ConvertedCount { get; set; }
    public double ConversionRate { get; set; }
    
    public decimal TotalInvoiceAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    
    public List<DateCountDto> ApplicationHeatmap { get; set; } = new();
    
    public List<NameCountDto> TopClasses { get; set; } = new();
}

public class MonthlyTrendDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Count { get; set; }
    public int PendingCount { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int ConvertedCount { get; set; }
}

public class NameCountDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DateCountDto
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}
