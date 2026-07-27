using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFeePaymentService
{
    Task<PagedResult<FeePaymentListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? feeInvoiceId = null, int? paymentMethod = null, CancellationToken cancellationToken = default);
    Task<FeePaymentUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(FeePaymentUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(FeePaymentUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<(int PaymentId, DateTime PaidAt)> VerifyReceiptCodeAsync(string code, CancellationToken cancellationToken = default);
}
