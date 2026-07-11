using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Repositories.Interfaces.Website;

namespace SchoolManagementSystem.Repositories.Implementations.Website;

public class SchoolSettingRepository : BaseRepository<SchoolSetting>, ISchoolSettingRepository
{
    public SchoolSettingRepository(SchoolDbContext context) : base(context)
    {
    }

    public async Task<SchoolSetting?> GetCurrentSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await _set.FirstOrDefaultAsync(s => !s.IsDeleted, cancellationToken);
    }
}

public class WebsitePageRepository : BaseRepository<WebsitePage>, IWebsitePageRepository
{
    public WebsitePageRepository(SchoolDbContext context) : base(context)
    {
    }

    public async Task<WebsitePage?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var lowerSlug = slug.ToLowerInvariant().Trim();
        return await _set.FirstOrDefaultAsync(p => p.Slug.ToLower() == lowerSlug && p.IsPublished && !p.IsDeleted, cancellationToken);
    }
}

public class SliderRepository : BaseRepository<Slider>, ISliderRepository
{
    public SliderRepository(SchoolDbContext context) : base(context)
    {
    }
}

public class EventRepository : BaseRepository<Event>, IEventRepository
{
    public EventRepository(SchoolDbContext context) : base(context)
    {
    }
}

public class EventCategoryRepository : BaseRepository<EventCategory>, IEventCategoryRepository
{
    public EventCategoryRepository(SchoolDbContext context) : base(context)
    {
    }
}

public class GalleryRepository : BaseRepository<Gallery>, IGalleryRepository
{
    public GalleryRepository(SchoolDbContext context) : base(context)
    {
    }

    public async Task<Gallery?> GetWithImagesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _set
            .AsNoTracking()
            .Include(g => g.Images.Where(i => !i.IsDeleted))
            .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted, cancellationToken);
    }
}

public class GalleryImageRepository : BaseRepository<GalleryImage>, IGalleryImageRepository
{
    public GalleryImageRepository(SchoolDbContext context) : base(context)
    {
    }
}

public class NoticeRepository : BaseRepository<Notice>, INoticeRepository
{
    public NoticeRepository(SchoolDbContext context) : base(context)
    {
    }
}

public class ContactMessageRepository : BaseRepository<ContactMessage>, IContactMessageRepository
{
    public ContactMessageRepository(SchoolDbContext context) : base(context)
    {
    }
}

public class EmailTemplateRepository : BaseRepository<EmailTemplate>, IEmailTemplateRepository
{
    public EmailTemplateRepository(SchoolDbContext context) : base(context)
    {
    }
}

public class EventNotificationRepository : BaseRepository<EventNotification>, IEventNotificationRepository
{
    public EventNotificationRepository(SchoolDbContext context) : base(context)
    {
    }

    public async Task<EventNotification?> GetWithRecipientsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _set
            .AsNoTracking()
            .Include(n => n.Event)
            .Include(n => n.Recipients.Where(r => !r.IsDeleted))
            .Include(n => n.Logs.Where(l => !l.IsDeleted))
            .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted, cancellationToken);
    }
}

public class EventNotificationRecipientRepository : BaseRepository<EventNotificationRecipient>, IEventNotificationRecipientRepository
{
    public EventNotificationRecipientRepository(SchoolDbContext context) : base(context)
    {
    }
}

public class EventNotificationLogRepository : BaseRepository<EventNotificationLog>, IEventNotificationLogRepository
{
    public EventNotificationLogRepository(SchoolDbContext context) : base(context)
    {
    }
}

public class EventNotificationQueueRepository : BaseRepository<EventNotificationQueue>, IEventNotificationQueueRepository
{
    public EventNotificationQueueRepository(SchoolDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<EventNotificationQueue>> GetPendingBatchAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _set
            .Where(q => !q.IsDeleted
                && (q.Status == "Pending"
                    || (q.Status == "Failed" && q.RetryCount < q.MaxRetries
                        && q.NextRetryAt != null && q.NextRetryAt <= now)))
            .OrderBy(q => q.Id)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _set
            .Where(q => !q.IsDeleted
                && (q.Status == "Pending"
                    || (q.Status == "Failed" && q.RetryCount < q.MaxRetries
                        && q.NextRetryAt != null && q.NextRetryAt <= now)))
            .CountAsync(cancellationToken);
    }
}

public class GuardainNotificationPreferenceRepository : BaseRepository<GuardainNotificationPreference>, IGuardainNotificationPreferenceRepository
{
    public GuardainNotificationPreferenceRepository(SchoolDbContext context) : base(context)
    {
    }

    public async Task<GuardainNotificationPreference?> GetByGuardianIdAsync(int guardianId, CancellationToken cancellationToken = default)
    {
        return await _set.FirstOrDefaultAsync(p => p.GuardianId == guardianId && !p.IsDeleted, cancellationToken);
    }
}

public class EventNotificationAttachmentRepository : BaseRepository<EventNotificationAttachment>, IEventNotificationAttachmentRepository
{
    public EventNotificationAttachmentRepository(SchoolDbContext context) : base(context)
    {
    }
}

public class ScheduledNotificationRepository : BaseRepository<ScheduledNotification>, IScheduledNotificationRepository
{
    public ScheduledNotificationRepository(SchoolDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ScheduledNotification>> GetPendingScheduledAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _set
            .Where(s => !s.IsDeleted && !s.IsProcessed && s.ScheduledAt <= now)
            .OrderBy(s => s.ScheduledAt)
            .ToListAsync(cancellationToken);
    }
}

public class ReminderConfigRepository : BaseRepository<ReminderConfig>, IReminderConfigRepository
{
    public ReminderConfigRepository(SchoolDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ReminderConfig>> GetActiveRemindersDueAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _set
            .Where(r => !r.IsDeleted && r.IsActive)
            .ToListAsync(cancellationToken);
    }
}
