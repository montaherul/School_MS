using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Repositories.Interfaces.AI;

namespace SchoolManagementSystem.Services.Implementations.AI.Background;

public class UsageLoggingWorker : BackgroundService
{
    private readonly UsageLoggingChannel _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UsageLoggingWorker> _logger;

    public UsageLoggingWorker(UsageLoggingChannel channel, IServiceScopeFactory scopeFactory, ILogger<UsageLoggingWorker> logger)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("UsageLoggingWorker started");

        await foreach (var entry in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var usageRepo = scope.ServiceProvider.GetRequiredService<IAIUsageRepository>();

                await usageRepo.InsertAsync(
                    entry.StudentId, entry.ConversationId, entry.MessageId, entry.Model,
                    entry.PromptTokens, entry.CompletionTokens, entry.TotalTokens,
                    entry.EstimatedCost, entry.LatencyMs, entry.CreatedBy, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist usage log entry");
            }
        }
    }
}
