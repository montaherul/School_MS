using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Entities.Exam;
//using System.Text.RegularExpressions;

namespace SchoolManagementSystem.Models.Entities.Academic;

public class ClassSubject : BaseEntity
{
    public int SchoolClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public int? StudentGroupId { get; set; }
    public StudentGroup? StudentGroup { get; set; }

    public int? SectionId { get; set; }
    public Section? Section { get; set; }

    /// <summary>
    /// Full marks for the subject
    /// </summary>
    public decimal FullMarks { get; set; } = 100;

    /// <summary>
    /// Minimum marks needed to pass
    /// </summary>
    public decimal PassMarks { get; set; } = 33;

    /// <summary>
    /// Component-wise marks distribution
    /// </summary>
    public decimal? WrittenMarks { get; set; }
    public decimal? MCQMarks { get; set; }
    public decimal? CQMarks { get; set; }
    public decimal? PracticalMarks { get; set; }
    public decimal? VivaMarks { get; set; }
    public decimal? LabMarks { get; set; }
    public decimal? OralMarks { get; set; }
    public decimal? AssignmentMarks { get; set; }
    public decimal? ContinuousAssessmentMarks { get; set; }

    /// <summary>
    /// For primary classes: competency-based evaluation
    /// </summary>
    public decimal? CompetencyMarks { get; set; }
    public decimal? BehaviourMarks { get; set; }
    public decimal? ParticipationMarks { get; set; }

    /// <summary>
    /// Subject configuration flags
    /// </summary>
    public bool IsMandatory { get; set; } = true;
    public bool IsOptional { get; set; } = false;
    public bool IsGroupSubject { get; set; } = false; // For Science/Humanities/Business groups
    public bool IsReligionSubject { get; set; } = false;

    [MaxLength(50)]
    public string? ReligionType { get; set; }

    [MaxLength(50)]
    public string? GroupName { get; set; } // Science, Humanities, Business for Class 9-10

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual ICollection<ClassSubjectTeacher> ClassSubjectTeachers { get; set; } = [];
    public virtual ICollection<SubjectComponent> SubjectComponents { get; set; } = [];
}