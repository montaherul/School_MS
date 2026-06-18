using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IFeeDashboardRepository
{
    Task<FeeDashboardDto> GetDashboardDataAsync(int? academicYearId, CancellationToken ct);
}
