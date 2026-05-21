using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.Website;

namespace SchoolManagementSystem.Controllers.Website;

public class NoticeController : Controller
{
    private readonly INoticeService _noticeService;

    public NoticeController(INoticeService noticeService)
    {
        _noticeService = noticeService;
    }

    [HttpGet("/notices")]
    public async Task<IActionResult> Index(string? search, int page = 1, CancellationToken ct = default)
    {
        int pageSize = 6;
        var (notices, totalCount) = await _noticeService.GetPagedNoticesAsync(search, page, pageSize, ct);
        
        ViewBag.Search = search;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)System.Math.Ceiling((double)totalCount / pageSize);
        ViewBag.TotalCount = totalCount;

        return View(notices);
    }

    [HttpGet("/notice/details/{id}")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var notice = await _noticeService.GetNoticeByIdAsync(id, ct);
        if (notice == null || notice.IsDeleted || !notice.IsPublished)
        {
            return NotFound();
        }
        return View(notice);
    }
}
