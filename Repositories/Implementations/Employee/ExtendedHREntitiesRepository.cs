using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Repositories.Interfaces.Employee;

namespace SchoolManagementSystem.Repositories.Implementations.Employee;

public class HolidayRepository : BaseRepository<Holiday>, IHolidayRepository
{
    public HolidayRepository(SchoolDbContext db) : base(db) { }

    public async Task<IEnumerable<Holiday>> GetUpcomingHolidaysAsync(int count, CancellationToken ct = default)
    {
        return await _db.Holidays
            .Where(h => h.EndDate >= DateTime.Today)
            .OrderBy(h => h.StartDate)
            .Take(count)
            .ToListAsync(ct);
    }
}

public class EmployeeDocumentRepository : BaseRepository<EmployeeDocument>, IEmployeeDocumentRepository
{
    public EmployeeDocumentRepository(SchoolDbContext db) : base(db) { }

    public async Task<IEnumerable<EmployeeDocument>> GetByEmployeeIdAsync(long employeeId, CancellationToken ct = default)
    {
        return await _db.EmployeeDocuments
            .Include(d => d.UploadedBy)
            .Where(d => d.EmployeeId == employeeId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync(ct);
    }
}

public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    public NotificationRepository(SchoolDbContext db) : base(db) { }

    public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(long userId, int count, CancellationToken ct = default)
    {
        return await _db.AppNotifications
            .Where(n => n.UserId == (int)userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync(ct);
    }

    public async Task MarkAsReadAsync(long notificationId, CancellationToken ct = default)
    {
        var n = await _db.AppNotifications.FindAsync(new object[] { notificationId }, ct);
        if (n != null)
        {
            n.IsRead = true;
            _db.AppNotifications.Update(n);
        }
    }
}
