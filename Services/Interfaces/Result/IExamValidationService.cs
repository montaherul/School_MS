namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IExamValidationService
{
    Task<List<string>> ValidateSubjectMarkStructuresAsync(List<int> subjectIds, CancellationToken ct = default);
    Task ThrowIfSubjectMarkStructureMissingAsync(List<int> subjectIds, CancellationToken ct = default);

    /// <summary>
    /// Validates Bangladesh Group Rules:
    /// Classes 1-8 must have StudentGroupId = null (General only)
    /// Classes 9-10 must have a valid StudentGroupId (Science/BusinessStudies/Humanities)
    /// </summary>
    Task ValidateBangladeshGroupRulesAsync(int classId, int? studentGroupId, CancellationToken ct = default);
}
