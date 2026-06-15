using Microsoft.Extensions.Logging;
using Moq;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class CalendarBackupRecoveryTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly Mock<ILogger<CalendarGenerationService>> _loggerMock = new();
    private readonly Mock<IBaseRepository<HolidayMaster>> _holidayRepoMock = new();
    private readonly Mock<IBaseRepository<ExamSchedule>> _scheduleRepoMock = new();
    private readonly List<HolidayMaster> _holidayData = new();
    private readonly List<ExamSchedule> _scheduleData = new();

    public CalendarBackupRecoveryTests()
    {
        _uowMock.Setup(x => x.Repository<HolidayMaster>()).Returns(_holidayRepoMock.Object);
        _uowMock.Setup(x => x.Repository<ExamSchedule>()).Returns(_scheduleRepoMock.Object);
        _holidayRepoMock.Setup(x => x.Query()).Returns(() => _holidayData.AsAsyncQueryable());
        _scheduleRepoMock.Setup(x => x.Query()).Returns(() => _scheduleData.AsAsyncQueryable());
    }

    [Fact]
    public async Task RegenerateYearAsync_CanRecreate()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);

        var first = await service.GenerateYearAsync(1, 2026);
        Assert.Equal(365, first.Count);

        calRepo.Setup(x => x.Query()).Returns(() => first.AsAsyncQueryable());

        var regenerated = await service.RegenerateYearAsync(1, 2026);
        Assert.Equal(365, regenerated.Count);
    }

    [Fact]
    public async Task SoftDeletedEntries_DoNotBlockRegeneration()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        var added = new List<AcademicCalendar>();
        calRepo.Setup(x => x.Query()).Returns(() => added.AsAsyncQueryable());
        calRepo.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<AcademicCalendar>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AcademicCalendar>, CancellationToken>((entries, _) => added.AddRange(entries));

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        await service.GenerateYearAsync(1, 2026);
        Assert.Equal(365, added.Count);

        foreach (var e in added) e.IsDeleted = true;

        calRepo.Setup(x => x.Query()).Returns(() => added.Where(e => !e.IsDeleted).ToList().AsAsyncQueryable());

        var result = await service.GenerateYearAsync(1, 2026);
        Assert.Equal(365, result.Count);
    }

    [Fact]
    public async Task RepairMissingDatesAsync_FillsSingleGap()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        var existing = Enumerable.Range(1, 364).Select(d => new AcademicCalendar
        {
            AcademicYearId = 1, Date = new DateOnly(2026, 1, 1).AddDays(d),
            IsHoliday = false, IsWorkingDay = true
        }).ToList();
        calRepo.Setup(x => x.Query()).Returns(() => existing.AsAsyncQueryable());

        var added = new List<AcademicCalendar>();
        calRepo.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<AcademicCalendar>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AcademicCalendar>, CancellationToken>((entries, _) => added.AddRange(entries));

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        await service.RepairMissingDatesAsync(1, 2026);

        Assert.Single(added);
        Assert.Equal(new DateOnly(2026, 1, 1), added[0].Date);
    }

    [Fact]
    public async Task RepairMissingDatesAsync_RepairsLeapYear()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        var existing = Enumerable.Range(1, 365).Select(d => new AcademicCalendar
        {
            AcademicYearId = 1, Date = new DateOnly(2024, 1, 1).AddDays(d),
            IsHoliday = false, IsWorkingDay = true
        }).ToList();
        calRepo.Setup(x => x.Query()).Returns(() => existing.AsAsyncQueryable());

        var added = new List<AcademicCalendar>();
        calRepo.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<AcademicCalendar>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AcademicCalendar>, CancellationToken>((entries, _) => added.AddRange(entries));

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        await service.RepairMissingDatesAsync(1, 2024);

        Assert.Single(added);
        Assert.Equal(new DateOnly(2024, 1, 1), added[0].Date);
    }

    [Fact]
    public async Task GenerateYearAsync_LeapYear_Has366Days()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.GenerateYearAsync(1, 2024);

        Assert.Equal(366, result.Count);
        Assert.Contains(result, e => e.Date == new DateOnly(2024, 2, 29));
    }

    [Fact]
    public async Task GenerateYearAsync_NonLeapYear_Has365Days()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.GenerateYearAsync(1, 2023);

        Assert.Equal(365, result.Count);
        var feb28 = result.First(e => e.Date == new DateOnly(2023, 2, 28));
        var mar1 = result.First(e => e.Date == new DateOnly(2023, 3, 1));
        Assert.NotNull(feb28);
        Assert.NotNull(mar1);
    }

    [Fact]
    public async Task ValidateCalendarAsync_DoesNotModifyValidEntries()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        var data = new List<AcademicCalendar>
        {
            new() { Id = 1, AcademicYearId = 1, Date = new DateOnly(2026, 6, 1), IsHoliday = false, IsWorkingDay = true, Title = "Working Day" },
            new() { Id = 2, AcademicYearId = 1, Date = new DateOnly(2026, 6, 5), IsHoliday = true, IsWorkingDay = false, Title = "Friday (Weekly Off)" }
        };
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        calRepo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        await service.ValidateCalendarAsync(1);

        Assert.All(data, e => Assert.Null(e.Remarks));
    }
}
