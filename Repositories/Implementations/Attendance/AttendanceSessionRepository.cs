using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;

namespace SchoolManagementSystem.Repositories.Implementations.Attendance
{
    public class AttendanceSessionRepository : BaseRepository<AttendanceSession>, IAttendanceSessionRepository
    {
        public AttendanceSessionRepository(SchoolDbContext context) : base(context) { }

        public async Task<AttendanceSession?> GetSessionAsync(int classId, int sectionId, DateOnly date, int? groupId = null, CancellationToken ct = default)
        {
            var q = _set.AsNoTracking().Where(s => s.SchoolClassId == classId && s.SectionId == sectionId && s.AttendanceDate == date && !s.IsDeleted);
            if (groupId.HasValue) q = q.Where(s => s.StudentGroupId == groupId.Value);
            return await q.FirstOrDefaultAsync(ct);
        }

        public async Task<bool> IsLockedAsync(int classId, int sectionId, DateOnly date, int? groupId = null, CancellationToken ct = default)
        {
            var q = _set.AsNoTracking().Where(s => s.SchoolClassId == classId && s.SectionId == sectionId && s.AttendanceDate == date && !s.IsDeleted);
            if (groupId.HasValue) q = q.Where(s => s.StudentGroupId == groupId.Value);
            return await q.AnyAsync(ct);
        }
    }
}
