using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IGradeCalculator
{
    (string? Grade, decimal? GradePoint) CalculateGrade(decimal marks, IEnumerable<GradingRule> gradingRules);
    string GetOverallGrade(decimal gpa);
}
