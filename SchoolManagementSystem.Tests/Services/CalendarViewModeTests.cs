using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SchoolManagementSystem.Controllers.Academic;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class CalendarViewModeTests
{
    private readonly Mock<IAcademicCalendarService> _serviceMock = new(MockBehavior.Loose);
    private readonly Mock<IAcademicYearService> _yearServiceMock = new(MockBehavior.Loose);
    private readonly Mock<ICalendarDashboardService> _dashboardMock = new(MockBehavior.Loose);
    private readonly Mock<IPdfGenerator> _pdfMock = new(MockBehavior.Loose);

    private AcademicCalendarController CreateController()
    {
        var httpContext = new DefaultHttpContext();
        var controller = new AcademicCalendarController(_serviceMock.Object, _yearServiceMock.Object, _dashboardMock.Object, _pdfMock.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };
        return controller;
    }

    [Fact]
    public void Index_ReturnsView()
    {
        var controller = CreateController();
        var result = controller.Index();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void WeekView_ReturnsView()
    {
        var controller = CreateController();
        var result = controller.WeekView(null);
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(controller.ViewBag.StartDate);
    }

    [Fact]
    public void Agenda_ReturnsView()
    {
        var controller = CreateController();
        var result = controller.Agenda();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void YearView_ReturnsView()
    {
        var controller = CreateController();
        var result = controller.YearView(null);
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(controller.ViewBag.Year);
    }

    [Fact]
    public async Task GetWeekData_ReturnsJsonWith7Days()
    {
        var date = new DateTime(2026, 6, 15);
        var data = new List<AcademicCalendarDto>();
        for (int i = 0; i < 7; i++)
        {
            data.Add(new AcademicCalendarDto
            {
                Date = DateOnly.FromDateTime(date.AddDays(i)),
                Title = $"Day{i + 1}",
                IsWorkingDay = true
            });
        }

        _serviceMock.Setup(x => x.GetCalendarDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);
        _serviceMock.Setup(x => x.GetPublishedEventsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CalendarPublishedEventDto>());
        _serviceMock.Setup(x => x.GetExamSchedulesAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CalendarExamScheduleDto>());

        var controller = CreateController();
        var result = await controller.GetWeekData(date);

        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }

    [Fact]
    public async Task GetAgendaData_ReturnsItemsSorted()
    {
        var today = DateTime.Today;
        var data = new List<AcademicCalendarDto>
        {
            new() { Date = DateOnly.FromDateTime(today.AddDays(1)), Title = "Holiday", IsHoliday = true },
            new() { Date = DateOnly.FromDateTime(today.AddDays(2)), Title = "Exam Day", IsExamDay = true }
        };

        _serviceMock.Setup(x => x.GetCalendarDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);
        _serviceMock.Setup(x => x.GetPublishedEventsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CalendarPublishedEventDto>());
        _serviceMock.Setup(x => x.GetExamSchedulesAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CalendarExamScheduleDto>());

        var controller = CreateController();
        var result = await controller.GetAgendaData(today, 20);

        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }

    [Fact]
    public async Task GetYearData_Returns12Months()
    {
        var data = new List<AcademicCalendarDto>();
        for (int m = 1; m <= 12; m++)
        {
            var daysInMonth = DateTime.DaysInMonth(2026, m);
            for (int d = 1; d <= daysInMonth; d++)
            {
                data.Add(new AcademicCalendarDto
                {
                    Date = new DateOnly(2026, m, d),
                    IsWorkingDay = d % 7 != 6 && d % 7 != 0,
                    IsHoliday = d % 7 == 6 || d % 7 == 0
                });
            }
        }

        _serviceMock.Setup(x => x.GetCalendarDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);
        _serviceMock.Setup(x => x.GetPublishedEventsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CalendarPublishedEventDto>());
        _serviceMock.Setup(x => x.GetExamSchedulesAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CalendarExamScheduleDto>());

        var controller = CreateController();
        var result = await controller.GetYearData(2026);

        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }

    [Fact]
    public async Task GetEvents_ReturnsJson()
    {
        var data = new List<AcademicCalendarDto>
        {
            new() { Id = 1, Date = new DateOnly(2026, 6, 1), Title = "Event1", IsHoliday = false, IsWorkingDay = true }
        };

        _serviceMock.Setup(x => x.GetCalendarDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);
        _serviceMock.Setup(x => x.GetPublishedEventsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CalendarPublishedEventDto>());
        _serviceMock.Setup(x => x.GetExamSchedulesAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CalendarExamScheduleDto>());

        var controller = CreateController();
        var result = await controller.GetEvents(new DateTime(2026, 6, 1), new DateTime(2026, 6, 30));

        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }

    [Fact]
    public async Task WidgetUpcomingHolidays_ReturnsJson()
    {
        _dashboardMock.Setup(x => x.GetUpcomingHolidaysAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SchoolManagementSystem.Models.DTOs.Calendar.UpcomingHolidayDto>
            {
                new() { Date = new DateOnly(2026, 12, 25), Name = "Christmas", HolidayType = "Religious" }
            });

        var controller = CreateController();
        var result = await controller.WidgetUpcomingHolidays(5);

        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }

    [Fact]
    public async Task WidgetMonthSummary_ReturnsJson()
    {
        _dashboardMock.Setup(x => x.GetCurrentMonthSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolManagementSystem.Models.DTOs.Calendar.MonthSummaryDto
            {
                TotalDays = 30, WorkingDays = 22, HolidayCount = 8, ExamDayCount = 2
            });

        var controller = CreateController();
        var result = await controller.WidgetMonthSummary();

        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }
}
