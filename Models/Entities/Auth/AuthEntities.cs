using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Auth;

public class ApplicationUser : BaseEntity
{
    [MaxLength(80)]
    public string UserName { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? PhoneNumber { get; set; }

    [MaxLength(512)]
    public string PasswordHash { get; set; } = string.Empty;

    public AccountStatus Status { get; set; } = AccountStatus.Active;

    // Activation-based onboarding fields (student activation flow).
    public bool IsEmailConfirmed { get; set; } = false;

    [MaxLength(64)]
    public string? ActivationToken { get; set; }

    public DateTime? ActivationTokenExpiry { get; set; }

    public DateTime? LastLoginAt { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutUntil { get; set; }
    
    public long? EmployeeId { get; set; }
    public virtual SchoolManagementSystem.Models.Entities.Employee.Employee? Employee { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

}

public class Role : BaseEntity
{
    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class Permission : BaseEntity
{
    [MaxLength(80)]
    public string Module { get; set; } = string.Empty;

    [MaxLength(80)]
    public string ModuleName { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Code { get; set; } = string.Empty;

    public bool CanCreate { get; set; }
    public bool CanRead { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
}

public class UserRole
{
    public int UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public int RoleId { get; set; }
    public Role? Role { get; set; }
}

public class RolePermission
{
    public int RoleId { get; set; }
    public Role? Role { get; set; }
    public int PermissionId { get; set; }
    public Permission? Permission { get; set; }
}

public class PasswordResetToken : BaseEntity
{
    public int UserId { get; set; }
    public ApplicationUser? User { get; set; }

    [MaxLength(12)]
    public string Otp { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public bool Used { get; set; }
}

public class AuditLog : BaseEntity
{
    public int? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    [MaxLength(80)]
    public string Module { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(64)]
    public string? IpAddress { get; set; }

    [MaxLength(1000)]
    public string? Details { get; set; }
}

public class UserSession : BaseEntity
{
    public int UserId { get; set; }
    public ApplicationUser? User { get; set; }

    [MaxLength(64)]
    public string SessionId { get; set; } = string.Empty;

    public DateTime LoginAt { get; set; } = DateTime.UtcNow;
    public DateTime? LogoutAt { get; set; }

    [MaxLength(64)]
    public string? IpAddress { get; set; }

    [MaxLength(512)]
    public string? UserAgent { get; set; }

    public bool IsActive { get; set; } = true;
}

public class Notification : BaseEntity
{
    public int UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; }
    
    public bool IsRead { get; set; }
    
    [MaxLength(500)]
    public string? RedirectUrl { get; set; }
}
