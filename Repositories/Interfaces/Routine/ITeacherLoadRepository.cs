using SchoolManagementSystem.Models.DTOs.Routine;

namespace SchoolManagementSystem.Repositories.Interfaces.Routine;

public interface ITeacherLoadRepository
{
    Task<List<TeacherLoadDto>> GetTeacherLoadSummaryAsync(int academicYearId);
}
