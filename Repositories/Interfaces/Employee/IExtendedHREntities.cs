using SchoolManagementSystem.Models.Entities.Employee;

namespace SchoolManagementSystem.Repositories.Interfaces.Employee;

public interface IHolidayRepository : IBaseRepository<Holiday>
{
    Task<IEnumerable<Holiday>> GetUpcomingHolidaysAsync(int count, CancellationToken ct = default);
}

public interface IEmployeeDocumentRepository : IBaseRepository<EmployeeDocument>
{
    Task<IEnumerable<EmployeeDocument>> GetByEmployeeIdAsync(long employeeId, CancellationToken ct = default);
}

public interface INotificationRepository : IBaseRepository<SchoolManagementSystem.Models.Entities.Auth.Notification>
{
    Task<IEnumerable<SchoolManagementSystem.Models.Entities.Auth.Notification>> GetUnreadByUserIdAsync(long userId, int count, CancellationToken ct = default);
    Task MarkAsReadAsync(long notificationId, CancellationToken ct = default);
}
