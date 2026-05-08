using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SchoolClassService : ISchoolClassService
{
    private readonly SchoolDbContext _db;

    public SchoolClassService(SchoolDbContext db) { _db = db; }

    public async Task<PagedResult<SchoolClassListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);

        var items = new List<SchoolClassListItemDto>();
        int totalCount = 0;

        using (var command = _db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetClassList";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageNumber", page));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageSize", pageSize));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));

            await _db.Database.OpenConnectionAsync(cancellationToken);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    items.Add(new SchoolClassListItemDto
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        SortOrder = reader.GetInt32(2),
                        SectionCount = reader.GetInt32(3),
                        StudentCount = reader.GetInt32(4),
                        TotalRecords = reader.IsDBNull(5) ? 0 : reader.GetInt32(5)
                    });
                }
            }
            await _db.Database.CloseConnectionAsync();
        }

        totalCount = items.FirstOrDefault()?.TotalRecords ?? 0;

        return new PagedResult<SchoolClassListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<SchoolClassUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Classes.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new SchoolClassUpsertDto { Id = entity.Id,Name = entity.Name,SortOrder = entity.SortOrder,        };
    }

    public async Task<int> CreateAsync(SchoolClassUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new SchoolClass { CreatedBy = createdBy,Name = dto.Name,SortOrder = dto.SortOrder,        };
        _db.Classes.Add(entity); await _db.SaveChangesAsync(cancellationToken); return entity.Id;
    }

    public async Task UpdateAsync(SchoolClassUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Classes.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("SchoolClass not found.");
        entity.Name = dto.Name;
        entity.SortOrder = dto.SortOrder;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Classes.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("SchoolClass not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }
}

