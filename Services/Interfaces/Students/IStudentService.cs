using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Student;

namespace SchoolManagementSystem.Services.Interfaces.Students;

public interface IStudentService
{
    Task<PagedResult<StudentListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<StudentUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(StudentUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(StudentUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}
