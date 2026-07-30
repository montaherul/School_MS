using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FinanceAnalyticsService : IFinanceAnalyticsService
{
    private readonly IFinanceAnalyticsRepository _repository;

    public FinanceAnalyticsService(IFinanceAnalyticsRepository repository)
    {
        _repository = repository;
    }

    public async Task<FinanceAnalyticsDashboardDto> GetDashboardAsync(CancellationToken ct = default)
    {
        return await _repository.GetDashboardAsync(ct);
    }

    public async Task<List<MonthlyFinanceSummaryDto>> GetMonthlySummariesAsync(int months = 12, CancellationToken ct = default)
    {
        var dashboard = await _repository.GetDashboardAsync(ct);
        return dashboard.MonthlySummaries.Take(months).ToList();
    }

    public async Task<List<DefaulterSegmentDto>> GetDefaulterSegmentsAsync(CancellationToken ct = default)
    {
        var dashboard = await _repository.GetDashboardAsync(ct);
        return dashboard.DefaulterPrediction.Segments;
    }
}
