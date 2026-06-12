using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class StudentSubjectResultRepository : BaseRepository<StudentSubjectResult>, IStudentSubjectResultRepository
{
    public StudentSubjectResultRepository(SchoolDbContext db) : base(db) { }
}
