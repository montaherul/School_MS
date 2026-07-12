using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface ISchoolShiftService
{
    Task<PagedResult<SchoolShiftListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<SchoolShiftUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SchoolShiftUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(SchoolShiftUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<IEnumerable<object>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}
