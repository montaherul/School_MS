using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using SchoolManagementSystem.Services.Interfaces.Auth;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Auth;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;
    private readonly IUnitOfWork _uow;

    public NotificationService(INotificationRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task SendNotificationAsync(int userId, string title, string message, NotificationType type, string? redirectUrl = null, CancellationToken ct = default)
    {
        var n = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            RedirectUrl = redirectUrl,
            IsRead = false
        };
        await _repo.AddAsync(n, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task MarkAsReadAsync(long notificationId, CancellationToken ct = default)
    {
        await _repo.MarkAsReadAsync(notificationId, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Notification>> GetRecentUnreadAsync(int userId, int count = 5, CancellationToken ct = default)
    {
        return await _repo.GetUnreadByUserIdAsync(userId, count, ct);
    }

    public async Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default)
    {
        return await _repo.Query().CountAsync(n => n.UserId == userId && !n.IsRead, ct);
    }
}
