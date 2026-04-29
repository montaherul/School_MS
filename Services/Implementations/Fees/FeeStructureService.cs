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
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 5, 100); var term = search?.Trim();
        var query = _db.FeeStructures.Where(x => !x.IsDeleted);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(x => new FeeStructureListItemDto {
            Id = x.Id,SchoolClassId = x.SchoolClassId,FeeName = x.FeeName,Amount = x.Amount,IsRecurring = x.IsRecurring,        }).ToListAsync(cancellationToken);
        return new PagedResult<FeeStructureListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
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

