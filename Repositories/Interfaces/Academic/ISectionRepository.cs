using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Repositories.Interfaces.Academic;

public interface ISectionRepository : IBaseRepository<Section>
{
    Task<List<SectionListSpResult>> GetListSpAsync(int pageNumber, int pageSize, string? searchTerm);
}
