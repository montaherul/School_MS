using SchoolManagementSystem.Models.DTOs.Routine;

namespace SchoolManagementSystem.Repositories.Interfaces.Routine;

public interface IRoutineDashboardRepository
{
    Task<RoutineDashboardDto> GetDashboardAsync(int academicYearId);
}
