using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IPaymentAllocationRepository : IBaseRepository<PaymentAllocation>
{
    Task<(List<PaymentAllocationListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? paymentId, int? feeInvoiceId, CancellationToken ct);
}
