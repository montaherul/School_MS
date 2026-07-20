using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Repositories.Interfaces.AI;

public interface IAIAdminRepository
{
    Task<List<AISettingDto>> GetSettingsAsync(CancellationToken ct = default);
    Task<int> UpsertSettingAsync(AISettingUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<bool> DeleteSettingAsync(int id, CancellationToken ct = default);

    Task<List<AIProviderDto>> GetProvidersAsync(CancellationToken ct = default);
    Task<int> UpsertProviderAsync(AIProviderUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<bool> DeleteProviderAsync(int id, CancellationToken ct = default);

    Task<List<AIModelDto>> GetModelsAsync(CancellationToken ct = default);
    Task<int> UpsertModelAsync(AIModelUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<bool> DeleteModelAsync(int id, CancellationToken ct = default);

    Task<List<AIPromptDto>> GetPromptsAsync(CancellationToken ct = default);
    Task<AIPromptDto?> GetActivePromptAsync(string name, string role, CancellationToken ct = default);
    Task<int> UpsertPromptAsync(AIPromptUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<bool> DeletePromptAsync(int id, CancellationToken ct = default);

    Task<List<AIFeatureFlagDto>> GetFeatureFlagsAsync(CancellationToken ct = default);
    Task<int> UpsertFeatureFlagAsync(AIFeatureFlagUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<bool> DeleteFeatureFlagAsync(int id, CancellationToken ct = default);

    Task<List<AIQuotaDto>> GetQuotasAsync(CancellationToken ct = default);
    Task<int> UpsertQuotaAsync(AIQuotaUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<bool> DeleteQuotaAsync(int id, CancellationToken ct = default);

    Task<List<AISecurityPolicyDto>> GetSecurityPoliciesAsync(CancellationToken ct = default);
    Task<int> UpsertSecurityPolicyAsync(AISecurityPolicyUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<bool> DeleteSecurityPolicyAsync(int id, CancellationToken ct = default);

    Task<int> InsertAuditLogAsync(AIAuditLogDto dto, string createdBy, CancellationToken ct = default);
    Task<(List<AIAuditLogDto> Items, int TotalRecords)> GetAuditLogsPagedAsync(int page, int pageSize, string? entityType = null, CancellationToken ct = default);

    Task<AIDashboardStatsDto?> GetDashboardStatsAsync(CancellationToken ct = default);
    Task<(List<AIRequestChartPoint> RequestsPerHour, List<AICostChartPoint> DailyCost, List<TopSubjectDto> TopSubjects)> GetDashboardChartsAsync(CancellationToken ct = default);

    Task<(List<AIConversationAdminDto> Items, int TotalRecords)> GetConversationsAdminAsync(int page, int pageSize, string? search, int? statusFilter, CancellationToken ct = default);

    Task<List<AIHealthCheckDto>> GetLatestHealthChecksAsync(CancellationToken ct = default);

    Task<List<AIKnowledgeBaseDto>> GetKnowledgeBasesAsync(CancellationToken ct = default);
    Task<int> UpsertKnowledgeBaseAsync(AIKnowledgeBaseUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<bool> DeleteKnowledgeBaseAsync(int id, CancellationToken ct = default);
}
