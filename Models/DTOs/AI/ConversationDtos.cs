using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.AI;

public class ConversationListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ConversationStatus Status { get; set; } = ConversationStatus.Active;
    public bool IsPinned { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MessageCount { get; set; }
    public int TotalRecords { get; set; }
}

public class ConversationDetailDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public ConversationStatus Status { get; set; } = ConversationStatus.Active;
    public bool IsPinned { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateConversationDto
{
    public int StudentId { get; set; }
    public string Title { get; set; } = "New Chat";
}

public class CreateConversationResultDto
{
    public int Id { get; set; }
}
