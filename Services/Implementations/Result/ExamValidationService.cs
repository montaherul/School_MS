using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ExamValidationService : IExamValidationService
{
    private readonly IUnitOfWork _uow;

    public ExamValidationService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<string>> ValidateSubjectMarkStructuresAsync(List<int> subjectIds, CancellationToken ct = default)
    {
        if (subjectIds.Count == 0)
            return [];

        var missing = new List<string>();

        foreach (var subjectId in subjectIds)
        {
            var hasStructure = await _uow.Repository<SubjectMarkStructure>().Query()
                .AnyAsync(s => s.SubjectId == subjectId && !s.IsDeleted && s.IsActive
                    && s.Component != null && s.Component.IsActive, ct);

            if (!hasStructure)
            {
                var subject = await _uow.Repository<Subject>().Query()
                    .Where(s => s.Id == subjectId && !s.IsDeleted)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync(ct);

                missing.Add(subject ?? $"Subject ID {subjectId}");
            }
        }

        return missing;
    }

    public async Task ThrowIfSubjectMarkStructureMissingAsync(List<int> subjectIds, CancellationToken ct = default)
    {
        var missing = await ValidateSubjectMarkStructuresAsync(subjectIds, ct);
        if (missing.Count > 0)
        {
            var names = string.Join(", ", missing);
            throw new InvalidOperationException(
                $"Subject mark structure is not configured for: {names}. " +
                "Please configure component mark distribution via Subject Mark Structure before creating the exam.");
        }
    }
}
