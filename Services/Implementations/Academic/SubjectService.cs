using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SubjectService : ISubjectService
{
    private readonly SchoolDbContext _db;

    public SubjectService(SchoolDbContext db) { _db = db; }

    public async Task<PagedResult<SubjectListItemDto>> GetPagedAsync(
       int page,
       int pageSize,
       string? search,
       CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);

        var items = new List<SubjectListItemDto>();
        int totalCount = 0;

        using (var command = _db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetSubjectList";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageNumber", page));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageSize", pageSize));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));

            await _db.Database.OpenConnectionAsync(cancellationToken);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    items.Add(new SubjectListItemDto
                    {
                        Id = reader.GetInt32(0),
                        Code = reader.GetString(1),
                        Name = reader.GetString(2),
                        IsReligionSubject = reader.GetBoolean(4),
                        ReligionType = reader.IsDBNull(5) ? null : reader.GetString(5)
                    });
                    totalCount = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                }
            }
            await _db.Database.CloseConnectionAsync();
        }

        return new PagedResult<SubjectListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<SubjectUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Subjects.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new SubjectUpsertDto 
        { 
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            IsReligionSubject = entity.IsReligionSubject,
            ReligionType = entity.ReligionType
        };
    }

    public async Task<int> CreateAsync(
     SubjectUpsertDto dto,
     string createdBy,
     CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ArgumentException("Subject code is required");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Subject name is required");

        // 🔥 Normalize input
        var code = dto.Code.Trim().ToUpper();
        var name = dto.Name.Trim();

        // 🔥 Case-insensitive duplicate check
        var exists = await _db.Subjects.AnyAsync(
            x => !x.IsDeleted && x.Code.ToUpper() == code,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException("Subject code already exists");

        var entity = new Subject
        {
            Code = code,
            Name = name,
            IsReligionSubject = dto.IsReligionSubject,
            ReligionType = dto.ReligionType,
            CreatedBy = createdBy
        };

        _db.Subjects.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
    public async Task UpdateAsync(
     SubjectUpsertDto dto,
     string updatedBy,
     CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ArgumentException("Subject code is required");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Subject name is required");

        var entity = await _db.Subjects
            .FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Subject not found.");

        // 🔥 Normalize
        var code = dto.Code.Trim().ToUpper();
        var name = dto.Name.Trim();

        // 🔥 Duplicate check (exclude current record)
        var exists = await _db.Subjects.AnyAsync(
            x => x.Id != dto.Id &&
                 !x.IsDeleted &&
                 x.Code.ToUpper() == code,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException("Subject code already exists");

        entity.Code = code;
        entity.Name = name;
        entity.IsReligionSubject = dto.IsReligionSubject;
        entity.ReligionType = dto.ReligionType;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
    }
    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Subjects.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("Subject not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }
}
