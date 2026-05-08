using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;

namespace SchoolManagementSystem.Services.Implementations.Attendance;

public class AttendanceRecordService : IAttendanceRecordService
{
    private readonly SchoolDbContext _db;

    public AttendanceRecordService(SchoolDbContext db) { _db = db; }

    public async Task<PagedResult<AttendanceRecordListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? studentId = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);

        var items = new List<AttendanceRecordListItemDto>();
        int totalCount = 0;

        using (var command = _db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetAttendanceList";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageNumber", page));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageSize", pageSize));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@StudentId", (object?)studentId ?? 0));

            await _db.Database.OpenConnectionAsync(cancellationToken);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    items.Add(new AttendanceRecordListItemDto
                    {
                        Id = reader.GetInt32(0),
                        StudentId = reader.GetInt32(1),
                        StudentName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        SchoolClassId = reader.GetInt32(3),
                        ClassName = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        SectionId = reader.GetInt32(5),
                        SectionName = reader.IsDBNull(6) ? "" : reader.GetString(6),
                        Status = (SchoolManagementSystem.Models.Enums.AttendanceStatus)reader.GetInt32(7),
                        Remarks = reader.IsDBNull(8) ? "" : reader.GetString(8),
                        TotalRecords = reader.IsDBNull(10) ? 0 : reader.GetInt32(10)
                    });
                }
            }
            await _db.Database.CloseConnectionAsync();
        }

        totalCount = items.FirstOrDefault()?.TotalRecords ?? 0;

        return new PagedResult<AttendanceRecordListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<AttendanceRecordUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Attendance.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new AttendanceRecordUpsertDto { Id = entity.Id,StudentId = entity.StudentId,SchoolClassId = entity.SchoolClassId,SectionId = entity.SectionId,Status = entity.Status,Remarks = entity.Remarks ?? string.Empty,        };
    }

    public async Task<int> CreateAsync(AttendanceRecordUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new AttendanceRecord { CreatedBy = createdBy,StudentId = dto.StudentId,SchoolClassId = dto.SchoolClassId,SectionId = dto.SectionId,Status = dto.Status,Remarks = dto.Remarks,        };
        _db.Attendance.Add(entity); await _db.SaveChangesAsync(cancellationToken); return entity.Id;
    }

    public async Task UpdateAsync(AttendanceRecordUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Attendance.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("AttendanceRecord not found.");
        entity.StudentId = dto.StudentId;
        entity.SchoolClassId = dto.SchoolClassId;
        entity.SectionId = dto.SectionId;
        entity.Status = dto.Status;
        entity.Remarks = dto.Remarks;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Attendance.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("AttendanceRecord not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }
}

