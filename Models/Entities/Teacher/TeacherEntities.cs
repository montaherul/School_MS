using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Teachers;

public class Teacher : BaseEntity
{
    [Required]

    [MaxLength(30)]
    public string TeacherNo { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? FullNameBangla { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    // ── Contact ──────────────────────────────────────────────────────────────
    [Required]
    [Phone]
    [MaxLength(30)]
    public string MobileNumber { get; set; } = string.Empty;

    [Phone]
    [MaxLength(30)]
    public string? AlternativeNumber { get; set; }

    [EmailAddress]
    [MaxLength(160)]
    public string? EmailAddress { get; set; }

    // ── Demographics ──────────────────────────────────────────────────────────
    [Required]

    public int EmployeeId { get; set; }
    public SchoolManagementSystem.Models.Entities.Employee.Employee? Employee { get; set; }

    [Required]

    [MaxLength(50)]
    public string TeacherCode { get; set; } = string.Empty;


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
    [Display(Name = "Blood Group")]
    public string? BloodGroup { get; set; }

    // ── Identity ─────────────────────────────────────────────────────────────
    [MaxLength(50)]
    public string? PassportNo { get; set; }

    [MaxLength(260)]
    public string? PassportPath { get; set; }

    [MaxLength(50)]
    public string? NationalIdNo { get; set; }

    [MaxLength(260)]
    public string? NationalIdPath { get; set; }

    // ── Professional ─────────────────────────────────────────────────────────
    [Required]
    [MaxLength(100)]
    public string Designation { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Department { get; set; }

    // Compatibility Alias
    [NotMapped]
    public string TeacherNo
    {
        get => TeacherCode;
        set => TeacherCode = value;
    }


    [MaxLength(200)]
    public string? SubjectSpecialization { get; set; }

    [MaxLength(100)]
    public string? TeachingLevel { get; set; } // Primary, Junior Secondary, Secondary

    public bool IsClassTeacher { get; set; }

    public bool IsExamController { get; set; }

    public bool IsRoutineCoordinator { get; set; }

    public int TeachingExperienceYears { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }


    // ── Status & Auth link ────────────────────────────────────────────────────
    [Required]
    public TeacherStatus Status { get; set; } = TeacherStatus.Active;

    // Compatibility Alias for designation/department properties queried dynamically
    [NotMapped]
    public string FullName => Employee?.FullName ?? string.Empty;


    [NotMapped]
    public string? FullNameBangla => Employee?.FullName; // Dynamic fallback

    [NotMapped]
    public string MobileNumber => Employee?.Phone ?? string.Empty;

    [NotMapped]
    public string? EmailAddress => Employee?.Email;

    [NotMapped]
    public string Designation => Employee?.Designation?.Name ?? string.Empty;

    [NotMapped]
    public string? Department => Employee?.Department?.Name;

    [NotMapped]
    public string? Qualification => "Academic Post"; // Dynamically derived if needed

    [NotMapped]
    public DateTime? JoiningDate => Employee?.JoiningDate;

    [NotMapped]
    public string? ProfilePicturePath => Employee?.ProfilePicturePath;

    [NotMapped]
    public TeacherStatus Status
    {
        get => Employee != null && Employee.Status == "Active" ? TeacherStatus.Active : TeacherStatus.Inactive;
        set { if (Employee != null) Employee.Status = value == TeacherStatus.Active ? "Active" : "Inactive"; }
    }

    [NotMapped]
    public int? UserId => Employee?.UserId;

    [NotMapped]
    public ApplicationUser? User => Employee?.User;

    // ── Navigation ────────────────────────────────────────────────────────────
    public ICollection<TeacherDocument> Documents { get; set; } = new List<TeacherDocument>();


    public ICollection<TeacherClassAssignment> ClassAssignments { get; set; }= new List<TeacherClassAssignment>();

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
