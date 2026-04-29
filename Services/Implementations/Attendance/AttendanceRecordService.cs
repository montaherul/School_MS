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

    public async Task<PagedResult<AttendanceRecordListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 5, 100); var term = search?.Trim();
        var query = _db.Attendance.Where(x => !x.IsDeleted);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(x => new AttendanceRecordListItemDto {
            Id = x.Id,StudentId = x.StudentId,SchoolClassId = x.SchoolClassId,SectionId = x.SectionId,Status = x.Status,Remarks = x.Remarks ?? string.Empty,        }).ToListAsync(cancellationToken);
        return new PagedResult<AttendanceRecordListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
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

