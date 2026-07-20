using System.Data;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Models.Entities.AI;
using SchoolManagementSystem.Repositories.Interfaces.AI;

namespace SchoolManagementSystem.Repositories.Implementations.AI;

public class AIMessageRepository : BaseRepository<AIMessage>, IAIMessageRepository
{
    public AIMessageRepository(SchoolDbContext db) : base(db) { }

    public async Task<int> InsertAsync(int conversationId, string role, string content, int? promptTokens, int? completionTokens, string? model, int? latencyMs, string createdBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIMessage_Insert";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@ConversationId", conversationId);
        AddParameter(cmd, "@Role", role);
        AddParameter(cmd, "@Content", content);
        AddParameter(cmd, "@PromptTokens", promptTokens);
        AddParameter(cmd, "@CompletionTokens", completionTokens);
        AddParameter(cmd, "@Model", model);
        AddParameter(cmd, "@LatencyMs", latencyMs);
        AddParameter(cmd, "@CreatedBy", createdBy);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        var id = Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
        return id;
    }

    public async Task<List<MessageDto>> ListAsync(int conversationId, int studentId, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIMessage_List";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@ConversationId", conversationId);
        AddParameter(cmd, "@StudentId", studentId);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var messages = new List<MessageDto>();
        while (await reader.ReadAsync(ct))
        {
            messages.Add(new MessageDto
            {
                Id = GetInt32(reader, "Id"),
                ConversationId = GetInt32(reader, "ConversationId"),
                Role = GetString(reader, "Role"),
                Content = GetString(reader, "Content"),
                CreatedAt = GetDateTime(reader, "CreatedAt")
            });
        }
        return messages;
    }
}
