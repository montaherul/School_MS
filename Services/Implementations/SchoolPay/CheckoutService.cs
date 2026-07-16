using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class CheckoutService : ICheckoutService
{
    private readonly IProviderManagementService _providerManagement;
    private readonly GatewayFactory _gatewayFactory;
    private readonly IOnlinePaymentService _onlinePaymentService;
    private readonly IFailoverService _failoverService;
    private readonly IEventBus _eventBus;
    private readonly IPaymentMethodManagementService _methodService;
    private readonly ILogger<CheckoutService> _logger;

    public CheckoutService(
        IProviderManagementService providerManagement,
        GatewayFactory gatewayFactory,
        IOnlinePaymentService onlinePaymentService,
        IFailoverService failoverService,
        IEventBus eventBus,
        IPaymentMethodManagementService methodService,
        ILogger<CheckoutService> logger)
    {
        _providerManagement = providerManagement;
        _gatewayFactory = gatewayFactory;
        _onlinePaymentService = onlinePaymentService;
        _failoverService = failoverService;
        _eventBus = eventBus;
        _methodService = methodService;
        _logger = logger;
    }

    public async Task<List<SchoolPayProviderMethodDto>> GetAvailablePaymentMethodsAsync(CancellationToken ct = default)
        => await _methodService.GetActiveMethodsForCheckoutAsync(ct);

    public async Task<List<SchoolPayProviderDto>> GetAvailableProvidersAsync(decimal amount, string? feeType = null, CancellationToken ct = default)
    {
        var providers = await _providerManagement.GetActiveProvidersForCheckoutAsync(amount, feeType, ct);
        var failoverProviderId = await _failoverService.ResolveWithFailoverAsync(amount, feeType, ct);

        foreach (var p in providers)
        {
            p.IsActive = p.Id == failoverProviderId;
        }

        return providers;
    }

    public async Task<SchoolPayCheckoutResponseDto> InitiateCheckoutAsync(
        int onlinePaymentRequestId,
        string providerCode,
        string? paymentMethodCode,
        string? returnUrl,
        string? cancelUrl,
        CancellationToken ct = default)
    {
        var provider = _gatewayFactory.GetProvider(providerCode);
        if (provider == null)
        {
            return new SchoolPayCheckoutResponseDto { Success = false, ErrorMessage = $"Provider '{providerCode}' not found" };
        }

        var request = await _onlinePaymentService.GetByIdAsync(onlinePaymentRequestId, ct);
        if (request == null)
        {
            return new SchoolPayCheckoutResponseDto { Success = false, ErrorMessage = "Payment request not found" };
        }

        if (request.Status == OnlinePaymentRequestStatus.Verified)
        {
            return new SchoolPayCheckoutResponseDto { Success = false, ErrorMessage = "Payment already verified" };
        }

        var methodCode = !string.IsNullOrEmpty(paymentMethodCode)
            ? paymentMethodCode
            : await GetDefaultMethodCodeAsync(ct);

        var initResult = await provider.InitiatePaymentAsync(
            onlinePaymentRequestId,
            request.Amount,
            $"SCH{onlinePaymentRequestId:D6}",
            $"Fee Invoice #{request.FeeInvoice?.InvoiceNo}",
            null, null, null,
            returnUrl, null, cancelUrl, null,
            methodCode,
            ct);

        if (initResult.Success)
        {
            await _eventBus.PublishAsync(new SchoolPayPaymentEvent
            {
                EventType = "Initiated",
                TransactionReference = initResult.TransactionReference,
                ProviderCode = providerCode,
                Amount = request.Amount
            }, ct);
        }

        return new SchoolPayCheckoutResponseDto
        {
            Success = initResult.Success,
            CheckoutUrl = initResult.GatewayPageUrl,
            TransactionReference = initResult.TransactionReference ?? string.Empty,
            ErrorMessage = initResult.ErrorMessage
        };
    }

    public async Task<SchoolPayCheckoutResponseDto> InitiateDirectCheckoutAsync(
        int invoiceId,
        int studentId,
        string providerCode,
        string? paymentMethodCode,
        string? returnUrl,
        string? cancelUrl,
        CancellationToken ct = default)
    {
        try
        {
            var request = await _onlinePaymentService.CreateGatewayPendingAsync(studentId, invoiceId, "SchoolPay", ct);
            return await InitiateCheckoutAsync(request.Id, providerCode, paymentMethodCode, returnUrl, cancelUrl, ct);
        }
        catch (InvalidOperationException ex)
        {
            return new SchoolPayCheckoutResponseDto { Success = false, ErrorMessage = ex.Message };
        }
    }

    private async Task<string?> GetDefaultMethodCodeAsync(CancellationToken ct)
    {
        var methods = await _methodService.GetActiveMethodsForCheckoutAsync(ct);
        return methods.FirstOrDefault(m => m.IsDefault)?.Code
            ?? methods.FirstOrDefault()?.Code;
    }
}
