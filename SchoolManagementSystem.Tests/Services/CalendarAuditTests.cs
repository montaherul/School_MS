using Microsoft.AspNetCore.Http;
using Moq;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class CalendarAuditTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<AuditLog>> _repoMock = new(MockBehavior.Loose);
    private readonly Mock<IHttpContextAccessor> _httpMock = new(MockBehavior.Loose);

    private ICalendarAuditService CreateService()
    {
        _httpMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        return new CalendarAuditService(_uowMock.Object, _httpMock.Object);
    }

    [Fact]
    public async Task LogAsync_CreatesAuditLogEntry()
    {
        _uowMock.Setup(x => x.Repository<AuditLog>()).Returns(_repoMock.Object);
        _repoMock.Setup(x => x.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();
        await service.LogAsync("Created", "CalendarEntry", 1, null, "New entry");

        _repoMock.Verify(x => x.AddAsync(It.Is<AuditLog>(a =>
            a.Module == "Calendar" &&
            a.Action == "CalendarEntry.Created" &&
            a.UserId == null), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogAsync_SetsCorrectModule()
    {
        _uowMock.Setup(x => x.Repository<AuditLog>()).Returns(_repoMock.Object);
        _repoMock.Setup(x => x.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();
        await service.LogAsync("Generated", "Holiday", null, null, "14 holidays generated");

        _repoMock.Verify(x => x.AddAsync(It.Is<AuditLog>(a =>
            a.Module == "Calendar"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogAsync_WithOldAndNewValues()
    {
        _uowMock.Setup(x => x.Repository<AuditLog>()).Returns(_repoMock.Object);
        _repoMock.Setup(x => x.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();
        await service.LogAsync("Updated", "CalendarEntry", 5, "IsHoliday:false", "IsHoliday:true");

        _repoMock.Verify(x => x.AddAsync(It.Is<AuditLog>(a =>
            a.Details != null &&
            a.Details.Contains("Old:") &&
            a.Details.Contains("New:")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogAsync_TruncatesLongDetails()
    {
        _uowMock.Setup(x => x.Repository<AuditLog>()).Returns(_repoMock.Object);
        _repoMock.Setup(x => x.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var longValue = new string('X', 2000);
        var service = CreateService();
        await service.LogAsync("Exported", "Calendar", null, null, longValue);

        _repoMock.Verify(x => x.AddAsync(It.Is<AuditLog>(a =>
            a.Details != null && a.Details.Length <= 1000), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogAsync_WithUser_SetsUserId()
    {
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "42"),
            new Claim(ClaimTypes.Name, "testuser")
        });
        httpContext.User = new ClaimsPrincipal(identity);
        _httpMock.Setup(x => x.HttpContext).Returns(httpContext);
        _uowMock.Setup(x => x.Repository<AuditLog>()).Returns(_repoMock.Object);
        _repoMock.Setup(x => x.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new CalendarAuditService(_uowMock.Object, _httpMock.Object);
        await service.LogAsync("Deleted", "CalendarEntry", 42, null, null);

        _repoMock.Verify(x => x.AddAsync(It.Is<AuditLog>(a =>
            a.UserId == 42 &&
            a.CreatedBy == "testuser"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogAsync_WithoutEntityId()
    {
        _uowMock.Setup(x => x.Repository<AuditLog>()).Returns(_repoMock.Object);
        _repoMock.Setup(x => x.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();
        await service.LogAsync("Repaired", "Calendar", null, null, "Repaired missing dates");

        _repoMock.Verify(x => x.AddAsync(It.Is<AuditLog>(a =>
            a.Details != null &&
            a.Details.Contains("[Calendar]")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogAsync_SavesToRepository()
    {
        _uowMock.Setup(x => x.Repository<AuditLog>()).Returns(_repoMock.Object);
        _repoMock.Setup(x => x.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();
        await service.LogAsync("Synced", "ExamDay", null, null, "Synced exam schedules");

        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
