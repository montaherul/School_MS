using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Models.Entities.AI;
using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.Repositories.Interfaces.AI;

public interface IAIMessageRepository : IBaseRepository<AIMessage>
{
    Task<int> InsertAsync(int conversationId, string role, string content, int? promptTokens, int? completionTokens, string? model, int? latencyMs, string createdBy, CancellationToken ct = default);
    Task<List<MessageDto>> ListAsync(int conversationId, int studentId, CancellationToken ct = default);
}
