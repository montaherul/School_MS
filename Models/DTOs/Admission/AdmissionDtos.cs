using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Admission;

public class AdmissionCreateDto
{
        [Required, MaxLength(120)]
        public string ApplicantName { get; set; } = string.Empty;

        [MaxLength(120)]
        public string? ApplicantNameBangla { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required, MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        // Father
        [Required, MaxLength(120)]
        public string FatherName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FatherOccupation { get; set; }

        // Mother
        [Required, MaxLength(120)]
        public string MotherName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? MotherOccupation { get; set; }

        // Guardian
        [MaxLength(120)]
        public string? GuardianName { get; set; }

        [MaxLength(100)]
        public string? GuardianOccupation { get; set; }

        // Contact
        [Required, MaxLength(30)]
        public string FatherOrGuardianMobileNo { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string ApplicantMobileNumber { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? AlternativeNumber { get; set; }

        [EmailAddress]
        public string? ApplicantEmail { get; set; }

        // Identity
        public string Nationality { get; set; } = "Bangladeshi";
        public string Country { get; set; } = "Bangladesh";
        public string MaritalStatus { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public string? BloodGroup { get; set; }
        public string? BirthCertificateNo { get; set; }
        public string? BirthCertificatePath { get; set; }
        public IFormFile? BirthCertificateFile { get; set; }
        public string? ProfilePicturePath { get; set; }
        public IFormFile? ProfilePicture { get; set; }

        // Payment
        public string? PaymentMethod { get; set; }
        public string? TransactionDetails { get; set; }
        public string? PaymentSlipPath { get; set; }
        public IFormFile? PaymentSlipFile { get; set; }

        public int AppliedClassId { get; set; }
        // Present
        public string? PresentVillage { get; set; }
        public string? PresentPostOffice { get; set; }
        public string? PresentThana { get; set; }
        public string? PresentDistrict { get; set; }

        // Permanent
        public string? PermanentVillage { get; set; }
        public string? PermanentPostOffice { get; set; }
        public string? PermanentThana { get; set; }
        public string? PermanentDistrict { get; set; }
}

public class AdmissionDecisionDto
{
    public int ApplicationId { get; set; }
    public AdmissionStatus Status { get; set; }
}

public class AdmissionApproveRequest
{
    public int Id { get; set; }
    public int SectionId { get; set; }
}

/// <summary>
/// DTO for Admission List results from sp_GetAdmissionList stored procedure
/// </summary>
public class AdmissionListResultDto
{
    public int Id { get; set; }
<<<<<<< HEAD
    public string ApplicationNo { get; set; }
    public string ApplicantName { get; set; }
    public string? ApplicantNameBangla { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public int AppliedClassId { get; set; }
    public string ClassName { get; set; }
    public string ApplicantMobileNumber { get; set; }
    public string FatherOrGuardianMobileNo { get; set; }
    public string AlternativeNumber { get; set; }
    public string ApplicantEmail { get; set; }
    public string Status { get; set; }
    public string FatherName { get; set; }
    public string FatherOccupation { get; set; }
    public string MotherName { get; set; }
    public string MotherOccupation { get; set; }
    public string GuardianName { get; set; }
    public string GuardianOccupation { get; set; }
    public string Nationality { get; set; }
    public string Religion { get; set; }
    public string BloodGroup { get; set; }
    public string BirthCertificateNo { get; set; }
    public string BirthCertificatePath { get; set; }
    public string PaymentSlipPath { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionDetails { get; set; }
    public string PresentVillage { get; set; }
    public string PresentPostOffice { get; set; }
    public string PresentThana { get; set; }
    public string PresentDistrict { get; set; }
    public string PermanentVillage { get; set; }
    public string PermanentPostOffice { get; set; }
    public string PermanentThana { get; set; }
    public string PermanentDistrict { get; set; }
    public string ProfilePicturePath { get; set; }
=======
    public string ApplicationNo { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string? ApplicantNameBangla { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public int AppliedClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string ApplicantMobileNumber { get; set; } = string.Empty;
    public string FatherOrGuardianMobileNo { get; set; } = string.Empty;
    public string? AlternativeNumber { get; set; }
    public string? ApplicantEmail { get; set; }
    public string Status { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string? FatherOccupation { get; set; }
    public string MotherName { get; set; } = string.Empty;
    public string? MotherOccupation { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianOccupation { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public string Religion { get; set; } = string.Empty;
    public string? BloodGroup { get; set; }
    public string? BirthCertificateNo { get; set; }
    public string? BirthCertificatePath { get; set; }
    public string? PaymentSlipPath { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionDetails { get; set; }
    public string? PresentVillage { get; set; }
    public string? PresentPostOffice { get; set; }
    public string? PresentThana { get; set; }
    public string? PresentDistrict { get; set; }
    public string? PermanentVillage { get; set; }
    public string? PermanentPostOffice { get; set; }
    public string? PermanentThana { get; set; }
    public string? PermanentDistrict { get; set; }
    public string? ProfilePicturePath { get; set; }
>>>>>>> d8b24e6 (attendece and website curtomize)
    public int TotalRecords { get; set; }

    // Computed properties for UI
    public int Age => DateTime.Today.Year - DateOfBirth.Year;
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedAtFormatted => CreatedAt.ToString("dd-MMM-yyyy");
    public int DaysApplied => (DateTime.Today - CreatedAt).Days;

    public string StatusBadgeClass => Status switch
    {
        "Approved" => "badge-success",
        "Rejected" => "badge-danger",
        "Under Review" => "badge-warning",
        _ => "badge-secondary"
    };
}
