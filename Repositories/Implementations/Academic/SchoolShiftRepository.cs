using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;

namespace SchoolManagementSystem.Repositories.Implementations.Academic;

public class SchoolShiftRepository : BaseRepository<SchoolShift>, ISchoolShiftRepository
{
    public SchoolShiftRepository(SchoolDbContext db) : base(db) { }
}
