using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface ISectionService
{
    Task<PagedResult<SectionListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<SectionUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SectionUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(SectionUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    
    // New methods for Admission/Academic management
    Task<IEnumerable<SectionOptionDto>> GetByClassIdAsync(int classId, int? studentGroupId = null, CancellationToken ct = default);
    Task<IEnumerable<SectionListItemDto>> GetGroupsByClassIdAsync(int classId, CancellationToken ct = default);
    Task<IEnumerable<SectionListItemDto>> GetStudentGroupsByClassIdAsync(int classId, CancellationToken ct = default);
    Task<int> CreateAjaxAsync(int classId, string name, int? parentId, string createdBy, CancellationToken ct = default);
    Task<IEnumerable<object>> GetAdmissionSectionsAsync(int classId, CancellationToken ct = default);
    Task<IEnumerable<dynamic>> GetAvailableClassesAsync(CancellationToken ct = default);

    Task<IEnumerable<SectionOptionDto>> GetSectionsByClassWithFilterAsync(int classId, bool isStaff, List<int>? assignedSectionIds, int? studentGroupId, CancellationToken ct = default);
    Task AssignStudentToSectionAsync(int studentId, int sectionId, CancellationToken ct = default);
}

