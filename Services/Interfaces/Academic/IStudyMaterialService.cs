using Microsoft.AspNetCore.Http;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface IStudyMaterialService
{
    Task<PagedResult<StudyMaterialListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
    Task<StudyMaterialUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(StudyMaterialUpsertDto dto, IFormFile? file, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(StudyMaterialUpsertDto dto, IFormFile? file, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);
    Task ToggleActiveAsync(int id, string updatedBy, CancellationToken ct = default);
    Task BulkActivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default);
    Task BulkDeactivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default);
    Task<string?> GetFilePathAsync(int id, CancellationToken ct = default);
    Task<byte[]?> ExportPdfAsync(int id, CancellationToken ct = default);
}
