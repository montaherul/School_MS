using System.Text.Json;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.Common;
using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Repositories.Interfaces.AI;
using SchoolManagementSystem.Services.Interfaces.AI;

namespace SchoolManagementSystem.Services.Implementations.AI;

public class AIAdminService : IAIAdminService
{
    private readonly IAIAdminRepository _adminRepo;
    private readonly ILogger<AIAdminService> _logger;

    public AIAdminService(IAIAdminRepository adminRepo, ILogger<AIAdminService> logger)
    {
        _adminRepo = adminRepo;
        _logger = logger;
    }

    // ── Settings ──────────────────────────────────────────────────────

    public async Task<Result<List<AISettingDto>>> GetSettingsAsync(CancellationToken ct)
    {
        try
        {
            var items = await _adminRepo.GetSettingsAsync(ct);
            return Result<List<AISettingDto>>.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI settings");
            return Result<List<AISettingDto>>.Fail("Failed to load settings.", "LOAD_FAILED");
        }
    }

    public async Task<Result<int>> UpsertSettingAsync(AISettingUpsertDto dto, string createdBy, CancellationToken ct)
    {
        try
        {
            var id = await _adminRepo.UpsertSettingAsync(dto, createdBy, ct);
            _ = InsertAuditLogAsync(new AIAuditLogDto
            {
                Action = "Upsert",
                EntityType = "AISetting",
                EntityId = id,
                NewValue = JsonSerializer.Serialize(dto),
                PerformedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            }, createdBy, ct);
            return Result<int>.Success(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert AI setting {Key}", dto.Key);
            return Result<int>.Fail("Failed to save setting.", "UPSERT_FAILED");
        }
    }

    public async Task<Result<bool>> DeleteSettingAsync(int id, CancellationToken ct)
    {
        try
        {
            var result = await _adminRepo.DeleteSettingAsync(id, ct);
            if (result)
            {
                _ = InsertAuditLogAsync(new AIAuditLogDto
                {
                    Action = "Delete",
                    EntityType = "AISetting",
                    EntityId = id,
                    PerformedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }, "System", ct);
            }
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete AI setting {Id}", id);
            return Result<bool>.Fail("Failed to delete setting.", "DELETE_FAILED");
        }
    }

    // ── Providers ─────────────────────────────────────────────────────

    public async Task<Result<List<AIProviderDto>>> GetProvidersAsync(CancellationToken ct)
    {
        try
        {
            var items = await _adminRepo.GetProvidersAsync(ct);
            return Result<List<AIProviderDto>>.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI providers");
            return Result<List<AIProviderDto>>.Fail("Failed to load providers.", "LOAD_FAILED");
        }
    }

    public async Task<Result<int>> UpsertProviderAsync(AIProviderUpsertDto dto, string createdBy, CancellationToken ct)
    {
        try
        {
            var id = await _adminRepo.UpsertProviderAsync(dto, createdBy, ct);
            _ = InsertAuditLogAsync(new AIAuditLogDto
            {
                Action = "Upsert",
                EntityType = "AIProvider",
                EntityId = id,
                NewValue = JsonSerializer.Serialize(dto),
                PerformedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            }, createdBy, ct);
            return Result<int>.Success(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert AI provider {Name}", dto.Name);
            return Result<int>.Fail("Failed to save provider.", "UPSERT_FAILED");
        }
    }

    public async Task<Result<bool>> DeleteProviderAsync(int id, CancellationToken ct)
    {
        try
        {
            var result = await _adminRepo.DeleteProviderAsync(id, ct);
            if (result)
            {
                _ = InsertAuditLogAsync(new AIAuditLogDto
                {
                    Action = "Delete",
                    EntityType = "AIProvider",
                    EntityId = id,
                    PerformedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }, "System", ct);
            }
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete AI provider {Id}", id);
            return Result<bool>.Fail("Failed to delete provider.", "DELETE_FAILED");
        }
    }

    // ── Models ────────────────────────────────────────────────────────

    public async Task<Result<List<AIModelDto>>> GetModelsAsync(CancellationToken ct)
    {
        try
        {
            var items = await _adminRepo.GetModelsAsync(ct);
            return Result<List<AIModelDto>>.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI models");
            return Result<List<AIModelDto>>.Fail("Failed to load models.", "LOAD_FAILED");
        }
    }

    public async Task<Result<int>> UpsertModelAsync(AIModelUpsertDto dto, string createdBy, CancellationToken ct)
    {
        try
        {
            var id = await _adminRepo.UpsertModelAsync(dto, createdBy, ct);
            _ = InsertAuditLogAsync(new AIAuditLogDto
            {
                Action = "Upsert",
                EntityType = "AIModel",
                EntityId = id,
                NewValue = JsonSerializer.Serialize(dto),
                PerformedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            }, createdBy, ct);
            return Result<int>.Success(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert AI model {Name}", dto.Name);
            return Result<int>.Fail("Failed to save model.", "UPSERT_FAILED");
        }
    }

    public async Task<Result<bool>> DeleteModelAsync(int id, CancellationToken ct)
    {
        try
        {
            var result = await _adminRepo.DeleteModelAsync(id, ct);
            if (result)
            {
                _ = InsertAuditLogAsync(new AIAuditLogDto
                {
                    Action = "Delete",
                    EntityType = "AIModel",
                    EntityId = id,
                    PerformedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }, "System", ct);
            }
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete AI model {Id}", id);
            return Result<bool>.Fail("Failed to delete model.", "DELETE_FAILED");
        }
    }

    // ── Prompts ──────────────────────────────────────────────────────

    public async Task<Result<List<AIPromptDto>>> GetPromptsAsync(CancellationToken ct)
    {
        try
        {
            var items = await _adminRepo.GetPromptsAsync(ct);
            return Result<List<AIPromptDto>>.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI prompts");
            return Result<List<AIPromptDto>>.Fail("Failed to load prompts.", "LOAD_FAILED");
        }
    }

    public async Task<Result<AIPromptDto?>> GetActivePromptAsync(string name, string role, CancellationToken ct)
    {
        try
        {
            var prompt = await _adminRepo.GetActivePromptAsync(name, role, ct);
            return Result<AIPromptDto?>.Success(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get active AI prompt {Name}/{Role}", name, role);
            return Result<AIPromptDto?>.Fail("Failed to load prompt.", "LOAD_FAILED");
        }
    }

    public async Task<Result<int>> UpsertPromptAsync(AIPromptUpsertDto dto, string createdBy, CancellationToken ct)
    {
        try
        {
            var id = await _adminRepo.UpsertPromptAsync(dto, createdBy, ct);
            _ = InsertAuditLogAsync(new AIAuditLogDto
            {
                Action = "Upsert",
                EntityType = "AIPrompt",
                EntityId = id,
                NewValue = JsonSerializer.Serialize(dto),
                PerformedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            }, createdBy, ct);
            return Result<int>.Success(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert AI prompt {Name}/{Role}", dto.Name, dto.Role);
            return Result<int>.Fail("Failed to save prompt.", "UPSERT_FAILED");
        }
    }

    public async Task<Result<bool>> DeletePromptAsync(int id, CancellationToken ct)
    {
        try
        {
            var result = await _adminRepo.DeletePromptAsync(id, ct);
            if (result)
            {
                _ = InsertAuditLogAsync(new AIAuditLogDto
                {
                    Action = "Delete",
                    EntityType = "AIPrompt",
                    EntityId = id,
                    PerformedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }, "System", ct);
            }
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete AI prompt {Id}", id);
            return Result<bool>.Fail("Failed to delete prompt.", "DELETE_FAILED");
        }
    }

    // ── Feature Flags ─────────────────────────────────────────────────

    public async Task<Result<List<AIFeatureFlagDto>>> GetFeatureFlagsAsync(CancellationToken ct)
    {
        try
        {
            var items = await _adminRepo.GetFeatureFlagsAsync(ct);
            return Result<List<AIFeatureFlagDto>>.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI feature flags");
            return Result<List<AIFeatureFlagDto>>.Fail("Failed to load feature flags.", "LOAD_FAILED");
        }
    }

    public async Task<Result<int>> UpsertFeatureFlagAsync(AIFeatureFlagUpsertDto dto, string createdBy, CancellationToken ct)
    {
        try
        {
            var id = await _adminRepo.UpsertFeatureFlagAsync(dto, createdBy, ct);
            _ = InsertAuditLogAsync(new AIAuditLogDto
            {
                Action = "Upsert",
                EntityType = "AIFeatureFlag",
                EntityId = id,
                NewValue = JsonSerializer.Serialize(dto),
                PerformedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            }, createdBy, ct);
            return Result<int>.Success(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert AI feature flag {Key}", dto.Key);
            return Result<int>.Fail("Failed to save feature flag.", "UPSERT_FAILED");
        }
    }

    public async Task<Result<bool>> DeleteFeatureFlagAsync(int id, CancellationToken ct)
    {
        try
        {
            var result = await _adminRepo.DeleteFeatureFlagAsync(id, ct);
            if (result)
            {
                _ = InsertAuditLogAsync(new AIAuditLogDto
                {
                    Action = "Delete",
                    EntityType = "AIFeatureFlag",
                    EntityId = id,
                    PerformedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }, "System", ct);
            }
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete AI feature flag {Id}", id);
            return Result<bool>.Fail("Failed to delete feature flag.", "DELETE_FAILED");
        }
    }

    // ── Quotas ────────────────────────────────────────────────────────

    public async Task<Result<List<AIQuotaDto>>> GetQuotasAsync(CancellationToken ct)
    {
        try
        {
            var items = await _adminRepo.GetQuotasAsync(ct);
            return Result<List<AIQuotaDto>>.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI quotas");
            return Result<List<AIQuotaDto>>.Fail("Failed to load quotas.", "LOAD_FAILED");
        }
    }

    public async Task<Result<int>> UpsertQuotaAsync(AIQuotaUpsertDto dto, string createdBy, CancellationToken ct)
    {
        try
        {
            var id = await _adminRepo.UpsertQuotaAsync(dto, createdBy, ct);
            _ = InsertAuditLogAsync(new AIAuditLogDto
            {
                Action = "Upsert",
                EntityType = "AIQuota",
                EntityId = id,
                NewValue = JsonSerializer.Serialize(dto),
                PerformedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            }, createdBy, ct);
            return Result<int>.Success(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert AI quota for role {Role}", dto.Role);
            return Result<int>.Fail("Failed to save quota.", "UPSERT_FAILED");
        }
    }

    public async Task<Result<bool>> DeleteQuotaAsync(int id, CancellationToken ct)
    {
        try
        {
            var result = await _adminRepo.DeleteQuotaAsync(id, ct);
            if (result)
            {
                _ = InsertAuditLogAsync(new AIAuditLogDto
                {
                    Action = "Delete",
                    EntityType = "AIQuota",
                    EntityId = id,
                    PerformedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }, "System", ct);
            }
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete AI quota {Id}", id);
            return Result<bool>.Fail("Failed to delete quota.", "DELETE_FAILED");
        }
    }

    // ── Security Policies ─────────────────────────────────────────────

    public async Task<Result<List<AISecurityPolicyDto>>> GetSecurityPoliciesAsync(CancellationToken ct)
    {
        try
        {
            var items = await _adminRepo.GetSecurityPoliciesAsync(ct);
            return Result<List<AISecurityPolicyDto>>.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI security policies");
            return Result<List<AISecurityPolicyDto>>.Fail("Failed to load security policies.", "LOAD_FAILED");
        }
    }

    public async Task<Result<int>> UpsertSecurityPolicyAsync(AISecurityPolicyUpsertDto dto, string createdBy, CancellationToken ct)
    {
        try
        {
            var id = await _adminRepo.UpsertSecurityPolicyAsync(dto, createdBy, ct);
            _ = InsertAuditLogAsync(new AIAuditLogDto
            {
                Action = "Upsert",
                EntityType = "AISecurityPolicy",
                EntityId = id,
                NewValue = JsonSerializer.Serialize(dto),
                PerformedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            }, createdBy, ct);
            return Result<int>.Success(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert AI security policy {Key}", dto.Key);
            return Result<int>.Fail("Failed to save security policy.", "UPSERT_FAILED");
        }
    }

    // ── Audit ─────────────────────────────────────────────────────────

    public async Task<Result<int>> InsertAuditLogAsync(AIAuditLogDto dto, string createdBy, CancellationToken ct)
    {
        try
        {
            dto.PerformedBy = createdBy;
            dto.CreatedAt = DateTime.UtcNow;
            var id = await _adminRepo.InsertAuditLogAsync(dto, createdBy, ct);
            return Result<int>.Success(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert AI audit log");
            return Result<int>.Fail("Failed to record audit log.", "AUDIT_FAILED");
        }
    }

    public async Task<Result<(List<AIAuditLogDto> Items, int TotalRecords)>> GetAuditLogsPagedAsync(
        int page, int pageSize, string? entityType, CancellationToken ct)
    {
        try
        {
            var result = await _adminRepo.GetAuditLogsPagedAsync(page, pageSize, entityType, ct);
            return Result<(List<AIAuditLogDto>, int)>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI audit logs");
            return Result<(List<AIAuditLogDto> Items, int TotalRecords)>.Fail("Failed to load audit logs.", "LOAD_FAILED");
        }
    }

    // ── Dashboard ─────────────────────────────────────────────────────

    public async Task<Result<AIDashboardStatsDto>> GetDashboardStatsAsync(CancellationToken ct)
    {
        try
        {
            var stats = await _adminRepo.GetDashboardStatsAsync(ct);
            return Result<AIDashboardStatsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI dashboard stats");
            return Result<AIDashboardStatsDto>.Fail("Failed to load dashboard stats.", "LOAD_FAILED");
        }
    }

    public async Task<Result<(List<AIRequestChartPoint> RequestsPerHour, List<AICostChartPoint> DailyCost, List<TopSubjectDto> TopSubjects)>> GetDashboardChartsAsync(CancellationToken ct)
    {
        try
        {
            var result = await _adminRepo.GetDashboardChartsAsync(ct);
            return Result<(List<AIRequestChartPoint>, List<AICostChartPoint>, List<TopSubjectDto>)>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI dashboard charts");
            return Result<(List<AIRequestChartPoint> RequestsPerHour, List<AICostChartPoint> DailyCost, List<TopSubjectDto> TopSubjects)>.Fail("Failed to load dashboard charts.", "LOAD_FAILED");
        }
    }

    // ── Conversations ─────────────────────────────────────────────────

    public async Task<Result<(List<AIConversationAdminDto> Items, int TotalRecords)>> GetConversationsAdminAsync(
        int page, int pageSize, string? search, int? statusFilter, CancellationToken ct)
    {
        try
        {
            var result = await _adminRepo.GetConversationsAdminAsync(page, pageSize, search, statusFilter, ct);
            return Result<(List<AIConversationAdminDto>, int)>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI conversations admin");
            return Result<(List<AIConversationAdminDto> Items, int TotalRecords)>.Fail("Failed to load conversations.", "LOAD_FAILED");
        }
    }

    // ── Health ────────────────────────────────────────────────────────

    public async Task<Result<List<AIHealthCheckDto>>> GetLatestHealthChecksAsync(CancellationToken ct)
    {
        try
        {
            var items = await _adminRepo.GetLatestHealthChecksAsync(ct);
            return Result<List<AIHealthCheckDto>>.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI health checks");
            return Result<List<AIHealthCheckDto>>.Fail("Failed to load health checks.", "LOAD_FAILED");
        }
    }

    // ── Knowledge Base ────────────────────────────────────────────────

    public async Task<Result<List<AIKnowledgeBaseDto>>> GetKnowledgeBasesAsync(CancellationToken ct)
    {
        try
        {
            var items = await _adminRepo.GetKnowledgeBasesAsync(ct);
            return Result<List<AIKnowledgeBaseDto>>.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI knowledge bases");
            return Result<List<AIKnowledgeBaseDto>>.Fail("Failed to load knowledge bases.", "LOAD_FAILED");
        }
    }

    public async Task<Result<bool>> DeleteSecurityPolicyAsync(int id, CancellationToken ct)
    {
        try
        {
            var result = await _adminRepo.DeleteSecurityPolicyAsync(id, ct);
            if (result)
            {
                _ = InsertAuditLogAsync(new AIAuditLogDto
                {
                    Action = "Delete",
                    EntityType = "AISecurityPolicy",
                    EntityId = id,
                    PerformedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }, "System", ct);
            }
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete AI security policy {Id}", id);
            return Result<bool>.Fail("Failed to delete security policy.", "DELETE_FAILED");
        }
    }

    public async Task<Result<bool>> DeleteKnowledgeBaseAsync(int id, CancellationToken ct)
    {
        try
        {
            var result = await _adminRepo.DeleteKnowledgeBaseAsync(id, ct);
            if (result)
            {
                _ = InsertAuditLogAsync(new AIAuditLogDto
                {
                    Action = "Delete",
                    EntityType = "AIKnowledgeBase",
                    EntityId = id,
                    PerformedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }, "System", ct);
            }
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete AI knowledge base {Id}", id);
            return Result<bool>.Fail("Failed to delete knowledge base.", "DELETE_FAILED");
        }
    }
}
