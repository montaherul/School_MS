using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.AI;

public class AIConversation : BaseEntity
{
    public int StudentId { get; set; }
    public string Title { get; set; } = "New Chat";
    public ConversationStatus Status { get; set; } = ConversationStatus.Active;
    public bool IsPinned { get; set; }

    public Student.Student Student { get; set; } = null!;
    public ICollection<AIMessage> Messages { get; set; } = new List<AIMessage>();
}

public class AIMessage : BaseEntity
{
    public int ConversationId { get; set; }
    public string Role { get; set; } = "user";
    public string Content { get; set; } = string.Empty;
    public int? PromptTokens { get; set; }
    public int? CompletionTokens { get; set; }
    public string? Model { get; set; }
    public int? LatencyMs { get; set; }

    public AIConversation Conversation { get; set; } = null!;
}

public class AIUsage : BaseEntity
{
    public int StudentId { get; set; }
    public int? ConversationId { get; set; }
    public int? MessageId { get; set; }
    public string Model { get; set; } = "gpt-4o-mini";
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
    public decimal EstimatedCost { get; set; }
    public int? LatencyMs { get; set; }
    public DateTime UsageDate { get; set; } = DateTime.UtcNow.Date;

    public Student.Student Student { get; set; } = null!;
    public AIConversation? Conversation { get; set; }
    public AIMessage? Message { get; set; }
}
