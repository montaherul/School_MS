using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class ScholarshipScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScholarshipScheduler> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(12);

    public ScholarshipScheduler(IServiceScopeFactory scopeFactory, ILogger<ScholarshipScheduler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ScholarshipScheduler started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunScholarshipEngineIfDueAsync(stoppingToken);
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScholarshipScheduler encountered an error.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("ScholarshipScheduler stopping.");
    }

    private async Task RunScholarshipEngineIfDueAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var engine = scope.ServiceProvider.GetRequiredService<IScholarshipEngineService>();

        var result = await engine.RunAsync(ct);

        _logger.LogInformation(
            "ScholarshipScheduler: {Applied} scholarship(s) applied to {Students} student(s), total {Total}, {Errors} error(s).",
            result.ScholarshipsApplied, result.StudentsProcessed, result.TotalDiscountAmount, result.Errors.Count);

        foreach (var error in result.Errors)
            _logger.LogWarning("ScholarshipScheduler error: {Error}", error);
    }
}
