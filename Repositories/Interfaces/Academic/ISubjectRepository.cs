using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Repositories.Interfaces.Academic;

public interface ISubjectRepository : IBaseRepository<Subject>
{
    Task<List<SubjectListSpResult>> GetListSpAsync(int pageNumber, int pageSize, string? searchTerm);
}
