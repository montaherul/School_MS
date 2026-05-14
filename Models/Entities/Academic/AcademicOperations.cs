using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Models.Entities.Academic;

public class EmployeeSubjectAssignment : BaseEntity
{
    [Required]
    public long EmployeeId { get; set; }
    
    [ForeignKey("EmployeeId")]
    public virtual Employee.Employee Employee { get; set; } = null!;

    [Required]
    public int SubjectId { get; set; }
    
    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; } = null!;

    [Required]
    public int ClassId { get; set; }
    
    [ForeignKey("ClassId")]
    public virtual SchoolClass Class { get; set; } = null!;

    [Required]
    public int SectionId { get; set; }
    
    [ForeignKey("SectionId")]
    public virtual Section Section { get; set; } = null!;

    [Required]
    public int AcademicYearId { get; set; }
    
    [ForeignKey("AcademicYearId")]
    public virtual AcademicYear AcademicYear { get; set; } = null!;

    public bool IsClassTeacher { get; set; }
}

public class ClassRoutine : BaseEntity
{
    [Required]
    public int ClassId { get; set; }
    
    [ForeignKey("ClassId")]
    public virtual SchoolClass Class { get; set; } = null!;

    [Required]
    public int SectionId { get; set; }
    
    [ForeignKey("SectionId")]
    public virtual Section Section { get; set; } = null!;

    [Required]
    public int SubjectId { get; set; }
    
    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; } = null!;

    [Required]
    public long EmployeeId { get; set; }
    
    [ForeignKey("EmployeeId")]
    public virtual Employee.Employee Employee { get; set; } = null!;

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [MaxLength(50)]
    public string? RoomNo { get; set; }
}

public class ExamDutyAssignment : BaseEntity
{
    [Required]
    public long EmployeeId { get; set; }
    
    [ForeignKey("EmployeeId")]
    public virtual Employee.Employee Employee { get; set; } = null!;

    [Required]
    public int ExamId { get; set; } // Assuming Exam entity exists in another file
    
    [MaxLength(50)]
    public string? RoomNo { get; set; }

    [Required]
    public DateTime DutyDate { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }
}

public class AcademicDocument : BaseEntity
{
    [Required]
    public int ClassId { get; set; }
    
    [ForeignKey("ClassId")]
    public virtual SchoolClass Class { get; set; } = null!;

    public int? SectionId { get; set; }
    
    [ForeignKey("SectionId")]
    public virtual Section? Section { get; set; }

    [Required]
    public int SubjectId { get; set; }
    
    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; } = null!;

    [Required]
    public long UploadedByEmployeeId { get; set; }
    
    [ForeignKey("UploadedByEmployeeId")]
    public virtual Employee.Employee UploadedBy { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(100)]
    public string DocumentType { get; set; } = "Notes"; // Notes, Assignment, Syllabus
}
