using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Repositories.Implementations.Fees;

public class FeeReportRepository : IFeeReportRepository
{
    private readonly SchoolDbContext _db;

    public FeeReportRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<(List<StudentLedgerReportDto> items, int total)> GetStudentLedgerReportAsync(int studentId, int page, int pageSize)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetStudentLedgerReport";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@StudentId", studentId));
        command.Parameters.Add(new SqlParameter("@PageNumber", page));
        command.Parameters.Add(new SqlParameter("@PageSize", pageSize));

        await _db.Database.OpenConnectionAsync();
        try
        {
            using var reader = await command.ExecuteReaderAsync();
            var items = new List<StudentLedgerReportDto>();
            while (await reader.ReadAsync())
            {
                items.Add(new StudentLedgerReportDto
                {
                    Id = reader.GetInt32(0),
                    TransactionType = ((FeeLedgerType)reader.GetInt32(1)).ToString(),
                    InvoiceNo = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    TransactionDate = reader.GetDateTime(3),
                    Debit = reader.GetDecimal(4),
                    Credit = reader.GetDecimal(5),
                    Balance = reader.GetDecimal(6),
                    Description = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    TotalRecords = reader.GetInt32(8)
                });
            }
            var total = items.FirstOrDefault()?.TotalRecords ?? 0;
            return (items, total);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<(List<DailyCollectionReportDto> items, int total)> GetDailyCollectionReportAsync(DateOnly date, int page, int pageSize)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetDailyCollectionReport";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@CollectionDate", date.ToDateTime(TimeOnly.MinValue)));
        command.Parameters.Add(new SqlParameter("@PageNumber", page));
        command.Parameters.Add(new SqlParameter("@PageSize", pageSize));

        await _db.Database.OpenConnectionAsync();
        try
        {
            using var reader = await command.ExecuteReaderAsync();
            var items = new List<DailyCollectionReportDto>();
            while (await reader.ReadAsync())
            {
                items.Add(new DailyCollectionReportDto
                {
                    Id = reader.GetInt32(0),
                    InvoiceNo = reader.GetString(1),
                    StudentName = reader.GetString(2),
                    Amount = reader.GetDecimal(3),
                    PaymentMethod = reader.GetString(4),
                    ReferenceNo = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    PaidAt = reader.GetDateTime(6),
                    TotalRecords = reader.GetInt32(7)
                });
            }
            var total = items.FirstOrDefault()?.TotalRecords ?? 0;
            return (items, total);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<(List<MonthlyCollectionReportDto> items, int total)> GetMonthlyCollectionReportAsync(int year, int page, int pageSize)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetMonthlyCollectionReport";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@Year", year));
        command.Parameters.Add(new SqlParameter("@PageNumber", page));
        command.Parameters.Add(new SqlParameter("@PageSize", pageSize));

        await _db.Database.OpenConnectionAsync();
        try
        {
            using var reader = await command.ExecuteReaderAsync();
            var items = new List<MonthlyCollectionReportDto>();
            while (await reader.ReadAsync())
            {
                items.Add(new MonthlyCollectionReportDto
                {
                    Year = reader.GetInt32(0),
                    Month = reader.GetInt32(1),
                    TotalCollected = reader.GetDecimal(2),
                    TransactionCount = reader.GetInt32(3),
                    TotalRecords = reader.GetInt32(4)
                });
            }
            var total = items.FirstOrDefault()?.TotalRecords ?? 0;
            return (items, total);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<(List<DueReportDto> items, int total)> GetDueReportAsync(int page, int pageSize, int classId = 0)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetDueReport";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@PageNumber", page));
        command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
        command.Parameters.Add(new SqlParameter("@ClassId", classId));

        await _db.Database.OpenConnectionAsync();
        try
        {
            using var reader = await command.ExecuteReaderAsync();
            var items = new List<DueReportDto>();
            while (await reader.ReadAsync())
            {
                items.Add(new DueReportDto
                {
                    Id = reader.GetInt32(0),
                    InvoiceNo = reader.GetString(1),
                    StudentName = reader.GetString(2),
                    ClassName = reader.GetString(3),
                    DueDate = DateOnly.FromDateTime(reader.GetDateTime(4)),
                    TotalAmount = reader.GetDecimal(5),
                    PaidAmount = reader.GetDecimal(6),
                    DueAmount = reader.GetDecimal(7),
                    DaysOverdue = reader.GetInt32(8),
                    TotalRecords = reader.GetInt32(9)
                });
            }
            var total = items.FirstOrDefault()?.TotalRecords ?? 0;
            return (items, total);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<(List<DiscountReportDto> items, int total)> GetDiscountReportAsync(int page, int pageSize)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetDiscountReport";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@PageNumber", page));
        command.Parameters.Add(new SqlParameter("@PageSize", pageSize));

        await _db.Database.OpenConnectionAsync();
        try
        {
            using var reader = await command.ExecuteReaderAsync();
            var items = new List<DiscountReportDto>();
            while (await reader.ReadAsync())
            {
                items.Add(new DiscountReportDto
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    DiscountType = reader.GetString(2),
                    Value = reader.GetDecimal(3),
                    ClassName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    FeeCategoryName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    IsActive = reader.GetBoolean(6),
                    TotalRecords = reader.GetInt32(7)
                });
            }
            var total = items.FirstOrDefault()?.TotalRecords ?? 0;
            return (items, total);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<(List<WaiverReportDto> items, int total)> GetWaiverReportAsync(int page, int pageSize)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetWaiverReport";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@PageNumber", page));
        command.Parameters.Add(new SqlParameter("@PageSize", pageSize));

        await _db.Database.OpenConnectionAsync();
        try
        {
            using var reader = await command.ExecuteReaderAsync();
            var items = new List<WaiverReportDto>();
            while (await reader.ReadAsync())
            {
                items.Add(new WaiverReportDto
                {
                    Id = reader.GetInt32(0),
                    StudentName = reader.GetString(1),
                    InvoiceNo = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    WaiverAmount = reader.GetDecimal(3),
                    Reason = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    IsApproved = reader.GetBoolean(5),
                    CreatedAt = reader.GetDateTime(6),
                    ApprovedBy = reader.IsDBNull(7) ? "N/A" : reader.GetString(7),
                    TotalRecords = reader.GetInt32(8)
                });
            }
            var total = items.FirstOrDefault()?.TotalRecords ?? 0;
            return (items, total);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<(List<RefundReportDto> items, int total)> GetRefundReportAsync(int page, int pageSize)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetRefundReport";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@PageNumber", page));
        command.Parameters.Add(new SqlParameter("@PageSize", pageSize));

        await _db.Database.OpenConnectionAsync();
        try
        {
            using var reader = await command.ExecuteReaderAsync();
            var items = new List<RefundReportDto>();
            while (await reader.ReadAsync())
            {
                items.Add(new RefundReportDto
                {
                    Id = reader.GetInt32(0),
                    StudentName = reader.GetString(1),
                    InvoiceNo = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    RefundAmount = reader.GetDecimal(3),
                    Reason = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    IsApproved = reader.GetBoolean(5),
                    RefundDate = reader.GetDateTime(6),
                    TotalRecords = reader.GetInt32(7)
                });
            }
            var total = items.FirstOrDefault()?.TotalRecords ?? 0;
            return (items, total);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<(List<ClassCollectionSummaryDto> items, int total)> GetClassCollectionSummaryAsync(int academicYearId, int page, int pageSize)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetClassCollectionSummary";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@AcademicYearId", academicYearId));
        command.Parameters.Add(new SqlParameter("@PageNumber", page));
        command.Parameters.Add(new SqlParameter("@PageSize", pageSize));

        await _db.Database.OpenConnectionAsync();
        try
        {
            using var reader = await command.ExecuteReaderAsync();
            var items = new List<ClassCollectionSummaryDto>();
            while (await reader.ReadAsync())
            {
                items.Add(new ClassCollectionSummaryDto
                {
                    ClassName = reader.GetString(0),
                    TotalAssigned = reader.GetDecimal(1),
                    TotalCollected = reader.GetDecimal(2),
                    TotalDue = reader.GetDecimal(3),
                    CollectionRate = reader.GetDecimal(4),
                    StudentCount = reader.GetInt32(5),
                    TotalRecords = reader.GetInt32(6)
                });
            }
            var total = items.FirstOrDefault()?.TotalRecords ?? 0;
            return (items, total);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<CashBookResultDto> GetCashBookAsync(DateOnly fromDate, DateOnly toDate, int? academicYearId = null)
    {
        var result = new CashBookResultDto();
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetCashBook";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@FromDate", fromDate.ToDateTime(TimeOnly.MinValue)));
        command.Parameters.Add(new SqlParameter("@ToDate", toDate.ToDateTime(TimeOnly.MinValue)));
        command.Parameters.Add(new SqlParameter("@AcademicYearId", academicYearId ?? (object)DBNull.Value));

        await _db.Database.OpenConnectionAsync();
        try
        {
            using var reader = await command.ExecuteReaderAsync();

            // Result set 1: opening cash balance
            if (await reader.ReadAsync())
                result.OpeningBalance = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);

            // Result set 2: daily cash flow
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    var day = new CashBookDayDto
                    {
                        OpeningBalance = result.OpeningBalance + result.Days.Sum(d => d.NetChange),
                        TransactionDate = reader.GetDateTime(0),
                        CashIn = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1),
                        CashOut = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2),
                        NetChange = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                        PaymentCount = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                        RefundCount = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                        EntryCount = reader.IsDBNull(6) ? 0 : reader.GetInt32(6)
                    };
                    day.ClosingBalance = day.OpeningBalance + day.NetChange;
                    result.Days.Add(day);
                }
            }

            result.TotalCashIn = result.Days.Sum(d => d.CashIn);
            result.TotalCashOut = result.Days.Sum(d => d.CashOut);
            result.ClosingBalance = result.OpeningBalance + result.TotalCashIn - result.TotalCashOut;
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
        return result;
    }
}

