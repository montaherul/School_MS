using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Repositories.Interfaces.Academic;

public interface IAcademicYearRepository : IBaseRepository<AcademicYear>
{
    Task<List<AcademicYearSpResult>> GetListSpAsync(int pageNumber, int pageSize, string? searchTerm);
}
