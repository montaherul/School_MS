using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.User;

namespace SchoolManagementSystem.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly SchoolDbContext _db;
    private readonly IPasswordHashService _passwordHashService;

    public UserController(SchoolDbContext db, IPasswordHashService passwordHashService)
    {
        _db = db;
        _passwordHashService = passwordHashService;
    }

    [RequirePermission("Users.View")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);

        var query = _db.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => u.UserName.Contains(search) || u.Email.Contains(search));
        }

        var total = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderByDescending(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var userIds = users.Select(u => u.Id).ToArray();

        var dict = new Dictionary<int, List<string>>();
        if (userIds.Length > 0)
        {
            var roleRows = await _db.UserRoles
                .Where(ur => userIds.Contains(ur.UserId) && ur.Role != null)
                .Select(ur => new { ur.UserId, RoleName = ur.Role!.Name })
                .ToListAsync(cancellationToken);

            foreach (var item in roleRows)
            {
                if (!dict.TryGetValue(item.UserId, out var list))
                {
                    list = [];
                    dict[item.UserId] = list;
                }
                list.Add(item.RoleName);
            }
        }

        var model = new UserIndexViewModel
        {
            Items = users.Select(u => new UserListItemVm
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Status = u.Status,
                RolesText = dict.TryGetValue(u.Id, out var roles) ? string.Join(", ", roles) : string.Empty
            }).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            Search = search
        };

        return View(model);
    }

    [RequirePermission("Users.Create")]
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var roles = await LoadRolesAsync(cancellationToken);
        return View("CreateEdit", new UserUpsertViewModel
        {
            Id = 0,
            Status = AccountStatus.Active,
            AvailableRoles = roles
        });
    }

    [RequirePermission("Users.Edit")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        var roles = await LoadRolesAsync(cancellationToken);
        var selectedRoleIds = await _db.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);

        return View("CreateEdit", new UserUpsertViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Status = user.Status,
            SelectedRoleIds = selectedRoleIds,
            AvailableRoles = roles
        });
    }

    [RequirePermission("Users.Create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserUpsertViewModel model, CancellationToken cancellationToken)
    {
        return await Save(model, isEdit: false, cancellationToken);
    }

    [RequirePermission("Users.Edit")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserUpsertViewModel model, CancellationToken cancellationToken)
    {
        return await Save(model, isEdit: true, cancellationToken);
    }

    private async Task<IActionResult> Save(UserUpsertViewModel model, bool isEdit, CancellationToken cancellationToken)
    {
        var actor = User.Identity?.Name ?? "system";

        var roles = await LoadRolesAsync(cancellationToken);
        var selectedRoleIds = model.SelectedRoleIds?.Distinct().ToList() ?? [];

        if (isEdit)
        {
            if (model.Id <= 0)
            {
                return BadRequest();
            }
        }

        if (!isEdit && string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError(nameof(model.Password), "Password is required.");
        }

        if (!string.IsNullOrWhiteSpace(model.Password) || !string.IsNullOrWhiteSpace(model.ConfirmPassword))
        {
            if (!string.Equals(model.Password, model.ConfirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError(nameof(model.ConfirmPassword), "Passwords do not match.");
            }
        }

        if (!ModelState.IsValid)
        {
            model.AvailableRoles = roles;
            model.SelectedRoleIds = selectedRoleIds;
            return View("CreateEdit", model);
        }

        var existingUserName = model.UserName.Trim();
        var existingEmail = model.Email.Trim();

        if (isEdit)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == model.Id && !u.IsDeleted, cancellationToken);
            if (user is null) return NotFound();

            var usernameTaken = await _db.Users.AnyAsync(
                u => u.Id != user.Id && !u.IsDeleted && u.UserName == existingUserName,
                cancellationToken);
            if (usernameTaken)
            {
                ModelState.AddModelError(nameof(model.UserName), "User name is already taken.");
                model.AvailableRoles = roles;
                model.SelectedRoleIds = selectedRoleIds;
                return View("CreateEdit", model);
            }

            var emailTaken = await _db.Users.AnyAsync(
                u => u.Id != user.Id && !u.IsDeleted && u.Email == existingEmail,
                cancellationToken);
            if (emailTaken)
            {
                ModelState.AddModelError(nameof(model.Email), "Email is already taken.");
                model.AvailableRoles = roles;
                model.SelectedRoleIds = selectedRoleIds;
                return View("CreateEdit", model);
            }

            user.UserName = existingUserName;
            user.Email = existingEmail;
            user.PhoneNumber = model.PhoneNumber?.Trim();
            user.Status = model.Status;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.PasswordHash = _passwordHashService.HashPassword(model.Password);
            }

            await UpdateUserRolesAsync(user.Id, selectedRoleIds, cancellationToken);

            // Confirmation logic:
            // - Admin-created/edited Active users with a real password are considered confirmed.
            // - Pending/Inactive (or Active without a password hash) remain unconfirmed until activation/password reset.
            if (user.Status == AccountStatus.Active && !string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                user.IsEmailConfirmed = true;
                user.ActivationToken = null;
                user.ActivationTokenExpiry = null;
            }
            else
            {
                user.IsEmailConfirmed = false;
            }

            user.UpdatedBy = actor;
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(cancellationToken);
        }
        else
        {
            var usernameTaken = await _db.Users.AnyAsync(
                u => !u.IsDeleted && u.UserName == existingUserName,
                cancellationToken);
            if (usernameTaken)
            {
                ModelState.AddModelError(nameof(model.UserName), "User name is already taken.");
                model.AvailableRoles = roles;
                model.SelectedRoleIds = selectedRoleIds;
                return View("CreateEdit", model);
            }

            var emailTaken = await _db.Users.AnyAsync(
                u => !u.IsDeleted && u.Email == existingEmail,
                cancellationToken);
            if (emailTaken)
            {
                ModelState.AddModelError(nameof(model.Email), "Email is already taken.");
                model.AvailableRoles = roles;
                model.SelectedRoleIds = selectedRoleIds;
                return View("CreateEdit", model);
            }

            var user = new ApplicationUser
            {
                UserName = existingUserName,
                Email = existingEmail,
                PhoneNumber = model.PhoneNumber?.Trim(),
                Status = model.Status,
                PasswordHash = _passwordHashService.HashPassword(model.Password ?? string.Empty),
                IsEmailConfirmed = model.Status == AccountStatus.Active,
                ActivationToken = null,
                ActivationTokenExpiry = null,
                CreatedBy = actor,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            await UpdateUserRolesAsync(user.Id, selectedRoleIds, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
        }

        TempData["SuccessMessage"] = isEdit ? "User updated successfully." : "User created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("Users.View")]
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

        if (user is null) return NotFound();

        var roles = await _db.UserRoles
            .Where(ur => ur.UserId == user.Id && ur.Role != null)
            .Select(ur => ur.Role!.Name)
            .ToListAsync(cancellationToken);

        return View(new UserDetailsViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Status = user.Status,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            UpdatedBy = user.UpdatedBy,
            UpdatedAt = user.UpdatedAt,
            Roles = roles
        });
    }

    [RequirePermission("Users.Delete")]
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

        if (user is null) return NotFound();

        var roles = await _db.UserRoles
            .Where(ur => ur.UserId == user.Id && ur.Role != null)
            .Select(ur => ur.Role!.Name)
            .ToListAsync(cancellationToken);

        return View("Delete", new UserDetailsViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Status = user.Status,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            UpdatedBy = user.UpdatedBy,
            UpdatedAt = user.UpdatedAt,
            Roles = roles
        });
    }

    [RequirePermission("Users.Delete")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);
        if (user is null) return NotFound();

        var actor = User.Identity?.Name ?? "system";
        user.IsDeleted = true;
        user.Status = AccountStatus.Inactive;
        user.UpdatedBy = actor;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        TempData["SuccessMessage"] = "User deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("Users.Assign")]
    [HttpGet]
    public async Task<IActionResult> AssignRoles(int id, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

        if (user is null) return NotFound();

        var roles = await LoadRolesAsync(cancellationToken);

        var selectedRoleIds = await _db.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);

        return View(new AssignRolesViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            SelectedRoleIds = selectedRoleIds,
            AvailableRoles = roles
        });
    }

    [RequirePermission("Users.Assign")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRoles(int id, List<int> selectedRoleIds, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);
        if (user is null) return NotFound();

        var distinct = selectedRoleIds?.Distinct().ToList() ?? [];
        await UpdateUserRolesAsync(user.Id, distinct, cancellationToken);

        TempData["SuccessMessage"] = "Roles updated successfully.";
        return RedirectToAction(nameof(Details), new { id = user.Id });
    }

    private async Task<IReadOnlyList<RoleOptionVm>> LoadRolesAsync(CancellationToken cancellationToken)
    {
        return await _db.Roles
            .AsNoTracking()
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Name)
            .Select(r => new RoleOptionVm
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            })
            .ToListAsync(cancellationToken);
    }

    private async Task UpdateUserRolesAsync(int userId, IReadOnlyList<int> selectedRoleIds, CancellationToken cancellationToken)
    {
        // Replace join-table rows.
        var existing = _db.UserRoles.Where(ur => ur.UserId == userId);
        _db.UserRoles.RemoveRange(existing);

        if (selectedRoleIds.Count == 0)
        {
            await _db.SaveChangesAsync(cancellationToken);
            return;
        }

        var validRoleIds = await _db.Roles
            .Where(r => !r.IsDeleted && selectedRoleIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        foreach (var roleId in validRoleIds)
        {
            _db.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
