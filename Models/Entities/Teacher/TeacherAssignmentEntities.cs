using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.Entities.Teachers;

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

    public bool IsClassTeacher { get; set; }
}

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
