using System.Data;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Models.Entities.AI;
using SchoolManagementSystem.Repositories.Interfaces.AI;

namespace SchoolManagementSystem.Repositories.Implementations.AI;

public class AIUsageRepository : BaseRepository<AIUsage>, IAIUsageRepository
{
    public AIUsageRepository(SchoolDbContext db) : base(db) { }

    public async Task<int> InsertAsync(int studentId, int? conversationId, int? messageId, string model, int promptTokens, int completionTokens, int totalTokens, decimal estimatedCost, int? latencyMs, string createdBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIUsage_Insert";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@StudentId", studentId);
        AddParameter(cmd, "@ConversationId", conversationId);
        AddParameter(cmd, "@MessageId", messageId);
        AddParameter(cmd, "@Model", model);
        AddParameter(cmd, "@PromptTokens", promptTokens);
        AddParameter(cmd, "@CompletionTokens", completionTokens);
        AddParameter(cmd, "@TotalTokens", totalTokens);
        AddParameter(cmd, "@EstimatedCost", estimatedCost);
        AddParameter(cmd, "@LatencyMs", latencyMs);
        AddParameter(cmd, "@CreatedBy", createdBy);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        var id = Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
        return id;
    }

    public async Task<List<UsageDailySummaryDto>> GetDailySummaryAsync(int? studentId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIUsage_DailySummary";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@StudentId", studentId);
        AddParameter(cmd, "@StartDate", startDate?.ToDateTime(TimeOnly.MinValue));
        AddParameter(cmd, "@EndDate", endDate?.ToDateTime(TimeOnly.MinValue));

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<UsageDailySummaryDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new UsageDailySummaryDto
            {
                UsageDate = GetDateTime(reader, "UsageDate"),
                Model = GetString(reader, "Model"),
                ConversationCount = GetInt32(reader, "ConversationCount"),
                RequestCount = GetInt32(reader, "RequestCount"),
                TotalPromptTokens = GetInt32(reader, "TotalPromptTokens"),
                TotalCompletionTokens = GetInt32(reader, "TotalCompletionTokens"),
                TotalTokens = GetInt32(reader, "TotalTokens"),
                TotalCost = GetDecimal(reader, "TotalCost"),
                AvgLatencyMs = Convert.ToDouble(GetDecimal(reader, "AvgLatencyMs"))
            });
        }
        return items;
    }
}
