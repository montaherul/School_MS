using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface ILessonPlanService
{
    Task<PagedResult<LessonPlanListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
    Task<LessonPlanUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(LessonPlanUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(LessonPlanUpsertDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);
    Task ToggleActiveAsync(int id, string updatedBy, CancellationToken ct = default);
    Task BulkActivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default);
    Task BulkDeactivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default);
    Task<byte[]?> ExportPdfAsync(int id, CancellationToken ct = default);
}
