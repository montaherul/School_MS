using System.Data;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Models.Entities.AI;
using SchoolManagementSystem.Repositories.Interfaces.AI;

namespace SchoolManagementSystem.Repositories.Implementations.AI;

public class AIConversationRepository : BaseRepository<AIConversation>, IAIConversationRepository
{
    public AIConversationRepository(SchoolDbContext db) : base(db) { }

    public async Task<CreateConversationResultDto> CreateAsync(int studentId, string title, string createdBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIConversation_Create";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@StudentId", studentId);
        AddParameter(cmd, "@Title", title);
        AddParameter(cmd, "@CreatedBy", createdBy);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        var id = Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
        return new CreateConversationResultDto { Id = id };
    }

    public async Task<(List<ConversationListItemDto> Items, int TotalRecords)> ListPagedAsync(int studentId, int page, int pageSize, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIConversation_List";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@StudentId", studentId);
        AddParameter(cmd, "@PageNumber", page);
        AddParameter(cmd, "@PageSize", pageSize);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<ConversationListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new ConversationListItemDto
            {
                Id = GetInt32(reader, "Id"),
                Title = GetString(reader, "Title"),
                Status = (SchoolManagementSystem.Models.Enums.ConversationStatus)GetInt32(reader, "Status"),
                IsPinned = GetBoolean(reader, "IsPinned"),
                CreatedAt = GetDateTime(reader, "CreatedAt"),
                MessageCount = GetInt32(reader, "MessageCount"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        var total = items.FirstOrDefault()?.TotalRecords ?? 0;
        return (items, total);
    }

    public async Task<ConversationDetailDto?> GetAsync(int conversationId, int studentId, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIConversation_Get";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@ConversationId", conversationId);
        AddParameter(cmd, "@StudentId", studentId);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            return new ConversationDetailDto
            {
                Id = GetInt32(reader, "Id"),
                StudentId = GetInt32(reader, "StudentId"),
                Title = GetString(reader, "Title"),
                Status = (SchoolManagementSystem.Models.Enums.ConversationStatus)GetInt32(reader, "Status"),
                IsPinned = GetBoolean(reader, "IsPinned"),
                CreatedAt = GetDateTime(reader, "CreatedAt"),
                UpdatedAt = GetNullableDateTime(reader, "UpdatedAt")
            };
        }
        return null;
    }

    public async Task UpdateTitleAsync(int conversationId, int studentId, string title, string updatedBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIConversation_UpdateTitle";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@ConversationId", conversationId);
        AddParameter(cmd, "@StudentId", studentId);
        AddParameter(cmd, "@Title", title);
        AddParameter(cmd, "@UpdatedBy", updatedBy);
        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task DeleteAsync(int conversationId, int studentId, string updatedBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIConversation_Delete";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@ConversationId", conversationId);
        AddParameter(cmd, "@StudentId", studentId);
        AddParameter(cmd, "@UpdatedBy", updatedBy);
        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
