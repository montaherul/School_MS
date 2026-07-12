using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface IStudentGroupService
{
    Task<List<StudentGroupListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
    Task<StudentGroupUpsertDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(StudentGroupUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(StudentGroupUpsertDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);
    Task<bool> IsCodeUniqueAsync(string code, int? excludeId, CancellationToken ct = default);
    Task<List<StudentGroupListItemDto>> GetAllAsync(CancellationToken ct = default);
}
