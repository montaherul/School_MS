using SchoolManagementSystem.Models.DTOs.Routine;

namespace SchoolManagementSystem.Repositories.Interfaces.Routine;

public interface IRoutineAnalyticsRepository
{
    Task<RoutineAnalyticsViewModel> GetAnalyticsAsync(int academicYearId);
}
