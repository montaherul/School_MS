using System.Threading.Channels;
using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Services.Implementations.AI.Background;

public class UsageLogEntry
{
    public int StudentId { get; init; }
    public int? ConversationId { get; init; }
    public int? MessageId { get; init; }
    public string Model { get; init; } = string.Empty;
    public int PromptTokens { get; init; }
    public int CompletionTokens { get; init; }
    public int TotalTokens { get; init; }
    public decimal EstimatedCost { get; init; }
    public int? LatencyMs { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
}

public class UsageLoggingChannel
{
    private readonly Channel<UsageLogEntry> _channel;

    public UsageLoggingChannel()
    {
        _channel = Channel.CreateBounded<UsageLogEntry>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.DropOldest
        });
    }

    public ChannelWriter<UsageLogEntry> Writer => _channel.Writer;
    public ChannelReader<UsageLogEntry> Reader => _channel.Reader;

    public ValueTask EnqueueAsync(UsageLogEntry entry, CancellationToken ct)
        => _channel.Writer.WriteAsync(entry, ct);
}
