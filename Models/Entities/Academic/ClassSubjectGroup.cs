using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Academic;

public class ClassSubjectGroup : BaseEntity
{
    public int ClassSubjectId { get; set; }
    public ClassSubject? ClassSubject { get; set; }

    public int StudentGroupId { get; set; }
    public StudentGroup? StudentGroup { get; set; }
}
