using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.Website;

namespace SchoolManagementSystem.Controllers.Website;

public class GalleryController : Controller
{
    private readonly IGalleryService _galleryService;

    public GalleryController(IGalleryService galleryService)
    {
        _galleryService = galleryService;
    }

    [HttpGet("/gallery")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var albums = await _galleryService.GetAllAlbumsAsync(ct);
        return View(albums);
    }

    [HttpGet("/gallery/album/{id}")]
    public async Task<IActionResult> Album(int id, CancellationToken ct)
    {
        var album = await _galleryService.GetAlbumWithImagesAsync(id, ct);
        if (album == null || album.IsDeleted)
        {
            return NotFound();
        }
        return View(album);
    }
}
