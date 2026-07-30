using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IOnlinePaymentRepository : IBaseRepository<OnlinePaymentRequest>
{
    Task<(List<OnlinePaymentRequestListItemDto> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber, int pageSize, string? searchTerm, int? statusFilter, CancellationToken ct);
}
