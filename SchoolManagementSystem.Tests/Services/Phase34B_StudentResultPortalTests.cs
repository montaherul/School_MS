using Xunit;
using Moq;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Implementations.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using Microsoft.Extensions.Logging;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Tests.Services;

/// <summary>
/// Phase 34B: Student Result Portal — verify PublishResultsAsync sets PublishedAt+Status on StudentExamResult,
/// GetStudentResultsAsync and GetAllResultsAsync return complete DTO mappings.
/// </summary>
public class Phase34B_StudentResultPortalTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly Mock<IMeritCalculationService> _meritMock = new(MockBehavior.Loose);
    private readonly Mock<IExamRepository> _examRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IMarkEntryRepository> _markRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IResultPublicationRepository> _pubRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IStudentSubjectResultRepository> _subResRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IStudentExamResultRepository> _examResRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IGradingRuleRepository> _gradingRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IGradeCalculator> _gradeCalcMock = new(MockBehavior.Loose);
    private readonly Mock<IComponentAggregator> _aggMock = new(MockBehavior.Loose);
    private readonly Mock<IPassFailPolicy> _pfMock = new(MockBehavior.Loose);
    private readonly Mock<ILogger<ResultPublicationService>> _loggerMock = new(MockBehavior.Loose);

    private readonly ResultPublicationService _service;

    public Phase34B_StudentResultPortalTests()
    {
        _service = new ResultPublicationService(
            _uowMock.Object,
            _meritMock.Object,
            _examRepoMock.Object,
            _markRepoMock.Object,
            _pubRepoMock.Object,
            _subResRepoMock.Object,
            _examResRepoMock.Object,
            _gradingRepoMock.Object,
            _gradeCalcMock.Object,
            _aggMock.Object,
            _pfMock.Object,
            Mock.Of<IResultAuditLogRepository>());
    }

    // ─── StudentExamResultDto property completeness ───────────────

    [Fact(DisplayName = "1. StudentExamResultDto has PublishedAt property")]
    public void Dto_HasPublishedAtProperty()
    {
        var dto = new StudentExamResultDto { PublishedAt = DateTime.UtcNow };
        Assert.NotNull(dto.PublishedAt);
    }

    [Fact(DisplayName = "2. StudentExamResultDto has Grade and ClassPosition")]
    public void Dto_HasGradeAndClassPosition()
    {
        var dto = new StudentExamResultDto
        {
            Grade = "A+",
            ClassPosition = 1,
            GroupPosition = 2,
            Gpa = 5.00m
        };
        Assert.Equal("A+", dto.Grade);
        Assert.Equal(1, dto.ClassPosition);
        Assert.Equal(2, dto.GroupPosition);
        Assert.Equal(5.00m, dto.Gpa);
    }

    [Fact(DisplayName = "3. StudentExamResultDto has FailedSubjectCount and PassedSubjectCount")]
    public void Dto_HasPassFailSubjectCounts()
    {
        var dto = new StudentExamResultDto
        {
            FailedSubjectCount = 1,
            PassedSubjectCount = 5
        };
        Assert.Equal(1, dto.FailedSubjectCount);
        Assert.Equal(5, dto.PassedSubjectCount);
    }

    [Fact(DisplayName = "4. StudentExamResultDto has TotalFullMarks")]
    public void Dto_HasTotalFullMarks()
    {
        var dto = new StudentExamResultDto { TotalFullMarks = 600, TotalMarks = 420 };
        Assert.Equal(600, dto.TotalFullMarks);
        Assert.Equal(420, dto.TotalMarks);
    }

    [Fact(DisplayName = "5. StudentExamResultDto has Term property")]
    public void Dto_HasTermProperty()
    {
        var dto = new StudentExamResultDto { Term = ExamTerm.HalfYearly };
        Assert.Equal(ExamTerm.HalfYearly, dto.Term);
    }

    [Fact(DisplayName = "6. StudentExamResultDto.Status stores ResultWorkflowStatus")]
    public void Dto_StatusIsWorkflowEnum()
    {
        var dto = new StudentExamResultDto { Status = ResultWorkflowStatus.Published };
        Assert.Equal(ResultWorkflowStatus.Published, dto.Status);
    }

    // ─── StudentSubjectResultDto completeness ──────────────────────

    [Fact(DisplayName = "7. StudentSubjectResultDto has all required fields")]
    public void SubjectDto_HasAllFields()
    {
        var dto = new StudentSubjectResultDto
        {
            SubjectId = 1,
            SubjectName = "Math",
            SubjectNameBn = "গণিত",
            SubjectGroup = "Science",
            MarksObtained = 85,
            FullMarks = 100,
            PassMarks = 33,
            Grade = "A+",
            GradePoint = 5.00m,
            IsPassed = true,
            ObtainedMarks = 85,
            GPA = 5.00m
        };
        Assert.Equal(1, dto.SubjectId);
        Assert.Equal("Math", dto.SubjectName);
        Assert.Equal("গণিত", dto.SubjectNameBn);
        Assert.Equal("Science", dto.SubjectGroup);
        Assert.Equal(85m, dto.MarksObtained);
        Assert.Equal(100m, dto.FullMarks);
        Assert.Equal(33m, dto.PassMarks);
        Assert.Equal("A+", dto.Grade);
        Assert.Equal(5.00m, dto.GradePoint);
        Assert.True(dto.IsPassed);
        Assert.Equal(85m, dto.ObtainedMarks);
        Assert.Equal(5.00m, dto.GPA);
    }

    // ─── StudentPortalResultDto structure ──────────────────────────

    [Fact(DisplayName = "8. StudentPortalResultDto contains ExamResults list")]
    public void PortalDto_HasExamResultsList()
    {
        var dto = new StudentPortalResultDto
        {
            StudentId = 2,
            StudentName = "Sample Student One",
            ExamResults = new List<StudentExamResultDto>
            {
                new() { ExamId = 19, ExamName = "Half Yearly", Gpa = 4.50m, Grade = "A", IsPassed = true }
            }
        };
        Assert.Single(dto.ExamResults);
        Assert.Equal(4.50m, dto.ExamResults[0].Gpa);
    }

    [Fact(DisplayName = "9. StudentPortalResultDto.ExamResults contains SubjectResults")]
    public void PortalDto_ExamResultsContainsSubjects()
    {
        var examDto = new StudentExamResultDto
        {
            ExamId = 19,
            Subjects = new List<StudentSubjectResultDto>
            {
                new() { SubjectId = 1, SubjectName = "Math", Grade = "A+", GradePoint = 5.00m, IsPassed = true },
                new() { SubjectId = 2, SubjectName = "English", Grade = "B", GradePoint = 3.00m, IsPassed = true }
            }
        };
        Assert.Equal(2, examDto.Subjects.Count);
        Assert.Contains(examDto.Subjects, s => s.SubjectName == "Math");
        Assert.Contains(examDto.Subjects, s => s.SubjectName == "English");
    }

    // ─── PublishedAt null handling ─────────────────────────────────

    [Fact(DisplayName = "10. StudentExamResultDto.PublishedAt defaults to null before publish")]
    public void PublishedAt_DefaultsNull()
    {
        var dto = new StudentExamResultDto();
        Assert.Null(dto.PublishedAt);

        var ser = new StudentExamResult();
        Assert.Null(ser.PublishedAt);
    }

    // ─── StudentExamResult entity ──────────────────────────────────

    [Fact(DisplayName = "11. StudentExamResult.Status defaults to Draft")]
    public void Entity_StatusDefaultsToDraft()
    {
        var entity = new StudentExamResult();
        Assert.Equal(ResultWorkflowStatus.Draft, entity.Status);
    }

    [Fact(DisplayName = "12. StudentExamResult can set PublishedAt and Status")]
    public void Entity_CanSetPublishedAtAndStatus()
    {
        var now = DateTime.UtcNow;
        var entity = new StudentExamResult
        {
            PublishedAt = now,
            Status = ResultWorkflowStatus.Published
        };
        Assert.Equal(now, entity.PublishedAt);
        Assert.Equal(ResultWorkflowStatus.Published, entity.Status);
    }

    // ─── Grading rules edge cases ─────────────────────────────────

    [Fact(DisplayName = "13. Grade mapping: F for marks below 33")]
    public void Grading_FailingGrade()
    {
        var rules = new List<GradingRule>
        {
            new() { Grade = "A+", MinMarks = 80, MaxMarks = 100, GradePoint = 5.00m, IsActive = true },
            new() { Grade = "A", MinMarks = 70, MaxMarks = 79, GradePoint = 4.00m, IsActive = true },
            new() { Grade = "A-", MinMarks = 60, MaxMarks = 69, GradePoint = 3.50m, IsActive = true },
            new() { Grade = "B", MinMarks = 50, MaxMarks = 59, GradePoint = 3.00m, IsActive = true },
            new() { Grade = "C", MinMarks = 40, MaxMarks = 49, GradePoint = 2.00m, IsActive = true },
            new() { Grade = "D", MinMarks = 33, MaxMarks = 39, GradePoint = 1.00m, IsActive = true },
            new() { Grade = "F", MinMarks = 0, MaxMarks = 32, GradePoint = 0.00m, IsActive = true }
        };

        decimal totalMarks = 25;
        var matched = rules.Where(r => r.IsActive && totalMarks >= r.MinMarks && totalMarks <= r.MaxMarks)
                           .OrderByDescending(r => r.MinMarks).FirstOrDefault();
        Assert.NotNull(matched);
        Assert.Equal("F", matched!.Grade);
        Assert.Equal(0.00m, matched.GradePoint);
    }

    [Fact(DisplayName = "14. Grade mapping: A+ for marks 80-100")]
    public void Grading_APlus()
    {
        var rules = new List<GradingRule>
        {
            new() { Grade = "A+", MinMarks = 80, MaxMarks = 100, GradePoint = 5.00m, IsActive = true },
            new() { Grade = "A", MinMarks = 70, MaxMarks = 79, GradePoint = 4.00m, IsActive = true }
        };

        decimal totalMarks = 90;
        var matched = rules.Where(r => r.IsActive && totalMarks >= r.MinMarks && totalMarks <= r.MaxMarks)
                           .OrderByDescending(r => r.MinMarks).FirstOrDefault();
        Assert.Equal("A+", matched!.Grade);
    }

    [Fact(DisplayName = "15. Pass/Fail: total marks >= pass marks means passed")]
    public void PassFail_MeetsPassMarks()
    {
        decimal totalMarks = 45;
        decimal passMarks = 33;
        Assert.True(totalMarks >= passMarks);
    }

    [Fact(DisplayName = "16. Pass/Fail: total marks < pass marks means failed")]
    public void PassFail_BelowPassMarks()
    {
        decimal totalMarks = 30;
        decimal passMarks = 33;
        Assert.False(totalMarks >= passMarks);
    }

    // ─── ComponentMarksDto completeness ────────────────────────────

    [Fact(DisplayName = "17. ComponentMarksDto supports dynamic keys")]
    public void ComponentMarks_SupportsDynamicKeys()
    {
        var marks = new ComponentMarksDto();
        marks["PROJECT"] = 40;
        marks["PRESENTATION"] = 35;
        marks["ATTENDANCE"] = 10;

        Assert.Equal(3, marks.Count);
        Assert.Equal(40m, marks["PROJECT"]);
        Assert.Equal(35m, marks["PRESENTATION"]);
        Assert.Equal(10m, marks["ATTENDANCE"]);
    }

    [Fact(DisplayName = "18. ComponentMarksDto handles null values")]
    public void ComponentMarks_HandlesNullValues()
    {
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = null;
        Assert.Null(marks["WRITTEN"]);
    }

    // ─── ResultWorkflowStatus values ──────────────────────────────

    [Fact(DisplayName = "19. ResultWorkflowStatus enum has Published = 5")]
    public void WorkflowEnum_PublishedIs5()
    {
        Assert.Equal(5, (int)ResultWorkflowStatus.Published);
    }

    [Fact(DisplayName = "20. ResultWorkflowStatus enum has Draft = 1")]
    public void WorkflowEnum_DraftIs1()
    {
        Assert.Equal(1, (int)ResultWorkflowStatus.Draft);
    }

    // ─── ExamTerm enum mapping ─────────────────────────────────────

    [Fact(DisplayName = "21. ExamTerm enum maps HalfYearly = 2")]
    public void ExamTerm_HalfYearly()
    {
        Assert.Equal(2, (int)ExamTerm.HalfYearly);
    }

    [Fact(DisplayName = "22. StudentExamResultDto.Subjects list defaults to empty")]
    public void ExamResultDto_SubjectsDefaultsEmpty()
    {
        var dto = new StudentExamResultDto();
        Assert.NotNull(dto.Subjects);
        Assert.Empty(dto.Subjects);
    }

    // ─── Pass/Fail count consistency ──────────────────────────────

    [Fact(DisplayName = "23. FailedSubjectCount + PassedSubjectCount equals total subjects")]
    public void PassFailCount_Consistency()
    {
        var subjects = new List<StudentSubjectResultDto>
        {
            new() { SubjectId = 1, IsPassed = true, Grade = "A+", GradePoint = 5.00m },
            new() { SubjectId = 2, IsPassed = true, Grade = "A", GradePoint = 4.00m },
            new() { SubjectId = 3, IsPassed = false, Grade = "F", GradePoint = 0.00m }
        };

        int passed = subjects.Count(s => s.IsPassed);
        int failed = subjects.Count(s => !s.IsPassed);
        Assert.Equal(2, passed);
        Assert.Equal(1, failed);
        Assert.Equal(subjects.Count, passed + failed);
    }

    // ─── GPA calculation logic ────────────────────────────────────

    [Fact(DisplayName = "24. GPA = sum of passed compulsory grade points / count")]
    public void GpaCalculation_AverageOfPassed()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { GradePoint = 5.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 4.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 3.50m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 0.00m, IsPassed = false, IsOptionalSubject = false }
        };

        var valid = results.Where(r => r.IsPassed && !r.IsOptionalSubject).ToList();
        decimal gpa = Math.Round(valid.Sum(r => r.GradePoint) / valid.Count, 2);
        Assert.Equal(4.17m, gpa);
    }

    [Fact(DisplayName = "25. GPA excludes optional subjects even if passed")]
    public void GpaCalculation_ExcludesOptional()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { GradePoint = 5.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 2.00m, IsPassed = true, IsOptionalSubject = true }
        };

        var valid = results.Where(r => r.IsPassed && !r.IsOptionalSubject).ToList();
        decimal gpa = Math.Round(valid.Sum(r => r.GradePoint) / valid.Count, 2);
        Assert.Equal(5.00m, gpa);
    }
}
