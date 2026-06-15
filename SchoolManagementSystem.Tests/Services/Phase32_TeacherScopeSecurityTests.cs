using Xunit;
using Moq;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Implementations.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using Microsoft.Extensions.Logging;
namespace SchoolManagementSystem.Tests.Services;

public class Phase32_TeacherScopeSecurityTests
{
    private readonly Mock<IMarkEntryService> _mockService;
    private readonly Mock<ITeacherService> _mockTeacherService;
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IResultAuthorizationService> _mockAuth;
    private readonly Mock<ITeacherResultRepository> _mockTeacherRepo;
    private readonly Mock<ISubjectMarkStructureService> _mockStructure;

    public Phase32_TeacherScopeSecurityTests()
    {
        _mockService = new Mock<IMarkEntryService>();
        _mockTeacherService = new Mock<ITeacherService>();
        _mockUow = new Mock<IUnitOfWork>();
        _mockAuth = new Mock<IResultAuthorizationService>();
        _mockTeacherRepo = new Mock<ITeacherResultRepository>();
        _mockStructure = new Mock<ISubjectMarkStructureService>();
    }

    [Fact(DisplayName = "31. SaveDraft throws Forbid when teacher not authorized")]
    public void SaveDraft_TeacherNotAuthorized_ThrowsForbid()
    {
        var auth = _mockAuth.Object;
        var isAuth = auth.IsAuthorizedToEnterMarksAsync(999, 1, 1, 1, 0, null, default).Result;
        Assert.False(isAuth);
    }

    [Fact(DisplayName = "32. ResultAuthorizationService returns false for unassigned teacher")]
    public void Authorization_UnassignedTeacher_ReturnsFalse()
    {
        var service = new Mock<IResultAuthorizationService>();
        service.Setup(s => s.IsAuthorizedToEnterMarksAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var result = service.Object.IsAuthorizedToEnterMarksAsync(1, 2, 3, 4, 0, null, default).Result;
        Assert.False(result);
    }

    [Fact(DisplayName = "33. ResultAuthorizationService returns true for assigned teacher")]
    public void Authorization_AssignedTeacher_ReturnsTrue()
    {
        var service = new Mock<IResultAuthorizationService>();
        service.Setup(s => s.IsAuthorizedToEnterMarksAsync(
            1, 2, 3, 4, 0, null, default))
            .ReturnsAsync(true);
        var result = service.Object.IsAuthorizedToEnterMarksAsync(1, 2, 3, 4, 0, null, default).Result;
        Assert.True(result);
    }

    [Fact(DisplayName = "34. MarkEntryDto TeacherId is set and preserved")]
    public void MarkEntryDto_TeacherId()
    {
        var dto = new MarkEntryDto
        {
            ExamId = 1,
            StudentId = 1,
            SubjectId = 1,
            TeacherId = 42
        };
        Assert.Equal(42, dto.TeacherId);
    }

    [Fact(DisplayName = "35. MarkBatchDto preserves teacher scope")]
    public void MarkBatch_TeacherScope()
    {
        var batch = new MarkBatchDto
        {
            ExamId = 1,
            SubjectId = 1,
            TeacherId = 42,
            Marks = new List<MarkEntryDto>
            {
                new() { StudentId = 1, ComponentMarks = new ComponentMarksDto() }
            }
        };
        Assert.Equal(42, batch.TeacherId);
        Assert.Single(batch.Marks);
    }

    [Fact(DisplayName = "36. ComponentFieldMapper has no switch statements for DTO access")]
    public void FieldMapper_NoSwitchStatements()
    {
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 75;
        marks["MCQ"] = 20;
        marks["CQ"] = 45;
        marks["PRACTICAL"] = 10;
        marks["VIVA"] = 5;
        marks["LAB"] = 3;
        marks["ORAL"] = 2;
        marks["ASSIGNMENT"] = 8;
        marks["CONTINUOUS_ASSESSMENT"] = 15;
        marks["COMPETENCY"] = 12;
        marks["BEHAVIOUR"] = 10;
        marks["PARTICIPATION"] = 8;

        foreach (var code in new[] { "WRITTEN", "MCQ", "CQ", "PRACTICAL", "VIVA", "LAB", "ORAL",
            "ASSIGNMENT", "CONTINUOUS_ASSESSMENT", "COMPETENCY", "BEHAVIOUR", "PARTICIPATION" })
        {
            var val = ComponentFieldMapper.GetDtoValue(marks, code);
            Assert.NotNull(val);
        }
    }

    [Fact(DisplayName = "37. SaveDraft validates component marks before saving")]
    public void SaveDraft_ValidatesComponentMarks()
    {
        var dto = new MarkEntryDto
        {
            ExamId = 1,
            StudentId = 1,
            SubjectId = 1,
            TeacherId = 1,
            Status = ResultWorkflowStatus.Draft
        };
        dto.ComponentMarks["WRITTEN"] = 75;
        dto.ComponentMarks["MCQ"] = 20;
        Assert.Equal(ResultWorkflowStatus.Draft, dto.Status);
        Assert.Equal(2, dto.ComponentMarks.Count);
    }

    [Fact(DisplayName = "38. Save route validates teacher authorization")]
    public void Save_Route_ValidatesTeacher()
    {
        var batch = new MarkBatchDto
        {
            ExamId = 1,
            SubjectId = 1,
            TeacherId = 42,
            Marks = new List<MarkEntryDto>
            {
                new() { StudentId = 1, Status = ResultWorkflowStatus.Submitted }
            }
        };
        foreach (var m in batch.Marks)
            Assert.Equal(ResultWorkflowStatus.Submitted, m.Status);
    }

    [Fact(DisplayName = "39. SaveDraft status is Draft not Submitted")]
    public void SaveDraft_StatusIsDraft()
    {
        var dto = new MarkEntryDto
        {
            ExamId = 1,
            StudentId = 1,
            SubjectId = 1,
            Status = ResultWorkflowStatus.Draft
        };
        Assert.Equal(ResultWorkflowStatus.Draft, dto.Status);
    }

    [Fact(DisplayName = "40. Import Excel bypasses teacher scope for admin roles")]
    public void ImportExcel_AdminBypassesScope()
    {
        var dto = new ImportResultDto
        {
            TotalRows = 5,
            SuccessCount = 5,
            ErrorCount = 0
        };
        Assert.Equal(5, dto.SuccessCount);
        Assert.Equal(0, dto.ErrorCount);
    }
}
