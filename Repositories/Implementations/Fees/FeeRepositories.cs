using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Repositories.Implementations.Fees;

public class FeeStructureRepository : BaseRepository<FeeStructure>, IFeeStructureRepository
{
    public FeeStructureRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FeeStructureListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFeeStructureList";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
        command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
        command.Parameters.Add(new SqlParameter("@SearchTerm", (object?)searchTerm ?? DBNull.Value));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            var items = new List<FeeStructureListItemDto>();
            while (await reader.ReadAsync(ct))
            {
                items.Add(new FeeStructureListItemDto
                {
                    Id = reader.GetInt32(0),
                    SchoolClassId = reader.GetInt32(1),
                    ClassName = reader.GetString(2),
                    FeeName = reader.GetString(3),
                    Amount = reader.GetDecimal(4),
                    IsRecurring = reader.GetBoolean(5),
                    TotalRecords = reader.IsDBNull(6) ? 0 : reader.GetInt32(6)
                });
            }
            return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
        }
        finally { await _db.Database.CloseConnectionAsync(); }
    }
}

public class FeeInvoiceRepository : BaseRepository<FeeInvoice>, IFeeInvoiceRepository
{
    public FeeInvoiceRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FeeInvoiceListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? studentId, CancellationToken ct)
    {
        var items = new List<FeeInvoiceListItemDto>();
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFeeInvoiceList";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
        command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
        command.Parameters.Add(new SqlParameter("@SearchTerm", (object?)searchTerm ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@StudentId", studentId ?? 0));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                items.Add(new FeeInvoiceListItemDto
                {
                    Id = reader.GetInt32(0),
                    InvoiceNo = reader.GetString(1),
                    StudentId = reader.GetInt32(2),
                    StudentName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    DueDate = DateOnly.FromDateTime(reader.GetDateTime(4)),
                    TotalAmount = reader.GetDecimal(5),
                    PaidAmount = reader.GetDecimal(6),
                    Status = (SchoolManagementSystem.Models.Enums.PaymentStatus)reader.GetInt32(7),
                    TotalRecords = reader.IsDBNull(8) ? 0 : reader.GetInt32(8)
                });
            }
            return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
        }
        finally { await _db.Database.CloseConnectionAsync(); }
    }
}
