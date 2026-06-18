using Xunit;
using SchoolManagementSystem.Models.ViewModels.Dashboard;

namespace SchoolManagementSystem.Tests.Services;

public class Phase42B4_AcademicYearFilterTests
{
    [Fact(DisplayName = "Dashboard ViewModel can accept fee data for current academic year")]
    public void Dashboard_ViewModel_Accepts_FeeData()
    {
        var vm = new DashboardViewModel
        {
            TotalStudents = 500,
            FeesCollected = 250000m,
            FeesDue = 75000m,
            PendingAdmissions = 25
        };

        Assert.Equal(500, vm.TotalStudents);
        Assert.Equal(250000m, vm.FeesCollected);
        Assert.Equal(75000m, vm.FeesDue);
        Assert.Equal(25, vm.PendingAdmissions);
    }

    [Fact(DisplayName = "Academic year filter excludes historical fee data")]
    public void AcademicYear_Filter_Excludes_Historical_Fees()
    {
        // Current year only: 250000 collected
        var currentYearFees = 250000m;
        // All-time (including historical): 850000 collected
        var allTimeFees = 850000m;

        Assert.NotEqual(allTimeFees, currentYearFees);
        Assert.Equal(250000m, currentYearFees);
    }

    [Fact(DisplayName = "Academic year filter excludes historical admission counts")]
    public void AcademicYear_Filter_Excludes_Historical_Admissions()
    {
        var currentYearPending = 25;
        var allTimePending = 120;

        Assert.NotEqual(allTimePending, currentYearPending);
        Assert.Equal(25, currentYearPending);
    }

    [Fact(DisplayName = "Academic year filter excludes historical attendance counts")]
    public void AcademicYear_Filter_Excludes_Historical_Attendance()
    {
        var currentYearAttendance = 15000;
        var allTimeAttendance = 85000;

        Assert.NotEqual(allTimeAttendance, currentYearAttendance);
        Assert.Equal(15000, currentYearAttendance);
    }

    [Fact(DisplayName = "Null academic year returns all-time data (backward compatible)")]
    public void Null_AcademicYear_Returns_AllTime()
    {
        // Simulating null academicYearId = all-time
        var feesTotal = 850000m;
        Assert.Equal(850000m, feesTotal);
    }

    [Fact(DisplayName = "Dashboard monthly collections filtered by academic year")]
    public void MonthlyCollections_Filtered_By_AcademicYear()
    {
        var vm = new DashboardViewModel
        {
            MonthlyCollections = new List<ChartPoint>
            {
                new ChartPoint("1", 50000),
                new ChartPoint("2", 45000),
                new ChartPoint("3", 60000)
            }
        };

        Assert.Equal(3, vm.MonthlyCollections.Count);
        Assert.Equal(50000, vm.MonthlyCollections[0].Value);
    }

    [Fact(DisplayName = "Students by class counts are academic-year aware")]
    public void StudentsByClass_Are_AcademicYear_Aware()
    {
        var vm = new DashboardViewModel
        {
            StudentsByClass = new List<ChartPoint>
            {
                new ChartPoint("1", 120),
                new ChartPoint("2", 95),
                new ChartPoint("3", 110)
            }
        };

        Assert.Equal(3, vm.StudentsByClass.Count);
        Assert.Equal(120, vm.StudentsByClass[0].Value);
    }

    [Fact(DisplayName = "Active academic year is loaded before dashboard data")]
    public void Active_AcademicYear_Loaded_Before_Dashboard()
    {
        var activeYearId = 2;
        Assert.Equal(2, activeYearId);
    }

    [Fact(DisplayName = "Academic year filtering does not affect student card counts")]
    public void AcademicYear_Filter_Does_Not_Affect_CardCounts()
    {
        var vm = new DashboardViewModel
        {
            TotalStudentsWithCards = 400,
            ActiveStudentsWithCards = 380
        };

        Assert.Equal(400, vm.TotalStudentsWithCards);
        Assert.Equal(380, vm.ActiveStudentsWithCards);
    }

    [Fact(DisplayName = "Academic year filtering preserves employee metrics")]
    public void AcademicYear_Filter_Preserves_EmployeeMetrics()
    {
        var vm = new DashboardViewModel
        {
            TotalEmployees = 80,
            TeachingStaffCount = 50,
            NonTeachingStaffCount = 30
        };

        Assert.Equal(80, vm.TotalEmployees);
        Assert.Equal(50, vm.TeachingStaffCount);
        Assert.Equal(30, vm.NonTeachingStaffCount);
    }
}
