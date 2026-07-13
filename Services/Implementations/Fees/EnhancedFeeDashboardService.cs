using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class EnhancedFeeDashboardService : IEnhancedFeeDashboardService
{
    private readonly IFeeDashboardRepository _dashboardRepo;
    private readonly IFeeReportRepository _reportRepo;
    private readonly IUnitOfWork _unitOfWork;

    public EnhancedFeeDashboardService(
        IFeeDashboardRepository dashboardRepo,
        IFeeReportRepository reportRepo,
        IUnitOfWork unitOfWork)
    {
        _dashboardRepo = dashboardRepo;
        _reportRepo = reportRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<EnhancedFeeDashboardDto> GetDashboardAsync(int? academicYearId = null)
    {
        var baseData = await _dashboardRepo.GetDashboardDataAsync(academicYearId, default);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1);

        var todayPayments = await _unitOfWork.Repository<Payment>().Query()
            .Where(p => !p.IsDeleted && p.PaidAt >= todayStart && p.PaidAt < todayEnd)
            .ToListAsync();

        var todayCollection = todayPayments.Sum(p => p.Amount + p.LateFee);

        var cashBook = await _reportRepo.GetCashBookAsync(today, today, academicYearId);

        var invoices = await _unitOfWork.Repository<FeeInvoice>().Query()
            .Where(i => !i.IsDeleted && (academicYearId == null || i.AcademicYearId == academicYearId))
            .ToListAsync();

        var dueStudentCount = invoices
            .Where(i => i.Status == PaymentStatus.Draft || i.Status == PaymentStatus.Issued || i.Status == PaymentStatus.Partial)
            .Select(i => i.StudentId)
            .Distinct()
            .Count();

        var pendingInvoiceCount = invoices.Count(i => i.Status == PaymentStatus.Draft || i.Status == PaymentStatus.Issued || i.Status == PaymentStatus.Partial);

        var lateFeeOutstanding = invoices.Where(i => i.LateFee > 0 && i.PaidAmount < i.TotalAmount).Sum(i => i.LateFee);

        var waivers = await _unitOfWork.Repository<FeeWaiver>().Query()
            .Where(w => !w.IsDeleted && w.IsApproved)
            .ToListAsync();

        var classSummary = await _reportRepo.GetClassCollectionSummaryAsync(academicYearId ?? 0, 1, 100);

        var totalLateFeeCollected = await _unitOfWork.Repository<Payment>().Query()
            .Where(p => !p.IsDeleted)
            .SumAsync(p => p.LateFee);

        var result = new EnhancedFeeDashboardDto
        {
            TotalAssigned = baseData.TotalAssigned,
            TotalCollected = baseData.TotalCollected,
            TotalOutstanding = baseData.TotalOutstanding,
            TotalDiscounted = baseData.TotalDiscounted,
            TotalInvoices = baseData.TotalInvoices,
            TotalPayments = baseData.TotalPayments,
            OverdueInvoices = baseData.OverdueInvoices,
            CollectionRate = baseData.CollectionRate,
            TotalWaived = waivers.Sum(w => w.WaiverAmount),
            TotalLateFeeCollected = totalLateFeeCollected,
            TodayCollection = todayCollection,
            TodayPaymentCount = todayPayments.Count,
            LateFeeOutstanding = lateFeeOutstanding,
            DueStudentCount = dueStudentCount,
            CashBalance = cashBook.ClosingBalance,
            ScholarshipAmount = waivers.Sum(w => w.WaiverAmount),
            ScholarshipCount = waivers.Count,
            PendingInvoiceCount = pendingInvoiceCount,
            ClassCollections = classSummary.items.Select(c => new ClassCollectionSummary
            {
                ClassName = c.ClassName,
                Assigned = c.TotalAssigned,
                Collected = c.TotalCollected,
                Due = c.TotalDue,
                Rate = c.CollectionRate
            }).ToList(),
            DueSoonInvoices = baseData.DueSoonInvoices,
            MonthlyTrend = baseData.MonthlyCollections,
            MonthlyCollections = baseData.MonthlyCollections,
            PaymentMethodBreakdown = baseData.PaymentMethodBreakdown
        };

        return result;
    }
}
