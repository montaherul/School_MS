using Moq;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Services.Implementations.Exam;
using SchoolManagementSystem.Services.Implementations.Result;
using SchoolManagementSystem.Services.Interfaces.Exam;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using Xunit;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Tests.Services;

public class Phase31B_SubjectComponentAssignmentTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly Mock<IExamValidationService> _validationMock = new(MockBehavior.Loose);

    [Fact]
    public async Task GetComponentPreviewsAsync_ReturnsCorrectPhysicsBreakdown()
    {
        var smsRepo = new Mock<IBaseRepository<SubjectMarkStructure>>(MockBehavior.Loose);
        var compRepo = new Mock<IBaseRepository<ExamComponent>>(MockBehavior.Loose);

        var components = new List<ExamComponent>
        {
            new() { Id = 1, Code = "WRITTEN", Name = "Written", IsActive = true },
            new() { Id = 2, Code = "MCQ", Name = "MCQ", IsActive = true },
            new() { Id = 4, Code = "PRACTICAL", Name = "Practical", IsActive = true },
        };

        var structures = new List<SubjectMarkStructure>
        {
            new() { SubjectId = 1, ComponentId = 1, FullMarks = 50, PassMarks = 17, IsActive = true, Component = components[0] },
            new() { SubjectId = 1, ComponentId = 2, FullMarks = 25, PassMarks = 8, IsActive = true, Component = components[1] },
            new() { SubjectId = 1, ComponentId = 4, FullMarks = 25, PassMarks = 8, IsActive = true, Component = components[2] },
        };

        _uowMock.Setup(x => x.Repository<SubjectMarkStructure>()).Returns(smsRepo.Object);
        _uowMock.Setup(x => x.Repository<ExamComponent>()).Returns(compRepo.Object);

        smsRepo.Setup(x => x.Query()).Returns(() => structures.AsAsyncQueryable());
        compRepo.Setup(x => x.Query()).Returns(() => components.AsAsyncQueryable());

        var service = new SubjectMarkStructureService(_uowMock.Object);
        var result = await service.GetComponentPreviewsAsync([1]);

        Assert.Single(result);
        Assert.Equal(1, result[0].SubjectId);
        Assert.Equal(3, result[0].Components.Count);
        Assert.Contains(result[0].Components, c => c.Name == "Written" && c.FullMarks == 50);
        Assert.Contains(result[0].Components, c => c.Name == "MCQ" && c.FullMarks == 25);
        Assert.Contains(result[0].Components, c => c.Name == "Practical" && c.FullMarks == 25);
        Assert.Equal(100, result[0].Components.Sum(c => c.FullMarks));
    }

    [Fact]
    public async Task GetComponentPreviewsAsync_ReturnsCorrectChemistryBreakdown()
    {
        var smsRepo = new Mock<IBaseRepository<SubjectMarkStructure>>(MockBehavior.Loose);
        var compRepo = new Mock<IBaseRepository<ExamComponent>>(MockBehavior.Loose);

        var components = new List<ExamComponent>
        {
            new() { Id = 1, Code = "WRITTEN", Name = "Written", IsActive = true },
            new() { Id = 2, Code = "MCQ", Name = "MCQ", IsActive = true },
            new() { Id = 5, Code = "LAB", Name = "Lab", IsActive = true },
        };

        var structures = new List<SubjectMarkStructure>
        {
            new() { SubjectId = 2, ComponentId = 1, FullMarks = 60, PassMarks = 20, IsActive = true, Component = components[0] },
            new() { SubjectId = 2, ComponentId = 2, FullMarks = 20, PassMarks = 7, IsActive = true, Component = components[1] },
            new() { SubjectId = 2, ComponentId = 5, FullMarks = 20, PassMarks = 7, IsActive = true, Component = components[2] },
        };

        _uowMock.Setup(x => x.Repository<SubjectMarkStructure>()).Returns(smsRepo.Object);
        _uowMock.Setup(x => x.Repository<ExamComponent>()).Returns(compRepo.Object);

        smsRepo.Setup(x => x.Query()).Returns(() => structures.AsAsyncQueryable());
        compRepo.Setup(x => x.Query()).Returns(() => components.AsAsyncQueryable());

        var service = new SubjectMarkStructureService(_uowMock.Object);
        var result = await service.GetComponentPreviewsAsync([2]);

        Assert.Single(result);
        Assert.Equal(2, result[0].SubjectId);
        Assert.Equal(3, result[0].Components.Count);
        Assert.Contains(result[0].Components, c => c.Name == "Written" && c.FullMarks == 60);
        Assert.Contains(result[0].Components, c => c.Name == "MCQ" && c.FullMarks == 20);
        Assert.Contains(result[0].Components, c => c.Name == "Lab" && c.FullMarks == 20);
        Assert.Equal(100, result[0].Components.Sum(c => c.FullMarks));
    }

    [Fact]
    public async Task GetComponentPreviewsAsync_ReturnsCorrectBanglaBreakdown()
    {
        var smsRepo = new Mock<IBaseRepository<SubjectMarkStructure>>(MockBehavior.Loose);
        var compRepo = new Mock<IBaseRepository<ExamComponent>>(MockBehavior.Loose);

        var components = new List<ExamComponent>
        {
            new() { Id = 1, Code = "WRITTEN", Name = "Written", IsActive = true },
            new() { Id = 2, Code = "MCQ", Name = "MCQ", IsActive = true },
        };

        var structures = new List<SubjectMarkStructure>
        {
            new() { SubjectId = 3, ComponentId = 1, FullMarks = 70, PassMarks = 23, IsActive = true, Component = components[0] },
            new() { SubjectId = 3, ComponentId = 2, FullMarks = 30, PassMarks = 10, IsActive = true, Component = components[1] },
        };

        _uowMock.Setup(x => x.Repository<SubjectMarkStructure>()).Returns(smsRepo.Object);
        _uowMock.Setup(x => x.Repository<ExamComponent>()).Returns(compRepo.Object);

        smsRepo.Setup(x => x.Query()).Returns(() => structures.AsAsyncQueryable());
        compRepo.Setup(x => x.Query()).Returns(() => components.AsAsyncQueryable());

        var service = new SubjectMarkStructureService(_uowMock.Object);
        var result = await service.GetComponentPreviewsAsync([3]);

        Assert.Single(result);
        Assert.Equal(2, result[0].Components.Count);
        Assert.Contains(result[0].Components, c => c.Name == "Written" && c.FullMarks == 70);
        Assert.Contains(result[0].Components, c => c.Name == "MCQ" && c.FullMarks == 30);
        Assert.Equal(100, result[0].Components.Sum(c => c.FullMarks));
    }

    [Fact]
    public async Task GetComponentPreviewsAsync_ReturnsCorrectICTBreakdown()
    {
        var smsRepo = new Mock<IBaseRepository<SubjectMarkStructure>>(MockBehavior.Loose);
        var compRepo = new Mock<IBaseRepository<ExamComponent>>(MockBehavior.Loose);

        var components = new List<ExamComponent>
        {
            new() { Id = 1, Code = "WRITTEN", Name = "Written", IsActive = true },
            new() { Id = 2, Code = "MCQ", Name = "MCQ", IsActive = true },
            new() { Id = 4, Code = "PRACTICAL", Name = "Practical", IsActive = true },
        };

        var structures = new List<SubjectMarkStructure>
        {
            new() { SubjectId = 4, ComponentId = 2, FullMarks = 25, PassMarks = 8, IsActive = true, Component = components[1] },
            new() { SubjectId = 4, ComponentId = 1, FullMarks = 25, PassMarks = 8, IsActive = true, Component = components[0] },
            new() { SubjectId = 4, ComponentId = 4, FullMarks = 50, PassMarks = 17, IsActive = true, Component = components[2] },
        };

        _uowMock.Setup(x => x.Repository<SubjectMarkStructure>()).Returns(smsRepo.Object);
        _uowMock.Setup(x => x.Repository<ExamComponent>()).Returns(compRepo.Object);

        smsRepo.Setup(x => x.Query()).Returns(() => structures.AsAsyncQueryable());
        compRepo.Setup(x => x.Query()).Returns(() => components.AsAsyncQueryable());

        var service = new SubjectMarkStructureService(_uowMock.Object);
        var result = await service.GetComponentPreviewsAsync([4]);

        Assert.Single(result);
        Assert.Equal(3, result[0].Components.Count);
        Assert.Contains(result[0].Components, c => c.Name == "Written" && c.FullMarks == 25);
        Assert.Contains(result[0].Components, c => c.Name == "MCQ" && c.FullMarks == 25);
        Assert.Contains(result[0].Components, c => c.Name == "Practical" && c.FullMarks == 50);
        Assert.Equal(100, result[0].Components.Sum(c => c.FullMarks));
    }

    [Fact]
    public async Task ExamSubjectService_SetupSubjectsAsync_ValidatesSubjectMarkStructure()
    {
        var examRepo = new Mock<IBaseRepository<ExamEntity>>(MockBehavior.Loose);
        var examSubRepo = new Mock<IBaseRepository<ExamSubject>>(MockBehavior.Loose);
        var smsService = new Mock<ISubjectMarkStructureService>(MockBehavior.Loose);

        _uowMock.Setup(x => x.Repository<ExamEntity>()).Returns(examRepo.Object);
        _uowMock.Setup(x => x.Repository<ExamSubject>()).Returns(examSubRepo.Object);

        examRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExamEntity { Id = 1, ClassId = 1 });

        examSubRepo.Setup(x => x.Query()).Returns(() => new List<ExamSubject>().AsAsyncQueryable());

        var missingSubjects = new List<string> { "Physics", "Chemistry" };
        _validationMock.Setup(x => x.ThrowIfSubjectMarkStructureMissingAsync(
            It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        smsService.Setup(x => x.GetComponentPreviewsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var service = new ExamSubjectService(_uowMock.Object, smsService.Object, _validationMock.Object);
        var subjects = new List<ExamSubjectConfigDto>
        {
            new() { SubjectId = 1, SubjectName = "Physics", IsActive = true, TeacherId = 1, PassMark = 33 },
            new() { SubjectId = 2, SubjectName = "Chemistry", IsActive = true, TeacherId = 2, PassMark = 33 },
        };

        await service.SetupSubjectsAsync(1, subjects);

        _validationMock.Verify(x => x.ThrowIfSubjectMarkStructureMissingAsync(
            It.Is<List<int>>(ids => ids.Count == 2 && ids.Contains(1) && ids.Contains(2)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExamSubjectService_SetupSubjectsAsync_ComputesFullMarksFromComponentPreview()
    {
        var examRepo = new Mock<IBaseRepository<ExamEntity>>(MockBehavior.Loose);
        var examSubRepo = new Mock<IBaseRepository<ExamSubject>>(MockBehavior.Loose);
        var smsService = new Mock<ISubjectMarkStructureService>(MockBehavior.Loose);

        _uowMock.Setup(x => x.Repository<ExamEntity>()).Returns(examRepo.Object);
        _uowMock.Setup(x => x.Repository<ExamSubject>()).Returns(examSubRepo.Object);

        examRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExamEntity { Id = 1, ClassId = 1 });

        examSubRepo.Setup(x => x.Query()).Returns(() => new List<ExamSubject>().AsAsyncQueryable());
        _validationMock.Setup(x => x.ThrowIfSubjectMarkStructureMissingAsync(
            It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var previews = new List<ComponentPreviewDto>
        {
            new()
            {
                SubjectId = 1,
                Components = new List<ComponentDetailDto>
                {
                    new() { Name = "Written", FullMarks = 50 },
                    new() { Name = "MCQ", FullMarks = 25 },
                    new() { Name = "Practical", FullMarks = 25 },
                }
            }
        };

        smsService.Setup(x => x.GetComponentPreviewsAsync(
            It.Is<List<int>>(ids => ids.Contains(1)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(previews);

        var addedSubjects = new List<ExamSubject>();
        examSubRepo.Setup(x => x.AddAsync(It.IsAny<ExamSubject>(), It.IsAny<CancellationToken>()))
            .Callback<ExamSubject, CancellationToken>((es, _) => addedSubjects.Add(es));

        var service = new ExamSubjectService(_uowMock.Object, smsService.Object, _validationMock.Object);
        var subjects = new List<ExamSubjectConfigDto>
        {
            new() { SubjectId = 1, SubjectName = "Physics", IsActive = true, TeacherId = 1, PassMark = 33 }
        };

        await service.SetupSubjectsAsync(1, subjects);

        var added = Assert.Single(addedSubjects);
        Assert.Equal(100, added.FullMarks);
        Assert.Equal(33, added.PassMarks);
        Assert.Equal(1, added.TeacherId);
        Assert.Equal(1, added.SubjectId);
    }

    [Fact]
    public async Task ExamValidationService_ThrowsWhenSubjectMarkStructureMissing()
    {
        var subRepo = new Mock<IBaseRepository<Subject>>(MockBehavior.Loose);
        var smsRepo = new Mock<IBaseRepository<SubjectMarkStructure>>(MockBehavior.Loose);
        var compRepo = new Mock<IBaseRepository<ExamComponent>>(MockBehavior.Loose);

        _uowMock.Setup(x => x.Repository<Subject>()).Returns(subRepo.Object);
        _uowMock.Setup(x => x.Repository<SubjectMarkStructure>()).Returns(smsRepo.Object);
        _uowMock.Setup(x => x.Repository<ExamComponent>()).Returns(compRepo.Object);

        smsRepo.Setup(x => x.Query()).Returns(() => new List<SubjectMarkStructure>().AsAsyncQueryable());
        compRepo.Setup(x => x.Query()).Returns(() => new List<ExamComponent>().AsAsyncQueryable());

        var subjects = new List<Subject>
        {
            new() { Id = 1, Name = "Physics" },
            new() { Id = 2, Name = "Chemistry" }
        };
        subRepo.Setup(x => x.Query()).Returns(() => subjects.AsAsyncQueryable());

        var validationService = new ExamValidationService(_uowMock.Object);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            validationService.ThrowIfSubjectMarkStructureMissingAsync([1, 2]));

        Assert.Contains("Physics", ex.Message);
        Assert.Contains("Chemistry", ex.Message);
        Assert.Contains("Subject mark structure is not configured", ex.Message);
    }

    [Fact]
    public async Task ExamSubjectConfigDto_NoLongerContainsLegacyTotalFields()
    {
        var dto = new ExamSubjectConfigDto
        {
            SubjectId = 1,
            SubjectName = "Test",
            FullMarks = 100,
            PassMark = 33,
            IsActive = true
        };

        Assert.Equal(100, dto.FullMarks);
        Assert.Equal(33, dto.PassMark);
        Assert.True(dto.IsActive);

        var props = dto.GetType().GetProperties();
        Assert.DoesNotContain(props, p => p.Name == "TotalWrittenMarks");
        Assert.DoesNotContain(props, p => p.Name == "TotalMCQMarks");
        Assert.DoesNotContain(props, p => p.Name == "TotalPracticalMarks");
        Assert.DoesNotContain(props, p => p.Name == "TotalVivaMarks");
        Assert.DoesNotContain(props, p => p.Name == "TotalAssignmentMarks");
    }

    [Fact]
    public async Task GetGridColumnsAsync_ReturnsComponentColumnNames()
    {
        var smsRepo = new Mock<IBaseRepository<SubjectMarkStructure>>(MockBehavior.Loose);
        var compRepo = new Mock<IBaseRepository<ExamComponent>>(MockBehavior.Loose);

        var components = new List<ExamComponent>
        {
            new() { Id = 1, Code = "WRITTEN", Name = "Written", IsActive = true },
            new() { Id = 2, Code = "MCQ", Name = "MCQ", IsActive = true },
            new() { Id = 4, Code = "PRACTICAL", Name = "Practical", IsActive = true },
        };

        var structures = new List<SubjectMarkStructure>
        {
            new() { SubjectId = 1, ComponentId = 1, FullMarks = 50, PassMarks = 17, IsActive = true, Component = components[0] },
            new() { SubjectId = 1, ComponentId = 2, FullMarks = 25, PassMarks = 8, IsActive = true, Component = components[1] },
            new() { SubjectId = 1, ComponentId = 4, FullMarks = 25, PassMarks = 7, IsActive = true, Component = components[2] },
        };

        _uowMock.Setup(x => x.Repository<SubjectMarkStructure>()).Returns(smsRepo.Object);
        _uowMock.Setup(x => x.Repository<ExamComponent>()).Returns(compRepo.Object);

        smsRepo.Setup(x => x.Query()).Returns(() => structures.AsAsyncQueryable());
        compRepo.Setup(x => x.Query()).Returns(() => components.AsAsyncQueryable());

        var service = new SubjectMarkStructureService(_uowMock.Object);
        var columns = await service.GetGridColumnsAsync(1);

        Assert.Equal(3, columns.Count);
        Assert.Contains(columns, c => c.ComponentName == "Written" && c.FullMarks == 50);
        Assert.Contains(columns, c => c.ComponentName == "MCQ" && c.FullMarks == 25);
        Assert.Contains(columns, c => c.ComponentName == "Practical" && c.FullMarks == 25);
    }

    [Fact]
    public async Task ExamSubjectService_SetupSubjectsAsync_ThrowsWhenTeacherNotAssigned()
    {
        var examRepo = new Mock<IBaseRepository<ExamEntity>>(MockBehavior.Loose);
        var examSubRepo = new Mock<IBaseRepository<ExamSubject>>(MockBehavior.Loose);
        var smsService = new Mock<ISubjectMarkStructureService>(MockBehavior.Loose);

        _uowMock.Setup(x => x.Repository<ExamEntity>()).Returns(examRepo.Object);
        _uowMock.Setup(x => x.Repository<ExamSubject>()).Returns(examSubRepo.Object);

        examRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExamEntity { Id = 1, ClassId = 1 });

        examSubRepo.Setup(x => x.Query()).Returns(() => new List<ExamSubject>().AsAsyncQueryable());
        _validationMock.Setup(x => x.ThrowIfSubjectMarkStructureMissingAsync(
            It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        smsService.Setup(x => x.GetComponentPreviewsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var service = new ExamSubjectService(_uowMock.Object, smsService.Object, _validationMock.Object);
        var subjects = new List<ExamSubjectConfigDto>
        {
            new() { SubjectId = 1, SubjectName = "Physics", IsActive = true, TeacherId = null, PassMark = 33 }
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SetupSubjectsAsync(1, subjects));

        Assert.Contains("teacher", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
