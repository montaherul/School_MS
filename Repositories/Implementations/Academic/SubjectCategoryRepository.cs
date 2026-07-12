using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;

namespace SchoolManagementSystem.Repositories.Implementations.Academic;

public class SubjectCategoryRepository : BaseRepository<SubjectCategory>, ISubjectCategoryRepository
{
    public SubjectCategoryRepository(SchoolDbContext db) : base(db) { }
}
