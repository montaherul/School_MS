using Microsoft.Extensions.Logging;
using Moq;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class CalendarOperationalTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly Mock<ILogger<CalendarGenerationService>> _loggerMock = new();
    private readonly Mock<IBaseRepository<HolidayMaster>> _holidayRepoMock = new();
    private readonly Mock<IBaseRepository<ExamSchedule>> _scheduleRepoMock = new();
    private readonly List<HolidayMaster> _holidayData = new();
    private readonly List<ExamSchedule> _scheduleData = new();

    public CalendarOperationalTests()
    {
        _uowMock.Setup(x => x.Repository<HolidayMaster>()).Returns(_holidayRepoMock.Object);
        _uowMock.Setup(x => x.Repository<ExamSchedule>()).Returns(_scheduleRepoMock.Object);
        _holidayRepoMock.Setup(x => x.Query()).Returns(() => _holidayData.AsAsyncQueryable());
        _scheduleRepoMock.Setup(x => x.Query()).Returns(() => _scheduleData.AsAsyncQueryable());
    }

    [Fact]
    public async Task GenerateYearAsync_GeneratesCorrectDayCount()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.GenerateYearAsync(1, 2026);

        Assert.Equal(365, result.Count);
    }

    [Fact]
    public async Task GenerateYearAsync_DatesAreConsecutive()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.GenerateYearAsync(1, 2026);

        for (int i = 1; i < result.Count; i++)
            Assert.Equal(result[i - 1].Date.AddDays(1), result[i].Date);
    }

    [Fact]
    public async Task GenerateYearAsync_WeekendsAreHolidays()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.GenerateYearAsync(1, 2026);

        var friday = result.First(d => d.Date.DayOfWeek == DayOfWeek.Friday);
        var saturday = result.First(d => d.Date.DayOfWeek == DayOfWeek.Saturday);
        var sunday = result.First(d => d.Date.DayOfWeek == DayOfWeek.Sunday);

        Assert.True(friday.IsHoliday);
        Assert.True(saturday.IsHoliday);
        Assert.False(sunday.IsHoliday);
    }

    [Fact]
    public async Task SyncHolidaysAsync_ReturnsNonNegative()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.SyncHolidaysAsync(1, 2026);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task ValidateCalendarAsync_PassesWithValidData()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var data = new List<AcademicCalendar>
        {
            new() { AcademicYearId = 1, Date = new DateOnly(2026, 6, 1), IsHoliday = false, IsWorkingDay = true },
            new() { AcademicYearId = 1, Date = new DateOnly(2026, 6, 2), IsHoliday = true, IsWorkingDay = false }
        };
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        calRepo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        await service.ValidateCalendarAsync(1);

        Assert.All(data, e => Assert.Null(e.Remarks));
    }

    [Fact]
    public async Task ValidateCalendarAsync_FlagsInvalidHolidayAndWorkingDay()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var data = new List<AcademicCalendar>
        {
            new() { Id = 1, AcademicYearId = 1, Date = new DateOnly(2026, 6, 1), IsHoliday = true, IsWorkingDay = true }
        };
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        calRepo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        await service.ValidateCalendarAsync(1);

        Assert.NotNull(data[0].Remarks);
        Assert.Contains("holiday", data[0].Remarks, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateCalendarAsync_FlagsExamOnHoliday()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var data = new List<AcademicCalendar>
        {
            new() { Id = 1, AcademicYearId = 1, Date = new DateOnly(2026, 6, 1), IsHoliday = true, IsExamDay = true, IsWorkingDay = false }
        };
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        calRepo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        await service.ValidateCalendarAsync(1);

        Assert.NotNull(data[0].Remarks);
        Assert.Contains("Exam", data[0].Remarks);
    }

    [Fact]
    public async Task SyncExamDaysAsync_Zero_WhenNoExamSchedules()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.SyncExamDaysAsync(1, 2026);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RepairMissingDatesAsync_AddsMissingDays()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        var existing = new List<AcademicCalendar>
        {
            new() { AcademicYearId = 1, Date = new DateOnly(2026, 1, 1), IsHoliday = false, IsWorkingDay = true }
        };
        var added = new List<AcademicCalendar>();
        calRepo.Setup(x => x.Query()).Returns(() => existing.AsAsyncQueryable());
        calRepo.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<AcademicCalendar>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AcademicCalendar>, CancellationToken>((entries, _) => added.AddRange(entries));

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        await service.RepairMissingDatesAsync(1, 2026);

        Assert.True(added.Count > 0);
        Assert.Equal(364, added.Count);
    }

    [Fact]
    public async Task RegenerateYearAsync_ReplacesAllEntries()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.RegenerateYearAsync(1, 2026);

        Assert.Equal(365, result.Count);
    }

    [Fact]
    public async Task GenerateYearAsync_SetsAcademicYearId()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.GenerateYearAsync(42, 2026);

        Assert.All(result, e => Assert.Equal(42, e.AcademicYearId));
    }
}
