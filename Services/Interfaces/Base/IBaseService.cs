using System.Linq.Expressions;

namespace SchoolManagementSystem.Services.Interfaces.Base;

public interface IBaseService<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TEntity> CreateAsync(TEntity entity, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<SchoolManagementSystem.Models.DTOs.Common.PagedResult<TEntity>> GetPagedAsync(int page, int pageSize, string? search = null, System.Security.Claims.ClaimsPrincipal? user = null, CancellationToken ct = default);
    IQueryable<TEntity> Query();
}

