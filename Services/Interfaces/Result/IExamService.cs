using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IExamService
{
    // Exam CRUD
    Task<IEnumerable<ExamUpsertDto>> GetExamsAsync(int academicYearId);
    Task<object?> CreateExamAsync(ExamUpsertDto dto, CancellationToken ct = default);
    Task<object?> UpdateExamAsync(int examId, ExamUpsertDto dto, CancellationToken ct = default);
    Task DeleteExamAsync(int examId, CancellationToken ct = default);
    Task<object?> GetExamByIdAsync(int examId, CancellationToken ct = default);

    // Grading Rules
    Task<IEnumerable<GradingRuleUpsertDto>> GetGradingRulesAsync(CancellationToken ct = default);
    Task<object?> UpsertGradingRuleAsync(GradingRuleUpsertDto dto, CancellationToken ct = default);
    Task DeleteGradingRuleAsync(int ruleId, CancellationToken ct = default);

    // Exam Management
    Task LockExamAsync(int examId, int userId, string? reason = null, CancellationToken ct = default);
    Task UnlockExamAsync(int examId, string? reason = null, CancellationToken ct = default);
    Task<object?> GetExamStatusAsync(int examId, CancellationToken ct = default);

    // Helpers
    Task<IEnumerable<object>> GetSubjectsAsync(CancellationToken ct = default);
    Task<IEnumerable<object>> GetSubjectsByClassIdAsync(int classId, int? groupId = null, int? sectionId = null, CancellationToken ct = default);
    Task<IEnumerable<object>> GetClassesAsync(CancellationToken ct = default);
    Task<IEnumerable<object>> GetSectionsAsync(int? classId = null, CancellationToken ct = default);
}

