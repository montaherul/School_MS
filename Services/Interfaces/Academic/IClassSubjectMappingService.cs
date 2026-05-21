using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface IClassSubjectMappingService
{
    Task<PagedResult<ClassSubjectListItemDto>> GetPagedAsync(
        int page, 
        int pageSize, 
        int? classId, 
        string? groupName, 
        string? search, 
        CancellationToken ct = default);

    Task<ClassSubjectUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default);

    Task<int> CreateOrUpdateAsync(ClassSubjectUpsertDto dto, string userId, CancellationToken ct = default);

    Task SaveAssignmentsAsync(ClassSubjectAssignmentDto dto, string userId, CancellationToken ct = default);

    Task DeleteAsync(int id, string userId, CancellationToken ct = default);
    
    Task<IEnumerable<SubjectListItemDto>> GetUnmappedSubjectsAsync(int classId, string? groupName, CancellationToken ct = default);
}
