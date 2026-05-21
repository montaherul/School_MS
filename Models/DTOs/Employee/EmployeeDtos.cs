using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Employee;

public class EmployeeListItemDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Status { get; set; } = "Active";
    public bool IsTeachingStaff { get; set; }
    public DateTime JoiningDate { get; set; }
    public string? ProfilePicturePath { get; set; }
}

public class EmployeeUpsertDto
{
    public int Id { get; set; }

    [Display(Name = "Employee Code")]
    public string? EmployeeCode { get; set; }

    [Required, MaxLength(120), Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(120), Display(Name = "Father's Name")]
    public string? FatherName { get; set; }

    [MaxLength(120), Display(Name = "Mother's Name")]
    public string? MotherName { get; set; }

    [Required, MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    [Required, Display(Name = "Date of Birth")]
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-25);

    [MaxLength(10), Display(Name = "Blood Group")]
    public string? BloodGroup { get; set; }

    [MaxLength(50)]
    public string? Religion { get; set; }

    [Required, MaxLength(50)]
    public string Nationality { get; set; } = "Bangladeshi";

    [MaxLength(50), Display(Name = "NID Number")]
    public string? NIDNumber { get; set; }

    [MaxLength(50), Display(Name = "Birth Certificate No")]
    public string? BirthCertificateNo { get; set; }

    [Required, Phone, MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress, MaxLength(160)]
    public string? Email { get; set; }

    [MaxLength(500), Display(Name = "Present Address")]
    public string? PresentAddress { get; set; }

    [MaxLength(500), Display(Name = "Permanent Address")]
    public string? PermanentAddress { get; set; }

    [Required, Display(Name = "Joining Date")]
    public DateTime JoiningDate { get; set; } = DateTime.Today;

    [Required, Display(Name = "Department")]
    public int DepartmentId { get; set; }

    [Required, Display(Name = "Designation")]
    public int DesignationId { get; set; }

    [Required, MaxLength(50), Display(Name = "Employee Type")]
    public string EmployeeType { get; set; } = "Full-Time";

    [Display(Name = "Is Teaching Staff?")]
    public bool IsTeachingStaff { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Active";

    [Display(Name = "Profile Picture")]
    public string? ProfilePicturePath { get; set; }
    public IFormFile? ProfilePictureFile { get; set; }

    [Display(Name = "Signature")]
    public string? SignaturePath { get; set; }
    public IFormFile? SignatureFile { get; set; }

    [MaxLength(120), Display(Name = "Emergency Contact Name")]
    public string? EmergencyContactName { get; set; }

    [MaxLength(30), Display(Name = "Emergency Contact Phone")]
    public string? EmergencyContactPhone { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public List<EmployeeQualificationDto> Qualifications { get; set; } = new();
    public List<EmployeeDocumentDto> Documents { get; set; } = new();
    public List<EmployeeExperienceDto> Experiences { get; set; } = new();
}

public class EmployeeDetailsDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? BloodGroup { get; set; }
    public string? Religion { get; set; }
    public string Nationality { get; set; } = "Bangladeshi";
    public string? NIDNumber { get; set; }
    public string? BirthCertificateNo { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PresentAddress { get; set; }
    public string? PermanentAddress { get; set; }
    public DateTime JoiningDate { get; set; }
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string EmployeeType { get; set; } = "Full-Time";
    public bool IsTeachingStaff { get; set; }
    public string Status { get; set; } = "Active";
    public string? ProfilePicturePath { get; set; }
    public string? SignaturePath { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Remarks { get; set; }
    public string? Username { get; set; }

    public List<EmployeeQualificationDto> Qualifications { get; set; } = new();
    public List<EmployeeDocumentDto> Documents { get; set; } = new();
    public List<EmployeeExperienceDto> Experiences { get; set; } = new();
}

public class EmployeeQualificationDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }

    [Required, MaxLength(100), Display(Name = "Exam Name")]
    public string ExamName { get; set; } = string.Empty;

    [MaxLength(150), Display(Name = "Board / University")]
    public string? BoardOrUniversity { get; set; }

    [MaxLength(150), Display(Name = "Institute Name")]
    public string? InstituteName { get; set; }

    [MaxLength(100), Display(Name = "Group / Subject")]
    public string? GroupOrSubject { get; set; }

    [MaxLength(10), Display(Name = "Passing Year")]
    public string? PassingYear { get; set; }

    [MaxLength(50)]
    public string? Result { get; set; }

    [MaxLength(50), Display(Name = "CGPA / Division")]
    public string? CGPAOrDivision { get; set; }

    public string? CertificateFilePath { get; set; }
    public IFormFile? CertificateFile { get; set; }
}

public class EmployeeDocumentDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }

    [Required, MaxLength(50), Display(Name = "Document Type")]
    public string DocumentType { get; set; } = string.Empty;

    [Required, MaxLength(150), Display(Name = "Document Name")]
    public string DocumentName { get; set; } = string.Empty;

    public string? FilePath { get; set; }
    public IFormFile? DocumentFile { get; set; }

    [Display(Name = "Expiry Date")]
    public DateTime? ExpiryDate { get; set; }

    [MaxLength(255)]
    public string? Remarks { get; set; }
}

public class EmployeeExperienceDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }

    [Required, MaxLength(150), Display(Name = "Organization")]
    public string OrganizationName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Designation { get; set; } = string.Empty;

    [Required, Display(Name = "Start Date")]
    public DateTime StartDate { get; set; }

    [Display(Name = "End Date")]
    public DateTime? EndDate { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }
}

public class DesignationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RoleLevel { get; set; }
    public bool IsTeachingRole { get; set; }
    public bool IsActive { get; set; }
}

public class DepartmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
