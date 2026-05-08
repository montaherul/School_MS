using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SectionService : ISectionService
{
    private readonly SchoolDbContext _db;

    public SectionService(SchoolDbContext db) { _db = db; }

    public async Task<PagedResult<SectionListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);

        var items = new List<SectionListItemDto>();
        int totalCount = 0;

        using (var command = _db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetSectionList";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageNumber", page));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageSize", pageSize));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));

            await _db.Database.OpenConnectionAsync(cancellationToken);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    items.Add(new SectionListItemDto
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        SchoolClassId = reader.GetInt32(2),
                        ClassName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        TotalRecords = reader.IsDBNull(5) ? 0 : reader.GetInt32(5)
                    });
                }
            }
            await _db.Database.CloseConnectionAsync();
        }

        totalCount = items.FirstOrDefault()?.TotalRecords ?? 0;

        return new PagedResult<SectionListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<SectionUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Sections.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new SectionUpsertDto
        {
            Id = entity.Id,
            SchoolClassId = entity.SchoolClassId,
            Name = entity.Name,
            ParentSectionId = entity.ParentSectionId
        };
    }

    public async Task<int> CreateAsync(SectionUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new Section
        {
            CreatedBy = createdBy,
            SchoolClassId = dto.SchoolClassId,
            Name = dto.Name,
            ParentSectionId = dto.ParentSectionId
        };
        _db.Sections.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(SectionUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Sections.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Section not found.");
        entity.SchoolClassId = dto.SchoolClassId;
        entity.Name = dto.Name;
        entity.ParentSectionId = dto.ParentSectionId;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Sections.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("Section not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }
}

