using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class GradingRuleRepository : BaseRepository<GradingRule>, IGradingRuleRepository 
{ 
    public GradingRuleRepository(SchoolDbContext db) : base(db) { } 
}

public class StudentSubjectResultRepository : BaseRepository<StudentSubjectResult>, IStudentSubjectResultRepository 
{ 
    public StudentSubjectResultRepository(SchoolDbContext db) : base(db) { } 
}

public class ReEvaluationRequestRepository : BaseRepository<ReEvaluationRequest>, IReEvaluationRequestRepository 
{ 
    public ReEvaluationRequestRepository(SchoolDbContext db) : base(db) { } 
}

public class ResultAuditLogRepository : BaseRepository<ResultAuditLog>, IResultAuditLogRepository 
{ 
    public ResultAuditLogRepository(SchoolDbContext db) : base(db) { } 
}
