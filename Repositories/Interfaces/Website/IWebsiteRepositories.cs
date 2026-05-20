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
