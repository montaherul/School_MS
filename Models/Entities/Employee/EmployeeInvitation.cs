using SchoolManagementSystem.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Models.Entities.Employee;

public class EmployeeInvitation : BaseEntity
{
    [Required]
    [MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string InvitationCode { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Mobile { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string InvitationToken { get; set; } = string.Empty;

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int DesignationId { get; set; }
    public Designation? Designation { get; set; }

    public DateTime JoiningDate { get; set; }

    [MaxLength(50)]
    public string EmploymentType { get; set; } = "Full-Time";

    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    public bool IsTeachingStaff { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public DateTime ExpiresAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? OpenedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsUsed { get; set; }
    public bool IsApproved { get; set; }

    public DateTime? OnboardedAt { get; set; }

    public int? CreatedEmployeeId { get; set; }

    [MaxLength(50)]
    public string InvitationStatus { get; set; } = "Started"; // Started, Sent, Opened, Completed, Approved, Expired, Cancelled
}
