using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.ViewModels.User;

namespace SchoolManagementSystem.Services.Interfaces.Admin;

public interface IRoleService
{
    Task<PagedResult<dynamic>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
    // Add other methods as needed
}

