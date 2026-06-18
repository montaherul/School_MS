using Xunit;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Services.Implementations.Fees;
using Moq;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Tests.Services;

public class Phase41B4_DashboardCollectionRateTests
{
    [Fact(DisplayName = "1. Collection rate 0% displays as 0.0%")]
    public void ZeroPercent_DisplaysCorrectly()
    {
        var rate = 0m;
        var formatted = rate.ToString("N1") + "%";
        Assert.Equal("0.0%", formatted);
    }

    [Fact(DisplayName = "2. Collection rate 25% displays as 25.0%")]
    public void TwentyFivePercent_DisplaysCorrectly()
    {
        var rate = 25m;
        var formatted = rate.ToString("N1") + "%";
        Assert.Equal("25.0%", formatted);
    }

    [Fact(DisplayName = "3. Collection rate 75.5% displays as 75.5%")]
    public void SeventyFivePointFivePercent_DisplaysCorrectly()
    {
        var rate = 75.5m;
        var formatted = rate.ToString("N1") + "%";
        Assert.Equal("75.5%", formatted);
    }

    [Fact(DisplayName = "4. Collection rate 100% displays as 100.0%")]
    public void OneHundredPercent_DisplaysCorrectly()
    {
        var rate = 100m;
        var formatted = rate.ToString("N1") + "%";
        Assert.Equal("100.0%", formatted);
    }

    [Fact(DisplayName = "5. No double multiplication occurs")]
    public void NoDoubleMultiplication()
    {
        var rate = 75.5m;
        Assert.Equal("75.5%", rate.ToString("N1") + "%");
        Assert.NotEqual("75.5%", rate.ToString("P1"));
        Assert.Equal("75.5%", (rate / 100m).ToString("P1"));
    }

    [Fact(DisplayName = "6. Dashboard DTO CollectionRate value passes through unchanged")]
    public async Task DashboardDto_CollectionRateUnchanged()
    {
        var mockRepo = new Mock<IFeeDashboardRepository>(MockBehavior.Strict);
        var expectedDto = new FeeDashboardDto
        {
            TotalAssigned = 10000m,
            TotalCollected = 7550m,
            TotalOutstanding = 2450m,
            TotalDiscounted = 500m,
            TotalInvoices = 50,
            TotalPayments = 40,
            OverdueInvoices = 5,
            CollectionRate = 75.5m
        };
        mockRepo.Setup(r => r.GetDashboardDataAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDto);

        var service = new FeeDashboardService(mockRepo.Object);
        var result = await service.GetDashboardDataAsync();

        Assert.Equal(75.5m, result.CollectionRate);
        Assert.Equal(10000m, result.TotalAssigned);
        Assert.Equal(7550m, result.TotalCollected);
        Assert.Equal(2450m, result.TotalOutstanding);
        Assert.Equal(500m, result.TotalDiscounted);
        Assert.Equal(50, result.TotalInvoices);
        Assert.Equal(40, result.TotalPayments);
        Assert.Equal(5, result.OverdueInvoices);
    }
}
