using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Website;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Website;

public class SchoolWebsiteService : ISchoolWebsiteService
{
    private readonly ISchoolSettingRepository _repo;
    private readonly IUnitOfWork _uow;

    public SchoolWebsiteService(ISchoolSettingRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<SchoolSetting> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
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
            existing.EIIN = settings.EIIN;
            existing.Address = settings.Address;
            existing.Phone = settings.Phone;
            existing.Email = settings.Email;
            existing.Website = settings.Website;
            existing.FacebookUrl = settings.FacebookUrl;
            existing.YouTubeUrl = settings.YouTubeUrl;
            if (!string.IsNullOrEmpty(settings.LogoPath)) existing.LogoPath = settings.LogoPath;
            if (!string.IsNullOrEmpty(settings.FaviconPath)) existing.FaviconPath = settings.FaviconPath;
            existing.PrincipalName = settings.PrincipalName;
            existing.PrincipalMessage = settings.PrincipalMessage;
            if (!string.IsNullOrEmpty(settings.PrincipalImagePath)) existing.PrincipalImagePath = settings.PrincipalImagePath;
            existing.Mission = settings.Mission;
            existing.Vision = settings.Vision;
            existing.FooterText = settings.FooterText;
            existing.GoogleMapEmbed = settings.GoogleMapEmbed;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = "admin";
            _repo.Update(existing);
        }
        await _uow.SaveChangesAsync(cancellationToken);
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

    public EventService(IEventRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
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
    }

    public async Task UpdateEventAsync(Event ev, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(ev.Id, cancellationToken);
        if (existing != null)
        {
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
        }
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
