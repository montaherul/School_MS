using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFeeDashboardService
{
    Task<FeeDashboardDto> GetDashboardDataAsync(int? academicYearId = null, CancellationToken cancellationToken = default);
}
