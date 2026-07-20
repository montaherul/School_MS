using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Models.Entities.AI;
using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.Repositories.Interfaces.AI;

public interface IAIConversationRepository : IBaseRepository<AIConversation>
{
    Task<CreateConversationResultDto> CreateAsync(int studentId, string title, string createdBy, CancellationToken ct = default);
    Task<(List<ConversationListItemDto> Items, int TotalRecords)> ListPagedAsync(int studentId, int page, int pageSize, CancellationToken ct = default);
    Task<ConversationDetailDto?> GetAsync(int conversationId, int studentId, CancellationToken ct = default);
    Task UpdateTitleAsync(int conversationId, int studentId, string title, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int conversationId, int studentId, string updatedBy, CancellationToken ct = default);
}
