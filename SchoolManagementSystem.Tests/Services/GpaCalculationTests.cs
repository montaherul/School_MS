using SchoolManagementSystem.Services.Implementations.Result;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class GpaCalculationTests
{
    private readonly GPACalculationService _service = new();

    [Theory]
    [InlineData(85, 5.00)]
    [InlineData(75, 4.00)]
    [InlineData(65, 3.50)]
    [InlineData(55, 3.00)]
    [InlineData(45, 2.00)]
    [InlineData(35, 1.00)]
    [InlineData(30, 0.00)]
    public async Task CalculateGPAAsync_ValidInputs_ReturnsCorrectGradePoint(decimal marks, decimal expectedGpa)
    {
        var result = await _service.CalculateGPAAsync(marks);
        Assert.Equal(expectedGpa, result);
    }

    [Fact]
    public async Task GetGradeAndPointAsync_Below33_ReturnsF()
    {
        var (grade, point) = await _service.GetGradeAndPointAsync(20);
        Assert.Equal("F", grade);
        Assert.Equal(0.00m, point);
    }

    [Fact]
    public async Task GetGradeAndPointAsync_Above80_ReturnsAPlus()
    {
        var (grade, point) = await _service.GetGradeAndPointAsync(95);
        Assert.Equal("A+", grade);
        Assert.Equal(5.00m, point);
    }

    [Fact]
    public async Task CalculateCumulativeGPAAsync_EmptyList_ReturnsZero()
    {
        var result = await _service.CalculateCumulativeGPAAsync(Array.Empty<decimal>());
        Assert.Equal(0, result);
    }

    [Theory]
    [InlineData(4.75, 5.00)]
    [InlineData(3.333, 3.33)]
    [InlineData(2.111, 2.11)]
    [InlineData(5.50, 5.00)]
    public async Task RoundGPAAccordingToRulesAsync_RoundsCorrectly(decimal raw, decimal expected)
    {
        var result = await _service.RoundGPAAccordingToRulesAsync(raw);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetLetterGrade_ValidGpa_ReturnsCorrectGrade()
    {
        Assert.Equal("A+", _service.GetLetterGrade(5.00m));
        Assert.Equal("F", _service.GetLetterGrade(0.00m));
    }

    [Fact]
    public async Task CalculateGPAAsync_FullMarksParam_ConvertsToPercentage()
    {
        var result = await _service.CalculateGPAAsync(40, 50);
        Assert.Equal(5.00m, result);
    }

    [Fact]
    public async Task GetGradeAndPointAsync_EdgeBoundaries_ReturnsCorrectGrade()
    {
        var (grade32, _) = await _service.GetGradeAndPointAsync(32.99m);
        Assert.Equal("F", grade32);

        var (grade33, _) = await _service.GetGradeAndPointAsync(33);
        Assert.Equal("D", grade33);

        var (grade80, _) = await _service.GetGradeAndPointAsync(80);
        Assert.Equal("A+", grade80);
    }
}
