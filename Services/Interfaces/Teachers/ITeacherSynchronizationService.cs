using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Interfaces.Teachers;

public interface ITeacherSynchronizationService
{
    Task SyncEmployeeToTeacherAsync(int employeeId, CancellationToken ct = default);
    Task SyncAllTeachingEmployeesAsync(CancellationToken ct = default);
}
