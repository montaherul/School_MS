using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Entities.Communication;

namespace SchoolManagementSystem.Services.Interfaces.Website;

public interface ISchoolWebsiteService
{
    Task<SchoolSetting> GetSettingsAsync(CancellationToken cancellationToken = default);
    Task UpdateSettingsAsync(SchoolSetting settings, CancellationToken cancellationToken = default);
}

public interface ISliderService
{
    Task<IReadOnlyList<Slider>> GetActiveSlidersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Slider>> GetAllSlidersAsync(CancellationToken cancellationToken = default);
    Task<Slider?> GetSliderByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateSliderAsync(Slider slider, CancellationToken cancellationToken = default);
    Task UpdateSliderAsync(Slider slider, CancellationToken cancellationToken = default);
    Task DeleteSliderAsync(int id, CancellationToken cancellationToken = default);
}

public interface INoticeService
{
    Task<IReadOnlyList<Notice>> GetLatestNoticesAsync(int count, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Notice> Notices, int TotalCount)> GetPagedNoticesAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Notice?> GetNoticeByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateNoticeAsync(Notice notice, CancellationToken cancellationToken = default);
    Task UpdateNoticeAsync(Notice notice, CancellationToken cancellationToken = default);
    Task DeleteNoticeAsync(int id, CancellationToken cancellationToken = default);
}

public interface IEventService
{
    Task<IReadOnlyList<Event>> GetUpcomingEventsAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> GetAllEventsAsync(CancellationToken cancellationToken = default);
    Task<Event?> GetEventByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateEventAsync(Event ev, CancellationToken cancellationToken = default);
    Task UpdateEventAsync(Event ev, CancellationToken cancellationToken = default);
    Task DeleteEventAsync(int id, CancellationToken cancellationToken = default);
}

public interface IGalleryService
{
    Task<IReadOnlyList<Gallery>> GetAllAlbumsAsync(CancellationToken cancellationToken = default);
    Task<Gallery?> GetAlbumWithImagesAsync(int id, CancellationToken cancellationToken = default);
    Task<Gallery?> GetAlbumByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAlbumAsync(Gallery album, CancellationToken cancellationToken = default);
    Task UpdateAlbumAsync(Gallery album, CancellationToken cancellationToken = default);
    Task DeleteAlbumAsync(int id, CancellationToken cancellationToken = default);
    
    // Gallery Images
    Task AddImageToAlbumAsync(GalleryImage image, CancellationToken cancellationToken = default);
    Task DeleteImageAsync(int imageId, CancellationToken cancellationToken = default);
}

public interface IWebsitePageService
{
    Task<WebsitePage?> GetPageBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WebsitePage>> GetAllPagesAsync(CancellationToken cancellationToken = default);
    Task<WebsitePage?> GetPageByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreatePageAsync(WebsitePage page, CancellationToken cancellationToken = default);
    Task UpdatePageAsync(WebsitePage page, CancellationToken cancellationToken = default);
    Task DeletePageAsync(int id, CancellationToken cancellationToken = default);
}

public interface IContactMessageService
{
    Task<IReadOnlyList<ContactMessage>> GetMessagesAsync(CancellationToken cancellationToken = default);
    Task<ContactMessage?> GetMessageByIdAsync(int id, CancellationToken cancellationToken = default);
    Task SaveMessageAsync(ContactMessage message, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteMessageAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(CancellationToken cancellationToken = default);
}

public interface IEmailTemplateService
{
    Task<IReadOnlyList<EmailTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default);
    Task<EmailTemplate?> GetTemplateByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EmailTemplate?> GetTemplateByNameAsync(string name, CancellationToken cancellationToken = default);
    Task CreateTemplateAsync(EmailTemplate template, CancellationToken cancellationToken = default);
    Task UpdateTemplateAsync(EmailTemplate template, CancellationToken cancellationToken = default);
    Task DeleteTemplateAsync(int id, CancellationToken cancellationToken = default);
    Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> placeholders, CancellationToken cancellationToken = default);
    Task<string> RenderTemplateSubjectAsync(string templateName, Dictionary<string, string> placeholders, CancellationToken cancellationToken = default);
}

public interface IAnnouncementService
{
    Task<IReadOnlyList<Announcement>> GetActiveAnnouncementsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Announcement>> GetAllAnnouncementsAsync(CancellationToken cancellationToken = default);
    Task<Announcement?> GetAnnouncementByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAnnouncementAsync(Announcement announcement, CancellationToken cancellationToken = default);
    Task UpdateAnnouncementAsync(Announcement announcement, CancellationToken cancellationToken = default);
    Task DeleteAnnouncementAsync(int id, CancellationToken cancellationToken = default);
}

public interface IAdmissionFeeStructureService
{
    Task<IReadOnlyList<AdmissionFeeStructure>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdmissionFeeStructure>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<AdmissionFeeStructure?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAsync(AdmissionFeeStructure fee, CancellationToken cancellationToken = default);
    Task UpdateAsync(AdmissionFeeStructure fee, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
