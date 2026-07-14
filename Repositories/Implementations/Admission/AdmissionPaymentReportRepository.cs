using System.Data;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Repositories.Interfaces.Admission;

namespace SchoolManagementSystem.Repositories.Implementations.Admission;

public class AdmissionPaymentReportRepository : BaseRepository<object>, IAdmissionPaymentReportRepository
{
    public AdmissionPaymentReportRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<AdmissionDailyCollectionDto> Items, int TotalRecords)> GetDailyCollectionAsync(DateTime date, int page, int pageSize, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetAdmissionDailyCollectionReport";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@CollectionDate", date.Date);
        AddParameter(cmd, "@PageNumber", page);
        AddParameter(cmd, "@PageSize", pageSize);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AdmissionDailyCollectionDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AdmissionDailyCollectionDto
            {
                Id = GetInt32(reader, "Id"),
                ReceiptNo = GetString(reader, "ReceiptNo"),
                AdmissionApplicationId = GetInt32(reader, "AdmissionApplicationId"),
                ApplicationNo = GetString(reader, "ApplicationNo"),
                Amount = GetDecimal(reader, "Amount"),
                PaymentMethod = GetNullableString(reader, "PaymentMethod"),
                GatewayTransactionId = GetNullableString(reader, "GatewayTransactionId"),
                ApplicantName = GetNullableString(reader, "ApplicantName"),
                ReceiptDate = GetDateTime(reader, "ReceiptDate"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }

    public async Task<(List<AdmissionMonthlyCollectionDto> Items, AdmissionMonthlySummaryDto Summary)> GetMonthlyCollectionAsync(int year, int month, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetAdmissionMonthlyCollectionReport";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Year", year);
        AddParameter(cmd, "@Month", month);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AdmissionMonthlyCollectionDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AdmissionMonthlyCollectionDto
            {
                Id = GetInt32(reader, "Id"),
                ReceiptNo = GetString(reader, "ReceiptNo"),
                AdmissionApplicationId = GetInt32(reader, "AdmissionApplicationId"),
                ApplicationNo = GetString(reader, "ApplicationNo"),
                Amount = GetDecimal(reader, "Amount"),
                PaymentMethod = GetNullableString(reader, "PaymentMethod"),
                GatewayTransactionId = GetNullableString(reader, "GatewayTransactionId"),
                ApplicantName = GetNullableString(reader, "ApplicantName"),
                ReceiptDate = GetDateTime(reader, "ReceiptDate")
            });
        }

        var summary = new AdmissionMonthlySummaryDto();
        if (await reader.NextResultAsync(ct) && await reader.ReadAsync(ct))
        {
            summary.TotalCount = GetInt32(reader, "TotalCount");
            summary.TotalCollected = GetDecimal(reader, "TotalCollected");
        }

        return (items, summary);
    }

    public async Task<AdmissionRevenueReportDto> GetRevenueReportAsync(DateTime from, DateTime to, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetAdmissionRevenueReport";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@FromDate", from.Date);
        AddParameter(cmd, "@ToDate", to.Date);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);

        var report = new AdmissionRevenueReportDto();
        if (await reader.ReadAsync(ct))
        {
            report.TotalCollected = GetDecimal(reader, "TotalCollected");
            report.TotalRefunded = GetDecimal(reader, "TotalRefunded");
            report.TotalTransactions = GetInt32(reader, "TotalTransactions");
            report.TotalRefunds = GetInt32(reader, "TotalRefunds");
        }

        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                report.DailyBreakdown.Add(new AdmissionRevenueDailyDto
                {
                    CollectionDate = GetDateTime(reader, "CollectionDate"),
                    DailyTotal = GetDecimal(reader, "DailyTotal"),
                    DailyCount = GetInt32(reader, "DailyCount")
                });
            }
        }

        return report;
    }

    public async Task<(List<AdmissionPaymentRegisterDto> Items, int TotalRecords)> GetPaymentRegisterAsync(DateTime? from, DateTime? to, string? paymentMethod, int page, int pageSize, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetAdmissionPaymentRegister";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@FromDate", from);
        AddParameter(cmd, "@ToDate", to);
        AddParameter(cmd, "@PaymentMethod", paymentMethod);
        AddParameter(cmd, "@PageNumber", page);
        AddParameter(cmd, "@PageSize", pageSize);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AdmissionPaymentRegisterDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AdmissionPaymentRegisterDto
            {
                Id = GetInt32(reader, "Id"),
                ReceiptNo = GetString(reader, "ReceiptNo"),
                AdmissionApplicationId = GetInt32(reader, "AdmissionApplicationId"),
                ApplicationNo = GetString(reader, "ApplicationNo"),
                Amount = GetDecimal(reader, "Amount"),
                PaymentMethod = GetNullableString(reader, "PaymentMethod"),
                GatewayTransactionId = GetNullableString(reader, "GatewayTransactionId"),
                ApplicantName = GetNullableString(reader, "ApplicantName"),
                ReceiptDate = GetDateTime(reader, "ReceiptDate"),
                IsRefunded = GetBoolean(reader, "IsRefunded"),
                RefundAmount = GetNullableDecimal(reader, "RefundAmount"),
                RefundedAt = GetNullableDateTime(reader, "RefundedAt"),
                RefundReason = GetNullableString(reader, "RefundReason"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }

    public async Task<(List<AdmissionRefundReportDto> Items, int TotalRecords)> GetRefundReportAsync(DateTime? from, DateTime? to, int page, int pageSize, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetAdmissionRefundReport";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@FromDate", from);
        AddParameter(cmd, "@ToDate", to);
        AddParameter(cmd, "@PageNumber", page);
        AddParameter(cmd, "@PageSize", pageSize);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AdmissionRefundReportDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AdmissionRefundReportDto
            {
                Id = GetInt32(reader, "Id"),
                ReceiptNo = GetString(reader, "ReceiptNo"),
                AdmissionApplicationId = GetInt32(reader, "AdmissionApplicationId"),
                ApplicationNo = GetString(reader, "ApplicationNo"),
                OriginalAmount = GetDecimal(reader, "OriginalAmount"),
                RefundAmount = GetNullableDecimal(reader, "RefundAmount"),
                RefundedAt = GetNullableDateTime(reader, "RefundedAt"),
                RefundReason = GetNullableString(reader, "RefundReason"),
                RefundedBy = GetNullableString(reader, "RefundedBy"),
                ApplicantName = GetNullableString(reader, "ApplicantName"),
                PaymentMethod = GetNullableString(reader, "PaymentMethod"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }

    public async Task<AdmissionPaymentDashboardDto> GetDashboardAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var todayEnd = todayStart.AddDays(1);
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var yearStart = new DateTime(now.Year, 1, 1);

        var todayReceipts = await _db.AdmissionReceipts
            .Where(r => !r.IsDeleted && !r.IsRefunded
                && r.ReceiptDate >= todayStart && r.ReceiptDate < todayEnd)
            .ToListAsync(ct);

        var monthReceipts = await _db.AdmissionReceipts
            .Where(r => !r.IsDeleted && !r.IsRefunded
                && r.ReceiptDate >= monthStart)
            .ToListAsync(ct);

        var yearReceipts = await _db.AdmissionReceipts
            .Where(r => !r.IsDeleted && !r.IsRefunded
                && r.ReceiptDate >= yearStart)
            .ToListAsync(ct);

        var pending = await _db.Admissions
            .Where(a => !a.IsDeleted && !a.AdmissionFeePaid && a.Status == Models.Enums.AdmissionStatus.Pending)
            .ToListAsync(ct);

        var refunds = await _db.AdmissionReceipts
            .Where(r => !r.IsDeleted && r.IsRefunded && r.RefundedAt >= yearStart)
            .ToListAsync(ct);

        return new AdmissionPaymentDashboardDto
        {
            TodayTotal = todayReceipts.Sum(r => r.Amount),
            TodayCount = todayReceipts.Count,
            MonthTotal = monthReceipts.Sum(r => r.Amount),
            MonthCount = monthReceipts.Count,
            YearTotal = yearReceipts.Sum(r => r.Amount),
            YearCount = yearReceipts.Count,
            PendingTotal = pending.Sum(a => a.AdmissionFee),
            PendingCount = pending.Count,
            RefundedTotal = refunds.Sum(r => r.RefundAmount ?? 0),
            RefundCount = refunds.Count,

            // Enterprise accounting dashboard metrics
            TodayAdmissionFees = todayReceipts.Sum(r => r.Amount),
            MonthlyAdmissionFees = monthReceipts.Sum(r => r.Amount),
            AdmissionRevenue = yearReceipts.Sum(r => r.Amount),
            PendingAdmissionPayments = pending.Count,
            RefundedAdmissionFees = refunds.Sum(r => r.RefundAmount ?? 0)
        };
    }
}
