using SchoolManagementSystem.Models.DTOs.Exam;

namespace SchoolManagementSystem.Models.ViewModels.Exam;

public class ExamSubjectSetupViewModel
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? GroupName { get; set; }
    public List<ExamSubjectConfigDto> Subjects { get; set; } = [];
    public List<TeacherOption> Teachers { get; set; } = [];
}

public class TeacherOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
