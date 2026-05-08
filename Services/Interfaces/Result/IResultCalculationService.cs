using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

/// <summary>
/// Service for calculating student results, GPA, merit positions, and fail detection
/// Supports Bangladesh grading system and flexible component aggregation
/// </summary>
public interface IResultCalculationService
{
    /// <summary>
    /// Calculates overall result for an exam including GPA and merit positions
    /// </summary>
    Task CalculateExamResultsAsync(int examId);

    /// <summary>
    /// Calculates subject-wise results for all students in an exam
    /// </summary>
    Task CalculateSubjectResultsAsync(int examId);

    /// <summary>
    /// Calculates merit positions for entire class, sections, and groups
    /// </summary>
    Task CalculateMeritPositionsAsync(int examId);

    /// <summary>
    /// Calculates GPA for Bangladesh grading system (80-100=A+, 70-79=A, etc.)
    /// </summary>
    Task<decimal> CalculateGpaAsync(IEnumerable<StudentSubjectResult> subjectResults);

    /// <summary>
    /// Calculates final GPA for entire academic year
    /// </summary>
    Task<decimal> CalculateFinalGpaAsync(int studentId, int academicYearId);

    /// <summary>
    /// Determines if student passed the exam based on subject failures
    /// </summary>
    Task<(bool IsPassed, int FailedSubjectCount)> DeterminePassFailStatusAsync(int studentId, int examId);

    /// <summary>
    /// Recalculates results after marks changes (for re-evaluation or corrections)
    /// </summary>
    Task RecalculateResultsAsync(int examId, int studentId);

    /// <summary>
    /// Aggregates component marks into total marks
    /// Supports different calculation rules for different class groups
    /// </summary>
    Task<decimal> AggregateComponentMarksAsync(MarkEntry markEntry);

    /// <summary>
    /// Validates if result calculation is allowed (exam not locked, etc.)
    /// </summary>
    Task<bool> CanCalculateResultsAsync(int examId);

    /// <summary>
    /// Gets subject-wise pass/fail statistics for reporting
    /// </summary>
    Task<IDictionary<int, (int Passed, int Failed)>> GetSubjectPassFailStatsAsync(int examId);
}