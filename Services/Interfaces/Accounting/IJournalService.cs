using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Accounting;

public interface IJournalEntryService
{
    Task<PagedResult<JournalEntryListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? entryType, CancellationToken ct = default);
    Task<JournalEntryDetailDto?> GetDetailAsync(int id, CancellationToken ct = default);
    Task<JournalEntryUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(JournalEntryUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(JournalEntryUpsertDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);
    Task PostAsync(int id, string postedBy, CancellationToken ct = default);
}
