using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Models.ViewModels.AI;

public class AIChatViewModel
{
    public int? ActiveConversationId { get; set; }
    public string ActiveConversationTitle { get; set; } = string.Empty;
    public List<ConversationListItemDto> Conversations { get; set; } = new();
    public List<MessageDto> Messages { get; set; } = new();
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}

public class MessageViewModel
{
    public int Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsUser => Role == "user";
}
