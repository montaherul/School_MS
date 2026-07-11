using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Repositories.Interfaces.Academic;

public interface ISchoolClassRepository : IBaseRepository<SchoolClass>
{
    Task<List<ClassListSpResult>> GetListSpAsync(int pageNumber, int pageSize, string? searchTerm);
}
