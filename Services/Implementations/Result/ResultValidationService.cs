using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ResultValidationService : IResultValidationService
{
    private readonly IUnitOfWork _uow;
    private readonly IExamService _examService;
    private readonly IComponentAggregator _componentAggregator;
    private readonly IGradeCalculator _gradeCalculator;
    private readonly ILogger<ResultValidationService> _logger;

    public ResultValidationService(
        IUnitOfWork uow,
        IExamService examService,
        IComponentAggregator componentAggregator,
        IGradeCalculator gradeCalculator,
        ILogger<ResultValidationService> logger)
    {
        _uow = uow;
        _examService = examService;
        _componentAggregator = componentAggregator;
        _gradeCalculator = gradeCalculator;
        _logger = logger;
    }

    public async Task<ResultValidationResultDto> ValidateAsync(ResultValidationRequest request, CancellationToken ct = default)
    {
        var result = new ResultValidationResultDto();
        var issues = new List<ResultValidationIssueDto>();

        var exam = await _uow.Repository<ExamEntity>().QueryNoTracking()
            .Include(e => e.Class)
            .FirstOrDefaultAsync(e => e.Id == request.ExamId, ct);

        if (exam == null)
        {
            result.IsValid = false;
            result.ExamName = "Unknown";
            return result;
        }

        result.ExamName = exam.Name;

        var examSubjects = await _uow.Repository<ExamSubject>().QueryNoTracking()
            .Include(es => es.Subject)
            .Where(es => es.ExamId == request.ExamId && es.IsActive)
            .ToListAsync(ct);

        var subjectIds = examSubjects.Select(es => es.SubjectId).ToHashSet();
        result.TotalSubjects = subjectIds.Count;

        var classIds = request.ClassIds ?? [exam.ClassId];

        var students = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().QueryNoTracking()
            .Where(s => classIds.Contains(s.ClassId) && s.Status == StudentStatus.Active)
            .ToListAsync(ct);

        result.TotalStudents = students.Count;
        var studentMap = students.ToDictionary(s => s.Id, s => s.FullName);

        var allMarkEntries = await _uow.Repository<MarkEntry>().QueryNoTracking()
            .Where(me => me.ExamId == request.ExamId)
            .ToListAsync(ct);

        var entryGrouped = allMarkEntries
            .GroupBy(me => new { me.StudentId, me.SubjectId })
            .ToDictionary(g => g.Key, g => g.ToList());

        var subjectResults = await _uow.Repository<StudentSubjectResult>().QueryNoTracking()
            .Where(ssr => ssr.ExamId == request.ExamId)
            .ToListAsync(ct);

        var gradingRules = await _uow.Repository<GradingRule>().QueryNoTracking()
            .Where(gr => gr.IsActive)
            .ToListAsync(ct);

        var subjectResultLookup = subjectResults
            .GroupBy(ssr => ssr.SubjectId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var studentSubjectResultsByStudent = subjectResults
            .GroupBy(ssr => ssr.StudentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var studentExamResults = await _uow.Repository<StudentExamResult>().QueryNoTracking()
            .Where(ser => ser.ExamId == request.ExamId)
            .ToListAsync(ct);

        var passedExamStudentIds = studentExamResults
            .Where(ser => ser.IsPassed)
            .Select(ser => ser.StudentId)
            .ToHashSet();

        result.PassedCount = passedExamStudentIds.Count;
        result.FailedCount = studentExamResults.Count(ser => !ser.IsPassed);

        var subjectsWithEntries = allMarkEntries
            .Select(me => me.SubjectId)
            .Distinct()
            .ToHashSet();

        var enrolledStudentIds = students.Select(s => s.Id).ToHashSet();

        // 1. Check Missing Marks
        if (request.CheckMissingMarks)
        {
            foreach (var examSubject in examSubjects)
            {
                foreach (var studentId in enrolledStudentIds)
                {
                    var key = new { StudentId = studentId, SubjectId = examSubject.SubjectId };
                    if (!entryGrouped.ContainsKey(key))
                    {
                        issues.Add(new ResultValidationIssueDto
                        {
                            Severity = "Error",
                            Category = "MissingMarks",
                            StudentId = studentId,
                            StudentName = studentMap.GetValueOrDefault(studentId, $"ID {studentId}"),
                            SubjectName = examSubject.Subject?.Name ?? $"Subject {examSubject.SubjectId}",
                            Message = $"No marks entered for {examSubject.Subject?.Name ?? $"Subject {examSubject.SubjectId}"}"
                        });
                    }
                }
            }
        }

        // 2. Check Duplicate Marks
        if (request.CheckDuplicateMarks)
        {
            foreach (var kvp in entryGrouped)
            {
                var key = kvp.Key;
                var entries = kvp.Value;
                if (entries.Count > 1)
                {
                    var examSubject = examSubjects.FirstOrDefault(es => es.SubjectId == key.SubjectId);
                    issues.Add(new ResultValidationIssueDto
                    {
                        Severity = "Error",
                        Category = "Duplicate",
                        StudentId = key.StudentId,
                        StudentName = studentMap.GetValueOrDefault(key.StudentId, $"ID {key.StudentId}"),
                        SubjectName = examSubject?.Subject?.Name ?? $"Subject {key.SubjectId}",
                        Message = $"Duplicate mark entries ({entries.Count} rows) for {examSubject?.Subject?.Name ?? $"Subject {key.SubjectId}"}",
                        Details = $"Entry IDs: {string.Join(", ", entries.Select(e => e.Id))}"
                    });
                }
            }
        }

        // 3. Check GPA Mismatch
        if (request.CheckGpaMismatch)
        {
            foreach (var kvp in entryGrouped)
            {
                var key = kvp.Key;
                var entries = kvp.Value;
                var primaryEntry = entries.First();
                var ssr = studentSubjectResultsByStudent
                    .GetValueOrDefault(key.StudentId)?
                    .FirstOrDefault(s => s.SubjectId == key.SubjectId);

                if (ssr == null) continue;

                var recalculatedTotal = _componentAggregator.AggregateAll(primaryEntry);
                var (recalculatedGrade, recalculatedGp) = _gradeCalculator.CalculateGrade(recalculatedTotal, gradingRules);

                var storedGradePoint = ssr.GradePoint;
                var storedGrade = ssr.Grade;

                if (recalculatedGp.HasValue && Math.Abs(recalculatedGp.Value - storedGradePoint) > 0.005m)
                {
                    var examSubject = examSubjects.FirstOrDefault(es => es.SubjectId == key.SubjectId);
                    issues.Add(new ResultValidationIssueDto
                    {
                        Severity = "Warning",
                        Category = "GpaMismatch",
                        StudentId = key.StudentId,
                        StudentName = studentMap.GetValueOrDefault(key.StudentId, $"ID {key.StudentId}"),
                        SubjectName = examSubject?.Subject?.Name ?? $"Subject {key.SubjectId}",
                        Message = $"GPA mismatch: stored {storedGradePoint:F2} ({storedGrade}) vs recalculated {recalculatedGp:F2} ({recalculatedGrade ?? "N/A"})",
                        Details = $"Marks: stored={ssr.MarksObtained}, recalculated={recalculatedTotal:F2}"
                    });
                }
            }
        }

        // 4. Check Incomplete Components
        if (request.CheckIncompleteComponents)
        {
            foreach (var entry in allMarkEntries)
            {
                var componentTotal = _componentAggregator.AggregateAll(entry);
                if (Math.Abs(componentTotal - entry.MarksObtained) > 0.005m && componentTotal > 0)
                {
                    var examSubject = examSubjects.FirstOrDefault(es => es.SubjectId == entry.SubjectId);
                    issues.Add(new ResultValidationIssueDto
                    {
                        Severity = "Warning",
                        Category = "IncompleteComponents",
                        StudentId = entry.StudentId,
                        StudentName = studentMap.GetValueOrDefault(entry.StudentId, $"ID {entry.StudentId}"),
                        SubjectName = examSubject?.Subject?.Name ?? $"Subject {entry.SubjectId}",
                        Message = $"Component marks ({componentTotal:F2}) do not match total ({entry.MarksObtained:F2})",
                        Details = $"Entry ID: {entry.Id}, SubjectId: {entry.SubjectId}"
                    });
                }
            }
        }

        // 5. Check Missing Subjects
        if (request.CheckMissingSubjects)
        {
            foreach (var examSubject in examSubjects)
            {
                if (!subjectsWithEntries.Contains(examSubject.SubjectId))
                {
                    issues.Add(new ResultValidationIssueDto
                    {
                        Severity = "Error",
                        Category = "MissingSubjects",
                        SubjectName = examSubject.Subject?.Name ?? $"Subject {examSubject.SubjectId}",
                        Message = $"No mark entries exist for {examSubject.Subject?.Name ?? $"Subject {examSubject.SubjectId}"} in this exam",
                        Details = $"SubjectId: {examSubject.SubjectId}, FullMarks: {examSubject.FullMarks}"
                    });
                }
            }
        }

        var totalStudentsPerClass = students
            .GroupBy(s => s.ClassId)
            .ToDictionary(g => g.Key, g => g.Count());

        var studentsWithAllMarks = allMarkEntries
            .GroupBy(me => me.StudentId)
            .Where(g => g.Select(x => x.SubjectId).Distinct().Count() >= subjectIds.Count)
            .Select(g => g.Key)
            .ToHashSet();

        result.IncompleteCount = enrolledStudentIds.Count(id => !studentsWithAllMarks.Contains(id));

        foreach (var classId in classIds)
        {
            var className = exam.Class?.Name ?? $"Class {classId}";
            var classStudentIds = students.Where(s => s.ClassId == classId).Select(s => s.Id).ToHashSet();
            var completed = classStudentIds.Count(id => studentsWithAllMarks.Contains(id));
            result.Summary.Add(new ResultValidationSummaryItemDto
            {
                ClassName = className,
                TotalStudents = totalStudentsPerClass.GetValueOrDefault(classId, 0),
                CompletedStudents = completed,
                IncompleteStudents = classStudentIds.Count - completed
            });
        }

        result.Issues = issues;
        result.TotalIssues = issues.Count;
        result.IsValid = issues.Count == 0;

        return result;
    }

    public async Task<ResultValidationResultDto> ValidatePrePublicationAsync(int examId, CancellationToken ct = default)
    {
        return await ValidateAsync(new ResultValidationRequest
        {
            ExamId = examId,
            CheckMissingMarks = true,
            CheckDuplicateMarks = true,
            CheckGpaMismatch = true,
            CheckIncompleteComponents = true,
            CheckMissingSubjects = true
        }, ct);
    }
}
