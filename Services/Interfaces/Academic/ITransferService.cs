using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface ITransferService
{
    Task<List<TransferCertificateListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
    Task<TransferCertificateUpsertDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(TransferCertificateUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(TransferCertificateUpsertDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);
    Task<int> ProcessTransferAsync(TransferCertificateUpsertDto dto, string createdBy, CancellationToken ct = default);
}
