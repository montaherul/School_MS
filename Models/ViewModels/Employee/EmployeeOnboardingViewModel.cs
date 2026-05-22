using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.ViewModels.Employee;

public class EmployeeOnboardingViewModel
{
    public string Token { get; set; } = string.Empty;

    // Locked Organization Fields (Admin Controlled)
    public string DepartmentName { get; set; } = string.Empty;
    public string DesignationName { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; }
    public string EmploymentType { get; set; } = string.Empty;
    public bool IsTeachingStaff { get; set; }

    // Personal Information
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string FatherName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string MotherName { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    public string? BloodGroup { get; set; }
    public string? Religion { get; set; }
    public string? Nationality { get; set; }

    [Required, StringLength(20)]
    public string NIDNumber { get; set; } = string.Empty;

    public string? BirthCertificateNo { get; set; }

    // Contact Information
    [Required, Phone]
    public string MobileNumber { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string PersonalEmail { get; set; } = string.Empty;

    [Required]
    public string PresentAddress { get; set; } = string.Empty;

    [Required]
    public string PermanentAddress { get; set; } = string.Empty;

    [Required]
    public string EmergencyContactName { get; set; } = string.Empty;

    [Required, Phone]
    public string EmergencyContactPhone { get; set; } = string.Empty;

    // Security
    [Required, MinLength(6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required, Compare("Password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

    // Files
    public IFormFile? ProfilePhoto { get; set; }
    public IFormFile? Signature { get; set; }

    // Multi-step collections
    public List<OnboardingQualificationViewModel> Qualifications { get; set; } = new();
    public List<OnboardingExperienceViewModel> Experiences { get; set; } = new();
}

public class OnboardingQualificationViewModel
{
    [Required]
    public string ExamName { get; set; } = string.Empty;
    [Required]
    public string BoardOrUniversity { get; set; } = string.Empty;
    [Required]
    public string InstituteName { get; set; } = string.Empty;
    public string? GroupOrSubject { get; set; }
    [Required]
    public string PassingYear { get; set; } = string.Empty;
    [Required]
    public string Result { get; set; } = string.Empty;
    public string? CGPAOrDivision { get; set; }
    public IFormFile? CertificateFile { get; set; }
}

public class OnboardingExperienceViewModel
{
    [Required]
    public string OrganizationName { get; set; } = string.Empty;
    [Required]
    public string Designation { get; set; } = string.Empty;
    [Required]
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Remarks { get; set; }
}
