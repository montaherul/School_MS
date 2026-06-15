using System.Collections.Concurrent;

namespace SchoolManagementSystem.Middleware;

public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, MetricsEntry> _metrics = new();
    private static readonly object _lock = new();

    public MetricsMiddleware(RequestDelegate next) => _next = next;

    public static IReadOnlyDictionary<string, MetricsEntry> Snapshot()
    {
        lock (_lock) { return _metrics.ToDictionary(kv => kv.Key, kv => kv.Value with { }); }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            var path = context.Request.Path.Value ?? "unknown";
            var method = context.Request.Method;
            var key = $"{method} {path}";
            var status = context.Response.StatusCode;

            lock (_lock)
            {
                var entry = _metrics.GetOrAdd(key, _ => new MetricsEntry());
                entry.Count++;
                entry.TotalDurationMs += sw.ElapsedMilliseconds;
                entry.LastDurationMs = sw.ElapsedMilliseconds;
                entry.LastStatus = status;
                entry.LastCalledAt = DateTime.UtcNow;
            }
        }
    }
}

public record MetricsEntry
{
    public long Count { get; set; }
    public double TotalDurationMs { get; set; }
    public double LastDurationMs { get; set; }
    public int LastStatus { get; set; }
    public DateTime LastCalledAt { get; set; }
    public double AverageDurationMs => Count > 0 ? Math.Round(TotalDurationMs / Count, 2) : 0;
}
