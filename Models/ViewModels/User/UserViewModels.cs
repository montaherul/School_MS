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
    public bool IsDeleted { get; set; }
    public string RolesText { get; set; } = string.Empty;

    /// <summary>User type: Employee, Guardian, Student, or System.</summary>
    public string UserType { get; set; } = "System";

    /// <summary>Display name of the linked entity (employee name, guardian name, etc.).</summary>
    public string LinkedEntityName { get; set; } = "—";
    
    /// <summary>Whether the linked employee is a teaching staff member.</summary>
    public bool? IsTeachingStaff { get; set; }

    /// <summary>Profile picture path from linked entity.</summary>
    public string? ProfilePicturePath { get; set; }
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

    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Password must include uppercase, lowercase, and a digit.")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
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
    public string StatusText => Status switch
    {
        AccountStatus.Active => "Active",
        AccountStatus.Inactive => "Inactive",
        AccountStatus.Locked => "Locked",
        AccountStatus.Pending => "Pending",
        _ => "Unknown"
    };
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];
    public string UserType { get; set; } = "System";
    public string LinkedEntityName { get; set; } = "—";
}

public class AssignRolesViewModel
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    public List<int> SelectedRoleIds { get; set; } = [];
    public IReadOnlyList<RoleOptionVm> AvailableRoles { get; set; } = [];
}

