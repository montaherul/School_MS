using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.ViewModels.AI;
using SchoolManagementSystem.Services.Interfaces.AI;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.AI;

[Authorize(Roles = "Admin,Principal")]
[Route("AI/[controller]")]
public class AIAuditController : Controller
{
    private readonly IAIAdminService _adminService;
    private readonly ILogger<AIAuditController> _logger;

    public AIAuditController(IAIAdminService adminService, ILogger<AIAuditController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

    [HttpGet("")]
    [RequirePermission("AI.View")]
    public async Task<IActionResult> Index(int page = 1, string? entityType = null, CancellationToken ct = default)
    {
        var result = await _adminService.GetAuditLogsPagedAsync(page, 20, entityType, ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return View("~/Views/AI/Audit/Index.cshtml", new AIAuditLogIndexViewModel());
        }

        var (items, totalRecords) = result.Data!;
        var totalPages = (int)Math.Ceiling((double)totalRecords / 20);

        return View("~/Views/AI/Audit/Index.cshtml", new AIAuditLogIndexViewModel
        {
            Items = items,
            Page = page,
            TotalPages = totalPages,
            TotalRecords = totalRecords
        });
    }
}
