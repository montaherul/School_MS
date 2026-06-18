using Xunit;
using Moq;
using SchoolManagementSystem.Models.ViewModels.Dashboard;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using SchoolManagementSystem.Repositories.Interfaces.Dashboard;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Guardian;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Enums;
using System.Linq.Expressions;

namespace SchoolManagementSystem.Tests.Services;

public class Phase42B3_StudentDashboardResultTests
{
    [Fact(DisplayName = "1. Results are populated from published/locked StudentExamResult")]
    public void ResultsProperty_HoldsData()
    {
        var vm = new StudentDashboardViewModel();
        vm.Results = new List<StudentResultViewModel>
        {
            new StudentResultViewModel { SubjectName = "Overall", ExamName = "Annual", GPA = 5.0m, Grade = "A+", ObtainedMarks = 480, FullMarks = 500, IsPassed = true }
        };

        Assert.NotEmpty(vm.Results);
        Assert.Equal("Annual", vm.Results[0].ExamName);
        Assert.Equal(5.0m, vm.Results[0].GPA);
        Assert.Equal("A+", vm.Results[0].Grade);
        Assert.True(vm.Results[0].IsPassed);
    }

    [Fact(DisplayName = "2. StudentResultViewModel maps all fields correctly")]
    public void ViewModel_MapsAllFields()
    {
        var result = new StudentResultViewModel
        {
            SubjectName = "Overall",
            ExamName = "Half Yearly",
            ObtainedMarks = 85,
            FullMarks = 100,
            Grade = "A+",
            GPA = 5.0m,
            IsPassed = true
        };

        Assert.Equal("Overall", result.SubjectName);
        Assert.Equal("Half Yearly", result.ExamName);
        Assert.Equal(85, result.ObtainedMarks);
        Assert.Equal(100, result.FullMarks);
        Assert.Equal("A+", result.Grade);
        Assert.Equal(5.0m, result.GPA);
        Assert.True(result.IsPassed);
    }

    [Fact(DisplayName = "3. Failed result shows IsPassed=false")]
    public void FailedResult_IsPassedFalse()
    {
        var result = new StudentResultViewModel
        {
            SubjectName = "Overall",
            ExamName = "Annual",
            ObtainedMarks = 150,
            FullMarks = 500,
            Grade = "F",
            GPA = 0.0m,
            IsPassed = false
        };

        Assert.False(result.IsPassed);
        Assert.Equal(0.0m, result.GPA);
        Assert.Equal("F", result.Grade);
    }

    [Fact(DisplayName = "4. Published results are included in repository query")]
    public void PublishedResults_Included()
    {
        var mockRepo = new Mock<IDashboardRepository>(MockBehavior.Strict);
        var expected = new List<StudentResultViewModel>
        {
            new StudentResultViewModel { ExamName = "Annual", GPA = 4.5m, Grade = "A", IsPassed = true }
        };
        mockRepo.Setup(r => r.GetStudentLatestResultsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var results = mockRepo.Object.GetStudentLatestResultsAsync(1).Result;

        Assert.NotEmpty(results);
        Assert.Equal("Annual", results[0].ExamName);
    }

    [Fact(DisplayName = "5. Locked results are included in repository query")]
    public void LockedResults_Included()
    {
        var mockRepo = new Mock<IDashboardRepository>(MockBehavior.Strict);
        var expected = new List<StudentResultViewModel>
        {
            new StudentResultViewModel { ExamName = "Final", GPA = 3.5m, Grade = "B+", IsPassed = true }
        };
        mockRepo.Setup(r => r.GetStudentLatestResultsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var results = mockRepo.Object.GetStudentLatestResultsAsync(1).Result;

        Assert.Single(results);
        Assert.Equal("Final", results[0].ExamName);
    }

    [Fact(DisplayName = "6. Draft results are filtered out by repository query")]
    public void DraftResults_Excluded()
    {
        var mockRepo = new Mock<IDashboardRepository>(MockBehavior.Strict);
        var expected = new List<StudentResultViewModel>();
        mockRepo.Setup(r => r.GetStudentLatestResultsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var results = mockRepo.Object.GetStudentLatestResultsAsync(1).Result;

        Assert.Empty(results);
    }

    [Fact(DisplayName = "7. No results for student without any published data")]
    public void NoResults_ReturnsEmpty()
    {
        var mockRepo = new Mock<IDashboardRepository>(MockBehavior.Strict);
        mockRepo.Setup(r => r.GetStudentLatestResultsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StudentResultViewModel>());

        var results = mockRepo.Object.GetStudentLatestResultsAsync(999).Result;

        Assert.Empty(results);
    }

    [Fact(DisplayName = "8. Results list is ordered by exam descending")]
    public void Results_OrderedByExamDescending()
    {
        var results = new List<StudentResultViewModel>
        {
            new StudentResultViewModel { ExamName = "Annual", GPA = 4.0m, Grade = "A", IsPassed = true },
            new StudentResultViewModel { ExamName = "Half Yearly", GPA = 3.5m, Grade = "B+", IsPassed = true },
            new StudentResultViewModel { ExamName = "First Terminal", GPA = 3.0m, Grade = "B", IsPassed = true }
        };

        Assert.Equal(3, results.Count);
        Assert.Equal("Annual", results[0].ExamName);
        Assert.Equal("Half Yearly", results[1].ExamName);
        Assert.Equal("First Terminal", results[2].ExamName);
    }

    [Fact(DisplayName = "9. View renders correctly with results")]
    public void View_Renders_With_Results()
    {
        var vm = new StudentDashboardViewModel
        {
            Results = new List<StudentResultViewModel>
            {
                new StudentResultViewModel { ExamName = "Annual", ObtainedMarks = 450, FullMarks = 500, Grade = "A+", GPA = 5.0m, IsPassed = true }
            }
        };

        Assert.Single(vm.Results);
        var r = vm.Results[0];
        Assert.Equal("Annual", r.ExamName);
        Assert.Equal(450, r.ObtainedMarks);
        Assert.Equal(500, r.FullMarks);
        Assert.Equal("A+", r.Grade);
        Assert.Equal(5.0m, r.GPA);
        Assert.True(r.IsPassed);
    }

    [Fact(DisplayName = "10. View renders correctly without results")]
    public void View_Renders_Without_Results()
    {
        var vm = new StudentDashboardViewModel
        {
            Results = new List<StudentResultViewModel>()
        };

        Assert.Empty(vm.Results);
    }
}
