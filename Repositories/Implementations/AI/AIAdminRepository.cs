using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Repositories.Interfaces.AI;

namespace SchoolManagementSystem.Repositories.Implementations.AI;

public class AIAdminRepository : IAIAdminRepository
{
    private readonly SchoolDbContext _db;

    public AIAdminRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<List<AISettingDto>> GetSettingsAsync(CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AISettings_GetAll";
        cmd.CommandType = CommandType.StoredProcedure;

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AISettingDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AISettingDto
            {
                Id = GetInt32(reader, "Id"),
                Key = GetString(reader, "Key"),
                Value = GetString(reader, "Value"),
                Description = GetString(reader, "Description"),
                Category = GetString(reader, "Category"),
                DisplayOrder = GetInt32(reader, "DisplayOrder")
            });
        }
        return items;
    }

    public async Task<int> UpsertSettingAsync(AISettingUpsertDto dto, string createdBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AISetting_Upsert";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", dto.Id > 0 ? dto.Id : 0);
        AddParameter(cmd, "@Key", dto.Key);
        AddParameter(cmd, "@Value", dto.Value);
        AddParameter(cmd, "@Description", dto.Description);
        AddParameter(cmd, "@Category", dto.Category);
        AddParameter(cmd, "@DisplayOrder", dto.DisplayOrder);
        AddParameter(cmd, "@CreatedBy", createdBy);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<bool> DeleteSettingAsync(int id, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AISetting_Delete";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", id);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        var rows = await cmd.ExecuteNonQueryAsync(ct);
        return rows > 0;
    }

    public async Task<List<AIProviderDto>> GetProvidersAsync(CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIProviders_GetAll";
        cmd.CommandType = CommandType.StoredProcedure;

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AIProviderDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AIProviderDto
            {
                Id = GetInt32(reader, "Id"),
                Name = GetString(reader, "Name"),
                ProviderType = GetString(reader, "ProviderType"),
                BaseUrl = GetString(reader, "BaseUrl"),
                ApiKey = GetString(reader, "ApiKey"),
                IsEnabled = GetBoolean(reader, "IsEnabled"),
                Priority = GetInt32(reader, "Priority"),
                RetryCount = GetInt32(reader, "RetryCount"),
                TimeoutSeconds = GetInt32(reader, "TimeoutSeconds")
            });
        }
        return items;
    }

    public async Task<int> UpsertProviderAsync(AIProviderUpsertDto dto, string createdBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIProvider_Upsert";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", dto.Id > 0 ? dto.Id : 0);
        AddParameter(cmd, "@Name", dto.Name);
        AddParameter(cmd, "@ProviderType", dto.ProviderType);
        AddParameter(cmd, "@BaseUrl", dto.BaseUrl);
        AddParameter(cmd, "@ApiKeyEncrypted", dto.ApiKey);
        AddParameter(cmd, "@IsEnabled", dto.IsEnabled);
        AddParameter(cmd, "@Priority", dto.Priority);
        AddParameter(cmd, "@RetryCount", dto.RetryCount);
        AddParameter(cmd, "@TimeoutSeconds", dto.TimeoutSeconds);
        AddParameter(cmd, "@CreatedBy", createdBy);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<bool> DeleteProviderAsync(int id, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIProvider_Delete";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", id);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        var rows = await cmd.ExecuteNonQueryAsync(ct);
        return rows > 0;
    }

    public async Task<List<AIModelDto>> GetModelsAsync(CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIModels_GetAll";
        cmd.CommandType = CommandType.StoredProcedure;

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AIModelDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AIModelDto
            {
                Id = GetInt32(reader, "Id"),
                Name = GetString(reader, "Name"),
                ProviderId = GetInt32(reader, "ProviderId"),
                ProviderName = GetString(reader, "ProviderName"),
                Role = GetString(reader, "Role"),
                IsDefault = GetBoolean(reader, "IsDefault"),
                MaxTokens = GetInt32(reader, "MaxTokens"),
                Temperature = Convert.ToDouble(GetDecimal(reader, "Temperature")),
                IsEnabled = GetBoolean(reader, "IsEnabled")
            });
        }
        return items;
    }

    public async Task<bool> DeleteModelAsync(int id, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIModel_Delete";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", id);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        var rows = await cmd.ExecuteNonQueryAsync(ct);
        return rows > 0;
    }

    public async Task<int> UpsertModelAsync(AIModelUpsertDto dto, string createdBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIModel_Upsert";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", dto.Id > 0 ? dto.Id : 0);
        AddParameter(cmd, "@Name", dto.Name);
        AddParameter(cmd, "@ProviderId", dto.ProviderId);
        AddParameter(cmd, "@Role", dto.Role);
        AddParameter(cmd, "@IsDefault", dto.IsDefault);
        AddParameter(cmd, "@MaxTokens", dto.MaxTokens);
        AddParameter(cmd, "@Temperature", dto.Temperature);
        AddParameter(cmd, "@IsEnabled", dto.IsEnabled);
        AddParameter(cmd, "@CreatedBy", createdBy);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<List<AIPromptDto>> GetPromptsAsync(CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIPrompts_GetAll";
        cmd.CommandType = CommandType.StoredProcedure;

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AIPromptDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AIPromptDto
            {
                Id = GetInt32(reader, "Id"),
                Name = GetString(reader, "Name"),
                Role = GetString(reader, "Role"),
                Prompt = GetString(reader, "Prompt"),
                Version = GetInt32(reader, "Version"),
                IsActive = GetBoolean(reader, "IsActive"),
                Category = GetString(reader, "Category")
            });
        }
        return items;
    }

    public async Task<AIPromptDto?> GetActivePromptAsync(string name, string role, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIPrompt_GetActive";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Name", name);
        AddParameter(cmd, "@Role", role);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            return new AIPromptDto
            {
                Id = GetInt32(reader, "Id"),
                Name = GetString(reader, "Name"),
                Role = GetString(reader, "Role"),
                Prompt = GetString(reader, "Prompt"),
                Version = GetInt32(reader, "Version"),
                IsActive = GetBoolean(reader, "IsActive"),
                Category = GetString(reader, "Category")
            };
        }
        return null;
    }

    public async Task<bool> DeletePromptAsync(int id, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIPrompt_Delete";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", id);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        var rows = await cmd.ExecuteNonQueryAsync(ct);
        return rows > 0;
    }

    public async Task<int> UpsertPromptAsync(AIPromptUpsertDto dto, string createdBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIPrompt_Upsert";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", dto.Id > 0 ? dto.Id : 0);
        AddParameter(cmd, "@Name", dto.Name);
        AddParameter(cmd, "@Role", dto.Role);
        AddParameter(cmd, "@Prompt", dto.Prompt);
        AddParameter(cmd, "@IsActive", dto.IsActive);
        AddParameter(cmd, "@Category", dto.Category);
        AddParameter(cmd, "@CreatedBy", createdBy);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<List<AIFeatureFlagDto>> GetFeatureFlagsAsync(CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIFeatureFlags_GetAll";
        cmd.CommandType = CommandType.StoredProcedure;

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AIFeatureFlagDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AIFeatureFlagDto
            {
                Id = GetInt32(reader, "Id"),
                Key = GetString(reader, "Key"),
                DisplayName = GetString(reader, "DisplayName"),
                IsEnabled = GetBoolean(reader, "IsEnabled"),
                Category = GetString(reader, "Category"),
                Description = GetString(reader, "Description")
            });
        }
        return items;
    }

    public async Task<bool> DeleteFeatureFlagAsync(int id, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIFeatureFlag_Delete";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", id);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        var rows = await cmd.ExecuteNonQueryAsync(ct);
        return rows > 0;
    }

    public async Task<int> UpsertFeatureFlagAsync(AIFeatureFlagUpsertDto dto, string createdBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIFeatureFlag_Upsert";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", dto.Id > 0 ? dto.Id : 0);
        AddParameter(cmd, "@Key", dto.Key);
        AddParameter(cmd, "@DisplayName", dto.DisplayName);
        AddParameter(cmd, "@IsEnabled", dto.IsEnabled);
        AddParameter(cmd, "@Category", dto.Category);
        AddParameter(cmd, "@Description", dto.Description);
        AddParameter(cmd, "@CreatedBy", createdBy);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<List<AIQuotaDto>> GetQuotasAsync(CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIQuotas_GetAll";
        cmd.CommandType = CommandType.StoredProcedure;

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AIQuotaDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AIQuotaDto
            {
                Id = GetInt32(reader, "Id"),
                Role = GetString(reader, "Role"),
                DailyLimit = GetInt32(reader, "DailyLimit"),
                MinuteLimit = GetInt32(reader, "MinuteLimit"),
                MaxTokensPerRequest = GetInt32(reader, "MaxTokensPerRequest"),
                IsUnlimited = GetBoolean(reader, "IsUnlimited")
            });
        }
        return items;
    }

    public async Task<bool> DeleteQuotaAsync(int id, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIQuota_Delete";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", id);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        var rows = await cmd.ExecuteNonQueryAsync(ct);
        return rows > 0;
    }

    public async Task<int> UpsertQuotaAsync(AIQuotaUpsertDto dto, string createdBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIQuota_Upsert";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", dto.Id > 0 ? dto.Id : 0);
        AddParameter(cmd, "@Role", dto.Role);
        AddParameter(cmd, "@DailyLimit", dto.DailyLimit);
        AddParameter(cmd, "@MinuteLimit", dto.MinuteLimit);
        AddParameter(cmd, "@MaxTokensPerRequest", dto.MaxTokensPerRequest);
        AddParameter(cmd, "@IsUnlimited", dto.IsUnlimited);
        AddParameter(cmd, "@CreatedBy", createdBy);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<List<AISecurityPolicyDto>> GetSecurityPoliciesAsync(CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AISecurityPolicies_GetAll";
        cmd.CommandType = CommandType.StoredProcedure;

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AISecurityPolicyDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AISecurityPolicyDto
            {
                Id = GetInt32(reader, "Id"),
                Key = GetString(reader, "Key"),
                Value = GetString(reader, "Value"),
                Description = GetString(reader, "Description"),
                Category = GetString(reader, "Category")
            });
        }
        return items;
    }

    public async Task<bool> DeleteSecurityPolicyAsync(int id, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AISecurityPolicy_Delete";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", id);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        var rows = await cmd.ExecuteNonQueryAsync(ct);
        return rows > 0;
    }

    public async Task<int> UpsertSecurityPolicyAsync(AISecurityPolicyUpsertDto dto, string createdBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AISecurityPolicy_Upsert";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", dto.Id > 0 ? dto.Id : 0);
        AddParameter(cmd, "@Key", dto.Key);
        AddParameter(cmd, "@Value", dto.Value);
        AddParameter(cmd, "@Description", dto.Description);
        AddParameter(cmd, "@Category", dto.Category);
        AddParameter(cmd, "@CreatedBy", createdBy);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<int> InsertAuditLogAsync(AIAuditLogDto dto, string createdBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIAuditLog_Insert";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Action", dto.Action);
        AddParameter(cmd, "@EntityType", dto.EntityType);
        AddParameter(cmd, "@EntityId", dto.EntityId > 0 ? dto.EntityId : (int?)null);
        AddParameter(cmd, "@OldValue", dto.OldValue);
        AddParameter(cmd, "@NewValue", dto.NewValue);
        AddParameter(cmd, "@IpAddress", dto.IpAddress);
        AddParameter(cmd, "@UserAgent", dto.UserAgent);
        AddParameter(cmd, "@CreatedBy", createdBy);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<(List<AIAuditLogDto> Items, int TotalRecords)> GetAuditLogsPagedAsync(int page, int pageSize, string? entityType, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIAuditLog_GetPaged";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@PageNumber", page);
        AddParameter(cmd, "@PageSize", pageSize);
        AddParameter(cmd, "@EntityType", entityType);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AIAuditLogDto>();
        var total = 0;
        while (await reader.ReadAsync(ct))
        {
            if (total == 0) total = GetInt32(reader, "TotalRecords");
            items.Add(new AIAuditLogDto
            {
                Id = GetInt32(reader, "Id"),
                Action = GetString(reader, "Action"),
                EntityType = GetString(reader, "EntityType"),
                EntityId = GetInt32(reader, "EntityId"),
                OldValue = GetString(reader, "OldValue"),
                NewValue = GetString(reader, "NewValue"),
                IpAddress = GetString(reader, "IpAddress"),
                UserAgent = GetString(reader, "UserAgent"),
                PerformedBy = GetString(reader, "PerformedBy"),
                CreatedAt = GetDateTime(reader, "CreatedAt")
            });
        }
        return (items, total);
    }

    public async Task<AIDashboardStatsDto?> GetDashboardStatsAsync(CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIDashboardStats";
        cmd.CommandType = CommandType.StoredProcedure;

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            return new AIDashboardStatsDto
            {
                TotalRequests = GetInt32(reader, "TotalRequests"),
                ActiveUsers = GetInt32(reader, "ActiveUsers"),
                StudentsToday = GetInt32(reader, "StudentsToday"),
                TeachersToday = GetInt32(reader, "TeachersToday"),
                TotalTokens = Convert.ToInt64(GetDecimal(reader, "TotalTokens")),
                PromptTokens = Convert.ToInt64(GetDecimal(reader, "PromptTokens")),
                CompletionTokens = Convert.ToInt64(GetDecimal(reader, "CompletionTokens")),
                DailyCost = GetDecimal(reader, "DailyCost"),
                MonthlyCost = GetDecimal(reader, "MonthlyCost"),
                AvgResponseTimeMs = Convert.ToDouble(GetDecimal(reader, "AvgResponseTimeMs")),
                OpenAiStatus = GetString(reader, "OpenAiStatus"),
                ErrorRate = Convert.ToDouble(GetDecimal(reader, "ErrorRate")),
                RateLimitHits = GetInt32(reader, "RateLimitHits"),
                BlockedInjections = GetInt32(reader, "BlockedInjections")
            };
        }
        return null;
    }

    public async Task<(List<AIRequestChartPoint> RequestsPerHour, List<AICostChartPoint> DailyCost, List<TopSubjectDto> TopSubjects)> GetDashboardChartsAsync(CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIDashboardCharts";
        cmd.CommandType = CommandType.StoredProcedure;

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);

        var requestsPerHour = new List<AIRequestChartPoint>();
        while (await reader.ReadAsync(ct))
        {
            requestsPerHour.Add(new AIRequestChartPoint
            {
                Hour = GetString(reader, "Hour"),
                Count = GetInt32(reader, "Count")
            });
        }

        await reader.NextResultAsync(ct);
        var dailyCost = new List<AICostChartPoint>();
        while (await reader.ReadAsync(ct))
        {
            dailyCost.Add(new AICostChartPoint
            {
                Date = GetString(reader, "Date"),
                Cost = GetDecimal(reader, "Cost")
            });
        }

        await reader.NextResultAsync(ct);
        var topSubjects = new List<TopSubjectDto>();
        while (await reader.ReadAsync(ct))
        {
            topSubjects.Add(new TopSubjectDto
            {
                SubjectName = GetString(reader, "SubjectName"),
                Count = GetInt32(reader, "Count")
            });
        }

        return (requestsPerHour, dailyCost, topSubjects);
    }

    public async Task<(List<AIConversationAdminDto> Items, int TotalRecords)> GetConversationsAdminAsync(int page, int pageSize, string? search, int? statusFilter, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIConversations_AdminList";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@PageNumber", page);
        AddParameter(cmd, "@PageSize", pageSize);
        AddParameter(cmd, "@Search", search);
        AddParameter(cmd, "@StatusFilter", statusFilter);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AIConversationAdminDto>();
        var total = 0;
        while (await reader.ReadAsync(ct))
        {
            if (total == 0) total = GetInt32(reader, "TotalRecords");
            items.Add(new AIConversationAdminDto
            {
                Id = GetInt32(reader, "Id"),
                StudentId = GetInt32(reader, "StudentId"),
                StudentName = GetString(reader, "StudentName"),
                Title = GetString(reader, "Title"),
                Status = GetString(reader, "Status"),
                IsPinned = GetBoolean(reader, "IsPinned"),
                MessageCount = GetInt32(reader, "MessageCount"),
                CreatedAt = GetDateTime(reader, "CreatedAt"),
                UpdatedAt = GetDateTime(reader, "UpdatedAt")
            });
        }
        return (items, total);
    }

    public async Task<List<AIHealthCheckDto>> GetLatestHealthChecksAsync(CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIHealthChecks_GetLatest";
        cmd.CommandType = CommandType.StoredProcedure;

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AIHealthCheckDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AIHealthCheckDto
            {
                Id = GetInt32(reader, "Id"),
                Component = GetString(reader, "Component"),
                Status = GetString(reader, "Status"),
                LastChecked = GetDateTime(reader, "LastChecked"),
                ResponseTimeMs = GetInt32(reader, "ResponseTimeMs"),
                ErrorMessage = GetString(reader, "ErrorMessage")
            });
        }
        return items;
    }

    public async Task<List<AIKnowledgeBaseDto>> GetKnowledgeBasesAsync(CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIKnowledgeBases_GetAll";
        cmd.CommandType = CommandType.StoredProcedure;

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AIKnowledgeBaseDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AIKnowledgeBaseDto
            {
                Id = GetInt32(reader, "Id"),
                Name = GetString(reader, "Name"),
                FilePath = GetString(reader, "FilePath"),
                ContentType = GetString(reader, "ContentType"),
                Size = Convert.ToInt64(GetDecimal(reader, "Size")),
                Version = GetInt32(reader, "Version"),
                IsActive = GetBoolean(reader, "IsActive"),
                Description = GetString(reader, "Description"),
                CreatedAt = GetDateTime(reader, "CreatedAt")
            });
        }
        return items;
    }

    public async Task<int> UpsertKnowledgeBaseAsync(AIKnowledgeBaseUpsertDto dto, string createdBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIKnowledgeBase_Upsert";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", 0);
        AddParameter(cmd, "@Name", dto.Name);
        AddParameter(cmd, "@FilePath", dto.FilePath);
        AddParameter(cmd, "@ContentType", dto.ContentType);
        AddParameter(cmd, "@Description", dto.Description);
        AddParameter(cmd, "@CreatedBy", createdBy);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<bool> DeleteKnowledgeBaseAsync(int id, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIKnowledgeBase_Delete";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Id", id);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        var rows = await cmd.ExecuteNonQueryAsync(ct);
        return rows > 0;
    }

    private static async Task<IAsyncDisposable> OpenConnectionAsync(DbConnection connection, CancellationToken ct)
    {
        var wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed) await connection.OpenAsync(ct);
        return new ConnectionLease(connection, wasClosed);
    }

    private static void AddParameter(DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }

    private static string GetString(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? string.Empty : Convert.ToString(reader[name]) ?? string.Empty;
    private static int GetInt32(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? 0 : Convert.ToInt32(reader[name]);
    private static decimal GetDecimal(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? 0m : Convert.ToDecimal(reader[name]);
    private static bool GetBoolean(DbDataReader reader, string name) => !reader.IsDBNull(reader.GetOrdinal(name)) && Convert.ToBoolean(reader[name]);
    private static DateTime GetDateTime(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? DateTime.MinValue : Convert.ToDateTime(reader[name]);

    private sealed class ConnectionLease : IAsyncDisposable
    {
        private readonly DbConnection _connection;
        private readonly bool _closeOnDispose;
        public ConnectionLease(DbConnection connection, bool closeOnDispose) { _connection = connection; _closeOnDispose = closeOnDispose; }
        public async ValueTask DisposeAsync() { if (_closeOnDispose) await _connection.CloseAsync(); }
    }
}
