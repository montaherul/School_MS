using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface ISubjectCategoryService
{
    Task<PagedResult<SubjectCategoryListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<SubjectCategoryUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SubjectCategoryUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(SubjectCategoryUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}
