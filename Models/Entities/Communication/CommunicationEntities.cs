using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

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

    [MaxLength(260)]
    public string? AttachmentPath { get; set; }

    public bool IsPublished { get; set; } = true;

    // Category for notice (Academic, Exam, Holiday, Emergency, Event, Admission, General)
    [NotMapped]
    [MaxLength(60)]
    public string Category { get; set; } = "General";

    // Mark important notices which should be highlighted and shown first for students
    [NotMapped]
    public bool IsImportant { get; set; } = false;
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
