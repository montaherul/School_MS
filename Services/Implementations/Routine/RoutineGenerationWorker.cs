using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Services.Interfaces.Routine;

namespace SchoolManagementSystem.Services.Implementations.Routine;

public class RoutineGenerationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RoutineGenerationQueue _queue;
    private readonly ILogger<RoutineGenerationWorker> _logger;

    public RoutineGenerationWorker(
        IServiceScopeFactory scopeFactory,
        RoutineGenerationQueue queue,
        ILogger<RoutineGenerationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RoutineGenerationWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var item = await _queue.DequeueAsync(stoppingToken);
                if (item.HasValue)
                {
                    _logger.LogInformation("Processing routine generation for AcademicYearId={AcademicYearId} by {CreatedBy}",
                        item.Value.AcademicYearId, item.Value.CreatedBy);

                    using var scope = _scopeFactory.CreateScope();
                    var engine = scope.ServiceProvider.GetRequiredService<IRoutineEngineService>();
                    await engine.GenerateRoutineAsync(item.Value.AcademicYearId, item.Value.CreatedBy, stoppingToken);

                    _logger.LogInformation("Routine generation completed for AcademicYearId={AcademicYearId}",
                        item.Value.AcademicYearId);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoutineGenerationWorker encountered an error.");
            }
        }

        _logger.LogInformation("RoutineGenerationWorker stopping.");
    }
}
