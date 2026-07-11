using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class LessonPlanService : ILessonPlanService
{
    private readonly IUnitOfWork _uow;
    private readonly IPdfGenerator _pdfGenerator;

    public LessonPlanService(IUnitOfWork uow, IPdfGenerator pdfGenerator)
    {
        _uow = uow;
        _pdfGenerator = pdfGenerator;
    }

    public async Task<PagedResult<LessonPlanListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var query = _uow.Repository<LessonPlan>().Query().AsNoTracking()
            .Include(l => l.SchoolClass)
            .Include(l => l.Subject)
            .Include(l => l.Teacher)
            .Where(l => !l.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLower();
            query = query.Where(l => l.Title.ToLower().Contains(lower));
        }

        var totalCount = await query.CountAsync(ct);
        var entities = await query.OrderByDescending(l => l.LessonDate)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);

        var items = entities.Select(l => new LessonPlanListItemDto
        {
            Id = l.Id,
            Title = l.Title,
            ClassName = l.SchoolClass?.Name ?? "",
            SubjectName = l.Subject?.Name ?? "",
            TeacherName = l.Teacher?.FullName ?? "",
            Status = l.Status,
            LessonDate = l.LessonDate,
            IsActive = l.IsActive,
            TotalRecords = totalCount
        }).ToList();

        return new PagedResult<LessonPlanListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<LessonPlanUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<LessonPlan>().FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, ct);
        if (entity is null) return null;
        return new LessonPlanUpsertDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Objectives = entity.Objectives,
            Materials = entity.Materials,
            Procedure = entity.Procedure,
            AssessmentMethod = entity.AssessmentMethod,
            DurationMinutes = entity.DurationMinutes,
            LessonDate = entity.LessonDate,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            TeacherId = entity.TeacherId,
            SchoolClassId = entity.SchoolClassId,
            SubjectId = entity.SubjectId,
            AcademicYearId = entity.AcademicYearId,
            Status = entity.Status,
            IsActive = entity.IsActive
        };
    }

    public async Task<int> CreateAsync(LessonPlanUpsertDto dto, string createdBy, CancellationToken ct = default)
    {
        var entity = new LessonPlan
        {
            Title = dto.Title.Trim(),
            Objectives = dto.Objectives,
            Materials = dto.Materials,
            Procedure = dto.Procedure,
            AssessmentMethod = dto.AssessmentMethod,
            DurationMinutes = dto.DurationMinutes,
            LessonDate = dto.LessonDate,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TeacherId = dto.TeacherId,
            SchoolClassId = dto.SchoolClassId,
            SubjectId = dto.SubjectId,
            AcademicYearId = dto.AcademicYearId,
            Status = dto.Status,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
        await _uow.Repository<LessonPlan>().AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(LessonPlanUpsertDto dto, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<LessonPlan>().FirstOrDefaultAsync(l => l.Id == dto.Id && !l.IsDeleted, ct)
            ?? throw new InvalidOperationException("Lesson Plan not found.");
        entity.Title = dto.Title.Trim();
        entity.Objectives = dto.Objectives;
        entity.Materials = dto.Materials;
        entity.Procedure = dto.Procedure;
        entity.AssessmentMethod = dto.AssessmentMethod;
        entity.DurationMinutes = dto.DurationMinutes;
        entity.LessonDate = dto.LessonDate;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.TeacherId = dto.TeacherId;
        entity.SchoolClassId = dto.SchoolClassId;
        entity.SubjectId = dto.SubjectId;
        entity.AcademicYearId = dto.AcademicYearId;
        entity.Status = dto.Status;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<LessonPlan>().FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, ct)
            ?? throw new InvalidOperationException("Lesson Plan not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task BulkActivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default)
    {
        if (ids == null || ids.Count == 0) return;
        await _uow.Repository<LessonPlan>().Query()
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsActive, true)
                .SetProperty(x => x.UpdatedBy, updatedBy)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow), ct);
    }

    public async Task BulkDeactivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default)
    {
        if (ids == null || ids.Count == 0) return;
        await _uow.Repository<LessonPlan>().Query()
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsActive, false)
                .SetProperty(x => x.UpdatedBy, updatedBy)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow), ct);
    }

    public async Task ToggleActiveAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<LessonPlan>().FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, ct)
            ?? throw new InvalidOperationException("Lesson Plan not found.");
        entity.IsActive = !entity.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<byte[]?> ExportPdfAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<LessonPlan>().Query().AsNoTracking()
            .Include(l => l.SchoolClass)
            .Include(l => l.Subject)
            .Include(l => l.Teacher)
            .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, ct);
        if (entity is null) return null;

        var html = $@"<html><head><meta charset='utf-8'><style>
body {{ font-family: 'Segoe UI', Arial, sans-serif; max-width: 800px; margin: 40px auto; padding: 20px; }}
h1 {{ color: #1a365d; border-bottom: 2px solid #3182ce; padding-bottom: 8px; }}
.label {{ font-weight: 600; color: #4a5568; width: 160px; display: inline-block; }}
.row {{ margin: 8px 0; }}
.section {{ margin: 16px 0; padding: 12px; background: #f7fafc; border-radius: 6px; }}
.section h2 {{ color: #2d3748; font-size: 16px; margin: 0 0 8px 0; }}
</style></head><body>
<h1>{System.Net.WebUtility.HtmlEncode(entity.Title)}</h1>
<div class='row'><span class='label'>Class:</span> {System.Net.WebUtility.HtmlEncode(entity.SchoolClass?.Name ?? "")}</div>
<div class='row'><span class='label'>Subject:</span> {System.Net.WebUtility.HtmlEncode(entity.Subject?.Name ?? "")}</div>
<div class='row'><span class='label'>Teacher:</span> {System.Net.WebUtility.HtmlEncode(entity.Teacher?.FullName ?? "")}</div>
<div class='row'><span class='label'>Date:</span> {entity.LessonDate:yyyy-MM-dd}</div>
<div class='row'><span class='label'>Duration:</span> {entity.DurationMinutes} min</div>
<div class='row'><span class='label'>Status:</span> {entity.Status}</div>
<div class='section'><h2>Objectives</h2>{System.Net.WebUtility.HtmlEncode(entity.Objectives ?? "")}</div>
<div class='section'><h2>Materials</h2>{System.Net.WebUtility.HtmlEncode(entity.Materials ?? "")}</div>
<div class='section'><h2>Procedure</h2>{System.Net.WebUtility.HtmlEncode(entity.Procedure ?? "")}</div>
<div class='section'><h2>Assessment</h2>{System.Net.WebUtility.HtmlEncode(entity.AssessmentMethod ?? "")}</div>
</body></html>";

        return _pdfGenerator.GenerateFromHtml(html);
    }
}
