using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Repositories.Implementations.Fees;

public class FinanceAnalyticsRepository : IFinanceAnalyticsRepository
{
    private readonly SchoolDbContext _db;

    public FinanceAnalyticsRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<FinanceAnalyticsDashboardDto> GetDashboardAsync(CancellationToken ct = default)
    {
        var result = new FinanceAnalyticsDashboardDto();

        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFinanceAnalyticsDashboard";
        command.CommandType = CommandType.StoredProcedure;

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);

            // Result Set 1: Summary KPIs
            if (await reader.ReadAsync(ct))
            {
                result.RevenueForecast.CurrentMonthRevenue = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                result.RevenueForecast.PreviousMonthRevenue = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
                result.CollectionPrediction.AtRiskCount = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                result.FeeTrend.TotalOutstanding = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3);
                result.CollectionPrediction.InvoicesDue = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);

                result.RevenueForecast.GrowthPercentage = result.RevenueForecast.PreviousMonthRevenue > 0
                    ? Math.Round((result.RevenueForecast.CurrentMonthRevenue - result.RevenueForecast.PreviousMonthRevenue)
                        / result.RevenueForecast.PreviousMonthRevenue * 100, 2)
                    : 0;
                result.RevenueForecast.ProjectedRevenue = result.RevenueForecast.CurrentMonthRevenue * 12;
            }

            // Result Set 2: Revenue Monthly Projections
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    result.RevenueForecast.MonthlyProjections.Add(new MonthlyRevenuePoint
                    {
                        Month = reader.GetString(0),
                        Actual = reader.GetDecimal(1),
                        Projected = reader.GetDecimal(2)
                    });
                }
            }

            // Result Set 3: Cash Flow Projections
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    result.CashFlowForecast.Projections.Add(new CashFlowPoint
                    {
                        Period = reader.GetString(0),
                        Inflow = reader.GetDecimal(1),
                        Outflow = reader.GetDecimal(2),
                        Balance = reader.GetDecimal(3)
                    });
                }

                result.CashFlowForecast.ExpectedInflow = result.CashFlowForecast.Projections.Sum(p => p.Inflow);
                result.CashFlowForecast.ExpectedOutflow = result.CashFlowForecast.Projections.Sum(p => p.Outflow);
                result.CashFlowForecast.NetProjection = result.CashFlowForecast.ExpectedInflow - result.CashFlowForecast.ExpectedOutflow;
                result.CashFlowForecast.CurrentBalance = result.CashFlowForecast.Projections.FirstOrDefault()?.Balance ?? 0;
            }

            // Result Set 4: Fee Trends
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    result.FeeTrend.Trends.Add(new FeeTrendPoint
                    {
                        Month = reader.GetString(0),
                        Collected = reader.GetDecimal(1),
                        Target = reader.GetDecimal(2)
                    });
                }

                result.FeeTrend.TotalCollected = result.FeeTrend.Trends.Sum(t => t.Collected);
                var totalTarget = result.FeeTrend.Trends.Sum(t => t.Target);
                result.FeeTrend.CollectionEfficiency = totalTarget > 0
                    ? Math.Round((double)(result.FeeTrend.TotalCollected / totalTarget * 100), 1)
                    : 0;
                result.CollectionPrediction.CollectionRate = result.FeeTrend.CollectionEfficiency;
            }

            // Result Set 5: Budget vs Actual by Category
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    var line = new BudgetVsActualLine
                    {
                        Category = reader.GetString(0),
                        Budget = reader.GetDecimal(1),
                        Actual = reader.GetDecimal(2)
                    };
                    result.BudgetVsActual.Lines.Add(line);
                    result.BudgetVsActual.TotalBudget += line.Budget;
                    result.BudgetVsActual.TotalActual += line.Actual;
                }

                result.BudgetVsActual.Variance = result.BudgetVsActual.TotalBudget - result.BudgetVsActual.TotalActual;
                result.BudgetVsActual.VariancePercentage = result.BudgetVsActual.TotalBudget > 0
                    ? Math.Round((double)result.BudgetVsActual.Variance / (double)result.BudgetVsActual.TotalBudget * 100, 1)
                    : 0;
            }

            // Result Set 6: Monthly Summaries
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    result.MonthlySummaries.Add(new MonthlyFinanceSummaryDto
                    {
                        Month = reader.GetString(0),
                        Year = reader.GetInt32(1),
                        Revenue = reader.GetDecimal(2),
                        PaymentCount = reader.GetInt32(3),
                        InvoiceCount = reader.GetInt32(4),
                        Outstanding = reader.GetDecimal(5),
                        Collections = reader.GetDecimal(2)
                    });
                }
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }

        // Compute derived KPIs
        result.CollectionPrediction.EstimatedCollection = result.RevenueForecast.CurrentMonthRevenue * 1.1m;
        result.CollectionPrediction.ExpectedOverdue = result.FeeTrend.TotalOutstanding * 0.3m;
        result.CollectionPrediction.OnTrackCount = Math.Max(0, result.CollectionPrediction.InvoicesDue - result.CollectionPrediction.AtRiskCount);

        return result;
    }
}
