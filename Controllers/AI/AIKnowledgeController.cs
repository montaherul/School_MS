using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.ViewModels.AI;
using SchoolManagementSystem.Services.Interfaces.AI;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.AI;

[Authorize(Roles = "Admin,Principal")]
[Route("AI/[controller]")]
public class AIKnowledgeController : Controller
{
    private readonly IAIAdminService _adminService;
    private readonly ILogger<AIKnowledgeController> _logger;

    public AIKnowledgeController(IAIAdminService adminService, ILogger<AIKnowledgeController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

    [HttpGet("")]
    [RequirePermission("AI.View")]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var result = await _adminService.GetKnowledgeBasesAsync(ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return View("~/Views/AI/Knowledge/Index.cshtml", new AIKnowledgeIndexViewModel());
        }

        return View("~/Views/AI/Knowledge/Index.cshtml", new AIKnowledgeIndexViewModel
        {
            Items = result.Data!
        });
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var result = await _adminService.DeleteKnowledgeBaseAsync(id, ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Knowledge base deleted.";
        return RedirectToAction(nameof(Index));
    }
}
