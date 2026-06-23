namespace SchoolManagementSystem.Services.Interfaces.Admin;

public interface IPermissionCacheService
{
    Task<bool> HasPermissionAsync(string[] roleNames, string permissionCode, CancellationToken ct = default);
    void InvalidateRolePermissions(int roleId);
    void InvalidateAll();
}
