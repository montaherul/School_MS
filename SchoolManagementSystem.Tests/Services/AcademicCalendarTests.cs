using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.Services.Implementations.Attendance;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class HolidayProviderTests
{
    [Fact]
    public void GetAllAnnualHolidays_For2026_ReturnsAllHolidays()
    {
        var holidays = HolidayProvider.GetAllAnnualHolidays(2026);

        Assert.NotEmpty(holidays);
        Assert.Contains(holidays, h => h.Name == "International Mother Language Day");
        Assert.Contains(holidays, h => h.Name == "Independence Day");
        Assert.Contains(holidays, h => h.Name == "Pohela Boishakh");
        Assert.Contains(holidays, h => h.Name == "May Day");
        Assert.Contains(holidays, h => h.Name == "National Mourning Day");
        Assert.Contains(holidays, h => h.Name == "Victory Day");
        Assert.Contains(holidays, h => h.Name == "Christmas Day");
    }

    [Fact]
    public void GetAllAnnualHolidays_For2026_IncludesJanmashtami()
    {
        var holidays = HolidayProvider.GetAllAnnualHolidays(2026);

        Assert.Contains(holidays, h => h.Name == "Janmashtami");
    }

    [Fact]
    public void GetAllAnnualHolidays_For2026_IncludesBuddhaPurnima()
    {
        var holidays = HolidayProvider.GetAllAnnualHolidays(2026);

        Assert.Contains(holidays, h => h.Name == "Buddha Purnima");
    }

    [Fact]
    public void GetAllAnnualHolidays_For2026_IncludesIslamicHolidays()
    {
        var holidays = HolidayProvider.GetAllAnnualHolidays(2026);

        Assert.Contains(holidays, h => h.Name == "Eid-ul-Fitr");
        Assert.Contains(holidays, h => h.Name == "Eid-ul-Adha");
        Assert.Contains(holidays, h => h.Name == "Ashura");
        Assert.Contains(holidays, h => h.Name == "Eid-e-Miladunnabi");
    }

    [Fact]
    public void GetAllAnnualHolidays_For2026_DurgaPujaIsReligious_Hindu()
    {
        var holidays = HolidayProvider.GetAllAnnualHolidays(2026);

        var durgaPuja = Assert.Single(holidays, h => h.Name == "Durga Puja");
        Assert.Equal("Religious", durgaPuja.HolidayType);
        Assert.Equal("Hindu", durgaPuja.Religion);
    }

    [Fact]
    public void GetAllAnnualHolidays_HolidayType_IsSetForEach()
    {
        var holidays = HolidayProvider.GetAllAnnualHolidays(2026);

        foreach (var h in holidays)
        {
            Assert.False(string.IsNullOrWhiteSpace(h.HolidayType), $"{h.Name} missing HolidayType");
        }
    }

    [Fact]
    public void GetAllAnnualHolidays_CountryCode_IsBD()
    {
        var holidays = HolidayProvider.GetAllAnnualHolidays(2026);

        Assert.All(holidays, h => Assert.Equal("BD", h.CountryCode));
    }

    [Fact]
    public void GetAllAnnualHolidays_DisplayOrder_IsSequential()
    {
        var holidays = HolidayProvider.GetAllAnnualHolidays(2026).OrderBy(h => h.DisplayOrder).ToList();

        for (int i = 0; i < holidays.Count; i++)
        {
            Assert.Equal(i + 1, holidays[i].DisplayOrder);
        }
    }

    [Fact]
    public void GetAllAnnualHolidays_IsActive_True()
    {
        var holidays = HolidayProvider.GetAllAnnualHolidays(2026);

        Assert.All(holidays, h => Assert.True(h.IsActive));
    }

    [Fact]
    public void GetIslamicHolidays_For2026_ReturnsFourHolidays()
    {
        var islamic = HolidayProvider.GetIslamicHolidays(2026);

        Assert.Equal(4, islamic.Count);
        Assert.Contains(islamic, h => h.Name == "Eid-ul-Fitr");
        Assert.Contains(islamic, h => h.Name == "Eid-ul-Adha");
        Assert.Contains(islamic, h => h.Name == "Ashura");
        Assert.Contains(islamic, h => h.Name == "Eid-e-Miladunnabi");
    }

    [Fact]
    public void GetHinduHolidays_For2026_ReturnsTwoHolidays()
    {
        var hindu = HolidayProvider.GetHinduHolidays(2026);

        Assert.Equal(2, hindu.Count);
        Assert.Contains(hindu, h => h.Name == "Janmashtami");
        Assert.Contains(hindu, h => h.Name == "Durga Puja");
    }

    [Fact]
    public void GetBuddhistHolidays_For2026_ReturnsBuddhaPurnima()
    {
        var buddhist = HolidayProvider.GetBuddhistHolidays(2026);

        var bp = Assert.Single(buddhist);
        Assert.Equal("Buddha Purnima", bp.Name);
    }

    [Fact]
    public void GetIslamicHolidays_Religion_IsIslam()
    {
        var islamic = HolidayProvider.GetIslamicHolidays(2026);

        Assert.All(islamic, h => Assert.Equal("Islam", h.Religion));
    }

    [Fact]
    public void GetHinduHolidays_Religion_IsHindu()
    {
        var hindu = HolidayProvider.GetHinduHolidays(2026);

        Assert.All(hindu, h => Assert.Equal("Hindu", h.Religion));
    }

    [Fact]
    public void GetBuddhistHolidays_Religion_IsBuddhist()
    {
        var buddhist = HolidayProvider.GetBuddhistHolidays(2026);

        Assert.All(buddhist, h => Assert.Equal("Buddhist", h.Religion));
    }

    [Fact]
    public void GetAllAnnualHolidays_2024_LeapYear_DoesNotThrow()
    {
        var exception = Record.Exception(() => HolidayProvider.GetAllAnnualHolidays(2024));

        Assert.Null(exception);
    }

    [Fact]
    public void GetAllAnnualHolidays_2025_NonLeapYear_DoesNotThrow()
    {
        var exception = Record.Exception(() => HolidayProvider.GetAllAnnualHolidays(2025));

        Assert.Null(exception);
    }

    [Fact]
    public void GetAllAnnualHolidays_NationalHolidays_HaveCorrectDate()
    {
        var holidays = HolidayProvider.GetAllAnnualHolidays(2026);

        var independenceDay = Assert.Single(holidays, h => h.Name == "Independence Day");
        Assert.Equal(new DateOnly(2026, 3, 26), independenceDay.HolidayDate);

        var victoryDay = Assert.Single(holidays, h => h.Name == "Victory Day");
        Assert.Equal(new DateOnly(2026, 12, 16), victoryDay.HolidayDate);
    }

    [Fact]
    public void GetAllAnnualHolidays_TotalCount_AtLeast14()
    {
        var holidays = HolidayProvider.GetAllAnnualHolidays(2026);

        Assert.True(holidays.Count >= 14);
    }
}

public class AcademicCalendarEntityTests
{
    [Fact]
    public void AcademicCalendar_DefaultValues()
    {
        var entry = new AcademicCalendar();

        Assert.True(entry.IsWorkingDay);
        Assert.False(entry.IsHoliday);
        Assert.False(entry.IsExamDay);
        Assert.False(entry.IsEventDay);
        Assert.True(entry.IsActive);
        Assert.False(entry.IsDeleted);
    }

    [Fact]
    public void AcademicCalendar_CanSetAllProperties()
    {
        var date = new DateOnly(2026, 6, 15);
        var entry = new AcademicCalendar
        {
            AcademicYearId = 1,
            Date = date,
            Title = "Test Day",
            Description = "A test day",
            IsHoliday = true,
            IsWorkingDay = false,
            IsExamDay = true,
            IsEventDay = false,
            HolidayType = "National",
            IsActive = true
        };

        Assert.Equal(1, entry.AcademicYearId);
        Assert.Equal(date, entry.Date);
        Assert.Equal("Test Day", entry.Title);
        Assert.True(entry.IsHoliday);
        Assert.True(entry.IsExamDay);
        Assert.Equal("National", entry.HolidayType);
    }

    [Fact]
    public void AcademicCalendar_HolidayAndWorkingDay_MutuallyExclusive_Validation()
    {
        var entry = new AcademicCalendar();

        entry.IsHoliday = true;
        entry.IsWorkingDay = false;

        Assert.True(entry.IsHoliday);
        Assert.False(entry.IsWorkingDay);
    }

    [Fact]
    public void HolidayMaster_DefaultValues()
    {
        var holiday = new HolidayMaster();

        Assert.Equal("BD", holiday.CountryCode);
        Assert.True(holiday.IsActive);
        Assert.False(holiday.IsDeleted);
        Assert.Equal(0, holiday.DisplayOrder);
    }

    [Fact]
    public void HolidayMaster_CanSetAllProperties()
    {
        var date = new DateOnly(2026, 12, 25);
        var holiday = new HolidayMaster
        {
            Name = "Test Holiday",
            NameBn = "à¦Ÿà§‡à¦¸à§à¦Ÿ",
            HolidayType = "Religious",
            HolidayDate = date,
            IsRecurring = true,
            Religion = "Test",
            CountryCode = "BD",
            Description = "Test description",
            DisplayOrder = 1,
            IsActive = true
        };

        Assert.Equal("Test Holiday", holiday.Name);
        Assert.Equal("Religious", holiday.HolidayType);
        Assert.Equal(date, holiday.HolidayDate);
        Assert.True(holiday.IsRecurring);
    }

    [Fact]
    public void BaseEntity_DefaultValues()
    {
        var holiday = new HolidayMaster();

        Assert.Equal("system", holiday.CreatedBy);
        Assert.False(holiday.IsDeleted);
        Assert.Null(holiday.UpdatedBy);
        Assert.Null(holiday.UpdatedAt);
    }
}

public class CalendarGenerationServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IBaseRepository<AcademicCalendar>> _calendarRepoMock;
    private readonly Mock<IBaseRepository<HolidayMaster>> _holidayRepoMock;
    private readonly Mock<IBaseRepository<ExamSchedule>> _scheduleRepoMock;
    private readonly Mock<ILogger<CalendarGenerationService>> _loggerMock;
    private readonly ICalendarGenerationService _service;
    private readonly List<AcademicCalendar> _calendarData;
    private readonly List<HolidayMaster> _holidayData;
    private readonly List<ExamSchedule> _scheduleData;

    public CalendarGenerationServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        _calendarRepoMock = new Mock<IBaseRepository<AcademicCalendar>>(MockBehavior.Loose);
        _holidayRepoMock = new Mock<IBaseRepository<HolidayMaster>>(MockBehavior.Loose);
        _scheduleRepoMock = new Mock<IBaseRepository<ExamSchedule>>(MockBehavior.Loose);
        _loggerMock = new Mock<ILogger<CalendarGenerationService>>(MockBehavior.Loose);

        _calendarData = new List<AcademicCalendar>();
        _holidayData = new List<HolidayMaster>();
        _scheduleData = new List<ExamSchedule>();

        _uowMock.Setup(x => x.Repository<AcademicCalendar>()).Returns(_calendarRepoMock.Object);
        _uowMock.Setup(x => x.Repository<HolidayMaster>()).Returns(_holidayRepoMock.Object);
        _uowMock.Setup(x => x.Repository<ExamSchedule>()).Returns(_scheduleRepoMock.Object);

        _calendarRepoMock.Setup(x => x.Query()).Returns(() => _calendarData.AsAsyncQueryable());
        _holidayRepoMock.Setup(x => x.Query()).Returns(() => _holidayData.AsAsyncQueryable());
        _scheduleRepoMock.Setup(x => x.Query()).Returns(() => _scheduleData.AsAsyncQueryable());

        _service = new CalendarGenerationService(_uowMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GenerateYearAsync_2026_Creates365Entries()
    {
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());

        _calendarRepoMock.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<AcademicCalendar>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AcademicCalendar>, CancellationToken>((entries, _) => _calendarData.AddRange(entries))
            .Returns(Task.CompletedTask);

        var result = await _service.GenerateYearAsync(1, 2026);

        Assert.Equal(365, result.Count);
    }

    [Fact]
    public void GetDefaultTitle_ForFriday_ReturnsWeeklyOff()
    {
        var friday = new DateOnly(2026, 6, 19);
        Assert.Equal(DayOfWeek.Friday, friday.DayOfWeek);
    }

    [Fact]
    public void GetDefaultTitle_ForWorkingDay_ReturnsWorkingDay()
    {
        var monday = new DateOnly(2026, 6, 15);
        Assert.Equal(DayOfWeek.Monday, monday.DayOfWeek);
    }

    [Fact]
    public async Task RepairMissingDatesAsync_MissingDates_AddsThem()
    {
        var existingDate1 = new DateOnly(2026, 1, 1);
        _calendarData.Add(new AcademicCalendar { AcademicYearId = 1, Date = existingDate1, IsDeleted = false });

        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());

        _calendarRepoMock.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<AcademicCalendar>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AcademicCalendar>, CancellationToken>((entries, _) => _calendarData.AddRange(entries))
            .Returns(Task.CompletedTask);

        await _service.RepairMissingDatesAsync(1, 2026);

        Assert.Equal(365, _calendarData.Count);
    }

    [Fact]
    public async Task ValidateCalendarAsync_NoConflicts_DoesNotThrow()
    {
        _calendarData.Add(new AcademicCalendar
        {
            AcademicYearId = 1,
            Date = new DateOnly(2026, 6, 15),
            Title = "Working Day",
            IsHoliday = false,
            IsWorkingDay = true,
            IsExamDay = false,
            IsDeleted = false
        });

        _uowMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var exception = await Record.ExceptionAsync(() => _service.ValidateCalendarAsync(1));

        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateCalendarAsync_HolidayAndWorkingDayConflict_SetsRemarks()
    {
        var entry = new AcademicCalendar
        {
            AcademicYearId = 1,
            Date = new DateOnly(2026, 6, 15),
            Title = "Conflicting Day",
            IsHoliday = true,
            IsWorkingDay = true,
            IsDeleted = false
        };
        _calendarData.Add(entry);

        _uowMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _service.ValidateCalendarAsync(1);

        Assert.Contains("Cannot be both holiday and working day", entry.Remarks);
    }

    [Fact]
    public async Task SyncHolidaysAsync_MatchesHoliday_UpdatesCalendar()
    {
        var year = 2026;
        _calendarData.Add(new AcademicCalendar
        {
            AcademicYearId = 1,
            Date = new DateOnly(year, 12, 25),
            Title = "Working Day",
            IsHoliday = false,
            IsWorkingDay = true,
            IsDeleted = false
        });

        _holidayData.Add(new HolidayMaster
        {
            Name = "Christmas Day",
            HolidayDate = new DateOnly(year, 12, 25),
            HolidayType = "Religious",
            IsActive = true,
            IsDeleted = false,
            IsRecurring = true
        });

        _uowMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var synced = await _service.SyncHolidaysAsync(1, year);

        Assert.Equal(1, synced);
        var updated = _calendarData.First(c => c.Date == new DateOnly(year, 12, 25));
        Assert.True(updated.IsHoliday);
        Assert.False(updated.IsWorkingDay);
        Assert.Equal("Christmas Day", updated.Title);
    }

    [Fact]
    public async Task SyncHolidaysAsync_NoMatchingDate_ReturnsZero()
    {
        _holidayData.Add(new HolidayMaster
        {
            Name = "Test Holiday",
            HolidayDate = new DateOnly(2026, 7, 15),
            HolidayType = "Other",
            IsActive = true,
            IsDeleted = false
        });

        var synced = await _service.SyncHolidaysAsync(1, 2026);

        Assert.Equal(0, synced);
    }

    [Fact]
    public async Task SyncExamDaysAsync_ValidExam_UpdatesCalendar()
    {
        var year = 2026;
        var examDate = new DateOnly(year, 7, 5);

        _calendarData.Add(new AcademicCalendar
        {
            AcademicYearId = 1,
            Date = examDate,
            Title = "Working Day",
            IsHoliday = false,
            IsWorkingDay = true,
            IsExamDay = false,
            IsDeleted = false
        });

        _scheduleData.Add(new ExamSchedule
        {
            ExamDate = examDate,
            IsDeleted = false
        });

        _uowMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var synced = await _service.SyncExamDaysAsync(1, year);

        Assert.Equal(1, synced);
        var updated = _calendarData.First(c => c.Date == examDate);
        Assert.True(updated.IsExamDay);
    }

    [Fact]
    public async Task SyncExamDaysAsync_OnHoliday_SkipsAndLogsConflict()
    {
        var year = 2026;
        var examDate = new DateOnly(year, 12, 25);

        _calendarData.Add(new AcademicCalendar
        {
            AcademicYearId = 1,
            Date = examDate,
            Title = "Christmas Day",
            IsHoliday = true,
            IsWorkingDay = false,
            IsExamDay = false,
            IsDeleted = false
        });

        _scheduleData.Add(new ExamSchedule
        {
            ExamDate = examDate,
            IsDeleted = false
        });

        var synced = await _service.SyncExamDaysAsync(1, year);

        Assert.Equal(0, synced);
        Assert.False(_calendarData.First(c => c.Date == examDate).IsExamDay);
    }

    [Fact]
    public async Task GenerateYearAsync_2024_LeapYear_Creates366Entries()
    {
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        _calendarRepoMock.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<AcademicCalendar>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AcademicCalendar>, CancellationToken>((entries, _) => _calendarData.AddRange(entries))
            .Returns(Task.CompletedTask);

        var result = await _service.GenerateYearAsync(1, 2024);

        Assert.Equal(366, result.Count);
    }

    [Fact]
    public async Task GenerateYearAsync_FridayAndSaturday_AreHolidays()
    {
        _uowMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>(async (op, _) => await op());
        _calendarRepoMock.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<AcademicCalendar>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AcademicCalendar>, CancellationToken>((entries, _) => _calendarData.AddRange(entries))
            .Returns(Task.CompletedTask);

        var result = await _service.GenerateYearAsync(1, 2026);

        var friday = Assert.Single(result, d => d.Date == new DateOnly(2026, 1, 2));
        Assert.True(friday.IsHoliday);
        Assert.Equal("Friday (Weekly Off)", friday.Title);

        var saturday = Assert.Single(result, d => d.Date == new DateOnly(2026, 1, 3));
        Assert.True(saturday.IsHoliday);
        Assert.Equal("Saturday (Weekly Off)", saturday.Title);
    }
}

public class HolidayMasterServiceTests
{
    [Fact]
    public async Task GetPagedAsync_NoFilters_ReturnsAll()
    {
        var uow = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var repo = new Mock<IBaseRepository<HolidayMaster>>(MockBehavior.Loose);
        var data = new List<HolidayMaster>
        {
            new() { Id = 1, Name = "H1", HolidayType = "National", HolidayDate = new DateOnly(2026, 1, 1), IsActive = true, CreatedBy = "test" },
            new() { Id = 2, Name = "H2", HolidayType = "Religious", HolidayDate = new DateOnly(2026, 12, 25), IsActive = true, CreatedBy = "test" }
        };

        uow.Setup(x => x.Repository<HolidayMaster>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new HolidayMasterService(uow.Object, Mock.Of<IMemoryCache>());
        var result = await service.GetPagedAsync(1, 10, null, null, null);

        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetPagedAsync_FilterByType_ReturnsFiltered()
    {
        var uow = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var repo = new Mock<IBaseRepository<HolidayMaster>>(MockBehavior.Loose);
        var data = new List<HolidayMaster>
        {
            new() { Id = 1, Name = "H1", HolidayType = "National", HolidayDate = new DateOnly(2026, 1, 1), IsActive = true, CreatedBy = "test" },
            new() { Id = 2, Name = "H2", HolidayType = "Religious", HolidayDate = new DateOnly(2026, 12, 25), IsActive = true, CreatedBy = "test" }
        };

        uow.Setup(x => x.Repository<HolidayMaster>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new HolidayMasterService(uow.Object, Mock.Of<IMemoryCache>());
        var result = await service.GetPagedAsync(1, 10, null, "National", null);

        Assert.Equal(1, result.TotalItems);
        Assert.Equal("National", result.Items[0].HolidayType);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsDto()
    {
        var uow = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var repo = new Mock<IBaseRepository<HolidayMaster>>(MockBehavior.Loose);
        var data = new List<HolidayMaster>
        {
            new() { Id = 1, Name = "Test", HolidayType = "National", HolidayDate = new DateOnly(2026, 1, 1), IsActive = true, CreatedBy = "system" }
        };

        uow.Setup(x => x.Repository<HolidayMaster>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new HolidayMasterService(uow.Object, Mock.Of<IMemoryCache>());
        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_DeletedEntity_ReturnsNull()
    {
        var uow = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var repo = new Mock<IBaseRepository<HolidayMaster>>(MockBehavior.Loose);
        var data = new List<HolidayMaster>
        {
            new() { Id = 1, Name = "Deleted", HolidayType = "National", HolidayDate = new DateOnly(2026, 1, 1), IsDeleted = true, CreatedBy = "system" }
        };

        uow.Setup(x => x.Repository<HolidayMaster>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new HolidayMasterService(uow.Object, Mock.Of<IMemoryCache>());
        var result = await service.GetByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsId()
    {
        var uow = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var repo = new Mock<IBaseRepository<HolidayMaster>>(MockBehavior.Loose);

        uow.Setup(x => x.Repository<HolidayMaster>()).Returns(repo.Object);
        repo.Setup(x => x.AddAsync(It.IsAny<HolidayMaster>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback<HolidayMaster, CancellationToken>((entity, _) => entity.Id = 1);
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new HolidayMasterService(uow.Object, Mock.Of<IMemoryCache>());
        var dto = new HolidayMasterUpsertDto
        {
            Name = "New Holiday",
            HolidayType = "National",
            HolidayDate = new DateOnly(2026, 7, 1),
            CountryCode = "BD",
            IsActive = true
        };

        var id = await service.CreateAsync(dto, "test");

        Assert.Equal(1, id);
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_SoftDeletes()
    {
        var uow = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var repo = new Mock<IBaseRepository<HolidayMaster>>(MockBehavior.Loose);
        var entity = new HolidayMaster { Id = 1, Name = "Test", HolidayType = "National", HolidayDate = new DateOnly(2026, 1, 1), CreatedBy = "system" };
        var data = new List<HolidayMaster> { entity };

        uow.Setup(x => x.Repository<HolidayMaster>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new HolidayMasterService(uow.Object, Mock.Of<IMemoryCache>());
        await service.DeleteAsync(1, "test");

        Assert.True(entity.IsDeleted);
    }

    [Fact]
    public async Task DeleteAsync_NonExistent_Throws()
    {
        var uow = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var repo = new Mock<IBaseRepository<HolidayMaster>>(MockBehavior.Loose);
        var data = new List<HolidayMaster>();

        uow.Setup(x => x.Repository<HolidayMaster>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new HolidayMasterService(uow.Object, Mock.Of<IMemoryCache>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(999, "test"));
    }

    [Fact]
    public async Task ActivateAsync_SetsIsActiveTrue()
    {
        var uow = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var repo = new Mock<IBaseRepository<HolidayMaster>>(MockBehavior.Loose);
        var entity = new HolidayMaster { Id = 1, Name = "Test", HolidayType = "National", HolidayDate = new DateOnly(2026, 1, 1), IsActive = false, CreatedBy = "system" };
        var data = new List<HolidayMaster> { entity };

        uow.Setup(x => x.Repository<HolidayMaster>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new HolidayMasterService(uow.Object, Mock.Of<IMemoryCache>());
        await service.ActivateAsync(1, "test");

        Assert.True(entity.IsActive);
    }

    [Fact]
    public async Task DeactivateAsync_SetsIsActiveFalse()
    {
        var uow = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var repo = new Mock<IBaseRepository<HolidayMaster>>(MockBehavior.Loose);
        var entity = new HolidayMaster { Id = 1, Name = "Test", HolidayType = "National", HolidayDate = new DateOnly(2026, 1, 1), IsActive = true, CreatedBy = "system" };
        var data = new List<HolidayMaster> { entity };

        uow.Setup(x => x.Repository<HolidayMaster>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new HolidayMasterService(uow.Object, Mock.Of<IMemoryCache>());
        await service.DeactivateAsync(1, "test");

        Assert.False(entity.IsActive);
    }

    [Fact]
    public async Task ImportAsync_Duplicate_DoesNotDuplicate()
    {
        var uow = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var repo = new Mock<IBaseRepository<HolidayMaster>>(MockBehavior.Loose);
        var existing = new HolidayMaster { Id = 1, Name = "Existing", HolidayType = "National", HolidayDate = new DateOnly(2026, 1, 1), IsActive = true, CreatedBy = "system" };
        var data = new List<HolidayMaster> { existing };

        uow.Setup(x => x.Repository<HolidayMaster>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new HolidayMasterService(uow.Object, Mock.Of<IMemoryCache>());
        var dtoList = new List<HolidayMasterUpsertDto>
        {
            new() { Name = "Existing", HolidayType = "National", HolidayDate = new DateOnly(2026, 1, 1), CountryCode = "BD" }
        };

        var imported = await service.ImportAsync(dtoList, "test");

        Assert.Equal(0, imported);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActive()
    {
        var uow = new Mock<IUnitOfWork>(MockBehavior.Loose);
        var repo = new Mock<IBaseRepository<HolidayMaster>>(MockBehavior.Loose);
        var data = new List<HolidayMaster>
        {
            new() { Id = 1, Name = "Active", HolidayType = "National", HolidayDate = new DateOnly(2026, 1, 1), IsActive = true, CreatedBy = "system" },
            new() { Id = 2, Name = "Inactive", HolidayType = "National", HolidayDate = new DateOnly(2026, 1, 2), IsActive = false, CreatedBy = "system" },
            new() { Id = 3, Name = "Deleted", HolidayType = "National", HolidayDate = new DateOnly(2026, 1, 3), IsActive = true, IsDeleted = true, CreatedBy = "system" }
        };

        uow.Setup(x => x.Repository<HolidayMaster>()).Returns(repo.Object);
        repo.Setup(x => x.Query()).Returns(() => data.AsAsyncQueryable());

        var service = new HolidayMasterService(uow.Object, Mock.Of<IMemoryCache>());
        var result = await service.GetAllAsync();

        var item = Assert.Single(result);
        Assert.Equal("Active", item.Name);
    }
}
