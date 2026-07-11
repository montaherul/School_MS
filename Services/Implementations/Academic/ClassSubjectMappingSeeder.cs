using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class ClassSubjectMappingSeeder
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<ClassSubjectMappingSeeder> _logger;

    public ClassSubjectMappingSeeder(
        IUnitOfWork uow,
        ILogger<ClassSubjectMappingSeeder> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var classRepo = _uow.Repository<SchoolClass>();
            var subjectRepo = _uow.Repository<Subject>();
            var groupRepo = _uow.Repository<StudentGroup>();

            // 1. Safety Check: Ensure core data exists to prevent dictionary errors in BuildMappings
            var classCount = await classRepo.CountAsync(x => !x.IsDeleted, cancellationToken);
            var subjectCount = await subjectRepo.CountAsync(x => !x.IsDeleted, cancellationToken);

            if (classCount == 0 || subjectCount == 0)
            {
                _logger.LogWarning("Skipping ClassSubject mapping seed because Classes or Subjects are not yet seeded.");
                return;
            }

            await _uow.ExecuteInTransactionAsync(async () =>
            {
                var classSubjectRepo = _uow.Repository<ClassSubject>();
                var csgRepo = _uow.Repository<ClassSubjectGroup>();

                // 2. Data Lookups
                var classLookup = await classRepo.Query().AsNoTracking().ToDictionaryAsync(c => c.SortOrder, c => c.Id, cancellationToken);
                var subjectLookup = await subjectRepo.Query().AsNoTracking().ToDictionaryAsync(s => s.Code.Trim().ToUpperInvariant(), s => s.Id, cancellationToken);
                var groupLookup = await groupRepo.Query().AsNoTracking().ToDictionaryAsync(g => g.Name.Trim().ToUpperInvariant(), g => g.Id, cancellationToken);

                // Build mappings without group FK; track group links separately
                var (mappings, groupLinks) = BuildMappings(classLookup, subjectLookup, groupLookup);

                // 3. Optimization: Pre-fetch existing mappings to avoid N+1 AnyAsync calls
                var existingList = await classSubjectRepo.Query()
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Select(x => new { x.SchoolClassId, x.SubjectId })
                    .ToListAsync(cancellationToken);
                var existingSet = new HashSet<(int SchoolClassId, int SubjectId)>(
                    existingList.Select(x => (x.SchoolClassId, x.SubjectId))
                );

                bool added = false;
                foreach (var mapping in mappings)
                {
                    if (!existingSet.Contains((mapping.SchoolClassId, mapping.SubjectId)))
                    {
                        await classSubjectRepo.AddAsync(mapping, cancellationToken);
                        added = true;
                    }
                }

                if (added)
                {
                    await _uow.SaveChangesAsync(cancellationToken);

                    // Create junction records for group-specific mappings
                    var savedMappings = await classSubjectRepo.Query()
                        .AsNoTracking()
                        .Where(x => !x.IsDeleted)
                        .Select(x => new { x.Id, x.SchoolClassId, x.SubjectId })
                        .ToListAsync(cancellationToken);

                    var savedLookup = savedMappings.ToDictionary(x => (x.SchoolClassId, x.SubjectId), x => x.Id);

                    var csgList = await csgRepo.Query()
                        .AsNoTracking()
                        .Where(x => !x.IsDeleted)
                        .Select(x => new { x.ClassSubjectId, x.StudentGroupId })
                        .ToListAsync(cancellationToken);
                    var existingCsgSet = new HashSet<(int ClassSubjectId, int StudentGroupId)>(
                        csgList.Select(x => (x.ClassSubjectId, x.StudentGroupId))
                    );

                    foreach (var (classId, subjectId, groupId) in groupLinks)
                    {
                        if (savedLookup.TryGetValue((classId, subjectId), out var csId) && !existingCsgSet.Contains((csId, groupId)))
                        {
                            await csgRepo.AddAsync(new ClassSubjectGroup { ClassSubjectId = csId, StudentGroupId = groupId, CreatedBy = "system" }, cancellationToken);
                        }
                    }
                    await _uow.SaveChangesAsync(cancellationToken);
                }
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during ClassSubject mapping seed.");
        }
    }

    private static (List<ClassSubject> Mappings, List<(int ClassId, int SubjectId, int GroupId)> GroupLinks) BuildMappings(
        IReadOnlyDictionary<int, int> classLookup,
        IReadOnlyDictionary<string, int> subjectLookup,
        IReadOnlyDictionary<string, int> groupLookup)
    {
        var mappings = new List<ClassSubject>();
        var groupLinks = new List<(int ClassId, int SubjectId, int GroupId)>();

        var classIds = new Dictionary<int, int>();

        for (var i = 1; i <= 10; i++)
        {
            if (!classLookup.TryGetValue(i, out var classId))
            {
                throw new InvalidOperationException(
                    $"Class sort order {i} not found.");
            }

            classIds[i] = classId;
        }

        // =========================================
        // Religion Subjects
        // =========================================
        var religionSubjects = new[]
        {
            new { Code = "IRE", ReligionType = "Islam" },
            new { Code = "HRE", ReligionType = "Hindu" },
            new { Code = "BRE", ReligionType = "Buddhist" },
            new { Code = "CRE", ReligionType = "Christian" }
        };

        // =========================================
        // Classes 1–5
        // =========================================

        var class1To5Compulsory = new[]
        {
            "BAN",       // বাংলা
            "ENG",       // ইংরেজি
            "MAT",       // গণিত
            "GSCI",      // প্রাথমিক বিজ্ঞান
            "SOC",       // বাংলাদেশ ও বিশ্ব পরিচয়
            "ICT",       // ICT
            "PE",        // শারীরিক শিক্ষা
            "ART",       // চারু ও কারুকলা
            "HEALTH",    // শারীরিক শিক্ষা, স্বাস্থ্য ও খেলাধুলা
            "MUS"        // সঙ্গীত (NCTB compulsory)
        };

        foreach (var classId in new[] { 1, 2, 3, 4, 5 })
        {
            foreach (var subjectCode in class1To5Compulsory)
            {
                mappings.Add(
                    CreateMapping(
                        classIds[classId],
                        GetSubjectId(subjectLookup, subjectCode)));
            }

            foreach (var religion in religionSubjects)
            {
                mappings.Add(
                    CreateMapping(
                        classIds[classId],
                        GetSubjectId(subjectLookup, religion.Code),
                        isReligion: true,
                        religionType: religion.ReligionType));
            }
        }

        // =========================================
        // Classes 6–8
        // =========================================

        var class6To8Compulsory = new[]
        {
            "BAN1",      // বাংলা ১ম
            "BAN2",      // বাংলা ২য়
            "ENG1",      // ইংরেজি ১ম
            "ENG2",      // ইংরেজি ২য়
            "MAT",       // গণিত
            "PE",        // শারীরিক শিক্ষা
            "ICT",       // ICT
            "ART",       // চারু ও কারুকলা
            "HEALTH",    // শারীরিক শিক্ষা, স্বাস্থ্য ও খেলাধুলা
            "MUS",       // সঙ্গীত
            "CAREER",    // কর্ম ও জীবনমুখী শিক্ষা
            "SOC",       // বাংলাদেশ ও বিশ্ব পরিচয়
            "SCI"        // বিজ্ঞান (Class 6-8)
        };

        var optionalSubjects6To8 = new[]
        {
            "AGR", // কৃষিশিক্ষা
            "HSC"  // গার্হস্থ্য বিজ্ঞান
        };

        foreach (var classId in new[] { 6, 7, 8 })
        {
            foreach (var subjectCode in class6To8Compulsory)
            {
                mappings.Add(
                    CreateMapping(
                        classIds[classId],
                        GetSubjectId(subjectLookup, subjectCode)));
            }

            foreach (var religion in religionSubjects)
            {
                mappings.Add(
                    CreateMapping(
                        classIds[classId],
                        GetSubjectId(subjectLookup, religion.Code),
                        isReligion: true,
                        religionType: religion.ReligionType));
            }

            foreach (var optionalSubject in optionalSubjects6To8)
            {
                mappings.Add(
                    CreateMapping(
                        classIds[classId],
                        GetSubjectId(subjectLookup, optionalSubject),
                        isOptional: true));
            }
        }

        // =========================================
        // Student Groups
        // =========================================

        var scienceGroupId = GetGroupId(groupLookup, "SCIENCE");
        var businessGroupId = GetGroupId(groupLookup, "BusinessStudies");
        var humanitiesGroupId = GetGroupId(groupLookup, "HUMANITIES");

        // =========================================
        // Science Group
        // =========================================

        var scienceSubjects = new[]
        {
            "BAN1", "BAN2", "ENG1", "ENG2", "MAT",
            "HMA", "PHY", "CHE", "BIO", "SOC", "ICT"
        };

        // =========================================
        // Business Studies Group
        // =========================================

        var businessSubjects = new[]
        {
            "BAN1", "BAN2", "ENG1", "ENG2", "MAT",
            "ACC", "BUS", "ECO", "FIN", "ICT", "CAREER"
        };

        // =========================================
        // Humanities Group
        // =========================================

        var humanitiesSubjects = new[]
        {
            "BAN1", "BAN2", "ENG1", "ENG2", "MAT",
            "HMA", "HIS", "GEO", "CIV", "ECO", "ICT", "CAREER"
        };

        foreach (var classId in new[] { 9, 10 })
        {
            // Science Group
            foreach (var subjectCode in scienceSubjects)
            {
                var subId = GetSubjectId(subjectLookup, subjectCode);
                var mapping = CreateMapping(classIds[classId], subId, groupName: "Science", isOptional: subjectCode == "HMA");
                mappings.Add(mapping);
                // Capture group link key — will be resolved via subject code after save
                groupLinks.Add((classIds[classId], subId, scienceGroupId));
            }

            // Business Studies Group
            foreach (var subjectCode in businessSubjects)
            {
                var subId = GetSubjectId(subjectLookup, subjectCode);
                mappings.Add(CreateMapping(classIds[classId], subId, groupName: "BusinessStudies"));
                groupLinks.Add((classIds[classId], subId, businessGroupId));
            }

            // Humanities Group
            foreach (var subjectCode in humanitiesSubjects)
            {
                var subId = GetSubjectId(subjectLookup, subjectCode);
                mappings.Add(CreateMapping(classIds[classId], subId, groupName: "Humanities", isOptional: subjectCode == "HMA"));
                groupLinks.Add((classIds[classId], subId, humanitiesGroupId));
            }

            // Religion Subjects
            foreach (var religion in religionSubjects)
            {
                mappings.Add(
                    CreateMapping(
                        classIds[classId],
                        GetSubjectId(subjectLookup, religion.Code),
                        isReligion: true,
                        religionType: religion.ReligionType));
            }
        }

        return (mappings, groupLinks);
    }

    private static ClassSubject CreateMapping(
        int classId,
        int subjectId,
        string? groupName = null,
        bool isOptional = false,
        bool isReligion = false,
        string? religionType = null)
    {
        return new ClassSubject
        {
            SchoolClassId = classId,
            SubjectId = subjectId,
            GroupName = groupName,
            IsOptional = isOptional,
            IsMandatory = !isOptional,
            IsReligionSubject = isReligion,
            ReligionType = religionType,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow
        };
    }

    private static int GetSubjectId(
        IReadOnlyDictionary<string, int> subjectLookup,
        string subjectCode)
    {
        if (!subjectLookup.TryGetValue(
                subjectCode.Trim().ToUpperInvariant(),
                out var subjectId))
        {
            throw new InvalidOperationException(
                $"Subject code '{subjectCode}' not found.");
        }

        return subjectId;
    }

    private static int GetGroupId(
        IReadOnlyDictionary<string, int> groupLookup,
        string groupName)
    {
        if (!groupLookup.TryGetValue(
                groupName.Trim().ToUpperInvariant(),
                out var groupId))
        {
            throw new InvalidOperationException(
                $"Student group '{groupName}' not found.");
        }

        return groupId;
    }
}
