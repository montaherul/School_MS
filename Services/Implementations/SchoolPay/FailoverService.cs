using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class FailoverService : IFailoverService
{
    private readonly ISchoolPayRepository _repo;
    private readonly IPaymentRoutingService _routingService;
    private readonly ILogger<FailoverService> _logger;

    public FailoverService(
        ISchoolPayRepository repo,
        IPaymentRoutingService routingService,
        ILogger<FailoverService> logger)
    {
        _repo = repo;
        _routingService = routingService;
        _logger = logger;
    }

    public async Task<List<SchoolPayFailoverStatusDto>> GetFailoverStatusAsync(CancellationToken ct = default)
    {
        var healthStatuses = await _repo.GetLatestHealthStatusAsync(ct);
        var rules = await _repo.GetAllRouteRulesAsync(ct);
        var primaryProviderIds = rules
            .Where(r => r.IsActive && r.Priority == PaymentRoutePriority.Primary)
            .Select(r => r.PaymentProviderId)
            .Distinct()
            .ToHashSet();

        return healthStatuses.Select(h => new SchoolPayFailoverStatusDto
        {
            ProviderId = h.ProviderId,
            ProviderName = h.ProviderName,
            IsHealthy = h.Status == ProviderHealthStatus.Healthy,
            IsPrimary = primaryProviderIds.Contains(h.ProviderId),
            Priority = 0,
            HealthStatus = h.Status,
            LastError = h.LastError,
            LastCheckedAt = h.LastCheckedAt
        }).ToList();
    }

    public async Task<int?> ResolveWithFailoverAsync(decimal amount, string? feeType = null, CancellationToken ct = default)
    {
        var rules = await _repo.GetActiveRouteRulesForAmountAsync(amount, feeType, ct);
        if (rules.Count == 0) return null;

        var healthStatuses = await _repo.GetLatestHealthStatusAsync(ct);
        var healthyProviderIds = healthStatuses
            .Where(h => h.Status == ProviderHealthStatus.Healthy)
            .Select(h => h.ProviderId)
            .ToHashSet();

        foreach (var rule in rules)
        {
            if (healthyProviderIds.Contains(rule.PaymentProviderId))
            {
                _logger.LogInformation("Failover: routed to provider {ProviderId} via rule {RuleName}", rule.PaymentProviderId, rule.RuleName);
                return rule.PaymentProviderId;
            }
        }

        return null;
    }

    public async Task<bool> IsProviderAvailableAsync(int providerId, CancellationToken ct = default)
    {
        var statuses = await _repo.GetLatestHealthStatusAsync(ct);
        var status = statuses.FirstOrDefault(s => s.ProviderId == providerId);
        return status != null && status.Status == ProviderHealthStatus.Healthy;
    }
}
