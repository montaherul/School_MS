using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Accounting;

public interface IFinancialPeriodService
{
    Task<PagedResult<FinancialPeriodListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
    Task<FinancialPeriodUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(FinancialPeriodUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(FinancialPeriodUpsertDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);
    Task ClosePeriodAsync(int id, string closedBy, CancellationToken ct = default);
    Task<List<SelectListItem>> GetPeriodSelectListAsync(bool activeOnly = true, CancellationToken ct = default);
}
