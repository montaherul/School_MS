using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.ViewModels.User;

public class RoleOptionVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UserListItemVm
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public AccountStatus Status { get; set; }
    public string RolesText { get; set; } = string.Empty;
}

public class UserIndexViewModel
{
    public IReadOnlyList<UserListItemVm> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
    public string? Search { get; set; }
}

public class UserUpsertViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "User name")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone")]
    public string? PhoneNumber { get; set; }

    [Required]
    public AccountStatus Status { get; set; } = AccountStatus.Active;

    // Admin-controlled raw password (optional on edit).
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    public string? ConfirmPassword { get; set; }

    public List<int> SelectedRoleIds { get; set; } = [];

    public IReadOnlyList<RoleOptionVm> AvailableRoles { get; set; } = [];
}

public class UserDetailsViewModel
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public AccountStatus Status { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];
}

public class AssignRolesViewModel
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    public List<int> SelectedRoleIds { get; set; } = [];
    public IReadOnlyList<RoleOptionVm> AvailableRoles { get; set; } = [];
}

