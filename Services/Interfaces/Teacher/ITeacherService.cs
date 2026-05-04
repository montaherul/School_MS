using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Teacher;

namespace SchoolManagementSystem.Services.Interfaces.Teachers;

public interface ITeacherService
{
    Task<PagedResult<TeacherListItemDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default);

    Task<TeacherUpsertDto?> GetForEditAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        TeacherUpsertDto dto,
        string createdBy,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        TeacherUpsertDto dto,
        string updatedBy,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        string updatedBy,
        CancellationToken cancellationToken = default);
}