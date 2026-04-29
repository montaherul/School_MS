using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.Entities.Teacher;

public class TeacherAttendance : BaseEntity
{
    public int TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }

    public DateTime AttendanceDate { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Present"; // Present, Absent, Half-Day, Late, OnLeave

    [MaxLength(255)]
    public string Remarks { get; set; } = string.Empty;
}

public class TeacherLeave : BaseEntity
{
    public int TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }

    [MaxLength(50)]
    public string LeaveType { get; set; } = string.Empty; // Sick, Casual, Earned, etc.

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

    [MaxLength(255)]
    public string ApproverRemarks { get; set; } = string.Empty;
}

public class TeacherDocument : BaseEntity
{
    public int TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }

    [MaxLength(50)]
    public string DocumentType { get; set; } = string.Empty; // Resume, ID Proof, Certificate

    [MaxLength(255)]
    public string FilePath { get; set; } = string.Empty;

    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
}

public class TeacherSalary : BaseEntity
{
    public int TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }

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
    public int TeacherProfileId { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }

    public int AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public DateTime EvaluationDate { get; set; }

    [MaxLength(100)]
    public string Evaluator { get; set; } = string.Empty;

    public int Rating { get; set; } // 1 to 5

    [MaxLength(500)]
    public string Comments { get; set; } = string.Empty;
}
