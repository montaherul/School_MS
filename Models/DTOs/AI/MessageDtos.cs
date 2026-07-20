namespace SchoolManagementSystem.Models.DTOs.AI;

public class MessageDto
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class SendMessageDto
{
    public int ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class AiResponseDto
{
    public string Content { get; set; } = string.Empty;
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public string Model { get; set; } = string.Empty;
    public int LatencyMs { get; set; }
    public decimal EstimatedCost { get; set; }
}
