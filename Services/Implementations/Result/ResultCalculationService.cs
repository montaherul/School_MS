using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

/// <summary>
/// Comprehensive result calculation service for Bangladesh school system
/// Handles GPA calculation, merit positions, fail detection, and component aggregation
/// </summary>
public class ResultCalculationService : IResultCalculationService
{
    private readonly IUnitOfWork _uow;
    private readonly SchoolDbContext _db;

    public ResultCalculationService(IUnitOfWork uow, SchoolDbContext db)
    {
        _uow = uow;
        _db = db;
    }

    /// <summary>
    /// Bangladesh grading system:
    /// 80-100 = A+ = 5.00, 70-79 = A = 4.00, 60-69 = A- = 3.50,
    /// 50-59 = B = 3.00, 40-49 = C = 2.00, 33-39 = D = 1.00, 0-32 = F = 0.00
    /// </summary>
    private readonly Dictionary<(decimal Min, decimal Max), (string Grade, decimal Point)> _bangladeshGradingRules = new()
    {
        {(80, 100), ("A+", 5.00m)},
        {(70, 79.99m), ("A", 4.00m)},
        {(60, 69.99m), ("A-", 3.50m)},
        {(50, 59.99m), ("B", 3.00m)},
        {(40, 49.99m), ("C", 2.00m)},
        {(33, 39.99m), ("D", 1.00m)},
        {(0, 32.99m), ("F", 0.00m)}
    };

    public async Task CalculateExamResultsAsync(int examId)
    {
        // Validate calculation can proceed
        if (!await CanCalculateResultsAsync(examId))
            throw new InvalidOperationException("Cannot calculate results - exam may be locked or published");

        // Calculate subject results first
        await CalculateSubjectResultsAsync(examId);

        // Get all students for this exam
        var exam = await _db.Exams
            .Include(e => e.ExamSubjects)
            .ThenInclude(es => es.Subject)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null) throw new ArgumentException("Exam not found");

        var classId = await _db.StudentExamResults
            .Include(r => r.Student)
            .Where(r => r.ExamId == examId)
            .Select(r => r.Student.ClassId)
            .FirstOrDefaultAsync();

        var students = await _db.Students
            .Where(s => s.ClassId == classId)
            .ToListAsync();

        // Fetch all subject results for all students in this exam to avoid N+1 in DeterminePassFailStatus
        var allSubjectResults = await _db.StudentSubjectResults
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
                if (examResult != null)
                {
                    newExamResults.Add(examResult);
                }
            }
        }

        if (newExamResults.Any())
        {
            // Remove existing results first to avoid duplicates
            var existingResults = await _db.StudentExamResults
                .Where(r => r.ExamId == examId && students.Select(s => s.Id).Contains(r.StudentId))
                .ToListAsync();
            _db.StudentExamResults.RemoveRange(existingResults);

            await _db.StudentExamResults.AddRangeAsync(newExamResults);
            await _db.SaveChangesAsync();
        }

        // Calculate merit positions
        await CalculateMeritPositionsAsync(examId);
    }

    public async Task CalculateSubjectResultsAsync(int examId)
    {
        var markEntries = await _db.Marks
            .Include(m => m.Student)
            .Include(m => m.Subject)
            .Where(m => m.ExamId == examId)
            .ToListAsync();

        // Pre-fetch grading rules and exam subjects
        var gradingRules = await _db.GradingRules.ToListAsync();
        var examSubjects = await _db.ExamSubjects
            .Where(es => es.ExamId == examId)
            .ToDictionaryAsync(es => es.SubjectId);

        // Remove existing results for this exam
        var existingResults = await _db.StudentSubjectResults
            .Where(r => r.ExamId == examId)
            .ToListAsync();
        _db.StudentSubjectResults.RemoveRange(existingResults);

        var newSubjectResults = new List<StudentSubjectResult>();

        foreach (var markEntry in markEntries)
        {
            examSubjects.TryGetValue(markEntry.SubjectId, out var examSubject);
            var result = CalculateSubjectResultInternal(markEntry, gradingRules, examSubject);
            newSubjectResults.Add(result);
        }

        if (newSubjectResults.Any())
        {
            await _db.StudentSubjectResults.AddRangeAsync(newSubjectResults);
            await _db.SaveChangesAsync();
        }
    }

    public async Task CalculateMeritPositionsAsync(int examId)
    {
        // Calculate class positions
        var classResults = await _db.StudentExamResults
            .Include(r => r.Student)
            .Where(r => r.ExamId == examId)
            .OrderByDescending(r => r.Gpa)
            .ThenByDescending(r => r.TotalMarks)
            .ToListAsync();

        int position = 1;
        foreach (var result in classResults)
        {
            result.Position = position++;
            result.ClassPosition = result.Position;
        }

        // Calculate section positions
        var sectionGroups = classResults.GroupBy(r => r.Student.SectionId);
        foreach (var sectionGroup in sectionGroups)
        {
            int sectionPosition = 1;
            foreach (var result in sectionGroup.OrderByDescending(r => r.Gpa).ThenByDescending(r => r.TotalMarks))
            {
                result.Position = sectionPosition++; // Override with section position
            }
        }

        // Calculate group positions for Class 9-10
        var groupResults = await _db.StudentExamResults
            .Include(r => r.Student)
            .ThenInclude(s => s.StudentGroup)
            .Where(r => r.ExamId == examId && r.Student.StudentGroupId != null)
            .GroupBy(r => r.Student.StudentGroupId)
            .ToListAsync();

        foreach (var group in groupResults)
        {
            int groupPosition = 1;
            foreach (var result in group.OrderByDescending(r => r.Gpa).ThenByDescending(r => r.TotalMarks))
            {
                result.GroupPosition = groupPosition++;
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task<decimal> CalculateGpaAsync(IEnumerable<StudentSubjectResult> subjectResults)
    {
        if (!subjectResults.Any()) return 0;

        var validResults = subjectResults.Where(r => r.IsPassed && r.Subject.IsMandatory != false);
        if (!validResults.Any()) return 0;

        decimal totalPoints = validResults.Sum(r => r.GradePoint);
        int subjectCount = validResults.Count();

        return Math.Round(totalPoints / subjectCount, 2);
    }

    public async Task<decimal> CalculateFinalGpaAsync(int studentId, int academicYearId)
    {
        // Get all exam results for the year
        var examResults = await _db.StudentExamResults
            .Include(r => r.Exam)
            .Where(r => r.StudentId == studentId && r.Exam.AcademicYearId == academicYearId)
            .ToListAsync();

        if (!examResults.Any()) return 0;

        decimal totalGpa = examResults.Sum(r => r.Gpa);
        return Math.Round(totalGpa / examResults.Count, 2);
    }

    public async Task<(bool IsPassed, int FailedSubjectCount)> DeterminePassFailStatusAsync(int studentId, int examId)
    {
        var subjectResults = await _db.StudentSubjectResults
            .Include(r => r.Subject)
            .Where(r => r.StudentId == studentId && r.ExamId == examId)
            .ToListAsync();

        return DeterminePassFailStatus(subjectResults);
    }

    private (bool IsPassed, int FailedSubjectCount) DeterminePassFailStatus(IEnumerable<StudentSubjectResult> subjectResults)
    {
        int failedSubjects = subjectResults.Count(r => !r.IsPassed);
        
        // For Bangladesh system, mandatory subjects must be passed
        var failedMandatory = subjectResults.Count(r => !r.IsPassed && r.Subject != null && r.Subject.IsMandatory);
        bool isPassed = failedMandatory == 0;

        return (isPassed, failedSubjects);
    }

    public async Task RecalculateResultsAsync(int examId, int studentId)
    {
        // Recalculate subject results
        var subjectResults = await _db.StudentSubjectResults
            .Include(r => r.Subject)
            .Where(r => r.ExamId == examId && r.StudentId == studentId)
            .ToListAsync();

        _db.StudentSubjectResults.RemoveRange(subjectResults);

        var markEntries = await _db.Marks
            .Where(m => m.ExamId == examId && m.StudentId == studentId)
            .ToListAsync();

        var gradingRules = await _db.GradingRules.ToListAsync();
        var examSubjects = await _db.ExamSubjects
            .Where(es => es.ExamId == examId)
            .ToDictionaryAsync(es => es.SubjectId);

        var newSubjectResults = new List<StudentSubjectResult>();
        foreach (var markEntry in markEntries)
        {
            examSubjects.TryGetValue(markEntry.SubjectId, out var examSubject);
            newSubjectResults.Add(CalculateSubjectResultInternal(markEntry, gradingRules, examSubject));
        }

        await _db.StudentSubjectResults.AddRangeAsync(newSubjectResults);
        await _db.SaveChangesAsync();

        // Recalculate exam result
        var existingExamResult = await _db.StudentExamResults
            .FirstOrDefaultAsync(r => r.ExamId == examId && r.StudentId == studentId);
        
        if (existingExamResult != null)
            _db.StudentExamResults.Remove(existingExamResult);

        var newExamResult = await CalculateStudentExamResultInternalAsync(examId, studentId, newSubjectResults);
        if (newExamResult != null)
        {
            await _db.StudentExamResults.AddAsync(newExamResult);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<decimal> AggregateComponentMarksAsync(MarkEntry markEntry)
    {
        // Sum all component marks for total
        decimal total = 0;

        total += markEntry.WrittenMarks ?? 0;
        total += markEntry.MCQMarks ?? 0;
        total += markEntry.CQMarks ?? 0;
        total += markEntry.PracticalMarks ?? 0;
        total += markEntry.VivaMarks ?? 0;
        total += markEntry.LabMarks ?? 0;
        total += markEntry.OralMarks ?? 0;
        total += markEntry.AssignmentMarks ?? 0;
        total += markEntry.ContinuousAssessmentMarks ?? 0;
        total += markEntry.CompetencyMarks ?? 0;
        total += markEntry.BehaviourMarks ?? 0;
        total += markEntry.ParticipationMarks ?? 0;

        return total;
    }

    public async Task<bool> CanCalculateResultsAsync(int examId)
    {
        var exam = await _db.Exams.FindAsync(examId);
        if (exam == null) return false;

        // Check if exam is locked or published
        var publication = await _db.ResultPublications
            .FirstOrDefaultAsync(p => p.ExamId == examId);

        return exam.Status != ResultWorkflowStatus.Published &&
               (publication == null || !publication.IsLocked);
    }

    public async Task<IDictionary<int, (int Passed, int Failed)>> GetSubjectPassFailStatsAsync(int examId)
    {
        var stats = await _db.StudentSubjectResults
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

    private StudentSubjectResult CalculateSubjectResultInternal(MarkEntry markEntry, IEnumerable<GradingRule> gradingRules, ExamSubject? examSubject)
    {
        // Aggregate component marks (synchronous version of summation)
        decimal totalMarks = AggregateComponentMarksInternal(markEntry);

        var (grade, gradePoint) = CalculateGrade(totalMarks, gradingRules);
        var resolvedGradePoint = gradePoint ?? 0;

        bool isPassed = totalMarks >= (examSubject?.PassMarks ?? 33);

        return new StudentSubjectResult
        {
            ExamId = markEntry.ExamId,
            StudentId = markEntry.StudentId,
            SubjectId = markEntry.SubjectId,
            MarksObtained = totalMarks,
            FullMarks = examSubject?.FullMarks ?? 100,
            PassMarks = examSubject?.PassMarks ?? 33,
            Grade = grade,
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
        decimal gpa = await CalculateGpaAsync(subjectResults);

        var (isPassed, failedCount) = DeterminePassFailStatus(subjectResults);

        return new StudentExamResult
        {
            ExamId = examId,
            StudentId = studentId,
            TotalMarks = totalMarks,
            TotalFullMarks = totalFullMarks,
            Gpa = gpa,
            Grade = GetOverallGrade(gpa),
            IsPassed = isPassed,
            FailedSubjectCount = failedCount,
            PassedSubjectCount = subjectResults.Count(r => r.IsPassed),
            Status = ResultWorkflowStatus.Draft,
            CalculatedAt = DateTime.Now
        };
    }

    private decimal AggregateComponentMarksInternal(MarkEntry markEntry)
    {
        decimal total = 0;
        total += markEntry.WrittenMarks ?? 0;
        total += markEntry.MCQMarks ?? 0;
        total += markEntry.CQMarks ?? 0;
        total += markEntry.PracticalMarks ?? 0;
        total += markEntry.VivaMarks ?? 0;
        total += markEntry.LabMarks ?? 0;
        total += markEntry.OralMarks ?? 0;
        total += markEntry.AssignmentMarks ?? 0;
        total += markEntry.ContinuousAssessmentMarks ?? 0;
        total += markEntry.CompetencyMarks ?? 0;
        total += markEntry.BehaviourMarks ?? 0;
        total += markEntry.ParticipationMarks ?? 0;
        return total;
    }

    public async Task CalculateSubjectResultAsync(MarkEntry markEntry)
    {
        var gradingRules = await _db.GradingRules.ToListAsync();
        var examSubject = await _db.ExamSubjects
            .FirstOrDefaultAsync(es => es.ExamId == markEntry.ExamId && es.SubjectId == markEntry.SubjectId);

        var result = CalculateSubjectResultInternal(markEntry, gradingRules, examSubject);

        await _db.StudentSubjectResults.AddAsync(result);
        await _db.SaveChangesAsync();
    }

    public async Task CalculateStudentExamResultAsync(int examId, int studentId)
    {
        var subjectResults = await _db.StudentSubjectResults
            .Include(r => r.Subject)
            .Where(r => r.ExamId == examId && r.StudentId == studentId)
            .ToListAsync();

        var result = await CalculateStudentExamResultInternalAsync(examId, studentId, subjectResults);
        if (result != null)
        {
            await _db.StudentExamResults.AddAsync(result);
            await _db.SaveChangesAsync();
        }
    }

    private (string? Grade, decimal? GradePoint) CalculateGrade(decimal marks, IEnumerable<GradingRule> gradingRules)
    {
        var rule = gradingRules
            .Where(r => marks >= r.MinMarks && marks <= r.MaxMarks)
            .OrderByDescending(r => r.GradePoint)
            .FirstOrDefault();

        return rule != null ? (rule.Grade, rule.GradePoint) : (null, null);
    }

    private string GetOverallGrade(decimal gpa)
    {
        if (gpa >= 5.00m) return "A+";
        if (gpa >= 4.00m) return "A";
        if (gpa >= 3.50m) return "A-";
        if (gpa >= 3.00m) return "B";
        if (gpa >= 2.00m) return "C";
        if (gpa >= 1.00m) return "D";
        return "F";
    }
}