using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface ISubjectService
{
    Task<PagedResult<SubjectListItemDto>> GetPagedAsync(int page, int pageSize, string? search, string? group = null, string? status = null, CancellationToken cancellationToken = default);
    Task<SubjectUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SubjectUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(SubjectUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<IDictionary<string?, List<SubjectListItemDto>>> GetGroupedSubjectsAsync(CancellationToken ct = default);
    Task ToggleActiveAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task BulkActivateAsync(List<int> ids, string updatedBy, CancellationToken cancellationToken = default);
    Task BulkDeactivateAsync(List<int> ids, string updatedBy, CancellationToken cancellationToken = default);
    Task BulkImportAsync(List<SubjectUpsertDto> dtos, string createdBy, CancellationToken cancellationToken = default);
    Task<List<SubjectListItemDto>> BulkExportAsync(CancellationToken cancellationToken = default);
}
