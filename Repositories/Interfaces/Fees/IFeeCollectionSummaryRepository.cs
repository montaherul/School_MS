using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IFeeCollectionSummaryRepository : IBaseRepository<FeeCollectionSummary>
{
    Task<(List<FeeCollectionSummaryListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, DateOnly? fromDate, DateOnly? toDate, CancellationToken ct);
}
