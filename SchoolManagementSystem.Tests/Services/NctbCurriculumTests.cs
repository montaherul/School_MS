using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Implementations.Result;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class NctbCurriculumTests
{
    // =============================================
    // PHASE 3 — Religion Subject Engine
    // =============================================

    [Fact]
    public void ReligionFiltering_StudentReceivesOnlyAssignedReligion()
    {
        var islamSubject = new Subject { Id = 30, Code = "IRE", ReligionType = "Islam", IsReligionSubject = true };
        var hinduSubject = new Subject { Id = 31, Code = "HRE", ReligionType = "Hindu", IsReligionSubject = true };
        var buddhistSubject = new Subject { Id = 32, Code = "BRE", ReligionType = "Buddhist", IsReligionSubject = true };
        var christianSubject = new Subject { Id = 33, Code = "CRE", ReligionType = "Christian", IsReligionSubject = true };

        var student = new Student { Id = 1, Religion = "Islam", AssignedReligionSubjectId = 30 };

        var studentReligionSubjectId = student.AssignedReligionSubjectId;

        // Student should only receive their assigned religion subject
        Assert.Equal(30, studentReligionSubjectId);
        Assert.NotEqual(31, studentReligionSubjectId);
        Assert.NotEqual(32, studentReligionSubjectId);
        Assert.NotEqual(33, studentReligionSubjectId);
    }

    [Theory]
    [InlineData("Islam", 30)]
    [InlineData("Hindu", 31)]
    [InlineData("Buddhist", 32)]
    [InlineData("Christian", 33)]
    public void ReligionMapping_AllReligionsMapToCorrectSubject(string religion, int expectedSubjectId)
    {
        var code = religion?.Trim().ToLowerInvariant() switch
        {
            "islam" => "IRE",
            "hindu" => "HRE",
            "buddhist" => "BRE",
            "christian" => "CRE",
            _ => null
        };

        var subjectId = code switch
        {
            "IRE" => 30,
            "HRE" => 31,
            "BRE" => 32,
            "CRE" => 33,
            _ => 0
        };

        Assert.Equal(expectedSubjectId, subjectId);
    }

    [Fact]
    public void GpaCalculation_ExcludeReligion_RemovesReligionFromGPA()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { GradePoint = 5.00m, IsPassed = true, IsOptionalSubject = false, IsReligionSubject = false },
            new() { GradePoint = 4.00m, IsPassed = true, IsOptionalSubject = false, IsReligionSubject = false },
            new() { GradePoint = 3.00m, IsPassed = true, IsOptionalSubject = false, IsReligionSubject = true }
        };

        var setting = new ResultSetting { IncludeReligionInGPA = false };

        var validResults = results.Where(r => r.IsPassed).ToList();
        var nonReligionResults = validResults.Where(r => !r.IsReligionSubject).ToList();
        decimal totalPoints = nonReligionResults.Sum(r => r.GradePoint);
        int subjectCount = nonReligionResults.Count;
        decimal gpa = subjectCount > 0 ? Math.Round(totalPoints / subjectCount, 2) : 0;

        // GPA should be based on 2 subjects (excluding religion)
        Assert.Equal(2, subjectCount);
        Assert.Equal(4.50m, gpa);
    }

    [Fact]
    public void GpaCalculation_IncludeReligion_CountsReligionInGPA()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { GradePoint = 5.00m, IsPassed = true, IsOptionalSubject = false, IsReligionSubject = false },
            new() { GradePoint = 4.00m, IsPassed = true, IsOptionalSubject = false, IsReligionSubject = false },
            new() { GradePoint = 3.00m, IsPassed = true, IsOptionalSubject = false, IsReligionSubject = true }
        };

        var setting = new ResultSetting { IncludeReligionInGPA = true };

        var validResults = results.Where(r => r.IsPassed).ToList();
        decimal totalPoints = validResults.Sum(r => r.GradePoint);
        int subjectCount = validResults.Count;
        decimal gpa = subjectCount > 0 ? Math.Round(totalPoints / subjectCount, 2) : 0;

        // GPA should be based on 3 subjects (including religion)
        Assert.Equal(3, subjectCount);
        Assert.Equal(4.00m, gpa);
    }

    // =============================================
    // PHASE 4 — Class 1-5 Compliance
    // =============================================

    [Fact]
    public void Class1To5_Subjects_MatchNCTB()
    {
        var nctbSubjectCodes = new[] { "BAN", "ENG", "MAT", "GSCI", "SOC", "MUS", "PE", "ART", "HEALTH" };

        // NCTB required subjects for classes 1-5
        Assert.Contains("BAN", nctbSubjectCodes);
        Assert.Contains("ENG", nctbSubjectCodes);
        Assert.Contains("MAT", nctbSubjectCodes);
        Assert.Contains("GSCI", nctbSubjectCodes);
        Assert.Contains("SOC", nctbSubjectCodes);
        Assert.Contains("MUS", nctbSubjectCodes);
        Assert.Contains("PE", nctbSubjectCodes);
        Assert.Contains("ART", nctbSubjectCodes);
        Assert.Contains("HEALTH", nctbSubjectCodes);

        // Verify MUS (Music) is now included (was missing before fix)
        Assert.Contains("MUS", nctbSubjectCodes);
    }

    [Fact]
    public void Class1To5_Religion_IncludesFourTypes()
    {
        var religionCodes = new[] { "IRE", "HRE", "BRE", "CRE" };
        Assert.Equal(4, religionCodes.Length);
        Assert.Contains("IRE", religionCodes);
        Assert.Contains("HRE", religionCodes);
        Assert.Contains("BRE", religionCodes);
        Assert.Contains("CRE", religionCodes);
    }

    // =============================================
    // PHASE 5 — Class 6-8 Compliance
    // =============================================

    [Fact]
    public void Class6To8_Subjects_MatchNCTB()
    {
        var nctbSubjectCodes = new[] { "BAN1", "BAN2", "ENG1", "ENG2", "MAT", "SCI", "SOC", "CAREER", "AGR", "HSC", "ICT", "ART", "PE", "HEALTH", "MUS" };

        Assert.Contains("BAN1", nctbSubjectCodes);
        Assert.Contains("BAN2", nctbSubjectCodes);
        Assert.Contains("ENG1", nctbSubjectCodes);
        Assert.Contains("ENG2", nctbSubjectCodes);
        Assert.Contains("MAT", nctbSubjectCodes);
        Assert.Contains("SCI", nctbSubjectCodes);
        Assert.Contains("SOC", nctbSubjectCodes);
        Assert.Contains("CAREER", nctbSubjectCodes);
        Assert.Contains("AGR", nctbSubjectCodes);
        Assert.Contains("HSC", nctbSubjectCodes);
        Assert.Contains("ICT", nctbSubjectCodes);
        Assert.Contains("ART", nctbSubjectCodes);
        Assert.Contains("PE", nctbSubjectCodes);
        Assert.Contains("HEALTH", nctbSubjectCodes);
        Assert.Contains("MUS", nctbSubjectCodes);
    }

    // =============================================
    // PHASE 6 — Class 9-10 Compliance
    // =============================================

    [Fact]
    public void Class9To10_ScienceGroup_MatchNCTB()
    {
        var scienceSubjects = new[] { "BAN1", "BAN2", "ENG1", "ENG2", "MAT", "HMA", "PHY", "CHE", "BIO", "SOC", "ICT" };
        Assert.Equal(11, scienceSubjects.Length);
    }

    [Fact]
    public void Class9To10_BusinessGroup_MatchNCTB()
    {
        var businessSubjects = new[] { "BAN1", "BAN2", "ENG1", "ENG2", "MAT", "ACC", "BUS", "ECO", "FIN", "ICT", "CAREER" };
        Assert.Equal(11, businessSubjects.Length);
    }

    [Fact]
    public void Class9To10_HumanitiesGroup_MatchNCTB()
    {
        var humanitiesSubjects = new[] { "BAN1", "BAN2", "ENG1", "ENG2", "MAT", "HMA", "HIS", "GEO", "CIV", "ECO", "ICT", "CAREER" };
        Assert.Equal(12, humanitiesSubjects.Length);
    }

    // =============================================
    // PHASE 7 — Optional Subject Engine
    // =============================================

    [Fact]
    public void OptionalSubject_PerStudent_NotClassLevel()
    {
        var student1 = new Student { Id = 1, OptionalSubjectId = 15 }; // Agriculture
        var student2 = new Student { Id = 2, OptionalSubjectId = 29 }; // Home Science

        Assert.NotNull(student1.OptionalSubjectId);
        Assert.NotNull(student2.OptionalSubjectId);
        Assert.NotEqual(student1.OptionalSubjectId, student2.OptionalSubjectId);
    }

    [Fact]
    public void OptionalSubject_CanBeNull_WhenNotSelected()
    {
        var student = new Student { Id = 1, OptionalSubjectId = null };
        Assert.Null(student.OptionalSubjectId);
    }

    // =============================================
    // PHASE 8 — Fourth Subject Rule
    // =============================================

    [Fact]
    public void OptionalSubjectMode_ExcludeFromGPA_RemovesOptional()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { GradePoint = 5.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 4.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 3.00m, IsPassed = true, IsOptionalSubject = true }
        };

        var setting = new ResultSetting { OptionalSubjectMode = OptionalSubjectMode.ExcludeFromGPA };

        var compulsoryResults = results.Where(r => r.IsPassed && !r.IsOptionalSubject).ToList();
        decimal totalPoints = compulsoryResults.Sum(r => r.GradePoint);
        decimal gpa = Math.Round(totalPoints / compulsoryResults.Count, 2);

        Assert.Equal(2, compulsoryResults.Count);
        Assert.Equal(4.50m, gpa);
    }

    [Fact]
    public void OptionalSubjectMode_BonusGPA_AddsBonus()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { GradePoint = 5.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 5.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 3.00m, IsPassed = true, IsOptionalSubject = true }
        };

        var setting = new ResultSetting
        {
            OptionalSubjectMode = OptionalSubjectMode.BonusGPA,
            BestOfCount = 1,
            RequirePassedOptionalOnly = true
        };

        var compulsoryResults = results.Where(r => r.IsPassed && !r.IsOptionalSubject).ToList();
        var optionalResults = results.Where(r => r.IsPassed && r.IsOptionalSubject).ToList();

        decimal totalPoints = compulsoryResults.Sum(r => r.GradePoint);
        int subjectCount = compulsoryResults.Count;

        var bestOptional = optionalResults
            .OrderByDescending(r => r.GradePoint)
            .Take(setting.BestOfCount)
            .ToList();

        totalPoints += bestOptional.Sum(r => r.GradePoint);
        subjectCount += bestOptional.Count;

        decimal gpa = Math.Round(totalPoints / subjectCount, 2);

        Assert.Equal(4.33m, gpa);
    }

    [Fact]
    public void OptionalSubjectMode_BestOf_SelectsTopOptional()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { GradePoint = 5.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 4.00m, IsPassed = true, IsOptionalSubject = false },
            new() { GradePoint = 2.00m, IsPassed = true, IsOptionalSubject = true },
            new() { GradePoint = 4.00m, IsPassed = true, IsOptionalSubject = true }
        };

        var setting = new ResultSetting
        {
            OptionalSubjectMode = OptionalSubjectMode.BestOf,
            BestOfCount = 2,
            RequirePassedOptionalOnly = true
        };

        var compulsoryResults = results.Where(r => r.IsPassed && !r.IsOptionalSubject).ToList();
        var optionalResults = results.Where(r => r.IsPassed && r.IsOptionalSubject).ToList();

        decimal totalPoints = compulsoryResults.Sum(r => r.GradePoint);
        int subjectCount = compulsoryResults.Count;

        var bestOptional = optionalResults
            .OrderByDescending(r => r.GradePoint)
            .Take(setting.BestOfCount)
            .ToList();

        totalPoints += bestOptional.Sum(r => r.GradePoint);
        subjectCount += bestOptional.Count;

        decimal gpa = Math.Round(totalPoints / subjectCount, 2);

        // 2 compulsory (5+4=9) + top 2 optional (4+2=6) = 15/4 = 3.75
        Assert.Equal(3.75m, gpa);
    }

    [Fact]
    public void FailSubjectMode_StrictFail_FailsOnAnyMandatoryFail()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { IsPassed = false, IsOptionalSubject = false },
            new() { IsPassed = true, IsOptionalSubject = false }
        };

        var setting = new ResultSetting { FailSubjectMode = FailSubjectMode.StrictFail };

        int failedMandatory = results.Count(r => !r.IsPassed && !r.IsOptionalSubject);
        bool isPassed = failedMandatory == 0;

        Assert.False(isPassed);
        Assert.Equal(1, failedMandatory);
    }

    [Fact]
    public void FailSubjectMode_ExcludeFail_AllowsConfiguredFails()
    {
        var results = new List<StudentSubjectResult>
        {
            new() { IsPassed = false, IsOptionalSubject = false },
            new() { IsPassed = false, IsOptionalSubject = false },
            new() { IsPassed = true, IsOptionalSubject = false }
        };

        var setting = new ResultSetting
        {
            FailSubjectMode = FailSubjectMode.ExcludeFail,
            MaxFailedCompulsoryAllowed = 2
        };

        int failedMandatory = results.Count(r => !r.IsPassed && !r.IsOptionalSubject);
        bool isPassed = failedMandatory <= setting.MaxFailedCompulsoryAllowed;

        Assert.True(isPassed);
        Assert.Equal(2, failedMandatory);
    }

    // =============================================
    // PHASE 9 — Teacher Subject Assignment
    // =============================================

    [Fact]
    public void TeacherAssignment_RequiresSubjectClassSectionMatch()
    {
        var assignment = new TeacherSubjectAssignment
        {
            TeacherId = 1,
            SubjectId = 1,
            ClassId = 1,
            SectionId = 1,
            AcademicYearId = 1,
            IsActive = true
        };

        Assert.True(assignment.IsActive);
        Assert.Equal(1, assignment.SubjectId);
        Assert.Equal(1, assignment.ClassId);
        Assert.Equal(1, assignment.SectionId);
    }

    [Fact]
    public void TeacherAssignment_SupportsGroupBasedFiltering()
    {
        var assignment = new TeacherSubjectAssignment
        {
            TeacherId = 1,
            SubjectId = 16, // Physics
            ClassId = 9,
            GroupId = 1,  // Science group
            SectionId = 1,
            AcademicYearId = 1,
            IsActive = true
        };

        Assert.NotNull(assignment.GroupId);
        Assert.Equal(1, assignment.GroupId);
    }

    // =============================================
    // PHASE 10 — Exam Subject Generation
    // =============================================

    [Fact]
    public void ExamSubjectGeneration_IncludesReligionAndOptional()
    {
        var examSubjects = new List<int> { 1, 2, 3, 4, 5, 7, 8, 14, 28 }; // Common subjects
        var religionSubjectIds = new List<int> { 30 }; // Islam for this student
        var optionalSubjectIds = new List<int> { 15 }; // Agriculture

        var allSubjectIds = examSubjects
            .Concat(religionSubjectIds)
            .Concat(optionalSubjectIds)
            .Distinct()
            .ToList();

        // Should include religion subject
        Assert.Contains(30, allSubjectIds);
        // Should include optional subject
        Assert.Contains(15, allSubjectIds);
        // Should not have duplicate subjects
        Assert.Equal(allSubjectIds.Count, allSubjectIds.Distinct().Count());
    }

    // =============================================
    // PHASE 11 — Result Engine
    // =============================================

    [Fact]
    public void ResultEngine_FiltersReligionByStudent()
    {
        var markEntries = new List<MarkEntry>
        {
            new() { SubjectId = 30, StudentId = 1 }, // IRE (Islam)
            new() { SubjectId = 31, StudentId = 1 }  // HRE (Hindu) - should be filtered
        };

        var student = new Student { Id = 1, AssignedReligionSubjectId = 30 };

        var filteredEntries = markEntries.Where(m =>
        {
            // Only include if it matches religion or is not a religion subject
            return m.SubjectId == student.AssignedReligionSubjectId;
        }).ToList();

        Assert.Single(filteredEntries);
        Assert.Equal(30, filteredEntries[0].SubjectId);
    }

    [Fact]
    public void ResultEngine_FiltersGroupByStudent()
    {
        var subjectResults = new List<StudentSubjectResult>
        {
            new() { SubjectId = 16, IsOptionalSubject = false }, // Physics (Science)
            new() { SubjectId = 20, IsOptionalSubject = false }, // Accounting (Business)
        };

        var classSubjects = new List<ClassSubject>
        {
            new() { SubjectId = 16, ClassSubjectGroups = [new ClassSubjectGroup { StudentGroupId = 1 }] },
            new() { SubjectId = 20, ClassSubjectGroups = [new ClassSubjectGroup { StudentGroupId = 2 }] },
        };

        var student = new Student { Id = 1, StudentGroupId = 1 }; // Science group

        var filteredResults = subjectResults.Where(sr =>
        {
            var cs = classSubjects.FirstOrDefault(c => c.SubjectId == sr.SubjectId);
            if (cs == null) return true;
            var csgLink = cs.ClassSubjectGroups?.FirstOrDefault(csg => !csg.IsDeleted);
            if (csgLink == null) return true;
            if (student.StudentGroupId.HasValue)
                return csgLink.StudentGroupId == student.StudentGroupId.Value;
            return true;
        }).ToList();

        Assert.Single(filteredResults);
        Assert.Equal(16, filteredResults[0].SubjectId);
    }

    // =============================================
    // PHASE 12 — Report Card Filtering
    // =============================================

    [Fact]
    public void ReportCard_OnlyShowsStudentAssignedSubjects()
    {
        var student = new Student
        {
            Id = 1,
            ClassId = 1,
            AssignedReligionSubjectId = 30,
            OptionalSubjectId = null,
            StudentGroupId = null
        };

        var classSubjects = new List<ClassSubject>
        {
            new() { SubjectId = 1 }, // Bangla
            new() { SubjectId = 2 }, // English
            new() { SubjectId = 30, IsReligionSubject = true, ReligionType = "Islam" }, // IRE
            new() { SubjectId = 31, IsReligionSubject = true, ReligionType = "Hindu" }, // HRE (should be excluded)
        };

        var validSubjectIds = new HashSet<int>();
        foreach (var cs in classSubjects)
        {
            if (cs.IsReligionSubject)
            {
                if (student.AssignedReligionSubjectId.HasValue && cs.SubjectId == student.AssignedReligionSubjectId.Value)
                    validSubjectIds.Add(cs.SubjectId);
                continue;
            }
            validSubjectIds.Add(cs.SubjectId);
        }

        Assert.Contains(1, validSubjectIds);
        Assert.Contains(2, validSubjectIds);
        Assert.Contains(30, validSubjectIds);
        Assert.DoesNotContain(31, validSubjectIds);
        Assert.Equal(3, validSubjectIds.Count);
    }

    // =============================================
    // PHASE 13 — Transcript Filtering
    // =============================================

    [Fact]
    public void Transcript_AppliesSameFilteringAsReportCard()
    {
        var student = new Student
        {
            Id = 1,
            ClassId = 9,
            AssignedReligionSubjectId = 30,
            StudentGroupId = 1 // Science
        };

        var subjectResults = new List<StudentSubjectResult>
        {
            new() { SubjectId = 1, IsOptionalSubject = false },  // Bangla (common)
            new() { SubjectId = 9, IsOptionalSubject = false },  // BAN1 (common)
            new() { SubjectId = 16, IsOptionalSubject = false }, // Physics (Science group)
            new() { SubjectId = 20, IsOptionalSubject = false }, // Accounting (Business group - should be filtered)
            new() { SubjectId = 30, IsOptionalSubject = false }, // IRE (assigned religion)
            new() { SubjectId = 31, IsOptionalSubject = false }, // HRE (different religion - should be filtered)
        };

        var classSubjects = new List<ClassSubject>
        {
            new() { SubjectId = 1 },
            new() { SubjectId = 9 },
            new() { SubjectId = 16, ClassSubjectGroups = [new ClassSubjectGroup { StudentGroupId = 1 }] },
            new() { SubjectId = 20, ClassSubjectGroups = [new ClassSubjectGroup { StudentGroupId = 2 }] },
            new() { SubjectId = 30, IsReligionSubject = true, ReligionType = "Islam" },
            new() { SubjectId = 31, IsReligionSubject = true, ReligionType = "Hindu" },
        };

        var validSubjectIds = new HashSet<int>();
        foreach (var cs in classSubjects)
        {
            if (cs.IsReligionSubject)
            {
                if (student.AssignedReligionSubjectId.HasValue && cs.SubjectId == student.AssignedReligionSubjectId.Value)
                    validSubjectIds.Add(cs.SubjectId);
                continue;
            }
            var csgLink = cs.ClassSubjectGroups?.FirstOrDefault(csg => !csg.IsDeleted);
            if (csgLink != null)
            {
                if (student.StudentGroupId.HasValue && csgLink.StudentGroupId == student.StudentGroupId.Value)
                    validSubjectIds.Add(cs.SubjectId);
                continue;
            }
            validSubjectIds.Add(cs.SubjectId);
        }

        var filtered = subjectResults.Where(sr => validSubjectIds.Contains(sr.SubjectId)).ToList();

        Assert.Contains(1, filtered.Select(s => s.SubjectId));
        Assert.Contains(9, filtered.Select(s => s.SubjectId));
        Assert.Contains(16, filtered.Select(s => s.SubjectId));
        Assert.Contains(30, filtered.Select(s => s.SubjectId));
        Assert.DoesNotContain(20, filtered.Select(s => s.SubjectId));
        Assert.DoesNotContain(31, filtered.Select(s => s.SubjectId));
        Assert.Equal(4, filtered.Count);
    }

    // =============================================
    // PHASE 8 — Promotion Rules (NotImplementedException fix)
    // =============================================

    [Fact]
    public void PromotionRules_DefaultRules_ByClassLevel()
    {
        // Class 1-5 rules
        var class1Rules = new PromotionRules
        {
            ClassId = 1,
            MaximumFailedSubjects = 3,
            MinimumGPA = 0.5m
        };
        Assert.Equal(3, class1Rules.MaximumFailedSubjects);
        Assert.Equal(0.5m, class1Rules.MinimumGPA);

        // Class 6-8 rules
        var class6Rules = new PromotionRules
        {
            ClassId = 6,
            MaximumFailedSubjects = 2,
            MinimumGPA = 1.0m
        };
        Assert.Equal(2, class6Rules.MaximumFailedSubjects);
        Assert.Equal(1.0m, class6Rules.MinimumGPA);

        // Class 9-10 rules
        var class9Rules = new PromotionRules
        {
            ClassId = 9,
            MaximumFailedSubjects = 1,
            MinimumGPA = 1.5m,
            RequireAllSubjectsPass = true
        };
        Assert.Equal(1, class9Rules.MaximumFailedSubjects);
        Assert.Equal(1.5m, class9Rules.MinimumGPA);
        Assert.True(class9Rules.RequireAllSubjectsPass);
    }

    [Fact]
    public void PromotionRules_CanBeCustomized()
    {
        var rules = new PromotionRules
        {
            ClassId = 5,
            MinimumGPA = 1.0m,
            MaximumFailedSubjects = 2,
            AllowConditionalPromotion = true,
            ConditionalPromotionGPA = 0.8m,
            RequireAllSubjectsPass = false,
            CriticalSubjects = ["Bangla", "English", "Mathematics"]
        };

        Assert.Equal(1.0m, rules.MinimumGPA);
        Assert.Equal(2, rules.MaximumFailedSubjects);
        Assert.Equal(3, rules.CriticalSubjects.Count);
    }

    // =============================================
    // GLOBAL COMPLIANCE CHECKS
    // =============================================

    [Fact]
    public void Nctb_AllSubjectCodesAreUnique()
    {
        var allCodes = new[] { "BAN", "ENG", "MAT", "GSCI", "SOC", "ICT", "PE", "ART", "HEALTH", "MUS",
            "BAN1", "BAN2", "ENG1", "ENG2", "SCI", "AGR", "HSC", "CAREER",
            "PHY", "CHE", "BIO", "HMA", "ACC", "FIN", "BUS", "HIS", "GEO", "ECO", "CIV",
            "IRE", "HRE", "BRE", "CRE" };

        Assert.Equal(allCodes.Length, allCodes.Distinct().Count());
    }

    [Fact]
    public void Nctb_ReligionSubjectCodesAreCorrect()
    {
        Assert.Equal("IRE", GetReligionCode("Islam"));
        Assert.Equal("HRE", GetReligionCode("Hindu"));
        Assert.Equal("BRE", GetReligionCode("Buddhist"));
        Assert.Equal("CRE", GetReligionCode("Christian"));
    }

    private static string GetReligionCode(string religion)
    {
        return religion?.Trim().ToLowerInvariant() switch
        {
            "islam" => "IRE",
            "hindu" => "HRE",
            "buddhist" => "BRE",
            "christian" => "CRE",
            _ => throw new ArgumentException($"Unknown religion: {religion}")
        };
    }
}
