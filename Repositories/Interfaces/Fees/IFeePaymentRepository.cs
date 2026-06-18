using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IFeePaymentRepository : IBaseRepository<Payment>
{
    Task<(List<FeePaymentListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? feeInvoiceId, int? paymentMethod, CancellationToken ct);
}
