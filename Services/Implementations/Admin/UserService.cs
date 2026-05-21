using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.User;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Admin;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHashService _passwordHashService;

    public UserService(IUnitOfWork unitOfWork, IPasswordHashService passwordHashService)
    {
        _unitOfWork = unitOfWork;
        _passwordHashService = passwordHashService;
    }

    public async Task<PagedResult<UserListItemVm>> GetPagedAsync(
       int page,
       int pageSize,
       string? search,
       int? status = null,
       string? role = null,
       CancellationToken ct = default)
    {
        var query = _unitOfWork
            .Repository<ApplicationUser>()
            .Query()
            .Where(u => !u.IsDeleted);

        // SEARCH
        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLower();

            query = query.Where(u =>

                u.UserName.ToLower().Contains(lower)

                || u.Email.ToLower().Contains(lower)

                || (u.PhoneNumber != null
                    && u.PhoneNumber.ToLower().Contains(lower)));
        }

        // STATUS FILTER
        if (status.HasValue)
        {
            query = query.Where(u => (int)u.Status == status.Value);
        }

        // ROLE FILTER
        if (!string.IsNullOrWhiteSpace(role))
        {
            query = query.Where(u =>
                u.UserRoles.Any(ur =>
                    ur.Role != null
                    && ur.Role.Name == role));
        }

        var totalCount = await query.CountAsync(ct);

        var users = await query

            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)

            .OrderByDescending(u => u.Id)

            .Skip((page - 1) * pageSize)

            .Take(pageSize)

            .ToListAsync(ct);

        var items = users.Select(u => new UserListItemVm
        {
            Id = u.Id,

            UserName = u.UserName,

            Email = u.Email,

            PhoneNumber = u.PhoneNumber,

            Status = u.Status,

            IsDeleted = u.IsDeleted,

            RolesText = string.Join(
                ", ",
                u.UserRoles
                    .Where(ur => ur.Role != null)
                    .Select(ur => ur.Role!.Name)
                    .Distinct()
                    .OrderBy(name => name)),

            TotalRecords = totalCount
        }).ToList();

        return new PagedResult<UserListItemVm>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<UserUpsertViewModel?> GetForEditAsync(int id, CancellationToken ct = default)
    {
        var user = await _unitOfWork.Repository<ApplicationUser>().FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, ct);
        if (user == null) return null;

        var selectedRoleIds = await _unitOfWork.Repository<UserRole>().Query()
            .Where(ur => ur.UserId == id)
            .Select(ur => ur.RoleId)
            .ToListAsync(ct);

        return new UserUpsertViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Status = user.Status,
            SelectedRoleIds = selectedRoleIds
        };
    }

    public async Task<UserDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default)
    {
        var user = await _unitOfWork.Repository<ApplicationUser>().Query()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, ct);

        if (user == null) return null;

        return new UserDetailsViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Status = user.Status,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            UpdatedBy = user.UpdatedBy,
            Roles = user.UserRoles.Where(ur => ur.Role != null).Select(ur => ur.Role!.Name).ToList()
        };
    }

    public async Task<int> CreateAsync(UserUpsertViewModel model, string createdBy, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<ApplicationUser>();
        if (await repo.AnyAsync(u => u.UserName == model.UserName.Trim() && !u.IsDeleted, ct))
            throw new InvalidOperationException("Username is already taken.");

        var user = new ApplicationUser
        {
            UserName = model.UserName.Trim(),
            Email = model.Email.Trim(),
            PhoneNumber = model.PhoneNumber?.Trim(),
            Status = model.Status,
            PasswordHash = _passwordHashService.HashPassword(model.Password ?? ""),
            IsEmailConfirmed = model.Status == AccountStatus.Active,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        if (model.SelectedRoleIds != null)
            await AssignRolesAsync(user.Id, model.SelectedRoleIds, ct);

        return user.Id;
    }

    public async Task UpdateAsync(UserUpsertViewModel model, string updatedBy, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<ApplicationUser>();
        var user = await repo.FirstOrDefaultAsync(u => u.Id == model.Id && !u.IsDeleted, ct) 
            ?? throw new InvalidOperationException("User not found.");

        if (await repo.AnyAsync(u => u.Id != model.Id && u.UserName == model.UserName.Trim() && !u.IsDeleted, ct))
            throw new InvalidOperationException("Username is already taken.");

        user.UserName = model.UserName.Trim();
        user.Email = model.Email.Trim();
        user.PhoneNumber = model.PhoneNumber?.Trim();
        user.Status = model.Status;
        user.UpdatedBy = updatedBy;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(model.Password))
            user.PasswordHash = _passwordHashService.HashPassword(model.Password);

        await _unitOfWork.SaveChangesAsync(ct);

        if (model.SelectedRoleIds != null)
            await AssignRolesAsync(user.Id, model.SelectedRoleIds, ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var user = await _unitOfWork.Repository<ApplicationUser>().FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, ct)
            ?? throw new InvalidOperationException("User not found.");

        user.IsDeleted = true;
        user.Status = AccountStatus.Inactive;
        user.UpdatedBy = updatedBy;
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task AssignRolesAsync(int userId, IEnumerable<int> roleIds, CancellationToken ct = default)
    {
        var urRepo = _unitOfWork.Repository<UserRole>();
        var existing = await urRepo.ListAsync(ur => ur.UserId == userId);
        foreach (var ex in existing) urRepo.Remove(ex);

        foreach (var roleId in roleIds.Distinct())
        {
            await urRepo.AddAsync(new UserRole { UserId = userId, RoleId = roleId }, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<RoleOptionVm>> GetAvailableRolesAsync(CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<Role>().Query()
            .AsNoTracking()
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Name)
            .Select(r => new RoleOptionVm { Id = r.Id, Name = r.Name, Description = r.Description })
            .ToListAsync(ct);
    }
}

