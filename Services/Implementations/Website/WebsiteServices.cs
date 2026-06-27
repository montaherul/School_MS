using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Website;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Website;

public class SchoolWebsiteService : ISchoolWebsiteService
{
    private readonly ISchoolSettingRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "SchoolSettings";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public SchoolWebsiteService(ISchoolSettingRepository repo, IUnitOfWork uow, IMemoryCache cache)
    {
        _repo = repo;
        _uow = uow;
        _cache = cache;
    }

    public async Task<SchoolSetting> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey, out SchoolSetting? cached) && cached != null)
            return cached;

        var settings = await _repo.GetCurrentSettingsAsync(cancellationToken);
        if (settings == null)
        {
            settings = new SchoolSetting
            {
                SchoolName = "Bangladesh Government High School",
                ShortName = "BGHS",
                EIIN = "123456",
                Address = "Dhaka, Bangladesh",
                Phone = "+8801700000000",
                Email = "info@school.gov.bd",
                Website = "https://school-ms-7l3e.onrender.com/",
                PrincipalName = "Principal Name",
                PrincipalMessage = "Welcome to our school. We strive for excellence in education and holistic development of our students.",
                Mission = "To provide high quality education and build character.",
                Vision = "To be a leading educational institution in Bangladesh.",
                FooterText = "© 2026 Bangladesh Government High School. All Rights Reserved."
            };
            await _repo.AddAsync(settings, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        _cache.Set(CacheKey, settings, CacheDuration);
        return settings;
    }

    public async Task UpdateSettingsAsync(SchoolSetting settings, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetCurrentSettingsAsync(cancellationToken);
        if (existing == null)
        {
            await _repo.AddAsync(settings, cancellationToken);
        }
        else
        {
            existing.SchoolName = settings.SchoolName;
            existing.ShortName = settings.ShortName;
            existing.BanglaName = settings.BanglaName;
            existing.EIIN = settings.EIIN;
            existing.SchoolCode = settings.SchoolCode;
            existing.EstablishedYear = settings.EstablishedYear;
            existing.SchoolMotto = settings.SchoolMotto;
            existing.SchoolDescription = settings.SchoolDescription;
            existing.Address = settings.Address;
            existing.Phone = settings.Phone;
            existing.Mobile = settings.Mobile;
            existing.Email = settings.Email;
            existing.Website = settings.Website;
            existing.FacebookUrl = settings.FacebookUrl;
            existing.YouTubeUrl = settings.YouTubeUrl;
            existing.InstagramUrl = settings.InstagramUrl;
            existing.LinkedInUrl = settings.LinkedInUrl;
            existing.TwitterUrl = settings.TwitterUrl;
            if (!string.IsNullOrEmpty(settings.LogoPath)) existing.LogoPath = settings.LogoPath;
            if (!string.IsNullOrEmpty(settings.FaviconPath)) existing.FaviconPath = settings.FaviconPath;
            if (!string.IsNullOrEmpty(settings.LoginLogoPath)) existing.LoginLogoPath = settings.LoginLogoPath;
            if (!string.IsNullOrEmpty(settings.FooterLogoPath)) existing.FooterLogoPath = settings.FooterLogoPath;
            if (!string.IsNullOrEmpty(settings.WebsiteBannerPath)) existing.WebsiteBannerPath = settings.WebsiteBannerPath;
            existing.PrincipalName = settings.PrincipalName;
            existing.PrincipalDesignation = settings.PrincipalDesignation;
            existing.PrincipalMessage = settings.PrincipalMessage;
            if (!string.IsNullOrEmpty(settings.PrincipalImagePath)) existing.PrincipalImagePath = settings.PrincipalImagePath;
            if (!string.IsNullOrEmpty(settings.PrincipalSignaturePath)) existing.PrincipalSignaturePath = settings.PrincipalSignaturePath;
            existing.PrincipalQualification = settings.PrincipalQualification;
            existing.Mission = settings.Mission;
            existing.Vision = settings.Vision;
            existing.FooterText = settings.FooterText;
            existing.CopyrightText = settings.CopyrightText;
            existing.GoogleMapEmbed = settings.GoogleMapEmbed;
            existing.MetaTitle = settings.MetaTitle;
            existing.MetaDescription = settings.MetaDescription;
            existing.MetaKeywords = settings.MetaKeywords;
            if (!string.IsNullOrEmpty(settings.OgImagePath)) existing.OgImagePath = settings.OgImagePath;
            existing.OgTitle = settings.OgTitle;
            existing.OgDescription = settings.OgDescription;
            existing.WelcomeHeading = settings.WelcomeHeading;
            existing.WelcomeTagline = settings.WelcomeTagline;
            existing.WelcomeText = settings.WelcomeText;
            existing.SchoolHistory = settings.SchoolHistory;
            existing.OfficeHours = settings.OfficeHours;
            existing.StudentLabel = settings.StudentLabel;
            existing.TeacherLabel = settings.TeacherLabel;
            existing.EmployeeLabel = settings.EmployeeLabel;
            existing.ClassLabel = settings.ClassLabel;
            existing.ShowSlider = settings.ShowSlider;
            existing.ShowPrincipalMessage = settings.ShowPrincipalMessage;
            existing.ShowNotices = settings.ShowNotices;
            existing.ShowEvents = settings.ShowEvents;
            existing.ShowGallery = settings.ShowGallery;
            existing.ShowAdmissionCTA = settings.ShowAdmissionCTA;
            existing.ShowStatistics = settings.ShowStatistics;
            existing.ShowWelcomeSection = settings.ShowWelcomeSection;

            // Admission Page Settings
            existing.AdmissionEnabled = settings.AdmissionEnabled;
            existing.OnlineAdmissionEnabled = settings.OnlineAdmissionEnabled;
            existing.ShowAdmissionPage = settings.ShowAdmissionPage;
            existing.ShowAdmissionFees = settings.ShowAdmissionFees;
            existing.ShowAdmissionGuidelines = settings.ShowAdmissionGuidelines;
            existing.ShowAdmissionRequirements = settings.ShowAdmissionRequirements;
            existing.ShowAdmissionDownloads = settings.ShowAdmissionDownloads;
            existing.AdmissionTitle = settings.AdmissionTitle;
            existing.AdmissionSubtitle = settings.AdmissionSubtitle;
            existing.AdmissionGuidelines = settings.AdmissionGuidelines;
            existing.AdmissionEligibility = settings.AdmissionEligibility;
            existing.AdmissionRequirements = settings.AdmissionRequirements;
            existing.AdmissionProcess = settings.AdmissionProcess;
            existing.AdmissionFeeNote = settings.AdmissionFeeNote;
            existing.AdmissionCtaTitle = settings.AdmissionCtaTitle;
            existing.AdmissionCtaText = settings.AdmissionCtaText;
            existing.AdmissionOpenDate = settings.AdmissionOpenDate;
            existing.AdmissionCloseDate = settings.AdmissionCloseDate;
            if (!string.IsNullOrEmpty(settings.AdmissionCircularPath)) existing.AdmissionCircularPath = settings.AdmissionCircularPath;
            if (!string.IsNullOrEmpty(settings.AdmissionFormPath)) existing.AdmissionFormPath = settings.AdmissionFormPath;

            // Admission SEO
            existing.AdmissionMetaTitle = settings.AdmissionMetaTitle;
            existing.AdmissionMetaDescription = settings.AdmissionMetaDescription;
            existing.AdmissionMetaKeywords = settings.AdmissionMetaKeywords;
            existing.AdmissionOgTitle = settings.AdmissionOgTitle;
            existing.AdmissionOgDescription = settings.AdmissionOgDescription;
            if (!string.IsNullOrEmpty(settings.AdmissionOgImagePath)) existing.AdmissionOgImagePath = settings.AdmissionOgImagePath;

            existing.AllowResultWithDue = settings.AllowResultWithDue;

            // Guardian Portal Feature Toggles
            existing.EnableGuardianPortal = settings.EnableGuardianPortal;
            existing.EnableGuardianActivation = settings.EnableGuardianActivation;
            existing.RequireGuardianForAdmission = settings.RequireGuardianForAdmission;
            existing.EnableGuardianNotifications = settings.EnableGuardianNotifications;

            // Event Notification Settings
            existing.EnableEventEmailNotifications = settings.EnableEventEmailNotifications;
            existing.EnableStudentNotifications = settings.EnableStudentNotifications;
            existing.SendImmediately = settings.SendImmediately;
            existing.SendOnPublish = settings.SendOnPublish;
            existing.DailyDigestMode = settings.DailyDigestMode;
            existing.MaximumEmailsPerBatch = settings.MaximumEmailsPerBatch;
            existing.DefaultEventTemplateId = settings.DefaultEventTemplateId;
            existing.NotificationSenderName = settings.NotificationSenderName;
            existing.NotificationSenderEmail = settings.NotificationSenderEmail;

            existing.SmtpHost = settings.SmtpHost;
            existing.SmtpPort = settings.SmtpPort;
            existing.SmtpEnableSsl = settings.SmtpEnableSsl;
            existing.SmtpUserName = settings.SmtpUserName;
            existing.SmtpPassword = settings.SmtpPassword;
            existing.SmtpFromEmail = settings.SmtpFromEmail;
            existing.BaseUrl = settings.BaseUrl;
            existing.LocalUrl = settings.LocalUrl;
            existing.PublicUrl = settings.PublicUrl;

            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
        }
        await _uow.SaveChangesAsync(cancellationToken);
        _cache.Remove(CacheKey);
    }
}

public class ContactMessageService : IContactMessageService
{
    private readonly IContactMessageRepository _repo;
    private readonly IUnitOfWork _uow;

    public ContactMessageService(IContactMessageRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<IReadOnlyList<ContactMessage>> GetMessagesAsync(CancellationToken cancellationToken = default)
    {
        return await _repo.ListAsync(m => !m.IsDeleted, cancellationToken);
    }

    public async Task<ContactMessage?> GetMessageByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repo.GetByIdAsync(id, cancellationToken);
    }

    public async Task SaveMessageAsync(ContactMessage message, CancellationToken cancellationToken = default)
    {
        message.CreatedAt = DateTime.UtcNow;
        message.CreatedBy = "public";
        message.Status = "Unread";
        await _repo.AddAsync(message, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(int id, CancellationToken cancellationToken = default)
    {
        var msg = await _repo.GetByIdAsync(id, cancellationToken);
        if (msg != null)
        {
            msg.Status = "Read";
            msg.UpdatedAt = DateTime.UtcNow;
            msg.UpdatedBy = "admin";
            _repo.Update(msg);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteMessageAsync(int id, CancellationToken cancellationToken = default)
    {
        var msg = await _repo.GetByIdAsync(id, cancellationToken);
        if (msg != null)
        {
            msg.IsDeleted = true;
            msg.UpdatedAt = DateTime.UtcNow;
            msg.UpdatedBy = "admin";
            _repo.Update(msg);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetUnreadCountAsync(CancellationToken cancellationToken = default)
    {
        return await _repo.CountAsync(m => !m.IsDeleted && m.Status == "Unread", cancellationToken);
    }
}

public class EmailTemplateService : IEmailTemplateService
{
    private readonly IEmailTemplateRepository _repo;
    private readonly IUnitOfWork _uow;

    public EmailTemplateService(IEmailTemplateRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<IReadOnlyList<EmailTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default)
    {
        return await _repo.ListAsync(t => !t.IsDeleted, cancellationToken);
    }

    public async Task<EmailTemplate?> GetTemplateByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repo.GetByIdAsync(id, cancellationToken);
    }

    public async Task<EmailTemplate?> GetTemplateByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var templates = await _repo.ListAsync(t => !t.IsDeleted && t.TemplateName == name && t.IsActive, cancellationToken);
        return templates.FirstOrDefault();
    }

    public async Task CreateTemplateAsync(EmailTemplate template, CancellationToken cancellationToken = default)
    {
        template.CreatedAt = DateTime.UtcNow;
        template.CreatedBy = "admin";
        await _repo.AddAsync(template, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateTemplateAsync(EmailTemplate template, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(template.Id, cancellationToken);
        if (existing != null)
        {
            existing.TemplateName = template.TemplateName;
            existing.Subject = template.Subject;
            existing.Body = template.Body;
            existing.Placeholders = template.Placeholders;
            existing.IsActive = template.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteTemplateAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> placeholders, CancellationToken cancellationToken = default)
    {
        var template = await GetTemplateByNameAsync(templateName, cancellationToken);
        if (template == null)
        {
            return string.Empty;
        }

        var body = template.Body;
        if (placeholders != null)
        {
            foreach (var kvp in placeholders)
            {
                body = body.Replace($"{{{kvp.Key}}}", kvp.Value);
            }
        }
        return body;
    }

    public async Task<string> RenderTemplateSubjectAsync(string templateName, Dictionary<string, string> placeholders, CancellationToken cancellationToken = default)
    {
        var template = await GetTemplateByNameAsync(templateName, cancellationToken);
        if (template == null)
        {
            return string.Empty;
        }

        var subject = template.Subject;
        if (placeholders != null)
        {
            foreach (var kvp in placeholders)
            {
                subject = subject.Replace($"{{{kvp.Key}}}", kvp.Value);
            }
        }
        return subject;
    }
}

public class SliderService : ISliderService
{
    private readonly ISliderRepository _repo;
    private readonly IUnitOfWork _uow;

    public SliderService(ISliderRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<IReadOnlyList<Slider>> GetActiveSlidersAsync(CancellationToken cancellationToken = default)
    {
        return await _repo.ListAsync(s => s.IsActive && !s.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Slider>> GetAllSlidersAsync(CancellationToken cancellationToken = default)
    {
        return await _repo.ListAsync(s => !s.IsDeleted, cancellationToken);
    }

    public async Task<Slider?> GetSliderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repo.GetByIdAsync(id, cancellationToken);
    }

    public async Task CreateSliderAsync(Slider slider, CancellationToken cancellationToken = default)
    {
        slider.CreatedAt = DateTime.UtcNow;
        slider.CreatedBy = "admin";
        await _repo.AddAsync(slider, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateSliderAsync(Slider slider, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(slider.Id, cancellationToken);
        if (existing != null)
        {
            existing.Title = slider.Title;
            existing.Subtitle = slider.Subtitle;
            existing.ButtonText = slider.ButtonText;
            existing.ButtonUrl = slider.ButtonUrl;
            if (!string.IsNullOrEmpty(slider.ImagePath)) existing.ImagePath = slider.ImagePath;
            existing.DisplayOrder = slider.DisplayOrder;
            existing.IsActive = slider.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteSliderAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}

public class NoticeService : INoticeService
{
    private readonly INoticeRepository _repo;
    private readonly IUnitOfWork _uow;

    public NoticeService(INoticeRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<IReadOnlyList<Notice>> GetLatestNoticesAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _repo.Query()
            .Where(n => n.IsPublished && !n.IsDeleted)
            .OrderByDescending(n => n.PublishAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Notice> Notices, int TotalCount)> GetPagedNoticesAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _repo.Query().Where(n => !n.IsDeleted);
        
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(n => n.Title.Contains(search) || n.Body.Contains(search));
        }

        var total = await query.CountAsync(cancellationToken);
        
        var notices = await query
            .OrderByDescending(n => n.PublishAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (notices, total);
    }

    public async Task<Notice?> GetNoticeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repo.GetByIdAsync(id, cancellationToken);
    }

    public async Task CreateNoticeAsync(Notice notice, CancellationToken cancellationToken = default)
    {
        notice.CreatedAt = DateTime.UtcNow;
        notice.CreatedBy = "admin";
        await _repo.AddAsync(notice, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateNoticeAsync(Notice notice, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(notice.Id, cancellationToken);
        if (existing != null)
        {
            existing.Title = notice.Title;
            existing.Body = notice.Body;
            existing.AudienceRole = notice.AudienceRole;
            existing.PublishAt = notice.PublishAt;
            existing.IsPublished = notice.IsPublished;
            if (!string.IsNullOrEmpty(notice.AttachmentPath)) existing.AttachmentPath = notice.AttachmentPath;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteNoticeAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}

public class EventService : IEventService
{
    private readonly IEventRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ISchoolSettingRepository _settingRepo;
    private readonly IEmailTemplateRepository _templateRepo;
    private readonly IEventNotificationService? _notificationService;

    public EventService(IEventRepository repo, IUnitOfWork uow,
        ISchoolSettingRepository settingRepo,
        IEmailTemplateRepository templateRepo,
        IEventNotificationService? notificationService = null)
    {
        _repo = repo;
        _uow = uow;
        _settingRepo = settingRepo;
        _templateRepo = templateRepo;
        _notificationService = notificationService;
    }

    public async Task<IReadOnlyList<Event>> GetUpcomingEventsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _repo.Query()
            .Where(e => e.IsPublished && !e.IsDeleted && e.EventDate >= DateTime.UtcNow)
            .OrderBy(e => e.EventDate)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetAllEventsAsync(CancellationToken cancellationToken = default)
    {
        return await _repo.Query()
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Event?> GetEventByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repo.GetByIdAsync(id, cancellationToken);
    }

    public async Task CreateEventAsync(Event ev, CancellationToken cancellationToken = default)
    {
        ev.CreatedAt = DateTime.UtcNow;
        ev.CreatedBy = "admin";
        await _repo.AddAsync(ev, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        await TryNotifyOnPublishAsync(ev.Id, ev.IsPublished, cancellationToken);
    }

    public async Task UpdateEventAsync(Event ev, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(ev.Id, cancellationToken);
        if (existing != null)
        {
            var wasPublished = existing.IsPublished;
            existing.Title = ev.Title;
            existing.Description = ev.Description;
            existing.EventDate = ev.EventDate;
            existing.EventLocation = ev.EventLocation;
            existing.IsUpcoming = ev.IsUpcoming;
            existing.IsPublished = ev.IsPublished;
            if (!string.IsNullOrEmpty(ev.CoverImagePath)) existing.CoverImagePath = ev.CoverImagePath;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);

            if (!wasPublished && existing.IsPublished)
            {
                await TryNotifyOnPublishAsync(existing.Id, true, cancellationToken);
            }
        }
    }

    private async Task TryNotifyOnPublishAsync(int eventId, bool isPublished, CancellationToken ct)
    {
        if (!isPublished || _notificationService == null) return;

        try
        {
            var settings = await _settingRepo.GetCurrentSettingsAsync(ct);
            if (settings == null || !settings.EnableEventEmailNotifications || !settings.SendOnPublish)
                return;

            var notification = await _notificationService.CreateNotificationAsync(eventId,
                EventScope.AllStudents, notifyGuardians: true, notifyStudents: settings.EnableStudentNotifications,
                primaryGuardianOnly: true, ct: ct);

            await _notificationService.QueueNotificationAsync(notification.Id, ct);

            if (settings.SendImmediately)
            {
                await _notificationService.SendNotificationAsync(notification.Id, ct);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EventNotification] Failed to auto-notify event {eventId}: {ex.Message}");
        }
    }

    public async Task DeleteEventAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);

            try
            {
                if (existing.IsPublished && _notificationService != null)
                {
                    var settings = await _settingRepo.GetCurrentSettingsAsync(cancellationToken);
                    if (settings != null && settings.EnableEventEmailNotifications)
                    {
                        var cancelTemplate = await _templateRepo.Query()
                            .Where(t => t.TemplateName == "EventCancelled" && t.IsActive && !t.IsDeleted)
                            .Select(t => t.Id)
                            .FirstOrDefaultAsync(cancellationToken);

                        var notification = await _notificationService.CreateNotificationAsync(id,
                            EventScope.AllStudents,
                            notifyGuardians: true,
                            notifyStudents: settings.EnableStudentNotifications,
                            primaryGuardianOnly: true,
                            templateId: cancelTemplate > 0 ? cancelTemplate : null,
                            ct: cancellationToken);

                        await _notificationService.QueueNotificationAsync(notification.Id, cancellationToken);
                        if (settings.SendImmediately)
                            await _notificationService.SendNotificationAsync(notification.Id, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EventNotification] Failed to notify cancellation for event {id}: {ex.Message}");
            }
        }
    }

    public async Task SubmitForApprovalAsync(int id, CancellationToken cancellationToken = default)
    {
        var ev = await _repo.GetByIdAsync(id, cancellationToken);
        if (ev == null) throw new KeyNotFoundException($"Event {id} not found.");

        var settings = await _settingRepo.GetCurrentSettingsAsync(cancellationToken);
        if (settings != null && settings.EnableEventApprovalWorkflow)
        {
            ev.ApprovalStatus = EventApprovalStatus.PendingApproval;
            ev.UpdatedAt = DateTime.UtcNow;
            ev.UpdatedBy = "admin";
            _repo.Update(ev);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task ApproveEventAsync(int id, int approvedBy, CancellationToken cancellationToken = default)
    {
        var ev = await _repo.GetByIdAsync(id, cancellationToken);
        if (ev == null) throw new KeyNotFoundException($"Event {id} not found.");

        ev.ApprovalStatus = EventApprovalStatus.Approved;
        ev.ApprovedBy = approvedBy;
        ev.ApprovedAt = DateTime.UtcNow;
        ev.IsPublished = true;
        ev.UpdatedAt = DateTime.UtcNow;
        ev.UpdatedBy = "admin";
        _repo.Update(ev);
        await _uow.SaveChangesAsync(cancellationToken);

        await TryNotifyOnPublishAsync(ev.Id, true, cancellationToken);
    }

    public async Task RejectEventAsync(int id, string reason, CancellationToken cancellationToken = default)
    {
        var ev = await _repo.GetByIdAsync(id, cancellationToken);
        if (ev == null) throw new KeyNotFoundException($"Event {id} not found.");

        ev.ApprovalStatus = EventApprovalStatus.Rejected;
        ev.RejectionReason = reason;
        ev.IsPublished = false;
        ev.UpdatedAt = DateTime.UtcNow;
        ev.UpdatedBy = "admin";
        _repo.Update(ev);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

public class GalleryService : IGalleryService
{
    private readonly IGalleryRepository _repo;
    private readonly IGalleryImageRepository _imageRepo;
    private readonly IUnitOfWork _uow;

    public GalleryService(IGalleryRepository repo, IGalleryImageRepository imageRepo, IUnitOfWork uow)
    {
        _repo = repo;
        _imageRepo = imageRepo;
        _uow = uow;
    }

    public async Task<IReadOnlyList<Gallery>> GetAllAlbumsAsync(CancellationToken cancellationToken = default)
    {
        return await _repo.ListAsync(g => !g.IsDeleted, cancellationToken);
    }

    public async Task<Gallery?> GetAlbumWithImagesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repo.GetWithImagesAsync(id, cancellationToken);
    }

    public async Task<Gallery?> GetAlbumByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repo.GetByIdAsync(id, cancellationToken);
    }

    public async Task CreateAlbumAsync(Gallery album, CancellationToken cancellationToken = default)
    {
        album.CreatedAt = DateTime.UtcNow;
        album.CreatedBy = "admin";
        await _repo.AddAsync(album, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAlbumAsync(Gallery album, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(album.Id, cancellationToken);
        if (existing != null)
        {
            existing.AlbumName = album.AlbumName;
            existing.Description = album.Description;
            if (!string.IsNullOrEmpty(album.CoverImagePath)) existing.CoverImagePath = album.CoverImagePath;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAlbumAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task AddImageToAlbumAsync(GalleryImage image, CancellationToken cancellationToken = default)
    {
        image.CreatedAt = DateTime.UtcNow;
        image.CreatedBy = "admin";
        await _imageRepo.AddAsync(image, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteImageAsync(int imageId, CancellationToken cancellationToken = default)
    {
        var existing = await _imageRepo.GetByIdAsync(imageId, cancellationToken);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _imageRepo.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}

public class WebsitePageService : IWebsitePageService
{
    private readonly IWebsitePageRepository _repo;
    private readonly IUnitOfWork _uow;

    public WebsitePageService(IWebsitePageRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<WebsitePage?> GetPageBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _repo.GetBySlugAsync(slug, cancellationToken);
    }

    public async Task<IReadOnlyList<WebsitePage>> GetAllPagesAsync(CancellationToken cancellationToken = default)
    {
        return await _repo.ListAsync(p => !p.IsDeleted, cancellationToken);
    }

    public async Task<WebsitePage?> GetPageByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repo.GetByIdAsync(id, cancellationToken);
    }

    public async Task CreatePageAsync(WebsitePage page, CancellationToken cancellationToken = default)
    {
        page.Slug = page.Slug.ToLowerInvariant().Trim();
        page.CreatedAt = DateTime.UtcNow;
        page.CreatedBy = "admin";
        await _repo.AddAsync(page, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePageAsync(WebsitePage page, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(page.Id, cancellationToken);
        if (existing != null)
        {
            existing.Title = page.Title;
            existing.Slug = page.Slug.ToLowerInvariant().Trim();
            existing.Content = page.Content;
            existing.MetaTitle = page.MetaTitle;
            existing.MetaDescription = page.MetaDescription;
            existing.IsPublished = page.IsPublished;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeletePageAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}

public class AdmissionFeeStructureService : IAdmissionFeeStructureService
{
    private readonly IUnitOfWork _uow;

    public AdmissionFeeStructureService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IReadOnlyList<AdmissionFeeStructure>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _uow.Repository<AdmissionFeeStructure>()
            .ListAsync(f => !f.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<AdmissionFeeStructure>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _uow.Repository<AdmissionFeeStructure>()
            .ListAsync(f => f.IsActive && !f.IsDeleted, cancellationToken);
    }

    public async Task<AdmissionFeeStructure?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _uow.Repository<AdmissionFeeStructure>().GetByIdAsync(id, cancellationToken);
    }

    public async Task CreateAsync(AdmissionFeeStructure fee, CancellationToken cancellationToken = default)
    {
        fee.CreatedAt = DateTime.UtcNow;
        fee.CreatedBy = "admin";
        await _uow.Repository<AdmissionFeeStructure>().AddAsync(fee, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AdmissionFeeStructure fee, CancellationToken cancellationToken = default)
    {
        var existing = await _uow.Repository<AdmissionFeeStructure>().GetByIdAsync(fee.Id, cancellationToken);
        if (existing != null)
        {
            existing.SchoolClassId = fee.SchoolClassId;
            existing.ClassName = fee.ClassName;
            existing.AdmissionFee = fee.AdmissionFee;
            existing.MonthlyFee = fee.MonthlyFee;
            existing.SessionFee = fee.SessionFee;
            existing.ExamFee = fee.ExamFee;
            existing.OtherFee = fee.OtherFee;
            existing.DisplayOrder = fee.DisplayOrder;
            existing.IsActive = fee.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _uow.Repository<AdmissionFeeStructure>().Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _uow.Repository<AdmissionFeeStructure>().GetByIdAsync(id, cancellationToken);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _uow.Repository<AdmissionFeeStructure>().Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}

public class AnnouncementService : IAnnouncementService
{
    private readonly IUnitOfWork _uow;

    public AnnouncementService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IReadOnlyList<Announcement>> GetActiveAnnouncementsAsync(CancellationToken cancellationToken = default)
    {
        return await _uow.Repository<Announcement>()
            .ListAsync(a => a.IsActive && !a.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Announcement>> GetAllAnnouncementsAsync(CancellationToken cancellationToken = default)
    {
        return await _uow.Repository<Announcement>()
            .ListAsync(a => !a.IsDeleted, cancellationToken);
    }

    public async Task<Announcement?> GetAnnouncementByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _uow.Repository<Announcement>().GetByIdAsync(id, cancellationToken);
    }

    public async Task CreateAnnouncementAsync(Announcement announcement, CancellationToken cancellationToken = default)
    {
        announcement.CreatedAt = DateTime.UtcNow;
        announcement.CreatedBy = "admin";
        await _uow.Repository<Announcement>().AddAsync(announcement, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAnnouncementAsync(Announcement announcement, CancellationToken cancellationToken = default)
    {
        var existing = await _uow.Repository<Announcement>().GetByIdAsync(announcement.Id, cancellationToken);
        if (existing != null)
        {
            existing.Title = announcement.Title;
            existing.Content = announcement.Content;
            existing.IsActive = announcement.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _uow.Repository<Announcement>().Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAnnouncementAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _uow.Repository<Announcement>().GetByIdAsync(id, cancellationToken);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _uow.Repository<Announcement>().Update(existing);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
