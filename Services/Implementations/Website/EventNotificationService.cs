using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Models.Entities.Guardian;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Website;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Website;

public class EventNotificationService : IEventNotificationService
{
    private readonly IEventNotificationRepository _notificationRepo;
    private readonly IEventNotificationRecipientRepository _recipientRepo;
    private readonly IEventNotificationLogRepository _logRepo;
    private readonly IEventNotificationQueueRepository _queueRepo;
    private readonly IGuardainNotificationPreferenceRepository _preferenceRepo;
    private readonly IEventNotificationAttachmentRepository _attachmentRepo;
    private readonly IScheduledNotificationRepository _scheduledRepo;
    private readonly IReminderConfigRepository _reminderConfigRepo;
    private readonly IEventRepository _eventRepo;
    private readonly IEmailTemplateRepository _templateRepo;
    private readonly ISchoolSettingRepository _settingRepo;
    private readonly IEmailTemplateService _templateService;
    private readonly IEmailSender _emailSender;
    private readonly IUnitOfWork _uow;

    public EventNotificationService(
        IEventNotificationRepository notificationRepo,
        IEventNotificationRecipientRepository recipientRepo,
        IEventNotificationLogRepository logRepo,
        IEventNotificationQueueRepository queueRepo,
        IGuardainNotificationPreferenceRepository preferenceRepo,
        IEventNotificationAttachmentRepository attachmentRepo,
        IScheduledNotificationRepository scheduledRepo,
        IReminderConfigRepository reminderConfigRepo,
        IEventRepository eventRepo,
        IEmailTemplateRepository templateRepo,
        ISchoolSettingRepository settingRepo,
        IEmailTemplateService templateService,
        IEmailSender emailSender,
        IUnitOfWork uow)
    {
        _notificationRepo = notificationRepo;
        _recipientRepo = recipientRepo;
        _logRepo = logRepo;
        _queueRepo = queueRepo;
        _preferenceRepo = preferenceRepo;
        _attachmentRepo = attachmentRepo;
        _scheduledRepo = scheduledRepo;
        _reminderConfigRepo = reminderConfigRepo;
        _eventRepo = eventRepo;
        _templateRepo = templateRepo;
        _settingRepo = settingRepo;
        _templateService = templateService;
        _emailSender = emailSender;
        _uow = uow;
    }

    public async Task<EventNotification> CreateNotificationAsync(int eventId, EventScope scope, int? classId = null,
        int? sectionId = null, int? groupId = null, string? studentIds = null, string? guardianIds = null,
        bool notifyGuardians = true, bool notifyStudents = false, bool primaryGuardianOnly = true,
        int? templateId = null, int? triggeredByUserId = null, CancellationToken ct = default)
    {
        var settings = await _settingRepo.GetCurrentSettingsAsync(ct);
        if (settings == null || !settings.EnableEventEmailNotifications)
        {
            throw new InvalidOperationException("Event email notifications are disabled in settings.");
        }

        var ev = await _eventRepo.GetByIdAsync(eventId, ct);
        if (ev == null || ev.IsDeleted)
            throw new KeyNotFoundException($"Event {eventId} not found.");

        var effectiveTemplateId = templateId ?? settings.DefaultEventTemplateId;
        var templateName = "EventPublished";

        if (effectiveTemplateId.HasValue)
        {
            var tpl = await _templateRepo.GetByIdAsync(effectiveTemplateId.Value, ct);
            if (tpl != null) templateName = tpl.TemplateName;
        }

        var hashInput = $"{eventId}|{scope}|{classId}|{sectionId}|{groupId}|{studentIds}|{guardianIds}|{notifyGuardians}|{notifyStudents}|{effectiveTemplateId}";
        var duplicateHash = ComputeHash(hashInput);

        var notification = new EventNotification
        {
            EventId = eventId,
            Scope = scope,
            ClassId = classId,
            SectionId = sectionId,
            GroupId = groupId,
            StudentIds = studentIds,
            GuardianIds = guardianIds,
            NotifyGuardians = notifyGuardians,
            NotifyStudents = notifyStudents,
            PrimaryGuardianOnly = primaryGuardianOnly,
            EmailTemplateId = effectiveTemplateId,
            Status = EventNotificationStatus.Pending,
            TriggeredByUserId = triggeredByUserId,
            DuplicateHash = duplicateHash,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "admin"
        };

        await _notificationRepo.AddAsync(notification, ct);
        await _uow.SaveChangesAsync(ct);

        await AddLogAsync(notification.Id, null, "Created", "Notification batch created.", "admin", ct);

        return notification;
    }

    public async Task QueueNotificationAsync(int notificationId, CancellationToken ct = default)
    {
        var notification = await _notificationRepo.GetByIdAsync(notificationId, ct);
        if (notification == null) throw new KeyNotFoundException($"Notification {notificationId} not found.");

        if (!string.IsNullOrEmpty(notification.DuplicateHash))
        {
            var isDup = await IsDuplicateNotificationAsync(notification.EventId, notification.DuplicateHash, ct);
            if (isDup)
            {
                await AddLogAsync(notificationId, null, "Skipped", "Duplicate notification detected, skipping queue.", "system", ct);
                return;
            }
        }

        var settings = await _settingRepo.GetCurrentSettingsAsync(ct);
        var schoolName = settings?.SchoolName ?? "School";
        var senderName = settings?.NotificationSenderName ?? schoolName;
        var ev = await _eventRepo.GetByIdAsync(notification.EventId, ct);

        var recipients = await ResolveRecipientsAsync(notification, ct);

        var template = notification.EmailTemplateId.HasValue
            ? await _templateRepo.GetByIdAsync(notification.EmailTemplateId.Value, ct)
            : null;

        var templateName = template?.TemplateName ?? "EventPublished";

        foreach (var r in recipients)
        {
            // Check guardian preferences
            if (r.GuardianId.HasValue)
            {
                var pref = await _preferenceRepo.GetByGuardianIdAsync(r.GuardianId.Value, ct);
                if (pref != null)
                {
                    if (!pref.OptInEventNotifications || !pref.OptInEmail)
                        continue;
                }
            }

            var placeholders = new Dictionary<string, string>
            {
                ["SchoolName"] = schoolName,
                ["GuardianName"] = r.RecipientName,
                ["EventTitle"] = ev?.Title ?? "",
                ["EventDate"] = ev?.EventDate.ToString("dddd, dd MMMM yyyy") ?? "",
                ["EventTime"] = ev?.EventDate.ToString("hh:mm tt") ?? "",
                ["Venue"] = ev?.EventLocation ?? "",
                ["Description"] = ev?.Description ?? ""
            };

            var subject = await _templateService.RenderTemplateSubjectAsync(templateName, placeholders, ct);
            if (string.IsNullOrEmpty(subject))
                subject = $"{schoolName} - New Event: {ev?.Title}";

            var body = await _templateService.RenderTemplateAsync(templateName, placeholders, ct);
            if (string.IsNullOrEmpty(body))
                body = $"<p>Dear {r.RecipientName},</p><p>A new school event has been announced: {ev?.Title} on {ev?.EventDate:dddd, dd MMMM yyyy}.</p>";

            var rec = new EventNotificationRecipient
            {
                EventNotificationId = notificationId,
                GuardianId = r.GuardianId,
                StudentId = r.StudentId,
                RecipientEmail = r.RecipientEmail,
                RecipientName = r.RecipientName,
                DeliveryStatus = EventNotificationRecipientStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system"
            };

            await _recipientRepo.AddAsync(rec, ct);
            await _uow.SaveChangesAsync(ct);

            var queueItem = new EventNotificationQueue
            {
                EventNotificationId = notificationId,
                RecipientId = rec.Id,
                RecipientEmail = r.RecipientEmail,
                RecipientName = r.RecipientName,
                Subject = subject,
                Body = body,
                Status = "Pending",
                MaxRetries = 3,
                ScheduledAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system"
            };

            await _queueRepo.AddAsync(queueItem, ct);
        }

        await _uow.SaveChangesAsync(ct);

        notification.TotalRecipients = recipients.Count;
        notification.Status = EventNotificationStatus.Pending;
        _notificationRepo.Update(notification);
        await _uow.SaveChangesAsync(ct);

        await AddLogAsync(notificationId, null, "Queued", $"{recipients.Count} emails queued after preference filtering.", "system", ct);
    }

    public async Task SendNotificationAsync(int notificationId, CancellationToken ct = default)
    {
        var notification = await _notificationRepo.GetByIdAsync(notificationId, ct);
        if (notification == null) throw new KeyNotFoundException($"Notification {notificationId} not found.");

        var settings = await _settingRepo.GetCurrentSettingsAsync(ct);
        var batchSize = settings?.MaximumEmailsPerBatch ?? 50;

        var pendingItems = await _queueRepo.Query()
            .Where(q => q.EventNotificationId == notificationId && !q.IsDeleted
                && (q.Status == "Pending" || q.Status == "Failed"))
            .OrderBy(q => q.Id)
            .Take(batchSize)
            .ToListAsync(ct);

        if (!pendingItems.Any()) return;

        notification.Status = EventNotificationStatus.Processing;
        _notificationRepo.Update(notification);
        await _uow.SaveChangesAsync(ct);

        int sent = 0, failed = 0;

        foreach (var item in pendingItems)
        {
            try
            {
                item.Status = "Processing";
                _queueRepo.Update(item);
                await _uow.SaveChangesAsync(ct);

                await _emailSender.SendAsync(item.RecipientEmail, item.Subject, item.Body, ct);

                item.Status = "Completed";
                item.ProcessedAt = DateTime.UtcNow;
                _queueRepo.Update(item);
                sent++;

                var rec = await _recipientRepo.GetByIdAsync(item.RecipientId ?? 0, ct);
                if (rec != null)
                {
                    rec.DeliveryStatus = EventNotificationRecipientStatus.Sent;
                    rec.DeliveredAt = DateTime.UtcNow;
                    _recipientRepo.Update(rec);
                }

                await AddLogAsync(notificationId, item.RecipientId, "Sent",
                    $"Email sent to {item.RecipientEmail}", "system", ct);
            }
            catch (Exception ex)
            {
                item.RetryCount++;
                item.Status = item.RetryCount >= item.MaxRetries ? "Failed" : "Failed";
                item.LastError = ex.Message;
                item.NextRetryAt = item.RetryCount < item.MaxRetries
                    ? DateTime.UtcNow.AddMinutes(Math.Pow(2, item.RetryCount) * 5)
                    : null;
                _queueRepo.Update(item);
                failed++;

                var rec = await _recipientRepo.GetByIdAsync(item.RecipientId ?? 0, ct);
                if (rec != null)
                {
                    rec.DeliveryStatus = EventNotificationRecipientStatus.Failed;
                    rec.FailureReason = ex.Message;
                    rec.RetryCount = item.RetryCount;
                    _recipientRepo.Update(rec);
                }

                await AddLogAsync(notificationId, item.RecipientId, "Failed",
                    $"Failed: {ex.Message}", "system", ct);
            }
        }

        await _uow.SaveChangesAsync(ct);

        notification.SentCount += sent;
        notification.FailedCount += failed;
        notification.Status = await _queueRepo.Query()
            .AnyAsync(q => q.EventNotificationId == notificationId && !q.IsDeleted
                && (q.Status == "Pending" || q.Status == "Failed" || q.Status == "Processing"), ct)
            ? EventNotificationStatus.Processing
            : EventNotificationStatus.Completed;

        if (notification.Status == EventNotificationStatus.Completed)
            notification.SentAt = DateTime.UtcNow;

        _notificationRepo.Update(notification);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task ResendNotificationAsync(int notificationId, CancellationToken ct = default)
    {
        var notification = await _notificationRepo.GetByIdAsync(notificationId, ct);
        if (notification == null) throw new KeyNotFoundException($"Notification {notificationId} not found.");

        var failedItems = await _queueRepo.Query()
            .Where(q => q.EventNotificationId == notificationId && !q.IsDeleted && q.Status == "Failed"
                && q.RetryCount < q.MaxRetries)
            .ToListAsync(ct);

        foreach (var item in failedItems)
        {
            item.Status = "Pending";
            item.LastError = null;
            item.NextRetryAt = null;
            _queueRepo.Update(item);
        }

        notification.Status = EventNotificationStatus.Pending;
        _notificationRepo.Update(notification);
        await _uow.SaveChangesAsync(ct);

        await AddLogAsync(notificationId, null, "Rescheduled",
            $"{failedItems.Count} failed items rescheduled for retry.", "admin", ct);

        if (failedItems.Any())
            await SendNotificationAsync(notificationId, ct);
    }

    public async Task<EventNotification?> GetNotificationAsync(int id, CancellationToken ct = default)
    {
        return await _notificationRepo.GetWithRecipientsAsync(id, ct);
    }

    public async Task<IReadOnlyList<EventNotification>> GetNotificationsByEventAsync(int eventId, CancellationToken ct = default)
    {
        return await _notificationRepo.Query()
            .Where(n => n.EventId == eventId && !n.IsDeleted)
            .Include(n => n.Event)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<EventNotification>> GetAllNotificationsAsync(CancellationToken ct = default)
    {
        return await _notificationRepo.Query()
            .Where(n => !n.IsDeleted)
            .Include(n => n.Event)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<EventNotification>> GetRecentNotificationsAsync(int count, CancellationToken ct = default)
    {
        return await _notificationRepo.Query()
            .Where(n => !n.IsDeleted)
            .Include(n => n.Event)
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync(ct);
    }

    public async Task<string> PreviewEmailAsync(int notificationId, CancellationToken ct = default)
    {
        var notification = await _notificationRepo.GetByIdAsync(notificationId, ct);
        if (notification == null) throw new KeyNotFoundException($"Notification {notificationId} not found.");

        var settings = await _settingRepo.GetCurrentSettingsAsync(ct);
        var schoolName = settings?.SchoolName ?? "School";
        var ev = await _eventRepo.GetByIdAsync(notification.EventId, ct);

        var template = notification.EmailTemplateId.HasValue
            ? await _templateRepo.GetByIdAsync(notification.EmailTemplateId.Value, ct)
            : null;

        var templateName = template?.TemplateName ?? "EventPublished";

        var placeholders = new Dictionary<string, string>
        {
            ["SchoolName"] = schoolName,
            ["GuardianName"] = "[Guardian Name]",
            ["EventTitle"] = ev?.Title ?? "",
            ["EventDate"] = ev?.EventDate.ToString("dddd, dd MMMM yyyy") ?? "",
            ["EventTime"] = ev?.EventDate.ToString("hh:mm tt") ?? "",
            ["Venue"] = ev?.EventLocation ?? "",
            ["Description"] = ev?.Description ?? ""
        };

        var body = await _templateService.RenderTemplateAsync(templateName, placeholders, ct);
        if (string.IsNullOrEmpty(body))
            body = $"<p>Dear [Guardian Name],</p><p>A new school event has been announced: {ev?.Title} on {ev?.EventDate:dddd, dd MMMM yyyy}.</p>";

        return body;
    }

    public async Task<IReadOnlyList<EventNotificationRecipient>> GetRecipientsAsync(int notificationId, CancellationToken ct = default)
    {
        return await _recipientRepo.Query()
            .Where(r => r.EventNotificationId == notificationId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);
    }

    // ── Dashboard & Analytics ──

    public async Task<EventNotificationDashboardDto> GetDashboardAsync(CancellationToken ct = default)
    {
        var totalEvents = await _eventRepo.CountAsync(e => !e.IsDeleted && e.IsPublished, ct);
        var notifications = await _notificationRepo.ListAsync(n => !n.IsDeleted, ct);

        var totalNotifications = notifications.Count;
        var sentCount = notifications.Sum(n => n.SentCount);
        var failedCount = notifications.Sum(n => n.FailedCount);
        var bounceCount = notifications.Sum(n => n.BounceCount);
        var complaintCount = notifications.Sum(n => n.ComplaintCount);
        var clickCount = notifications.Sum(n => n.ClickCount);

        var queuePending = await _queueRepo.GetPendingCountAsync(ct);

        var totalRecips = await _recipientRepo.Query()
            .Where(r => !r.IsDeleted)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Delivered = g.Count(x => x.DeliveryStatus == EventNotificationRecipientStatus.Delivered),
                Opened = g.Count(x => x.DeliveryStatus == EventNotificationRecipientStatus.Opened),
                Total = g.Count()
            })
            .FirstOrDefaultAsync(ct);

        var totalGuardians = await _uow.Repository<SchoolManagementSystem.Models.Entities.Guardian.Guardian>()
            .CountAsync(g => !g.IsDeleted, ct);

        var reachedGuardians = await _recipientRepo.Query()
            .Where(r => !r.IsDeleted && r.GuardianId != null
                && (r.DeliveryStatus == EventNotificationRecipientStatus.Sent
                    || r.DeliveryStatus == EventNotificationRecipientStatus.Delivered
                    || r.DeliveryStatus == EventNotificationRecipientStatus.Opened))
            .Select(r => r.GuardianId)
            .Distinct()
            .CountAsync(ct);

        var deliveryTotal = totalRecips?.Total ?? 0;
        var delivered = totalRecips?.Delivered ?? 0;
        var opened = totalRecips?.Opened ?? 0;

        var totalAttempted = sentCount + failedCount;

        return new EventNotificationDashboardDto
        {
            TotalEvents = totalEvents,
            TotalNotifications = totalNotifications,
            EmailsSent = sentCount,
            FailedCount = failedCount,
            PendingCount = queuePending,
            OpenCount = opened,
            DeliveredCount = delivered,
            BounceCount = bounceCount,
            ComplaintCount = complaintCount,
            ClickCount = clickCount,
            DeliveryRate = totalAttempted > 0 ? Math.Round((double)(sentCount - failedCount) / Math.Max(totalAttempted, 1) * 100, 2) : 0,
            OpenRate = sentCount > 0 ? Math.Round((double)opened / Math.Max(sentCount, 1) * 100, 2) : 0,
            ClickRate = sentCount > 0 ? Math.Round((double)clickCount / Math.Max(sentCount, 1) * 100, 2) : 0,
            BounceRate = sentCount > 0 ? Math.Round((double)bounceCount / Math.Max(sentCount, 1) * 100, 2) : 0,
            GuardianReachPercent = totalGuardians > 0 ? Math.Round((double)reachedGuardians / totalGuardians * 100, 2) : 0,
            UniqueGuardiansReached = reachedGuardians,
            TotalGuardians = totalGuardians
        };
    }

    public async Task<EventNotificationAnalyticsDto> GetAnalyticsAsync(int notificationId, CancellationToken ct = default)
    {
        var notification = await _notificationRepo.GetByIdAsync(notificationId, ct);
        if (notification == null) throw new KeyNotFoundException($"Notification {notificationId} not found.");

        var recipients = await _recipientRepo.Query()
            .Where(r => r.EventNotificationId == notificationId && !r.IsDeleted)
            .ToListAsync(ct);

        var totalRecipients = recipients.Count;
        var sentCount = recipients.Count(r => r.DeliveryStatus == EventNotificationRecipientStatus.Sent
            || r.DeliveryStatus == EventNotificationRecipientStatus.Delivered
            || r.DeliveryStatus == EventNotificationRecipientStatus.Opened);
        var failedCount = recipients.Count(r => r.DeliveryStatus == EventNotificationRecipientStatus.Failed);
        var bounceCount = recipients.Count(r => r.DeliveryStatus == EventNotificationRecipientStatus.Bounced);
        var complaintCount = recipients.Count(r => r.DeliveryStatus == EventNotificationRecipientStatus.Complained);
        var openedCount = recipients.Count(r => r.DeliveryStatus == EventNotificationRecipientStatus.Opened);
        var clickCount = recipients.Count(r => r.ClickedAt != null);

        var totalAttempted = sentCount + failedCount;

        return new EventNotificationAnalyticsDto
        {
            NotificationId = notificationId,
            TotalRecipients = totalRecipients,
            SentCount = sentCount,
            FailedCount = failedCount,
            BounceCount = bounceCount,
            ComplaintCount = complaintCount,
            OpenedCount = openedCount,
            ClickCount = clickCount,
            DeliveryRate = totalAttempted > 0 ? Math.Round((double)(sentCount - failedCount) / Math.Max(totalAttempted, 1) * 100, 2) : 0,
            OpenRate = sentCount > 0 ? Math.Round((double)openedCount / Math.Max(sentCount, 1) * 100, 2) : 0,
            ClickRate = sentCount > 0 ? Math.Round((double)clickCount / Math.Max(sentCount, 1) * 100, 2) : 0,
            BounceRate = sentCount > 0 ? Math.Round((double)bounceCount / Math.Max(sentCount, 1) * 100, 2) : 0,
            ComplaintRate = sentCount > 0 ? Math.Round((double)complaintCount / Math.Max(sentCount, 1) * 100, 2) : 0,
            FailedRecipients = recipients.Where(r => r.DeliveryStatus == EventNotificationRecipientStatus.Failed
                || r.DeliveryStatus == EventNotificationRecipientStatus.Bounced).ToList()
        };
    }

    // ── Guardian Preferences ──

    public async Task<GuardainNotificationPreference?> GetGuardianPreferenceAsync(int guardianId, CancellationToken ct = default)
    {
        return await _preferenceRepo.GetByGuardianIdAsync(guardianId, ct);
    }

    public async Task SetGuardianPreferenceAsync(int guardianId, GuardainNotificationPreference preference, CancellationToken ct = default)
    {
        var existing = await _preferenceRepo.GetByGuardianIdAsync(guardianId, ct);
        if (existing != null)
        {
            existing.OptInEventNotifications = preference.OptInEventNotifications;
            existing.OptInSMS = preference.OptInSMS;
            existing.OptInEmail = preference.OptInEmail;
            existing.OptInWhatsApp = preference.OptInWhatsApp;
            existing.OptInInApp = preference.OptInInApp;
            existing.SubscribedEventTypes = preference.SubscribedEventTypes;
            existing.QuietHoursStart = preference.QuietHoursStart;
            existing.QuietHoursEnd = preference.QuietHoursEnd;
            existing.AllowReminders = preference.AllowReminders;
            existing.ReminderLeadMinutes = preference.ReminderLeadMinutes;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "guardian";
            _preferenceRepo.Update(existing);
        }
        else
        {
            preference.GuardianId = guardianId;
            preference.CreatedAt = DateTime.UtcNow;
            preference.CreatedBy = "guardian";
            await _preferenceRepo.AddAsync(preference, ct);
        }

        await _uow.SaveChangesAsync(ct);
    }

    public async Task VerifyGuardianEmailAsync(int guardianId, string email, CancellationToken ct = default)
    {
        var pref = await _preferenceRepo.GetByGuardianIdAsync(guardianId, ct);
        if (pref == null)
        {
            pref = new GuardainNotificationPreference
            {
                GuardianId = guardianId,
                EmailVerified = true,
                VerifiedEmail = email,
                EmailVerifiedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system"
            };
            await _preferenceRepo.AddAsync(pref, ct);
        }
        else
        {
            pref.EmailVerified = true;
            pref.VerifiedEmail = email;
            pref.EmailVerifiedAt = DateTime.UtcNow;
            pref.UpdatedAt = DateTime.UtcNow;
            pref.UpdatedBy = "system";
            _preferenceRepo.Update(pref);
        }

        await _uow.SaveChangesAsync(ct);
        await AddLogAsync(0, null, "EmailVerified", $"Guardian {guardianId} verified email: {email}", "system", ct);
    }

    // ── Attachments ──

    public async Task AddAttachmentAsync(int notificationId, string fileName, string filePath, string contentType,
        long fileSize, string? description = null, bool isInline = false, CancellationToken ct = default)
    {
        var attachment = new EventNotificationAttachment
        {
            EventNotificationId = notificationId,
            FileName = fileName,
            FilePath = filePath,
            ContentType = contentType,
            FileSize = fileSize,
            Description = description,
            IsInline = isInline,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "admin"
        };

        await _attachmentRepo.AddAsync(attachment, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<EventNotificationAttachment>> GetAttachmentsAsync(int notificationId, CancellationToken ct = default)
    {
        return await _attachmentRepo.Query()
            .Where(a => a.EventNotificationId == notificationId && !a.IsDeleted)
            .OrderBy(a => a.Id)
            .ToListAsync(ct);
    }

    public async Task RemoveAttachmentAsync(int attachmentId, CancellationToken ct = default)
    {
        var attachment = await _attachmentRepo.GetByIdAsync(attachmentId, ct);
        if (attachment != null)
        {
            attachment.IsDeleted = true;
            attachment.UpdatedAt = DateTime.UtcNow;
            attachment.UpdatedBy = "admin";
            _attachmentRepo.Update(attachment);
            await _uow.SaveChangesAsync(ct);
        }
    }

    // ── Scheduled Notifications ──

    public async Task ScheduleNotificationAsync(int notificationId, DateTime scheduledAt, CancellationToken ct = default)
    {
        var notification = await _notificationRepo.GetByIdAsync(notificationId, ct);
        if (notification == null) throw new KeyNotFoundException($"Notification {notificationId} not found.");

        var scheduled = new ScheduledNotification
        {
            EventNotificationId = notificationId,
            ScheduledAt = scheduledAt,
            IsProcessed = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "admin"
        };

        await _scheduledRepo.AddAsync(scheduled, ct);

        notification.Status = EventNotificationStatus.Pending;
        _notificationRepo.Update(notification);

        await _uow.SaveChangesAsync(ct);

        await AddLogAsync(notificationId, null, "Scheduled",
            $"Notification scheduled for {scheduledAt:yyyy-MM-dd HH:mm:ss UTC}.", "admin", ct);
    }

    public async Task ProcessScheduledNotificationsAsync(CancellationToken ct = default)
    {
        var due = await _scheduledRepo.GetPendingScheduledAsync(ct);

        foreach (var scheduled in due)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                await QueueNotificationAsync(scheduled.EventNotificationId, ct);

                if (ct.IsCancellationRequested) break;

                var settings = await _settingRepo.GetCurrentSettingsAsync(ct);
                if (settings != null && settings.SendImmediately)
                {
                    await SendNotificationAsync(scheduled.EventNotificationId, ct);
                }

                scheduled.IsProcessed = true;
                scheduled.ProcessedAt = DateTime.UtcNow;
                _scheduledRepo.Update(scheduled);
                await _uow.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                await AddLogAsync(scheduled.EventNotificationId, null, "ScheduledProcessingFailed",
                    $"Failed to process scheduled notification: {ex.Message}", "system", ct);
            }
        }
    }

    // ── Reminders ──

    public async Task<ReminderConfig> CreateReminderConfigAsync(int eventId, int reminderValue, ReminderUnit reminderUnit, CancellationToken ct = default)
    {
        var config = new ReminderConfig
        {
            EventId = eventId,
            IsActive = true,
            ReminderValue = reminderValue,
            ReminderUnit = reminderUnit,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "admin"
        };

        await _reminderConfigRepo.AddAsync(config, ct);
        await _uow.SaveChangesAsync(ct);

        return config;
    }

    public async Task UpdateReminderConfigAsync(ReminderConfig config, CancellationToken ct = default)
    {
        var existing = await _reminderConfigRepo.GetByIdAsync(config.Id, ct);
        if (existing == null) throw new KeyNotFoundException($"ReminderConfig {config.Id} not found.");

        existing.ReminderValue = config.ReminderValue;
        existing.ReminderUnit = config.ReminderUnit;
        existing.IsActive = config.IsActive;
        existing.Notes = config.Notes;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = "admin";

        _reminderConfigRepo.Update(existing);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteReminderConfigAsync(int configId, CancellationToken ct = default)
    {
        var config = await _reminderConfigRepo.GetByIdAsync(configId, ct);
        if (config != null)
        {
            config.IsDeleted = true;
            config.UpdatedAt = DateTime.UtcNow;
            config.UpdatedBy = "admin";
            _reminderConfigRepo.Update(config);
            await _uow.SaveChangesAsync(ct);
        }
    }

    public async Task<IReadOnlyList<ReminderConfig>> GetReminderConfigsAsync(int eventId, CancellationToken ct = default)
    {
        return await _reminderConfigRepo.Query()
            .Where(r => r.EventId == eventId && !r.IsDeleted)
            .OrderBy(r => r.Id)
            .ToListAsync(ct);
    }

    public async Task ProcessRemindersAsync(CancellationToken ct = default)
    {
        var settings = await _settingRepo.GetCurrentSettingsAsync(ct);
        if (settings == null || !settings.EnableEventReminders) return;

        var activeReminders = await _reminderConfigRepo.GetActiveRemindersDueAsync(ct);

        foreach (var reminder in activeReminders)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                var ev = await _eventRepo.GetByIdAsync(reminder.EventId, ct);
                if (ev == null || ev.IsDeleted || !ev.IsPublished) continue;

                var reminderDate = reminder.ReminderUnit switch
                {
                    ReminderUnit.Minutes => ev.EventDate.AddMinutes(-reminder.ReminderValue),
                    ReminderUnit.Hours => ev.EventDate.AddHours(-reminder.ReminderValue),
                    ReminderUnit.Days => ev.EventDate.AddDays(-reminder.ReminderValue),
                    ReminderUnit.Weeks => ev.EventDate.AddDays(-reminder.ReminderValue * 7),
                    _ => ev.EventDate.AddDays(-reminder.ReminderValue)
                };

                if (DateTime.UtcNow < reminderDate) continue;

                if (settings.MaxRemindersPerEvent > 0 && reminder.SentCount >= settings.MaxRemindersPerEvent)
                    continue;

                var notification = await CreateNotificationAsync(reminder.EventId,
                    EventScope.AllStudents, notifyGuardians: true,
                    notifyStudents: settings.EnableStudentNotifications,
                    primaryGuardianOnly: true, ct: ct);

                await QueueNotificationAsync(notification.Id, ct);

                if (settings.SendImmediately)
                {
                    await SendNotificationAsync(notification.Id, ct);
                }

                reminder.LastSentAt = DateTime.UtcNow;
                reminder.SentCount++;
                _reminderConfigRepo.Update(reminder);
                await _uow.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                await AddLogAsync(0, null, "ReminderProcessingFailed",
                    $"Failed to process reminder {reminder.Id} for event {reminder.EventId}: {ex.Message}", "system", ct);
            }
        }
    }

    // ── Duplicate detection ──

    public async Task<bool> IsDuplicateNotificationAsync(int eventId, string hash, CancellationToken ct = default)
    {
        return await _notificationRepo.Query()
            .AnyAsync(n => n.EventId == eventId && n.DuplicateHash == hash
                && n.Status != EventNotificationStatus.Failed && !n.IsDeleted, ct);
    }

    // ── Private helpers ──

    private async Task<List<RecipientInfo>> ResolveRecipientsAsync(EventNotification notification, CancellationToken ct = default)
    {
        var recipients = new List<RecipientInfo>();

        if (!notification.NotifyGuardians && !notification.NotifyStudents)
            return recipients;

        IQueryable<StudentGuardian>? query = null;

        switch (notification.Scope)
        {
            case EventScope.AllStudents:
                query = _uow.Repository<StudentGuardian>().Query()
                    .Where(sg => !sg.IsDeleted && sg.ReceivesEmail);
                break;
            case EventScope.SpecificClass when notification.ClassId.HasValue:
                query = _uow.Repository<StudentGuardian>().Query()
                    .Where(sg => !sg.IsDeleted && sg.ReceivesEmail
                        && sg.Student != null && sg.Student.ClassId == notification.ClassId.Value);
                break;
            case EventScope.SpecificSection when notification.SectionId.HasValue:
                query = _uow.Repository<StudentGuardian>().Query()
                    .Where(sg => !sg.IsDeleted && sg.ReceivesEmail
                        && sg.Student != null && sg.Student.SectionId == notification.SectionId.Value);
                break;
            case EventScope.SpecificGroup when notification.GroupId.HasValue:
                query = _uow.Repository<StudentGuardian>().Query()
                    .Where(sg => !sg.IsDeleted && sg.ReceivesEmail
                        && sg.Student != null && sg.Student.StudentGroupId == notification.GroupId.Value);
                break;
            case EventScope.SpecificStudents when !string.IsNullOrEmpty(notification.StudentIds):
                var sIds = notification.StudentIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.TryParse(x.Trim(), out var id) ? id : (int?)0)
                    .Where(x => x > 0)
                    .Select(x => x!.Value)
                    .ToHashSet();
                query = _uow.Repository<StudentGuardian>().Query()
                    .Where(sg => !sg.IsDeleted && sg.ReceivesEmail
                        && sIds.Contains(sg.StudentId));
                break;
            case EventScope.SpecificGuardians when !string.IsNullOrEmpty(notification.GuardianIds):
                var gIds = notification.GuardianIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.TryParse(x.Trim(), out var id) ? id : (int?)0)
                    .Where(x => x > 0)
                    .Select(x => x!.Value)
                    .ToHashSet();
                query = _uow.Repository<StudentGuardian>().Query()
                    .Where(sg => !sg.IsDeleted && sg.ReceivesEmail
                        && gIds.Contains(sg.GuardianId));
                break;
        }

        if (query != null)
        {
            if (notification.PrimaryGuardianOnly)
                query = query.Where(sg => sg.IsPrimaryGuardian);

            if (!string.IsNullOrEmpty(notification.StudentIds) && notification.Scope != EventScope.SpecificStudents)
            {
                var sIds = notification.StudentIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.TryParse(x.Trim(), out var id) ? id : (int?)0)
                    .Where(x => x > 0)
                    .Select(x => x!.Value)
                    .ToHashSet();
                if (sIds.Any())
                    query = query.Where(sg => sIds.Contains(sg.StudentId));
            }

            var mappings = await query
                .Include(sg => sg.Guardian)
                .Include(sg => sg.Student)
                .ToListAsync(ct);

            foreach (var sg in mappings)
            {
                if (notification.NotifyGuardians && sg.Guardian != null && !string.IsNullOrEmpty(sg.Guardian.Email))
                {
                    recipients.Add(new RecipientInfo
                    {
                        GuardianId = sg.GuardianId,
                        RecipientEmail = sg.Guardian.Email,
                        RecipientName = sg.Guardian.FullName
                    });
                }

                if (notification.NotifyStudents && sg.Student != null && !string.IsNullOrEmpty(sg.Student.EmailAddress))
                {
                    recipients.Add(new RecipientInfo
                    {
                        StudentId = sg.StudentId,
                        RecipientEmail = sg.Student.EmailAddress,
                        RecipientName = sg.Student.FullName
                    });
                }
            }
        }

        var deduped = recipients
            .GroupBy(r => new { r.RecipientEmail, r.GuardianId, r.StudentId })
            .Select(g => g.First())
            .ToList();

        return deduped;
    }

    private async Task AddLogAsync(int notificationId, int? recipientId, string action, string details, string performedBy, CancellationToken ct = default)
    {
        var log = new EventNotificationLog
        {
            EventNotificationId = notificationId,
            RecipientId = recipientId,
            Action = action,
            Details = details,
            PerformedBy = performedBy,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = performedBy
        };
        await _logRepo.AddAsync(log, ct);
        await _uow.SaveChangesAsync(ct);
    }

    private static string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private class RecipientInfo
    {
        public int? GuardianId { get; set; }
        public int? StudentId { get; set; }
        public string RecipientEmail { get; set; } = string.Empty;
        public string RecipientName { get; set; } = string.Empty;
    }
}
