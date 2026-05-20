using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.Website;

namespace SchoolManagementSystem.Controllers.Website;

public class PageController : Controller
{
    private readonly IWebsitePageService _pageService;

    public PageController(IWebsitePageService pageService)
    {
        _pageService = pageService;
    }

    [HttpGet("/p/{slug}")]
    public async Task<IActionResult> Details(string slug, CancellationToken ct)
    {
        var page = await _pageService.GetPageBySlugAsync(slug, ct);
        if (page == null)
        {
            return NotFound();
        }
        return View(page);
    }
}
