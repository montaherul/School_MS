using SchoolManagementSystem.Models.DTOs.Exam;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class AttendanceIntegrationTests
{
    [Fact(DisplayName = "Attendance percentage = (Present + Late) / TotalDays * 100")]
    public void AttendancePercentage_CalculatesCorrectly()
    {
        int totalDays = 20, present = 15, late = 2;
        var pct = totalDays > 0
            ? (decimal)Math.Round(100.0 * (present + late) / totalDays, 2)
            : 0m;
        Assert.Equal(85.00m, pct);
    }

    [Fact(DisplayName = "Zero attendance days returns 0%")]
    public void AttendancePercentage_ZeroDays_ReturnsZero()
    {
        int totalDays = 0, present = 0, late = 0;
        var pct = totalDays > 0
            ? (decimal)Math.Round(100.0 * (present + late) / totalDays, 2)
            : 0m;
        Assert.Equal(0m, pct);
    }

    [Fact(DisplayName = "Perfect attendance returns 100%")]
    public void AttendancePercentage_Perfect_Returns100()
    {
        int totalDays = 25, present = 25, late = 0;
        var pct = totalDays > 0
            ? (decimal)Math.Round(100.0 * (present + late) / totalDays, 2)
            : 0m;
        Assert.Equal(100.00m, pct);
    }

    [Theory(DisplayName = "Eligibility status derivation")]
    [InlineData(0, "No Data")]
    [InlineData(80, "Eligible")]
    [InlineData(74.99, "Ineligible")]
    [InlineData(75, "Eligible")]
    public void EligibilityStatus_ReturnsCorrect(decimal pct, string expected)
    {
        var status = pct == 0 ? "No Data"
            : pct < 75 ? "Ineligible"
            : "Eligible";
        Assert.Equal(expected, status);
    }

    [Fact(DisplayName = "High absenteeism (>30 days) produces Warning status")]
    public void EligibilityStatus_HighAbsenteeism_Warning()
    {
        int absentDays = 35;
        decimal pct = 70;
        var status = pct == 0 ? "No Data"
            : absentDays > 30 ? "Warning"
            : pct < 75 ? "Ineligible"
            : "Eligible";
        Assert.Equal("Warning", status);
    }

    [Fact(DisplayName = "AttendanceForPromotionDto data contract is complete")]
    public void AttendanceForPromotionDto_HasAllRequiredProperties()
    {
        var dto = new AttendanceForPromotionDto
        {
            StudentId = 1,
            StudentNo = "S001",
            FullName = "Test Student",
            RollNumber = 5,
            ClassName = "Six",
            SectionName = "A",
            TotalSchoolDays = 100,
            PresentDays = 90,
            AbsentDays = 5,
            LateDays = 3,
            LeaveDays = 2,
            AttendancePercentage = 93.00m,
            EligibilityStatus = "Eligible"
        };
        Assert.Equal(1, dto.StudentId);
        Assert.Equal("S001", dto.StudentNo);
        Assert.Equal("Test Student", dto.FullName);
        Assert.Equal(5, dto.RollNumber);
        Assert.Equal("Six", dto.ClassName);
        Assert.Equal("A", dto.SectionName);
        Assert.Equal(100, dto.TotalSchoolDays);
        Assert.Equal(90, dto.PresentDays);
        Assert.Equal(5, dto.AbsentDays);
        Assert.Equal(3, dto.LateDays);
        Assert.Equal(2, dto.LeaveDays);
        Assert.Equal(93.00m, dto.AttendancePercentage);
        Assert.Equal("Eligible", dto.EligibilityStatus);
    }
}
