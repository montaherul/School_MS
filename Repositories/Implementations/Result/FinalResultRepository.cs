using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class FinalResultRepository : BaseRepository<FinalResult>, IFinalResultRepository
{
    public FinalResultRepository(SchoolDbContext db) : base(db) { }
}
