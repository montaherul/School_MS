using SchoolManagementSystem.Services.Interfaces.Result;

namespace SchoolManagementSystem.Services.Implementations.Result;

/// <summary>
/// GPA Calculation Service implementing Bangladesh education system grading
/// </summary>
public class GPACalculationService : IGPACalculationService
{
    /// <summary>
    /// Bangladesh grading system mapping
    /// </summary>
    private readonly Dictionary<(decimal Min, decimal Max), (string Grade, decimal Point)> _gradingRules = new()
    {
        {(80, 100), ("A+", 5.00m)},
        {(70, 79.99m), ("A", 4.00m)},
        {(60, 69.99m), ("A-", 3.50m)},
        {(50, 59.99m), ("B", 3.00m)},
        {(40, 49.99m), ("C", 2.00m)},
        {(33, 39.99m), ("D", 1.00m)},
        {(0, 32.99m), ("F", 0.00m)}
    };

    public async Task<decimal> CalculateGPAAsync(decimal marks, decimal fullMarks = 100)
    {
        // Convert to percentage if needed
        decimal percentage = fullMarks > 0 ? (marks / fullMarks) * 100 : 0;

        var (grade, gradePoint) = await GetGradeAndPointAsync(percentage);

        return gradePoint;
    }

    public async Task<(string Grade, decimal GradePoint)> GetGradeAndPointAsync(decimal marks)
    {
        foreach (var rule in _gradingRules.OrderByDescending(r => r.Key.Min))
        {
            if (marks >= rule.Key.Min && marks <= rule.Key.Max)
            {
                return (rule.Value.Grade, rule.Value.Point);
            }
        }

        // Default to F for marks below 33
        return ("F", 0.00m);
    }

    public async Task<decimal> CalculateCumulativeGPAAsync(IEnumerable<decimal> gpas)
    {
        if (!gpas.Any()) return 0;

        decimal sum = gpas.Sum();
        decimal average = sum / gpas.Count();

        return await RoundGPAAccordingToRulesAsync(average);
    }

    public async Task<decimal> RoundGPAAccordingToRulesAsync(decimal rawGpa)
    {
        // Bangladesh GPA rounding rules:
        // Round to 2 decimal places, but ensure proper grading boundaries

        decimal rounded = Math.Round(rawGpa, 2);

        // Ensure GPA stays within valid grade boundaries
        if (rounded >= 4.00m && rounded < 5.00m)
        {
            // Between A and A+ range, round to nearest 0.50
            decimal fractional = rounded % 1;
            if (fractional >= 0.25m && fractional < 0.75m)
                rounded = Math.Floor(rounded) + 0.50m;
            else if (fractional >= 0.75m)
                rounded = Math.Ceiling(rounded);
            else
                rounded = Math.Floor(rounded);
        }

        // Ensure GPA doesn't exceed 5.00
        return Math.Min(rounded, 5.00m);
    }

    public async Task<bool> ValidateGPACalculationAsync(decimal marks, decimal gpa)
    {
        var expectedGpa = await CalculateGPAAsync(marks);

        // Allow small rounding differences
        return Math.Abs(expectedGpa - gpa) <= 0.01m;
    }

    /// <summary>
    /// Gets all grading rules for reference
    /// </summary>
    public Dictionary<(decimal Min, decimal Max), (string Grade, decimal Point)> GetGradingRules()
    {
        return _gradingRules;
    }

    /// <summary>
    /// Converts GPA to letter grade
    /// </summary>
    public string GetLetterGrade(decimal gpa)
    {
        if (gpa >= 5.00m) return "A+";
        if (gpa >= 4.00m) return "A";
        if (gpa >= 3.50m) return "A-";
        if (gpa >= 3.00m) return "B";
        if (gpa >= 2.00m) return "C";
        if (gpa >= 1.00m) return "D";
        return "F";
    }

    /// <summary>
    /// Gets minimum marks required for a grade
    /// </summary>
    public decimal GetMinimumMarksForGrade(string grade)
    {
        return grade switch
        {
            "A+" => 80,
            "A" => 70,
            "A-" => 60,
            "B" => 50,
            "C" => 40,
            "D" => 33,
            "F" => 0,
            _ => 0
        };
    }
}