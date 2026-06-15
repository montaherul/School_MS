using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolManagementSystem.Models.DTOs.Calendar;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class CalendarDashboardTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    private readonly Mock<ILogger<CalendarDashboardService>> _loggerMock = new();

    private ICalendarDashboardService CreateService()
    {
        return new CalendarDashboardService(_uowMock.Object, _cache, _loggerMock.Object);
    }

    [Fact]
    public async Task GetUpcomingHolidaysAsync_ReturnsUpcomingHolidays()
    {
        var repo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var data = new List<AcademicCalendar>
        {
            new() { Date = today.AddDays(1), IsHoliday = true, Title = "Test Holiday", HolidayType = "National", IsWorkingDay = false },
            new() { Date = today.AddDays(5), IsHoliday = true, Title = "Another Holiday", HolidayType = "Religious", IsWorkingDay = false }
        };
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = CreateService();
        var result = await service.GetUpcomingHolidaysAsync(5);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, h => h.Name == "Test Holiday");
        Assert.Contains(result, h => h.Name == "Another Holiday");
    }

    [Fact]
    public async Task GetUpcomingHolidaysAsync_EmptyList_WhenNoHolidays()
    {
        var repo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = CreateService();
        var result = await service.GetUpcomingHolidaysAsync(5);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUpcomingHolidaysAsync_LimitsCount()
    {
        var repo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var data = Enumerable.Range(1, 10).Select(i => new AcademicCalendar
        {
            Date = today.AddDays(i), IsHoliday = true, Title = $"H{i}", HolidayType = "National"
        }).ToList();
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = CreateService();
        var result = await service.GetUpcomingHolidaysAsync(3);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetUpcomingExamsAsync_ReturnsUpcomingExams()
    {
        var repo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var data = new List<AcademicCalendar>
        {
            new() { Date = today.AddDays(2), IsExamDay = true, Title = "Midterm" },
            new() { Date = today.AddDays(7), IsExamDay = true, Title = "Final" }
        };
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = CreateService();
        var result = await service.GetUpcomingExamsAsync(5);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, e => e.ExamName == "Midterm");
    }

    [Fact]
    public async Task GetUpcomingExamsAsync_EmptyList_WhenNoExams()
    {
        var repo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = CreateService();
        var result = await service.GetUpcomingExamsAsync(5);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUpcomingEventsAsync_ReturnsUpcomingEvents()
    {
        var repo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var data = new List<AcademicCalendar>
        {
            new() { Date = today.AddDays(3), IsEventDay = true, Title = "Sports Day" },
            new() { Date = today.AddDays(10), IsEventDay = true, Title = "Cultural Program" }
        };
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = CreateService();
        var result = await service.GetUpcomingEventsAsync(5);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, e => e.Title == "Sports Day");
    }

    [Fact]
    public async Task GetCurrentMonthSummaryAsync_ReturnsCorrectCounts()
    {
        var repo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var today = DateTime.Today;
        var startDate = new DateOnly(today.Year, today.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var monthDays = Enumerable.Range(1, endDate.Day).Select(d => new DateOnly(today.Year, today.Month, d)).ToList();

        var data = new List<AcademicCalendar>();
        foreach (var d in monthDays)
        {
            var isWeekend = d.DayOfWeek == DayOfWeek.Friday || d.DayOfWeek == DayOfWeek.Saturday;
            data.Add(new AcademicCalendar
            {
                Date = d, IsHoliday = isWeekend, IsWorkingDay = !isWeekend, IsExamDay = false
            });
        }

        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = CreateService();
        var result = await service.GetCurrentMonthSummaryAsync();

        Assert.Equal(endDate.Day, result.TotalDays);
        Assert.True(result.WorkingDays > 0);
        Assert.True(result.HolidayCount > 0);
        Assert.Equal(today.ToString("MMMM"), result.MonthName);
        Assert.Equal(today.Year, result.Year);
    }

    [Fact]
    public async Task GetAllWidgetsAsync_ReturnsAllWidgets()
    {
        var repo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var data = new List<AcademicCalendar>
        {
            new() { Date = today.AddDays(1), IsHoliday = true, Title = "Holiday", HolidayType = "National", IsWorkingDay = false },
            new() { Date = today.AddDays(2), IsExamDay = true, Title = "Exam" },
            new() { Date = today.AddDays(3), IsEventDay = true, Title = "Event" }
        };
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var monthDays = Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month))
            .Select(d => new AcademicCalendar
            {
                Date = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, d),
                IsHoliday = false, IsWorkingDay = true
            }).ToList();
        var repo2 = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo2.Object);
        repo2.Setup(x => x.Query()).Returns(() => monthDays.AsAsyncQueryable());

        var service = CreateService();
        var result = await service.GetAllWidgetsAsync();

        Assert.NotNull(result);
        Assert.NotNull(result.UpcomingHolidays);
        Assert.NotNull(result.UpcomingExams);
        Assert.NotNull(result.UpcomingEvents);
        Assert.NotNull(result.MonthSummary);
    }

    [Fact]
    public async Task UpcomingHolidayDto_DayOfWeek_IsCorrect()
    {
        var dto = new UpcomingHolidayDto { Date = new DateOnly(2026, 6, 15), Name = "Test", HolidayType = "National" };
        Assert.Equal("Monday", dto.DayOfWeek);
    }

    [Fact]
    public async Task UpcomingExamDto_DayOfWeek_IsCorrect()
    {
        var dto = new UpcomingExamDto { Date = new DateOnly(2026, 6, 20), ExamName = "Test", Subject = "Math" };
        Assert.Equal("Saturday", dto.DayOfWeek);
    }

    [Fact]
    public async Task UpcomingEventDto_DayOfWeek_IsCorrect()
    {
        var dto = new UpcomingEventDto { Date = new DateOnly(2026, 12, 25), Title = "Christmas", Description = "" };
        Assert.Equal("Friday", dto.DayOfWeek);
    }
}
