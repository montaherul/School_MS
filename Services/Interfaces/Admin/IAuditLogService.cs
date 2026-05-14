using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.ViewModels.Admin;

namespace SchoolManagementSystem.Services.Interfaces.Admin;

public interface IAuditLogService
{
    Task<PagedResult<AuditLogListItemViewModel>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
}
