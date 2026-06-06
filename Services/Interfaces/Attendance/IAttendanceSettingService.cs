using SchoolManagementSystem.Models.Entities.Attendance;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Interfaces.Attendance
{
    public interface IAttendanceSettingService
    {
        Task<AttendanceSetting?> GetCurrentAsync(CancellationToken ct = default);
        Task<AttendanceSetting> GetOrCreateDefaultAsync(CancellationToken ct = default);
        Task UpdateAsync(AttendanceSetting setting, string updatedBy, CancellationToken ct = default);
    }
}
