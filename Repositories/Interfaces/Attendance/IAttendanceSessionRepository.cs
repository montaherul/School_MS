using System.Threading;
using System.Threading.Tasks;
using SchoolManagementSystem.Models.Entities.Attendance;

namespace SchoolManagementSystem.Repositories.Interfaces.Attendance
{
    public interface IAttendanceSessionRepository : IBaseRepository<AttendanceSession>
    {
        Task<AttendanceSession?> GetSessionAsync(int classId, int sectionId, DateOnly date, int? groupId = null, CancellationToken ct = default);
        Task<bool> IsLockedAsync(int classId, int sectionId, DateOnly date, int? groupId = null, CancellationToken ct = default);
    }
}
