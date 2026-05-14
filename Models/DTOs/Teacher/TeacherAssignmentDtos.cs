namespace SchoolManagementSystem.Models.DTOs.Teacher;

public class TeacherClassAssignmentDto
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
}

public class TeacherSubjectAssignmentDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public int SectionId { get; set; }
}
