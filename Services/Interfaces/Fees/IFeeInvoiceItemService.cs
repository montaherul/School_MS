using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFeeInvoiceItemService
{
    Task<PagedResult<FeeInvoiceItemListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? feeInvoiceId = null, CancellationToken cancellationToken = default);
    Task<FeeInvoiceItemUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(FeeInvoiceItemUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(FeeInvoiceItemUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}
