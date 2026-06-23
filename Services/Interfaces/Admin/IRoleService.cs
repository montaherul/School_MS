using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.ViewModels.User;

using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Services.Interfaces.Admin;

public interface IRoleService
{
    Task<PagedResult<dynamic>> GetPagedAsync(int page, int pageSize, string? search, string? sortColumn = null, string? sortDirection = null, CancellationToken ct = default);
    Task<List<int>> GetPermissionsByRoleIdAsync(int roleId, CancellationToken ct = default);
    Task<bool> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds, CancellationToken ct = default);
    Task<List<Permission>> GetAllPermissionsAsync(CancellationToken ct = default);
    Task DeleteAsync(int id);
}

