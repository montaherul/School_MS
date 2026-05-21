using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Interfaces.Attendance;

public interface IAttendanceAuthorizationService
{
    Task<bool> IsAuthorizedToMarkAttendanceAsync(int teacherId, int classId, int sectionId, int academicYearId, CancellationToken ct = default);
}