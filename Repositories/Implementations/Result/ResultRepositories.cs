using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class ExamRepository : GenericRepository<Exam>, IExamRepository
{
    public ExamRepository(SchoolDbContext db) : base(db) { }
}

public class MarkEntryRepository : GenericRepository<MarkEntry>, IMarkEntryRepository
{
    public MarkEntryRepository(SchoolDbContext db) : base(db) { }
}

public class GradingRuleRepository : GenericRepository<GradingRule>, IGradingRuleRepository
{
    public GradingRuleRepository(SchoolDbContext db) : base(db) { }
}

public class ResultPublicationRepository : GenericRepository<ResultPublication>, IResultPublicationRepository
{
    public ResultPublicationRepository(SchoolDbContext db) : base(db) { }
}

public class StudentSubjectResultRepository : GenericRepository<StudentSubjectResult>, IStudentSubjectResultRepository
{
    public StudentSubjectResultRepository(SchoolDbContext db) : base(db) { }
}

public class StudentExamResultRepository : GenericRepository<StudentExamResult>, IStudentExamResultRepository
{
    public StudentExamResultRepository(SchoolDbContext db) : base(db) { }
}

public class FinalResultRepository : GenericRepository<FinalResult>, IFinalResultRepository
{
    public FinalResultRepository(SchoolDbContext db) : base(db) { }
}

public class ResultAuditLogRepository : GenericRepository<ResultAuditLog>, IResultAuditLogRepository
{
    public ResultAuditLogRepository(SchoolDbContext db) : base(db) { }
}

public class ReEvaluationRequestRepository : GenericRepository<ReEvaluationRequest>, IReEvaluationRequestRepository
{
    public ReEvaluationRequestRepository(SchoolDbContext db) : base(db) { }
}
