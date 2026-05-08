using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeInvoiceService : IFeeInvoiceService
{
    private readonly SchoolDbContext _db;

    public FeeInvoiceService(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<FeeInvoiceListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? studentId = null, CancellationToken cancellationToken = default)
    {
        var items = new List<FeeInvoiceListItemDto>();
        int totalCount = 0;

        using (var command = _db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetFeeInvoiceList";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@PageNumber", page));
            command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
            command.Parameters.Add(new SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@StudentId", (object?)studentId ?? 0));

            await _db.Database.OpenConnectionAsync(cancellationToken);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
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
            }
            await _db.Database.CloseConnectionAsync();
        }

        totalCount = items.FirstOrDefault()?.TotalRecords ?? 0;

        return new PagedResult<FeeInvoiceListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<FeeInvoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.FeeInvoices.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<int> CreateAsync(FeeInvoice invoice, string createdBy, CancellationToken cancellationToken = default)
    {
        invoice.CreatedBy = createdBy;
        invoice.CreatedAt = DateTime.UtcNow;
        _db.FeeInvoices.Add(invoice);
        await _db.SaveChangesAsync(cancellationToken);
        return invoice.Id;
    }

    public async Task UpdateAsync(FeeInvoice invoice, string updatedBy, CancellationToken cancellationToken = default)
    {
        var existing = await _db.FeeInvoices.FirstOrDefaultAsync(x => x.Id == invoice.Id && !x.IsDeleted, cancellationToken)
            ?? throw new Exception("Invoice not found");

        existing.InvoiceNo = invoice.InvoiceNo;
        existing.StudentId = invoice.StudentId;
        existing.DueDate = invoice.DueDate;
        existing.TotalAmount = invoice.TotalAmount;
        existing.PaidAmount = invoice.PaidAmount;
        existing.Status = invoice.Status;
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var existing = await _db.FeeInvoices.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new Exception("Invoice not found");

        existing.IsDeleted = true;
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
