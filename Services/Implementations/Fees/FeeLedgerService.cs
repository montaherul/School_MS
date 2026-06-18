using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeLedgerService : IFeeLedgerService
{
    private readonly IFeeLedgerRepository _repository;

    public FeeLedgerService(IFeeLedgerRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<FeeLedgerListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? studentId = null, int? transactionType = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _repository.GetListByStoredProcedureAsync(page, pageSize, search, studentId, transactionType, cancellationToken);
        return new PagedResult<FeeLedgerListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }
}
