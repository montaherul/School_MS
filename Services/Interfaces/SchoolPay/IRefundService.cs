using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IRefundService
{
    Task<SchoolPayRefundDto?> ProcessRefundAsync(
        int transactionId,
        decimal amount,
        string? reason,
        string processedBy,
        CancellationToken ct = default);
    Task<List<SchoolPayRefundDto>> GetRefundsAsync(CancellationToken ct = default);
}
