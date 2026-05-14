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
        if (User?.Identity == null || !User.Identity.IsAuthenticated)
        {
            return View(new List<SchoolManagementSystem.Models.Entities.Auth.Notification>());
        }

        try
        {
            var userIdStr = (User as ClaimsPrincipal)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out var userId))
            {
                var unreadCount = await _notificationService.GetUnreadCountAsync(userId);
                var recentNotifications = await _notificationService.GetRecentUnreadAsync(userId);
                
                ViewBag.UnreadCount = unreadCount;
                return View(recentNotifications);
            }
        }
        catch
        {
            // Fail gracefully
        }
        
        return View(new List<SchoolManagementSystem.Models.Entities.Auth.Notification>());
    }
}
