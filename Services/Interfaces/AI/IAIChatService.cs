using SchoolManagementSystem.Models.Common;
using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Services.Interfaces.AI;

public interface IAIChatService
{
    Task<Result<ConversationDetailDto>> CreateConversationAsync(int studentId, string createdBy, CancellationToken ct = default);
    Task<Result<(List<ConversationListItemDto> Items, int TotalPages)>> GetConversationsAsync(int studentId, int page, int pageSize, CancellationToken ct = default);
    Task<Result<ConversationDetailDto>> GetConversationAsync(int conversationId, int studentId, CancellationToken ct = default);
    Task<Result<List<MessageDto>>> GetMessagesAsync(int conversationId, int studentId, CancellationToken ct = default);
    Task<Result<AiResponseDto>> SendMessageAsync(int conversationId, int studentId, string message, string createdBy, CancellationToken ct = default);
    Task<Result<bool>> DeleteConversationAsync(int conversationId, int studentId, string updatedBy, CancellationToken ct = default);
    Task<Result<List<UsageDailySummaryDto>>> GetUsageSummaryAsync(int? studentId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct = default);
}
