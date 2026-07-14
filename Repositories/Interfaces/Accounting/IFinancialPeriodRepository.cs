using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.Repositories.Interfaces.Accounting;

public interface IFinancialPeriodRepository : IBaseRepository<FinancialPeriod>
{
    Task<(List<FinancialPeriodListItemDto> Items, int TotalRecords)> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct);
    Task<FinancialPeriod?> GetActivePeriodAsync(CancellationToken ct);
    Task CloseFinancialPeriodAsync(int financialPeriodId, string closedBy, CancellationToken ct = default);
}
