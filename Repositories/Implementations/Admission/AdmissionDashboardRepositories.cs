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
                dto.TodayApplications = reader.GetInt32(0);

            await reader.NextResultAsync(ct);
            if (await reader.ReadAsync(ct))
                dto.WeekApplications = reader.GetInt32(0);

            await reader.NextResultAsync(ct);
            if (await reader.ReadAsync(ct))
                dto.MonthApplications = reader.GetInt32(0);

            await reader.NextResultAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                dto.PendingVerification = reader.GetInt32(0);
                dto.Approved = reader.GetInt32(1);
                dto.Rejected = reader.GetInt32(2);
                dto.Converted = reader.GetInt32(3);
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.MonthlyTrend.Add(new MonthlyTrendDto
                {
                    Year = reader.GetInt32(0),
                    Month = reader.GetInt32(1),
                    Count = reader.GetInt32(2),
                    PendingCount = reader.GetInt32(3),
                    ApprovedCount = reader.GetInt32(4),
                    RejectedCount = reader.GetInt32(5),
                    ConvertedCount = reader.GetInt32(6)
                });
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.ClassDistribution.Add(new NameCountDto
                {
                    Name = reader.GetString(0),
                    Count = reader.GetInt32(1)
                });
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.GenderDistribution.Add(new NameCountDto
                {
                    Name = reader.GetString(0),
                    Count = reader.GetInt32(1)
                });
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.ReligionDistribution.Add(new NameCountDto
                {
                    Name = reader.GetString(0),
                    Count = reader.GetInt32(1)
                });
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.DistrictDistribution.Add(new NameCountDto
                {
                    Name = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0),
                    Count = reader.GetInt32(1)
                });
            }

            await reader.NextResultAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                dto.TotalApplications = reader.GetInt32(0);
                dto.ConvertedCount = reader.GetInt32(1);
                dto.ConversionRate = reader.GetDouble(2);
            }

            await reader.NextResultAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                dto.TotalInvoiceAmount = reader.GetDecimal(0);
                dto.TotalPaidAmount = reader.GetDecimal(1);
                dto.TotalInvoices = reader.GetInt32(2);
                dto.PaidInvoices = reader.GetInt32(3);
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.ApplicationHeatmap.Add(new DateCountDto
                {
                    Date = reader.GetDateTime(0),
                    Count = reader.GetInt32(1)
                });
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.TopClasses.Add(new NameCountDto
                {
                    Name = reader.GetString(0),
                    Count = reader.GetInt32(1)
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
                    SerialNo = reader.GetInt32(0),
                    ApplicationNo = reader.GetString(1),
                    ApplicantName = reader.GetString(2),
                    NameBangla = reader.IsDBNull(3) ? null : reader.GetString(3),
                    DateOfBirth = reader.GetDateTime(4),
                    Gender = reader.GetString(5),
                    FatherName = reader.GetString(6),
                    MotherName = reader.GetString(7),
                    Mobile = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    Religion = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                    AppliedClass = reader.GetString(10),
                    Status = reader.GetString(11),
                    SubmittedAt = reader.GetDateTime(12)
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
                    PeriodLabel = reader.GetString(0),
                    PeriodYear = reader.GetInt32(1),
                    PeriodMonth = reader.GetInt32(2),
                    TotalApplications = reader.GetInt32(3),
                    PendingCount = reader.GetInt32(4),
                    ApprovedCount = reader.GetInt32(5),
                    RejectedCount = reader.GetInt32(6),
                    ConvertedCount = reader.GetInt32(7),
                    ConversionRate = reader.GetDouble(8)
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
                dto.TotalApplications = reader.GetInt32(2);
                dto.DocumentVerified = reader.GetInt32(3);
                dto.InterviewCompleted = reader.GetInt32(4);
                dto.FeePaid = reader.GetInt32(5);
                dto.Approved = reader.GetInt32(6);
                dto.Converted = reader.GetInt32(7);
                dto.ConversionRate = reader.GetDouble(8);
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
                    ClassName = reader.GetString(0),
                    SortOrder = reader.GetInt32(1),
                    TotalApplications = reader.GetInt32(2),
                    ConvertedCount = reader.GetInt32(3),
                    PendingCount = reader.GetInt32(4),
                    ApprovedCount = reader.GetInt32(5),
                    ConversionRate = reader.GetDouble(6),
                    GenderCount = reader.GetInt32(7),
                    ReligionDiversity = reader.GetInt32(8)
                });
            }

            await reader.NextResultAsync(ct);
            var classMap = result.ToDictionary(r => r.ClassName, r => r);
            while (await reader.ReadAsync(ct))
            {
                var className = reader.GetString(0);
                if (classMap.TryGetValue(className, out var dto))
                {
                    dto.GenderBreakdown.Add(new NameCountDto
                    {
                        Name = reader.GetString(1),
                        Count = reader.GetInt32(2)
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
                dto.TotalInvoiceAmount = reader.GetDecimal(0);
                dto.TotalPaidAmount = reader.GetDecimal(1);
                dto.TotalDueAmount = reader.GetDecimal(2);
                dto.TotalInvoices = reader.GetInt32(3);
                dto.PaidInvoices = reader.GetInt32(4);
                dto.PendingInvoices = reader.GetInt32(5);
                dto.CollectionRate = reader.GetDouble(6);
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.ByClass.Add(new RevenueByClassDto
                {
                    ClassName = reader.GetString(0),
                    TotalInvoiceAmount = reader.GetDecimal(1),
                    TotalPaidAmount = reader.GetDecimal(2),
                    InvoiceCount = reader.GetInt32(3),
                    PaidCount = reader.GetInt32(4)
                });
            }

            await reader.NextResultAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                dto.MonthlyTrend.Add(new RevenueTrendDto
                {
                    PeriodLabel = reader.GetString(0),
                    PeriodYear = reader.GetInt32(1),
                    PeriodMonth = reader.GetInt32(2),
                    TotalInvoiceAmount = reader.GetDecimal(3),
                    TotalPaidAmount = reader.GetDecimal(4),
                    InvoiceCount = reader.GetInt32(5)
                });
            }

            await reader.NextResultAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                dto.WaiverSummary = new WaiverSummaryDto
                {
                    TotalWaivers = reader.GetInt32(0),
                    TotalWaiverAmount = reader.GetDecimal(1),
                    AvgWaiverPercentage = reader.GetDouble(2)
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
