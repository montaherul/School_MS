using SchoolManagementSystem.Models.ViewModels.Dashboard;

namespace SchoolManagementSystem.Services.Interfaces.Dashboard;

public interface IDashboardResolverService
{
    Task<object> GetDashboardModelAsync(long userId, string[] roles, CancellationToken ct = default);
    Task<string> GetDashboardViewNameAsync(string[] roles);
}
