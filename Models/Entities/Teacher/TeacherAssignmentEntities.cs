using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.Entities.Teachers;

/// <summary>
/// Maps a teacher as the authorized Class Teacher for a specific class and section.
/// Attendance marking and class monitoring are restricted based on this assignment.
/// </summary>
public class TeacherClassAssignment : BaseEntity
{
    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    public int ClassId { get; set; }
    public SchoolClass? Class { get; set; }

    public int SectionId { get; set; }
    public Section? Section { get; set; }

    public int AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public bool IsClassTeacher { get; set; } // Can be true for Class Teacher, false for assistant

    public bool IsActive { get; set; } = true;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(64)]
    public string? AssignedBy { get; set; }

    [MaxLength(250)]
    public string? Remarks { get; set; }
}

/// <summary>
/// Maps a teacher as the authorized Subject Teacher.
/// Marks entry and result processing are restricted based on this assignment.
/// </summary>
public class TeacherSubjectAssignment : BaseEntity
{
    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public int ClassId { get; set; }
    public SchoolClass? Class { get; set; }

    public int SectionId { get; set; }
    public Section? Section { get; set; }

    public int AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(64)]
    public string? AssignedBy { get; set; }

    [MaxLength(250)]
    public string? Remarks { get; set; }
}

/// <summary>
/// Optional extension entity for tracking teacher specializations and eligibility.
/// </summary>
public class TeacherAcademicProfile : BaseEntity
{
    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    [MaxLength(100)]
    public string? SubjectSpecialization { get; set; }

    [MaxLength(50)]
    public string? TeachingLevel { get; set; } // e.g., Primary, Secondary, Higher Secondary

    public bool IsExamController { get; set; }
    public bool IsRoutineCoordinator { get; set; }
    public bool IsClassTeacherEligible { get; set; } = true;

    public int ExperienceYears { get; set; }
}

/// <summary>
/// Audit log for tracking assignment changes and authorization failures.
/// </summary>
public class TeacherAssignmentLog : BaseEntity
{
    public int? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    [MaxLength(50)]
    public string Action { get; set; } = string.Empty; // Create, Update, Delete, AuthFailure

    [MaxLength(50)]
    public string EntityName { get; set; } = string.Empty; // ClassTeacherAssignment, SubjectTeacherAssignment

    public int? EntityId { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(45)]
    public string? IPAddress { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }
}

public class TeacherTimetable : BaseEntity
{
    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    public int ClassId { get; set; }
    public SchoolClass? Class { get; set; }

    public int SectionId { get; set; }
    public Section? Section { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    [MaxLength(20)]
    public string DayOfWeek { get; set; } = string.Empty;

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    [MaxLength(50)]
    public string? RoomNo { get; set; }
}
