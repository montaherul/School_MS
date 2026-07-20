using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.ViewModels.AI;
using SchoolManagementSystem.Services.Interfaces.AI;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.AI;

[Authorize(Roles = "Admin,Principal")]
[Route("AI/[controller]")]
public class AIHealthController : Controller
{
    private readonly IAIAdminService _adminService;
    private readonly ILogger<AIHealthController> _logger;

    public AIHealthController(IAIAdminService adminService, ILogger<AIHealthController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

    [HttpGet("")]
    [RequirePermission("AI.View")]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var result = await _adminService.GetLatestHealthChecksAsync(ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return View("~/Views/AI/Health/Index.cshtml", new AIHealthIndexViewModel());
        }

        return View("~/Views/AI/Health/Index.cshtml", new AIHealthIndexViewModel
        {
            Checks = result.Data!
        });
    }

    [HttpPost("Refresh")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> Refresh(CancellationToken ct = default)
    {
        var result = await _adminService.GetLatestHealthChecksAsync(ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Health checks refreshed.";
        return RedirectToAction(nameof(Index));
    }
}
