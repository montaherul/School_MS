using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Student;

namespace SchoolManagementSystem.Services.Interfaces.Students;

public interface IStudentService
{
    Task<PagedResult<StudentListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? classId = null, int? sectionId = null, int? status = null, CancellationToken cancellationToken = default);
    Task<StudentUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<StudentUpsertDto?> GetByStudentNoAsync(string studentNo, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(StudentUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(StudentUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<StudentUpsertDto?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<int?> GetStudentIdByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetOptionalSubjectsAsync(int classId, CancellationToken cancellationToken = default);
}

