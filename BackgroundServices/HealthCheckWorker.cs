using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.BackgroundServices;

public class HealthCheckWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<HealthCheckWorker> _logger;

    public HealthCheckWorker(IServiceScopeFactory scopeFactory, ILogger<HealthCheckWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HealthCheckWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Run health checks every 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var monitor = scope.ServiceProvider.GetRequiredService<IHealthMonitorService>();
                await monitor.RunAllHealthChecksAsync(stoppingToken);

                _logger.LogInformation("HealthCheckWorker completed cycle");
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HealthCheckWorker error");
            }
        }

        _logger.LogInformation("HealthCheckWorker stopped");
    }
}
