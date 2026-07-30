using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFinanceAnalyticsService
{
    Task<FinanceAnalyticsDashboardDto> GetDashboardAsync(CancellationToken ct = default);
    Task<List<MonthlyFinanceSummaryDto>> GetMonthlySummariesAsync(int months = 12, CancellationToken ct = default);
    Task<List<DefaulterSegmentDto>> GetDefaulterSegmentsAsync(CancellationToken ct = default);
}
