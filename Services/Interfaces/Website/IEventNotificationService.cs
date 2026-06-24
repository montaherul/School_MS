using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SchoolManagementSystem.Models.Entities.Website;

namespace SchoolManagementSystem.Services.Interfaces.Website;

public interface IEventNotificationService
{
    // Core notification lifecycle
    Task<EventNotification> CreateNotificationAsync(int eventId, EventScope scope, int? classId = null,
        int? sectionId = null, int? groupId = null, string? studentIds = null, string? guardianIds = null,
        bool notifyGuardians = true, bool notifyStudents = false, bool primaryGuardianOnly = true,
        int? templateId = null, int? triggeredByUserId = null, CancellationToken ct = default);

    Task QueueNotificationAsync(int notificationId, CancellationToken ct = default);

    Task SendNotificationAsync(int notificationId, CancellationToken ct = default);

    Task ResendNotificationAsync(int notificationId, CancellationToken ct = default);

    // Queries
    Task<EventNotification?> GetNotificationAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<EventNotification>> GetNotificationsByEventAsync(int eventId, CancellationToken ct = default);

    Task<IReadOnlyList<EventNotification>> GetAllNotificationsAsync(CancellationToken ct = default);

    Task<IReadOnlyList<EventNotification>> GetRecentNotificationsAsync(int count, CancellationToken ct = default);

    Task<string> PreviewEmailAsync(int notificationId, CancellationToken ct = default);

    Task<IReadOnlyList<EventNotificationRecipient>> GetRecipientsAsync(int notificationId, CancellationToken ct = default);

    // Dashboard & Analytics
    Task<EventNotificationDashboardDto> GetDashboardAsync(CancellationToken ct = default);

    Task<EventNotificationAnalyticsDto> GetAnalyticsAsync(int notificationId, CancellationToken ct = default);

    // Guardian Preferences
    Task<GuardainNotificationPreference?> GetGuardianPreferenceAsync(int guardianId, CancellationToken ct = default);

    Task SetGuardianPreferenceAsync(int guardianId, GuardainNotificationPreference preference, CancellationToken ct = default);

    Task VerifyGuardianEmailAsync(int guardianId, string email, CancellationToken ct = default);

    // Attachments
    Task AddAttachmentAsync(int notificationId, string fileName, string filePath, string contentType, long fileSize, string? description = null, bool isInline = false, CancellationToken ct = default);

    Task<IReadOnlyList<EventNotificationAttachment>> GetAttachmentsAsync(int notificationId, CancellationToken ct = default);

    Task RemoveAttachmentAsync(int attachmentId, CancellationToken ct = default);

    // Scheduled Notifications
    Task ScheduleNotificationAsync(int notificationId, DateTime scheduledAt, CancellationToken ct = default);

    Task ProcessScheduledNotificationsAsync(CancellationToken ct = default);

    // Reminders
    Task<ReminderConfig> CreateReminderConfigAsync(int eventId, int reminderValue, ReminderUnit reminderUnit, CancellationToken ct = default);

    Task UpdateReminderConfigAsync(ReminderConfig config, CancellationToken ct = default);

    Task DeleteReminderConfigAsync(int configId, CancellationToken ct = default);

    Task<IReadOnlyList<ReminderConfig>> GetReminderConfigsAsync(int eventId, CancellationToken ct = default);

    Task ProcessRemindersAsync(CancellationToken ct = default);

    // Duplicate detection
    Task<bool> IsDuplicateNotificationAsync(int eventId, string hash, CancellationToken ct = default);
}

public class EventNotificationDashboardDto
{
    public int TotalEvents { get; set; }
    public int TotalNotifications { get; set; }
    public int EmailsSent { get; set; }
    public int FailedCount { get; set; }
    public int PendingCount { get; set; }
    public int OpenCount { get; set; }
    public int DeliveredCount { get; set; }
    public int BounceCount { get; set; }
    public int ComplaintCount { get; set; }
    public int ClickCount { get; set; }
    public double DeliveryRate { get; set; }
    public double OpenRate { get; set; }
    public double ClickRate { get; set; }
    public double BounceRate { get; set; }
    public double GuardianReachPercent { get; set; }
    public int UniqueGuardiansReached { get; set; }
    public int TotalGuardians { get; set; }
}

public class EventNotificationAnalyticsDto
{
    public int NotificationId { get; set; }
    public int TotalRecipients { get; set; }
    public int SentCount { get; set; }
    public int FailedCount { get; set; }
    public int BounceCount { get; set; }
    public int ComplaintCount { get; set; }
    public int OpenedCount { get; set; }
    public int ClickCount { get; set; }
    public double DeliveryRate { get; set; }
    public double OpenRate { get; set; }
    public double ClickRate { get; set; }
    public double BounceRate { get; set; }
    public double ComplaintRate { get; set; }
    public List<EventNotificationRecipient> FailedRecipients { get; set; } = new();
}
