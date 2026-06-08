using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Services.Implementations.Result;

/// <summary>
/// Comprehensive result calculation service for Bangladesh school system
/// Handles GPA calculation, merit positions, fail detection, and component aggregation
/// </summary>
public class ResultCalculationService : IResultCalculationService
{
    private readonly IUnitOfWork _uow;
    private readonly IExamRepository _examRepository;
    private readonly IMarkEntryRepository _markEntryRepository;
    private readonly IGradingRuleRepository _gradingRuleRepository;
    private readonly IStudentSubjectResultRepository _subjectResultRepository;
    private readonly IStudentExamResultRepository _examResultRepository;
    private readonly ISubjectMarkStructureService _markStructureService;
    private readonly IGradeCalculator _gradeCalculator;
    private readonly IComponentAggregator _componentAggregator;
    private readonly IPassFailPolicy _passFailPolicy;

    public ResultCalculationService(
        IUnitOfWork uow,
        IExamRepository examRepository,
        IMarkEntryRepository markEntryRepository,
        IGradingRuleRepository gradingRuleRepository,
        IStudentSubjectResultRepository subjectResultRepository,
        IStudentExamResultRepository examResultRepository,
        ISubjectMarkStructureService markStructureService,
        IGradeCalculator gradeCalculator,
        IComponentAggregator componentAggregator,
        IPassFailPolicy passFailPolicy)
    {
        _uow = uow;
        _examRepository = examRepository;
        _markEntryRepository = markEntryRepository;
        _gradingRuleRepository = gradingRuleRepository;
        _subjectResultRepository = subjectResultRepository;
        _examResultRepository = examResultRepository;
        _markStructureService = markStructureService;
        _gradeCalculator = gradeCalculator;
        _componentAggregator = componentAggregator;
        _passFailPolicy = passFailPolicy;
    }

    private async Task<ResultSetting> GetResultSettingAsync(int academicYearId, CancellationToken ct = default)
    {
        var setting = await _uow.Repository<ResultSetting>().Query()
            .Where(rs => rs.AcademicYearId == academicYearId && rs.IsActive && !rs.IsDeleted)
            .FirstOrDefaultAsync(ct);

        return setting ?? new ResultSetting
        {
            OptionalSubjectMode = OptionalSubjectMode.ExcludeFromGPA,
            FailSubjectMode = FailSubjectMode.StrictFail,
            OptionalBonusMaxGPA = 0.50m,
            BestOfCount = 1,
            RequirePassedOptionalOnly = true,
            MaxFailedCompulsoryAllowed = 0,
            MinimumPromotionGPA = 1.00m,
            IncludeReligionInGPA = true,
            AutoCalculateComponentTotal = true,
            GpaRoundingPrecision = 2
        };
    }

    public async Task CalculateExamResultsAsync(int examId)
    {
        if (!await CanCalculateResultsAsync(examId))
            throw new InvalidOperationException("Cannot calculate results - exam may be locked or published");

        await CalculateSubjectResultsAsync(examId);

        var exam = await _examRepository.Query()
            .Include(e => e.ExamSubjects)
            .ThenInclude(es => es.Subject)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null) throw new ArgumentException("Exam not found");

        var classId = await _examResultRepository.Query()
            .Include(r => r.Student)
            .Where(r => r.ExamId == examId)
            .Select(r => r.Student.ClassId)
            .FirstOrDefaultAsync();

        var students = await _uow.Repository<Student>().Query()
            .Where(s => s.ClassId == classId)
            .ToListAsync();

        var allSubjectResults = await _subjectResultRepository.Query()
            .Include(r => r.Subject)
            .Where(r => r.ExamId == examId && students.Select(s => s.Id).Contains(r.StudentId))
            .ToListAsync();

        var subjectResultsByStudent = allSubjectResults.GroupBy(r => r.StudentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var newExamResults = new List<StudentExamResult>();

        foreach (var student in students)
        {
            if (subjectResultsByStudent.TryGetValue(student.Id, out var studentSubjectResults))
            {
                var examResult = await CalculateStudentExamResultInternalAsync(examId, student.Id, studentSubjectResults);
                if (examResult != null) newExamResults.Add(examResult);
            }
        }

        if (newExamResults.Any())
        {
            var existingResults = await _examResultRepository.Query()
                .Where(r => r.ExamId == examId && students.Select(s => s.Id).Contains(r.StudentId))
                .ToListAsync();
            _examResultRepository.RemoveRange(existingResults);

            await _examResultRepository.AddRangeAsync(newExamResults);
            await _uow.SaveChangesAsync();
        }

        await CalculateMeritPositionsAsync(examId);
    }

    public async Task CalculateSubjectResultsAsync(int examId)
    {
        var markEntries = await _markEntryRepository.Query()
            .Include(m => m.Student)
            .Include(m => m.Subject)
            .Where(m => m.ExamId == examId)
            .ToListAsync();

        var gradingRules = await _gradingRuleRepository.ListAsync();
        var examSubjects = await _uow.Repository<ExamSubject>().Query()
            .Where(es => es.ExamId == examId)
            .ToDictionaryAsync(es => es.SubjectId);

        int? classId = await _uow.Repository<Student>().Query()
            .Where(s => s.Id == markEntries.FirstOrDefault()!.StudentId)
            .Select(s => s.ClassId)
            .FirstOrDefaultAsync();

        var studentIds = markEntries.Select(m => m.StudentId).Distinct().ToList();
        var studentGroups = await _uow.Repository<Student>().Query()
            .AsNoTracking()
            .Where(s => studentIds.Contains(s.Id))
            .Select(s => new { s.Id, s.StudentGroupId, s.AssignedReligionSubjectId })
            .ToListAsync();

        var classSubjects = await _uow.Repository<ClassSubject>().Query()
            .Include(cs => cs.Subject)
            .Where(cs => cs.SchoolClassId == classId && !cs.IsDeleted && cs.IsActive)
            .ToListAsync();

        var classSubjectLookup = classSubjects
            .GroupBy(cs => cs.SubjectId)
            .ToDictionary(g => g.Key, g => g.First());

        var existingResults = await _subjectResultRepository.Query()
            .Where(r => r.ExamId == examId)
            .ToListAsync();
        _subjectResultRepository.RemoveRange(existingResults);

        var newSubjectResults = new List<StudentSubjectResult>();
        foreach (var markEntry in markEntries)
        {
            var classSubject = classSubjectLookup.GetValueOrDefault(markEntry.SubjectId);
            if (classSubject == null) continue;

            // Religion subject filtering: only include if it matches the student's assigned religion
            if (classSubject.IsReligionSubject && markEntry.Student != null)
            {
                var studentReligionSubjectId = markEntry.Student.AssignedReligionSubjectId;
                if (studentReligionSubjectId.HasValue && markEntry.SubjectId != studentReligionSubjectId.Value)
                    continue;
            }

            // Group subject filtering: only include if the subject belongs to the student's group
            // or is a common (non-group) subject
            var studentGroup = studentGroups.FirstOrDefault(sg => sg.Id == markEntry.StudentId);
            if (classSubject.IsGroupSubject && studentGroup != null)
            {
                if (classSubject.StudentGroupId.HasValue && studentGroup.StudentGroupId.HasValue)
                {
                    if (classSubject.StudentGroupId.Value != studentGroup.StudentGroupId.Value)
                        continue;
                }
            }

            examSubjects.TryGetValue(markEntry.SubjectId, out var examSubject);
            var result = CalculateSubjectResultInternal(markEntry, gradingRules, examSubject,
                classSubject);
            newSubjectResults.Add(result);
        }

        if (newSubjectResults.Any())
        {
            await _subjectResultRepository.AddRangeAsync(newSubjectResults);
            await _uow.SaveChangesAsync();
        }
    }

    public async Task CalculateExamResultsWithOptionsAsync(int examId, int? classId = null, int? groupId = null)
    {
        await CalculateExamResultsAsync(examId);
    }

    public async Task<StudentSubjectResult> CalculateSubjectResultWithMappingAsync(MarkEntry markEntry, IEnumerable<GradingRule> gradingRules, ExamSubject? examSubject, ClassSubject? classSubject)
    {
        return CalculateSubjectResultInternal(markEntry, gradingRules, examSubject, classSubject);
    }

    public async Task CalculateMeritPositionsAsync(int examId)
    {
        var classResults = await _examResultRepository.Query()
            .Include(r => r.Student)
            .Where(r => r.ExamId == examId)
            .OrderByDescending(r => r.Gpa)
            .ThenByDescending(r => r.TotalMarks)
            .ToListAsync();

        // Class-wide merit with tie handling
        int position = 1;
        foreach (var tieGroup in classResults.GroupBy(r => new { r.Gpa, r.TotalMarks }))
        {
            foreach (var result in tieGroup)
            {
                result.Position = position;
                result.ClassPosition = position;
            }
            position += tieGroup.Count();
        }

        // Section merit with tie handling
        foreach (var sectionGroup in classResults.GroupBy(r => r.Student.SectionId))
        {
            int sectionPosition = 1;
            foreach (var tieGroup in sectionGroup
                .OrderByDescending(r => r.Gpa)
                .ThenByDescending(r => r.TotalMarks)
                .GroupBy(r => new { r.Gpa, r.TotalMarks }))
            {
                foreach (var result in tieGroup)
                    result.Position = sectionPosition;
                sectionPosition += tieGroup.Count();
            }
        }

        // Group merit with tie handling (reuse classResults, avoid extra query)
        foreach (var group in classResults
            .Where(r => r.Student.StudentGroupId != null)
            .GroupBy(r => r.Student.StudentGroupId))
        {
            int groupPosition = 1;
            foreach (var tieGroup in group
                .OrderByDescending(r => r.Gpa)
                .ThenByDescending(r => r.TotalMarks)
                .GroupBy(r => new { r.Gpa, r.TotalMarks }))
            {
                foreach (var result in tieGroup)
                    result.GroupPosition = groupPosition;
                groupPosition += tieGroup.Count();
            }
        }

        await _uow.SaveChangesAsync();
    }

    public async Task<decimal> CalculateGpaAsync(IEnumerable<StudentSubjectResult> subjectResults)
    {
        if (!subjectResults.Any()) return 0;

        var examId = subjectResults.First().ExamId;
        var exam = await _examRepository.GetByIdAsync(examId);
        var setting = await GetResultSettingAsync(exam?.AcademicYearId ?? 0);

        return CalculateGpaAsync(subjectResults, setting);
    }

    public decimal CalculateGpaAsync(IEnumerable<StudentSubjectResult> subjectResults, ResultSetting setting)
    {
        if (!subjectResults.Any()) return 0;

        var validResults = subjectResults.Where(r => r.IsPassed).ToList();
        if (!validResults.Any()) return 0;

        var compulsoryResults = validResults.Where(r => !r.IsOptionalSubject).ToList();
        var optionalResults = validResults.Where(r => r.IsOptionalSubject).ToList();

        decimal totalPoints = compulsoryResults.Sum(r => r.GradePoint);
        int subjectCount = compulsoryResults.Count;

        if (setting.OptionalSubjectMode == OptionalSubjectMode.BonusGPA && optionalResults.Any())
        {
            var passedOptional = setting.RequirePassedOptionalOnly
                ? optionalResults.Where(r => r.IsPassed).ToList()
                : optionalResults;

            var bestOptional = passedOptional
                .OrderByDescending(r => r.GradePoint)
                .Take(setting.BestOfCount)
                .ToList();

            totalPoints += bestOptional.Sum(r => r.GradePoint);
            subjectCount += bestOptional.Count;
        }
        else if (setting.OptionalSubjectMode == OptionalSubjectMode.BestOf && optionalResults.Any())
        {
            var passedOptional = setting.RequirePassedOptionalOnly
                ? optionalResults.Where(r => r.IsPassed).ToList()
                : optionalResults;

            var bestOptional = passedOptional
                .OrderByDescending(r => r.GradePoint)
                .Take(setting.BestOfCount)
                .ToList();

            totalPoints += bestOptional.Sum(r => r.GradePoint);
            subjectCount += bestOptional.Count;
        }
        else if (setting.OptionalSubjectMode == OptionalSubjectMode.IncludeInGPA && optionalResults.Any())
        {
            totalPoints += optionalResults.Sum(r => r.GradePoint);
            subjectCount += optionalResults.Count;
        }

        if (setting.IncludeReligionInGPA == false)
        {
            var religionResults = validResults.Where(r => r.IsReligionSubject).ToList();
            totalPoints -= religionResults.Sum(r => r.GradePoint);
            subjectCount -= religionResults.Count;
        }

        if (subjectCount <= 0) return 0;

        var gpa = totalPoints / subjectCount;
        return Math.Round(gpa, setting.GpaRoundingPrecision);
    }

    public async Task<decimal> CalculateFinalGpaAsync(int studentId, int academicYearId)
    {
        var examResults = await _examResultRepository.Query()
            .Include(r => r.Exam)
            .Where(r => r.StudentId == studentId && r.Exam.AcademicYearId == academicYearId)
            .ToListAsync();

        if (!examResults.Any()) return 0;

        decimal totalGpa = examResults.Sum(r => r.Gpa);
        return Math.Round(totalGpa / examResults.Count, 2);
    }

    public async Task<(bool IsPassed, int FailedSubjectCount)> DeterminePassFailStatusAsync(int studentId, int examId)
    {
        var subjectResults = await _subjectResultRepository.Query()
            .Include(r => r.Subject)
            .Where(r => r.StudentId == studentId && r.ExamId == examId)
            .ToListAsync();

        var exam = await _examRepository.GetByIdAsync(examId);
        var setting = await GetResultSettingAsync(exam?.AcademicYearId ?? 0);

        return DeterminePassFailStatus(subjectResults, setting);
    }

    public (bool IsPassed, int FailedSubjectCount) DeterminePassFailStatus(IEnumerable<StudentSubjectResult> subjectResults, ResultSetting setting)
    {
        return _passFailPolicy.DeterminePassFailStatus(subjectResults, setting);
    }

    public async Task RecalculateResultsAsync(int examId, int studentId)
    {
        var subjectResults = await _subjectResultRepository.Query()
            .Include(r => r.Subject)
            .Where(r => r.ExamId == examId && r.StudentId == studentId)
            .ToListAsync();

        _subjectResultRepository.RemoveRange(subjectResults);

        var markEntries = await _markEntryRepository.Query()
            .Where(m => m.ExamId == examId && m.StudentId == studentId)
            .ToListAsync();

        var gradingRules = await _gradingRuleRepository.ListAsync();
        var examSubjects = await _uow.Repository<ExamSubject>().Query()
            .Where(es => es.ExamId == examId)
            .ToDictionaryAsync(es => es.SubjectId);

        int? classId = await _uow.Repository<Student>().Query()
            .Where(s => s.Id == studentId)
            .Select(s => s.ClassId)
            .FirstOrDefaultAsync();

        var classSubjects = await _uow.Repository<ClassSubject>().Query()
            .Include(cs => cs.Subject)
            .Where(cs => cs.SchoolClassId == classId && !cs.IsDeleted && cs.IsActive)
            .ToListAsync();

        var classSubjectLookup = classSubjects
            .GroupBy(cs => cs.SubjectId)
            .ToDictionary(g => g.Key, g => g.First());

        var student = await _uow.Repository<Student>().Query()
            .AsNoTracking()
            .Where(s => s.Id == studentId)
            .Select(s => new { s.AssignedReligionSubjectId, s.StudentGroupId })
            .FirstOrDefaultAsync();

        var newSubjectResults = new List<StudentSubjectResult>();
        foreach (var markEntry in markEntries)
        {
            var classSubject = classSubjectLookup.GetValueOrDefault(markEntry.SubjectId);
            if (classSubject == null) continue;

            // Religion subject filtering
            if (classSubject.IsReligionSubject && student != null)
            {
                if (student.AssignedReligionSubjectId.HasValue && markEntry.SubjectId != student.AssignedReligionSubjectId.Value)
                    continue;
            }

            // Group subject filtering
            if (classSubject.IsGroupSubject && student != null)
            {
                if (classSubject.StudentGroupId.HasValue && student.StudentGroupId.HasValue)
                {
                    if (classSubject.StudentGroupId.Value != student.StudentGroupId.Value)
                        continue;
                }
            }

            examSubjects.TryGetValue(markEntry.SubjectId, out var examSubject);
            newSubjectResults.Add(CalculateSubjectResultInternal(markEntry, gradingRules, examSubject, classSubject));
        }

        await _subjectResultRepository.AddRangeAsync(newSubjectResults);
        await _uow.SaveChangesAsync();

        var existingExamResult = await _examResultRepository.FirstOrDefaultAsync(r => r.ExamId == examId && r.StudentId == studentId);
        if (existingExamResult != null) _examResultRepository.Remove(existingExamResult);

        var newExamResult = await CalculateStudentExamResultInternalAsync(examId, studentId, newSubjectResults);
        if (newExamResult != null)
        {
            await _examResultRepository.AddAsync(newExamResult);
            await _uow.SaveChangesAsync();
        }
    }

    public async Task<decimal> AggregateComponentMarksAsync(MarkEntry markEntry)
    {
        var components = await _markStructureService.GetGridColumnsAsync(
            markEntry.ExamId, markEntry.SubjectId);

        if (components.Count == 0)
            return _componentAggregator.AggregateAll(markEntry);

        return _componentAggregator.Aggregate(markEntry, components);
    }

    public async Task<bool> CanCalculateResultsAsync(int examId)
    {
        var exam = await _examRepository.GetByIdAsync(examId);
        if (exam == null) return false;

        var publication = await _uow.Repository<ResultPublication>().FirstOrDefaultAsync(p => p.ExamId == examId);
        return exam.Status != ResultWorkflowStatus.Published && (publication == null || !publication.IsLocked);
    }

    public async Task<IDictionary<int, (int Passed, int Failed)>> GetSubjectPassFailStatsAsync(int examId)
    {
        var stats = await _subjectResultRepository.Query()
            .Where(r => r.ExamId == examId)
            .GroupBy(r => r.SubjectId)
            .Select(g => new
            {
                SubjectId = g.Key,
                Passed = g.Count(r => r.IsPassed),
                Failed = g.Count(r => !r.IsPassed)
            })
            .ToDictionaryAsync(x => x.SubjectId, x => (x.Passed, x.Failed));

        return stats;
    }

    private StudentSubjectResult CalculateSubjectResultInternal(MarkEntry markEntry, IEnumerable<GradingRule> gradingRules, ExamSubject? examSubject, ClassSubject? classSubject = null)
    {
        decimal totalMarks = AggregateComponentMarksInternal(markEntry);
        var (grade, gradePoint) = _gradeCalculator.CalculateGrade(totalMarks, gradingRules);
        var resolvedGradePoint = gradePoint ?? 0;
        bool isPassed = totalMarks >= (examSubject?.PassMarks ?? classSubject?.PassMarks ?? 33);

        return new StudentSubjectResult
        {
            ExamId = markEntry.ExamId,
            StudentId = markEntry.StudentId,
            SubjectId = markEntry.SubjectId,
            AcademicYearId = markEntry.AcademicYearId,
            ClassId = markEntry.ClassId,
            SectionId = markEntry.SectionId,
            IsOptionalSubject = classSubject?.IsOptional ?? false,
            IsReligionSubject = classSubject?.IsReligionSubject ?? false,
            MarksObtained = totalMarks,
            FullMarks = examSubject?.FullMarks ?? classSubject?.FullMarks ?? 100,
            PassMarks = examSubject?.PassMarks ?? classSubject?.PassMarks ?? 33,
            Grade = grade ?? "F",
            GradePoint = resolvedGradePoint,
            IsPassed = isPassed,
            CalculatedAt = DateTime.Now
        };
    }

    private async Task<StudentExamResult?> CalculateStudentExamResultInternalAsync(int examId, int studentId, IEnumerable<StudentSubjectResult> subjectResults)
    {
        if (!subjectResults.Any()) return null;

        decimal totalMarks = subjectResults.Sum(r => r.MarksObtained);
        decimal totalFullMarks = subjectResults.Sum(r => r.FullMarks);

        var exam = await _examRepository.GetByIdAsync(examId);
        var setting = await GetResultSettingAsync(exam?.AcademicYearId ?? 0);

        decimal gpa = CalculateGpaAsync(subjectResults, setting);
        var (isPassed, failedCount) = _passFailPolicy.DeterminePassFailStatus(subjectResults, setting);

        var first = subjectResults.First();
        var student = await _uow.Repository<Student>().Query()
            .Where(s => s.Id == studentId)
            .Select(s => new { s.ClassId, s.SectionId, s.StudentGroupId })
            .FirstOrDefaultAsync();

        return new StudentExamResult
        {
            ExamId = examId,
            StudentId = studentId,
            AcademicYearId = first.AcademicYearId,
            ClassId = student?.ClassId ?? first.ClassId,
            SectionId = student?.SectionId ?? first.SectionId,
            StudentGroupId = student?.StudentGroupId,
            TotalMarks = totalMarks,
            TotalFullMarks = totalFullMarks,
            Gpa = gpa,
            Grade = _gradeCalculator.GetOverallGrade(gpa),
            IsPassed = isPassed,
            FailedSubjectCount = failedCount,
            PassedSubjectCount = subjectResults.Count(r => r.IsPassed),
            Status = ResultWorkflowStatus.Draft,
            CalculatedAt = DateTime.Now
        };
    }

    private decimal AggregateComponentMarksInternal(MarkEntry markEntry)
    {
        return _componentAggregator.AggregateAll(markEntry);
    }

    public async Task CalculateSubjectResultAsync(MarkEntry markEntry)
    {
        var gradingRules = await _gradingRuleRepository.ListAsync();
        var examSubject = await _uow.Repository<ExamSubject>().FirstOrDefaultAsync(es => es.ExamId == markEntry.ExamId && es.SubjectId == markEntry.SubjectId);

        int? classId = await _uow.Repository<Student>().Query()
            .Where(s => s.Id == markEntry.StudentId)
            .Select(s => s.ClassId)
            .FirstOrDefaultAsync();

        var classSubject = await _uow.Repository<ClassSubject>().Query()
            .Where(cs => cs.SchoolClassId == classId && cs.SubjectId == markEntry.SubjectId && !cs.IsDeleted && cs.IsActive)
            .FirstOrDefaultAsync();

        var result = CalculateSubjectResultInternal(markEntry, gradingRules, examSubject, classSubject);
        await _subjectResultRepository.AddAsync(result);
        await _uow.SaveChangesAsync();
    }

    public async Task CalculateStudentExamResultAsync(int examId, int studentId)
    {
        var subjectResults = await _subjectResultRepository.Query()
            .Include(r => r.Subject)
            .Where(r => r.ExamId == examId && r.StudentId == studentId)
            .ToListAsync();

        var result = await CalculateStudentExamResultInternalAsync(examId, studentId, subjectResults);
        if (result != null)
        {
            await _examResultRepository.AddAsync(result);
            await _uow.SaveChangesAsync();
        }
    }

}
