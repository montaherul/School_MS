using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Services.Interfaces.AI;

public interface IOpenAIClient
{
    Task<AiResponseDto> SendMessageAsync(string systemPrompt, List<MessageDto> conversationHistory, string userMessage, CancellationToken ct = default);
}
