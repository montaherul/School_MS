using System.Threading;
using System.Threading.Tasks;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Entities.Communication;

namespace SchoolManagementSystem.Repositories.Interfaces.Website;

public interface ISchoolSettingRepository : IBaseRepository<SchoolSetting>
{
    Task<SchoolSetting?> GetCurrentSettingsAsync(CancellationToken cancellationToken = default);
}

public interface IWebsitePageRepository : IBaseRepository<WebsitePage>
{
    Task<WebsitePage?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
}

public interface ISliderRepository : IBaseRepository<Slider>
{
}

public interface IEventRepository : IBaseRepository<Event>
{
}

public interface IEventCategoryRepository : IBaseRepository<EventCategory>
{
}

public interface IGalleryRepository : IBaseRepository<Gallery>
{
    Task<Gallery?> GetWithImagesAsync(int id, CancellationToken cancellationToken = default);
}

public interface IGalleryImageRepository : IBaseRepository<GalleryImage>
{
}

public interface INoticeRepository : IBaseRepository<Notice>
{
}

public interface IContactMessageRepository : IBaseRepository<ContactMessage>
{
}

public interface IEmailTemplateRepository : IBaseRepository<EmailTemplate>
{
}

public interface IEventNotificationRepository : IBaseRepository<EventNotification>
{
    Task<EventNotification?> GetWithRecipientsAsync(int id, CancellationToken cancellationToken = default);
}

public interface IEventNotificationRecipientRepository : IBaseRepository<EventNotificationRecipient>
{
}

public interface IEventNotificationLogRepository : IBaseRepository<EventNotificationLog>
{
}

public interface IEventNotificationQueueRepository : IBaseRepository<EventNotificationQueue>
{
    Task<IReadOnlyList<EventNotificationQueue>> GetPendingBatchAsync(int batchSize, CancellationToken cancellationToken = default);
    Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default);
}

public interface IGuardainNotificationPreferenceRepository : IBaseRepository<GuardainNotificationPreference>
{
    Task<GuardainNotificationPreference?> GetByGuardianIdAsync(int guardianId, CancellationToken cancellationToken = default);
}

public interface IEventNotificationAttachmentRepository : IBaseRepository<EventNotificationAttachment>
{
}

public interface IScheduledNotificationRepository : IBaseRepository<ScheduledNotification>
{
    Task<IReadOnlyList<ScheduledNotification>> GetPendingScheduledAsync(CancellationToken cancellationToken = default);
}

public interface IReminderConfigRepository : IBaseRepository<ReminderConfig>
{
    Task<IReadOnlyList<ReminderConfig>> GetActiveRemindersDueAsync(CancellationToken cancellationToken = default);
}
