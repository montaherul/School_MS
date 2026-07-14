using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.Repositories.Interfaces.Accounting;

public interface IChartOfAccountRepository : IBaseRepository<ChartOfAccount>
{
    Task<(List<AccountListItemDto> Items, int TotalRecords)> GetPagedAsync(int page, int pageSize, string? search, int? accountType, CancellationToken ct);
    Task<List<AccountTreeDto>> GetTreeAsync(CancellationToken ct);
    Task<string> GenerateAccountCodeAsync(int accountType, CancellationToken ct);
}
