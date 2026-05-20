using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.ViewModels.User;

<<<<<<< HEAD
=======
using SchoolManagementSystem.Models.Entities.Auth;

>>>>>>> d8b24e6 (attendece and website curtomize)
namespace SchoolManagementSystem.Services.Interfaces.Admin;

public interface IRoleService
{
    Task<PagedResult<dynamic>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
<<<<<<< HEAD
    // Add other methods as needed
=======
    Task<List<int>> GetPermissionsByRoleIdAsync(int roleId, CancellationToken ct = default);
    Task<bool> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds, CancellationToken ct = default);
    Task<List<Permission>> GetAllPermissionsAsync(CancellationToken ct = default);
>>>>>>> d8b24e6 (attendece and website curtomize)
}

