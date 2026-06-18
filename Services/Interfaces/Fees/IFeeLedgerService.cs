using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFeeLedgerService
{
    Task<PagedResult<FeeLedgerListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? studentId = null, int? transactionType = null, CancellationToken cancellationToken = default);
}
