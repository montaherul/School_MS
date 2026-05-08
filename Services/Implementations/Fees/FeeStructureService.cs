using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeStructureService : IFeeStructureService
{
    private readonly SchoolDbContext _db;

    public FeeStructureService(SchoolDbContext db) { _db = db; }

    public async Task<PagedResult<FeeStructureListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);

        var items = new List<FeeStructureListItemDto>();
        int totalCount = 0;

        using (var command = _db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetFeeStructureList";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageNumber", page));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageSize", pageSize));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));

            await _db.Database.OpenConnectionAsync(cancellationToken);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    items.Add(new FeeStructureListItemDto
                    {
                        Id = reader.GetInt32(0),
                        SchoolClassId = reader.GetInt32(1),
                        ClassName = reader.GetString(2),
                        FeeName = reader.GetString(3),
                        Amount = reader.GetDecimal(4),
                        IsRecurring = reader.GetBoolean(5),
                        TotalRecords = reader.IsDBNull(6) ? 0 : reader.GetInt32(6)
                    });
                }
            }
            await _db.Database.CloseConnectionAsync();
        }

        totalCount = items.FirstOrDefault()?.TotalRecords ?? 0;

        return new PagedResult<FeeStructureListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<FeeStructureUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.FeeStructures.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new FeeStructureUpsertDto { Id = entity.Id,SchoolClassId = entity.SchoolClassId,FeeName = entity.FeeName,Amount = entity.Amount,IsRecurring = entity.IsRecurring,        };
    }

    public async Task<int> CreateAsync(FeeStructureUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new FeeStructure { CreatedBy = createdBy,SchoolClassId = dto.SchoolClassId,FeeName = dto.FeeName,Amount = dto.Amount,IsRecurring = dto.IsRecurring,        };
        _db.FeeStructures.Add(entity); await _db.SaveChangesAsync(cancellationToken); return entity.Id;
    }

    public async Task UpdateAsync(FeeStructureUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.FeeStructures.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("FeeStructure not found.");
        entity.SchoolClassId = dto.SchoolClassId;
        entity.FeeName = dto.FeeName;
        entity.Amount = dto.Amount;
        entity.IsRecurring = dto.IsRecurring;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.FeeStructures.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("FeeStructure not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }
}

