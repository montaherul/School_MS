using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class CalendarPerformanceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly Mock<ILogger<CalendarGenerationService>> _genLoggerMock = new();
    private readonly Mock<ILogger<CalendarDashboardService>> _dashLoggerMock = new();

    [Fact]
    public async Task GetUpcomingHolidaysAsync_CompletesUnder500ms()
    {
        var repo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var data = Enumerable.Range(1, 100).Select(i => new AcademicCalendar
        {
            Date = today.AddDays(i), IsHoliday = true, Title = $"H{i}", HolidayType = "National", IsWorkingDay = false
        }).ToList();
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new CalendarDashboardService(_uowMock.Object, _cache, _dashLoggerMock.Object);
        var sw = Stopwatch.StartNew();
        await service.GetUpcomingHolidaysAsync(5);
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < 500,
            $"GetUpcomingHolidaysAsync took {sw.ElapsedMilliseconds}ms (limit: 500ms)");
    }

    [Fact]
    public async Task GetUpcomingExamsAsync_CompletesUnder500ms()
    {
        var repo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var data = Enumerable.Range(1, 50).Select(i => new AcademicCalendar
        {
            Date = today.AddDays(i), IsExamDay = true, Title = $"E{i}"
        }).ToList();
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new CalendarDashboardService(_uowMock.Object, _cache, _dashLoggerMock.Object);
        var sw = Stopwatch.StartNew();
        await service.GetUpcomingExamsAsync(5);
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < 500,
            $"GetUpcomingExamsAsync took {sw.ElapsedMilliseconds}ms (limit: 500ms)");
    }

    [Fact]
    public async Task GetUpcomingEventsAsync_CompletesUnder500ms()
    {
        var repo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var data = Enumerable.Range(1, 30).Select(i => new AcademicCalendar
        {
            Date = today.AddDays(i), IsEventDay = true, Title = $"Ev{i}"
        }).ToList();
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new CalendarDashboardService(_uowMock.Object, _cache, _dashLoggerMock.Object);
        var sw = Stopwatch.StartNew();
        await service.GetUpcomingEventsAsync(5);
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < 500,
            $"GetUpcomingEventsAsync took {sw.ElapsedMilliseconds}ms (limit: 500ms)");
    }

    [Fact]
    public async Task GetCurrentMonthSummaryAsync_CompletesUnder500ms()
    {
        var repo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var today = DateTime.Today;
        var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
        var data = Enumerable.Range(1, daysInMonth).Select(d => new AcademicCalendar
        {
            Date = new DateOnly(today.Year, today.Month, d),
            IsHoliday = d <= 7, IsWorkingDay = d > 7
        }).ToList();
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new CalendarDashboardService(_uowMock.Object, _cache, _dashLoggerMock.Object);
        var sw = Stopwatch.StartNew();
        await service.GetCurrentMonthSummaryAsync();
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < 500,
            $"GetCurrentMonthSummaryAsync took {sw.ElapsedMilliseconds}ms (limit: 500ms)");
    }

    [Fact]
    public async Task CacheSpeedsUpSecondCall()
    {
        var repo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var data = new List<AcademicCalendar>
        {
            new() { Date = today.AddDays(1), IsHoliday = true, Title = "H1", HolidayType = "Nat", IsWorkingDay = false }
        };
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new CalendarDashboardService(_uowMock.Object, _cache, _dashLoggerMock.Object);
        await service.GetUpcomingHolidaysAsync(5);

        var sw = Stopwatch.StartNew();
        var cached = await service.GetUpcomingHolidaysAsync(5);
        sw.Stop();

        Assert.Single(cached);
        Assert.True(sw.ElapsedMilliseconds < 100,
            $"Cached call took {sw.ElapsedMilliseconds}ms (limit: 100ms)");
    }
}
