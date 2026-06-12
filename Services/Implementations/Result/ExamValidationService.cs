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

    /// <summary>
    /// Validates Bangladesh Group Rules:
    /// Classes 1-8 → must have no group (General only)
    /// Classes 9-10 → must have a valid group: Science, BusinessStudies, or Humanities
    /// </summary>
    public async Task ValidateBangladeshGroupRulesAsync(int classId, int? studentGroupId, CancellationToken ct = default)
    {
        var schoolClass = await _uow.Repository<SchoolClass>().GetByIdAsync(classId, ct);
        if (schoolClass == null)
            throw new InvalidOperationException($"Class with ID {classId} not found.");

        // Parse numeric part from class name (e.g., "Class 6", "Six", "6")
        var classNumber = ExtractClassNumber(schoolClass.Name);

        if (classNumber >= 1 && classNumber <= 8)
        {
            // Classes 1-8: Group must be null (General only)
            if (studentGroupId.HasValue)
            {
                var group = await _uow.Repository<StudentGroup>().GetByIdAsync(studentGroupId.Value, ct);
                var groupName = group?.Name ?? "Unknown";
                throw new InvalidOperationException(
                    $"Class {classNumber} does not support groups. Group '{groupName}' is not allowed. " +
                    $"Only General (no group) is permitted for Classes 1-8.");
            }
        }
        else if (classNumber >= 9 && classNumber <= 10)
        {
            // Classes 9-10: Group is required and must be valid
            if (!studentGroupId.HasValue)
            {
                throw new InvalidOperationException(
                    $"Class {classNumber} requires a group selection. " +
                    $"Please select Science, Business Studies, or Humanities.");
            }

            var group = await _uow.Repository<StudentGroup>().GetByIdAsync(studentGroupId.Value, ct);
            if (group == null)
            {
                throw new InvalidOperationException($"Selected group not found.");
            }

            var validGroups = new[] { "Science", "Business Studies", "BusinessStudies", "Humanities" };
            if (!validGroups.Contains(group.Name, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Invalid group '{group.Name}' for Class {classNumber}. " +
                    $"Allowed groups: Science, Business Studies, Humanities.");
            }
        }
    }

    private static int ExtractClassNumber(string className)
    {
        if (string.IsNullOrEmpty(className)) return 0;

        // Try to parse digits from the class name
        var digits = new string(className.Where(char.IsDigit).ToArray());
        if (int.TryParse(digits, out var number) && number > 0 && number <= 12)
            return number;

        // Handle word-based class names
        var wordMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            {"one", 1}, {"two", 2}, {"three", 3}, {"four", 4}, {"five", 5},
            {"six", 6}, {"seven", 7}, {"eight", 8}, {"nine", 9}, {"ten", 10},
            {"eleven", 11}, {"twelve", 12}
        };
        var words = className.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            var clean = word.Trim().ToLowerInvariant();
            if (wordMap.TryGetValue(clean, out var num))
                return num;
        }

        return 0;
    }

    public async Task<List<string>> ValidateSubjectMarkStructuresAsync(List<int> subjectIds, CancellationToken ct = default)
    {
        if (subjectIds.Count == 0)
            return [];

        var anyStructure = await _uow.Repository<SubjectMarkStructure>().Query()
            .AnyAsync(s => !s.IsDeleted && s.IsActive
                && s.Component != null && s.Component.IsActive, ct);

        if (!anyStructure)
        {
            var hasDefaults = await _uow.Repository<ExamComponent>().Query()
                .AnyAsync(c => !c.IsDeleted && c.IsActive, ct);

            if (hasDefaults)
                return [];

            var names = await _uow.Repository<Subject>().Query()
                .Where(s => subjectIds.Contains(s.Id) && !s.IsDeleted)
                .ToDictionaryAsync(s => s.Id, s => s.Name, ct);

            return subjectIds.Select(id => names.GetValueOrDefault(id) ?? $"Subject ID {id}").ToList();
        }

        var existingIds = await _uow.Repository<SubjectMarkStructure>().Query()
            .Where(s => s.SubjectId != null && subjectIds.Contains(s.SubjectId.Value) && !s.IsDeleted && s.IsActive
                && s.Component != null && s.Component.IsActive)
            .Select(s => s.SubjectId!.Value)
            .Distinct()
            .ToListAsync(ct);

        var existingSet = new HashSet<int>(existingIds);
        var missingIds = subjectIds.Where(id => !existingSet.Contains(id)).ToList();

        if (missingIds.Count == 0)
            return [];

        var missingNames = await _uow.Repository<Subject>().Query()
            .Where(s => missingIds.Contains(s.Id) && !s.IsDeleted)
            .ToDictionaryAsync(s => s.Id, s => s.Name, ct);

        return missingIds.Select(id => missingNames.GetValueOrDefault(id) ?? $"Subject ID {id}").ToList();
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
