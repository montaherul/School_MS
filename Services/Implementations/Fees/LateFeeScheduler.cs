using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class LateFeeScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LateFeeScheduler> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6);

    private DateOnly? _lastRunDate;

    public LateFeeScheduler(IServiceScopeFactory scopeFactory, ILogger<LateFeeScheduler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LateFeeScheduler started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunLateFeeIfDueAsync(stoppingToken);
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LateFeeScheduler encountered an error.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("LateFeeScheduler stopping.");
    }

    private async Task RunLateFeeIfDueAsync(CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (_lastRunDate == today)
            return;

        using var scope = _scopeFactory.CreateScope();
        var engine = scope.ServiceProvider.GetRequiredService<ILateFeeEngineService>();

        var result = await engine.RunAsync(ct);

        _lastRunDate = today;

        _logger.LogInformation(
            "LateFeeScheduler: {Count} invoice(s) processed, total {Total} applied, {Errors} error(s).",
            result.InvoicesProcessed, result.TotalLateFeeApplied, result.Errors.Count);

        if (result.Errors.Count > 0)
        {
            foreach (var error in result.Errors)
                _logger.LogWarning("LateFeeScheduler error: {Error}", error);
        }
    }
}
