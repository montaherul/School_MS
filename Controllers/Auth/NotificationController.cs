using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.Auth;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Auth;

[Authorize]
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var notifications = await _notificationService.GetRecentUnreadAsync(userId, 50);
        return View(notifications);
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(long id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var unread = await _notificationService.GetRecentUnreadAsync(userId, 100);
        foreach (var n in unread)
        {
            await _notificationService.MarkAsReadAsync(n.Id);
        }
        return RedirectToAction(nameof(Index));
    }
}
