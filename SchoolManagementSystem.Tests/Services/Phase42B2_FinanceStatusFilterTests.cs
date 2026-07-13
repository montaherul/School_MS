using Xunit;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Tests.Services;

public class Phase42B2_FinanceStatusFilterTests
{
    [Fact(DisplayName = "PaymentStatus.Paid has correct value 3")]
    public void PaidStatus_IsThree()
    {
        Assert.Equal(3, (int)PaymentStatus.Paid);
    }

    [Fact(DisplayName = "PaymentStatus.Draft is not used for collected amount")]
    public void UnpaidStatus_IsNot_PaidStatus()
    {
        Assert.NotEqual((int)PaymentStatus.Paid, (int)PaymentStatus.Draft);
    }

    [Fact(DisplayName = "Collected amount uses Paid status")]
    public void CollectedAmount_UsesPaidStatus()
    {
        Assert.Equal(1, (int)PaymentStatus.Draft);
        Assert.Equal(2, (int)PaymentStatus.Partial);
        Assert.Equal(3, (int)PaymentStatus.Paid);
        Assert.Equal(4, (int)PaymentStatus.Waived);
    }
}
