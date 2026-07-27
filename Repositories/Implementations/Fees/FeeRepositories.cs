using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Repositories.Implementations.Fees;

public class FeeCategoryRepository : BaseRepository<FeeCategory>, IFeeCategoryRepository
{
    public FeeCategoryRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FeeCategoryListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFeeCategoriesPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<FeeCategoryListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new FeeCategoryListItemDto
            {
                Id = GetInt32(reader, "Id"),
                Name = GetString(reader, "Name"),
                Description = GetNullableString(reader, "Description"),
                DisplayOrder = GetInt32(reader, "DisplayOrder"),
                IsActive = GetBoolean(reader, "IsActive"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

public class FeeStructureRepository : BaseRepository<FeeStructure>, IFeeStructureRepository
{
    public FeeStructureRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FeeStructureListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? schoolClassId, int? feeCategoryId, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFeeStructureList";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);
        AddParameter(command, "@SchoolClassId", schoolClassId ?? 0);
        AddParameter(command, "@FeeCategoryId", feeCategoryId ?? 0);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<FeeStructureListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new FeeStructureListItemDto
            {
                Id = GetInt32(reader, "Id"),
                SchoolClassId = GetInt32(reader, "SchoolClassId"),
                ClassName = GetString(reader, "ClassName"),
                FeeCategoryId = GetNullableInt32(reader, "FeeCategoryId"),
                FeeCategoryName = GetNullableString(reader, "FeeCategoryName"),
                AcademicYearId = GetNullableInt32(reader, "AcademicYearId"),
                AcademicYearName = GetNullableString(reader, "AcademicYearName"),
                FeeName = GetString(reader, "FeeName"),
                Description = GetNullableString(reader, "Description"),
                Amount = GetDecimal(reader, "Amount"),
                IsRecurring = GetBoolean(reader, "IsRecurring"),
                Frequency = GetInt32(reader, "Frequency"),
                DueDay = GetNullableInt32(reader, "DueDay"),
                IsActive = GetBoolean(reader, "IsActive"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

public class StudentFeeAssignmentRepository : BaseRepository<StudentFeeAssignment>, IStudentFeeAssignmentRepository
{
    public StudentFeeAssignmentRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<StudentFeeAssignmentListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? studentId, int? feeStructureId, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetStudentFeeAssignmentsPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);
        AddParameter(command, "@StudentId", studentId ?? 0);
        AddParameter(command, "@FeeStructureId", feeStructureId ?? 0);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<StudentFeeAssignmentListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new StudentFeeAssignmentListItemDto
            {
                Id = GetInt32(reader, "Id"),
                StudentId = GetInt32(reader, "StudentId"),
                StudentName = GetString(reader, "StudentName"),
                StudentNo = GetString(reader, "StudentNo"),
                FeeStructureId = GetInt32(reader, "FeeStructureId"),
                FeeStructureName = GetString(reader, "FeeStructureName"),
                AcademicYearId = GetNullableInt32(reader, "AcademicYearId"),
                AcademicYearName = GetNullableString(reader, "AcademicYearName"),
                CustomAmount = GetNullableDecimal(reader, "CustomAmount"),
                IsActive = GetBoolean(reader, "IsActive"),
                ValidFrom = GetNullableDateOnly(reader, "ValidFrom"),
                ValidTo = GetNullableDateOnly(reader, "ValidTo"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

public class FeeInvoiceRepository : BaseRepository<FeeInvoice>, IFeeInvoiceRepository
{
    public FeeInvoiceRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FeeInvoiceListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? studentId, int? status, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFeeInvoiceList";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);
        AddParameter(command, "@StudentId", studentId ?? 0);
        AddParameter(command, "@Status", status ?? 0);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<FeeInvoiceListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new FeeInvoiceListItemDto
            {
                Id = GetInt32(reader, "Id"),
                InvoiceNo = GetString(reader, "InvoiceNo"),
                StudentId = GetInt32(reader, "StudentId"),
                StudentName = GetString(reader, "StudentName"),
                AcademicYearId = GetNullableInt32(reader, "AcademicYearId"),
                AcademicYearName = GetNullableString(reader, "AcademicYearName"),
                DueDate = DateOnly.FromDateTime(GetDateTime(reader, "DueDate")),
                TotalAmount = GetDecimal(reader, "TotalAmount"),
                PaidAmount = GetDecimal(reader, "PaidAmount"),
                DiscountAmount = GetDecimal(reader, "DiscountAmount"),
                LateFee = GetDecimal(reader, "LateFee"),
                Status = GetInt32(reader, "Status"),
                Remarks = GetNullableString(reader, "Remarks"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

public class FeeInvoiceItemRepository : BaseRepository<FeeInvoiceItem>, IFeeInvoiceItemRepository
{
    public FeeInvoiceItemRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FeeInvoiceItemListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? feeInvoiceId, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFeeInvoiceItemsPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);
        AddParameter(command, "@FeeInvoiceId", feeInvoiceId ?? 0);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<FeeInvoiceItemListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new FeeInvoiceItemListItemDto
            {
                Id = GetInt32(reader, "Id"),
                FeeInvoiceId = GetInt32(reader, "FeeInvoiceId"),
                InvoiceNo = GetString(reader, "InvoiceNo"),
                FeeStructureId = GetNullableInt32(reader, "FeeStructureId"),
                FeeStructureName = GetNullableString(reader, "FeeStructureName"),
                FeeCategoryId = GetNullableInt32(reader, "FeeCategoryId"),
                FeeCategoryName = GetNullableString(reader, "FeeCategoryName"),
                Description = GetString(reader, "Description"),
                Amount = GetDecimal(reader, "Amount"),
                DiscountAmount = GetDecimal(reader, "DiscountAmount"),
                NetAmount = GetDecimal(reader, "NetAmount"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

public class FeePaymentRepository : BaseRepository<Payment>, IFeePaymentRepository
{
    public FeePaymentRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FeePaymentListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? feeInvoiceId, int? paymentMethod, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFeePaymentsPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);
        AddParameter(command, "@FeeInvoiceId", feeInvoiceId ?? 0);
        AddParameter(command, "@PaymentMethod", paymentMethod ?? 0);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<FeePaymentListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new FeePaymentListItemDto
            {
                Id = GetInt32(reader, "Id"),
                FeeInvoiceId = GetInt32(reader, "FeeInvoiceId"),
                InvoiceNo = GetString(reader, "InvoiceNo"),
                StudentId = GetInt32(reader, "StudentId"),
                StudentName = GetString(reader, "StudentName"),
                Amount = GetDecimal(reader, "Amount"),
                LateFee = GetDecimal(reader, "LateFee"),
                DiscountAmount = GetDecimal(reader, "DiscountAmount"),
                Method = GetInt32(reader, "Method"),
                ReferenceNo = GetNullableString(reader, "ReferenceNo"),
                PaidAt = GetDateTime(reader, "PaidAt"),
                Remarks = GetNullableString(reader, "Remarks"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

public class FeeDiscountRepository : BaseRepository<FeeDiscount>, IFeeDiscountRepository
{
    public FeeDiscountRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FeeDiscountListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFeeDiscountsPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<FeeDiscountListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new FeeDiscountListItemDto
            {
                Id = GetInt32(reader, "Id"),
                Name = GetString(reader, "Name"),
                Description = GetNullableString(reader, "Description"),
                DiscountType = GetInt32(reader, "DiscountType"),
                Value = GetDecimal(reader, "Value"),
                SchoolClassId = GetNullableInt32(reader, "SchoolClassId"),
                ClassName = GetNullableString(reader, "ClassName"),
                FeeCategoryId = GetNullableInt32(reader, "FeeCategoryId"),
                FeeCategoryName = GetNullableString(reader, "FeeCategoryName"),
                FeeStructureId = GetNullableInt32(reader, "FeeStructureId"),
                FeeStructureName = GetNullableString(reader, "FeeStructureName"),
                IsActive = GetBoolean(reader, "IsActive"),
                ValidFrom = GetNullableDateOnly(reader, "ValidFrom"),
                ValidTo = GetNullableDateOnly(reader, "ValidTo"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

public class FeeWaiverRepository : BaseRepository<FeeWaiver>, IFeeWaiverRepository
{
    public FeeWaiverRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FeeWaiverListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? studentId, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFeeWaiversPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);
        AddParameter(command, "@StudentId", studentId ?? 0);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<FeeWaiverListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new FeeWaiverListItemDto
            {
                Id = GetInt32(reader, "Id"),
                StudentId = GetInt32(reader, "StudentId"),
                StudentName = GetString(reader, "StudentName"),
                FeeInvoiceId = GetNullableInt32(reader, "FeeInvoiceId"),
                InvoiceNo = GetNullableString(reader, "InvoiceNo"),
                FeeCategoryId = GetNullableInt32(reader, "FeeCategoryId"),
                FeeCategoryName = GetNullableString(reader, "FeeCategoryName"),
                FeeStructureId = GetNullableInt32(reader, "FeeStructureId"),
                FeeStructureName = GetNullableString(reader, "FeeStructureName"),
                WaiverType = GetInt32(reader, "WaiverType"),
                WaiverValue = GetDecimal(reader, "WaiverValue"),
                WaiverAmount = GetDecimal(reader, "WaiverAmount"),
                Reason = GetNullableString(reader, "Reason"),
                IsApproved = GetBoolean(reader, "IsApproved"),
                ValidFrom = GetNullableDateOnly(reader, "ValidFrom"),
                ValidTo = GetNullableDateOnly(reader, "ValidTo"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

public class FeeRefundRepository : BaseRepository<FeeRefund>, IFeeRefundRepository
{
    public FeeRefundRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FeeRefundListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFeeRefundsPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<FeeRefundListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new FeeRefundListItemDto
            {
                Id = GetInt32(reader, "Id"),
                FeePaymentId = GetInt32(reader, "FeePaymentId"),
                FeeInvoiceId = GetInt32(reader, "FeeInvoiceId"),
                InvoiceNo = GetString(reader, "InvoiceNo"),
                StudentId = GetInt32(reader, "StudentId"),
                StudentName = GetString(reader, "StudentName"),
                RefundAmount = GetDecimal(reader, "RefundAmount"),
                RefundMethod = GetInt32(reader, "RefundMethod"),
                ReferenceNo = GetNullableString(reader, "ReferenceNo"),
                Reason = GetNullableString(reader, "Reason"),
                IsApproved = GetBoolean(reader, "IsApproved"),
                RefundDate = GetDateTime(reader, "RefundDate"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

public class FeeLedgerRepository : BaseRepository<FeeLedger>, IFeeLedgerRepository
{
    public FeeLedgerRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FeeLedgerListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? studentId, int? transactionType, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFeeLedgerPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);
        AddParameter(command, "@StudentId", studentId ?? 0);
        AddParameter(command, "@TransactionType", transactionType ?? 0);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<FeeLedgerListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new FeeLedgerListItemDto
            {
                Id = GetInt32(reader, "Id"),
                StudentId = GetInt32(reader, "StudentId"),
                StudentName = GetString(reader, "StudentName"),
                FeeInvoiceId = GetNullableInt32(reader, "FeeInvoiceId"),
                InvoiceNo = GetNullableString(reader, "InvoiceNo"),
                FeePaymentId = GetNullableInt32(reader, "FeePaymentId"),
                TransactionType = GetInt32(reader, "TransactionType"),
                Debit = GetDecimal(reader, "Debit"),
                Credit = GetDecimal(reader, "Credit"),
                Balance = GetDecimal(reader, "Balance"),
                Description = GetNullableString(reader, "Description"),
                TransactionDate = GetDateTime(reader, "TransactionDate"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

public class FeeCollectionSummaryRepository : BaseRepository<FeeCollectionSummary>, IFeeCollectionSummaryRepository
{
    public FeeCollectionSummaryRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FeeCollectionSummaryListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, DateOnly? fromDate, DateOnly? toDate, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFeeCollectionSummariesPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);
        AddParameter(command, "@FromDate", fromDate?.ToDateTime(TimeOnly.MinValue));
        AddParameter(command, "@ToDate", toDate?.ToDateTime(TimeOnly.MinValue));

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<FeeCollectionSummaryListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new FeeCollectionSummaryListItemDto
            {
                Id = GetInt32(reader, "Id"),
                CollectionDate = DateOnly.FromDateTime(GetDateTime(reader, "CollectionDate")),
                TotalCollected = GetDecimal(reader, "TotalCollected"),
                TotalDiscounted = GetDecimal(reader, "TotalDiscounted"),
                TotalRefunded = GetDecimal(reader, "TotalRefunded"),
                TotalTransactions = GetInt32(reader, "TotalTransactions"),
                PaymentMethod = GetNullableInt32(reader, "PaymentMethod"),
                IsDailySummary = GetBoolean(reader, "IsDailySummary"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

public class LateFeeRuleRepository : BaseRepository<LateFeeRule>, ILateFeeRuleRepository
{
    public LateFeeRuleRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<LateFeeRuleListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetLateFeeRulesPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<LateFeeRuleListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new LateFeeRuleListItemDto
            {
                Id = GetInt32(reader, "Id"),
                Name = GetString(reader, "Name"),
                GraceDays = GetInt32(reader, "GraceDays"),
                FeeType = GetInt32(reader, "FeeType"),
                FeeValue = GetDecimal(reader, "FeeValue"),
                MaxFee = GetDecimal(reader, "MaxFee"),
                SchoolClassId = GetNullableInt32(reader, "SchoolClassId"),
                ClassName = GetNullableString(reader, "ClassName"),
                FeeCategoryId = GetNullableInt32(reader, "FeeCategoryId"),
                FeeCategoryName = GetNullableString(reader, "FeeCategoryName"),
                IsActive = GetBoolean(reader, "IsActive"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

    public class FeeTypeRepository : BaseRepository<FeeType>, IFeeTypeRepository
    {
        public FeeTypeRepository(SchoolDbContext db) : base(db) { }

        public async Task<(List<FeeTypeListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
            int pageNumber, int pageSize, string? searchTerm, CancellationToken ct)
        {
            using var command = _db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "sp_GetFeeTypesPaged";
            command.CommandType = CommandType.StoredProcedure;
            AddParameter(command, "@PageNumber", pageNumber);
            AddParameter(command, "@PageSize", pageSize);
            AddParameter(command, "@SearchTerm", searchTerm);

            await using var lease = await OpenConnectionAsync(command.Connection!, ct);
            using var reader = await command.ExecuteReaderAsync(ct);
            var items = new List<FeeTypeListItemDto>();
            while (await reader.ReadAsync(ct))
            {
                items.Add(new FeeTypeListItemDto
                {
                    Id = GetInt32(reader, "Id"),
                    Name = GetString(reader, "Name"),
                    Description = GetNullableString(reader, "Description"),
                    DisplayOrder = GetInt32(reader, "DisplayOrder"),
                    IsActive = GetBoolean(reader, "IsActive"),
                    TotalRecords = GetInt32(reader, "TotalRecords")
                });
            }
            return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
        }
    }

    public class PaymentAllocationRepository : BaseRepository<PaymentAllocation>, IPaymentAllocationRepository
{
    public PaymentAllocationRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<PaymentAllocationListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? paymentId, int? feeInvoiceId, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetPaymentAllocationsPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);
        AddParameter(command, "@PaymentId", paymentId ?? 0);
        AddParameter(command, "@FeeInvoiceId", feeInvoiceId ?? 0);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<PaymentAllocationListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new PaymentAllocationListItemDto
            {
                Id = GetInt32(reader, "Id"),
                PaymentId = GetInt32(reader, "PaymentId"),
                PaymentReference = GetNullableString(reader, "PaymentReference"),
                FeeInvoiceId = GetInt32(reader, "FeeInvoiceId"),
                InvoiceNo = GetNullableString(reader, "InvoiceNo"),
                AllocatedAmount = GetDecimal(reader, "AllocatedAmount"),
                Remarks = GetNullableString(reader, "Remarks"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

public class ScholarshipRepository : BaseRepository<Scholarship>, IScholarshipRepository
{
    public ScholarshipRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<ScholarshipListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetScholarshipsPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<ScholarshipListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new ScholarshipListItemDto
            {
                Id = GetInt32(reader, "Id"),
                Name = GetString(reader, "Name"),
                Description = GetNullableString(reader, "Description"),
                DiscountType = GetInt32(reader, "DiscountType") == 0 ? "Percentage" : "Fixed",
                Value = GetDecimal(reader, "Value"),
                SchoolClassId = GetNullableInt32(reader, "SchoolClassId"),
                ClassName = GetNullableString(reader, "ClassName"),
                FeeCategoryId = GetNullableInt32(reader, "FeeCategoryId"),
                FeeCategoryName = GetNullableString(reader, "FeeCategoryName"),
                IsActive = GetBoolean(reader, "IsActive"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}

public class FineRuleRepository : BaseRepository<FineRule>, IFineRuleRepository
{
    public FineRuleRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FineRuleListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetFineRulesPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", searchTerm);

        await using var lease = await OpenConnectionAsync(command.Connection!, ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        var items = new List<FineRuleListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new FineRuleListItemDto
            {
                Id = GetInt32(reader, "Id"),
                Name = GetString(reader, "Name"),
                GraceDays = GetInt32(reader, "GraceDays"),
                FinePerDay = GetDecimal(reader, "FinePerDay"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }
}
