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
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string? search = null, CancellationToken ct = default)
    {
        var result = await _auditLogService.GetPagedAsync(page, pageSize, search, ct);
        var model = new AuditLogIndexViewModel
        {
            Items = result.Items.ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            Search = search
        };
        return View(model);
    }
}
