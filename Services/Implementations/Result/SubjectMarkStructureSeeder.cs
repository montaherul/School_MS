using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class SubjectMarkStructureSeeder
{
    private readonly SchoolDbContext _db;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<SubjectMarkStructureSeeder> _logger;

    public SubjectMarkStructureSeeder(
        SchoolDbContext db,
        IUnitOfWork uow,
        ILogger<SubjectMarkStructureSeeder> logger)
    {
        _db = db;
        _uow = uow;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
                var now = DateTime.UtcNow;

                // 1. Seed ExamComponents if empty
                var existingComponents = await _uow.Repository<ExamComponent>()
                    .Query().Where(c => !c.IsDeleted).ToListAsync(cancellationToken);

                if (existingComponents.Count == 0)
                {
                    _logger.LogInformation("Seeding ExamComponents...");

                    var components = new List<ExamComponent>
                    {
                        new() { Name = "Written", Code = "WRITTEN", Description = "Written examination", DisplayOrder = 1, DefaultFullMarks = 70, DefaultPassMarks = 28, IsActive = true, CreatedAt = now, CreatedBy = "system" },
                        new() { Name = "MCQ", Code = "MCQ", Description = "Multiple Choice Questions", DisplayOrder = 2, DefaultFullMarks = 30, DefaultPassMarks = 12, IsActive = true, CreatedAt = now, CreatedBy = "system" },
                        new() { Name = "Creative Question", Code = "CQ", Description = "Creative/analytical questions", DisplayOrder = 3, DefaultFullMarks = 50, DefaultPassMarks = 20, IsActive = true, CreatedAt = now, CreatedBy = "system" },
                        new() { Name = "Practical", Code = "PRACTICAL", Description = "Practical examination", DisplayOrder = 4, DefaultFullMarks = 50, DefaultPassMarks = 20, IsPractical = true, IsActive = true, CreatedAt = now, CreatedBy = "system" },
                        new() { Name = "Lab", Code = "LAB", Description = "Laboratory work", DisplayOrder = 5, DefaultFullMarks = 25, DefaultPassMarks = 10, IsPractical = true, IsActive = true, CreatedAt = now, CreatedBy = "system" },
                        new() { Name = "Class Test", Code = "CT", Description = "Class test / continuous assessment", DisplayOrder = 6, DefaultFullMarks = 10, DefaultPassMarks = 4, IsActive = true, CreatedAt = now, CreatedBy = "system" },
                        new() { Name = "Assignment", Code = "ASSIGNMENT", Description = "Assignment/project work", DisplayOrder = 7, DefaultFullMarks = 20, DefaultPassMarks = 8, IsActive = true, CreatedAt = now, CreatedBy = "system" },
                        new() { Name = "Viva", Code = "VIVA", Description = "Oral viva examination", DisplayOrder = 8, DefaultFullMarks = 20, DefaultPassMarks = 8, IsActive = true, CreatedAt = now, CreatedBy = "system" },
                        new() { Name = "Oral", Code = "ORAL", Description = "Oral test", DisplayOrder = 9, DefaultFullMarks = 10, DefaultPassMarks = 4, IsActive = true, CreatedAt = now, CreatedBy = "system" },
                        new() { Name = "Continuous Assessment", Code = "CONTINUOUS_ASSESSMENT", Description = "Continuous assessment throughout the term", DisplayOrder = 10, DefaultFullMarks = 20, DefaultPassMarks = 8, IsActive = true, CreatedAt = now, CreatedBy = "system" },
                    };

                    await _uow.Repository<ExamComponent>().AddRangeAsync(components, cancellationToken);
                    await _uow.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Seeded {Count} ExamComponents.", components.Count);
                    existingComponents = await _uow.Repository<ExamComponent>()
                        .Query().Where(c => !c.IsDeleted).ToListAsync(cancellationToken);
                }
                else
                {
                    _logger.LogInformation("ExamComponents already seeded ({Count} found).", existingComponents.Count);
                }

                // 2. Seed SubjectMarkStructure per subject where missing
                var subjects = await _uow.Repository<Subject>()
                    .Query().Where(s => !s.IsDeleted).ToListAsync(cancellationToken);

                if (subjects.Count == 0)
                {
                    _logger.LogWarning("No subjects found. Skipping SubjectMarkStructure seed.");
                    return;
                }

                var componentMap = existingComponents.ToDictionary(c => c.Code, StringComparer.OrdinalIgnoreCase);
                var existingStructures = await _uow.Repository<SubjectMarkStructure>()
                    .Query().Where(s => !s.IsDeleted).ToListAsync(cancellationToken);
                var existingKeySet = existingStructures
                    .Select(s => (s.ComponentId, s.SubjectId, s.StudentGroupId))
                    .ToHashSet();

                var scienceSubjectCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { "SCI", "PHY", "CHE", "BIO", "AGR" };
                var practicalSubjectCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { "SCI", "PHY", "CHE", "BIO", "AGR", "ART", "PE", "HSC" };
                var labSubjectCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { "ICT" };

                var newStructures = new List<SubjectMarkStructure>();
                var now2 = DateTime.UtcNow;
                var order = 1;

                foreach (var subject in subjects)
                {
                    var isPractical = practicalSubjectCodes.Contains(subject.Code);
                    var isLab = labSubjectCodes.Contains(subject.Code);
                    var isScience = scienceSubjectCodes.Contains(subject.Code);

                    var subjectComponents = new List<(string Code, decimal Full, decimal Pass)>();
                    subjectComponents.Add(("WRITTEN", 70, 28));
                    subjectComponents.Add(("MCQ", 30, 12));

                    if (isScience || isPractical)
                        subjectComponents.Add(("PRACTICAL", 50, 20));
                    if (isLab)
                        subjectComponents.Add(("LAB", 25, 10));

                    subjectComponents.Add(("CT", 10, 4));
                    subjectComponents.Add(("ASSIGNMENT", 20, 8));

                    foreach (var (code, full, pass) in subjectComponents)
                    {
                        if (!componentMap.TryGetValue(code, out var comp)) continue;
                        var key = (comp.Id, (int?)subject.Id, (int?)null);
                        if (existingKeySet.Contains(key)) continue;

                        newStructures.Add(new SubjectMarkStructure
                        {
                            ComponentId = comp.Id,
                            SubjectId = subject.Id,
                            StudentGroupId = null,
                            FullMarks = full,
                            PassMarks = pass,
                            DisplayOrder = order++,
                            IsActive = true,
                            CreatedAt = now2,
                            CreatedBy = "system"
                        });
                    }
                }

                if (newStructures.Count > 0)
                {
                    await _uow.Repository<SubjectMarkStructure>().AddRangeAsync(newStructures, cancellationToken);
                    await _uow.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Seeded {Count} SubjectMarkStructure entries.", newStructures.Count);
                }
                else
                {
                    _logger.LogInformation("All SubjectMarkStructure entries already exist. Nothing to seed.");
                }

                await transaction.CommitAsync(cancellationToken);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed SubjectMarkStructures.");
        }
    }
}
