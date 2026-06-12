using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

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

    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;

    public int DisplayOrder { get; set; } = 0;

    public bool IsMandatory { get; set; } = true;
    public bool IsOptional { get; set; } = false;
    public bool IsGroupSubject { get; set; } = false;
    public bool IsReligionSubject { get; set; } = false;

    [MaxLength(50)]
    public string? ReligionType { get; set; }

    [MaxLength(50)]
    public string GroupName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public virtual ICollection<ClassSubjectTeacher> ClassSubjectTeachers { get; set; } = [];
}
