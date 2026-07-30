using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IFinanceAnalyticsRepository
{
    Task<FinanceAnalyticsDashboardDto> GetDashboardAsync(CancellationToken ct = default);
}
