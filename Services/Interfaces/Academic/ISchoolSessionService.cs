using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface ISchoolSessionService
{
    Task<PagedResult<SchoolSessionListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<SchoolSessionUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<List<SchoolSessionListItemDto>> GetActiveSessionsAsync(CancellationToken ct = default);
    Task<int> CreateAsync(SchoolSessionUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(SchoolSessionUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}
