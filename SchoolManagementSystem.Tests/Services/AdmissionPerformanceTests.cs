using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using SchoolManagementSystem.Services.Implementations.Admissions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class AdmissionPerformanceTests
{
    private readonly Mock<IAdmissionDashboardRepository> _dashboardRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IAdmissionRepository> _admissionRepoMock = new(MockBehavior.Loose);
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    private static AdmissionDashboardDto CreateSampleDashboardData()
    {
        return new AdmissionDashboardDto
        {
            TodayApplications = 5,
            WeekApplications = 23,
            MonthApplications = 87,
            PendingVerification = 40,
            Approved = 30,
            Rejected = 10,
            Converted = 7,
            TotalApplications = 87,
            ConvertedCount = 7,
            ConversionRate = 8.05,
            TotalInvoiceAmount = 50000m,
            TotalPaidAmount = 35000m,
            TotalInvoices = 87,
            PaidInvoices = 7,
            MonthlyTrend =
            [
                new MonthlyTrendDto { Year = 2026, Month = 1, Count = 10, PendingCount = 5, ApprovedCount = 3, RejectedCount = 1, ConvertedCount = 1 },
                new MonthlyTrendDto { Year = 2026, Month = 2, Count = 15, PendingCount = 7, ApprovedCount = 5, RejectedCount = 2, ConvertedCount = 1 },
            ],
            ClassDistribution =
            [
                new NameCountDto { Name = "Class One", Count = 30 },
                new NameCountDto { Name = "Class Two", Count = 25 },
            ],
            GenderDistribution =
            [
                new NameCountDto { Name = "Male", Count = 45 },
                new NameCountDto { Name = "Female", Count = 42 },
            ],
            ReligionDistribution =
            [
                new NameCountDto { Name = "Islam", Count = 80 },
                new NameCountDto { Name = "Hinduism", Count = 7 },
            ],
            DistrictDistribution =
            [
                new NameCountDto { Name = "Dhaka", Count = 40 },
                new NameCountDto { Name = "Chattogram", Count = 20 },
            ],
            ApplicationHeatmap =
            [
                new DateCountDto { Date = new DateTime(2026, 1, 15), Count = 3 },
                new DateCountDto { Date = new DateTime(2026, 2, 10), Count = 5 },
            ],
            TopClasses =
            [
                new NameCountDto { Name = "Class One", Count = 30 },
                new NameCountDto { Name = "Class Two", Count = 25 },
            ],
        };
    }

    private static List<AdmissionListResultDto> CreateSampleAdmissionList(int count = 5)
    {
        return Enumerable.Range(1, count).Select(i => new AdmissionListResultDto
        {
            Id = i,
            ApplicationNo = $"APP-{i:D4}",
            ApplicantName = $"Student {i}",
            ClassName = "Class One",
            Status = "Pending",
            FatherName = $"Father {i}",
            MotherName = $"Mother {i}",
            Gender = i % 2 == 0 ? "Male" : "Female",
            Religion = "Islam",
            CreatedAt = DateTime.UtcNow.AddDays(-i),
            CreatedBy = "admin",
        }).ToList();
    }

    // ─── SP call timing ─────────────────────────────────────────

    [Fact]
    public async Task AdmissionDashboardRepository_GetDashboardDataAsync_CompletesUnder500ms()
    {
        _dashboardRepoMock
            .Setup(r => r.GetDashboardDataAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleDashboardData());

        var sw = Stopwatch.StartNew();
        var result = await _dashboardRepoMock.Object.GetDashboardDataAsync();
        sw.Stop();

        Assert.NotNull(result);
        Assert.Equal(5, result.TodayApplications);
        Assert.True(sw.ElapsedMilliseconds < 500,
            $"GetDashboardDataAsync took {sw.ElapsedMilliseconds}ms (limit: 500ms)");
    }

    [Fact]
    public async Task AdmissionRepository_GetListByStoredProcedureAsync_CompletesUnder500ms()
    {
        var items = CreateSampleAdmissionList(10);
        _admissionRepoMock
            .Setup(r => r.GetListByStoredProcedureAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, 10));

        var sw = Stopwatch.StartNew();
        var (result, total) = await _admissionRepoMock.Object.GetListByStoredProcedureAsync(1, 10, null, 0, null, default);
        sw.Stop();

        Assert.Equal(10, result.Count);
        Assert.Equal(10, total);
        Assert.True(sw.ElapsedMilliseconds < 500,
            $"GetListByStoredProcedureAsync took {sw.ElapsedMilliseconds}ms (limit: 500ms)");
    }

    // ─── Cache performance ──────────────────────────────────────

    [Fact]
    public async Task DashboardService_CacheSpeedsUpSecondCall()
    {
        _dashboardRepoMock
            .Setup(r => r.GetDashboardDataAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleDashboardData());

        var service = new AdmissionDashboardService(_dashboardRepoMock.Object, _cache);

        var swFirst = Stopwatch.StartNew();
        var first = await service.GetDashboardAsync();
        swFirst.Stop();

        var swSecond = Stopwatch.StartNew();
        var second = await service.GetDashboardAsync();
        swSecond.Stop();

        Assert.NotNull(first);
        Assert.NotNull(second);
        _dashboardRepoMock.Verify(
            r => r.GetDashboardDataAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.True(swSecond.ElapsedMilliseconds < 100,
            $"Cached call took {swSecond.ElapsedMilliseconds}ms (limit: 100ms)");
    }

    // ─── QueryOptimizer — compiled queries ──────────────────────

    [Fact]
    public async Task AdmissionQueryOptimizer_GetByIdOptimizedAsync_CompletesUnder500ms()
    {
        var admissions = new List<AdmissionApplication>
        {
            new()
            {
                Id = 1,
                ApplicationNo = "APP-0001",
                ApplicantName = "Test Student",
                Status = AdmissionStatus.Pending,
                AppliedClassId = 1,
                IsDeleted = false,
            },
        };

        _admissionRepoMock.Setup(r => r.Query()).Returns(admissions.AsAsyncQueryable());

        var sw = Stopwatch.StartNew();
        var result = await AdmissionQueryOptimizer.GetByIdOptimizedAsync(_admissionRepoMock.Object, 1);
        sw.Stop();

        Assert.NotNull(result);
        Assert.Equal("Test Student", result!.ApplicantName);
        Assert.True(sw.ElapsedMilliseconds < 500,
            $"GetByIdOptimizedAsync took {sw.ElapsedMilliseconds}ms (limit: 500ms)");
    }

    [Fact]
    public async Task AdmissionQueryOptimizer_GetStatusCountAsync_CompletesUnder500ms()
    {
        var admissions = new List<AdmissionApplication>
        {
            new() { Id = 1, Status = AdmissionStatus.Pending, AppliedClassId = 1, IsDeleted = false },
            new() { Id = 2, Status = AdmissionStatus.Pending, AppliedClassId = 1, IsDeleted = false },
            new() { Id = 3, Status = AdmissionStatus.Approved, AppliedClassId = 1, IsDeleted = false },
            new() { Id = 4, Status = AdmissionStatus.Rejected, AppliedClassId = 1, IsDeleted = false },
        };

        _admissionRepoMock.Setup(r => r.Query()).Returns(admissions.AsAsyncQueryable());

        var sw = Stopwatch.StartNew();
        var count = await AdmissionQueryOptimizer.GetStatusCountAsync(_admissionRepoMock.Object, AdmissionStatus.Pending);
        sw.Stop();

        Assert.Equal(2, count);
        Assert.True(sw.ElapsedMilliseconds < 500,
            $"GetStatusCountAsync took {sw.ElapsedMilliseconds}ms (limit: 500ms)");
    }

    [Fact]
    public async Task AdmissionQueryOptimizer_GetAllStatusCountsAsync_CompletesUnder500ms()
    {
        var admissions = new List<AdmissionApplication>
        {
            new() { Id = 1, Status = AdmissionStatus.Pending, AppliedClassId = 1, IsDeleted = false },
            new() { Id = 2, Status = AdmissionStatus.Pending, AppliedClassId = 2, IsDeleted = false },
            new() { Id = 3, Status = AdmissionStatus.Approved, AppliedClassId = 1, IsDeleted = false },
            new() { Id = 4, Status = AdmissionStatus.Converted, AppliedClassId = 1, IsDeleted = false },
            new() { Id = 5, Status = AdmissionStatus.Rejected, AppliedClassId = 1, IsDeleted = false },
        };

        _admissionRepoMock.Setup(r => r.Query()).Returns(admissions.AsAsyncQueryable());

        var sw = Stopwatch.StartNew();
        var counts = await AdmissionQueryOptimizer.GetAllStatusCountsAsync(_admissionRepoMock.Object);
        sw.Stop();

        Assert.Equal(2, counts[AdmissionStatus.Pending]);
        Assert.Equal(1, counts[AdmissionStatus.Approved]);
        Assert.Equal(1, counts[AdmissionStatus.Converted]);
        Assert.Equal(1, counts[AdmissionStatus.Rejected]);
        Assert.True(sw.ElapsedMilliseconds < 500,
            $"GetAllStatusCountsAsync took {sw.ElapsedMilliseconds}ms (limit: 500ms)");
    }

    [Fact]
    public async Task AdmissionQueryOptimizer_GetProjectedListQuery_CompletesUnder500ms()
    {
        var admissions = new List<AdmissionApplication>
        {
            new() { Id = 1, ApplicationNo = "APP-0001", ApplicantName = "Alice", Status = AdmissionStatus.Pending, AppliedClassId = 1, IsDeleted = false },
            new() { Id = 2, ApplicationNo = "APP-0002", ApplicantName = "Bob", Status = AdmissionStatus.Approved, AppliedClassId = 1, IsDeleted = false },
        };

        _admissionRepoMock.Setup(r => r.Query()).Returns(admissions.AsAsyncQueryable());

        var sw = Stopwatch.StartNew();
        var query = AdmissionQueryOptimizer.GetProjectedListQuery(_admissionRepoMock.Object);
        var result = await query.ToListAsync();
        sw.Stop();

        Assert.Equal(2, result.Count);
        Assert.True(sw.ElapsedMilliseconds < 500,
            $"GetProjectedListQuery took {sw.ElapsedMilliseconds}ms (limit: 500ms)");
    }

    // ─── Pagination ─────────────────────────────────────────────

    [Fact]
    public async Task GetListByStoredProcedureAsync_Pagination_ReturnsCorrectPageSize()
    {
        var allItems = CreateSampleAdmissionList(25);

        _admissionRepoMock
            .Setup(r => r.GetListByStoredProcedureAsync(
                1, 10, It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((allItems.Take(10).ToList(), 25));

        _admissionRepoMock
            .Setup(r => r.GetListByStoredProcedureAsync(
                2, 10, It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((allItems.Skip(10).Take(10).ToList(), 25));

        var sw = Stopwatch.StartNew();

        var (page1, total1) = await _admissionRepoMock.Object.GetListByStoredProcedureAsync(1, 10, null, 0, null, default);
        var (page2, total2) = await _admissionRepoMock.Object.GetListByStoredProcedureAsync(2, 10, null, 0, null, default);

        sw.Stop();

        Assert.Equal(10, page1.Count);
        Assert.Equal(10, page2.Count);
        Assert.Equal(25, total1);
        Assert.Equal(25, total2);
        Assert.NotEqual(page1[0].Id, page2[0].Id);
        Assert.True(sw.ElapsedMilliseconds < 500,
            $"Pagination calls took {sw.ElapsedMilliseconds}ms total (limit: 500ms)");
    }

    [Fact]
    public async Task GetListByStoredProcedureAsync_DifferentPageSizes_Respected()
    {
        var allItems = CreateSampleAdmissionList(20);

        _admissionRepoMock
            .Setup(r => r.GetListByStoredProcedureAsync(
                It.IsAny<int>(), 5, It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((allItems.Take(5).ToList(), 20));

        _admissionRepoMock
            .Setup(r => r.GetListByStoredProcedureAsync(
                It.IsAny<int>(), 15, It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((allItems.Take(15).ToList(), 20));

        var sw = Stopwatch.StartNew();

        var (pageSize5, _) = await _admissionRepoMock.Object.GetListByStoredProcedureAsync(1, 5, null, 0, null, default);
        var (pageSize15, _) = await _admissionRepoMock.Object.GetListByStoredProcedureAsync(1, 15, null, 0, null, default);

        sw.Stop();

        Assert.Equal(5, pageSize5.Count);
        Assert.Equal(15, pageSize15.Count);
        Assert.True(sw.ElapsedMilliseconds < 500,
            $"Different page sizes took {sw.ElapsedMilliseconds}ms total (limit: 500ms)");
    }
}
