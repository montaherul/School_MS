using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Accounting;

public interface IChartOfAccountService
{
    Task<PagedResult<AccountListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? accountType, CancellationToken ct = default);
    Task<AccountUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(AccountUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(AccountUpsertDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);
    Task<List<AccountTreeDto>> GetTreeAsync(CancellationToken ct = default);
    Task<string> GenerateAccountCodeAsync(int accountType, CancellationToken ct = default);
    Task<List<SelectListItem>> GetAccountSelectListAsync(CancellationToken ct = default);
    Task<List<SelectListItem>> GetActiveAccountSelectListAsync(CancellationToken ct = default);
}
