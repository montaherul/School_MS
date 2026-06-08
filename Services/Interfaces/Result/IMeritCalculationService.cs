using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

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


