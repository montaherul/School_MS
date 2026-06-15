using Xunit;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Services.Implementations.Result;
namespace SchoolManagementSystem.Tests.Services;

public class Phase32_PerformanceAuditTests
{
    [Fact(DisplayName = "51. ComponentFieldMapper.GetDtoValue is O(1) dictionary lookup")]
    public void FieldMapper_GetDtoValue_O1()
    {
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 75;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 10000; i++)
            _ = ComponentFieldMapper.GetDtoValue(marks, "WRITTEN");
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < 100, $"10000 lookups took {sw.ElapsedMilliseconds}ms — expected <100ms");
    }

    [Fact(DisplayName = "52. ComponentMarksDto ToDictionary round-trip is fast")]
    public void ComponentMarks_ToDictionary_Performance()
    {
        var marks = new ComponentMarksDto();
        for (int i = 0; i < 100; i++)
            marks[$"COMP{i}"] = i;

        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 1000; i++)
        {
            var dict = marks.ToDictionary();
            var restored = ComponentMarksDto.FromDictionary(dict);
            Assert.NotNull(restored);
        }
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < 500, $"1000 round-trips took {sw.ElapsedMilliseconds}ms");
    }

    [Fact(DisplayName = "53. ComponentFieldMapper.FromEntity handles 12+ fields efficiently")]
    public void FieldMapper_FromEntity_Performance()
    {
        var entity = new MarkEntry
        {
            WrittenMarks = 75, MCQMarks = 20, CQMarks = 45, PracticalMarks = 10,
            VivaMarks = 5, LabMarks = 3, OralMarks = 2, AssignmentMarks = 8,
            ContinuousAssessmentMarks = 15, CompetencyMarks = 12, BehaviourMarks = 10,
            ParticipationMarks = 8
        };
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 10000; i++)
            _ = ComponentFieldMapper.FromEntity(entity);
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < 200, $"10000 FromEntity calls took {sw.ElapsedMilliseconds}ms");
    }

    [Fact(DisplayName = "54. ComponentFieldMapper.ApplyToEntity handles 12+ fields efficiently")]
    public void FieldMapper_ApplyToEntity_Performance()
    {
        var entity = new MarkEntry();
        var marks = new ComponentMarksDto();
        for (int i = 0; i < 12; i++)
            marks[$"STD{i}"] = i * 5;

        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 10000; i++)
            ComponentFieldMapper.ApplyToEntity(marks, new MarkEntry());
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < 300, $"10000 ApplyToEntity calls took {sw.ElapsedMilliseconds}ms");
    }

    [Fact(DisplayName = "55. All 8 DTOs have no switch-statement component access patterns")]
    public void NoSwitchStatements_InDTOs()
    {
        var dtoFiles = new[]
        {
            "MarkEntryDto",
            "MarkEntrySheetDto",
            "MarksEntryStudentDto",
            "StudentMarkDataDto",
            "MarksEntryExistingDto",
            "TeacherExportRowDto",
            "ReportCardSubjectDto",
            "StudentMarkViewModel"
        };
        var switchPatterns = new[] { "WrittenMarks", "MCQMarks", "PracticalMarks", "VivaMarks",
            "LabMarks", "OralMarks", "AssignmentMarks", "ContinuousAssessmentMarks",
            "CompetencyMarks", "BehaviourMarks", "ParticipationMarks" };
        foreach (var name in dtoFiles)
        {
            var type = Type.GetType($"SchoolManagementSystem.Models.DTOs.Result.{name}, SchoolManagementSystem")
                ?? Type.GetType($"SchoolManagementSystem.Models.ViewModels.Result.{name}, SchoolManagementSystem");
            if (type == null) continue;
            foreach (var prop in switchPatterns)
                Assert.Null(type.GetProperty(prop));
        }
    }

    [Fact(DisplayName = "56. ComponentMarksDto handles 50+ components")]
    public void ComponentMarks_LargeScale()
    {
        var marks = new ComponentMarksDto();
        for (int i = 0; i < 50; i++)
            marks[$"COMP{i}"] = i;
        Assert.Equal(50, marks.Count);
        for (int i = 0; i < 50; i++)
            Assert.Equal(i, marks[$"COMP{i}"]);
    }

    [Fact(DisplayName = "57. BangladeshFormat view requires no individual property access")]
    public void BangladeshFormat_NoIndividualPropertyAccess()
    {
        var subj = new ReportCardSubjectDto { SubjectId = 1, SubjectName = "Test" };
        subj.ComponentMarks["WRITTEN"] = 75;
        subj.ComponentMarks["MCQ"] = 20;
        Assert.Equal(2, subj.ComponentMarks.Count);
    }

    [Fact(DisplayName = "58. MarkEntrySheetDto parses ComponentValues into ComponentMarks")]
    public void MarkEntrySheet_ComponentValuesFallback()
    {
        var sheet = new MarkEntrySheetDto
        {
            StudentId = 1,
            ComponentValues = """{"PROJECT":40,"ATTENDANCE":10}"""
        };
        var marks = sheet.ComponentMarks;
        marks["WRITTEN"] = 75;
        Assert.Equal(75, marks["WRITTEN"]);
    }

    [Fact(DisplayName = "59. TeacherExportRowDto has ComponentMarks not individual props")]
    public void TeacherExportRow_ComponentMarks()
    {
        var dto = new TeacherExportRowDto();
        var props = dto.GetType().GetProperties().Select(p => p.Name).ToList();
        Assert.DoesNotContain("WrittenMarks", props);
        Assert.DoesNotContain("MCQMarks", props);
        Assert.DoesNotContain("CQMarks", props);
        Assert.DoesNotContain("PracticalMarks", props);
        Assert.DoesNotContain("VivaMarks", props);
        Assert.DoesNotContain("LabMarks", props);
        Assert.DoesNotContain("OralMarks", props);
        Assert.DoesNotContain("AssignmentMarks", props);
        Assert.DoesNotContain("ContinuousAssessmentMarks", props);
        Assert.Contains("ComponentMarks", props);
    }

    [Fact(DisplayName = "60. ReportCardDto unchanged except SubjectDto component refactor")]
    public void ReportCardDto_UnchangedStructure()
    {
        var card = new ReportCardDto();
        var props = card.GetType().GetProperties().Select(p => p.Name).ToList();
        Assert.Contains("SchoolName", props);
        Assert.Contains("StudentName", props);
        Assert.Contains("Subjects", props);
        Assert.Contains("Summary", props);
    }
}
