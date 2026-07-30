namespace SchoolManagementSystem.Models.DTOs.Finance;

public class CashFlowStatementDto
{
    public DateTime? AsOfDate { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public CashFlowSectionDto OperatingActivities { get; set; } = new();
    public CashFlowSectionDto InvestingActivities { get; set; } = new();
    public CashFlowSectionDto FinancingActivities { get; set; } = new();
    public decimal NetCashFlow { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
}

public class CashFlowSectionDto
{
    public string SectionName { get; set; } = string.Empty;
    public List<CashFlowLineDto> Lines { get; set; } = new();
    public decimal Total { get; set; }
}

public class CashFlowLineDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsTotal { get; set; }
}

public class CashFlowFilterDto
{
    public int Year { get; set; } = DateTime.UtcNow.Year;
    public int? Month { get; set; }
    public int? PeriodType { get; set; } = 3;
    public int? FromMonth { get; set; }
    public int? ToMonth { get; set; }
}
