using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Employee;

public class EmployeeBankAccountDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }

    [Required, MaxLength(100), Display(Name = "Bank Name")]
    public string BankName { get; set; } = string.Empty;

    [Required, MaxLength(50), Display(Name = "Branch Name")]
    public string BranchName { get; set; } = string.Empty;

    [Required, MaxLength(50), Display(Name = "Account Number")]
    public string AccountNumber { get; set; } = string.Empty;

    [MaxLength(50), Display(Name = "Routing Number")]
    public string? RoutingNumber { get; set; }

    [MaxLength(50), Display(Name = "Account Type")]
    public string? AccountType { get; set; }

    [Display(Name = "Default Account")]
    public bool IsDefault { get; set; }

    public bool IsActive { get; set; } = true;
}

public class EmployeePromotionDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }

    [Display(Name = "Previous Designation")]
    public int PreviousDesignationId { get; set; }
    public string? PreviousDesignation { get; set; }

    [Display(Name = "New Designation")]
    public int NewDesignationId { get; set; }
    public string? NewDesignation { get; set; }

    public string? Reason { get; set; }

    [Display(Name = "Promotion Date")]
    public DateTime PromotionDate { get; set; }

    [Display(Name = "Previous Salary")]
    public decimal? PreviousSalary { get; set; }

    [Display(Name = "New Salary")]
    public decimal? NewSalary { get; set; }

    public string? Remarks { get; set; }
}

public class EmployeeTransferDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }

    [Display(Name = "From Department")]
    public int FromDepartmentId { get; set; }
    public string? FromDepartment { get; set; }

    [Display(Name = "To Department")]
    public int ToDepartmentId { get; set; }
    public string? ToDepartment { get; set; }

    public string? Reason { get; set; }

    [Display(Name = "Transfer Date")]
    public DateTime TransferDate { get; set; }

    public string? Remarks { get; set; }
}

public class EmployeeTrainingDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }

    [Required, MaxLength(200), Display(Name = "Training Name")]
    public string TrainingName { get; set; } = string.Empty;

    [MaxLength(200), Display(Name = "Institution")]
    public string? InstitutionName { get; set; }

    public string? Duration { get; set; }

    [Display(Name = "Start Date")]
    public DateTime? StartDate { get; set; }

    [Display(Name = "End Date")]
    public DateTime? EndDate { get; set; }

    public string? CertificatePath { get; set; }
    public IFormFile? CertificateFile { get; set; }

    public string? Remarks { get; set; }
}

public class EmployeeAwardDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }

    [Required, MaxLength(200), Display(Name = "Award Name")]
    public string AwardName { get; set; } = string.Empty;

    [MaxLength(200), Display(Name = "Awarded By")]
    public string? AwardedBy { get; set; }

    [Display(Name = "Award Date")]
    public DateTime AwardDate { get; set; }

    public string? Description { get; set; }

    public string? CertificatePath { get; set; }
    public IFormFile? CertificateFile { get; set; }
}

public class EmployeeDashboardDto
{
    public int TotalEmployees { get; set; }
    public int TeachingStaff { get; set; }
    public int NonTeachingStaff { get; set; }
    public int ActiveEmployees { get; set; }
    public int InactiveEmployees { get; set; }
    public int OnLeaveEmployees { get; set; }
    public int ResignedEmployees { get; set; }
    public int RetiredEmployees { get; set; }
    public int NewHiresThisYear { get; set; }
    public int BirthdaysThisMonth { get; set; }
    public List<DepartmentStat> DepartmentStats { get; set; } = new();
    public List<StatusStat> StatusStats { get; set; } = new();
    public List<RecentHireDto> RecentHires { get; set; } = new();
    public List<BirthdayDto> UpcomingBirthdays { get; set; } = new();
}

public class DepartmentStat
{
    public string DepartmentName { get; set; } = string.Empty;
    public int Count { get; set; }
    public int TeachingCount { get; set; }
    public int NonTeachingCount { get; set; }
}

public class StatusStat
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class RecentHireDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; }
    public string? ProfilePicturePath { get; set; }
}

public class BirthdayDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? ProfilePicturePath { get; set; }
}

public class EmployeeDisciplinaryActionDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }

    [Required, MaxLength(200), Display(Name = "Action Type")]
    public string ActionType { get; set; } = string.Empty;

    public string? Reason { get; set; }

    [Display(Name = "Action Date")]
    public DateTime ActionDate { get; set; }

    public string? Description { get; set; }

    public string? DocumentPath { get; set; }
    public IFormFile? DocumentFile { get; set; }

    [Display(Name = "Is Resolved")]
    public bool IsResolved { get; set; }

    [Display(Name = "Resolved At")]
    public DateTime? ResolvedAt { get; set; }

    [Display(Name = "Resolution Remarks")]
    public string? ResolutionRemarks { get; set; }
}
