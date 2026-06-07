using SchoolManagementSystem.Models.Enums;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class WorkflowStatusTests
{
    [Fact]
    public void ResultWorkflowStatus_Draft_IsOne()
    {
        Assert.Equal(1, (int)ResultWorkflowStatus.Draft);
    }

    [Fact]
    public void ResultWorkflowStatus_Submitted_IsTwo()
    {
        Assert.Equal(2, (int)ResultWorkflowStatus.Submitted);
    }

    [Fact]
    public void ResultWorkflowStatus_Reviewed_IsThree()
    {
        Assert.Equal(3, (int)ResultWorkflowStatus.Reviewed);
    }

    [Fact]
    public void ResultWorkflowStatus_Approved_IsFour()
    {
        Assert.Equal(4, (int)ResultWorkflowStatus.Approved);
    }

    [Fact]
    public void ResultWorkflowStatus_Published_IsFive()
    {
        Assert.Equal(5, (int)ResultWorkflowStatus.Published);
    }

    [Fact]
    public void ResultWorkflowStatus_Locked_IsSix()
    {
        Assert.Equal(6, (int)ResultWorkflowStatus.Locked);
    }

    [Fact]
    public void ResultWorkflowStatus_Unpublished_IsSeven()
    {
        Assert.Equal(7, (int)ResultWorkflowStatus.Unpublished);
    }

    [Fact]
    public void OptionalSubjectMode_HasAllExpectedValues()
    {
        Assert.Equal(0, (int)OptionalSubjectMode.Disabled);
        Assert.Equal(1, (int)OptionalSubjectMode.ExcludeFromGPA);
        Assert.Equal(2, (int)OptionalSubjectMode.BonusGPA);
        Assert.Equal(3, (int)OptionalSubjectMode.BestOf);
        Assert.Equal(4, (int)OptionalSubjectMode.Custom);
    }

    [Fact]
    public void FailSubjectMode_HasAllExpectedValues()
    {
        Assert.Equal(0, (int)FailSubjectMode.StrictFail);
        Assert.Equal(1, (int)FailSubjectMode.ExcludeFail);
        Assert.Equal(2, (int)FailSubjectMode.Custom);
    }

    [Fact]
    public void PromotedStatus_Values_AreCorrect()
    {
        Assert.Equal(1, (int)PromotionStatus.Pending);
        Assert.Equal(2, (int)PromotionStatus.Promoted);
        Assert.Equal(3, (int)PromotionStatus.Repeat);
        Assert.Equal(4, (int)PromotionStatus.Failed);
    }
}
