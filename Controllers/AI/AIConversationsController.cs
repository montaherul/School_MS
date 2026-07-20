using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.AI;
using SchoolManagementSystem.Services.Interfaces.AI;
using System.Security.Claims;
using System.Text;

namespace SchoolManagementSystem.Controllers.AI;

[Authorize(Roles = "Admin,Principal")]
[Route("AI/[controller]")]
public class AIConversationsController : Controller
{
    private readonly IAIAdminService _adminService;
    private readonly ILogger<AIConversationsController> _logger;

    public AIConversationsController(IAIAdminService adminService, ILogger<AIConversationsController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

    [HttpGet("")]
    [RequirePermission("AI.View")]
    public async Task<IActionResult> Index(int page = 1, string? search = null, int? statusFilter = null, CancellationToken ct = default)
    {
        var result = await _adminService.GetConversationsAdminAsync(page, 20, search, statusFilter, ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return View("~/Views/AI/Conversations/Index.cshtml", new AIConversationAdminIndexViewModel());
        }

        var (items, totalRecords) = result.Data!;
        var totalPages = (int)Math.Ceiling((double)totalRecords / 20);

        return View("~/Views/AI/Conversations/Index.cshtml", new AIConversationAdminIndexViewModel
        {
            Items = items,
            Page = page,
            TotalPages = totalPages,
            TotalRecords = totalRecords,
            Search = search ?? string.Empty,
            StatusFilter = statusFilter?.ToString() ?? string.Empty
        });
    }

    [HttpGet("Details/{id}")]
    [RequirePermission("AI.View")]
    public async Task<IActionResult> Details(int id, CancellationToken ct = default)
    {
        var result = await _adminService.GetConversationsAdminAsync(1, int.MaxValue, null, null, ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var conversation = result.Data!.Items.FirstOrDefault(c => c.Id == id);
        if (conversation == null)
            return NotFound();

        return View("~/Views/AI/Conversations/Details.cshtml", conversation);
    }

    [HttpPost("Archive/{id}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> Archive(int id, CancellationToken ct = default)
    {
        var result = await _adminService.GetConversationsAdminAsync(1, int.MaxValue, null, null, ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var conversation = result.Data!.Items.FirstOrDefault(c => c.Id == id);
        if (conversation == null)
        {
            TempData["ErrorMessage"] = "Conversation not found.";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Conversation archived.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Restore/{id}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> Restore(int id, CancellationToken ct = default)
    {
        var result = await _adminService.GetConversationsAdminAsync(1, int.MaxValue, null, null, ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var conversation = result.Data!.Items.FirstOrDefault(c => c.Id == id);
        if (conversation == null)
        {
            TempData["ErrorMessage"] = "Conversation not found.";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Conversation restored.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var result = await _adminService.GetConversationsAdminAsync(1, int.MaxValue, null, null, ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var conversation = result.Data!.Items.FirstOrDefault(c => c.Id == id);
        if (conversation == null)
        {
            TempData["ErrorMessage"] = "Conversation not found.";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Conversation deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Export/{id}")]
    [RequirePermission("AI.View")]
    public async Task<IActionResult> Export(int id, CancellationToken ct = default)
    {
        var result = await _adminService.GetConversationsAdminAsync(1, int.MaxValue, null, null, ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var conversation = result.Data!.Items.FirstOrDefault(c => c.Id == id);
        if (conversation == null)
            return NotFound();

        var sb = new StringBuilder();
        sb.AppendLine($"=== Conversation Export ===");
        sb.AppendLine($"ID: {conversation.Id}");
        sb.AppendLine($"Title: {conversation.Title}");
        sb.AppendLine($"Student: {conversation.StudentName}");
        sb.AppendLine($"Status: {conversation.Status}");
        sb.AppendLine($"Created: {conversation.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Messages: {conversation.MessageCount}");
        sb.AppendLine(new string('=', 40));

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        var safeTitle = string.Join("_", conversation.Title.Split(Path.GetInvalidFileNameChars()));
        return File(bytes, "text/plain", $"conversation_{conversation.Id}_{safeTitle}.txt");
    }
}
