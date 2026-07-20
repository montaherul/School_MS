using SchoolManagementSystem.Models.Common;
using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Services.Interfaces.AI;

public interface IAIAdminService
{
    // Settings
    Task<Result<List<AISettingDto>>> GetSettingsAsync(CancellationToken ct = default);
    Task<Result<int>> UpsertSettingAsync(AISettingUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<Result<bool>> DeleteSettingAsync(int id, CancellationToken ct = default);

    // Providers
    Task<Result<List<AIProviderDto>>> GetProvidersAsync(CancellationToken ct = default);
    Task<Result<int>> UpsertProviderAsync(AIProviderUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<Result<bool>> DeleteProviderAsync(int id, CancellationToken ct = default);

    // Models
    Task<Result<List<AIModelDto>>> GetModelsAsync(CancellationToken ct = default);
    Task<Result<int>> UpsertModelAsync(AIModelUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<Result<bool>> DeleteModelAsync(int id, CancellationToken ct = default);

    // Prompts
    Task<Result<List<AIPromptDto>>> GetPromptsAsync(CancellationToken ct = default);
    Task<Result<AIPromptDto?>> GetActivePromptAsync(string name, string role, CancellationToken ct = default);
    Task<Result<int>> UpsertPromptAsync(AIPromptUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<Result<bool>> DeletePromptAsync(int id, CancellationToken ct = default);

    // Feature Flags
    Task<Result<List<AIFeatureFlagDto>>> GetFeatureFlagsAsync(CancellationToken ct = default);
    Task<Result<int>> UpsertFeatureFlagAsync(AIFeatureFlagUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<Result<bool>> DeleteFeatureFlagAsync(int id, CancellationToken ct = default);

    // Quotas
    Task<Result<List<AIQuotaDto>>> GetQuotasAsync(CancellationToken ct = default);
    Task<Result<int>> UpsertQuotaAsync(AIQuotaUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<Result<bool>> DeleteQuotaAsync(int id, CancellationToken ct = default);

    // Security
    Task<Result<List<AISecurityPolicyDto>>> GetSecurityPoliciesAsync(CancellationToken ct = default);
    Task<Result<int>> UpsertSecurityPolicyAsync(AISecurityPolicyUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<Result<bool>> DeleteSecurityPolicyAsync(int id, CancellationToken ct = default);

    // Audit
    Task<Result<int>> InsertAuditLogAsync(AIAuditLogDto dto, string createdBy, CancellationToken ct = default);
    Task<Result<(List<AIAuditLogDto> Items, int TotalRecords)>> GetAuditLogsPagedAsync(int page, int pageSize, string? entityType = null, CancellationToken ct = default);

    // Dashboard
    Task<Result<AIDashboardStatsDto>> GetDashboardStatsAsync(CancellationToken ct = default);
    Task<Result<(List<AIRequestChartPoint> RequestsPerHour, List<AICostChartPoint> DailyCost, List<TopSubjectDto> TopSubjects)>> GetDashboardChartsAsync(CancellationToken ct = default);

    // Conversations
    Task<Result<(List<AIConversationAdminDto> Items, int TotalRecords)>> GetConversationsAdminAsync(int page, int pageSize, string? search, int? statusFilter, CancellationToken ct = default);

    // Health
    Task<Result<List<AIHealthCheckDto>>> GetLatestHealthChecksAsync(CancellationToken ct = default);

    // Knowledge Base
    Task<Result<List<AIKnowledgeBaseDto>>> GetKnowledgeBasesAsync(CancellationToken ct = default);
    Task<Result<bool>> DeleteKnowledgeBaseAsync(int id, CancellationToken ct = default);
}
