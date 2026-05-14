using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Services.Interfaces.Auth;

public interface INotificationService
{
    Task SendNotificationAsync(int userId, string title, string message, NotificationType type, string? redirectUrl = null, CancellationToken ct = default);
    Task MarkAsReadAsync(long notificationId, CancellationToken ct = default);
    Task<IEnumerable<Notification>> GetRecentUnreadAsync(int userId, int count = 5, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default);
}
