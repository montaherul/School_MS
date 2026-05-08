using SchoolManagementSystem.Services.Interfaces.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

/// <summary>
/// Service for GPA calculation with Bangladesh grading system
/// </summary>
public interface IGPACalculationService
{
    /// <summary>
    /// Calculates GPA for Bangladesh grading system
    /// 80-100 = A+ = 5.00, 70-79 = A = 4.00, 60-69 = A- = 3.50,
    /// 50-59 = B = 3.00, 40-49 = C = 2.00, 33-39 = D = 1.00, 0-32 = F = 0.00
    /// </summary>
    Task<decimal> CalculateGPAAsync(decimal marks, decimal fullMarks = 100);

    /// <summary>
    /// Gets grade and grade point for given marks
    /// </summary>
    Task<(string Grade, decimal GradePoint)> GetGradeAndPointAsync(decimal marks);

    /// <summary>
    /// Calculates cumulative GPA across multiple exams
    /// </summary>
    Task<decimal> CalculateCumulativeGPAAsync(IEnumerable<decimal> gpas);

    /// <summary>
    /// Rounds GPA according to Bangladesh education system rules
    /// </summary>
    Task<decimal> RoundGPAAccordingToRulesAsync(decimal rawGpa);

    /// <summary>
    /// Validates if GPA calculation follows Bangladesh rules
    /// </summary>
    Task<bool> ValidateGPACalculationAsync(decimal marks, decimal gpa);
}