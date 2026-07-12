using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Repositories.Interfaces.Result;

public interface IPromotioSessionRepository : IBaseRepository<PromotioSession>
{
    Task<List<PromotioSessionListItemDto>> GetPagedSessionsAsync(int page, int size, string? search, string? status, CancellationToken ct = default);
}
