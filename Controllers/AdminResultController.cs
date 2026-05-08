using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Services.Interfaces.Result;
using System.Security.Claims;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Helpers.Common;
using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Controllers;

[Authorize]
public class AdminResultController : Controller
{
    private readonly SchoolDbContext _db;
    private readonly IResultCalculationService _resultCalculationService;
    private readonly IMeritCalculationService _meritCalculationService;
    private readonly IPromotionService _promotionService;

    public AdminResultController(
        SchoolDbContext db,
        IResultCalculationService resultCalculationService,
        IMeritCalculationService meritCalculationService,
        IPromotionService promotionService)
    {
        _db = db;
        _resultCalculationService = resultCalculationService;
        _meritCalculationService = meritCalculationService;
        _promotionService = promotionService;
    }

    /// <summary>
    /// Admin dashboard showing all results overview
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> Dashboard()
    {
        var activeYear = await _db.AcademicYears.FirstOrDefaultAsync(x => x.IsActive);
        if (activeYear == null)
        {
            ViewBag.Message = "No active academic year found.";
            return View();
        }

        // Get all exams for the active year
        var exams = await _db.Exams
            .Include(e => e.ExamSubjects)
            .Where(e => e.AcademicYearId == activeYear.Id && !e.IsDeleted)
            .OrderBy(e => e.StartsOn)
            .ToListAsync();

        // Get result statistics
        var resultStats = await GetResultStatisticsAsync(activeYear.Id);

        ViewBag.ActiveYear = activeYear;
        ViewBag.Exams = exams;
        ViewBag.ResultStats = resultStats;

        return View();
    }

    /// <summary>
    /// View all subjects with their configurations
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> AllSubjects()
    {
        var subjects = await _db.Subjects
            .Include(s => s.ClassSubjects)
            .ThenInclude(cs => cs.SchoolClass)
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync();

        // Group subjects by group for better display
        var groupedSubjects = subjects
            .GroupBy(s => s.SubjectGroup)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());

        ViewBag.GroupedSubjects = groupedSubjects;
        return View(subjects);
    }

    /// <summary>
    /// View all results across all exams
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> AllResults(int? examId, int? classId, string? status)
    {
        var query = _db.StudentExamResults
            .Include(r => r.Student)
            .ThenInclude(s => s.Class)
            .Include(r => r.Exam)
            .Where(r => !r.IsDeleted);

        // Apply filters
        if (examId.HasValue)
            query = query.Where(r => r.ExamId == examId.Value);

        if (classId.HasValue)
            query = query.Where(r => r.Student.ClassId == classId.Value);

        if (!string.IsNullOrEmpty(status))
        {
            var statusFilter = status switch
            {
                "published" => ResultWorkflowStatus.Published,
                "approved" => ResultWorkflowStatus.Approved,
                "draft" => ResultWorkflowStatus.Draft,
                _ => (ResultWorkflowStatus?)null
            };

            if (statusFilter.HasValue)
                query = query.Where(r => r.Status == statusFilter.Value);
        }

        var results = await query
            .OrderByDescending(r => r.CreatedAt)
            .Take(1000) // Limit for performance
            .ToListAsync();

        // Get filter options
        var exams = await _db.Exams
            .Where(e => !e.IsDeleted)
            .Select(e => new { e.Id, e.Name })
            .ToListAsync();

        var classes = await _db.Classes
            .Where(c => !c.IsDeleted)
            .Select(c => new { c.Id, c.Name })
            .ToListAsync();

        ViewBag.Exams = exams;
        ViewBag.Classes = classes;
        ViewBag.SelectedExamId = examId;
        ViewBag.SelectedClassId = classId;
        ViewBag.SelectedStatus = status;

        return View(results);
    }

    /// <summary>
    /// Comprehensive tabulation sheet view
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> TabulationSheet(int examId, int? classId, int? sectionId)
    {
        var exam = await _db.Exams
            .Include(e => e.ExamSubjects)
            .ThenInclude(es => es.Subject)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null) return NotFound("Exam not found");

        // Get students based on class/section filter
        var studentQuery = _db.Students
            .Include(s => s.Class)
            .Include(s => s.Section)
            .Where(s => !s.IsDeleted);

        if (classId.HasValue)
            studentQuery = studentQuery.Where(s => s.ClassId == classId.Value);

        if (sectionId.HasValue)
            studentQuery = studentQuery.Where(s => s.SectionId == sectionId.Value);

        var students = await studentQuery
            .OrderBy(s => s.RollNumber)
            .ToListAsync();

        // Get all subject results for all students in this filter at once
        var allSubjectResults = await _db.StudentSubjectResults
            .Include(r => r.Subject)
            .Where(r => r.ExamId == examId && students.Select(s => s.Id).Contains(r.StudentId))
            .ToListAsync();

        var subjectResultsByStudent = allSubjectResults.GroupBy(r => r.StudentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Get all exam results for all students in this filter at once
        var allExamResults = await _db.StudentExamResults
            .Where(r => r.ExamId == examId && students.Select(s => s.Id).Contains(r.StudentId))
            .ToDictionaryAsync(r => r.StudentId);

        // Get subject results for each student
        var tabulationData = new List<TabulationStudentDto>();

        foreach (var student in students)
        {
            subjectResultsByStudent.TryGetValue(student.Id, out var subjectResults);
            subjectResults ??= new List<StudentSubjectResult>();

            var studentData = new TabulationStudentDto
            {
                StudentId = student.Id,
                StudentName = student.FullName,
                RollNumber = student.RollNumber,
                SubjectMarks = subjectResults.ToDictionary(
                    r => r.SubjectId,
                    r => new SubjectMarkDto
                    {
                        MarksObtained = r.MarksObtained,
                        Grade = r.Grade,
                        GradePoint = r.GradePoint,
                        IsPassed = r.IsPassed
                    }
                )
            };

            // Calculate total marks and GPA
            studentData.TotalMarks = subjectResults.Sum(r => r.MarksObtained);
            
            if (allExamResults.TryGetValue(student.Id, out var examResult))
            {
                studentData.GPA = examResult.Gpa;
                studentData.Grade = examResult.Grade;
                studentData.Position = examResult.Position;
                studentData.IsPassed = examResult.IsPassed;
            }

            tabulationData.Add(studentData);
        }

        // Get subject information
        var subjects = exam.ExamSubjects
            .Select(es => new TabulationSubjectDto
            {
                SubjectId = es.SubjectId,
                SubjectName = es.Subject.IsReligionSubject && !string.IsNullOrEmpty(es.Subject.ReligionType)
                    ? ReligionHelper.GetReligionSubjectName(es.Subject.ReligionType)
                    : es.Subject.Name,
                FullMarks = es.FullMarks,
                PassMarks = es.PassMarks
            })
            .ToList();

        // Calculate subject-wise statistics
        foreach (var subject in subjects)
        {
            var subjectMarks = tabulationData
                .Where(s => s.SubjectMarks.ContainsKey(subject.SubjectId))
                .Select(s => s.SubjectMarks[subject.SubjectId])
                .ToList();

            if (subjectMarks.Any())
            {
                subject.AverageMarks = subjectMarks.Average(m => m.MarksObtained);
                subject.HighestMarks = subjectMarks.Max(m => m.MarksObtained);
                subject.PassedCount = subjectMarks.Count(m => m.IsPassed);
                subject.FailedCount = subjectMarks.Count(m => !m.IsPassed);
                subject.PassPercentage = subjectMarks.Any() ?
                    (decimal)subject.PassedCount / subjectMarks.Count * 100 : 0;
            }
        }

        // Calculate summary statistics
        var summary = new TabulationSummaryDto
        {
            TotalStudents = tabulationData.Count,
            PassedStudents = tabulationData.Count(s => s.IsPassed),
            FailedStudents = tabulationData.Count(s => !s.IsPassed),
            ClassAverageGPA = tabulationData.Any() ? tabulationData.Average(s => s.GPA) : 0,
            HighestGPA = tabulationData.Any() ? tabulationData.Max(s => s.GPA) : 0,
            LowestGPA = tabulationData.Any() ? tabulationData.Min(s => s.GPA) : 0
        };

        summary.PassPercentage = summary.TotalStudents > 0 ?
            (decimal)summary.PassedStudents / summary.TotalStudents * 100 : 0;

        var tabulationSheet = new TabulationSheetDto
        {
            ExamId = examId,
            ExamName = exam.Name,
            ClassId = classId ?? 0,
            ClassName = classId.HasValue ?
                (await _db.Classes.FindAsync(classId.Value))?.Name ?? "All Classes" : "All Classes",
            SectionName = sectionId.HasValue ?
                (await _db.Sections.FindAsync(sectionId.Value))?.Name ?? "All Sections" : "All Sections",
            Students = tabulationData,
            Subjects = subjects,
            Summary = summary
        };

        // Get filter options
        var classes = await _db.Classes
            .Where(c => !c.IsDeleted)
            .Select(c => new { c.Id, c.Name })
            .ToListAsync();

        var sections = classId.HasValue ?
            (await _db.Sections
                .Where(s => s.SchoolClassId == classId.Value && !s.IsDeleted)
                .Select(s => new { s.Id, s.Name })
                .ToListAsync()).Cast<dynamic>().ToList()
            : new List<dynamic>();

        ViewBag.Exam = exam;
        ViewBag.Classes = classes;
        ViewBag.Sections = sections;
        ViewBag.SelectedClassId = classId;
        ViewBag.SelectedSectionId = sectionId;

        return View(tabulationSheet);
    }

    /// <summary>
    /// Merit lists for different categories
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> MeritLists(int examId)
    {
        var exam = await _db.Exams.FindAsync(examId);
        if (exam == null) return NotFound();

        // Class merit list
        var classMerit = await _meritCalculationService.GetMeritListAsync(examId, MeritCategory.Class);

        // Section merit list
        var sectionMerit = await _meritCalculationService.GetMeritListAsync(examId, MeritCategory.Section);

        // Group merit list (for classes 9-10)
        var groupMerit = await _meritCalculationService.GetMeritListAsync(examId, MeritCategory.Group);

        // School merit list
        var schoolMerit = await _meritCalculationService.GetMeritListAsync(examId, MeritCategory.School);

        ViewBag.Exam = exam;
        ViewBag.ClassMerit = classMerit.Take(50).ToList(); // Top 50
        ViewBag.SectionMerit = sectionMerit.Take(50).ToList();
        ViewBag.GroupMerit = groupMerit.Take(50).ToList();
        ViewBag.SchoolMerit = schoolMerit.Take(50).ToList();

        return View();
    }

    /// <summary>
    /// Subject-wise performance analysis
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> SubjectAnalysis(int examId)
    {
        var exam = await _db.Exams
            .Include(e => e.ExamSubjects)
            .ThenInclude(es => es.Subject)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null) return NotFound();

        var subjectAnalysis = new List<SubjectPerformanceDto>();

        // Fetch all subject results for the exam at once
        var allSubjectResults = await _db.StudentSubjectResults
            .Where(r => r.ExamId == examId)
            .ToListAsync();

        var resultsBySubject = allSubjectResults.GroupBy(r => r.SubjectId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var examSubject in exam.ExamSubjects)
        {
            if (!resultsBySubject.TryGetValue(examSubject.SubjectId, out var subjectResults))
                continue;

            var analysis = new SubjectPerformanceDto
            {
                SubjectId = examSubject.SubjectId,
                SubjectName = examSubject.Subject.IsReligionSubject && !string.IsNullOrEmpty(examSubject.Subject.ReligionType)
                    ? ReligionHelper.GetReligionSubjectName(examSubject.Subject.ReligionType)
                    : examSubject.Subject.Name,
                AverageMarks = subjectResults.Average(r => r.MarksObtained),
                HighestMarks = subjectResults.Max(r => r.MarksObtained),
                LowestMarks = subjectResults.Min(r => r.MarksObtained),
                PassPercentage = subjectResults.Any() ?
                    (decimal)subjectResults.Count(r => r.IsPassed) / subjectResults.Count * 100 : 0,
                PassedCount = subjectResults.Count(r => r.IsPassed),
                FailedCount = subjectResults.Count(r => !r.IsPassed)
            };

            subjectAnalysis.Add(analysis);
        }

        ViewBag.Exam = exam;
        return View(subjectAnalysis);
    }

    /// <summary>
    /// Result publishing and locking management
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> ResultPublishing()
    {
        var activeYear = await _db.AcademicYears.FirstOrDefaultAsync(x => x.IsActive);
        if (activeYear == null)
        {
            ViewBag.Message = "No active academic year found.";
            return View(new List<dynamic>());
        }

        var resultPublications = await _db.ResultPublications
            .Include(rp => rp.Exam)
            .Where(rp => rp.Exam.AcademicYearId == activeYear.Id && !rp.IsDeleted)
            .OrderByDescending(rp => rp.CreatedAt)
            .ToListAsync();

        ViewBag.ActiveYear = activeYear;
        return View(resultPublications);
    }

    /// <summary>
    /// Bulk result operations
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    public async Task<IActionResult> RecalculateResults(int examId)
    {
        try
        {
            await _resultCalculationService.RecalculateResultsAsync(examId, 0); // Recalculate all students
            TempData["Success"] = "Results recalculated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error recalculating results: {ex.Message}";
        }

        return RedirectToAction("Dashboard");
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    public async Task<IActionResult> RecalculateMeritPositions(int examId)
    {
        try
        {
            await _meritCalculationService.RecalculateMeritPositionsAsync(examId);
            TempData["Success"] = "Merit positions recalculated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error recalculating merit positions: {ex.Message}";
        }

        return RedirectToAction("Dashboard");
    }

    private async Task<ResultStatisticsDto> GetResultStatisticsAsync(int academicYearId)
    {
        var stats = new ResultStatisticsDto();

        // Count exams
        stats.TotalExams = await _db.Exams.CountAsync(e => e.AcademicYearId == academicYearId && !e.IsDeleted);

        // Count published results
        stats.PublishedResults = await _db.ResultPublications
            .CountAsync(rp => rp.Exam.AcademicYearId == academicYearId && rp.Status == ResultWorkflowStatus.Published);

        // Count total results
        stats.TotalResults = await _db.StudentExamResults
            .CountAsync(r => r.Exam.AcademicYearId == academicYearId);

        // Calculate average GPA
        var gpaStats = await _db.StudentExamResults
            .Where(r => r.Exam.AcademicYearId == academicYearId)
            .GroupBy(r => 1)
            .Select(g => new
            {
                AverageGPA = g.Average(r => r.Gpa),
                HighestGPA = g.Max(r => r.Gpa),
                PassPercentage = g.Count(r => r.IsPassed) * 100.0 / g.Count()
            })
            .FirstOrDefaultAsync();

        if (gpaStats != null)
        {
            stats.AverageGPA = gpaStats.AverageGPA;
            stats.HighestGPA = gpaStats.HighestGPA;
            stats.PassPercentage = (decimal)gpaStats.PassPercentage;
        }

        return stats;
    }

    private async Task<int?> GetCurrentUserIdAsync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdStr, out var userId) ? userId : null;
    }
}

public class ResultStatisticsDto
{
    public int TotalExams { get; set; }
    public int PublishedResults { get; set; }
    public int TotalResults { get; set; }
    public decimal AverageGPA { get; set; }
    public decimal HighestGPA { get; set; }
    public decimal PassPercentage { get; set; }
}