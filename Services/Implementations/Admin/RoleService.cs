using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Admin;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionCacheService _permissionCache;

    private static readonly string[] SystemRoles = ["Super Admin", "Admin", "Guardian", "Student", "Teacher", "Senior Lecturer", "Lecturer", "Principal", "Accountant", "Exam Controller"];

    public RoleService(IUnitOfWork unitOfWork, IPermissionCacheService permissionCache)
    {
        _unitOfWork = unitOfWork;
        _permissionCache = permissionCache;
    }

    public async Task<PagedResult<dynamic>> GetPagedAsync(int page, int pageSize, string? search, string? sortColumn = null, string? sortDirection = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<Role>().Query().AsNoTracking().Where(r => !r.IsDeleted);

        if (!string.IsNullOrEmpty(search))
        {
            var lower = search.ToLower();
            query = query.Where(r => r.Name.ToLower().Contains(lower) || (r.Description ?? string.Empty).ToLower().Contains(lower));
        }

        var totalCount = await query.CountAsync(ct);

        var isDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        query = sortColumn?.ToLower() switch
        {
            "name" => isDesc ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
            "description" => isDesc ? query.OrderByDescending(r => r.Description ?? "") : query.OrderBy(r => r.Description ?? ""),
            _ => isDesc ? query.OrderByDescending(r => r.Id) : query.OrderBy(r => r.Id)
        };

        var items = await query
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

    public async Task<List<int>> GetPermissionsByRoleIdAsync(int roleId, CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<RolePermission>().Query()
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync(ct);
    }

    public async Task<bool> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<RolePermission>();
        
        // Remove existing permissions
        var existing = await repo.Query().Where(rp => rp.RoleId == roleId).ToListAsync(ct);
        foreach (var rp in existing)
        {
            repo.Remove(rp);
        }

        // Add new permissions
        foreach (var pid in permissionIds)
        {
            await repo.AddAsync(new RolePermission { RoleId = roleId, PermissionId = pid }, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        _permissionCache.InvalidateRolePermissions(roleId);
        return true;
    }

    public async Task<List<Permission>> GetAllPermissionsAsync(CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<Permission>().Query()
            .OrderBy(p => p.Module).ThenBy(p => p.Action)
            .ToListAsync(ct);
    }

    public async Task DeleteAsync(int id)
    {
        var role = await _unitOfWork.Repository<Role>().GetByIdAsync(id)
            ?? throw new ArgumentException("Role not found.");

        if (SystemRoles.Contains(role.Name))
            throw new InvalidOperationException($"Cannot delete system role '{role.Name}'.");

        role.IsDeleted = true;

        var userRoles = await _unitOfWork.Repository<UserRole>().Query()
            .Where(ur => ur.RoleId == id).ToListAsync();
        foreach (var ur in userRoles)
            _unitOfWork.Repository<UserRole>().Remove(ur);

        var rolePermissions = await _unitOfWork.Repository<RolePermission>().Query()
            .Where(rp => rp.RoleId == id).ToListAsync();
        foreach (var rp in rolePermissions)
            _unitOfWork.Repository<RolePermission>().Remove(rp);

        await _unitOfWork.SaveChangesAsync();
        _permissionCache.InvalidateRolePermissions(id);
    }
}

