namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FinanceAnalyticsDashboardDto
{
    public RevenueForecastDto RevenueForecast { get; set; } = new();
    public CollectionPredictionDto CollectionPrediction { get; set; } = new();
    public DefaulterPredictionDto DefaulterPrediction { get; set; } = new();
    public CashFlowForecastDto CashFlowForecast { get; set; } = new();
    public FeeTrendDto FeeTrend { get; set; } = new();
    public BudgetVsActualDto BudgetVsActual { get; set; } = new();
    public List<MonthlyFinanceSummaryDto> MonthlySummaries { get; set; } = [];
}

public class RevenueForecastDto
{
    public decimal CurrentMonthRevenue { get; set; }
    public decimal ProjectedRevenue { get; set; }
    public decimal PreviousMonthRevenue { get; set; }
    public decimal GrowthPercentage { get; set; }
    public List<MonthlyRevenuePoint> MonthlyProjections { get; set; } = [];
}

public class MonthlyRevenuePoint
{
    public string Month { get; set; } = string.Empty;
    public decimal Actual { get; set; }
    public decimal Projected { get; set; }
}

public class CollectionPredictionDto
{
    public decimal EstimatedCollection { get; set; }
    public decimal ExpectedOverdue { get; set; }
    public double CollectionRate { get; set; }
    public int InvoicesDue { get; set; }
    public int OnTrackCount { get; set; }
    public int AtRiskCount { get; set; }
}

public class DefaulterPredictionDto
{
    public int AtRiskStudents { get; set; }
    public decimal AtRiskAmount { get; set; }
    public int CriticalStudents { get; set; }
    public List<DefaulterSegmentDto> Segments { get; set; } = [];
}

public class DefaulterSegmentDto
{
    public string Segment { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public decimal TotalDue { get; set; }
}

public class CashFlowForecastDto
{
    public decimal CurrentBalance { get; set; }
    public decimal ExpectedInflow { get; set; }
    public decimal ExpectedOutflow { get; set; }
    public decimal NetProjection { get; set; }
    public List<CashFlowPoint> Projections { get; set; } = [];
}

public class CashFlowPoint
{
    public string Period { get; set; } = string.Empty;
    public decimal Inflow { get; set; }
    public decimal Outflow { get; set; }
    public decimal Balance { get; set; }
}

public class FeeTrendDto
{
    public decimal TotalCollected { get; set; }
    public decimal TotalOutstanding { get; set; }
    public double CollectionEfficiency { get; set; }
    public List<FeeTrendPoint> Trends { get; set; } = [];
}

public class FeeTrendPoint
{
    public string Month { get; set; } = string.Empty;
    public decimal Collected { get; set; }
    public decimal Target { get; set; }
}

public class BudgetVsActualDto
{
    public decimal TotalBudget { get; set; }
    public decimal TotalActual { get; set; }
    public decimal Variance { get; set; }
    public double VariancePercentage { get; set; }
    public List<BudgetVsActualLine> Lines { get; set; } = [];
}

public class BudgetVsActualLine
{
    public string Category { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal Actual { get; set; }
}

public class MonthlyFinanceSummaryDto
{
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal Collections { get; set; }
    public decimal Outstanding { get; set; }
    public int InvoiceCount { get; set; }
    public int PaymentCount { get; set; }
}
