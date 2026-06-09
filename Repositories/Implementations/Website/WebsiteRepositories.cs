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

public class GalleryRepository : BaseRepository<Gallery>, IGalleryRepository
{
    public GalleryRepository(SchoolDbContext context) : base(context)
    {
    }

    public async Task<Gallery?> GetWithImagesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _set
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
