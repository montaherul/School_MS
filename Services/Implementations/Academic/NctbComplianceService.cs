using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class NctbComplianceService : INctbComplianceService
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INctbComplianceRepository _nctbRepo;

    public NctbComplianceService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor, INctbComplianceRepository nctbRepo)
    {
        _uow = uow;
        _httpContextAccessor = httpContextAccessor;
        _nctbRepo = nctbRepo;
    }

    public async Task<NctbComplianceReportDto> GetComplianceReportAsync(int academicYearId, CancellationToken ct = default)
    {
        var spData = await _nctbRepo.GetComplianceReportSpAsync(academicYearId, ct);

        var hasScience = spData.HasScienceGroup;
        var hasBusiness = spData.HasBusinessStudiesGroup;
        var hasHumanities = spData.HasHumanitiesGroup;
        var coreCount = spData.CoreSubjectCount;
        var electiveCount = spData.ElectiveSubjectCount;
        var vocationalCount = spData.VocationalSubjectCount;
        var religionCount = spData.ReligionSubjectCount;

        var checklist = new List<NctbChecklistItem>
        {
            new() { Id = "groups", Label = "Academic Groups", Description = "Science, Business Studies, Humanities groups configured", Passed = hasScience && hasBusiness && hasHumanities, Severity = "high" },
            new() { Id = "core", Label = "Core Subjects", Description = $"At least 6 core subjects (found {coreCount})", Passed = spData.HasCompulsoryCoreSubjects, Severity = "high" },
            new() { Id = "religion", Label = "Religion Subjects", Description = $"Both Islam & others (found {religionCount})", Passed = spData.HasAllReligionTypes, Severity = "medium" },
            new() { Id = "vocational", Label = "Vocational Subjects", Description = $"Vocational subjects configured (found {vocationalCount})", Passed = vocationalCount > 0, Severity = "medium" },
            new() { Id = "primary", Label = "Primary Classes (1-5)", Description = "Primary section configured", Passed = spData.HasPrimaryClasses, Severity = "high" },
            new() { Id = "secondary", Label = "Secondary Classes (6-10)", Description = "Secondary section configured", Passed = spData.HasSecondaryClasses, Severity = "high" },
            new() { Id = "elective", Label = "Elective Subjects", Description = $"Elective subjects available (found {electiveCount})", Passed = electiveCount > 0, Severity = "low" },
            new() { Id = "islam", Label = "Islamic Studies", Description = "Islamic Studies subject exists", Passed = spData.HasIslamicStudies, Severity = "medium" },
        };

        var passed = checklist.Count(c => c.Passed);
        var totalChecks = checklist.Count;
        var complianceScore = totalChecks > 0 ? Math.Round((double)passed / totalChecks * 100, 1) : 0;

        var missing = checklist.Where(c => !c.Passed).Select(c => c.Label).ToList();
        var recommendations = new List<string>();

        if (!hasScience) recommendations.Add("Configure Science group for class 9-10");
        if (!hasBusiness) recommendations.Add("Configure Business Studies group for class 9-10");
        if (!hasHumanities) recommendations.Add("Configure Humanities group for class 9-10");
        if (vocationalCount == 0) recommendations.Add("Add vocational subjects for NCTB compliance");
        if (!spData.HasCompulsoryCoreSubjects) recommendations.Add("Add more core subjects (minimum 6 required)");

        var splitSubjects = (string input) => string.IsNullOrWhiteSpace(input)
            ? new List<string>()
            : input.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();

        return new NctbComplianceReportDto
        {
            AcademicYearId = spData.AcademicYearId,
            AcademicYearName = spData.AcademicYearName,
            ComplianceScore = complianceScore,
            TotalChecks = totalChecks,
            PassedChecks = passed,
            HasScienceGroup = hasScience,
            HasBusinessStudiesGroup = hasBusiness,
            HasHumanitiesGroup = hasHumanities,
            HasCompulsoryCoreSubjects = spData.HasCompulsoryCoreSubjects,
            HasAllReligionTypes = spData.HasAllReligionTypes,
            VocationalSubjectCount = vocationalCount,
            TotalSubjectCount = spData.TotalSubjectCount,
            GroupCount = spData.GroupCount,
            ReligionSubjectCount = religionCount,
            MissingSubjects = missing,
            Warnings = new List<string>(),
            Recommendations = recommendations,
            Checklist = checklist,
            SubjectCategoryBreakdown = new List<SubjectCategoryBreakdown>
            {
                new() { Category = "Core", Count = coreCount, Subjects = splitSubjects(spData.CoreSubjectNames) },
                new() { Category = "Elective", Count = electiveCount, Subjects = splitSubjects(spData.ElectiveSubjectNames) },
                new() { Category = "Vocational", Count = vocationalCount, Subjects = splitSubjects(spData.VocationalSubjectNames) },
                new() { Category = "Religion", Count = religionCount, Subjects = splitSubjects(spData.ReligionSubjectNames) },
            }
        };
    }

    public async Task<List<CurriculumVersionDto>> GetCurriculumVersionsAsync(CancellationToken ct = default)
    {
        var versions = await _uow.Repository<CurriculumVersion>().Query().AsNoTracking()
            .Include(v => v.AcademicYear)
            .OrderByDescending(v => v.EffectiveFrom)
            .ToListAsync(ct);

        return versions.Select(v => new CurriculumVersionDto
        {
            Id = v.Id,
            VersionName = v.VersionName,
            AcademicYearId = v.AcademicYearId,
            AcademicYearName = v.AcademicYear?.Name ?? "",
            EffectiveFrom = v.EffectiveFrom,
            IsCurrent = v.IsCurrent,
            Description = v.Description
        }).ToList();
    }

    public async Task<CurriculumVersionDto> GetCurriculumVersionByIdAsync(int id, CancellationToken ct = default)
    {
        var v = await _uow.Repository<CurriculumVersion>().Query().AsNoTracking()
            .Include(x => x.AcademicYear)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (v is null) return null!;

        return new CurriculumVersionDto
        {
            Id = v.Id, VersionName = v.VersionName, AcademicYearId = v.AcademicYearId,
            AcademicYearName = v.AcademicYear?.Name ?? "", EffectiveFrom = v.EffectiveFrom,
            IsCurrent = v.IsCurrent, Description = v.Description
        };
    }

    public async Task<CurriculumVersionDto> CreateCurriculumVersionAsync(CurriculumVersionUpsertDto dto, CancellationToken ct = default)
    {
        var repo = _uow.Repository<CurriculumVersion>();
        var entity = new CurriculumVersion
        {
            VersionName = dto.VersionName,
            AcademicYearId = dto.AcademicYearId,
            EffectiveFrom = dto.EffectiveFrom,
            IsCurrent = dto.IsCurrent,
            Description = dto.Description
        };
        if (entity.IsCurrent) await ClearCurrentFlagAsync(ct);
        await repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        await LogAuditAsync("Created", "CurriculumVersion", entity.Id, null, dto.VersionName, ct);

        return await GetCurriculumVersionByIdAsync(entity.Id, ct);
    }

    public async Task<CurriculumVersionDto> UpdateCurriculumVersionAsync(int id, CurriculumVersionUpsertDto dto, CancellationToken ct = default)
    {
        var repo = _uow.Repository<CurriculumVersion>();
        var entity = await repo.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) throw new KeyNotFoundException($"CurriculumVersion with id {id} not found");

        var oldName = entity.VersionName;
        entity.VersionName = dto.VersionName;
        entity.AcademicYearId = dto.AcademicYearId;
        entity.EffectiveFrom = dto.EffectiveFrom;
        entity.IsCurrent = dto.IsCurrent;
        entity.Description = dto.Description;

        if (entity.IsCurrent) await ClearCurrentFlagAsync(ct);
        repo.Update(entity);
        await _uow.SaveChangesAsync(ct);

        await LogAuditAsync("Updated", "CurriculumVersion", id, oldName, dto.VersionName, ct);

        return await GetCurriculumVersionByIdAsync(id, ct);
    }

    public async Task<bool> DeleteCurriculumVersionAsync(int id, CancellationToken ct = default)
    {
        var repo = _uow.Repository<CurriculumVersion>();
        var entity = await repo.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        var versionName = entity.VersionName;
        entity.IsDeleted = true;
        repo.Update(entity);
        await _uow.SaveChangesAsync(ct);

        await LogAuditAsync("Deleted", "CurriculumVersion", id, versionName, null, ct);
        return true;
    }

    public async Task<List<CurriculumSubjectDto>> GetCurriculumSubjectsAsync(int curriculumVersionId, CancellationToken ct = default)
    {
        var subjects = await _uow.Repository<CurriculumSubject>().Query().AsNoTracking()
            .Include(x => x.Subject)
            .Where(x => x.CurriculumVersionId == curriculumVersionId && !x.IsDeleted)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(ct);

        return subjects.Select(x => new CurriculumSubjectDto
        {
            Id = x.Id,
            CurriculumVersionId = x.CurriculumVersionId,
            SubjectId = x.SubjectId,
            SubjectCode = x.SubjectCode,
            SubjectName = x.Subject?.Name ?? "",
            Category = x.Category,
            TotalHours = x.TotalHours,
            IsCompulsory = x.IsCompulsory,
            SortOrder = x.SortOrder
        }).ToList();
    }

    public async Task<CurriculumSubjectDto> AddSubjectToCurriculumAsync(CurriculumSubjectUpsertDto dto, CancellationToken ct = default)
    {
        var subject = await _uow.Repository<Subject>().FirstOrDefaultAsync(x => x.Id == dto.SubjectId && !x.IsDeleted, ct);
        if (subject is null) throw new KeyNotFoundException($"Subject with id {dto.SubjectId} not found");

        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

        var entity = new CurriculumSubject
        {
            CurriculumVersionId = dto.CurriculumVersionId,
            SubjectId = dto.SubjectId,
            SubjectCode = subject.Code,
            Category = dto.Category,
            TotalHours = dto.TotalHours,
            IsCompulsory = dto.IsCompulsory,
            SortOrder = dto.SortOrder,
            CreatedBy = userName
        };

        var repo = _uow.Repository<CurriculumSubject>();
        await repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        await LogAuditAsync("Created", "CurriculumSubject", entity.Id, null, $"{subject.Name} ({subject.Code})", ct);

        return new CurriculumSubjectDto
        {
            Id = entity.Id,
            CurriculumVersionId = entity.CurriculumVersionId,
            SubjectId = entity.SubjectId,
            SubjectCode = entity.SubjectCode,
            SubjectName = subject.Name,
            Category = entity.Category,
            TotalHours = entity.TotalHours,
            IsCompulsory = entity.IsCompulsory,
            SortOrder = entity.SortOrder
        };
    }

    public async Task<bool> RemoveSubjectFromCurriculumAsync(int id, CancellationToken ct = default)
    {
        var repo = _uow.Repository<CurriculumSubject>();
        var entity = await repo.Query()
            .Include(x => x.Subject)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

        if (entity is null) return false;

        var subjectName = entity.Subject?.Name ?? entity.SubjectCode;
        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

        entity.IsDeleted = true;
        entity.UpdatedBy = userName;
        repo.Update(entity);
        await _uow.SaveChangesAsync(ct);

        await LogAuditAsync("Deleted", "CurriculumSubject", id, subjectName, null, ct);
        return true;
    }

    private async Task LogAuditAsync(string action, string entity, int? entityId, string? oldValue, string? newValue, CancellationToken ct = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userIdStr = httpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        int? userId = userIdStr != null && int.TryParse(userIdStr, out var uid) ? uid : null;

        var details = entityId.HasValue
            ? $"[{entity}#{entityId}] {action}"
            : $"[{entity}] {action}";

        if (oldValue != null || newValue != null)
            details += $" | Old: {oldValue} | New: {newValue}";

        var log = new AuditLog
        {
            UserId = userId,
            Module = "Curriculum",
            Action = $"{entity}.{action}",
            IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
            Details = details.Length > 1000 ? details[..1000] : details,
            CreatedBy = httpContext?.User?.Identity?.Name ?? "system",
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<AuditLog>().AddAsync(log, ct);
        await _uow.SaveChangesAsync(ct);
    }

    private async Task ClearCurrentFlagAsync(CancellationToken ct = default)
    {
        var current = await _uow.Repository<CurriculumVersion>()
            .FirstOrDefaultAsync(x => x.IsCurrent && !x.IsDeleted, ct);
        if (current is not null)
        {
            current.IsCurrent = false;
            _uow.Repository<CurriculumVersion>().Update(current);
        }
    }
}
