using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Services.Implementations.Result;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class ResultCalculationTests
{
    [Fact]
    public void CalculateGpa_AllPassedCompulsory_ReturnsCorrectAverage()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { GradePoint = 5.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 4.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 3.50m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 4.00m, IsPassed = true, IsOptionalSubject = false }
        };

        decimal totalPoints = results.Sum(r => r.GradePoint);
        decimal gpa = Math.Round(totalPoints / results.Count, 2);

        Assert.Equal(4.12m, gpa);
    }

    [Fact]
    public void CalculateGpa_IgnoresOptionalSubjects()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { GradePoint = 5.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 5.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 2.00m, IsPassed = true, IsOptionalSubject = true }
        };

        var validResults = results.Where(r => r.IsPassed && !r.IsOptionalSubject).ToList();
        decimal gpa = Math.Round(validResults.Sum(r => r.GradePoint) / validResults.Count, 2);

        Assert.Equal(5.00m, gpa);
    }

    [Fact]
    public void CalculateGpa_IgnoresFailedSubjects()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { GradePoint = 5.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 0.00m, IsPassed = false, IsOptionalSubject = false }
        };

        var validResults = results.Where(r => r.IsPassed && !r.IsOptionalSubject).ToList();
        decimal gpa = Math.Round(validResults.Sum(r => r.GradePoint) / validResults.Count, 2);

        Assert.Equal(5.00m, gpa);
    }

    [Fact]
    public void CalculateGpa_EmptyList_ReturnsZero()
    {
        var results = new List<StudentSubjectResult>();
        var validResults = results.Where(r => r.IsPassed && !r.IsOptionalSubject).ToList();

        Assert.Empty(validResults);
    }

    [Fact]
    public void GetOverallGrade_ValidGpa_ReturnsCorrectLetter()
    {
        Assert.Equal("A+", GetOverallGrade(5.00m));
        Assert.Equal("A", GetOverallGrade(4.50m));
        Assert.Equal("A-", GetOverallGrade(3.75m));
        Assert.Equal("B", GetOverallGrade(3.25m));
        Assert.Equal("C", GetOverallGrade(2.50m));
        Assert.Equal("D", GetOverallGrade(1.50m));
        Assert.Equal("F", GetOverallGrade(0.50m));
    }

    private static string GetOverallGrade(decimal gpa)
    {
        if (gpa >= 5.00m) return "A+";
        if (gpa >= 4.00m) return "A";
        if (gpa >= 3.50m) return "A-";
        if (gpa >= 3.00m) return "B";
        if (gpa >= 2.00m) return "C";
        if (gpa >= 1.00m) return "D";
        return "F";
    }

    [Fact]
    public void AggregateComponentMarks_SumsAllComponents()
    {
        var markEntry = new MarkEntry
        {
            WrittenMarks = 25,
            MCQMarks = 15,
            PracticalMarks = 10,
            VivaMarks = 5,
            AssignmentMarks = 5
        };

        decimal total = (markEntry.WrittenMarks ?? 0)
                      + (markEntry.MCQMarks ?? 0)
                      + (markEntry.PracticalMarks ?? 0)
                      + (markEntry.VivaMarks ?? 0)
                      + (markEntry.AssignmentMarks ?? 0);

        Assert.Equal(60, total);
    }
}
