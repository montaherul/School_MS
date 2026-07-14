using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Services.Implementations.Accounting;

public class JournalEntryService : IJournalEntryService
{
    private readonly IUnitOfWork _uow;
    private readonly IJournalEntryRepository _repo;
    private readonly IFinancePostingService _postingService;

    public JournalEntryService(IUnitOfWork uow, IJournalEntryRepository repo, IFinancePostingService postingService)
    {
        _uow = uow;
        _repo = repo;
        _postingService = postingService;
    }

    public async Task<PagedResult<JournalEntryListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? entryType, CancellationToken ct)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, total) = await _repo.GetPagedAsync(page, pageSize, search, entryType, ct);
        return new PagedResult<JournalEntryListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<JournalEntryDetailDto?> GetDetailAsync(int id, CancellationToken ct)
        => await _repo.GetDetailAsync(id, ct);

    public async Task<JournalEntryUpsertDto?> GetForEditAsync(int id, CancellationToken ct)
    {
        var detail = await _repo.GetDetailAsync(id, ct);
        if (detail is null || detail.IsPosted) return null;

        return new JournalEntryUpsertDto
        {
            Id = detail.Id,
            JournalNo = detail.JournalNo,
            EntryDate = detail.EntryDate,
            EntryType = Enum.Parse<JournalEntryType>(detail.EntryType),
            Description = detail.Description,
            FinancialPeriodId = detail.FinancialPeriodId,
            Lines = detail.Lines.Select(l => new JournalLineDto
            {
                AccountId = l.AccountId,
                LineType = l.LineType == "Debit" ? JournalLineType.Debit : JournalLineType.Credit,
                Amount = l.Amount,
                Narration = l.Narration
            }).ToList()
        };
    }

    private const int MaxRetries = 3;

    public async Task<int> CreateAsync(JournalEntryUpsertDto dto, string createdBy, CancellationToken ct)
    {
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                if (attempt > 1)
                {
                    dto.JournalNo = await _repo.GenerateJournalNoAsync(dto.EntryDate, ct);
                }
                return await CreateInternalAsync(dto, createdBy, ct);
            }
            catch (DbUpdateException ex) when (attempt < MaxRetries && ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                continue;
            }
        }
        throw new InvalidOperationException("Failed to create journal entry due to concurrent journal number generation. Please try again.");
    }

    private async Task<int> CreateInternalAsync(JournalEntryUpsertDto dto, string createdBy, CancellationToken ct)
    {
        var existing = await _uow.Repository<JournalEntry>()
            .FirstOrDefaultAsync(x => x.JournalNo == dto.JournalNo && !x.IsDeleted, ct);
        if (existing != null)
            throw new InvalidOperationException($"Journal number '{dto.JournalNo}' already exists.");

        var entry = new JournalEntry
        {
            CreatedBy = createdBy,
            JournalNo = dto.JournalNo,
            EntryDate = dto.EntryDate,
            EntryType = dto.EntryType,
            Description = dto.Description,
            FinancialPeriodId = dto.FinancialPeriodId,
            ReferenceId = dto.ReferenceId,
            ReferenceType = dto.ReferenceType,
            IsPosted = false
        };
        await _uow.Repository<JournalEntry>().AddAsync(entry, ct);
        await _uow.SaveChangesAsync(ct);

        foreach (var line in dto.Lines)
        {
            var entryLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = line.AccountId,
                LineType = line.LineType,
                Amount = line.Amount,
                Narration = line.Narration
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(entryLine, ct);
        }
        await _uow.SaveChangesAsync(ct);

        return entry.Id;
    }

    public async Task UpdateAsync(JournalEntryUpsertDto dto, string updatedBy, CancellationToken ct)
    {
        var entry = await _uow.Repository<JournalEntry>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted && !x.IsPosted, ct);
        if (entry is null) return;

        entry.EntryDate = dto.EntryDate;
        entry.EntryType = dto.EntryType;
        entry.Description = dto.Description;
        entry.FinancialPeriodId = dto.FinancialPeriodId;
        entry.UpdatedAt = DateTime.UtcNow;
        entry.UpdatedBy = updatedBy;

        // Remove old lines
        var oldLines = await _uow.Repository<JournalEntryLine>().ListAsync(l => l.JournalEntryId == dto.Id, ct);
        foreach (var l in oldLines)
        {
            l.IsDeleted = true;
            l.UpdatedAt = DateTime.UtcNow;
            l.UpdatedBy = updatedBy;
            _uow.Repository<JournalEntryLine>().Update(l);
        }

        // Add new lines
        foreach (var line in dto.Lines)
        {
            var entryLine = new JournalEntryLine
            {
                CreatedBy = updatedBy,
                JournalEntryId = entry.Id,
                AccountId = line.AccountId,
                LineType = line.LineType,
                Amount = line.Amount,
                Narration = line.Narration
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(entryLine, ct);
        }

        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct)
    {
        var entry = await _uow.Repository<JournalEntry>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted && !x.IsPosted, ct);
        if (entry is null) return;
        entry.IsDeleted = true;
        entry.UpdatedAt = DateTime.UtcNow;
        entry.UpdatedBy = updatedBy;
        _uow.Repository<JournalEntry>().Update(entry);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task PostAsync(int id, string postedBy, CancellationToken ct)
    {
        await _repo.PostJournalEntryAsync(id, postedBy, ct);
    }
}
