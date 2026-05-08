using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFeeInvoiceService
{
    Task<PagedResult<FeeInvoiceListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? studentId = null, CancellationToken cancellationToken = default);
    Task<FeeInvoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(FeeInvoice invoice, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(FeeInvoice invoice, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}

public class FeeInvoiceListItemDto
{
    public int Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public SchoolManagementSystem.Models.Enums.PaymentStatus Status { get; set; }
    public int TotalRecords { get; set; }
}
