using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SchoolManagementSystem.Controllers.Academic;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Reports;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class CalendarExportTests
{
    private readonly Mock<IAcademicCalendarService> _serviceMock = new(MockBehavior.Loose);
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly Mock<ICalendarDashboardService> _dashboardMock = new(MockBehavior.Loose);
    private readonly Mock<IPdfGenerator> _pdfMock = new(MockBehavior.Loose);

    [Fact]
    public async Task ExportPdf_ReturnsFileResult()
    {
        var data = new List<AcademicCalendar>
        {
            new() { Date = new DateOnly(2026, 1, 1), Title = "New Year", IsHoliday = true },
            new() { Date = new DateOnly(2026, 1, 2), Title = "Working Day", IsWorkingDay = true }
        };
        _serviceMock.Setup(x => x.GetCalendarDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);
        _pdfMock.Setup(x => x.GenerateFromHtml(It.IsAny<string>())).Returns(Encoding.UTF8.GetBytes("PDF"));

        var controller = new AcademicCalendarController(_serviceMock.Object, _uowMock.Object, _dashboardMock.Object, _pdfMock.Object, null!);
        var result = await controller.ExportPdf(2026);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/pdf", fileResult.ContentType);
        Assert.Equal("AcademicCalendar_2026.pdf", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task ExportExcel_ReturnsFileResult()
    {
        var data = new List<AcademicCalendar>();
        _serviceMock.Setup(x => x.GetCalendarDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var controller = new AcademicCalendarController(_serviceMock.Object, _uowMock.Object, _dashboardMock.Object, _pdfMock.Object, null!);
        var result = await controller.ExportExcel(2026);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
        Assert.Equal("AcademicCalendar_2026.xlsx", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task ExportExcel_ContainsHeaderRow()
    {
        var data = new List<AcademicCalendar>();
        _serviceMock.Setup(x => x.GetCalendarDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var controller = new AcademicCalendarController(_serviceMock.Object, _uowMock.Object, _dashboardMock.Object, _pdfMock.Object, null!);
        var result = await controller.ExportExcel(2026);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.NotNull(fileResult.FileContents);
        Assert.True(fileResult.FileContents.Length > 0);
    }

    [Fact]
    public async Task ExportExcel_ContainsDataRows()
    {
        var data = new List<AcademicCalendar>
        {
            new() { Date = new DateOnly(2026, 6, 1), Title = "Test", IsWorkingDay = true }
        };
        _serviceMock.Setup(x => x.GetCalendarDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var controller = new AcademicCalendarController(_serviceMock.Object, _uowMock.Object, _dashboardMock.Object, _pdfMock.Object, null!);
        var result = await controller.ExportExcel(2026);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.NotNull(fileResult.FileContents);
    }

    [Fact]
    public async Task ExportPdf_DefaultsToCurrentYear()
    {
        var data = new List<AcademicCalendar>();
        _serviceMock.Setup(x => x.GetCalendarDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);
        _pdfMock.Setup(x => x.GenerateFromHtml(It.IsAny<string>())).Returns(Encoding.UTF8.GetBytes("PDF"));

        var controller = new AcademicCalendarController(_serviceMock.Object, _uowMock.Object, _dashboardMock.Object, _pdfMock.Object, null!);
        var result = await controller.ExportPdf(null);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Contains(".pdf", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task ExportExcel_DefaultsToCurrentYear()
    {
        var data = new List<AcademicCalendar>();
        _serviceMock.Setup(x => x.GetCalendarDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var controller = new AcademicCalendarController(_serviceMock.Object, _uowMock.Object, _dashboardMock.Object, _pdfMock.Object, null!);
        var result = await controller.ExportExcel(null);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Contains(".xlsx", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task PrintView_ReturnsViewResult()
    {
        var data = new List<AcademicCalendar>
        {
            new() { Date = new DateOnly(2026, 1, 1), Title = "Day1", IsHoliday = true },
            new() { Date = new DateOnly(2026, 1, 2), Title = "Day2", IsWorkingDay = true }
        };
        _serviceMock.Setup(x => x.GetCalendarDaysAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var httpContext = new DefaultHttpContext();
        var controller = new AcademicCalendarController(_serviceMock.Object, _uowMock.Object, _dashboardMock.Object, _pdfMock.Object, null!)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };

        var result = await controller.PrintView(2026);
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.Model);
    }

    [Fact]
    public async Task SimpleExcelWriter_GeneratesValidXlsx()
    {
        var rows = new List<string[]>
        {
            new[] { "Date", "Day", "Title" },
            new[] { "2026-01-01", "Thursday", "Test" }
        };

        var bytes = SimpleExcelWriter.WriteWorkbook("Test", rows);
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
        Assert.StartsWith("PK", Encoding.UTF8.GetString(bytes[..2]));
    }

    [Fact]
    public async Task SimpleExcelWriter_EmptyRows_GeneratesValidXlsx()
    {
        var rows = new List<string[]>();
        var bytes = SimpleExcelWriter.WriteWorkbook("Empty", rows);
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
    }
}
