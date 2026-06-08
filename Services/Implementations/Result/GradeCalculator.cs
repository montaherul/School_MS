using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Services.Interfaces.Result;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class GradeCalculator : IGradeCalculator
{
    public (string? Grade, decimal? GradePoint) CalculateGrade(decimal marks, IEnumerable<GradingRule> gradingRules)
    {
        var rule = gradingRules
            .Where(r => marks >= r.MinMarks && marks <= r.MaxMarks)
            .OrderByDescending(r => r.GradePoint)
            .FirstOrDefault();

        return rule != null ? (rule.Grade, rule.GradePoint) : (null, null);
    }

    public string GetOverallGrade(decimal gpa)
    {
        if (gpa >= 5.00m) return "A+";
        if (gpa >= 4.00m) return "A";
        if (gpa >= 3.50m) return "A-";
        if (gpa >= 3.00m) return "B";
        if (gpa >= 2.00m) return "C";
        if (gpa >= 1.00m) return "D";
        return "F";
    }
}
