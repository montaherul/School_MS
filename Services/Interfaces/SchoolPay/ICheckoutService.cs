using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface ICheckoutService
{
    Task<List<SchoolPayProviderDto>> GetAvailableProvidersAsync(decimal amount, string? feeType = null, CancellationToken ct = default);
    Task<List<SchoolPayProviderMethodDto>> GetAvailablePaymentMethodsAsync(CancellationToken ct = default);
    Task<SchoolPayCheckoutResponseDto> InitiateCheckoutAsync(
        int onlinePaymentRequestId,
        string providerCode,
        string? paymentMethodCode,
        string? returnUrl,
        string? cancelUrl,
        CancellationToken ct = default);
    Task<SchoolPayCheckoutResponseDto> InitiateDirectCheckoutAsync(
        int invoiceId,
        int studentId,
        string providerCode,
        string? paymentMethodCode,
        string? returnUrl,
        string? cancelUrl,
        CancellationToken ct = default);
}
