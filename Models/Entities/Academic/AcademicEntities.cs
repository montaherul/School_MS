using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Entities.Teachers;
namespace SchoolManagementSystem.Models.Entities.Academic;

public class AcademicYear : BaseEntity
{
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    public DateTime StartsOn { get; set; }
    public DateTime EndsOn { get; set; }
    public bool IsActive { get; set; }
}

public class SchoolClass : BaseEntity
{
    [MaxLength(60)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(60)]
    public string NameBn { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    public int SortOrder { get; set; }
    public int Capacity { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsGroupBased { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ArchivedAt { get; set; }

    public ICollection<Section> Sections { get; set; } = new List<Section>();
    public SchoolClass(string name) { Name = name; }
    public SchoolClass() { }
}

public class Section : BaseEntity
{
    public int SchoolClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }

    public int? StudentGroupId { get; set; }
    public StudentGroup? StudentGroup { get; set; }

    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; } = 50;

    // Hierarchy support (null = top-level group)
    public int? ParentSectionId { get; set; }
    public Section? ParentSection { get; set; }
    public ICollection<Section> SubSections { get; set; } = new List<Section>();

    // True if this is a group container (has children)
    public bool IsGroup => ParentSectionId == null && SubSections.Any();
}

public class Subject : BaseEntity
{
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string NameBn { get; set; } = string.Empty;

    [MaxLength(30)]
    public string ShortName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // Core, Elective, Vocational, Religion

    [MaxLength(50)]
    public string SubjectGroup { get; set; } = string.Empty;

    public bool IsMandatory { get; set; } = true;
    public bool IsOptional { get; set; } = false;
    public bool IsReligionSubject { get; set; } = false;
    public bool IsPractical { get; set; } = false;

    [MaxLength(50)]
    public string? ReligionType { get; set; }

    public decimal DefaultFullMarks { get; set; } = 100;
    public decimal DefaultPassMarks { get; set; } = 33;

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public virtual ICollection<ClassSubject> ClassSubjects { get; set; } = [];
}


public class ClassSubjectTeacher : BaseEntity
{
    public int ClassSubjectId { get; set; }
    public ClassSubject? ClassSubject { get; set; }

    public int TeacherId { get; set; }   // ✅ FIXED
    public Teacher? Teacher { get; set; }

    public int AcademicYearId { get; set; }
}
public class Syllabus : BaseEntity
{
    public int SchoolClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public int AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(260)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? FileName { get; set; }

    public long? FileSize { get; set; }

    [MaxLength(50)]
    public string? FileType { get; set; }

    [MaxLength(64)]
    public string UploadedBy { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}

public class LessonPlan : BaseEntity
{
    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    public int SchoolClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public int AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Objectives { get; set; }

    public string? Materials { get; set; }

    public string? Procedure { get; set; }

    [MaxLength(500)]
    public string? AssessmentMethod { get; set; }

    public int? DurationMinutes { get; set; }

    public DateTime LessonDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = "Draft"; // Draft, Published, Archived

    public bool IsActive { get; set; } = true;
}

public class StudyMaterial : BaseEntity
{
    public int SchoolClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public int AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(30)]
    public string MaterialType { get; set; } = "Note"; // Note, Video, Reference, Worksheet

    [MaxLength(260)]
    public string? FilePath { get; set; }

    [MaxLength(200)]
    public string? FileName { get; set; }

    public long? FileSize { get; set; }

    [MaxLength(50)]
    public string? FileType { get; set; }

    [MaxLength(500)]
    public string? ExternalUrl { get; set; }

    [MaxLength(260)]
    public string ResourceUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
public class StudentGroup : BaseEntity
{
    /// <summary>
    /// Group name: Science, Humanities, Business Studies
    /// </summary>
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Minimum class for this group (usually 9)
    /// </summary>
    public int MinClass { get; set; } = 9;

    /// <summary>
    /// Maximum class for this group (usually 10)
    /// </summary>
    public int MaxClass { get; set; } = 10;

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual ICollection<ClassSubjectGroup> ClassSubjectGroups { get; set; } = [];
    public virtual ICollection<StudentGroupAssignment> StudentAssignments { get; set; } = [];
}

/// <summary>
/// Student assignment to groups for class 9-10 streams
/// Tracks which student belongs to which group (Science, Humanities, Business)
/// </summary>
public class StudentGroupAssignment : BaseEntity
{
    public int StudentId { get; set; }
    public int StudentGroupId { get; set; }
    public int SchoolClassId { get; set; }
    public int AcademicYearId { get; set; }

    public DateTime AssignedDate { get; set; } = DateTime.Now;

    // Navigation
    public virtual Student.Student Student { get; set; } = null!;
    public virtual StudentGroup StudentGroup { get; set; } = null!;
    public virtual SchoolClass Class { get; set; } = null!;
    public virtual AcademicYear AcademicYear { get; set; } = null!;
}
