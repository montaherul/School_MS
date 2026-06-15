using Microsoft.Extensions.Logging;
using Moq;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class CalendarAccessibilityTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly Mock<ILogger<CalendarGenerationService>> _loggerMock = new();
    private readonly Mock<IBaseRepository<HolidayMaster>> _holidayRepoMock = new();
    private readonly Mock<IBaseRepository<ExamSchedule>> _scheduleRepoMock = new();
    private readonly List<HolidayMaster> _holidayData = new();
    private readonly List<ExamSchedule> _scheduleData = new();

    public CalendarAccessibilityTests()
    {
        _uowMock.Setup(x => x.Repository<HolidayMaster>()).Returns(_holidayRepoMock.Object);
        _uowMock.Setup(x => x.Repository<ExamSchedule>()).Returns(_scheduleRepoMock.Object);
        _holidayRepoMock.Setup(x => x.Query()).Returns(() => _holidayData.AsAsyncQueryable());
        _scheduleRepoMock.Setup(x => x.Query()).Returns(() => _scheduleData.AsAsyncQueryable());
    }

    [Fact]
    public async Task GenerateYearAsync_AllEntriesHaveTitles()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.GenerateYearAsync(1, 2026);

        Assert.All(result, e => Assert.False(string.IsNullOrWhiteSpace(e.Title)));
    }

    [Fact]
    public async Task GenerateYearAsync_WeekdayTitlesDescribeWorkingDay()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.GenerateYearAsync(1, 2026);

        var monday = result.First(d => d.Date.DayOfWeek == DayOfWeek.Monday);
        Assert.Equal("Working Day", monday.Title);
    }

    [Fact]
    public async Task GenerateYearAsync_WeekendTitlesDescribeWeekend()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.GenerateYearAsync(1, 2026);

        var friday = result.First(d => d.Date.DayOfWeek == DayOfWeek.Friday);
        Assert.Contains("Weekly Off", friday.Title);
    }

    [Fact]
    public async Task GenerateYearAsync_AllDatesAreDateOnly()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.GenerateYearAsync(1, 2026);

        Assert.All(result, e => Assert.IsType<DateOnly>(e.Date));
    }

    [Fact]
    public async Task GenerateYearAsync_NoFutureDatesBeyondYear()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.GenerateYearAsync(1, 2026);

        Assert.All(result, e => Assert.InRange(e.Date.Year, 2026, 2026));
    }

    [Fact]
    public void UpcomingHolidayDto_HasDayOfWeekProperty()
    {
        var dto = new SchoolManagementSystem.Models.DTOs.Calendar.UpcomingHolidayDto
        {
            Date = new DateOnly(2026, 12, 25),
            Name = "Christmas",
            HolidayType = "Religious"
        };
        Assert.Equal("Friday", dto.DayOfWeek);
    }

    [Fact]
    public void UpcomingExamDto_HasDayOfWeekProperty()
    {
        var dto = new SchoolManagementSystem.Models.DTOs.Calendar.UpcomingExamDto
        {
            Date = new DateOnly(2026, 6, 15),
            ExamName = "Final",
            Subject = "Math"
        };
        Assert.Equal("Monday", dto.DayOfWeek);
    }

    [Fact]
    public void UpcomingEventDto_HasDayOfWeekProperty()
    {
        var dto = new SchoolManagementSystem.Models.DTOs.Calendar.UpcomingEventDto
        {
            Date = new DateOnly(2026, 1, 1),
            Title = "New Year",
            Description = "First day"
        };
        Assert.Equal("Thursday", dto.DayOfWeek);
    }
}
