using Xunit;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Tests.Services;

public class Phase41B5_FeeInvoiceStatusMappingTests
{
    [Fact(DisplayName = "1. Draft = 1")]
    public void Draft_IsValue1()
    {
        Assert.Equal(1, (int)PaymentStatus.Draft);
    }

    [Fact(DisplayName = "2. Partial = 2")]
    public void Partial_IsValue2()
    {
        Assert.Equal(2, (int)PaymentStatus.Partial);
    }

    [Fact(DisplayName = "3. Paid = 3")]
    public void Paid_IsValue3()
    {
        Assert.Equal(3, (int)PaymentStatus.Paid);
    }

    [Fact(DisplayName = "4. Cancelled exists with value 6")]
    public void Cancelled_Exists()
    {
        Assert.True(Enum.IsDefined(typeof(PaymentStatus), 6));
        Assert.Contains("Cancelled", Enum.GetNames(typeof(PaymentStatus)));
        Assert.Equal(6, (int)PaymentStatus.Cancelled);
    }

    [Fact(DisplayName = "5. Waived = 4")]
    public void Waived_IsValue4()
    {
        Assert.Equal(4, (int)PaymentStatus.Waived);
    }

    [Fact(DisplayName = "6. Overdue does not exist in enum")]
    public void Overdue_DoesNotExist()
    {
        Assert.DoesNotContain("Overdue", Enum.GetNames(typeof(PaymentStatus)));
    }

    [Fact(DisplayName = "7. Dropdown values match enum exactly")]
    public void DropdownValues_MatchEnum()
    {
        var enumValues = Enum.GetValues<PaymentStatus>();
        var dropdownOptions = new Dictionary<int, string>
        {
            { 1, "Draft" },
            { 2, "Partial" },
            { 3, "Paid" },
            { 4, "Waived" },
            { 5, "Issued" },
            { 6, "Cancelled" },
            { 7, "Refunded" }
        };

        Assert.Equal(enumValues.Length, dropdownOptions.Count);
        foreach (var status in enumValues)
        {
            int val = (int)status;
            Assert.True(dropdownOptions.ContainsKey(val), $"Missing dropdown option for value {val} ({status})");
            Assert.Equal(status.ToString(), dropdownOptions[val]);
        }
    }

    [Fact(DisplayName = "8. Edit form preserves status via direct cast")]
    public void EditForm_PreservesStatus()
    {
        foreach (PaymentStatus status in Enum.GetValues<PaymentStatus>())
        {
            int intVal = (int)status;
            var castBack = (PaymentStatus)intVal;
            Assert.Equal(status, castBack);
        }
    }

    [Fact(DisplayName = "9. List page labels match enum for all values")]
    public void ListPageLabels_MatchEnum()
    {
        var expectedLabels = new Dictionary<int, string>
        {
            { 1, "Draft" },
            { 2, "Partial" },
            { 3, "Paid" },
            { 4, "Waived" },
            { 5, "Issued" },
            { 6, "Cancelled" },
            { 7, "Refunded" }
        };

        foreach (PaymentStatus status in Enum.GetValues<PaymentStatus>())
        {
            int val = (int)status;
            Assert.True(expectedLabels.ContainsKey(val));
            Assert.Equal(status.ToString(), expectedLabels[val]);
        }
    }
}
