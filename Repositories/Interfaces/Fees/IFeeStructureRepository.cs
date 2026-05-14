using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IFeeStructureRepository : IBaseRepository<FeeStructure>
{
    Task<(List<FeeStructureListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, CancellationToken ct);
}
