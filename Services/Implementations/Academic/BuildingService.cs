using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class BuildingService : IBuildingService
{
    private readonly IUnitOfWork _uow;

    public BuildingService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<PagedResult<BuildingListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _uow.Repository<Building>().Query()
            .Where(x => !x.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Name.Contains(search) || x.Code.Contains(search));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtoItems = items.Select(x => new BuildingListItemDto
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            Address = x.Address,
            TotalFloors = x.TotalFloors,
            IsActive = x.IsActive,
            TotalRecords = totalCount
        }).ToList();

        return new PagedResult<BuildingListItemDto>
        {
            Items = dtoItems,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<BuildingUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Repository<Building>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new BuildingUpsertDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Address = entity.Address,
            TotalFloors = entity.TotalFloors,
            Description = entity.Description,
            IsActive = entity.IsActive
        };
    }

    public async Task<int> CreateAsync(BuildingUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new Building
        {
            Name = dto.Name.Trim(),
            Code = dto.Code.Trim().ToUpperInvariant(),
            Address = dto.Address?.Trim(),
            TotalFloors = dto.TotalFloors,
            Description = dto.Description?.Trim(),
            IsActive = dto.IsActive,
            CreatedBy = createdBy
        };
        await _uow.Repository<Building>().AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(BuildingUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Repository<Building>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Building not found.");
        entity.Name = dto.Name.Trim();
        entity.Code = dto.Code.Trim().ToUpperInvariant();
        entity.Address = dto.Address?.Trim();
        entity.TotalFloors = dto.TotalFloors;
        entity.Description = dto.Description?.Trim();
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Repository<Building>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Building not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
