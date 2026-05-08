using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class AcademicYearService : IAcademicYearService
{
    private readonly SchoolDbContext _db;

    public AcademicYearService(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<AcademicYearListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);

        var items = new List<AcademicYearListItemDto>();
        int totalCount = 0;

        using (var command = _db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetAcademicYearList";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageNumber", page));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageSize", pageSize));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));

            await _db.Database.OpenConnectionAsync(cancellationToken);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    items.Add(new AcademicYearListItemDto
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        StartsOn = reader.GetDateTime(2).ToString("yyyy-MM-dd"),
                        EndsOn = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                        IsActive = reader.GetBoolean(4)
                    });
                    totalCount = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                }
            }
            await _db.Database.CloseConnectionAsync();
        }

        return new PagedResult<AcademicYearListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<AcademicYearUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.AcademicYears.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;

        return new AcademicYearUpsertDto
        {
            Id = entity.Id,
            Name = entity.Name,
            StartsOn = entity.StartsOn,
            EndsOn = entity.EndsOn,
            IsActive = entity.IsActive
        };
    }

    public async Task<int> CreateAsync(AcademicYearUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new AcademicYear
        {
            Name = dto.Name.Trim(),
            StartsOn = dto.StartsOn,
            EndsOn = dto.EndsOn,
            IsActive = dto.IsActive,
            CreatedBy = createdBy
        };

        _db.AcademicYears.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(AcademicYearUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.AcademicYears.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Academic Year not found.");

        entity.Name = dto.Name.Trim();
        entity.StartsOn = dto.StartsOn;
        entity.EndsOn = dto.EndsOn;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.AcademicYears.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Academic Year not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
