namespace SchoolManagementSystem.Models.ViewModels.Student;

public class StudentProfileViewModel
{
    public SchoolManagementSystem.Models.Entities.Student.Student Student { get; set; } = null!;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
}
