using SchoolManagementSystem.Models.Entities.Result;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class FailSubjectTests
{
    [Fact]
    public void DeterminePassFail_NoFailedMandatory_Passes()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { IsPassed = true, IsOptionalSubject = false },
            new() { IsPassed = true, IsOptionalSubject = false }
        };

        int failedMandatory = results.Count(r => !r.IsPassed && !r.IsOptionalSubject);
        bool isPassed = failedMandatory == 0;

        Assert.True(isPassed);
        Assert.Equal(0, failedMandatory);
    }

    [Fact]
    public void DeterminePassFail_FailedOptional_StillPasses()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { IsPassed = true, IsOptionalSubject = false },
            new() { IsPassed = true, IsOptionalSubject = false },
            new() { IsPassed = false, IsOptionalSubject = true }
        };

        int failedMandatory = results.Count(r => !r.IsPassed && !r.IsOptionalSubject);
        int failedSubjects = results.Count(r => !r.IsPassed);
        bool isPassed = failedMandatory == 0;

        Assert.True(isPassed);
        Assert.Equal(1, failedSubjects);
        Assert.Equal(0, failedMandatory);
    }

    [Fact]
    public void DeterminePassFail_FailedMandatory_Fails()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { IsPassed = false, IsOptionalSubject = false },
            new() { IsPassed = true, IsOptionalSubject = false }
        };

        int failedMandatory = results.Count(r => !r.IsPassed && !r.IsOptionalSubject);
        bool isPassed = failedMandatory == 0;

        Assert.False(isPassed);
        Assert.Equal(1, failedMandatory);
    }

    [Fact]
    public void DeterminePassFail_MultipleFailedMandatory_Fails()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { IsPassed = false, IsOptionalSubject = false },
            new() { IsPassed = false, IsOptionalSubject = false },
            new() { IsPassed = false, IsOptionalSubject = true }
        };

        int failedMandatory = results.Count(r => !r.IsPassed && !r.IsOptionalSubject);
        bool isPassed = failedMandatory == 0;

        Assert.False(isPassed);
        Assert.Equal(2, failedMandatory);
    }

    [Fact]
    public void DeterminePassFail_AllFailed_Fails()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { IsPassed = false, IsOptionalSubject = false },
            new() { IsPassed = false, IsOptionalSubject = false }
        };

        int failedMandatory = results.Count(r => !r.IsPassed && !r.IsOptionalSubject);
        bool isPassed = failedMandatory == 0;

        Assert.False(isPassed);
        Assert.Equal(2, failedMandatory);
    }

    [Fact]
    public void DeterminePassFail_AllPassedIncludingOptional_Passes()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { IsPassed = true, IsOptionalSubject = false },
            new() { IsPassed = true, IsOptionalSubject = false },
            new() { IsPassed = true, IsOptionalSubject = true },
            new() { IsPassed = true, IsOptionalSubject = true }
        };

        int failedMandatory = results.Count(r => !r.IsPassed && !r.IsOptionalSubject);
        bool isPassed = failedMandatory == 0;

        Assert.True(isPassed);
        Assert.Equal(0, failedMandatory);
    }
}
