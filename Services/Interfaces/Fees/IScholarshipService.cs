using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IScholarshipService
{
    Task<PagedResult<ScholarshipListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<ScholarshipUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(ScholarshipUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScholarshipUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}
