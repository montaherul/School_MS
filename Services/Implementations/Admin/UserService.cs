using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Auth;
using EmpEntity = SchoolManagementSystem.Models.Entities.Employee.Employee;
using GdnEntity = SchoolManagementSystem.Models.Entities.Guardian.Guardian;
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
       string? userType = null,
       string? sortColumn = null,
       string? sortDirection = null,
       CancellationToken ct = default)
    {
        var query = _unitOfWork
            .Repository<ApplicationUser>()
            .Query()
            .AsNoTracking()
            .Where(u => !u.IsDeleted);

        // SEARCH
        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLower();
            query = query.Where(u =>
                u.UserName.ToLower().Contains(lower)
                || u.Email.ToLower().Contains(lower)
                || (u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(lower)));
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
                u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name == role));
        }

        // USER TYPE FILTER
        if (!string.IsNullOrWhiteSpace(userType))
        {
            if (userType == "Student")
            {
                query = query.Where(u =>
                    u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name == "Student"));
            }
            else if (userType == "Employee")
            {
                query = query.Where(u => u.EmployeeId != null);
            }
            else if (userType == "Guardian")
            {
                var guardianUserIds = await _unitOfWork.Repository<GdnEntity>().Query().AsNoTracking()
                    .Where(g => g.UserId != null && !g.IsDeleted)
                    .Select(g => g.UserId!.Value)
                    .ToListAsync(ct);
                query = query.Where(u => guardianUserIds.Contains(u.Id));
            }
            // "System" type = users with no link to Employee, Guardian, or Student role
        }

        var totalCount = await query.CountAsync(ct);

        // Dynamic sort
        var isDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        query = sortColumn?.ToLower() switch
        {
            "username" => isDesc ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName),
            "email" => isDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "phonenumber" => isDesc ? query.OrderByDescending(u => u.PhoneNumber ?? "") : query.OrderBy(u => u.PhoneNumber ?? ""),
            "status" => isDesc ? query.OrderByDescending(u => u.Status) : query.OrderBy(u => u.Status),
            _ => isDesc ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id)
        };

        var users = await query
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        // Fetch linked entity data for Employee and Guardian lookups
        var userIds = users.Select(u => u.Id).ToList();

        var employeeMap = await _unitOfWork.Repository<EmpEntity>().Query().AsNoTracking()
            .Where(e => e.UserId.HasValue && userIds.Contains(e.UserId.Value) && !e.IsDeleted)
            .Select(e => new { e.UserId, e.FullName, e.IsTeachingStaff, e.ProfilePicturePath })
            .ToDictionaryAsync(e => e.UserId!.Value, ct);

        var guardianMap = await _unitOfWork.Repository<GdnEntity>().Query().AsNoTracking()
            .Where(g => g.UserId.HasValue && userIds.Contains(g.UserId.Value) && !g.IsDeleted)
            .Select(g => new { g.UserId, g.FullName, PhotoPath = g.PhotoPath ?? "" })
            .ToDictionaryAsync(g => g.UserId!.Value, ct);

        var items = users.Select(u =>
        {
            string userType;
            string linkedEntityName;
            bool? isTeachingStaff = null;
            string? profilePicturePath = null;

            if (employeeMap.TryGetValue(u.Id, out var emp))
            {
                userType = "Employee";
                linkedEntityName = emp.FullName;
                isTeachingStaff = emp.IsTeachingStaff;
                profilePicturePath = emp.ProfilePicturePath;
            }
            else if (guardianMap.TryGetValue(u.Id, out var gdn))
            {
                userType = "Guardian";
                linkedEntityName = gdn.FullName;
                profilePicturePath = gdn.PhotoPath;
            }
            else if (u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name == "Student"))
            {
                userType = "Student";
                linkedEntityName = "—";
            }
            else
            {
                userType = "System";
                linkedEntityName = "—";
            }

            return new UserListItemVm
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Status = u.Status,
                IsDeleted = u.IsDeleted,
                UserType = userType,
                LinkedEntityName = linkedEntityName,
                IsTeachingStaff = isTeachingStaff,
                ProfilePicturePath = profilePicturePath,
                RolesText = string.Join(", ",
                    u.UserRoles.Where(ur => ur.Role != null)
                               .Select(ur => ur.Role!.Name)
                               .Distinct()
                               .OrderBy(name => name))
            };
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

        var selectedRoleIds = await _unitOfWork.Repository<UserRole>().Query().AsNoTracking()
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
        var user = await _unitOfWork.Repository<ApplicationUser>().Query().AsNoTracking()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, ct);

        if (user == null) return null;

        // Resolve user type
        string userType = "System";
        string linkedEntityName = "—";

        var emp = await _unitOfWork.Repository<EmpEntity>().Query().AsNoTracking()
            .Where(e => e.UserId == id && !e.IsDeleted)
            .Select(e => new { e.FullName })
            .FirstOrDefaultAsync(ct);

        if (emp != null)
        {
            userType = "Employee";
            linkedEntityName = emp.FullName;
        }
        else
        {
            var gdn = await _unitOfWork.Repository<GdnEntity>().Query().AsNoTracking()
                .Where(g => g.UserId == id && !g.IsDeleted)
                .Select(g => new { g.FullName })
                .FirstOrDefaultAsync(ct);

            if (gdn != null)
            {
                userType = "Guardian";
                linkedEntityName = gdn.FullName;
            }
            else if (user.UserRoles.Any(ur => ur.Role != null && ur.Role.Name == "Student"))
            {
                userType = "Student";
            }
        }

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
            Roles = user.UserRoles.Where(ur => ur.Role != null).Select(ur => ur.Role!.Name).ToList(),
            UserType = userType,
            LinkedEntityName = linkedEntityName
        };
    }

    public async Task<int> CreateAsync(UserUpsertViewModel model, string createdBy, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(model.Password))
            throw new InvalidOperationException("Password is required when creating a new user.");

        ValidatePasswordComplexity(model.Password);

        var repo = _unitOfWork.Repository<ApplicationUser>();
        if (await repo.AnyAsync(u => u.UserName == model.UserName.Trim() && !u.IsDeleted, ct))
            throw new InvalidOperationException("Username is already taken.");

        if (await repo.AnyAsync(u => u.Email == model.Email.Trim() && !u.IsDeleted, ct))
            throw new InvalidOperationException("Email is already in use by another user.");

        var user = new ApplicationUser
        {
            UserName = model.UserName.Trim(),
            Email = model.Email.Trim(),
            PhoneNumber = model.PhoneNumber?.Trim(),
            Status = model.Status,
            PasswordHash = _passwordHashService.HashPassword(model.Password),
            IsEmailConfirmed = model.Status == AccountStatus.Active,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        if (model.SelectedRoleIds != null && model.SelectedRoleIds.Count > 0)
            await AssignRolesAsync(user.Id, model.SelectedRoleIds, null, ct);

        return user.Id;
    }

    public async Task UpdateAsync(UserUpsertViewModel model, string updatedBy, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<ApplicationUser>();
        var user = await repo.FirstOrDefaultAsync(u => u.Id == model.Id && !u.IsDeleted, ct) 
            ?? throw new InvalidOperationException("User not found.");

        if (await repo.AnyAsync(u => u.Id != model.Id && u.UserName == model.UserName.Trim() && !u.IsDeleted, ct))
            throw new InvalidOperationException("Username is already taken.");

        if (await repo.AnyAsync(u => u.Id != model.Id && u.Email == model.Email.Trim() && !u.IsDeleted, ct))
            throw new InvalidOperationException("Email is already in use by another user.");

        user.UserName = model.UserName.Trim();
        user.Email = model.Email.Trim();
        user.PhoneNumber = model.PhoneNumber?.Trim();
        user.Status = model.Status;
        user.UpdatedBy = updatedBy;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            ValidatePasswordComplexity(model.Password);
            user.PasswordHash = _passwordHashService.HashPassword(model.Password);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        if (model.SelectedRoleIds != null)
            await AssignRolesAsync(user.Id, model.SelectedRoleIds, null, ct);
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

    public async Task AssignRolesAsync(int userId, IEnumerable<int> roleIds, int? performedByUserId = null, CancellationToken ct = default)
    {
        if (performedByUserId.HasValue)
        {
            var performerRoleNames = await _unitOfWork.Repository<UserRole>().Query().AsNoTracking()
                .Where(ur => ur.UserId == performedByUserId.Value && ur.Role != null)
                .Select(ur => ur.Role!.Name)
                .ToListAsync(ct);
            var isSuperAdmin = performerRoleNames.Any(r => r == "Super Admin");

            if (!isSuperAdmin)
            {
                var targetRoleNames = await _unitOfWork.Repository<Role>().Query().AsNoTracking()
                    .Where(r => roleIds.Contains(r.Id))
                    .Select(r => r.Name)
                    .ToListAsync(ct);
                if (targetRoleNames.Any(r => r == "Super Admin" || r == "Admin"))
                    throw new InvalidOperationException("Only Super Admin can assign Super Admin or Admin roles.");

                if (userId == performedByUserId.Value)
                    throw new InvalidOperationException("Cannot modify your own roles.");
            }
        }
        else
        {
            var targetRoleNames = await _unitOfWork.Repository<Role>().Query().AsNoTracking()
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToListAsync(ct);
            if (targetRoleNames.Any(r => r == "Super Admin"))
                throw new InvalidOperationException("Only Super Admin can assign Super Admin role.");
        }

        var urRepo = _unitOfWork.Repository<UserRole>();
        var existing = await urRepo.ListAsync(ur => ur.UserId == userId);
        var existingIds = existing.Select(e => e.RoleId).ToHashSet();
        var incomingIds = roleIds.Distinct().ToHashSet();

        // Remove roles no longer selected
        foreach (var ex in existing.Where(e => !incomingIds.Contains(e.RoleId)))
            urRepo.Remove(ex);

        // Add new roles not previously assigned
        foreach (var roleId in incomingIds.Where(id => !existingIds.Contains(id)))
        {
            await urRepo.AddAsync(new UserRole { UserId = userId, RoleId = roleId }, ct);
        }

        if (existing.Any(e => !incomingIds.Contains(e.RoleId)) || incomingIds.Any(id => !existingIds.Contains(id)))
            await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<List<int>> GetAssignedRoleIdsAsync(int userId, CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<UserRole>().Query().AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(ct);
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

    private static void ValidatePasswordComplexity(string password)
    {
        if (password.Length < 8)
            throw new InvalidOperationException("Password must be at least 8 characters long.");
        if (!password.Any(char.IsUpper))
            throw new InvalidOperationException("Password must contain at least one uppercase letter.");
        if (!password.Any(char.IsLower))
            throw new InvalidOperationException("Password must contain at least one lowercase letter.");
        if (!password.Any(char.IsDigit))
            throw new InvalidOperationException("Password must contain at least one digit.");
    }
}

