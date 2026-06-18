using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFeeReceiptService
{
    Task<FeeReceiptDto?> GetReceiptDataAsync(int paymentId, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateReceiptPdfAsync(int paymentId, CancellationToken cancellationToken = default);
    string GenerateVerificationCode(int paymentId, DateTime paidAt);
}
