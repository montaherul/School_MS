using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolManagementSystem.Models.ViewModels.Employee;

public class EmployeeViewModel
{
    public long Id { get; set; }

    [Required]
    [Display(Name = "Employee Code")]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    [Display(Name = "Blood Group")]
    public string? BloodGroup { get; set; }

    public string Nationality { get; set; } = "Bangladeshi";

    // Present Address
    [Display(Name = "Present Village")]
    public string? PresentVillage { get; set; }

    [Display(Name = "Present Post Office")]
    public string? PresentPostOffice { get; set; }

    [Display(Name = "Present Thana")]
    public string? PresentThana { get; set; }

    [Display(Name = "Present District")]
    public string? PresentDistrict { get; set; }

    // Permanent Address
    [Display(Name = "Permanent Village")]
    public string? PermanentVillage { get; set; }

    [Display(Name = "Permanent Post Office")]
    public string? PermanentPostOffice { get; set; }

    [Display(Name = "Permanent Thana")]
    public string? PermanentThana { get; set; }

    [Display(Name = "Permanent District")]
    public string? PermanentDistrict { get; set; }

    [Required]
    [Display(Name = "Joining Date")]
    [DataType(DataType.Date)]
    public DateTime JoiningDate { get; set; } = DateTime.Today;

    [Required]
    [Range(0, 9999999.99)]
    public decimal Salary { get; set; }

    [Display(Name = "Photo")]
    public string? PhotoPath { get; set; }

    /// <summary>Upload file from the form — not persisted directly, saved via service.</summary>
    [Display(Name = "Upload Photo")]
    public IFormFile? PhotoFile { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Required]
    [Display(Name = "Department")]
    public long DepartmentId { get; set; }

    [Required]
    [Display(Name = "Designation")]
    public long DesignationId { get; set; }
    public string? DepartmentName { get; set; }
    public string? DesignationName { get; set; }

    public IEnumerable<SelectListItem>? Departments { get; set; }
    public IEnumerable<SelectListItem>? Designations { get; set; }

    // System Access
    [Display(Name = "Create Login Account")]
    public bool CreateLoginAccount { get; set; }

    [Display(Name = "Username")]
    public string? Username { get; set; }

    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
        ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number and one special character.")]
    public string? Password { get; set; }

    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string? ConfirmPassword { get; set; }

    [Display(Name = "System Role")]
    public int? RoleId { get; set; }

    public string? RoleName { get; set; }
    public bool HasLoginAccount { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? AccountStatus { get; set; }

    public IEnumerable<SelectListItem>? Roles { get; set; }
}

public class DepartmentViewModel
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;
}

public class DesignationViewModel
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}
