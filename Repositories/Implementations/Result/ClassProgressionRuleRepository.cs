using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class ClassProgressionRuleRepository : BaseRepository<ClassProgressionRule>, IClassProgressionRuleRepository
{
    public ClassProgressionRuleRepository(SchoolDbContext db) : base(db) { }
}
