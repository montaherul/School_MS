using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class EventBus : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<object>> _handlers = new();
    private readonly ILogger<EventBus> _logger;

    public EventBus(ILogger<EventBus> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class
    {
        var eventType = typeof(T);
        _logger.LogInformation("Publishing event {EventType}: {@Event}", eventType.Name, @event);

        if (!_handlers.TryGetValue(eventType, out var handlers)) return;

        foreach (var handler in handlers)
        {
            if (ct.IsCancellationRequested) break;
            try
            {
                var func = (Func<T, CancellationToken, Task>)handler;
                await func(@event, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Event handler failed for {EventType}", eventType.Name);
            }
        }
    }

    public void Subscribe<T>(Func<T, CancellationToken, Task> handler) where T : class
    {
        var eventType = typeof(T);
        _handlers.AddOrUpdate(eventType,
            _ => new List<object> { handler },
            (_, list) => { list.Add(handler); return list; });
        _logger.LogInformation("Handler registered for event {EventType}", eventType.Name);
    }
}
