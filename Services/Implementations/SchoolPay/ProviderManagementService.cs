using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Entities.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class ProviderManagementService : IProviderManagementService
{
    private readonly ISchoolPayRepository _repository;
    private readonly ILogger<ProviderManagementService> _logger;
    private readonly GatewayFactory _gatewayFactory;

    public ProviderManagementService(
        ISchoolPayRepository repository,
        ILogger<ProviderManagementService> logger,
        GatewayFactory gatewayFactory)
    {
        _repository = repository;
        _logger = logger;
        _gatewayFactory = gatewayFactory;
    }

    public async Task<List<SchoolPayProviderListDto>> GetAllProvidersAsync(CancellationToken ct = default)
        => await _repository.GetAllProvidersAsync(ct);

    public async Task<SchoolPayProviderDto?> GetProviderByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetProviderByIdAsync(id, ct);

    public async Task<SchoolPayProviderDto?> GetProviderByCodeAsync(string code, CancellationToken ct = default)
    {
        var provider = await _repository.GetProviderEntityByCodeAsync(code, ct);
        if (provider == null) return null;
        return await _repository.GetProviderByIdAsync(provider.Id, ct);
    }

    public async Task<int> CreateProviderAsync(SchoolPayProviderUpsertDto dto, string createdBy, CancellationToken ct = default)
    {
        var provider = new PaymentProvider
        {
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            LogoUrl = dto.LogoUrl,
            IsSandbox = dto.IsSandbox,
            SupportsRefund = dto.SupportsRefund,
            SupportsSettlement = dto.SupportsSettlement,
            MaxRetryAttempts = dto.MaxRetryAttempts,
            SupportedCurrencies = dto.SupportedCurrencies,
            ClassName = dto.ClassName,
            Priority = dto.Priority,
            Status = ProviderStatus.Active,
            IsActive = true,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        var configs = dto.Configurations.Select(kvp => new PaymentProviderConfiguration
        {
            Key = kvp.Key,
            Value = kvp.Value,
            IsActive = true,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        return await _repository.CreateProviderAsync(provider, configs, ct);
    }

    public async Task<bool> UpdateProviderAsync(int id, SchoolPayProviderUpsertDto dto, string updatedBy, CancellationToken ct = default)
    {
        var provider = await _repository.GetProviderEntityByIdAsync(id, ct);
        if (provider == null) return false;

        provider.Name = dto.Name;
        provider.Description = dto.Description;
        provider.LogoUrl = dto.LogoUrl;
        provider.IsSandbox = dto.IsSandbox;
        provider.SupportsRefund = dto.SupportsRefund;
        provider.SupportsSettlement = dto.SupportsSettlement;
        provider.MaxRetryAttempts = dto.MaxRetryAttempts;
        provider.SupportedCurrencies = dto.SupportedCurrencies;
        provider.ClassName = dto.ClassName;
        provider.Priority = dto.Priority;
        provider.UpdatedBy = updatedBy;
        provider.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateProviderAsync(provider, ct);

        foreach (var kvp in dto.Configurations)
        {
            await _repository.UpsertProviderConfigurationAsync(id, kvp.Key, kvp.Value, updatedBy, ct);
        }
        return true;
    }

    public async Task<bool> ToggleProviderStatusAsync(int id, bool isActive, string updatedBy, CancellationToken ct = default)
    {
        var provider = await _repository.GetProviderEntityByIdAsync(id, ct);
        if (provider == null) return false;
        provider.IsActive = isActive;
        provider.UpdatedBy = updatedBy;
        provider.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateProviderAsync(provider, ct);
        return true;
    }

    public async Task<bool> DeleteProviderAsync(int id, CancellationToken ct = default)
    {
        var provider = await _repository.GetProviderEntityByIdAsync(id, ct);
        if (provider == null) return false;
        provider.IsDeleted = true;
        provider.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateProviderAsync(provider, ct);
        return true;
    }

    public async Task<bool> UpdateProviderPriorityAsync(int id, int priority, CancellationToken ct = default)
    {
        var provider = await _repository.GetProviderEntityByIdAsync(id, ct);
        if (provider == null) return false;
        provider.Priority = priority;
        provider.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateProviderAsync(provider, ct);
        return true;
    }

    public async Task<bool> ToggleSandboxModeAsync(int id, bool isSandbox, CancellationToken ct = default)
    {
        var provider = await _repository.GetProviderEntityByIdAsync(id, ct);
        if (provider == null) return false;
        provider.IsSandbox = isSandbox;
        provider.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateProviderAsync(provider, ct);
        return true;
    }

    public async Task<List<SchoolPayProviderDto>> GetActiveProvidersForCheckoutAsync(decimal amount, string? feeType = null, CancellationToken ct = default)
    {
        var providers = await _repository.GetActiveProviderDtosAsync(ct);
        var result = new List<SchoolPayProviderDto>();
        foreach (var p in providers)
        {
            var gatewayProvider = _gatewayFactory.GetProvider(p.Code);
            if (gatewayProvider == null || !gatewayProvider.IsAvailable) continue;
            p.Methods = await _repository.GetPaymentMethodsAsync(p.Id, ct);
            result.Add(p);
        }
        return result;
    }
}
