using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;

namespace SchoolManagementSystem.Repositories.Implementations.Academic;

public class StudyMaterialRepository : BaseRepository<StudyMaterial>, IStudyMaterialRepository
{
    public StudyMaterialRepository(SchoolDbContext db) : base(db) { }
}
