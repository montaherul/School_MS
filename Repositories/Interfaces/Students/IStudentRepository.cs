using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.DTOs.Student;

namespace SchoolManagementSystem.Repositories.Interfaces.Students;

public interface IStudentRepository : IBaseRepository<Student>
{
    Task<(List<StudentListItemDto> items, int totalRecords)> GetPagedAsync(int page, int pageSize, string? search, int? classId, int? sectionId, int? status, CancellationToken ct);
    Task<StudentUpsertDto?> GetForEditAsync(int id, CancellationToken ct);
    Task<StudentUpsertDto?> GetByStudentNoAsync(string studentNo, CancellationToken ct);
}
