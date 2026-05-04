using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Communication;

public class Notice : BaseEntity
{
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(3000)]
    public string Body { get; set; } = string.Empty;

    [MaxLength(80)]
    public string AudienceRole { get; set; } = "All";

    public DateTime PublishAt { get; set; } = DateTime.UtcNow;
}

public class MessageThread : BaseEntity
{
    [MaxLength(160)]
    public string Subject { get; set; } = string.Empty;
}

public class MessageItem : BaseEntity
{
    public int MessageThreadId { get; set; }
    public int SenderUserId { get; set; }
    public int ReceiverUserId { get; set; }

    [MaxLength(2000)]
    public string Body { get; set; } = string.Empty;
}

public class Circular : BaseEntity
{
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(260)]
    public string FilePath { get; set; } = string.Empty;
}
