using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Teacher;

namespace SchoolManagementSystem.Services.Interfaces.Teachers;

public interface ITeacherService
{
    Task<int> CreateAsync(TeacherUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(TeacherUpsertDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);

    Task<TeacherUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default);
    Task<PagedResult<TeacherListItemDto>> GetPagedAsync(int page, int pageSize, string? search, string? department, string? status, CancellationToken ct = default);

    Task DeactivateAsync(int id, string updatedBy, CancellationToken ct = default);
    Task ActivateAsync(int id, string updatedBy, CancellationToken ct = default);
}
