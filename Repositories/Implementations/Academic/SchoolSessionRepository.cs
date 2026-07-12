using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;

namespace SchoolManagementSystem.Repositories.Implementations.Academic;

public class SchoolSessionRepository : BaseRepository<SchoolSession>, ISchoolSessionRepository
{
    public SchoolSessionRepository(SchoolDbContext db) : base(db) { }
}
