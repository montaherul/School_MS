using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class GradingRuleRepository : BaseRepository<GradingRule>, IGradingRuleRepository
{
    public GradingRuleRepository(SchoolDbContext db) : base(db) { }
}
