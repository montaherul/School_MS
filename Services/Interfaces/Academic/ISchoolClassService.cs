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
    Task<IEnumerable<SchoolManagementSystem.Models.Entities.Academic.SchoolClass>> GetAllSchoolClassesAsync(CancellationToken cancellationToken = default);

    Task<SchoolClassListItemDto> CloneAsync(int id, string createdBy, CancellationToken cancellationToken = default);
    Task ArchiveAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task ToggleActiveAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task BulkActivateAsync(List<int> ids, string updatedBy, CancellationToken cancellationToken = default);
    Task BulkDeactivateAsync(List<int> ids, string updatedBy, CancellationToken cancellationToken = default);
    Task<bool> IsNameUniqueAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> CanDeleteAsync(int id, CancellationToken cancellationToken = default);
}
