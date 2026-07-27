using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFeeTypeService
{
    Task<PagedResult<FeeTypeListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<FeeTypeUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(FeeTypeUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(FeeTypeUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}
