using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public enum AdmissionWorkItemType
{
    BulkEmail,
    BulkIdCardGeneration,
    BulkConversion
}

public record AdmissionWorkItem
{
    public AdmissionWorkItemType WorkItemType { get; init; }
    public Guid WorkItemId { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public string? Payload { get; init; }
    public required Func<CancellationToken, Task> Callback { get; init; }
}

public class AdmissionBackgroundQueue
{
    private readonly Channel<AdmissionWorkItem> _queue;

    public AdmissionBackgroundQueue()
    {
        _queue = Channel.CreateBounded<AdmissionWorkItem>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = true
        });
    }

    public async Task EnqueueAsync(Func<CancellationToken, Task> workItem, CancellationToken ct = default)
    {
        await _queue.Writer.WriteAsync(new AdmissionWorkItem
        {
            WorkItemType = AdmissionWorkItemType.BulkConversion,
            Callback = workItem
        }, ct);
    }

    public async Task EnqueueAsync(AdmissionWorkItem workItem, CancellationToken ct = default)
    {
        await _queue.Writer.WriteAsync(workItem, ct);
    }

    public async Task<AdmissionWorkItem> DequeueAsync(CancellationToken ct)
    {
        return await _queue.Reader.ReadAsync(ct);
    }

    public bool TryComplete()
    {
        return _queue.Writer.TryComplete();
    }

    public int GetQueueLength()
    {
        return _queue.Reader.Count;
    }
}

public class AdmissionBackgroundWorker : BackgroundService
{
    private readonly AdmissionBackgroundQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AdmissionBackgroundWorker> _logger;

    public AdmissionBackgroundWorker(
        AdmissionBackgroundQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<AdmissionBackgroundWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Admission background worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await _queue.DequeueAsync(stoppingToken);
                _logger.LogInformation("Processing work item {WorkItemId} of type {WorkItemType}",
                    workItem.WorkItemId, workItem.WorkItemType);
                await workItem.Callback(stoppingToken);
                _logger.LogInformation("Completed work item {WorkItemId} of type {WorkItemType}",
                    workItem.WorkItemId, workItem.WorkItemType);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admission background worker error");
            }
        }
    }
}
