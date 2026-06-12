using SchoolManagementSystem.Models.Entities.Student;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IStudentSubjectFilterService
{
    Task<HashSet<int>> GetValidSubjectIdsForStudentAsync(Student student, CancellationToken ct = default);
}
