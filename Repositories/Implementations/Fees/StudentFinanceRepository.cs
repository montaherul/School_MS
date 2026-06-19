using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Repositories.Implementations.Fees;

public class StudentFinanceRepository : IStudentFinanceRepository
{
    private readonly SchoolDbContext _db;

    public StudentFinanceRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<(List<StudentInvoiceDto> Items, int TotalRecords)> GetInvoicesPagedAsync(
        int studentId, int page, int pageSize, string? search, int? status, CancellationToken ct)
    {
        var items = new List<StudentInvoiceDto>();
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetStudentInvoicesPaged";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(new SqlParameter("@PageNumber", page));
        cmd.Parameters.Add(new SqlParameter("@PageSize", pageSize));
        cmd.Parameters.Add(new SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@StudentId", studentId));
        cmd.Parameters.Add(new SqlParameter("@Status", status ?? 0));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                items.Add(new StudentInvoiceDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    InvoiceNo = reader.GetString(reader.GetOrdinal("InvoiceNo")),
                    InvoiceDate = reader.GetDateTime(reader.GetOrdinal("InvoiceDate")),
                    DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    PaidAmount = reader.GetDecimal(reader.GetOrdinal("PaidAmount")),
                    DiscountAmount = reader.GetDecimal(reader.GetOrdinal("DiscountAmount")),
                    LateFee = reader.GetDecimal(reader.GetOrdinal("LateFee")),
                    Status = reader.GetInt32(reader.GetOrdinal("Status")),
                    TotalRecords = reader.GetInt32(reader.GetOrdinal("TotalRecords"))
                });
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
        var totalRecords = items.FirstOrDefault()?.TotalRecords ?? 0;
        return (items, totalRecords);
    }

    public async Task<(List<StudentPaymentDto> Items, int TotalRecords)> GetPaymentsPagedAsync(
        int studentId, int page, int pageSize, string? search, CancellationToken ct)
    {
        var items = new List<StudentPaymentDto>();
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetStudentPaymentsPaged";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(new SqlParameter("@PageNumber", page));
        cmd.Parameters.Add(new SqlParameter("@PageSize", pageSize));
        cmd.Parameters.Add(new SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@StudentId", studentId));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                items.Add(new StudentPaymentDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate")),
                    Method = reader.GetInt32(reader.GetOrdinal("Method")),
                    ReferenceNo = reader.IsDBNull(reader.GetOrdinal("ReferenceNo")) ? null : reader.GetString(reader.GetOrdinal("ReferenceNo")),
                    Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                    LateFee = reader.GetDecimal(reader.GetOrdinal("LateFee")),
                    DiscountAmount = reader.GetDecimal(reader.GetOrdinal("DiscountAmount")),
                    InvoiceNo = reader.GetString(reader.GetOrdinal("InvoiceNo")),
                    TotalRecords = reader.GetInt32(reader.GetOrdinal("TotalRecords"))
                });
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
        var totalRecords = items.FirstOrDefault()?.TotalRecords ?? 0;
        return (items, totalRecords);
    }

    public async Task<(List<StudentLedgerEntryDto> Items, int TotalRecords)> GetLedgerPagedAsync(
        int studentId, int page, int pageSize, string? search, CancellationToken ct)
    {
        var items = new List<StudentLedgerEntryDto>();
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetStudentLedgerPaged";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(new SqlParameter("@PageNumber", page));
        cmd.Parameters.Add(new SqlParameter("@PageSize", pageSize));
        cmd.Parameters.Add(new SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@StudentId", studentId));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                items.Add(new StudentLedgerEntryDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    TransactionDate = reader.GetDateTime(reader.GetOrdinal("TransactionDate")),
                    Type = reader.GetInt32(reader.GetOrdinal("Type")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                    Debit = reader.GetDecimal(reader.GetOrdinal("Debit")),
                    Credit = reader.GetDecimal(reader.GetOrdinal("Credit")),
                    Balance = reader.GetDecimal(reader.GetOrdinal("Balance")),
                    TotalRecords = reader.GetInt32(reader.GetOrdinal("TotalRecords"))
                });
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
        var totalRecords = items.FirstOrDefault()?.TotalRecords ?? 0;
        return (items, totalRecords);
    }

    public async Task<(decimal TotalInvoiced, decimal TotalPaid)> GetFinanceSummaryAsync(int studentId, CancellationToken ct)
    {
        var invoices = await _db.FeeInvoices
            .Where(fi => fi.StudentId == studentId && !fi.IsDeleted)
            .ToListAsync(ct);
        return (invoices.Sum(fi => fi.TotalAmount + fi.LateFee), invoices.Sum(fi => fi.PaidAmount));
    }

    public async Task<StudentPaymentDto?> GetLastPaymentAsync(int studentId, CancellationToken ct)
    {
        var payment = await _db.Payments
            .Where(p => !p.IsDeleted && p.FeeInvoice != null && p.FeeInvoice.StudentId == studentId)
            .OrderByDescending(p => p.PaidAt)
            .Select(p => new StudentPaymentDto
            {
                Id = p.Id,
                PaymentDate = p.PaidAt,
                Method = (int)p.Method,
                ReferenceNo = p.ReferenceNo,
                Amount = p.Amount
            })
            .FirstOrDefaultAsync(ct);
        return payment;
    }
}