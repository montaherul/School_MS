using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.DTOs.Result;
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
    private readonly IMeritCalculationService _meritCalculationService;

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
        IPassFailPolicy passFailPolicy,
        IMeritCalculationService meritCalculationService)
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
        _meritCalculationService = meritCalculationService;
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

        var classIds = await _examResultRepository.Query()
            .Include(r => r.Student)
            .Where(r => r.ExamId == examId)
            .Select(r => r.Student.ClassId)
            .Distinct()
            .ToListAsync();

        var students = await _uow.Repository<Student>().Query()
            .Where(s => classIds.Contains(s.ClassId))
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
                var examResult = await CalculateStudentExamResultInternalAsync(examId, student.Id, studentSubjectResults, student.ClassId, student.SectionId, student.StudentGroupId);
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

        await _meritCalculationService.RecalculateMeritPositionsAsync(examId);
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

        var studentIds = markEntries.Select(m => m.StudentId).Distinct().ToList();

        // Get all distinct class IDs from the students in mark entries
        var classIds = await _uow.Repository<Student>().Query()
            .Where(s => studentIds.Contains(s.Id))
            .Select(s => s.ClassId)
            .Distinct()
            .ToListAsync();

        var studentGroups = await _uow.Repository<Student>().Query()
            .AsNoTracking()
            .Where(s => studentIds.Contains(s.Id))
            .Select(s => new { s.Id, s.ClassId, s.StudentGroupId, s.AssignedReligionSubjectId })
            .ToListAsync();

        var studentClassLookup = studentGroups.ToDictionary(sg => sg.Id, sg => sg.ClassId);
        var studentGroupLookup = studentGroups.ToDictionary(sg => sg.Id, sg => sg);

        var classSubjects = await _uow.Repository<ClassSubject>().Query()
            .Include(cs => cs.Subject)
            .Where(cs => classIds.Contains(cs.SchoolClassId) && !cs.IsDeleted && cs.IsActive)
            .ToListAsync();

        var classSubjectLookup = classSubjects
            .GroupBy(cs => cs.SubjectId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var existingResults = await _subjectResultRepository.Query()
            .Where(r => r.ExamId == examId)
            .ToListAsync();
        _subjectResultRepository.RemoveRange(existingResults);

        var newSubjectResults = new List<StudentSubjectResult>();
        foreach (var markEntry in markEntries)
        {
            var studentClassId = studentClassLookup.GetValueOrDefault(markEntry.StudentId);
            var classSubjectsForSubject = classSubjectLookup.GetValueOrDefault(markEntry.SubjectId);
            if (classSubjectsForSubject == null || classSubjectsForSubject.Count == 0) continue;

            // Match the class subject to the student's class
            var classSubject = classSubjectsForSubject.FirstOrDefault(cs => cs.SchoolClassId == studentClassId);
            if (classSubject == null) continue;

            // Religion subject filtering: only include if it matches the student's assigned religion
            var studentInfo = studentGroupLookup.GetValueOrDefault(markEntry.StudentId);
            if (classSubject.IsReligionSubject && studentInfo != null)
            {
                if (studentInfo.AssignedReligionSubjectId.HasValue && markEntry.SubjectId != studentInfo.AssignedReligionSubjectId.Value)
                    continue;
            }

            // Group subject filtering: only include if the subject belongs to the student's group
            if (classSubject.IsGroupSubject && studentInfo != null)
            {
                if (classSubject.StudentGroupId.HasValue && studentInfo.StudentGroupId.HasValue)
                {
                    if (classSubject.StudentGroupId.Value != studentInfo.StudentGroupId.Value)
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
        return Math.Round(gpa, setting.GpaRoundingPrecision, MidpointRounding.AwayFromZero);
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

    public async Task<FinalResultGenerationResult> GenerateFinalResultsAsync(int academicYearId)
    {
        var result = new FinalResultGenerationResult { AcademicYearId = academicYearId };

        var examIds = await _uow.Repository<Models.Entities.Exam.Exam>().Query()
            .Where(e => e.AcademicYearId == academicYearId && !e.IsDeleted)
            .Select(e => e.Id)
            .ToListAsync();

        if (!examIds.Any())
        {
            result.Errors.Add("No exams found for this academic year.");
            return result;
        }

        var examResults = await _examResultRepository.Query()
            .Include(r => r.Student)
            .Where(r => examIds.Contains(r.ExamId))
            .ToListAsync();

        var studentGroups = examResults
            .GroupBy(r => r.StudentId)
            .ToList();

        result.TotalStudents = studentGroups.Count;

        var existingFinalResults = await _uow.Repository<FinalResult>().Query()
            .Where(fr => fr.AcademicYearId == academicYearId)
            .ToListAsync();
        var existingByStudent = existingFinalResults.ToDictionary(fr => fr.StudentId);

        foreach (var group in studentGroups)
        {
            try
            {
                var first = group.First();
                var student = first.Student;
                if (student == null) continue;

                var gpa = Math.Round(group.Average(r => r.Gpa), 2);
                var totalFailed = group.Count(r => !r.IsPassed);
                var isPassed = totalFailed == 0;
                var grade = CalculateGradeFromGpa(gpa);

                if (existingByStudent.TryGetValue(student.Id, out var existing))
                {
                    existing.FinalGpa = gpa;
                    existing.FinalGrade = grade;
                    existing.IsPassed = isPassed;
                    existing.TotalFailedSubjects = totalFailed;
                    existing.SchoolClassId = student.ClassId;
                    existing.SectionId = student.SectionId;
                    existing.StudentGroupId = student.StudentGroupId;
                    existing.CalculatedAt = DateTime.Now;
                    _uow.Repository<FinalResult>().Update(existing);
                    result.UpdatedCount++;
                }
                else
                {
                    await _uow.Repository<FinalResult>().AddAsync(new FinalResult
                    {
                        AcademicYearId = academicYearId,
                        StudentId = student.Id,
                        SchoolClassId = student.ClassId,
                        SectionId = student.SectionId,
                        StudentGroupId = student.StudentGroupId,
                        FinalGpa = gpa,
                        FinalGrade = grade,
                        IsPassed = isPassed,
                        TotalFailedSubjects = totalFailed,
                        PromotionStatus = PromotionStatus.Pending,
                        CalculatedAt = DateTime.Now
                    });
                    result.GeneratedCount++;
                }
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                result.Errors.Add($"Student {group.Key}: {ex.Message}");
            }
        }

        await _uow.SaveChangesAsync();

        result.GeneratedCount += result.UpdatedCount;

        return result;
    }

    private static string CalculateGradeFromGpa(decimal gpa)
    {
        return gpa switch
        {
            >= 5.00m => "A+",
            >= 4.00m => "A",
            >= 3.50m => "A-",
            >= 3.00m => "B",
            >= 2.00m => "C",
            >= 1.00m => "D",
            _ => "F"
        };
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
            markEntry.SubjectId);

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
            StudentGroupId = markEntry.StudentGroupId,
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

    private async Task<StudentExamResult?> CalculateStudentExamResultInternalAsync(int examId, int studentId, IEnumerable<StudentSubjectResult> subjectResults, int? classIdOverride = null, int? sectionIdOverride = null, int? groupIdOverride = null)
    {
        if (!subjectResults.Any()) return null;

        decimal totalMarks = subjectResults.Sum(r => r.MarksObtained);
        decimal totalFullMarks = subjectResults.Sum(r => r.FullMarks);

        var exam = await _examRepository.GetByIdAsync(examId);
        var setting = await GetResultSettingAsync(exam?.AcademicYearId ?? 0);

        decimal gpa = CalculateGpaAsync(subjectResults, setting);
        var (isPassed, failedCount) = _passFailPolicy.DeterminePassFailStatus(subjectResults, setting);

        var first = subjectResults.First();
        int resolvedClassId, resolvedSectionId;
        int? resolvedGroupId;
        if (classIdOverride.HasValue)
        {
            resolvedClassId = classIdOverride.Value;
            resolvedSectionId = sectionIdOverride ?? first.SectionId;
            resolvedGroupId = groupIdOverride;
        }
        else
        {
            var student = await _uow.Repository<Student>().Query()
                .Where(s => s.Id == studentId)
                .Select(s => new { s.ClassId, s.SectionId, s.StudentGroupId })
                .FirstOrDefaultAsync();
            resolvedClassId = student?.ClassId ?? first.ClassId;
            resolvedSectionId = student?.SectionId ?? first.SectionId;
            resolvedGroupId = student?.StudentGroupId;
        }

        return new StudentExamResult
        {
            ExamId = examId,
            StudentId = studentId,
            AcademicYearId = first.AcademicYearId,
            ClassId = resolvedClassId,
            SectionId = resolvedSectionId,
            StudentGroupId = resolvedGroupId,
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
