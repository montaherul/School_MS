using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class ClassSubjectMappingSeeder
{
    private readonly SchoolDbContext _db;

    public ClassSubjectMappingSeeder(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var strategy = _db.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _db.Database.BeginTransactionAsync(cancellationToken);

            var classLookup = await _db.Classes
                .AsNoTracking()
                .ToDictionaryAsync(
                    c => c.SortOrder,
                    c => c.Id,
                    cancellationToken);

            var subjectLookup = await _db.Subjects
                .AsNoTracking()
                .ToDictionaryAsync(
                    s => s.Code.Trim().ToUpperInvariant(),
                    s => s.Id,
                    cancellationToken);

            var groupLookup = await _db.StudentGroups
                .AsNoTracking()
                .ToDictionaryAsync(
                    g => g.Name.Trim().ToUpperInvariant(),
                    g => g.Id,
                    cancellationToken);

            var mappings = BuildMappings(
                classLookup,
                subjectLookup,
                groupLookup);

            foreach (var mapping in mappings)
            {
                var exists = await _db.ClassSubjects.AnyAsync(x =>
                        !x.IsDeleted &&
                        x.SchoolClassId == mapping.SchoolClassId &&
                        x.SubjectId == mapping.SubjectId &&
                        x.StudentGroupId == mapping.StudentGroupId,
                    cancellationToken);

                if (!exists)
                {
                    _db.ClassSubjects.Add(mapping);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });
    }

    private static List<ClassSubject> BuildMappings(
        IReadOnlyDictionary<int, int> classLookup,
        IReadOnlyDictionary<string, int> subjectLookup,
        IReadOnlyDictionary<string, int> groupLookup)
    {
        var mappings = new List<ClassSubject>();

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
    new { Code = "REL", ReligionType = "Islam" },
    new { Code = "REL", ReligionType = "Hindu" },
    new { Code = "REL", ReligionType = "Buddhist" },
    new { Code = "REL", ReligionType = "Christian" }
};

        // =========================================
        // Classes 1–5
        // =========================================

        var class1To5Compulsory = new[]
        {
            "BAN",       // বাংলা
            "ENG",       // ইংরেজি
            "MAT",       // গণিত
            "SCI",   // প্রাথমিক বিজ্ঞান
            "SOC",       // বাংলাদেশ ও বিশ্ব পরিচয়
            "ICT",       // ICT
            "PE",        // শারীরিক শিক্ষা
            "ART"        // চারু ও কারুকলা
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
            "CAREER",    // কর্ম ও জীবনমুখী শিক্ষা
            "SOC"        // বাংলাদেশ ও বিশ্ব পরিচয়
        };

        // =========================================
        // Optional Subjects 6–8
        // =========================================

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

        var scienceGroupId =
            GetGroupId(groupLookup, "SCIENCE");

        var businessGroupId =
            GetGroupId(groupLookup, "BUSINESS STUDIES");

        var humanitiesGroupId =
            GetGroupId(groupLookup, "HUMANITIES");

        // =========================================
        // Science Group
        // =========================================

        var scienceSubjects = new[]
        {
            "BAN1",
            "BAN2",
            "ENG1",
            "ENG2",
            "MAT",
            "HMA",
            "PHY",
            "CHE",
            "BIO",
            "SOC",
            "ICT"
        };

        // =========================================
        // Business Studies Group
        // =========================================

        var businessSubjects = new[]
        {
            "BAN1",
            "BAN2",
            "ENG1",
            "ENG2",
            "MAT",
            "ACC",
            "BUS",
            "ECO",
            "FIN",
            "ICT",
            "CAREER"
        };

        // =========================================
        // Humanities Group
        // =========================================

        var humanitiesSubjects = new[]
        {
            "BAN1",
            "BAN2",
            "ENG1",
            "ENG2",
            "MAT",
            "HMA",
            "HIS",
            "GEO",
            "CIV",
            "ECO",
            "ICT",
            "CAREER"
        };

        foreach (var classId in new[] { 9, 10 })
        {
            // Science Group

            foreach (var subjectCode in scienceSubjects)
            {
                mappings.Add(
                    CreateMapping(
                        classIds[classId],
                        GetSubjectId(subjectLookup, subjectCode),
                        studentGroupId: scienceGroupId,
                        groupName: "Science"));
            }

            // Business Studies Group

            foreach (var subjectCode in businessSubjects)
            {
                mappings.Add(
                    CreateMapping(
                        classIds[classId],
                        GetSubjectId(subjectLookup, subjectCode),
                        studentGroupId: businessGroupId,
                        groupName: "Business Studies"));
            }

            // Humanities Group

            foreach (var subjectCode in humanitiesSubjects)
            {
                var isOptional = subjectCode == "HMA";

                mappings.Add(
                    CreateMapping(
                        classIds[classId],
                        GetSubjectId(subjectLookup, subjectCode),
                        studentGroupId: humanitiesGroupId,
                        groupName: "Humanities",
                        isOptional: isOptional));
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

        return mappings;
    }

    private static ClassSubject CreateMapping(
        int classId,
        int subjectId,
        int? studentGroupId = null,
        string? groupName = null,
        bool isOptional = false,
        bool isReligion = false,
        string? religionType = null)
    {
        return new ClassSubject
        {
            SchoolClassId = classId,
            SubjectId = subjectId,
            StudentGroupId = studentGroupId,
            GroupName = groupName,
            IsOptional = isOptional,
            IsMandatory = !isOptional,
            IsGroupSubject = studentGroupId.HasValue,
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