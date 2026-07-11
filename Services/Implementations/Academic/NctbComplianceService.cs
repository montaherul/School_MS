using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class NctbComplianceService : INctbComplianceService
{
    private readonly IUnitOfWork _uow;

    public NctbComplianceService(IUnitOfWork uow) { _uow = uow; }

    public async Task<NctbComplianceReportDto> GetComplianceReportAsync(int academicYearId, CancellationToken ct = default)
    {
        var academicYear = await _uow.Repository<AcademicYear>().FirstOrDefaultAsync(y => y.Id == academicYearId, ct);
        var yearName = academicYear?.Name ?? "Unknown";

        var activeSubjects = await _uow.Repository<Subject>().ListAsync(x => !x.IsDeleted && x.IsActive, ct);
        var groups = await _uow.Repository<StudentGroup>().ListAsync(x => !x.IsDeleted && x.IsActive, ct);
        var classes = await _uow.Repository<SchoolClass>().ListAsync(x => !x.IsDeleted && x.IsActive, ct);

        var hasScience = groups.Any(g => g.Name.Contains("Science", StringComparison.OrdinalIgnoreCase));
        var hasBusiness = groups.Any(g => g.Name.Contains("Business", StringComparison.OrdinalIgnoreCase));
        var hasHumanities = groups.Any(g => g.Name.Contains("Humanities", StringComparison.OrdinalIgnoreCase));

        var coreCount = activeSubjects.Count(s => s.Category == "Core");
        var electiveCount = activeSubjects.Count(s => s.Category == "Elective");
        var vocationalCount = activeSubjects.Count(s => s.Category == "Vocational");
        var religionCount = activeSubjects.Count(s => s.IsReligionSubject);

        var hasCompulsoryCore = coreCount >= 6;
        var hasAllReligion = religionCount >= 2;

        var checklist = new List<NctbChecklistItem>
        {
            new() { Id = "groups", Label = "Academic Groups", Description = "Science, Business Studies, Humanities groups configured", Passed = hasScience && hasBusiness && hasHumanities, Severity = "high" },
            new() { Id = "core", Label = "Core Subjects", Description = $"At least 6 core subjects (found {coreCount})", Passed = hasCompulsoryCore, Severity = "high" },
            new() { Id = "religion", Label = "Religion Subjects", Description = $"Both Islam & others (found {religionCount})", Passed = hasAllReligion, Severity = "medium" },
            new() { Id = "vocational", Label = "Vocational Subjects", Description = $"Vocational subjects configured (found {vocationalCount})", Passed = vocationalCount > 0, Severity = "medium" },
            new() { Id = "primary", Label = "Primary Classes (1-5)", Description = $"Primary section configured", Passed = classes.Any(c => c.Name.StartsWith("1") || c.Name.StartsWith("2") || c.Name.StartsWith("3") || c.Name.StartsWith("4") || c.Name.StartsWith("5")), Severity = "high" },
            new() { Id = "secondary", Label = "Secondary Classes (6-10)", Description = $"Secondary section configured", Passed = classes.Any(c => c.Name.StartsWith("6") || c.Name.StartsWith("7") || c.Name.StartsWith("8") || c.Name.StartsWith("9") || c.Name.StartsWith("10")), Severity = "high" },
            new() { Id = "elective", Label = "Elective Subjects", Description = $"Elective subjects available (found {electiveCount})", Passed = electiveCount > 0, Severity = "low" },
            new() { Id = "islam", Label = "Islamic Studies", Description = "Islamic Studies subject exists", Passed = activeSubjects.Any(s => s.IsReligionSubject && (s.ReligionType ?? "").Contains("Islam", StringComparison.OrdinalIgnoreCase)), Severity = "medium" },
        };

        var passed = checklist.Count(c => c.Passed);
        var totalChecks = checklist.Count;
        var complianceScore = totalChecks > 0 ? Math.Round((double)passed / totalChecks * 100, 1) : 0;

        var missing = checklist.Where(c => !c.Passed).Select(c => c.Label).ToList();
        var warnings = new List<string>();
        var recommendations = new List<string>();

        if (!hasScience) recommendations.Add("Configure Science group for class 9-10");
        if (!hasBusiness) recommendations.Add("Configure Business Studies group for class 9-10");
        if (!hasHumanities) recommendations.Add("Configure Humanities group for class 9-10");
        if (vocationalCount == 0) recommendations.Add("Add vocational subjects for NCTB compliance");
        if (!hasCompulsoryCore) recommendations.Add("Add more core subjects (minimum 6 required)");

        return new NctbComplianceReportDto
        {
            AcademicYearId = academicYearId,
            AcademicYearName = yearName,
            ComplianceScore = complianceScore,
            TotalChecks = totalChecks,
            PassedChecks = passed,
            HasScienceGroup = hasScience,
            HasBusinessStudiesGroup = hasBusiness,
            HasHumanitiesGroup = hasHumanities,
            HasCompulsoryCoreSubjects = hasCompulsoryCore,
            HasAllReligionTypes = hasAllReligion,
            VocationalSubjectCount = vocationalCount,
            TotalSubjectCount = activeSubjects.Count,
            GroupCount = groups.Count,
            ReligionSubjectCount = religionCount,
            MissingSubjects = missing,
            Warnings = warnings,
            Recommendations = recommendations,
            Checklist = checklist,
            SubjectCategoryBreakdown = new List<SubjectCategoryBreakdown>
            {
                new() { Category = "Core", Count = coreCount, Subjects = activeSubjects.Where(s => s.Category == "Core").Select(s => s.Name).ToList() },
                new() { Category = "Elective", Count = electiveCount, Subjects = activeSubjects.Where(s => s.Category == "Elective").Select(s => s.Name).ToList() },
                new() { Category = "Vocational", Count = vocationalCount, Subjects = activeSubjects.Where(s => s.Category == "Vocational").Select(s => s.Name).ToList() },
                new() { Category = "Religion", Count = religionCount, Subjects = activeSubjects.Where(s => s.IsReligionSubject).Select(s => s.Name).ToList() },
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

        return await GetCurriculumVersionByIdAsync(entity.Id, ct);
    }

    public async Task<CurriculumVersionDto> UpdateCurriculumVersionAsync(int id, CurriculumVersionUpsertDto dto, CancellationToken ct = default)
    {
        var repo = _uow.Repository<CurriculumVersion>();
        var entity = await repo.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) throw new KeyNotFoundException($"CurriculumVersion with id {id} not found");

        entity.VersionName = dto.VersionName;
        entity.AcademicYearId = dto.AcademicYearId;
        entity.EffectiveFrom = dto.EffectiveFrom;
        entity.IsCurrent = dto.IsCurrent;
        entity.Description = dto.Description;

        if (entity.IsCurrent) await ClearCurrentFlagAsync(ct);
        repo.Update(entity);
        await _uow.SaveChangesAsync(ct);

        return await GetCurriculumVersionByIdAsync(id, ct);
    }

    public async Task<bool> DeleteCurriculumVersionAsync(int id, CancellationToken ct = default)
    {
        var repo = _uow.Repository<CurriculumVersion>();
        var entity = await repo.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        entity.IsDeleted = true;
        repo.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
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
