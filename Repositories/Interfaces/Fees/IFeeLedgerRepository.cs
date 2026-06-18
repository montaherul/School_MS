using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IFeeLedgerRepository : IBaseRepository<FeeLedger>
{
    Task<(List<FeeLedgerListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? studentId, int? transactionType, CancellationToken ct);
}
