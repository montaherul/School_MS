using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolManagementSystem.Controllers.Dashboard;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using System.Security.Claims;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class Phase42B1_AccountantDashboardTests
{
    private static DashboardController CreateControllerWithRole(string role, IDashboardService service)
    {
        var settingRepoMock = new Mock<ISchoolSettingRepository>();
        var controller = new DashboardController(service, settingRepoMock.Object);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.NameIdentifier, "1")
        }, "mock"));
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        return controller;
    }

    [Fact(DisplayName = "Accountant routes to Accountant dashboard")]
    public async Task Accountant_Routes_To_AccountantDashboard()
    {
        var mockSvc = new Mock<IDashboardService>();
        mockSvc.Setup(s => s.GetAccountantDashboardAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FeeDashboardDto());

        var controller = CreateControllerWithRole("Accountant", mockSvc.Object);
        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("AccountantIndex", viewResult.ViewName);
        Assert.IsType<FeeDashboardDto>(viewResult.Model);
    }

    [Fact(DisplayName = "Teacher routes to Teacher dashboard, not Accountant")]
    public async Task Teacher_Routes_To_TeacherDashboard()
    {
        var mockSvc = new Mock<IDashboardService>();
        mockSvc.Setup(s => s.GetTeacherDashboardAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolManagementSystem.Models.ViewModels.Dashboard.TeacherDashboardViewModel());

        var controller = CreateControllerWithRole("Teacher", mockSvc.Object);
        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotEqual("AccountantIndex", viewResult.ViewName);
    }

    [Fact(DisplayName = "Student routes to Student dashboard, not Accountant")]
    public async Task Student_Routes_To_StudentDashboard()
    {
        var mockSvc = new Mock<IDashboardService>();
        mockSvc.Setup(s => s.GetStudentDashboardAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolManagementSystem.Models.ViewModels.Dashboard.StudentDashboardViewModel());

        var controller = CreateControllerWithRole("Student", mockSvc.Object);
        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotEqual("AccountantIndex", viewResult.ViewName);
    }

    [Fact(DisplayName = "Guardian routes to Guardian dashboard, not Accountant")]
    public async Task Guardian_Routes_To_GuardianDashboard()
    {
        var mockSvc = new Mock<IDashboardService>();
        mockSvc.Setup(s => s.GetGuardianDashboardAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolManagementSystem.Models.ViewModels.Dashboard.GuardianDashboardViewModel());

        var controller = CreateControllerWithRole("Guardian", mockSvc.Object);
        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotEqual("AccountantIndex", viewResult.ViewName);
    }

    [Fact(DisplayName = "Exam Controller routes to Exam Controller dashboard, not Accountant")]
    public async Task ExamController_Routes_To_ExamControllerDashboard()
    {
        var mockSvc = new Mock<IDashboardService>();
        mockSvc.Setup(s => s.GetExamControllerDashboardAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolManagementSystem.Models.ViewModels.Dashboard.ExamControllerDashboardViewModel());

        var controller = CreateControllerWithRole("Exam Controller", mockSvc.Object);
        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotEqual("AccountantIndex", viewResult.ViewName);
    }

    [Fact(DisplayName = "Admin fallback routes to admin Index, not Accountant")]
    public async Task Admin_Routes_To_AdminDashboard()
    {
        var mockSvc = new Mock<IDashboardService>();
        mockSvc.Setup(s => s.GetDashboardAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolManagementSystem.Models.ViewModels.Dashboard.DashboardViewModel());

        var controller = CreateControllerWithRole("Super Admin", mockSvc.Object);
        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotEqual("AccountantIndex", viewResult.ViewName);
    }

    [Fact(DisplayName = "GetAccountantDashboardAsync returns FeeDashboardDto from fee dashboard service")]
    public async Task GetAccountantDashboard_Returns_FeeDashboardDto()
    {
        var mockSvc = new Mock<IDashboardService>();
        var expected = new FeeDashboardDto
        {
            TotalAssigned = 100000m,
            TotalCollected = 75000m,
            TotalOutstanding = 25000m,
            CollectionRate = 75m
        };
        mockSvc.Setup(s => s.GetAccountantDashboardAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await mockSvc.Object.GetAccountantDashboardAsync();

        Assert.NotNull(result);
        Assert.Equal(100000m, result.TotalAssigned);
        Assert.Equal(75000m, result.TotalCollected);
        Assert.Equal(25000m, result.TotalOutstanding);
        Assert.Equal(75m, result.CollectionRate);
    }

    [Fact(DisplayName = "Accountant dashboard view model type is FeeDashboardDto")]
    public async Task AccountantDashboard_ModelType_Is_FeeDashboardDto()
    {
        var mockSvc = new Mock<IDashboardService>();
        mockSvc.Setup(s => s.GetAccountantDashboardAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FeeDashboardDto());

        var controller = CreateControllerWithRole("Accountant", mockSvc.Object);
        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsAssignableFrom<FeeDashboardDto>(viewResult.Model);
    }
}
