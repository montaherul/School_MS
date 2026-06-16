using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Services.Interfaces.Base;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Models.DTOs.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Services.Implementations.Base;

public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : BaseEntity
{
    protected readonly IUnitOfWork _unitOfWork;

    public BaseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<TEntity>().ListAsync(x => !x.IsDeleted, ct);
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id, ct);
        return entity is { IsDeleted: false } ? entity : null;
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity, string createdBy, CancellationToken ct = default)
    {
        entity.CreatedBy = createdBy;
        entity.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.Repository<TEntity>().AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity, string updatedBy, CancellationToken ct = default)
    {
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<TEntity>().Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<TEntity>().Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }

    public virtual async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<TEntity>().AnyAsync(x => x.Id == id && !x.IsDeleted, ct);
    }

    public virtual async Task<PagedResult<TEntity>> GetPagedAsync(int page, int pageSize, string? search = null, System.Security.Claims.ClaimsPrincipal? user = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<TEntity>().Query().AsNoTracking().Where(x => !x.IsDeleted);

        if (user != null)
        {
            query = ApplySecurityFilters(query, user);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchableProperties = typeof(TEntity).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string))
                .ToList();

            if (searchableProperties.Count > 0)
            {
                var items = await query.ToListAsync(ct);
                items = items.Where(x => searchableProperties.Any(p => 
                    (p.GetValue(x) as string)?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)).ToList();
                
                var total = items.Count;
                var pagedItems = items.OrderByDescending(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                
                return new PagedResult<TEntity>
                {
                    Items = pagedItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = total
                };
            }
        }

        var totalCount = await query.CountAsync(ct);
        var resultItems = await query.OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<TEntity>
        {
            Items = resultItems,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    protected virtual IQueryable<TEntity> ApplySecurityFilters(IQueryable<TEntity> query, System.Security.Claims.ClaimsPrincipal user)
    {
        return query;
    }

    public virtual IQueryable<TEntity> Query()
    {
        return _unitOfWork.Repository<TEntity>().Query().Where(x => !x.IsDeleted);
    }
}

