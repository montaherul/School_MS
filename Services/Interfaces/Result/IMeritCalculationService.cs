using SchoolManagementSystem.Services.Interfaces.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

/// <summary>
/// Service for calculating merit positions and rankings
/// Supports class-wise, section-wise, and group-wise rankings
/// </summary>
public interface IMeritCalculationService
{
    /// <summary>
    /// Calculates merit positions for entire class
    /// </summary>
    Task CalculateClassMeritPositionsAsync(int examId, int classId);

    /// <summary>
    /// Calculates merit positions within sections
    /// </summary>
    Task CalculateSectionMeritPositionsAsync(int examId, int classId);

    /// <summary>
    /// Calculates merit positions for student groups (Science/Humanities/Business)
    /// </summary>
    Task CalculateGroupMeritPositionsAsync(int examId);

    /// <summary>
    /// Calculates overall school merit list
    /// </summary>
    Task CalculateSchoolMeritPositionsAsync(int examId);

    /// <summary>
    /// Recalculates positions after result changes
    /// </summary>
    Task RecalculateMeritPositionsAsync(int examId);

    /// <summary>
    /// Gets merit list for a specific category
    /// </summary>
    Task<IEnumerable<MeritListItem>> GetMeritListAsync(int examId, MeritCategory category);

    /// <summary>
    /// Gets top performers for dashboard
    /// </summary>
    Task<IEnumerable<TopPerformer>> GetTopPerformersAsync(int examId, int count = 10);
}

public enum MeritCategory
{
    Class,
    Section,
    Group,
    School
}

public class MeritListItem
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public decimal GPA { get; set; }
    public decimal TotalMarks { get; set; }
    public int Position { get; set; }
    public string Grade { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string StudentGroup { get; set; } = string.Empty;
}

public class TopPerformer
{
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public int RollNumber { get; set; }
    public decimal GPA { get; set; }
    public string Grade { get; set; } = string.Empty;
    public int Position { get; set; }
}
