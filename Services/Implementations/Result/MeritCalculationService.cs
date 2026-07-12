using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Services.Implementations.Result;

/// <summary>
/// Merit calculation service with configurable ranking rules.
/// Calculates all 4 position types: School, Class, Section, Group.
/// Supports admin-configurable tie-breaking rules.
/// </summary>
public class MeritCalculationService : IMeritCalculationService
{
    private readonly IUnitOfWork _uow;
    private readonly IStudentExamResultRepository _examResultRepository;
    private readonly IExamRepository _examRepository;
    private readonly IResultPolicyService _resultPolicyService;
    private readonly IFinalResultRepository _finalResultRepository;

    public MeritCalculationService(
        IUnitOfWork uow,
        IStudentExamResultRepository examResultRepository,
        IExamRepository examRepository,
        IResultPolicyService resultPolicyService,
        IFinalResultRepository finalResultRepository)
    {
        _uow = uow;
        _examResultRepository = examResultRepository;
        _examRepository = examRepository;
        _resultPolicyService = resultPolicyService;
        _finalResultRepository = finalResultRepository;
    }

    public async Task CalculateClassMeritPositionsAsync(int examId, int classId)
    {
        var classResults = await GetSortedResultsAsync(examId, classId);
        int position = 1;
        foreach (var result in classResults)
        {
            result.ClassPosition = position++;
            _examResultRepository.Update(result);
        }
        await _uow.SaveChangesAsync();
    }

    public async Task CalculateSectionMeritPositionsAsync(int examId, int classId)
    {
        var allResults = await GetSortedResultsAsync(examId, classId);
        var sectionGroups = allResults.GroupBy(r => r.SectionId);
        foreach (var sectionGroup in sectionGroups)
        {
            int position = 1;
            foreach (var result in sectionGroup)
            {
                result.Position = position++;
                _examResultRepository.Update(result);
            }
        }
        await _uow.SaveChangesAsync();
    }

    public async Task CalculateGroupMeritPositionsAsync(int examId)
    {
        var groupResults = await _examResultRepository.QueryNoTracking()
            .Include(r => r.Student)
            .ThenInclude(s => s.StudentGroup)
            .Where(r => r.ExamId == examId && r.Student.StudentGroupId != null)
            .ToListAsync();

        var groupGroups = groupResults.GroupBy(r => r.Student.StudentGroupId);
        foreach (var group in groupGroups)
        {
            var sorted = ApplyConfiguredSorting(group.AsQueryable()).ToList();
            int position = 1;
            foreach (var result in sorted)
            {
                result.GroupPosition = position++;
                _examResultRepository.Update(result);
            }
        }
        await _uow.SaveChangesAsync();
    }

    public async Task RecalculateMeritPositionsAsync(int examId)
    {
        var exam = await _examRepository.QueryNoTracking()
            .Include(e => e.ExamSubjects)
            .ThenInclude(es => es.Subject)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null) return;

        await _examResultRepository.CalculateMeritBySpAsync(exam.Name);
    }

    /// <summary>
    /// Calculate all 4 FinalResult positions for an academic year.
    /// Called after GenerateFinalResultsAsync to assign FinalPosition, FinalClassPosition,
    /// FinalSectionPosition, FinalGroupPosition.
    /// </summary>
    public async Task CalculateFinalResultPositionsAsync(int academicYearId, CancellationToken ct = default)
    {
        await _finalResultRepository.CalculateFinalPositionsBySpAsync(academicYearId, ct);
    }

    public async Task<IEnumerable<MeritListItem>> GetMeritListAsync(int examId, MeritCategory category)
    {
        IQueryable<StudentExamResult> query = _examResultRepository.Query()
            .AsNoTracking()
            .Include(r => r.Student)
            .ThenInclude(s => s.Section)
            .Include(r => r.Student.StudentGroup)
            .Where(r => r.ExamId == examId);

        switch (category)
        {
            case MeritCategory.Class:
                query = query.OrderByDescending(r => r.ClassPosition);
                break;
            case MeritCategory.Section:
                query = query.OrderByDescending(r => r.Position);
                break;
            case MeritCategory.Group:
                query = query.Where(r => r.GroupPosition.HasValue)
                            .OrderByDescending(r => r.GroupPosition);
                break;
            case MeritCategory.School:
                query = ApplyConfiguredSorting(query);
                break;
        }

        var results = await query.ToListAsync();

        return results.Select(r => new MeritListItem
        {
            StudentId = r.StudentId,
            StudentName = r.Student.FullName,
            RollNumber = r.Student.RollNumber,
            GPA = r.Gpa,
            TotalMarks = r.TotalMarks,
            Position = GetPositionForCategory(r, category),
            Grade = r.Grade,
            Section = r.Student.Section?.Name ?? "",
            StudentGroup = r.Student.StudentGroup?.Name ?? ""
        });
    }

    public async Task<IEnumerable<TopPerformer>> GetTopPerformersAsync(int examId, int count = 10)
    {
        var topResults = await _examResultRepository.Query()
            .AsNoTracking()
            .Include(r => r.Student)
            .Where(r => r.ExamId == examId)
            .OrderByDescending(r => r.Gpa)
            .ThenByDescending(r => r.TotalMarks)
            .Take(count)
            .ToListAsync();

        return topResults.Select((r, index) => new TopPerformer
        {
            StudentId = r.StudentId,
            StudentName = r.Student.FullName,
            RollNumber = r.Student.RollNumber,
            GPA = r.Gpa,
            Grade = r.Grade,
            Position = index + 1
        });
    }

    private async Task<List<StudentExamResult>> GetSortedResultsAsync(int examId, int classId)
    {
        var results = await _examResultRepository.Query()
            .AsNoTracking()
            .Include(r => r.Student)
            .Where(r => r.ExamId == examId && r.Student.ClassId == classId)
            .ToListAsync();

        return ApplyConfiguredSorting(results.AsQueryable()).ToList();
    }

    private IOrderedQueryable<StudentExamResult> ApplyConfiguredSorting(IQueryable<StudentExamResult> query)
    {
        return query
            .OrderByDescending(r => r.Gpa)
            .ThenByDescending(r => r.TotalMarks)
            .ThenBy(r => r.Student.RollNumber);
    }

    private IOrderedQueryable<FinalResult> ApplyConfiguredFinalSorting(IQueryable<FinalResult> query)
    {
        return query
            .OrderByDescending(fr => fr.FinalGpa)
            .ThenByDescending(fr => fr.WeightedTotalMarks)
            .ThenBy(fr => fr.SectionId);
    }

    private int GetPositionForCategory(StudentExamResult result, MeritCategory category)
    {
        return category switch
        {
            MeritCategory.Class => result.ClassPosition,
            MeritCategory.Section => result.Position,
            MeritCategory.Group => result.GroupPosition ?? 0,
            MeritCategory.School => result.Position,
            _ => 0
        };
    }
}
