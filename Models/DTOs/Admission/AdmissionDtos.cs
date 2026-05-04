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
        public string? PassportNo { get; set; }
        public string? NationalIdNo { get; set; }
        public string? BirthCertificateNo { get; set; }
        public string? ProfilePicturePath { get; set; }
        public IFormFile? ProfilePicture { get; set; }

        // Payment
        public string? PaymentMethod { get; set; }
        public string? TransactionDetails { get; set; }

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
