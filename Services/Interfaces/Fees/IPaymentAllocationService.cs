using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IPaymentAllocationService
{
    Task<PagedResult<PaymentAllocationListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? paymentId, int? feeInvoiceId, CancellationToken cancellationToken = default);
    Task<PaymentAllocationUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(PaymentAllocationUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(PaymentAllocationUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}
