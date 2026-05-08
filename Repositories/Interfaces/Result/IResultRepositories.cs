using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Repositories.Interfaces.Result;

public interface IExamRepository : IGenericRepository<Exam> { }
public interface IMarkEntryRepository : IGenericRepository<MarkEntry> { }
public interface IGradingRuleRepository : IGenericRepository<GradingRule> { }
public interface IResultPublicationRepository : IGenericRepository<ResultPublication> { }
public interface IStudentSubjectResultRepository : IGenericRepository<StudentSubjectResult> { }
public interface IStudentExamResultRepository : IGenericRepository<StudentExamResult> { }
public interface IFinalResultRepository : IGenericRepository<FinalResult> { }
public interface IResultAuditLogRepository : IGenericRepository<ResultAuditLog> { }
public interface IReEvaluationRequestRepository : IGenericRepository<ReEvaluationRequest> { }
