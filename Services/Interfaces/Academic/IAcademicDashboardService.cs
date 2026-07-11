using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface IAcademicDashboardService
{
    Task<AcademicDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}
