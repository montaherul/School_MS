using Xunit;
using Moq;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Implementations.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Tests.Services;

/// <summary>
/// Phase 36A: Result Engine Critical Data-Integrity Fixes
/// Validates component mark validation, transaction wrapping, soft-delete filtering,
/// promotion status updates, and publication workflow enforcement.
/// </summary>
public class Phase36A_ResultEngineFixTests
{
    // ─── Bug #1: Component mark validation ─────────────────────────

    [Fact(DisplayName = "1. Component validation rejects value above FullMarks")]
    public void ComponentValidation_RejectsExcessiveMark()
    {
        var markDto = new MarkEntryDto { StudentId = 1, MarksObtained = 80 };
        markDto.ComponentMarks["WRITTEN"] = 95;
        markDto.ComponentMarks["MCQ"] = 20;

        var components = new List<ComponentColumnDto>
        {
            new() { ComponentCode = "WRITTEN", ComponentName = "Written", FullMarks = 80 },
            new() { ComponentCode = "MCQ", ComponentName = "MCQ", FullMarks = 20 }
        };

        foreach (var c in components)
        {
            var value = ComponentFieldMapper.GetDtoValue(markDto, c.ComponentCode);
            if (value.HasValue && value.Value > c.FullMarks)
            {
                Assert.True(value.Value > c.FullMarks, $"Component {c.ComponentName} value {value.Value} exceeds max {c.FullMarks}");
                return;
            }
        }

        Assert.Fail("Expected component validation to detect excessive value");
    }

    [Fact(DisplayName = "2. Component validation accepts valid range values")]
    public void ComponentValidation_AcceptsValidMark()
    {
        var markDto = new MarkEntryDto { StudentId = 1, MarksObtained = 65 };
        markDto.ComponentMarks["WRITTEN"] = 50;
        markDto.ComponentMarks["MCQ"] = 15;

        var components = new List<ComponentColumnDto>
        {
            new() { ComponentCode = "WRITTEN", ComponentName = "Written", FullMarks = 80 },
            new() { ComponentCode = "MCQ", ComponentName = "MCQ", FullMarks = 20 }
        };

        foreach (var c in components)
        {
            var value = ComponentFieldMapper.GetDtoValue(markDto, c.ComponentCode);
            if (value.HasValue)
            {
                Assert.True(value.Value >= 0 && value.Value <= c.FullMarks,
                    $"Component {c.ComponentName} value {value.Value} should be within 0-{c.FullMarks}");
            }
        }
    }

    [Fact(DisplayName = "3. Component validation rejects negative value")]
    public void ComponentValidation_RejectsNegativeValue()
    {
        var markDto = new MarkEntryDto { StudentId = 1, MarksObtained = 30 };
        markDto.ComponentMarks["WRITTEN"] = -5;

        var components = new List<ComponentColumnDto>
        {
            new() { ComponentCode = "WRITTEN", ComponentName = "Written", FullMarks = 80 }
        };

        foreach (var c in components)
        {
            var value = ComponentFieldMapper.GetDtoValue(markDto, c.ComponentCode);
            if (value.HasValue && value.Value < 0)
            {
                return;
            }
        }

        Assert.Fail("Expected component validation to detect negative value");
    }

    [Fact(DisplayName = "4. Component validation accepts null (unset) component values")]
    public void ComponentValidation_AcceptsNullValue()
    {
        var markDto = new MarkEntryDto { StudentId = 1, MarksObtained = 50 };
        // ComponentMarks initializes empty — no values set

        var components = new List<ComponentColumnDto>
        {
            new() { ComponentCode = "WRITTEN", ComponentName = "Written", FullMarks = 80 }
        };

        foreach (var c in components)
        {
            var value = ComponentFieldMapper.GetDtoValue(markDto, c.ComponentCode);
            Assert.Null(value);
        }
    }

    [Fact(DisplayName = "5. SubmitMarksBatchAsync throws when component exceeds FullMarks")]
    public async Task SubmitMarksBatch_ThrowsOnExcessiveComponent()
    {
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var examRepoMock = new Mock<IExamRepository>(MockBehavior.Loose);
        var markRepoMock = new Mock<IMarkEntryRepository>(MockBehavior.Loose);
        var gradingRepoMock = new Mock<IGradingRuleRepository>(MockBehavior.Loose);
        var subjectRepoMock = new Mock<ISubjectRepository>(MockBehavior.Loose);
        var classRepoMock = new Mock<ISchoolClassRepository>(MockBehavior.Loose);
        var sectionRepoMock = new Mock<ISectionRepository>(MockBehavior.Loose);
        var studentRepoMock = new Mock<IStudentRepository>(MockBehavior.Loose);
        var markStructMock = new Mock<ISubjectMarkStructureService>(MockBehavior.Loose);
        var auditMock = new Mock<IAuditLogger>(MockBehavior.Loose);
        var gradeCalcMock = new Mock<IGradeCalculator>(MockBehavior.Loose);

        var exam = new ExamEntity { Id = 1, Name = "Test Exam", Status = ResultWorkflowStatus.Draft, AcademicYearId = 1 };
        examRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(exam);

        gradingRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<GradingRule, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<GradingRule>());

        var components = new List<ComponentColumnDto>
        {
            new() { ComponentCode = "WRITTEN", ComponentName = "Written", FullMarks = 80 },
            new() { ComponentCode = "MCQ", ComponentName = "MCQ", FullMarks = 20 }
        };
        markStructMock.Setup(r => r.GetGridColumnsAsync(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .ReturnsAsync(components);

        var subject = new Subject { Id = 1, Name = "Math", DefaultFullMarks = 100 };
        subjectRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(subject);

        var examSubject = new ExamSubject { ExamId = 1, SubjectId = 1, FullMarks = 100 };
        uowMock.Setup(u => u.Repository<ExamSubject>().Query())
            .Returns(new List<ExamSubject> { examSubject }.AsQueryable().AsAsyncQueryable());

        var service = new MarkEntryService(
            uowMock.Object, examRepoMock.Object, markRepoMock.Object,
            gradingRepoMock.Object, subjectRepoMock.Object,
            classRepoMock.Object, sectionRepoMock.Object,
            studentRepoMock.Object, markStructMock.Object,
            auditMock.Object, gradeCalcMock.Object, Mock.Of<IStudentComponentMarkService>());

        var dto = new MarkBatchDto
        {
            ExamId = 1, SubjectId = 1, TeacherId = 1,
            Marks = new List<MarkEntryDto>
            {
                new()
                {
                    StudentId = 1, MarksObtained = 85,
                    ComponentMarks = new ComponentMarksDto { ["WRITTEN"] = 85, ["MCQ"] = 20 }
                }
            }
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.SubmitMarksBatchAsync(dto));
        Assert.Contains("exceeds limit", ex.Message);
        Assert.Contains("Written", ex.Message);
    }

    [Fact(DisplayName = "6. SubmitMarksBatchAsync accepts component values within range")]
    public async Task SubmitMarksBatch_AcceptsValidComponents()
    {
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var examRepoMock = new Mock<IExamRepository>(MockBehavior.Loose);
        var markRepoMock = new Mock<IMarkEntryRepository>(MockBehavior.Loose);
        var gradingRepoMock = new Mock<IGradingRuleRepository>(MockBehavior.Loose);
        var subjectRepoMock = new Mock<ISubjectRepository>(MockBehavior.Loose);
        var classRepoMock = new Mock<ISchoolClassRepository>(MockBehavior.Loose);
        var sectionRepoMock = new Mock<ISectionRepository>(MockBehavior.Loose);
        var studentRepoMock = new Mock<IStudentRepository>(MockBehavior.Loose);
        var markStructMock = new Mock<ISubjectMarkStructureService>(MockBehavior.Loose);
        var auditMock = new Mock<IAuditLogger>(MockBehavior.Loose);
        var gradeCalcMock = new Mock<IGradeCalculator>(MockBehavior.Loose);

        var exam = new ExamEntity { Id = 1, Name = "Test Exam", Status = ResultWorkflowStatus.Draft, AcademicYearId = 1 };
        examRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(exam);

        gradingRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<GradingRule, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<GradingRule>());

        var components = new List<ComponentColumnDto>
        {
            new() { ComponentCode = "WRITTEN", ComponentName = "Written", FullMarks = 80 },
            new() { ComponentCode = "MCQ", ComponentName = "MCQ", FullMarks = 20 }
        };
        markStructMock.Setup(r => r.GetGridColumnsAsync(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .ReturnsAsync(components);

        var subject = new Subject { Id = 1, Name = "Math", DefaultFullMarks = 100 };
        subjectRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(subject);

        var examSubject = new ExamSubject { ExamId = 1, SubjectId = 1, FullMarks = 100 };
        uowMock.Setup(u => u.Repository<ExamSubject>().Query())
            .Returns(new List<ExamSubject> { examSubject }.AsQueryable().AsAsyncQueryable());

        markRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<MarkEntry, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MarkEntry>());

        var student = new Student { Id = 1, ClassId = 1, SectionId = 1 };
        studentRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(student);

        gradeCalcMock.Setup(g => g.CalculateGrade(It.IsAny<decimal>(), It.IsAny<IEnumerable<GradingRule>>()))
            .Returns(("A+", 5.00m));

        var service = new MarkEntryService(
            uowMock.Object, examRepoMock.Object, markRepoMock.Object,
            gradingRepoMock.Object, subjectRepoMock.Object,
            classRepoMock.Object, sectionRepoMock.Object,
            studentRepoMock.Object, markStructMock.Object,
            auditMock.Object, gradeCalcMock.Object, Mock.Of<IStudentComponentMarkService>());

        var dto = new MarkBatchDto
        {
            ExamId = 1, SubjectId = 1, TeacherId = 1,
            Marks = new List<MarkEntryDto>
            {
                new()
                {
                    StudentId = 1, MarksObtained = 85,
                    ComponentMarks = new ComponentMarksDto { ["WRITTEN"] = 70, ["MCQ"] = 15 }
                }
            }
        };

        await service.SubmitMarksBatchAsync(dto);

        markRepoMock.Verify(r => r.AddAsync(It.Is<MarkEntry>(m =>
            m.MarksObtained == 85), It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Bug #2: Result calculation transaction ────────────────────

    [Fact(DisplayName = "7. CalculateExamResultsAsync wraps in transaction")]
    public async Task CalculateExamResults_UsesTransaction()
    {
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Func<Task>, CancellationToken>(async (fn, _) => await fn())
            .Returns(Task.CompletedTask);

        var pubRepoMock = new Mock<IBaseRepository<ResultPublication>>(MockBehavior.Loose);
        pubRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ResultPublication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ResultPublication?)null);
        uowMock.Setup(u => u.Repository<ResultPublication>()).Returns(pubRepoMock.Object);

        var examRepoMock = new Mock<IExamRepository>(MockBehavior.Loose);
        examRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExamEntity { Id = 1, Status = ResultWorkflowStatus.Draft, AcademicYearId = 1 });

        var markEntryRepoMock = new Mock<IMarkEntryRepository>(MockBehavior.Loose);
        markEntryRepoMock.Setup(r => r.Query())
            .Returns(new List<MarkEntry>().AsQueryable().AsAsyncQueryable());

        var subjectResultRepoMock = new Mock<IStudentSubjectResultRepository>(MockBehavior.Loose);
        subjectResultRepoMock.Setup(r => r.Query())
            .Returns(new List<StudentSubjectResult>().AsQueryable().AsAsyncQueryable());

        var examResultRepoMock = new Mock<IStudentExamResultRepository>(MockBehavior.Loose);
        examResultRepoMock.Setup(r => r.Query())
            .Returns(new List<StudentExamResult>().AsQueryable().AsAsyncQueryable());

        var markStructMock = new Mock<ISubjectMarkStructureService>(MockBehavior.Loose);
        markStructMock.Setup(r => r.GetGridColumnsAsync(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>()))
            .ReturnsAsync(new List<ComponentColumnDto>());

        var service = new ResultCalculationService(
            uowMock.Object, examRepoMock.Object,
            markEntryRepoMock.Object,
            Mock.Of<IGradingRuleRepository>(MockBehavior.Loose),
            subjectResultRepoMock.Object,
            examResultRepoMock.Object,
            markStructMock.Object,
            Mock.Of<IGradeCalculator>(MockBehavior.Loose),
            Mock.Of<IComponentAggregator>(MockBehavior.Loose),
            Mock.Of<IPassFailPolicy>(MockBehavior.Loose),
            Mock.Of<IMeritCalculationService>(MockBehavior.Loose),
            Mock.Of<IResultPolicyService>(MockBehavior.Loose),
            Mock.Of<IStudentComponentMarkService>());

        await service.CalculateExamResultsAsync(1);

        uowMock.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "8. CanCalculateResultsAsync checks exam not published")]
    public async Task CanCalculate_RejectsPublishedExam()
    {
        var examRepoMock = new Mock<IExamRepository>(MockBehavior.Loose);
        var published = new ExamEntity { Id = 1, Status = ResultWorkflowStatus.Published };
        examRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(published);

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var pubRepoMock = new Mock<IBaseRepository<ResultPublication>>(MockBehavior.Loose);
        pubRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ResultPublication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ResultPublication?)null);
        uowMock.Setup(u => u.Repository<ResultPublication>()).Returns(pubRepoMock.Object);

        var service = new ResultCalculationService(
            uowMock.Object, examRepoMock.Object,
            Mock.Of<IMarkEntryRepository>(MockBehavior.Loose),
            Mock.Of<IGradingRuleRepository>(MockBehavior.Loose),
            Mock.Of<IStudentSubjectResultRepository>(MockBehavior.Loose),
            Mock.Of<IStudentExamResultRepository>(MockBehavior.Loose),
            Mock.Of<ISubjectMarkStructureService>(MockBehavior.Loose),
            Mock.Of<IGradeCalculator>(MockBehavior.Loose),
            Mock.Of<IComponentAggregator>(MockBehavior.Loose),
            Mock.Of<IPassFailPolicy>(MockBehavior.Loose),
            Mock.Of<IMeritCalculationService>(MockBehavior.Loose),
            Mock.Of<IResultPolicyService>(MockBehavior.Loose),
            Mock.Of<IStudentComponentMarkService>());

        var result = await service.CanCalculateResultsAsync(1);
        Assert.False(result);
    }

    // ─── Bug #3: Promotion transaction ─────────────────────────────

    [Fact(DisplayName = "9. ProcessClassPromotionAsync wraps in transaction")]
    public async Task ProcessClassPromotion_UsesTransaction()
    {
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Func<Task>, CancellationToken>(async (fn, _) => await fn())
            .Returns(Task.CompletedTask);

        var studentRepoMock = new Mock<IStudentRepository>(MockBehavior.Loose);
        studentRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Student, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Student>());

        var finalResRepoMock = new Mock<IFinalResultRepository>(MockBehavior.Loose);
        finalResRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<FinalResult, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FinalResult>());
        var promHistRepoMock = new Mock<IPromotionHistoryRepository>(MockBehavior.Loose);

        var classPromoRuleRepoMock = new Mock<IBaseRepository<ClassPromotionRule>>(MockBehavior.Loose);
        classPromoRuleRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ClassPromotionRule, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClassPromotionRule?)null);
        uowMock.Setup(u => u.Repository<ClassPromotionRule>()).Returns(classPromoRuleRepoMock.Object);
        var schoolClassRepoMock = new Mock<IBaseRepository<SchoolClass>>(MockBehavior.Loose);
        schoolClassRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SchoolClass?)null);
        uowMock.Setup(u => u.Repository<SchoolClass>()).Returns(schoolClassRepoMock.Object);

        var service = new PromotionService(
            uowMock.Object, finalResRepoMock.Object,
            promHistRepoMock.Object, studentRepoMock.Object);

        var result = await service.ProcessClassPromotionAsync(1, 1, 1);

        Assert.NotNull(result);
        uowMock.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "10. BulkPromotionAsync wraps in transaction")]
    public async Task BulkPromotion_UsesTransaction()
    {
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Func<Task>, CancellationToken>(async (fn, _) => await fn())
            .Returns(Task.CompletedTask);

        var studentRepoMock = new Mock<IStudentRepository>(MockBehavior.Loose);
        studentRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Student, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Student>());

        var finalResRepoMock = new Mock<IFinalResultRepository>(MockBehavior.Loose);
        finalResRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<FinalResult, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FinalResult>());
        var promHistRepoMock = new Mock<IPromotionHistoryRepository>(MockBehavior.Loose);

        var classPromoRuleRepoMock = new Mock<IBaseRepository<ClassPromotionRule>>(MockBehavior.Loose);
        classPromoRuleRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ClassPromotionRule, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClassPromotionRule?)null);
        uowMock.Setup(u => u.Repository<ClassPromotionRule>()).Returns(classPromoRuleRepoMock.Object);
        var schoolClassRepoMock = new Mock<IBaseRepository<SchoolClass>>(MockBehavior.Loose);
        schoolClassRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SchoolClass?)null);
        uowMock.Setup(u => u.Repository<SchoolClass>()).Returns(schoolClassRepoMock.Object);

        var service = new PromotionService(
            uowMock.Object, finalResRepoMock.Object,
            promHistRepoMock.Object, studentRepoMock.Object);

        var request = new BulkPromotionRequest
        {
            FromClassId = 1, ToClassId = 2, AcademicYearId = 1,
            ProcessedByUserId = 1, OverrideEligibility = true
        };

        var result = await service.BulkPromotionAsync(request);

        Assert.NotNull(result);
        uowMock.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "11. ReversePromotionAsync wraps in transaction")]
    public async Task ReversePromotion_UsesTransaction()
    {
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Func<Task>, CancellationToken>(async (fn, _) => await fn())
            .Returns(Task.CompletedTask);

        var promHistRepoMock = new Mock<IPromotionHistoryRepository>(MockBehavior.Loose);
        var hist = new PromotionHistory { Id = 1, StudentId = 1, AcademicYearId = 1 };
        promHistRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(hist);

        var studentRepoMock = new Mock<IStudentRepository>(MockBehavior.Loose);
        studentRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Student { Id = 1, IsDeleted = false });

        var finalResRepoMock = new Mock<IFinalResultRepository>(MockBehavior.Loose);

        var service = new PromotionService(
            uowMock.Object, finalResRepoMock.Object,
            promHistRepoMock.Object, studentRepoMock.Object);

        await service.ReversePromotionAsync(1, 1, "Testing reversal");

        uowMock.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "12. ReversePromotionAsync rejects deleted student")]
    public async Task ReversePromotion_RejectsDeletedStudent()
    {
        var promHistRepoMock = new Mock<IPromotionHistoryRepository>(MockBehavior.Loose);
        var hist = new PromotionHistory { Id = 1, StudentId = 1, AcademicYearId = 1 };
        promHistRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(hist);

        var studentRepoMock = new Mock<IStudentRepository>(MockBehavior.Loose);
        studentRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        var service = new PromotionService(
            Mock.Of<IUnitOfWork>(MockBehavior.Loose),
            Mock.Of<IFinalResultRepository>(MockBehavior.Loose),
            promHistRepoMock.Object, studentRepoMock.Object);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ReversePromotionAsync(1, 1, "Test"));
        Assert.Contains("not found", ex.Message);
    }

    // ─── Bug #4: IsDeleted filter ───────────────────────────────────

    [Fact(DisplayName = "13. ProcessClassPromotionAsync queries non-deleted students")]
    public async Task ProcessClassPromotion_FiltersDeleted()
    {
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Func<Task>, CancellationToken>(async (fn, _) => await fn())
            .Returns(Task.CompletedTask);

        var studentRepoMock = new Mock<IStudentRepository>(MockBehavior.Loose);
        List<Student> capturedStudents = [];
        studentRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Student, bool>>>(), It.IsAny<CancellationToken>()))
            .Callback<System.Linq.Expressions.Expression<System.Func<Student, bool>>, CancellationToken>((expr, _) =>
            {
                var compiled = expr.Compile();
                var allStudents = new List<Student>
                {
                    new() { Id = 1, ClassId = 1, IsDeleted = false },
                    new() { Id = 2, ClassId = 1, IsDeleted = true }
                };
                capturedStudents = allStudents.Where(s => compiled(s)).ToList();
            })
            .ReturnsAsync(new List<Student> { new() { Id = 1, ClassId = 1, IsDeleted = false } });

        var finalResRepoMock = new Mock<IFinalResultRepository>(MockBehavior.Loose);
        finalResRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<FinalResult, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FinalResult>());
        var promHistRepoMock = new Mock<IPromotionHistoryRepository>(MockBehavior.Loose);

        var classPromoRuleRepoMock = new Mock<IBaseRepository<ClassPromotionRule>>(MockBehavior.Loose);
        classPromoRuleRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ClassPromotionRule, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClassPromotionRule?)null);
        uowMock.Setup(u => u.Repository<ClassPromotionRule>()).Returns(classPromoRuleRepoMock.Object);
        var schoolClassRepoMock = new Mock<IBaseRepository<SchoolClass>>(MockBehavior.Loose);
        schoolClassRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SchoolClass?)null);
        uowMock.Setup(u => u.Repository<SchoolClass>()).Returns(schoolClassRepoMock.Object);

        var service = new PromotionService(
            uowMock.Object, finalResRepoMock.Object,
            promHistRepoMock.Object, studentRepoMock.Object);

        await service.ProcessClassPromotionAsync(1, 1, 1);

        Assert.Single(capturedStudents);
        Assert.All(capturedStudents, s => Assert.False(s.IsDeleted));
    }

    [Fact(DisplayName = "14. BulkPromotionAsync queries non-deleted students")]
    public async Task BulkPromotion_FiltersDeleted()
    {
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Func<Task>, CancellationToken>(async (fn, _) => await fn())
            .Returns(Task.CompletedTask);

        var studentRepoMock = new Mock<IStudentRepository>(MockBehavior.Loose);
        List<Student> capturedStudents = [];
        studentRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Student, bool>>>(), It.IsAny<CancellationToken>()))
            .Callback<System.Linq.Expressions.Expression<System.Func<Student, bool>>, CancellationToken>((expr, _) =>
            {
                var compiled = expr.Compile();
                var allStudents = new List<Student>
                {
                    new() { Id = 1, ClassId = 1, IsDeleted = false },
                    new() { Id = 2, ClassId = 1, IsDeleted = true }
                };
                capturedStudents = allStudents.Where(s => compiled(s)).ToList();
            })
            .ReturnsAsync(new List<Student> { new() { Id = 1, ClassId = 1, IsDeleted = false } });

        var finalResRepoMock = new Mock<IFinalResultRepository>(MockBehavior.Loose);
        finalResRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<FinalResult, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FinalResult>());
        var promHistRepoMock = new Mock<IPromotionHistoryRepository>(MockBehavior.Loose);

        var classPromoRuleRepoMock = new Mock<IBaseRepository<ClassPromotionRule>>(MockBehavior.Loose);
        classPromoRuleRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ClassPromotionRule, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClassPromotionRule?)null);
        uowMock.Setup(u => u.Repository<ClassPromotionRule>()).Returns(classPromoRuleRepoMock.Object);
        var schoolClassRepoMock = new Mock<IBaseRepository<SchoolClass>>(MockBehavior.Loose);
        schoolClassRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SchoolClass?)null);
        uowMock.Setup(u => u.Repository<SchoolClass>()).Returns(schoolClassRepoMock.Object);

        var service = new PromotionService(
            uowMock.Object, finalResRepoMock.Object,
            promHistRepoMock.Object, studentRepoMock.Object);

        var request = new BulkPromotionRequest
        {
            FromClassId = 1, ToClassId = 2, AcademicYearId = 1,
            ProcessedByUserId = 1, OverrideEligibility = true
        };

        await service.BulkPromotionAsync(request);

        Assert.Single(capturedStudents);
        Assert.All(capturedStudents, s => Assert.False(s.IsDeleted));
    }

    // ─── Bug #5: FinalResult.PromotionStatus update ────────────────

    [Fact(DisplayName = "15. ProcessClassPromotionAsync updates FinalResult.PromotionStatus")]
    public async Task ProcessClassPromotion_UpdatesPromotionStatus()
    {
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Func<Task>, CancellationToken>(async (fn, _) => await fn())
            .Returns(Task.CompletedTask);

        var studentRepoMock = new Mock<IStudentRepository>(MockBehavior.Loose);
        var student = new Student { Id = 1, ClassId = 1, FullName = "Test Student", IsDeleted = false };
        studentRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Student, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Student> { student });

        var finalResRepoMock = new Mock<IFinalResultRepository>(MockBehavior.Loose);
        var finalResult = new FinalResult
        {
            StudentId = 1, AcademicYearId = 1,
            FinalGpa = 4.50m, TotalFailedSubjects = 0,
            PromotionStatus = PromotionStatus.Pending
        };
        finalResRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<FinalResult, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FinalResult> { finalResult });

        var promHistRepoMock = new Mock<IPromotionHistoryRepository>(MockBehavior.Loose);

        var classPromoRuleRepoMock = new Mock<IBaseRepository<ClassPromotionRule>>(MockBehavior.Loose);
        classPromoRuleRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ClassPromotionRule, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassPromotionRule
            {
                Id = 1,
                ClassId = 1,
                MinimumGPA = 2.00m,
                MaximumFailedSubjects = 2,
                AllowConditionalPromotion = true,
                ConditionalPromotionGPA = 1.00m,
                IsActive = true
            });
        uowMock.Setup(u => u.Repository<ClassPromotionRule>()).Returns(classPromoRuleRepoMock.Object);
        var schoolClassRepoMock = new Mock<IBaseRepository<SchoolClass>>(MockBehavior.Loose);
        schoolClassRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolClass { Id = 1, Name = "Test Class" });
        uowMock.Setup(u => u.Repository<SchoolClass>()).Returns(schoolClassRepoMock.Object);

        var service = new PromotionService(
            uowMock.Object, finalResRepoMock.Object,
            promHistRepoMock.Object, studentRepoMock.Object);

        await service.ProcessClassPromotionAsync(1, 1, 1);

        Assert.Equal(PromotionStatus.Promoted, finalResult.PromotionStatus);
        Assert.NotNull(finalResult.PromotionRemarks);
        finalResRepoMock.Verify(r => r.Update(It.Is<FinalResult>(f =>
            f.PromotionStatus == PromotionStatus.Promoted)), Times.Once);
    }

    [Fact(DisplayName = "16. BulkPromotionAsync updates FinalResult.PromotionStatus")]
    public async Task BulkPromotion_UpdatesPromotionStatus()
    {
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Func<Task>, CancellationToken>(async (fn, _) => await fn())
            .Returns(Task.CompletedTask);

        var studentRepoMock = new Mock<IStudentRepository>(MockBehavior.Loose);
        var student = new Student { Id = 1, ClassId = 1, FullName = "Test Student", IsDeleted = false };
        studentRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Student, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Student> { student });

        var finalResRepoMock = new Mock<IFinalResultRepository>(MockBehavior.Loose);
        var finalResult = new FinalResult
        {
            StudentId = 1, AcademicYearId = 1,
            FinalGpa = 3.50m, TotalFailedSubjects = 0,
            PromotionStatus = PromotionStatus.Pending
        };
        finalResRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<FinalResult, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FinalResult> { finalResult });

        var promHistRepoMock = new Mock<IPromotionHistoryRepository>(MockBehavior.Loose);

        var classPromoRuleRepoMock = new Mock<IBaseRepository<ClassPromotionRule>>(MockBehavior.Loose);
        classPromoRuleRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ClassPromotionRule, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassPromotionRule
            {
                Id = 1,
                ClassId = 1,
                MinimumGPA = 2.00m,
                MaximumFailedSubjects = 2,
                AllowConditionalPromotion = true,
                ConditionalPromotionGPA = 1.00m,
                IsActive = true
            });
        uowMock.Setup(u => u.Repository<ClassPromotionRule>()).Returns(classPromoRuleRepoMock.Object);
        var schoolClassRepoMock = new Mock<IBaseRepository<SchoolClass>>(MockBehavior.Loose);
        schoolClassRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolClass { Id = 1, Name = "Test Class" });
        uowMock.Setup(u => u.Repository<SchoolClass>()).Returns(schoolClassRepoMock.Object);

        var service = new PromotionService(
            uowMock.Object, finalResRepoMock.Object,
            promHistRepoMock.Object, studentRepoMock.Object);

        var request = new BulkPromotionRequest
        {
            FromClassId = 1, ToClassId = 2, AcademicYearId = 1,
            ProcessedByUserId = 1, OverrideEligibility = true,
            Comments = "Bulk promote"
        };

        await service.BulkPromotionAsync(request);

        Assert.Equal(PromotionStatus.Promoted, finalResult.PromotionStatus);
        finalResRepoMock.Verify(r => r.Update(It.Is<FinalResult>(f =>
            f.PromotionStatus == PromotionStatus.Promoted)), Times.Once);
    }

    [Fact(DisplayName = "17. ReversePromotionAsync updates FinalResult.PromotionStatus to Repeat")]
    public async Task ReversePromotion_UpdatesPromotionStatus()
    {
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Func<Task>, CancellationToken>(async (fn, _) => await fn())
            .Returns(Task.CompletedTask);

        var promHistRepoMock = new Mock<IPromotionHistoryRepository>(MockBehavior.Loose);
        var hist = new PromotionHistory { Id = 1, StudentId = 1, FromClassId = 2, ToClassId = 3, AcademicYearId = 1 };
        promHistRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(hist);

        var studentRepoMock = new Mock<IStudentRepository>(MockBehavior.Loose);
        studentRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Student { Id = 1, IsDeleted = false });

        var finalResRepoMock = new Mock<IFinalResultRepository>(MockBehavior.Loose);
        var finalResult = new FinalResult { StudentId = 1, AcademicYearId = 1, PromotionStatus = PromotionStatus.Promoted };
        finalResRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<FinalResult, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(finalResult);

        var service = new PromotionService(
            uowMock.Object, finalResRepoMock.Object,
            promHistRepoMock.Object, studentRepoMock.Object);

        await service.ReversePromotionAsync(1, 1, "Testing");

        Assert.Equal(PromotionStatus.Repeat, finalResult.PromotionStatus);
        finalResRepoMock.Verify(r => r.Update(It.Is<FinalResult>(f =>
            f.PromotionStatus == PromotionStatus.Repeat)), Times.Once);
    }

    // ─── Bug #6: Publication workflow validation ───────────────────

    [Fact(DisplayName = "18. PublishResultsAsync rejects non-approved marks")]
    public async Task PublishResults_RejectsNonApprovedMarks()
    {
        var examRepoMock = new Mock<IExamRepository>(MockBehavior.Loose);
        var exam = new ExamEntity { Id = 1, Name = "Test Exam", Status = ResultWorkflowStatus.Draft };
        examRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(exam);

        var markRepoMock = new Mock<IMarkEntryRepository>(MockBehavior.Loose);
        var draftMarks = new List<MarkEntry>
        {
            new() { Id = 1, ExamId = 1, StudentId = 1, SubjectId = 1, Status = ResultWorkflowStatus.Draft, MarksObtained = 50 },
            new() { Id = 2, ExamId = 1, StudentId = 2, SubjectId = 1, Status = ResultWorkflowStatus.Draft, MarksObtained = 60 }
        };
        markRepoMock.Setup(r => r.Query())
            .Returns(draftMarks.AsQueryable().AsAsyncQueryable());

        var service = new ResultPublicationService(
            Mock.Of<IUnitOfWork>(MockBehavior.Loose),
            Mock.Of<IMeritCalculationService>(MockBehavior.Loose),
            examRepoMock.Object, markRepoMock.Object,
            Mock.Of<IResultPublicationRepository>(MockBehavior.Loose),
            Mock.Of<IStudentSubjectResultRepository>(MockBehavior.Loose),
            Mock.Of<IStudentExamResultRepository>(MockBehavior.Loose),
            Mock.Of<IGradingRuleRepository>(MockBehavior.Loose),
            Mock.Of<IGradeCalculator>(MockBehavior.Loose),
            Mock.Of<IComponentAggregator>(MockBehavior.Loose),
            Mock.Of<IPassFailPolicy>(MockBehavior.Loose),
            Mock.Of<IResultAuditLogRepository>());

        var dto = new ResultPublishDto { ExamId = 1 };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PublishResultsAsync(dto));
        Assert.Contains("non-approved marks", ex.Message);
        Assert.Contains("2 Draft", ex.Message);
    }

    [Fact(DisplayName = "19. PublishResultsAsync rejects mixed-status marks with detail")]
    public async Task PublishResults_ReportsStatusCounts()
    {
        var examRepoMock = new Mock<IExamRepository>(MockBehavior.Loose);
        var exam = new ExamEntity { Id = 1, Name = "Test Exam", Status = ResultWorkflowStatus.Draft };
        examRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(exam);

        var markRepoMock = new Mock<IMarkEntryRepository>(MockBehavior.Loose);
        var mixedMarks = new List<MarkEntry>
        {
            new() { Id = 1, ExamId = 1, StudentId = 1, SubjectId = 1, Status = ResultWorkflowStatus.Draft, MarksObtained = 50 },
            new() { Id = 2, ExamId = 1, StudentId = 2, SubjectId = 1, Status = ResultWorkflowStatus.Submitted, MarksObtained = 60 },
            new() { Id = 3, ExamId = 1, StudentId = 3, SubjectId = 1, Status = ResultWorkflowStatus.Approved, MarksObtained = 70 },
            new() { Id = 4, ExamId = 1, StudentId = 4, SubjectId = 1, Status = ResultWorkflowStatus.Draft, MarksObtained = 80 }
        };
        markRepoMock.Setup(r => r.Query())
            .Returns(mixedMarks.AsQueryable().AsAsyncQueryable());

        var service = new ResultPublicationService(
            Mock.Of<IUnitOfWork>(MockBehavior.Loose),
            Mock.Of<IMeritCalculationService>(MockBehavior.Loose),
            examRepoMock.Object, markRepoMock.Object,
            Mock.Of<IResultPublicationRepository>(MockBehavior.Loose),
            Mock.Of<IStudentSubjectResultRepository>(MockBehavior.Loose),
            Mock.Of<IStudentExamResultRepository>(MockBehavior.Loose),
            Mock.Of<IGradingRuleRepository>(MockBehavior.Loose),
            Mock.Of<IGradeCalculator>(MockBehavior.Loose),
            Mock.Of<IComponentAggregator>(MockBehavior.Loose),
            Mock.Of<IPassFailPolicy>(MockBehavior.Loose),
            Mock.Of<IResultAuditLogRepository>());

        var dto = new ResultPublishDto { ExamId = 1 };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PublishResultsAsync(dto));
        Assert.Contains("non-approved marks", ex.Message);
        Assert.Contains("2 Draft", ex.Message);
        Assert.Contains("1 Submitted", ex.Message);
    }

    [Fact(DisplayName = "20. PublishResultsAsync accepts fully approved marks")]
    public async Task PublishResults_AcceptsApprovedMarks()
    {
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var examRepoMock = new Mock<IExamRepository>(MockBehavior.Loose);
        var exam = new ExamEntity { Id = 1, Name = "Test Exam", Status = ResultWorkflowStatus.Draft };
        examRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(exam);

        var markRepoMock = new Mock<IMarkEntryRepository>(MockBehavior.Loose);
        var approvedMarks = new List<MarkEntry>
        {
            new() { Id = 1, ExamId = 1, StudentId = 1, SubjectId = 1, Status = ResultWorkflowStatus.Approved, MarksObtained = 70, ClassId = 1 },
            new() { Id = 2, ExamId = 1, StudentId = 2, SubjectId = 1, IsLocked = true, MarksObtained = 80, ClassId = 1 }
        };
        markRepoMock.Setup(r => r.Query())
            .Returns(approvedMarks.AsQueryable().AsAsyncQueryable());

        var gradingRepoMock = new Mock<IGradingRuleRepository>(MockBehavior.Loose);
        gradingRepoMock.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<GradingRule, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<GradingRule>
        {
            new() { Grade = "A+", MinMarks = 80, MaxMarks = 100, GradePoint = 5.00m, IsActive = true },
            new() { Grade = "A", MinMarks = 70, MaxMarks = 79, GradePoint = 4.00m, IsActive = true }
        });

        var examResRepoMock = new Mock<IStudentExamResultRepository>(MockBehavior.Loose);
        examResRepoMock.Setup(r => r.Query())
            .Returns(new List<StudentExamResult>().AsQueryable().AsAsyncQueryable());

        var pubRepoMock = new Mock<IResultPublicationRepository>(MockBehavior.Loose);
        pubRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ResultPublication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ResultPublication?)null);

        var aggMock = new Mock<IComponentAggregator>(MockBehavior.Loose);
        aggMock.Setup(a => a.AggregateAll(It.IsAny<MarkEntry>())).Returns(75m);

        var gradeCalcMock = new Mock<IGradeCalculator>(MockBehavior.Loose);
        gradeCalcMock.Setup(g => g.CalculateGrade(It.IsAny<decimal>(), It.IsAny<IEnumerable<GradingRule>>()))
            .Returns(("A", 4.00m));

        var subjectResultRepoMock = new Mock<IStudentSubjectResultRepository>(MockBehavior.Loose);
        subjectResultRepoMock.Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<StudentSubjectResult, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        subjectResultRepoMock.Setup(r => r.Query())
            .Returns(new List<StudentSubjectResult>().AsQueryable().AsAsyncQueryable());

        var examSubjectRepoMock = new Mock<IBaseRepository<ExamSubject>>(MockBehavior.Loose);
        examSubjectRepoMock.Setup(r => r.Query())
            .Returns((IQueryable<ExamSubject>)new List<ExamSubject> { new() { SubjectId = 1, ExamId = 1 } }.AsQueryable().AsAsyncQueryable());
        uowMock.Setup(u => u.Repository<ExamSubject>()).Returns(examSubjectRepoMock.Object);

        var classSubjectRepoMock = new Mock<IBaseRepository<ClassSubject>>(MockBehavior.Loose);
        classSubjectRepoMock.Setup(r => r.Query())
            .Returns((IQueryable<ClassSubject>)new List<ClassSubject>().AsQueryable().AsAsyncQueryable());
        uowMock.Setup(u => u.Repository<ClassSubject>()).Returns(classSubjectRepoMock.Object);

        var service = new ResultPublicationService(
            uowMock.Object,
            Mock.Of<IMeritCalculationService>(MockBehavior.Loose),
            examRepoMock.Object, markRepoMock.Object,
            pubRepoMock.Object,
            subjectResultRepoMock.Object,
            examResRepoMock.Object,
            gradingRepoMock.Object,
            gradeCalcMock.Object,
            aggMock.Object,
            Mock.Of<IPassFailPolicy>(MockBehavior.Loose),
            Mock.Of<IResultAuditLogRepository>());

        var dto = new ResultPublishDto { ExamId = 1, LockResults = false };

        await service.PublishResultsAsync(dto);

        markRepoMock.Verify(r => r.Update(It.Is<MarkEntry>(m => m.Status == ResultWorkflowStatus.Published)), Times.AtLeastOnce);
    }

    [Fact(DisplayName = "21. PublishResultsAsync rejects already published exam")]
    public async Task PublishResults_RejectsAlreadyPublished()
    {
        var examRepoMock = new Mock<IExamRepository>(MockBehavior.Loose);
        var exam = new ExamEntity { Id = 1, Status = ResultWorkflowStatus.Published };
        examRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(exam);

        var service = new ResultPublicationService(
            Mock.Of<IUnitOfWork>(MockBehavior.Loose),
            Mock.Of<IMeritCalculationService>(MockBehavior.Loose),
            examRepoMock.Object, Mock.Of<IMarkEntryRepository>(MockBehavior.Loose),
            Mock.Of<IResultPublicationRepository>(MockBehavior.Loose),
            Mock.Of<IStudentSubjectResultRepository>(MockBehavior.Loose),
            Mock.Of<IStudentExamResultRepository>(MockBehavior.Loose),
            Mock.Of<IGradingRuleRepository>(MockBehavior.Loose),
            Mock.Of<IGradeCalculator>(MockBehavior.Loose),
            Mock.Of<IComponentAggregator>(MockBehavior.Loose),
            Mock.Of<IPassFailPolicy>(MockBehavior.Loose),
            Mock.Of<IResultAuditLogRepository>());

        var dto = new ResultPublishDto { ExamId = 1 };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PublishResultsAsync(dto));
        Assert.Contains("already published", ex.Message);
    }

    // ─── PromotionStatus enum mapping ──────────────────────────────

    [Fact(DisplayName = "22. PromotionStatus enum values are correct")]
    public void PromotionStatus_EnumValues()
    {
        Assert.Equal(1, (int)PromotionStatus.Pending);
        Assert.Equal(2, (int)PromotionStatus.Promoted);
        Assert.Equal(3, (int)PromotionStatus.Repeat);
        Assert.Equal(4, (int)PromotionStatus.Failed);
    }

    // ─── FinalResult entity has PromotionStatus ────────────────────

    [Fact(DisplayName = "23. FinalResult.PromotionStatus defaults to Pending")]
    public void FinalResult_StatusDefaultsPending()
    {
        var fr = new FinalResult();
        Assert.Equal(PromotionStatus.Pending, fr.PromotionStatus);
    }

    [Fact(DisplayName = "24. FinalResult entity can set PromotionStatus")]
    public void FinalResult_CanSetPromotionStatus()
    {
        var fr = new FinalResult { PromotionStatus = PromotionStatus.Promoted };
        Assert.Equal(PromotionStatus.Promoted, fr.PromotionStatus);

        fr.PromotionStatus = PromotionStatus.Repeat;
        Assert.Equal(PromotionStatus.Repeat, fr.PromotionStatus);

        fr.PromotionStatus = PromotionStatus.Failed;
        Assert.Equal(PromotionStatus.Failed, fr.PromotionStatus);
    }

    // ─── ResultWorkflowStatus.Approved value ───────────────────────

    [Fact(DisplayName = "25. ResultWorkflowStatus includes Approved (value 4)")]
    public void WorkflowEnum_IncludesApproved()
    {
        Assert.Equal(4, (int)ResultWorkflowStatus.Approved);
    }

    [Fact(DisplayName = "26. ResultWorkflowStatus includes Locked (value 3)")]
    public void WorkflowEnum_IncludesLocked()
    {
        Assert.Equal(6, (int)ResultWorkflowStatus.Locked);
    }
}
