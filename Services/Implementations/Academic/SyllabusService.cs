using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Files;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SyllabusService : ISyllabusService
{
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _fileStorage;
    private readonly IPdfGenerator _pdfGenerator;

    public SyllabusService(IUnitOfWork uow, IFileStorageService fileStorage, IPdfGenerator pdfGenerator)
    {
        _uow = uow;
        _fileStorage = fileStorage;
        _pdfGenerator = pdfGenerator;
    }

    public async Task<PagedResult<SyllabusListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var query = _uow.Repository<Syllabus>().Query().AsNoTracking()
            .Include(s => s.SchoolClass)
            .Include(s => s.Subject)
            .Include(s => s.AcademicYear)
            .Where(s => !s.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLower();
            query = query.Where(s => s.Title.ToLower().Contains(lower));
        }

        var totalCount = await query.CountAsync(ct);
        var entities = await query.OrderByDescending(s => s.UploadedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);

        var items = entities.Select(s => new SyllabusListItemDto
        {
            Id = s.Id,
            Title = s.Title,
            ClassName = s.SchoolClass?.Name ?? "",
            SubjectName = s.Subject?.Name ?? "",
            AcademicYearName = s.AcademicYear?.Name ?? "",
            FileName = s.FileName ?? "",
            FileSize = s.FileSize,
            UploadedBy = s.UploadedBy,
            UploadedAt = s.UploadedAt,
            IsActive = s.IsActive,
            TotalRecords = totalCount
        }).ToList();

        return new PagedResult<SyllabusListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<SyllabusUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<Syllabus>().FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);
        if (entity is null) return null;
        return new SyllabusUpsertDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            SchoolClassId = entity.SchoolClassId,
            SubjectId = entity.SubjectId,
            AcademicYearId = entity.AcademicYearId,
            IsActive = entity.IsActive,
            ExistingFileName = entity.FileName
        };
    }

    public async Task<int> CreateAsync(SyllabusUpsertDto dto, IFormFile? file, string createdBy, CancellationToken ct = default)
    {
        var entity = new Syllabus
        {
            Title = dto.Title.Trim(),
            Description = dto.Description,
            SchoolClassId = dto.SchoolClassId,
            SubjectId = dto.SubjectId,
            AcademicYearId = dto.AcademicYearId,
            UploadedBy = createdBy,
            UploadedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsActive = dto.IsActive
        };

        if (file is not null && file.Length > 0)
        {
            var path = await _fileStorage.SaveAsync(file, "syllabus", ct);
            entity.FilePath = path;
            entity.FileName = file.FileName;
            entity.FileSize = file.Length;
            entity.FileType = Path.GetExtension(file.FileName).TrimStart('.').ToLowerInvariant();
        }

        await _uow.Repository<Syllabus>().AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(SyllabusUpsertDto dto, IFormFile? file, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<Syllabus>().FirstOrDefaultAsync(s => s.Id == dto.Id && !s.IsDeleted, ct)
            ?? throw new InvalidOperationException("Syllabus not found.");

        entity.Title = dto.Title.Trim();
        entity.Description = dto.Description;
        entity.SchoolClassId = dto.SchoolClassId;
        entity.SubjectId = dto.SubjectId;
        entity.AcademicYearId = dto.AcademicYearId;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        if (file is not null && file.Length > 0)
        {
            var path = await _fileStorage.SaveAsync(file, "syllabus", ct);
            entity.FilePath = path;
            entity.FileName = file.FileName;
            entity.FileSize = file.Length;
            entity.FileType = Path.GetExtension(file.FileName).TrimStart('.').ToLowerInvariant();
        }

        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<Syllabus>().FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct)
            ?? throw new InvalidOperationException("Syllabus not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task BulkActivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default)
    {
        if (ids == null || ids.Count == 0) return;
        await _uow.Repository<Syllabus>().Query()
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsActive, true)
                .SetProperty(x => x.UpdatedBy, updatedBy)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow), ct);
    }

    public async Task BulkDeactivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default)
    {
        if (ids == null || ids.Count == 0) return;
        await _uow.Repository<Syllabus>().Query()
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsActive, false)
                .SetProperty(x => x.UpdatedBy, updatedBy)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow), ct);
    }

    public async Task ToggleActiveAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<Syllabus>().FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct)
            ?? throw new InvalidOperationException("Syllabus not found.");
        entity.IsActive = !entity.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<byte[]?> ExportPdfAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<Syllabus>().Query().AsNoTracking()
            .Include(s => s.SchoolClass)
            .Include(s => s.Subject)
            .Include(s => s.AcademicYear)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);
        if (entity is null) return null;

        var html = $@"<html><head><meta charset='utf-8'><style>
body {{ font-family: 'Segoe UI', Arial, sans-serif; max-width: 800px; margin: 40px auto; padding: 20px; }}
h1 {{ color: #1a365d; border-bottom: 2px solid #3182ce; padding-bottom: 8px; }}
.label {{ font-weight: 600; color: #4a5568; width: 140px; display: inline-block; }}
.row {{ margin: 8px 0; }}
</style></head><body>
<h1>{System.Net.WebUtility.HtmlEncode(entity.Title)}</h1>
<div class='row'><span class='label'>Class:</span> {System.Net.WebUtility.HtmlEncode(entity.SchoolClass?.Name ?? "")}</div>
<div class='row'><span class='label'>Subject:</span> {System.Net.WebUtility.HtmlEncode(entity.Subject?.Name ?? "")}</div>
<div class='row'><span class='label'>Academic Year:</span> {System.Net.WebUtility.HtmlEncode(entity.AcademicYear?.Name ?? "")}</div>
<div class='row'><span class='label'>Description:</span> {System.Net.WebUtility.HtmlEncode(entity.Description ?? "")}</div>
<div class='row'><span class='label'>File:</span> {System.Net.WebUtility.HtmlEncode(entity.FileName ?? "")}</div>
<div class='row'><span class='label'>Uploaded By:</span> {System.Net.WebUtility.HtmlEncode(entity.UploadedBy)}</div>
<div class='row'><span class='label'>Uploaded At:</span> {entity.UploadedAt:yyyy-MM-dd HH:mm}</div>
</body></html>";

        return _pdfGenerator.GenerateFromHtml(html);
    }

    public async Task<string?> GetFilePathAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<Syllabus>().Query().AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);
        return entity?.FilePath;
    }
}
