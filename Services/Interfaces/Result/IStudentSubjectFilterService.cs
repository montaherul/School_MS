using StudentModel = SchoolManagementSystem.Models.Entities.Student.Student;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IStudentSubjectFilterService
{
    Task<HashSet<int>> GetValidSubjectIdsForStudentAsync(StudentModel student, CancellationToken ct = default);
}
