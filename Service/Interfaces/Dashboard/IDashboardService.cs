using SchoolManagementSystem.Models.ViewModels.Dashboard;

namespace SchoolManagementSystem.Service.Interfaces.Dashboard;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default);
}
