using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class AutoWriteOffScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AutoWriteOffScheduler> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromDays(1);

    public AutoWriteOffScheduler(IServiceScopeFactory scopeFactory, ILogger<AutoWriteOffScheduler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AutoWriteOffScheduler started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunWriteOffIfDueAsync(stoppingToken);
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AutoWriteOffScheduler encountered an error.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("AutoWriteOffScheduler stopping.");
    }

    private async Task RunWriteOffIfDueAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAutoWriteOffService>();

        var result = await service.RunAsync(ct);

        _logger.LogInformation(
            "AutoWriteOffScheduler: {Count} invoice(s) written off, total {Total}, {Errors} error(s).",
            result.InvoicesWrittenOff, result.TotalWrittenOff, result.Errors.Count);

        foreach (var error in result.Errors)
            _logger.LogWarning("AutoWriteOffScheduler error: {Error}", error);
    }
}
