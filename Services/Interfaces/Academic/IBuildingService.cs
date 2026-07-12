using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface IBuildingService
{
    Task<PagedResult<BuildingListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<BuildingUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(BuildingUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(BuildingUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}
