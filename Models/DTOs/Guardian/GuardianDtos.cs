using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Guardian;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Guardian;

public class GuardianListItemDto
{
    public int Id { get; set; }
    public string GuardianCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string RelationType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ChildrenCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GuardianDetailsDto
{
    public int Id { get; set; }
    public string GuardianCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string RelationType { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? PassportNumber { get; set; }
    public string? Occupation { get; set; }
    public string? EmployerName { get; set; }
    public decimal? MonthlyIncome { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string? AlternativeMobileNumber { get; set; }
    public string? Email { get; set; }
    public string? PresentAddress { get; set; }
    public string? PermanentAddress { get; set; }
    public string? PhotoPath { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactNumber { get; set; }
    public bool PortalAccessEnabled { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public List<GuardianChildDto> Children { get; set; } = new();
}

public class GuardianChildDto
{
    public int StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
    public double AttendancePercentage { get; set; }
    public decimal? GPA { get; set; }
    public string FeeStatus { get; set; } = string.Empty;
}

public class GuardianProfileUpdateDto
{
    public int Id { get; set; }

    [MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(80)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(160)]
    public string? Email { get; set; }

    [MaxLength(30)]
    public string? MobileNumber { get; set; }

    [MaxLength(50)]
    public string? NationalId { get; set; }

    [MaxLength(100)]
    public string? Occupation { get; set; }

    [MaxLength(250)]
    public string? PresentAddress { get; set; }

    [MaxLength(250)]
    public string? PermanentAddress { get; set; }

    [MaxLength(100)]
    public string? EmergencyContactName { get; set; }

    [MaxLength(30)]
    public string? EmergencyContactNumber { get; set; }

    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }

    public IFormFile? PhotoFile { get; set; }
}

public class GuardianUpsertDto
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public GuardianRelationshipType RelationType { get; set; }

    [Required]
    [MaxLength(30)]
    public string MobileNumber { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(160)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? NationalId { get; set; }

    public string? Occupation { get; set; }
    public string? PresentAddress { get; set; }
    public string? PermanentAddress { get; set; }
    
    public bool PortalAccessEnabled { get; set; } = true;
}

public class GuardianDashboardDataDto
{
    public int TotalChildren { get; set; }
    public decimal TotalOutstandingFees { get; set; }
    public int UnreadNotifications { get; set; }
    public List<GuardianChildAttendanceSummaryDto> ChildrenAttendance { get; set; } = new();
    public List<GuardianRecentNoticeDto> RecentNotices { get; set; } = new();
}

public class GuardianChildAttendanceSummaryDto
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int TotalDays { get; set; }
    public double AttendancePercentage => TotalDays > 0 ? (double)(TotalDays - AbsentCount) / TotalDays * 100 : 0;
}

public class GuardianRecentNoticeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Excerpt { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
