using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.ViewModels.Admin;
using SchoolManagementSystem.Services.Interfaces.Admin;

namespace SchoolManagementSystem.Controllers.Admin;

[Authorize]
public class AuditLogController : Controller
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [RequirePermission("AuditLogs.View")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 15, string? search = null, CancellationToken ct = default)
    {
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            var result = await _auditLogService.GetPagedAsync(page, pageSize, search, ct);
            var lastPage = (int)Math.Ceiling((double)result.TotalItems / pageSize);
            return Json(new { data = result.Items, last_page = lastPage });
        }
        var viewResult = await _auditLogService.GetPagedAsync(page, 20, search, ct);
        var model = new AuditLogIndexViewModel
        {
            Items = viewResult.Items.ToList(),
            Page = viewResult.Page,
            PageSize = viewResult.PageSize,
            TotalItems = viewResult.TotalItems,
            Search = search
        };
        return View(model);
    }
}
