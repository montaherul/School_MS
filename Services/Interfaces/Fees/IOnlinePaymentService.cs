using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IOnlinePaymentService
{
    Task<OnlinePaymentRequest> CreateAsync(int studentId, OnlinePaymentSubmitDto dto, string createdBy, CancellationToken ct = default);
    Task<OnlinePaymentRequest> CreateGatewayPendingAsync(int studentId, int invoiceId, string createdBy, CancellationToken ct = default);
    Task<OnlinePaymentRequest?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OnlinePaymentRequest?> GetByGatewayTransactionIdAsync(string tranId, CancellationToken ct = default);
    Task<List<OnlinePaymentRequestDto>> GetPendingAsync(CancellationToken ct = default);
    Task<List<OnlinePaymentRequestDto>> GetByStudentAsync(int studentId, CancellationToken ct = default);
    Task<bool> VerifyAsync(int id, string verifiedBy, string? adminNotes, CancellationToken ct = default);
    Task<bool> RejectAsync(int id, string rejectedBy, string? adminNotes, CancellationToken ct = default);
    Task ExpireStaleRequestsAsync(CancellationToken ct = default);
}
