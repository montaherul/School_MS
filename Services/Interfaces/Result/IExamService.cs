using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IExamService
{
    // Exam read operations
    Task<IEnumerable<ExamListDto>> GetExamsAsync(int academicYearId, CancellationToken ct = default);
    Task<(IEnumerable<ExamListDto> Items, int TotalCount)> GetPagedExamsAsync(
        int academicYearId, string? searchTerm, int? status,
        int pageNumber, int pageSize, string sortColumn, string sortDirection,
        CancellationToken ct = default);
    Task<ExamDetailsDto?> GetExamDetailsAsync(int examId, CancellationToken ct = default);
    Task<ExamUpsertDto?> GetExamForEditAsync(int examId, CancellationToken ct = default);

    // Exam write operations (upsert DTO only)
    Task<object?> CreateExamAsync(ExamUpsertDto dto, CancellationToken ct = default);
    Task<List<object?>> CreateExamsBulkAsync(ExamUpsertDto dto, CancellationToken ct = default);
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

    // Dynamic curriculum-based exam subject generation (Phase 10)
    Task<int> GenerateExamSubjectsFromCurriculumAsync(int examId, int classId, int? groupId = null, CancellationToken ct = default);
    Task<int> GenerateReligionExamSubjectsAsync(int examId, int classId, CancellationToken ct = default);
    Task<int> GenerateOptionalExamSubjectsAsync(int examId, int classId, CancellationToken ct = default);
}

