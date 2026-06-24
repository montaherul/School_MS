using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Services.Interfaces.Website;

namespace SchoolManagementSystem.Services.Implementations.Website;

public class EventNotificationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EventNotificationWorker> _logger;

    public EventNotificationWorker(IServiceScopeFactory scopeFactory, ILogger<EventNotificationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EventNotificationWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<IEventNotificationService>();

                // Process scheduled notifications that are due
                _logger.LogInformation("EventNotificationWorker processing scheduled notifications...");
                await notificationService.ProcessScheduledNotificationsAsync(stoppingToken);

                // Send pending notifications
                var recentNotifications = await notificationService.GetRecentNotificationsAsync(20, stoppingToken);

                var pending = recentNotifications
                    .Where(n => n.Status == Models.Entities.Website.EventNotificationStatus.Pending)
                    .ToList();

                foreach (var notification in pending)
                {
                    if (stoppingToken.IsCancellationRequested) break;

                    _logger.LogInformation("Processing queued notification {NotificationId} for event {EventId}",
                        notification.Id, notification.EventId);

                    await notificationService.SendNotificationAsync(notification.Id, stoppingToken);
                }
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EventNotificationWorker encountered an error.");
            }

            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }

        _logger.LogInformation("EventNotificationWorker stopping.");
    }
}
