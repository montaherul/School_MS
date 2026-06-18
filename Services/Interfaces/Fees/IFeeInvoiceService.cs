using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFeeInvoiceService
{
    Task<PagedResult<FeeInvoiceListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? studentId = null, int? status = null, CancellationToken cancellationToken = default);
    Task<FeeInvoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(FeeInvoice invoice, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(FeeInvoice invoice, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<LateFeeEngineResultDto> ApplyLateFeesAsync(CancellationToken cancellationToken = default);
}

