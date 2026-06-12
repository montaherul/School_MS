using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class PromotionHistoryRepository : BaseRepository<PromotionHistory>, IPromotionHistoryRepository
{
    public PromotionHistoryRepository(SchoolDbContext db) : base(db) { }
}
