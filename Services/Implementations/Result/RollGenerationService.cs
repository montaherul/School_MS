using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using StudentEntity = SchoolManagementSystem.Models.Entities.Student.Student;

namespace SchoolManagementSystem.Services.Implementations.Result;

/// <summary>
/// Roll generation service: configurable strategies for assigning roll numbers.
/// Supports merit-based, alphabetical, previous roll, and manual.
/// </summary>
public class RollGenerationService : IRollGenerationService
{
    private readonly IUnitOfWork _uow;

    public RollGenerationService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<RollGenerationConfig?> GetConfigAsync(int academicYearId, int classId, CancellationToken ct = default)
    {
        return await _uow.Repository<RollGenerationConfig>().Query()
            .Where(c => c.AcademicYearId == academicYearId && c.SchoolClassId == classId
                && c.IsActive && !c.IsDeleted)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<RollGenerationConfig> SaveConfigAsync(int academicYearId, int classId, RollGenerationStrategy strategy, CancellationToken ct = default)
    {
        var existing = await GetConfigAsync(academicYearId, classId, ct);
        if (existing != null)
        {
            existing.Strategy = strategy;
            existing.UpdatedAt = DateTime.UtcNow;
            _uow.Repository<RollGenerationConfig>().Update(existing);
        }
        else
        {
            existing = new RollGenerationConfig
            {
                AcademicYearId = academicYearId,
                SchoolClassId = classId,
                Strategy = strategy,
                IsActive = true
            };
            await _uow.Repository<RollGenerationConfig>().AddAsync(existing);
        }

        await _uow.SaveChangesAsync();
        return existing;
    }

    public async Task<List<RollGenerationResult>> GenerateRollsAsync(int academicYearId, int classId, CancellationToken ct = default)
    {
        var config = await GetConfigAsync(academicYearId, classId, ct);
        var strategy = config?.Strategy ?? RollGenerationStrategy.MeritBased;

        var students = await _uow.Repository<StudentEntity>().Query()
            .Where(s => s.ClassId == classId && !s.IsDeleted)
            .ToListAsync(ct);

        var studentIds = students.Select(s => s.Id).ToList();
        var finalResults = await _uow.Repository<FinalResult>().Query()
            .Where(f => f.AcademicYearId == academicYearId && studentIds.Contains(f.StudentId))
            .ToListAsync(ct);

        var finalResultDict = finalResults.ToDictionary(f => f.StudentId);
        var results = new List<RollGenerationResult>();

        switch (strategy)
        {
            case RollGenerationStrategy.MeritBased:
                var ranked = students
                    .Select(s => new { Student = s, FR = finalResultDict.GetValueOrDefault(s.Id) })
                    .OrderByDescending(x => x.FR?.FinalGpa ?? 0)
                    .ThenByDescending(x => x.FR?.WeightedTotalMarks ?? 0)
                    .ThenBy(x => x.Student.FullName)
                    .ToList();

                for (int i = 0; i < ranked.Count; i++)
                {
                    int newRoll = i + 1;
                    var student = ranked[i].Student;
                    results.Add(new RollGenerationResult
                    {
                        StudentId = student.Id,
                        StudentName = student.FullName,
                        OldRoll = student.RollNumber,
                        NewRoll = newRoll,
                        Strategy = "MeritBased"
                    });

                    student.RollNumber = newRoll;
                    _uow.Repository<StudentEntity>().Update(student);
                }
                break;

            case RollGenerationStrategy.Alphabetical:
                var sorted = students.OrderBy(s => s.FullName).ToList();
                for (int i = 0; i < sorted.Count; i++)
                {
                    int newRoll = i + 1;
                    var student = sorted[i];
                    results.Add(new RollGenerationResult
                    {
                        StudentId = student.Id,
                        StudentName = student.FullName,
                        OldRoll = student.RollNumber,
                        NewRoll = newRoll,
                        Strategy = "Alphabetical"
                    });

                    student.RollNumber = newRoll;
                    _uow.Repository<StudentEntity>().Update(student);
                }
                break;

            case RollGenerationStrategy.PreviousRoll:
                foreach (var student in students)
                {
                    results.Add(new RollGenerationResult
                    {
                        StudentId = student.Id,
                        StudentName = student.FullName,
                        OldRoll = student.RollNumber,
                        NewRoll = student.RollNumber,
                        Strategy = "PreviousRoll"
                    });
                }
                break;

            case RollGenerationStrategy.Manual:
                foreach (var student in students)
                {
                    results.Add(new RollGenerationResult
                    {
                        StudentId = student.Id,
                        StudentName = student.FullName,
                        OldRoll = student.RollNumber,
                        NewRoll = student.RollNumber,
                        Strategy = "Manual"
                    });
                }
                break;
        }

        if (strategy != RollGenerationStrategy.Manual)
        {
            await _uow.SaveChangesAsync();
        }

        return results;
    }
}
