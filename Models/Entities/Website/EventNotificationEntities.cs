using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public enum EventNotificationStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3
}

public enum EventNotificationRecipientStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2,
    Opened = 3,
    Delivered = 4,
    Bounced = 5,
    Complained = 6
}

public enum EventScope
{
    AllStudents = 0,
    SpecificClass = 1,
    SpecificSection = 2,
    SpecificGroup = 3,
    SpecificStudents = 4,
    SpecificGuardians = 5
}

public enum NotificationChannel
{
    Email = 0,
    SMS = 1,
    WhatsApp = 2,
    InApp = 3
}

public enum ReminderUnit
{
    Minutes = 0,
    Hours = 1,
    Days = 2,
    Weeks = 3
}

public class EventNotification : BaseEntity
{
    public int EventId { get; set; }
    public Event? Event { get; set; }

    public EventScope Scope { get; set; } = EventScope.AllStudents;

    public int? ClassId { get; set; }
    public int? SectionId { get; set; }
    public int? GroupId { get; set; }

    [MaxLength(4000)]
    public string? StudentIds { get; set; }

    [MaxLength(4000)]
    public string? GuardianIds { get; set; }

    public bool NotifyGuardians { get; set; } = true;
    public bool NotifyStudents { get; set; }
    public bool PrimaryGuardianOnly { get; set; } = true;

    public int? EmailTemplateId { get; set; }
    public EmailTemplate? EmailTemplate { get; set; }

    public EventNotificationStatus Status { get; set; } = EventNotificationStatus.Pending;

    public int TotalRecipients { get; set; }
    public int SentCount { get; set; }
    public int FailedCount { get; set; }
    public int BounceCount { get; set; }
    public int ComplaintCount { get; set; }
    public int ClickCount { get; set; }

    public int? TriggeredByUserId { get; set; }
    public DateTime? SentAt { get; set; }

    [MaxLength(128)]
    public string? DuplicateHash { get; set; }

    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public ICollection<EventNotificationRecipient> Recipients { get; set; } = new List<EventNotificationRecipient>();
    public ICollection<EventNotificationLog> Logs { get; set; } = new List<EventNotificationLog>();
    public ICollection<EventNotificationAttachment> Attachments { get; set; } = new List<EventNotificationAttachment>();
}

public class EventNotificationRecipient : BaseEntity
{
    public int EventNotificationId { get; set; }
    public EventNotification? EventNotification { get; set; }

    public int? GuardianId { get; set; }
    public Guardian.Guardian? Guardian { get; set; }

    public int? StudentId { get; set; }
    public Student.Student? Student { get; set; }

    [MaxLength(160)]
    public string RecipientEmail { get; set; } = string.Empty;

    [MaxLength(160)]
    public string RecipientName { get; set; } = string.Empty;

    public EventNotificationRecipientStatus DeliveryStatus { get; set; } = EventNotificationRecipientStatus.Pending;

    [MaxLength(500)]
    public string? FailureReason { get; set; }

    public int RetryCount { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? OpenedAt { get; set; }
    public DateTime? ClickedAt { get; set; }
    public DateTime? BouncedAt { get; set; }
    public DateTime? ComplaintAt { get; set; }

    [MaxLength(200)]
    public string? MessageId { get; set; }
}

public class EventNotificationLog : BaseEntity
{
    public int EventNotificationId { get; set; }
    public EventNotification? EventNotification { get; set; }

    public int? RecipientId { get; set; }
    public EventNotificationRecipient? Recipient { get; set; }

    [MaxLength(50)]
    public string Action { get; set; } = string.Empty; // Queued, Sent, Failed, Retried, Opened, Delivered, Bounced, Complained

    [MaxLength(500)]
    public string? Details { get; set; }

    [MaxLength(500)]
    public string? ErrorMessage { get; set; }

    [MaxLength(160)]
    public string? PerformedBy { get; set; }
}

public class EventNotificationQueue : BaseEntity
{
    public int EventNotificationId { get; set; }
    public EventNotification? EventNotification { get; set; }

    public int? RecipientId { get; set; }
    public EventNotificationRecipient? Recipient { get; set; }

    [MaxLength(160)]
    public string RecipientEmail { get; set; } = string.Empty;

    [MaxLength(160)]
    public string RecipientName { get; set; } = string.Empty;

    [MaxLength(260)]
    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed

    public int RetryCount { get; set; }
    public int MaxRetries { get; set; } = 3;

    public DateTime? ScheduledAt { get; set; }
    public DateTime? ProcessedAt { get; set; }

    [MaxLength(500)]
    public string? LastError { get; set; }

    public DateTime? NextRetryAt { get; set; }

    public NotificationChannel Channel { get; set; } = NotificationChannel.Email;

    [MaxLength(500)]
    public string? AttachmentIds { get; set; }
}

public class GuardainNotificationPreference : BaseEntity
{
    public int GuardianId { get; set; }
    public Guardian.Guardian? Guardian { get; set; }

    public bool OptInEventNotifications { get; set; } = true;

    public bool OptInSMS { get; set; } = true;
    public bool OptInEmail { get; set; } = true;
    public bool OptInWhatsApp { get; set; }
    public bool OptInInApp { get; set; } = true;

    public bool EmailVerified { get; set; }
    [MaxLength(160)]
    public string? VerifiedEmail { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }

    [MaxLength(500)]
    public string? SubscribedEventTypes { get; set; }

    [MaxLength(500)]
    public string? QuietHoursStart { get; set; }

    [MaxLength(500)]
    public string? QuietHoursEnd { get; set; }

    public bool AllowReminders { get; set; } = true;
    public int ReminderLeadMinutes { get; set; } = 1440;
}

public class EventNotificationAttachment : BaseEntity
{
    public int EventNotificationId { get; set; }
    public EventNotification? EventNotification { get; set; }

    [MaxLength(260)]
    public string FileName { get; set; } = string.Empty;

    [MaxLength(260)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    [MaxLength(160)]
    public string? Description { get; set; }

    public bool IsInline { get; set; }
}

public class ScheduledNotification : BaseEntity
{
    public int EventNotificationId { get; set; }
    public EventNotification? EventNotification { get; set; }

    public DateTime ScheduledAt { get; set; }

    public bool IsProcessed { get; set; }
    public DateTime? ProcessedAt { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class ReminderConfig : BaseEntity
{
    public int EventId { get; set; }
    public Event? Event { get; set; }

    public bool IsActive { get; set; } = true;

    public int ReminderValue { get; set; } = 1;
    public ReminderUnit ReminderUnit { get; set; } = ReminderUnit.Days;

    public DateTime? LastSentAt { get; set; }
    public int SentCount { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
