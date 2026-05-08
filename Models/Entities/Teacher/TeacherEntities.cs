using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Teachers;

public class Teacher : BaseEntity
{
    [MaxLength(30)]
    public string TeacherNo { get; set; } = string.Empty;

    [MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? FullNameBangla { get; set; }

    public DateTime DateOfBirth { get; set; }

    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    // ── Contact ──────────────────────────────────────────────────────────────
    [MaxLength(30)]
    public string MobileNumber { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? AlternativeNumber { get; set; }

    [MaxLength(160)]
    public string? EmailAddress { get; set; }

    // ── Demographics ──────────────────────────────────────────────────────────
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

    // ── Identity ─────────────────────────────────────────────────────────────
    [MaxLength(50)]
    public string? PassportNo { get; set; }

    [MaxLength(50)]
    public string? NationalIdNo { get; set; }

    // ── Professional ─────────────────────────────────────────────────────────
    [MaxLength(100)]
    public string Designation { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Department { get; set; }

    [MaxLength(200)]
    public string? Qualification { get; set; }

    [MaxLength(200)]
    public string? Specialization { get; set; }

    public DateTime? JoiningDate { get; set; }

    // ── Family ───────────────────────────────────────────────────────────────
    [MaxLength(120)]
    public string? FatherName { get; set; }

    [MaxLength(120)]
    public string? MotherName { get; set; }

    [MaxLength(120)]
    public string? SpouseName { get; set; }

    // ── Address ───────────────────────────────────────────────────────────────
    [MaxLength(150)]
    public string? PresentVillage { get; set; }

    [MaxLength(150)]
    public string? PresentPostOffice { get; set; }

    [MaxLength(150)]
    public string? PresentThana { get; set; }

    [MaxLength(100)]
    public string? PresentDistrict { get; set; }

    [MaxLength(150)]
    public string? PermanentVillage { get; set; }

    [MaxLength(150)]
    public string? PermanentPostOffice { get; set; }

    [MaxLength(150)]
    public string? PermanentThana { get; set; }

    [MaxLength(100)]
    public string? PermanentDistrict { get; set; }

    // ── Media ─────────────────────────────────────────────────────────────────
    [MaxLength(260)]
    public string? ProfilePicturePath { get; set; }

    // ── Status & Auth link ────────────────────────────────────────────────────
    public TeacherStatus Status { get; set; } = TeacherStatus.Active;

    public int? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────────
    public ICollection<TeacherDocument> Documents { get; set; } = new List<TeacherDocument>();
    public ICollection<TeacherClassAssignment> ClassAssignments { get; set; } = new List<TeacherClassAssignment>();
    public ICollection<TeacherSubjectAssignment> SubjectAssignments { get; set; } = new List<TeacherSubjectAssignment>();
}

public class TeacherAttendance : BaseEntity
{
    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    public DateTime AttendanceDate { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Present"; // Present, Absent, Half-Day, Late, OnLeave

    [MaxLength(255)]
    public string Remarks { get; set; } = string.Empty;
}

public class TeacherLeave : BaseEntity
{
    public int TeacherProfileId { get; set; }
    public Teacher? Teacher { get; set; }

    [MaxLength(50)]
    public string LeaveType { get; set; } = string.Empty; // Sick, Casual, Earned, etc.

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

    [MaxLength(255)]
    public string? ApproverRemarks { get; set; }

    public int? ApprovedByUserId { get; set; }
    public ApplicationUser? ApprovedByUser { get; set; }
    public DateTime? ApprovedDate { get; set; }
}

public class TeacherDocument : BaseEntity
{
    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    [MaxLength(50)]
    public string DocumentType { get; set; } = string.Empty; // Resume, ID Proof, Certificate

    [MaxLength(255)]
    public string FilePath { get; set; } = string.Empty;

    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
}

public class TeacherSalary : BaseEntity
{
    public int TeacherProfileId { get; set; }
    public Teacher? Teacher { get; set; }

    public DateTime MonthYear { get; set; }

    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Unpaid"; // Paid, Unpaid, Pending
}

public class TeacherPerformance : BaseEntity
{
    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    public int AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public DateTime EvaluationDate { get; set; }

    public int? EvaluatorUserId { get; set; }
    public ApplicationUser? EvaluatorUser { get; set; }

    public int Rating { get; set; } // 1 to 5

    [MaxLength(500)]
    public string Comments { get; set; } = string.Empty;
}
