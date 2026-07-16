using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Entities.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class HealthMonitorService : IHealthMonitorService
{
    private readonly ISchoolPayRepository _repo;
    private readonly ILogger<HealthMonitorService> _logger;

    public HealthMonitorService(ISchoolPayRepository repo, ILogger<HealthMonitorService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<SchoolPayHealthStatusDto>> GetLatestHealthStatusAsync(CancellationToken ct = default)
        => await _repo.GetLatestHealthStatusAsync(ct);

    public async Task<List<SchoolPayHealthHistoryDto>> GetHealthHistoryAsync(int providerId, int days = 30, CancellationToken ct = default)
        => await _repo.GetHealthHistoryAsync(providerId, days, ct);

    public async Task<bool> CheckProviderHealthAsync(int providerId, CancellationToken ct = default)
    {
        try
        {
            var provider = await _repo.GetProviderEntityByIdAsync(providerId, ct);
            if (provider == null) return false;

            // Simulate health check — ping the provider
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var isHealthy = await SimulateProviderPingAsync(provider, ct);
            sw.Stop();

            var status = isHealthy ? ProviderHealthStatus.Healthy : ProviderHealthStatus.Unhealthy;
            var responseTime = (int)sw.ElapsedMilliseconds;

            await _repo.RecordHealthCheckAsync(
                providerId, status, responseTime,
                isHealthy ? 100m : 0m,
                1, isHealthy ? 0 : 1,
                isHealthy ? null : "Provider health check failed",
                ct);

            return isHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed for provider {ProviderId}", providerId);
            await _repo.RecordHealthCheckAsync(providerId, ProviderHealthStatus.Unhealthy, 0, 0m, 1, 1, ex.Message, ct);
            return false;
        }
    }

    public async Task RunAllHealthChecksAsync(CancellationToken ct = default)
    {
        var statuses = await _repo.GetLatestHealthStatusAsync(ct);
        foreach (var status in statuses)
        {
            if (ct.IsCancellationRequested) break;
            try
            {
                await CheckProviderHealthAsync(status.ProviderId, ct);
                await Task.Delay(100, ct); // rate limit
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Health check error for provider {ProviderId}", status.ProviderId);
            }
        }
    }

    private async Task<bool> SimulateProviderPingAsync(PaymentProvider provider, CancellationToken ct)
    {
        try
        {
            await Task.Delay(50, ct);
            return provider.IsActive && provider.Status == ProviderStatus.Active;
        }
        catch
        {
            return false;
        }
    }
}
