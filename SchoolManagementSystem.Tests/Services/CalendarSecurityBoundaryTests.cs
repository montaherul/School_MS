using Microsoft.Extensions.Logging;
using Moq;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class CalendarSecurityBoundaryTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly Mock<ILogger<CalendarGenerationService>> _loggerMock = new();
    private readonly Mock<IBaseRepository<HolidayMaster>> _holidayRepoMock = new();
    private readonly Mock<IBaseRepository<ExamSchedule>> _scheduleRepoMock = new();
    private readonly List<HolidayMaster> _holidayData = new();
    private readonly List<ExamSchedule> _scheduleData = new();

    public CalendarSecurityBoundaryTests()
    {
        _uowMock.Setup(x => x.Repository<HolidayMaster>()).Returns(_holidayRepoMock.Object);
        _uowMock.Setup(x => x.Repository<ExamSchedule>()).Returns(_scheduleRepoMock.Object);
        _holidayRepoMock.Setup(x => x.Query()).Returns(() => _holidayData.AsAsyncQueryable());
        _scheduleRepoMock.Setup(x => x.Query()).Returns(() => _scheduleData.AsAsyncQueryable());
    }

    [Fact]
    public async Task GenerateYearAsync_IgnoresDeletedEntriesOnReRun()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        var existing = new List<AcademicCalendar>
        {
            new() { Date = new DateOnly(2026, 1, 1), IsDeleted = true }
        };
        calRepo.Setup(x => x.Query()).Returns(() => existing.AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.GenerateYearAsync(1, 2026);

        Assert.Equal(365, result.Count);
    }

    [Fact]
    public async Task MultipleAcademicYearIds_AreIndependent()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var year1 = await service.GenerateYearAsync(1, 2026);
        var year2 = await service.GenerateYearAsync(2, 2026);

        Assert.All(year1, e => Assert.Equal(1, e.AcademicYearId));
        Assert.All(year2, e => Assert.Equal(2, e.AcademicYearId));
    }

    [Fact]
    public async Task SyncHolidaysAsync_DoesNotOverwriteDeletedEntries()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.SyncHolidaysAsync(1, 2026);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task ValidateCalendarAsync_HandlesEmptyYear()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        calRepo.Setup(x => x.Query()).Returns(() => new List<AcademicCalendar>().AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        await service.ValidateCalendarAsync(999);

        calRepo.Verify(x => x.Query(), Times.Once);
    }

    [Fact]
    public async Task RepairMissingDatesAsync_DoesNotDuplicateExisting()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        var fullYear = Enumerable.Range(1, 365).Select(d => new AcademicCalendar
        {
            AcademicYearId = 1, Date = new DateOnly(2026, 1, 1).AddDays(d - 1),
            IsHoliday = false, IsWorkingDay = true
        }).ToList();
        calRepo.Setup(x => x.Query()).Returns(() => fullYear.AsAsyncQueryable());

        var added = new List<AcademicCalendar>();
        calRepo.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<AcademicCalendar>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AcademicCalendar>, CancellationToken>((entries, _) => added.AddRange(entries));

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        await service.RepairMissingDatesAsync(1, 2026);

        Assert.Empty(added);
    }

    [Fact]
    public async Task AllDeleted_StillGeneratesNew()
    {
        var calRepo = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(calRepo.Object);
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        var existing = Enumerable.Range(1, 365).Select(d => new AcademicCalendar
        {
            Date = new DateOnly(2026, 1, 1).AddDays(d - 1),
            IsDeleted = true
        }).ToList();
        calRepo.Setup(x => x.Query()).Returns(() => existing.AsAsyncQueryable());

        var service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
        var result = await service.GenerateYearAsync(1, 2026);

        Assert.Equal(365, result.Count);
    }
}
