using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Services.Interfaces.Result;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class AuditLogger : IAuditLogger
{
    private readonly IUnitOfWork _uow;

    public AuditLogger(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task LogMarkChangeAsync(int examId, int studentId, int subjectId, decimal oldMarks, decimal newMarks, int changedByUserId, string reason)
    {
        var audit = new ResultAuditLog
        {
            ExamId = examId,
            StudentId = studentId,
            SubjectId = subjectId,
            OldMarks = oldMarks,
            NewMarks = newMarks,
            ChangedByUserId = changedByUserId,
            Reason = reason
        };

        await _uow.Repository<ResultAuditLog>().AddAsync(audit);
    }
}
