using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Admission;

public class AdmissionApplication : BaseEntity
{
    [Required]
    [MaxLength(30)]
    public string ApplicationNo { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string ApplicantName { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? ApplicantNameBangla { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    // 🔹 Father
    [Required]
    [MaxLength(120)]
    public string FatherName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? FatherOccupation { get; set; }

    // 🔹 Mother
    [Required]
    [MaxLength(120)]
    public string MotherName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? MotherOccupation { get; set; }

    // 🔹 Guardian
    [MaxLength(120)]
    public string? GuardianName { get; set; }

    [MaxLength(100)]
    public string? GuardianOccupation { get; set; }

    // 🔹 Contact
    [Required]
    [MaxLength(30)]
    public string FatherOrGuardianMobileNo { get; set; } = string.Empty;

    [MaxLength(30)]
    public string ApplicantMobileNumber { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? AlternativeNumber { get; set; }

    [EmailAddress]
    [MaxLength(160)]
    public string? ApplicantEmail { get; set; }

    // 🔹 Identity
    [Required]
    [MaxLength(50)]
    public string Nationality { get; set; } = "Bangladeshi";

    [Required]
    [MaxLength(50)]
    public string Country { get; set; } = "Bangladesh";

    [Required]
    [MaxLength(30)]
    public string MaritalStatus { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Religion { get; set; } = string.Empty;

    [MaxLength(10)]
    public string? BloodGroup { get; set; }

    [MaxLength(50)]
    public string? BirthCertificateNo { get; set; }

    [MaxLength(260)]
    public string? BirthCertificatePath { get; set; }

    // 🔹 Payment
    [MaxLength(50)]
    public string? PaymentMethod { get; set; }

    [MaxLength(250)]
    public string? TransactionDetails { get; set; }

    [MaxLength(260)]
    public string? PaymentSlipPath { get; set; }

    // 🔹 Present Address
    [MaxLength(150)]
    public string? PresentVillage { get; set; }

    [MaxLength(150)]
    public string? PresentPostOffice { get; set; }

    [MaxLength(150)]
    public string? PresentThana { get; set; }

    [MaxLength(100)]
    public string? PresentDistrict { get; set; }

    // 🔹 Permanent Address
    [MaxLength(150)]
    public string? PermanentVillage { get; set; }

    [MaxLength(150)]
    public string? PermanentPostOffice { get; set; }

    [MaxLength(150)]
    public string? PermanentThana { get; set; }

    [MaxLength(100)]
    public string? PermanentDistrict { get; set; }

    // 🔹 Admission Info
    [Required]
    public int AppliedClassId { get; set; }

    [Required]
    public AdmissionStatus Status { get; set; } = AdmissionStatus.Pending;

    [Required]
    public decimal AdmissionFee { get; set; }

    [Required]
    public bool AdmissionFeePaid { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public int? ReviewedByUserId { get; set; }

    [MaxLength(260)]
    public string? ProfilePicturePath { get; set; }

    public ICollection<AdmissionDocument> Documents { get; set; } = new List<AdmissionDocument>();
}

public class AdmissionDocument : BaseEntity
{
    [Required]
    public int AdmissionApplicationId { get; set; }

    public AdmissionApplication? AdmissionApplication { get; set; }

    [Required]
    [MaxLength(80)]
    public string DocumentType { get; set; } = string.Empty;

    [Required]
    [MaxLength(260)]
    public string FilePath { get; set; } = string.Empty;
}