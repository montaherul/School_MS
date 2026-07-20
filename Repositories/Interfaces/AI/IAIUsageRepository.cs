using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Models.Entities.AI;
using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.Repositories.Interfaces.AI;

public interface IAIUsageRepository : IBaseRepository<AIUsage>
{
    Task<int> InsertAsync(int studentId, int? conversationId, int? messageId, string model, int promptTokens, int completionTokens, int totalTokens, decimal estimatedCost, int? latencyMs, string createdBy, CancellationToken ct = default);
    Task<List<UsageDailySummaryDto>> GetDailySummaryAsync(int? studentId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct = default);
}
