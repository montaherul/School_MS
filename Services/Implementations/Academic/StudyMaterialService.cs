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

public class StudyMaterialService : IStudyMaterialService
{
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _fileStorage;
    private readonly IPdfGenerator _pdfGenerator;

    public StudyMaterialService(IUnitOfWork uow, IFileStorageService fileStorage, IPdfGenerator pdfGenerator)
    {
        _uow = uow;
        _fileStorage = fileStorage;
        _pdfGenerator = pdfGenerator;
    }

    public async Task<PagedResult<StudyMaterialListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var query = _uow.Repository<StudyMaterial>().Query().AsNoTracking()
            .Include(m => m.SchoolClass)
            .Include(m => m.Subject)
            .Where(m => !m.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLower();
            query = query.Where(m => m.Title.ToLower().Contains(lower) || m.MaterialType.ToLower().Contains(lower));
        }

        var totalCount = await query.CountAsync(ct);
        var entities = await query.OrderByDescending(m => m.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);

        var items = entities.Select(m => new StudyMaterialListItemDto
        {
            Id = m.Id,
            Title = m.Title,
            ClassName = m.SchoolClass?.Name ?? "",
            SubjectName = m.Subject?.Name ?? "",
            MaterialType = m.MaterialType,
            FileName = m.FileName ?? "",
            FileSize = m.FileSize,
            IsActive = m.IsActive,
            TotalRecords = totalCount
        }).ToList();

        return new PagedResult<StudyMaterialListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<StudyMaterialUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<StudyMaterial>().FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, ct);
        if (entity is null) return null;
        return new StudyMaterialUpsertDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            MaterialType = entity.MaterialType,
            ExternalUrl = entity.ExternalUrl,
            SchoolClassId = entity.SchoolClassId,
            SubjectId = entity.SubjectId,
            AcademicYearId = entity.AcademicYearId,
            IsActive = entity.IsActive,
            ExistingFileName = entity.FileName
        };
    }

    public async Task<int> CreateAsync(StudyMaterialUpsertDto dto, IFormFile? file, string createdBy, CancellationToken ct = default)
    {
        var entity = new StudyMaterial
        {
            Title = dto.Title.Trim(),
            Description = dto.Description,
            MaterialType = dto.MaterialType,
            ExternalUrl = dto.ExternalUrl,
            SchoolClassId = dto.SchoolClassId,
            SubjectId = dto.SubjectId,
            AcademicYearId = dto.AcademicYearId,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        if (file is not null && file.Length > 0)
        {
            var path = await _fileStorage.SaveAsync(file, "studymaterial", ct);
            entity.FilePath = path;
            entity.FileName = file.FileName;
            entity.FileSize = file.Length;
            entity.FileType = Path.GetExtension(file.FileName).TrimStart('.').ToLowerInvariant();
        }

        await _uow.Repository<StudyMaterial>().AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(StudyMaterialUpsertDto dto, IFormFile? file, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<StudyMaterial>().FirstOrDefaultAsync(m => m.Id == dto.Id && !m.IsDeleted, ct)
            ?? throw new InvalidOperationException("Study Material not found.");

        entity.Title = dto.Title.Trim();
        entity.Description = dto.Description;
        entity.MaterialType = dto.MaterialType;
        entity.ExternalUrl = dto.ExternalUrl;
        entity.SchoolClassId = dto.SchoolClassId;
        entity.SubjectId = dto.SubjectId;
        entity.AcademicYearId = dto.AcademicYearId;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        if (file is not null && file.Length > 0)
        {
            var path = await _fileStorage.SaveAsync(file, "studymaterial", ct);
            entity.FilePath = path;
            entity.FileName = file.FileName;
            entity.FileSize = file.Length;
            entity.FileType = Path.GetExtension(file.FileName).TrimStart('.').ToLowerInvariant();
        }

        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<StudyMaterial>().FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, ct)
            ?? throw new InvalidOperationException("Study Material not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task BulkActivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default)
    {
        if (ids == null || ids.Count == 0) return;
        await _uow.Repository<StudyMaterial>().Query()
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsActive, true)
                .SetProperty(x => x.UpdatedBy, updatedBy)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow), ct);
    }

    public async Task BulkDeactivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default)
    {
        if (ids == null || ids.Count == 0) return;
        await _uow.Repository<StudyMaterial>().Query()
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsActive, false)
                .SetProperty(x => x.UpdatedBy, updatedBy)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow), ct);
    }

    public async Task ToggleActiveAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<StudyMaterial>().FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, ct)
            ?? throw new InvalidOperationException("Study Material not found.");
        entity.IsActive = !entity.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<string?> GetFilePathAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<StudyMaterial>().Query().AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, ct);
        return entity?.FilePath;
    }

    public async Task<byte[]?> ExportPdfAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<StudyMaterial>().Query().AsNoTracking()
            .Include(m => m.SchoolClass)
            .Include(m => m.Subject)
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, ct);
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
<div class='row'><span class='label'>Type:</span> {entity.MaterialType}</div>
<div class='row'><span class='label'>Description:</span> {System.Net.WebUtility.HtmlEncode(entity.Description ?? "")}</div>
<div class='row'><span class='label'>File:</span> {System.Net.WebUtility.HtmlEncode(entity.FileName ?? "")}</div>
{(string.IsNullOrEmpty(entity.ExternalUrl) ? "" : $"<div class='row'><span class='label'>URL:</span> {System.Net.WebUtility.HtmlEncode(entity.ExternalUrl)}</div>")}
</body></html>";

        return _pdfGenerator.GenerateFromHtml(html);
    }
}
