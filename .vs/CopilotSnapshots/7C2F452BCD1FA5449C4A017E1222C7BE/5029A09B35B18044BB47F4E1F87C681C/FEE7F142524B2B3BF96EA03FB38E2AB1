using SchoolManagementSystem.Models.Entities.Base;
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

    public ICollection<ClassSubjectTeacher> ClassSubjectTeachers { get; set; } = new List<ClassSubjectTeacher>();
}