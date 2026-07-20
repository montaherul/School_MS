namespace SchoolManagementSystem.Models.DTOs.AI;

public class AiContextDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string? GroupName { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
    public List<string> Subjects { get; set; } = new();
}
