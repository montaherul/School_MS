using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface ISchoolClassService
{
    Task<PagedResult<SchoolClassListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<SchoolClassUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SchoolClassUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(SchoolClassUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<IEnumerable<SchoolClassListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
}


