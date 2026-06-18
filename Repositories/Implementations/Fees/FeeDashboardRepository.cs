using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Repositories.Implementations.Fees;

public class FeeDashboardRepository : IFeeDashboardRepository
{
    private readonly SchoolDbContext _db;

    public FeeDashboardRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<FeeDashboardDto> GetDashboardDataAsync(int? academicYearId, CancellationToken ct)
    {
        var result = new FeeDashboardDto();

        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFeeDashboard";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@AcademicYearId", academicYearId ?? 0));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);

            // First result set: summary
            if (await reader.ReadAsync(ct))
            {
                result.TotalAssigned = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                result.TotalCollected = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
                result.TotalOutstanding = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);
                result.TotalDiscounted = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3);
                result.TotalInvoices = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                result.TotalPayments = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                result.OverdueInvoices = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                result.CollectionRate = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7);
            }

            // Second result set: monthly collection trend
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    result.MonthlyCollections.Add(new MonthlyCollectionDto
                    {
                        Year = reader.GetInt32(0),
                        Month = reader.GetInt32(1),
                        Collected = reader.GetDecimal(2),
                        TransactionCount = reader.GetInt32(3)
                    });
                }
            }

            // Third result set: payment method breakdown
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    result.PaymentMethodBreakdown.Add(new PaymentMethodBreakdownDto
                    {
                        Method = reader.GetInt32(0),
                        Count = reader.GetInt32(1),
                        Total = reader.GetDecimal(2)
                    });
                }
            }

            // Fourth result set: due soon invoices
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    result.DueSoonInvoices.Add(new DueSoonInvoiceDto
                    {
                        Id = reader.GetInt32(0),
                        InvoiceNo = reader.GetString(1),
                        StudentName = reader.GetString(2),
                        DueDate = DateOnly.FromDateTime(reader.GetDateTime(3)),
                        TotalAmount = reader.GetDecimal(4),
                        PaidAmount = reader.GetDecimal(5),
                        DueAmount = reader.GetDecimal(6),
                        DaysRemaining = reader.GetInt32(7)
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
}
