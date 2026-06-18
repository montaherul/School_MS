using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IFeeInvoiceItemRepository : IBaseRepository<FeeInvoiceItem>
{
    Task<(List<FeeInvoiceItemListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? feeInvoiceId, CancellationToken ct);
}
