namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IAuditLogger
{
    Task LogMarkChangeAsync(int examId, int studentId, int subjectId, decimal oldMarks, decimal newMarks, int changedByUserId, string reason);
}
