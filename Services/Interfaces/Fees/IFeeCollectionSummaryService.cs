using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFeeCollectionSummaryService
{
    Task<PagedResult<FeeCollectionSummaryListItemDto>> GetPagedAsync(int page, int pageSize, string? search, DateOnly? fromDate = null, DateOnly? toDate = null, CancellationToken cancellationToken = default);
    Task<FeeCollectionSummaryUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(FeeCollectionSummaryUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(FeeCollectionSummaryUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}
