using SchoolManagementSystem.Models.ViewModels.Dashboard;

namespace SchoolManagementSystem.Services.Interfaces.Dashboard;

public interface IDashboardResolverService
{
    Task<object> GetDashboardModelAsync(long userId, string[] roles, CancellationToken ct = default);
    Task<string> GetDashboardViewNameAsync(string[] roles);
    
    /// <summary>
    /// Resolves the primary dashboard view name based on user roles and priorities.
    /// </summary>
    Task<string> ResolveDashboardViewAsync(int userId);

    /// <summary>
    /// Gets all authorized widgets for a user across all their roles.
    /// </summary>
    Task<IEnumerable<string>> GetAuthorizedWidgetsAsync(int userId);

    /// <summary>
    /// Checks if a user has a specific permission, considering super admin override and merged roles.
    /// </summary>
    Task<bool> HasPermissionAsync(int userId, string permissionCode);
}
