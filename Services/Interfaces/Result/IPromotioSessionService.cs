using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IPromotioSessionService
{
    Task<PagedResult<PromotioSessionListItemDto>> GetPagedAsync(int page, int size, string? search, string? status, CancellationToken ct = default);
    Task<PromotioSessionUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default);
    Task<SchoolManagementSystem.Models.Entities.Result.PromotioSession?> GetSessionWithAcademicYearAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(PromotioSessionUpsertDto dto, string userId, CancellationToken ct = default);
    Task UpdateAsync(PromotioSessionUpsertDto dto, string userId, CancellationToken ct = default);
    Task DeleteAsync(int id, string userId, CancellationToken ct = default);
    Task<List<PromotioCandidateDto>> GetCandidatesAsync(int classId, int academicYearId, string? search, CancellationToken ct = default);
    Task<PromotioResult> BulkPromoteAsync(int sessionId, int fromClassId, int toClassId, int academicYearId, string userId, CancellationToken ct = default);
    Task<List<ClassProgressionRuleDto>> GetProgressionRulesAsync(CancellationToken ct = default);
    Task UpdateProgressionRulesAsync(List<ClassProgressionRuleUpsertDto> rules, string userId, CancellationToken ct = default);
    Task RollbackSessionAsync(int sessionId, string userId, CancellationToken ct = default);
    Task ApproveSessionAsync(int sessionId, string userId, CancellationToken ct = default);
    Task<List<PromotioRegisterDto>> GetPromotioRegisterAsync(int sessionId, CancellationToken ct = default);
    Task<List<FailedStudentDto>> GetFailedStudentsAsync(int academicYearId, int? classId, CancellationToken ct = default);
    Task<PromotioDashboardDto> GetDashboardAsync(int academicYearId, CancellationToken ct = default);
}
