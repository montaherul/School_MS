using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Repositories.Interfaces.Academic;

public interface IClassSubjectRepository : IBaseRepository<ClassSubject>
{
    Task<List<ClassSubjectListItemDto>> GetPagedBySpAsync(int page, int pageSize, int? classId, string? groupName, string? search);
}
