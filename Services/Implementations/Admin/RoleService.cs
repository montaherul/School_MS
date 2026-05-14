using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Admin;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<PagedResult<dynamic>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<Role>().Query().Where(r => !r.IsDeleted);

        if (!string.IsNullOrEmpty(search))
        {
            var lower = search.ToLower();
            query = query.Where(r => r.Name.ToLower().Contains(lower) || (r.Description ?? string.Empty).ToLower().Contains(lower));
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query.OrderBy(r => r.Name)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(r => new
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description ?? "",
                PermissionCount = r.RolePermissions.Count,
                UserCount = r.UserRoles.Count
            }).ToListAsync(ct);

        return new PagedResult<dynamic> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }
}

