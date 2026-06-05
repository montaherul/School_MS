using System.Data;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
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
            else q = q.Where(s => s.StudentGroupId == null);
            return await q.FirstOrDefaultAsync(ct);
        }

        public async Task<bool> IsLockedAsync(int classId, int sectionId, DateOnly date, int? groupId = null, CancellationToken ct = default)
        {
            var q = _set.AsNoTracking().Where(s => s.SchoolClassId == classId && s.SectionId == sectionId && s.AttendanceDate == date && !s.IsDeleted);
            if (groupId.HasValue) q = q.Where(s => s.StudentGroupId == groupId.Value);
            else q = q.Where(s => s.StudentGroupId == null);

            return await q.AnyAsync(s =>
                s.Status == SchoolManagementSystem.Models.Enums.AttendanceSessionStatus.Locked ||
                s.Status == SchoolManagementSystem.Models.Enums.AttendanceSessionStatus.Approved, ct);
        }

        public async Task<(List<AttendanceSession> Items, int TotalRecords)> GetListByStoredProcedureAsync(
            int pageNumber, int pageSize, string? searchTerm,
            int classId, int sectionId, int studentGroupId, int status,
            DateOnly? attendanceDate, CancellationToken ct = default)
        {
            var items = new List<AttendanceSession>();
            int totalRecords = 0;

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "sp_GetAttendanceSessions";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
                command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
                command.Parameters.Add(new SqlParameter("@SearchTerm", (object?)searchTerm ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@ClassId", classId));
                command.Parameters.Add(new SqlParameter("@SectionId", sectionId));
                command.Parameters.Add(new SqlParameter("@StudentGroupId", studentGroupId));
                command.Parameters.Add(new SqlParameter("@Status", status));
                command.Parameters.Add(new SqlParameter("@AttendanceDate", (object?)attendanceDate?.ToDateTime(TimeOnly.MinValue).Date ?? DBNull.Value));

                if (command.Connection!.State != ConnectionState.Open)
                    await _db.Database.OpenConnectionAsync(ct);

                using (var reader = await command.ExecuteReaderAsync(ct))
                {
                    while (await reader.ReadAsync(ct))
                    {
                        var session = new AttendanceSession
                        {
                            Id = reader.GetInt32(0),
                            AttendanceDate = DateOnly.FromDateTime(reader.GetDateTime(1)),
                            SchoolClassId = reader.GetInt32(2),
                            SectionId = reader.GetInt32(4),
                            StudentGroupId = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                            Status = (SchoolManagementSystem.Models.Enums.AttendanceSessionStatus)reader.GetInt32(8),
                            CreatedBy = reader.IsDBNull(9) ? "" : reader.GetString(9),
                            CreatedAt = reader.GetDateTime(10),
                            LockedBy = reader.IsDBNull(11) ? null : reader.GetString(11),
                            LockedAt = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                            UpdatedAt = reader.IsDBNull(13) ? null : reader.GetDateTime(13)
                        };
                        items.Add(session);
                        totalRecords = reader.GetInt32(reader.FieldCount - 1);
                    }
                }
            }

            return (items, totalRecords);
        }
    }
}
