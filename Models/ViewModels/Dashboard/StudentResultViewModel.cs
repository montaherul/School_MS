namespace SchoolManagementSystem.Models.ViewModels.Dashboard;

public class StudentResultViewModel
{
    public string SubjectName { get; set; } = string.Empty;

    public string ExamName { get; set; } = string.Empty;

    public decimal ObtainedMarks { get; set; }

    public decimal FullMarks { get; set; }

    public string Grade { get; set; } = string.Empty;

    public decimal GPA { get; set; }

    public bool IsPassed { get; set; }
}