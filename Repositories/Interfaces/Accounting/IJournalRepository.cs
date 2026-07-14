using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.Repositories.Interfaces.Accounting;

public interface IJournalEntryRepository : IBaseRepository<JournalEntry>
{
    Task<(List<JournalEntryListItemDto> Items, int TotalRecords)> GetPagedAsync(int page, int pageSize, string? search, int? entryType, CancellationToken ct);
    Task<JournalEntryDetailDto?> GetDetailAsync(int id, CancellationToken ct);
    Task<string> GenerateJournalNoAsync(DateTime entryDate, CancellationToken ct);
    Task PostJournalEntryAsync(int journalEntryId, string postedBy, CancellationToken ct = default);
}
