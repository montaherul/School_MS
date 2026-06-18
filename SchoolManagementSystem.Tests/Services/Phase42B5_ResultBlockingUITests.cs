using Xunit;
using SchoolManagementSystem.Models.ViewModels.Dashboard;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class Phase42B5_ResultBlockingUITests
{
    [Fact(DisplayName = "Student with due cannot see results")]
    public void Student_WithDue_CannotSeeResults()
    {
        var vm = new StudentDashboardViewModel { IsResultBlocked = true };

        Assert.True(vm.IsResultBlocked);
    }

    [Fact(DisplayName = "Guardian with due cannot see results")]
    public void Guardian_WithDue_CannotSeeResults()
    {
        var vm = new GuardianDashboardViewModel { IsResultBlocked = true };

        Assert.True(vm.IsResultBlocked);
    }

    [Fact(DisplayName = "Student with no due can see results")]
    public void Student_NoDue_CanSeeResults()
    {
        var vm = new StudentDashboardViewModel { IsResultBlocked = false };

        Assert.False(vm.IsResultBlocked);
    }

    [Fact(DisplayName = "Guardian with no due can see results")]
    public void Guardian_NoDue_CanSeeResults()
    {
        var vm = new GuardianDashboardViewModel { IsResultBlocked = false };

        Assert.False(vm.IsResultBlocked);
    }

    [Fact(DisplayName = "ViewModel IsResultBlocked defaults to false")]
    public void IsResultBlocked_DefaultsToFalse()
    {
        var studentVm = new StudentDashboardViewModel();
        var guardianVm = new GuardianDashboardViewModel();

        Assert.False(studentVm.IsResultBlocked);
        Assert.False(guardianVm.IsResultBlocked);
    }
}
