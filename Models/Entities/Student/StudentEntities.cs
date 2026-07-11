using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Models.Entities.Student;

public class Student : BaseEntity
{
    [MaxLength(30)]
    public string StudentNo { get; set; } = string.Empty;

    [MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? FullNameBangla { get; set; }

    public DateTime DateOfBirth { get; set; }

    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    [MaxLength(120)]
    public string FatherName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? FatherOccupation { get; set; }

    [MaxLength(120)]
    public string MotherName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? MotherOccupation { get; set; }

    [MaxLength(30)]
    public string MobileNumber { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? AlternativeNumber { get; set; }

    [MaxLength(160)]
    public string? EmailAddress { get; set; }

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
    public string? BirthCertificateNo { get; set; }

    [MaxLength(260)]
    public string? ProfilePicturePath { get; set; }
    // Present
    [MaxLength(150)]
    public string? PresentVillage { get; set; }

    [MaxLength(150)]
    public string? PresentPostOffice { get; set; }

    [MaxLength(150)]
    public string? PresentThana { get; set; }

    [MaxLength(100)]
    public string? PresentDistrict { get; set; }

    // Permanent
    [MaxLength(150)]
    public string? PermanentVillage { get; set; }

    [MaxLength(150)]
    public string? PermanentPostOffice { get; set; }

    [MaxLength(150)]
    public string? PermanentThana { get; set; }

    [MaxLength(100)]
    public string? PermanentDistrict { get; set; }

    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public SchoolClass Class { get; set; } = null!;
    public Section Section { get; set; } = null!;
    public int RollNumber { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Active;

    // Links the student profile to an authenticated application user (student onboarding).
    public int? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    // Religion-based subject assignment
    // Students automatically get the appropriate religion subject based on Religion field
    // E.g., Religion="Islam" → ইসলাম ও নৈতিক শিক্ষা
    //       Religion="Hindu" → হিন্দু ধর্ম ও নৈতিক শিক্ষা
    public int? AssignedReligionSubjectId { get; set; }
    public Subject? AssignedReligionSubject { get; set; }

    // Per-student optional subject selection (e.g., Agriculture or Home Science for 6-8)
    public int? OptionalSubjectId { get; set; }
    public Subject? OptionalSubject { get; set; }

    // Group assignment for Class 9-10 (Science, Humanities, Business)
    public int? StudentGroupId { get; set; }
    public StudentGroup? StudentGroup { get; set; }

    public ICollection<SchoolManagementSystem.Models.Entities.Guardian.StudentGuardian> StudentGuardians { get; set; } = new List<SchoolManagementSystem.Models.Entities.Guardian.StudentGuardian>();
    public ICollection<StudentDocument> Documents { get; set; } = new List<StudentDocument>();
    public ICollection<StudentGroupAssignment> GroupAssignments { get; set; } = [];
}

public class StudentDocument : BaseEntity
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    [MaxLength(80)]
    public string DocumentType { get; set; } = string.Empty;

    [MaxLength(260)]
    public string FilePath { get; set; } = string.Empty;
}

public class StudentPromotion : BaseEntity
{
    public int StudentId { get; set; }
    public int FromClassId { get; set; }
    public int ToClassId { get; set; }
    public int AcademicYearId { get; set; }
    public DateTime PromotedAt { get; set; } = DateTime.UtcNow;
}

public class TransferCertificate : BaseEntity
{
    public int StudentId { get; set; }

    public int OldClassId { get; set; }

    public int? OldSectionId { get; set; }

    [MaxLength(200)]
    public string NewSchoolName { get; set; } = string.Empty;

    [MaxLength(40)]
    public string CertificateNo { get; set; } = string.Empty;

    public DateTime IssueDate { get; set; }

    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
