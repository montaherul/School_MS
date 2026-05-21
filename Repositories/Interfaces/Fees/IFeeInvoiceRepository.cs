using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IFeeInvoiceRepository : IBaseRepository<FeeInvoice>
{
    Task<(List<FeeInvoiceListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? studentId, CancellationToken ct);
}
