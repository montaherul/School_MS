namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IExamValidationService
{
    Task<List<string>> ValidateSubjectMarkStructuresAsync(List<int> subjectIds, CancellationToken ct = default);
    Task ThrowIfSubjectMarkStructureMissingAsync(List<int> subjectIds, CancellationToken ct = default);
}
