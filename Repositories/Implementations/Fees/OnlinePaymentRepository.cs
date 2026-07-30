using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Repositories.Implementations.Fees;

public class OnlinePaymentRepository : BaseRepository<OnlinePaymentRequest>, IOnlinePaymentRepository
{
    public OnlinePaymentRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<OnlinePaymentRequestListItemDto> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber, int pageSize, string? searchTerm, int? statusFilter, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetOnlinePaymentRequestsPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);
        AddParameter(command, "@StatusFilter", statusFilter ?? 0);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<OnlinePaymentRequestListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new OnlinePaymentRequestListItemDto
            {
                Id = GetInt32(reader, "Id"),
                StudentId = GetInt32(reader, "StudentId"),
                StudentName = GetString(reader, "StudentName"),
                FeeInvoiceId = GetInt32(reader, "FeeInvoiceId"),
                InvoiceNo = GetString(reader, "InvoiceNo"),
                Amount = GetDecimal(reader, "Amount"),
                PaymentMethod = GetInt32(reader, "PaymentMethod"),
                ReferenceNo = GetNullableString(reader, "ReferenceNo"),
                Status = GetInt32(reader, "Status"),
                Remarks = GetNullableString(reader, "Remarks"),
                AdminNotes = GetNullableString(reader, "AdminNotes"),
                CreatedAt = GetDateTime(reader, "CreatedAt"),
                VerifiedAt = GetNullableDateTime(reader, "VerifiedAt"),
                VerifiedBy = GetNullableString(reader, "VerifiedBy"),
                RejectedAt = GetNullableDateTime(reader, "RejectedAt"),
                RejectedBy = GetNullableString(reader, "RejectedBy"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}
