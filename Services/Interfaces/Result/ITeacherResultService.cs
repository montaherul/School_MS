using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface ITeacherResultService
{
    Task<TeacherResultDashboardDto> GetDashboardAsync(int teacherId, int academicYearId, CancellationToken ct = default);
}
