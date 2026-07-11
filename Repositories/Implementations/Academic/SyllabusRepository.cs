using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;

namespace SchoolManagementSystem.Repositories.Implementations.Academic;

public class SyllabusRepository : BaseRepository<Syllabus>, ISyllabusRepository
{
    public SyllabusRepository(SchoolDbContext db) : base(db) { }
}
