using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;

namespace SchoolManagementSystem.Repositories.Implementations.Attendance;

public class AttendanceRepository : BaseRepository<AttendanceRecord>, IAttendanceRepository
{
    public AttendanceRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<AttendanceRecordListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm,
        int studentId, int classId, int sectionId, DateOnly? attendanceDate,
        CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetAttendanceList";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@PageNumber",     pageNumber));
        command.Parameters.Add(new SqlParameter("@PageSize",       pageSize));
        command.Parameters.Add(new SqlParameter("@SearchTerm",     (object?)searchTerm   ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@StudentId",      studentId));
        command.Parameters.Add(new SqlParameter("@ClassId",        classId));
        command.Parameters.Add(new SqlParameter("@SectionId",      sectionId));
        command.Parameters.Add(new SqlParameter("@AttendanceDate",
            attendanceDate.HasValue ? (object)attendanceDate.Value.ToDateTime(TimeOnly.MinValue).Date : DBNull.Value));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            var items = new List<AttendanceRecordListItemDto>();

            while (await reader.ReadAsync(ct))
            {
                items.Add(new AttendanceRecordListItemDto
                {
                    Id             = reader.GetInt32(0),
                    StudentId      = reader.GetInt32(1),
                    StudentName    = reader.IsDBNull(2)  ? "" : reader.GetString(2),
                    SchoolClassId  = reader.GetInt32(3),
                    ClassName      = reader.IsDBNull(4)  ? "" : reader.GetString(4),
                    SectionId      = reader.GetInt32(5),
                    SectionName    = reader.IsDBNull(6)  ? "" : reader.GetString(6),
                    Status         = (SchoolManagementSystem.Models.Enums.AttendanceStatus)reader.GetInt32(7),
                    Remarks        = reader.IsDBNull(8)  ? "" : reader.GetString(8),
                    AttendanceDate = reader.IsDBNull(9)  ? DateOnly.MinValue : DateOnly.FromDateTime(reader.GetDateTime(9)),
                    TotalRecords   = reader.IsDBNull(10) ? 0  : reader.GetInt32(10)
                });
            }

            return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }
}
