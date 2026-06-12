using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class ReEvaluationRequestRepository : BaseRepository<ReEvaluationRequest>, IReEvaluationRequestRepository
{
    public ReEvaluationRequestRepository(SchoolDbContext db) : base(db) { }
}
