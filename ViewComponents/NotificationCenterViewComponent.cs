using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.Auth;
using System.Security.Claims;

namespace SchoolManagementSystem.ViewComponents;

public class NotificationCenterViewComponent : ViewComponent
{
    private readonly INotificationService _notificationService;

    public NotificationCenterViewComponent(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userIdStr = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdStr, out var userId))
        {
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId);
            var recentNotifications = await _notificationService.GetRecentUnreadAsync(userId);
            
            ViewBag.UnreadCount = unreadCount;
            return View(recentNotifications);
        }
        return View(new List<SchoolManagementSystem.Models.Entities.Auth.Notification>());
    }
}
