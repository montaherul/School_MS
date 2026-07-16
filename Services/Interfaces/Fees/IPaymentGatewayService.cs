using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IPaymentGatewayService
{
    Task<SslCommerzInitResponse?> InitiatePaymentAsync(int onlinePaymentRequestId, string? preferredCardType = null, CancellationToken ct = default);
    Task<SslCommerzValidationResponse?> ValidateTransactionAsync(string valId, CancellationToken ct = default);
    Task<bool> ProcessIpnAsync(string? bankTranId, string? tranId, string? valId, string status, CancellationToken ct = default);
}
