using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Services.Interfaces.Website;

namespace SchoolManagementSystem.Services.Implementations.Website;

public class EventReminderWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EventReminderWorker> _logger;

    public EventReminderWorker(IServiceScopeFactory scopeFactory, ILogger<EventReminderWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EventReminderWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<IEventNotificationService>();

                _logger.LogInformation("EventReminderWorker processing reminders...");
                await notificationService.ProcessRemindersAsync(stoppingToken);
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EventReminderWorker encountered an error.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }

        _logger.LogInformation("EventReminderWorker stopping.");
    }
}
