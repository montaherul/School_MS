using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Repositories.Interfaces.Result;

public interface IExamRepository : IBaseRepository<Exam>
{
    Task<IEnumerable<ExamUpsertDto>> GetExamsForAdminAsync(int academicYearId, CancellationToken ct);
}
