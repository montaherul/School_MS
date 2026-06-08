using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class MeritResultRepository : BaseRepository<MeritResult>, IMeritResultRepository 
{ 
    public MeritResultRepository(SchoolDbContext db) : base(db) { } 
}

public class FinalResultRepository : BaseRepository<FinalResult>, IFinalResultRepository 
{ 
    public FinalResultRepository(SchoolDbContext db) : base(db) { } 
}

public class PromotionHistoryRepository : BaseRepository<PromotionHistory>, IPromotionHistoryRepository 
{ 
    public PromotionHistoryRepository(SchoolDbContext db) : base(db) { } 
}
