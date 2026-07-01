using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Repositories.Interfaces.Admission;

namespace SchoolManagementSystem.Repositories.Implementations.Admission;

public class AdmissionDashboardRepository : IAdmissionDashboardRepository
{
    private readonly SchoolDbContext _db;

    public AdmissionDashboardRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<AdmissionDashboardDto> GetDashboardDataAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        var dto = new AdmissionDashboardDto();

        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_AdmissionDashboard";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@DateFrom", (object?)dateFrom ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@DateTo", (object?)dateTo ?? DBNull.Value));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);

            if (await reader.ReadAsync(ct))
                dto.TodayApplications = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);

            await reader.NextResultAsync(ct);
            if (await reader.ReadAsync(ct))
                dto.WeekApplications = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);

            await reader.NextResultAsync(ct);
            if (await reader.ReadAsync(ct))
                dto.MonthApplications = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);

            await reader.NextResultAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                dto.PendingVerification = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                dto.Approved = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                dto.Rejected = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                dto.Converted = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.MonthlyTrend.Add(new MonthlyTrendDto
                {
                    Year = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    Month = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                    Count = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                    PendingCount = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    ApprovedCount = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                    RejectedCount = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    ConvertedCount = reader.IsDBNull(6) ? 0 : reader.GetInt32(6)
                });
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.ClassDistribution.Add(new NameCountDto
                {
                    Name = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0),
                    Count = reader.IsDBNull(1) ? 0 : reader.GetInt32(1)
                });
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.GenderDistribution.Add(new NameCountDto
                {
                    Name = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0),
                    Count = reader.IsDBNull(1) ? 0 : reader.GetInt32(1)
                });
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.ReligionDistribution.Add(new NameCountDto
                {
                    Name = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0),
                    Count = reader.IsDBNull(1) ? 0 : reader.GetInt32(1)
                });
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.DistrictDistribution.Add(new NameCountDto
                {
                    Name = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0),
                    Count = reader.IsDBNull(1) ? 0 : reader.GetInt32(1)
                });
            }

            await reader.NextResultAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                dto.TotalApplications = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                dto.ConvertedCount = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                dto.ConversionRate = reader.IsDBNull(2) ? 0 : reader.GetDouble(2);
            }

            await reader.NextResultAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                dto.TotalInvoiceAmount = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                dto.TotalPaidAmount = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
                dto.TotalInvoices = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                dto.PaidInvoices = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.ApplicationHeatmap.Add(new DateCountDto
                {
                    Date = reader.IsDBNull(0) ? DateTime.MinValue : reader.GetDateTime(0),
                    Count = reader.IsDBNull(1) ? 0 : reader.GetInt32(1)
                });
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.TopClasses.Add(new NameCountDto
                {
                    Name = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0),
                    Count = reader.IsDBNull(1) ? 0 : reader.GetInt32(1)
                });
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }

        return dto;
    }

    public async Task<AdmissionRegisterReportDto> GetRegisterReportAsync(AdmissionReportRequest request, CancellationToken ct = default)
    {
        var dto = new AdmissionRegisterReportDto { Title = "Admission Register", GeneratedAt = DateTime.UtcNow };

        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_AdmissionRegisterReport";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@DateFrom", (object?)request.DateFrom ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@DateTo", (object?)request.DateTo ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@ClassId", (object?)request.ClassId ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Status", (object?)request.Status ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Gender", (object?)request.Gender ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Religion", (object?)request.Religion ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@District", (object?)request.District ?? DBNull.Value));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.Rows.Add(new AdmissionRegisterRow
                {
                    SerialNo = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    ApplicationNo = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    ApplicantName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    NameBangla = reader.IsDBNull(3) ? null : reader.GetString(3),
                    DateOfBirth = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                    Gender = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    FatherName = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    MotherName = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    Mobile = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    Religion = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                    AppliedClass = reader.IsDBNull(10) ? "" : reader.GetString(10),
                    Status = reader.IsDBNull(11) ? "" : reader.GetString(11),
                    SubmittedAt = reader.IsDBNull(12) ? DateTime.MinValue : reader.GetDateTime(12)
                });
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }

        return dto;
    }

    public async Task<List<TrendAnalysisDto>> GetTrendAnalysisAsync(DateTime? dateFrom = null, DateTime? dateTo = null, string? groupBy = "Month", CancellationToken ct = default)
    {
        var result = new List<TrendAnalysisDto>();

        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_AdmissionTrendAnalysis";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@DateFrom", (object?)dateFrom ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@DateTo", (object?)dateTo ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@GroupBy", groupBy ?? "Month"));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                result.Add(new TrendAnalysisDto
                {
                    PeriodLabel = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    PeriodYear = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                    PeriodMonth = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                    TotalApplications = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    PendingCount = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                    ApprovedCount = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    RejectedCount = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                    ConvertedCount = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                    ConversionRate = reader.IsDBNull(8) ? 0 : reader.GetDouble(8)
                });
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }

        return result;
    }

    public async Task<ConversionFunnelDto> GetConversionFunnelAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        var dto = new ConversionFunnelDto();

        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_AdmissionConversionFunnel";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@DateFrom", (object?)dateFrom ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@DateTo", (object?)dateTo ?? DBNull.Value));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                dto.TotalApplications = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                dto.DocumentVerified = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                dto.InterviewCompleted = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                dto.FeePaid = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                dto.Approved = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                dto.Converted = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);
                dto.ConversionRate = reader.IsDBNull(8) ? 0 : reader.GetDouble(8);
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }

        return dto;
    }

    public async Task<List<ClassDemandDto>> GetClassDemandAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        var result = new List<ClassDemandDto>();

        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_AdmissionClassDemand";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@DateFrom", (object?)dateFrom ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@DateTo", (object?)dateTo ?? DBNull.Value));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                result.Add(new ClassDemandDto
                {
                    ClassName = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0),
                    SortOrder = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                    TotalApplications = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                    ConvertedCount = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    PendingCount = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                    ApprovedCount = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    ConversionRate = reader.IsDBNull(6) ? 0 : reader.GetDouble(6),
                    GenderCount = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                    ReligionDiversity = reader.IsDBNull(8) ? 0 : reader.GetInt32(8)
                });
            }

            await reader.NextResultAsync(ct);
            var classMap = result.ToDictionary(r => r.ClassName, r => r);
            while (await reader.ReadAsync(ct))
            {
                var className = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0);
                if (classMap.TryGetValue(className, out var dto))
                {
                    dto.GenderBreakdown.Add(new NameCountDto
                    {
                        Name = reader.IsDBNull(1) ? "Unknown" : reader.GetString(1),
                        Count = reader.IsDBNull(2) ? 0 : reader.GetInt32(2)
                    });
                }
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }

        return result;
    }

    public async Task<RevenueReportDto> GetRevenueReportAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        var dto = new RevenueReportDto();

        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_AdmissionRevenueReport";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@DateFrom", (object?)dateFrom ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@DateTo", (object?)dateTo ?? DBNull.Value));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                dto.TotalInvoiceAmount = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                dto.TotalPaidAmount = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
                dto.TotalDueAmount = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);
                dto.TotalInvoices = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                dto.PaidInvoices = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                dto.PendingInvoices = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                dto.CollectionRate = reader.IsDBNull(6) ? 0 : reader.GetDouble(6);
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.ByClass.Add(new RevenueByClassDto
                {
                    ClassName = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0),
                    TotalInvoiceAmount = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1),
                    TotalPaidAmount = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2),
                    InvoiceCount = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    PaidCount = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                });
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.MonthlyTrend.Add(new RevenueTrendDto
                {
                    PeriodLabel = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    PeriodYear = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                    PeriodMonth = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                    TotalInvoiceAmount = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                    TotalPaidAmount = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),
                    InvoiceCount = reader.IsDBNull(5) ? 0 : reader.GetInt32(5)
                });
            }

            await reader.NextResultAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                dto.WaiverSummary = new WaiverSummaryDto
                {
                    TotalWaivers = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    TotalWaiverAmount = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1),
                    AvgWaiverPercentage = reader.IsDBNull(2) ? 0 : reader.GetDouble(2)
                };
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }

        return dto;
    }
}
