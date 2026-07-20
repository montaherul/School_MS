using SchoolManagementSystem.Models.Common;
using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Services.Interfaces.AI;

public interface IOpenAIService
{
    Task<Result<AiResponseDto>> SendMessageAsync(string systemPrompt, List<MessageDto> conversationHistory, string userMessage, CancellationToken ct = default);
}
