using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.ViewModels.User;

namespace SchoolManagementSystem.Services.Interfaces.Admin;

public interface IUserService
{
    Task<PagedResult<UserListItemVm>> GetPagedAsync(
      int page,
      int pageSize,
      string? search,
      int? status = null,
      string? role = null,
      CancellationToken ct = default);
    Task<UserUpsertViewModel?> GetForEditAsync(int id, CancellationToken ct = default);
    Task<UserDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(UserUpsertViewModel model, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(UserUpsertViewModel model, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);
    Task AssignRolesAsync(int userId, IEnumerable<int> roleIds, CancellationToken ct = default);
    Task<IEnumerable<RoleOptionVm>> GetAvailableRolesAsync(CancellationToken ct = default);
}

