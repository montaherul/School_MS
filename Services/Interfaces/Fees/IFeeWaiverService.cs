using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFeeWaiverService
{
    Task<PagedResult<FeeWaiverListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? studentId = null, CancellationToken cancellationToken = default);
    Task<FeeWaiverUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(FeeWaiverUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(FeeWaiverUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task ApproveAsync(int id, string approvedBy, CancellationToken cancellationToken = default);
    Task RejectAsync(int id, string rejectedBy, CancellationToken cancellationToken = default);
}
