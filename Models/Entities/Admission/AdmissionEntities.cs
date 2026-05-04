using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Admission;

public class AdmissionApplication : BaseEntity
 {
    [MaxLength(30)]
            public string ApplicationNo { get; set; } = string.Empty;

        [MaxLength(120)]
        public string ApplicantName { get; set; } = string.Empty;

        [MaxLength(120)]
        public string? ApplicantNameBangla { get; set; }

        public DateTime DateOfBirth { get; set; }

        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        // 🔹 Father
        [MaxLength(120)]
        public string FatherName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FatherOccupation { get; set; }

        // 🔹 Mother
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
        [MaxLength(30)]
        public string FatherOrGuardianMobileNo { get; set; } = string.Empty;

        [MaxLength(30)]
        public string ApplicantMobileNumber { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? AlternativeNumber { get; set; }

        [MaxLength(160)]
        public string? ApplicantEmail { get; set; }

        // 🔹 Identity
        [MaxLength(50)]
        public string Nationality { get; set; } = "Bangladeshi";

        [MaxLength(50)]
        public string Country { get; set; } = "Bangladesh";

        [MaxLength(30)]
        public string MaritalStatus { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Religion { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? BloodGroup { get; set; }

        [MaxLength(50)]
        public string? PassportNo { get; set; }

        [MaxLength(50)]
        public string? NationalIdNo { get; set; }

        [MaxLength(50)]
        public string? BirthCertificateNo { get; set; }

        // 🔹 Payment
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        [MaxLength(250)]
        public string? TransactionDetails { get; set; }


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
         public int AppliedClassId { get; set; }

        public AdmissionStatus Status { get; set; } = AdmissionStatus.Pending;

        public decimal AdmissionFee { get; set; }

        public bool AdmissionFeePaid { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public int? ReviewedByUserId { get; set; }
        [MaxLength(260)]
        public string? ProfilePicturePath { get; set; }

    public ICollection<AdmissionDocument> Documents { get; set; } = new List<AdmissionDocument>();

}

public class AdmissionDocument : BaseEntity
{
    public int AdmissionApplicationId { get; set; }
    public AdmissionApplication? AdmissionApplication { get; set; }

    [MaxLength(80)]
    public string DocumentType { get; set; } = string.Empty;

    [MaxLength(260)]
    public string FilePath { get; set; } = string.Empty;
}
