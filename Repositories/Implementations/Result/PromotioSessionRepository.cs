using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class PromotioSessionRepository : BaseRepository<PromotioSession>, IPromotioSessionRepository
{
    public PromotioSessionRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<PromotioSessionListItemDto>> GetPagedSessionsAsync(int page, int size, string? search, string? status, CancellationToken ct = default)
    {
        return await ExecuteStoredProcAsync<PromotioSessionListItemDto>(
            "sp_GetPromotionSessionsPaged @p0, @p1, @p2, @p3",
            page, size, search ?? (object)DBNull.Value, status ?? (object)DBNull.Value);
    }
}
