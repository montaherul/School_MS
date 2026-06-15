using Xunit;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.ViewModels.Result;
using SchoolManagementSystem.Services.Implementations.Result;
namespace SchoolManagementSystem.Tests.Services;

public class Phase32_ReportCardDynamicComponentTests
{
    [Fact(DisplayName = "41. ReportCardDto stores subjects with ComponentMarks")]
    public void ReportCardDto_HasSubjectsWithComponentMarks()
    {
        var card = new ReportCardDto
        {
            SchoolName = "Test School",
            StudentName = "Test Student",
            ExamName = "Half Yearly"
        };
        var subj = new ReportCardSubjectDto
        {
            SubjectId = 1,
            SubjectName = "Bangla",
            FullMarks = 100,
            MarksObtained = 85
        };
        subj.ComponentMarks["WRITTEN"] = 50;
        subj.ComponentMarks["MCQ"] = 25;
        subj.ComponentMarks["CQ"] = 10;
        card.Subjects.Add(subj);
        Assert.Single(card.Subjects);
        Assert.Equal(50, card.Subjects[0].ComponentMarks["WRITTEN"]);
    }

    [Fact(DisplayName = "42. ReportCardSubjectDto no longer has individual WrittenMarks property")]
    public void ReportCardSubject_NoIndividualProps()
    {
        var subj = new ReportCardSubjectDto
        {
            SubjectId = 1,
            SubjectName = "Physics",
            MarksObtained = 90
        };
        var props = subj.GetType().GetProperties().Select(p => p.Name).ToList();
        Assert.DoesNotContain("WrittenMarks", props);
        Assert.DoesNotContain("MCQMarks", props);
        Assert.DoesNotContain("PracticalMarks", props);
        Assert.DoesNotContain("MarksWritten", props);
        Assert.DoesNotContain("MarksMCQ", props);
        Assert.DoesNotContain("MarksPractical", props);
    }

    [Fact(DisplayName = "43. ReportCardSubjectDto uses ComponentMarks for all component data")]
    public void ReportCardSubject_ComponentMarksOnly()
    {
        var subj = new ReportCardSubjectDto();
        subj.ComponentMarks["WRITTEN"] = 65;
        subj.ComponentMarks["MCQ"] = 20;
        subj.ComponentMarks["PRACTICAL"] = 15;
        Assert.Equal(65, subj.ComponentMarks["WRITTEN"]);
        Assert.Equal(20, subj.ComponentMarks["MCQ"]);
        Assert.Equal(15, subj.ComponentMarks["PRACTICAL"]);
        Assert.Equal(3, subj.ComponentMarks.Count);
    }

    [Fact(DisplayName = "44. ReportCardDto subjects with varying components render dynamically")]
    public void ReportCard_VaryingComponentCounts()
    {
        var card = new ReportCardDto { SchoolName = "S", StudentName = "T" };
        var bangla = new ReportCardSubjectDto { SubjectId = 1, SubjectName = "Bangla", MarksObtained = 80 };
        bangla.ComponentMarks["CQ"] = 50;
        bangla.ComponentMarks["MCQ"] = 30;
        var english = new ReportCardSubjectDto { SubjectId = 2, SubjectName = "English", MarksObtained = 75 };
        english.ComponentMarks["WRITTEN"] = 45;
        english.ComponentMarks["MCQ"] = 20;
        english.ComponentMarks["ORAL"] = 10;
        card.Subjects.Add(bangla);
        card.Subjects.Add(english);

        var allCodes = card.Subjects.SelectMany(s => s.ComponentMarks.Keys).Distinct().ToList();
        Assert.Contains("CQ", allCodes);
        Assert.Contains("MCQ", allCodes);
        Assert.Contains("WRITTEN", allCodes);
        Assert.Contains("ORAL", allCodes);
        Assert.Equal(4, allCodes.Count);
    }

    [Fact(DisplayName = "45. BangladeshFormat view renders all component columns")]
    public void BangladeshFormat_RendersAllComponentColumns()
    {
        var subjects = new List<ReportCardSubjectDto>();
        var bangla = new ReportCardSubjectDto { SubjectId = 1, SubjectName = "Bangla" };
        bangla.ComponentMarks["CQ"] = 50;
        bangla.ComponentMarks["MCQ"] = 30;
        var physics = new ReportCardSubjectDto { SubjectId = 2, SubjectName = "Physics" };
        physics.ComponentMarks["WRITTEN"] = 45;
        physics.ComponentMarks["PRACTICAL"] = 25;
        physics.ComponentMarks["VIVA"] = 10;
        subjects.Add(bangla);
        subjects.Add(physics);

        var allComponentCodes = subjects.SelectMany(s => s.ComponentMarks.Keys).Distinct().OrderBy(c => c).ToList();
        Assert.Equal(5, allComponentCodes.Count);
        Assert.Contains("CQ", allComponentCodes);
        Assert.Contains("MCQ", allComponentCodes);
        Assert.Contains("WRITTEN", allComponentCodes);
        Assert.Contains("PRACTICAL", allComponentCodes);
        Assert.Contains("VIVA", allComponentCodes);
    }

    [Fact(DisplayName = "46. ReportCardSummaryDto unchanged by refactoring")]
    public void ReportCardSummary_Unchanged()
    {
        var summary = new ReportCardSummaryDto
        {
            TotalMarks = 450,
            TotalFullMarks = 600,
            Gpa = 4.50m,
            Grade = "A",
            IsPassed = true,
            Position = 1,
            PassedSubjectCount = 8,
            FailedSubjectCount = 0
        };
        Assert.Equal(450, summary.TotalMarks);
        Assert.Equal(4.50m, summary.Gpa);
        Assert.True(summary.IsPassed);
    }

    [Fact(DisplayName = "47. StudentMarkDataDto replaces Dictionary ComponentValues with ComponentMarks")]
    public void StudentMarkData_ComponentMarksReplacesComponentValues()
    {
        var d = new StudentMarkDataDto();
        var props = d.GetType().GetProperties().Select(p => p.Name).ToList();
        Assert.DoesNotContain("ComponentValues", props);
        Assert.Contains("ComponentMarks", props);
    }

    [Fact(DisplayName = "48. StudentMarkViewModel replaces Dictionary ComponentValues with ComponentMarks")]
    public void StudentMarkVM_ComponentMarksReplacesComponentValues()
    {
        var vm = new StudentMarkViewModel();
        var props = vm.GetType().GetProperties().Select(p => p.Name).ToList();
        Assert.DoesNotContain("ComponentValues", props);
        Assert.Contains("ComponentMarks", props);
    }

    [Fact(DisplayName = "49. MarkEntryDto no longer has individual WrittenMarks/MCQMarks props")]
    public void MarkEntryDto_NoIndividualProps()
    {
        var props = typeof(MarkEntryDto).GetProperties().Select(p => p.Name).ToList();
        Assert.DoesNotContain("WrittenMarks", props);
        Assert.DoesNotContain("MCQMarks", props);
        Assert.DoesNotContain("ComponentValues", props);
        Assert.Contains("ComponentMarks", props);
    }

    [Fact(DisplayName = "50. All 8 DTOs use ComponentMarksDto instead of individual props")]
    public void AllDTOs_UseComponentMarks()
    {
        var dtoTypes = new[]
        {
            typeof(MarkEntryDto),
            typeof(MarkEntrySheetDto),
            typeof(MarksEntryStudentDto),
            typeof(StudentMarkDataDto),
            typeof(MarksEntryExistingDto),
            typeof(TeacherExportRowDto),
            typeof(ReportCardSubjectDto),
            typeof(StudentMarkViewModel)
        };
        foreach (var type in dtoTypes)
        {
            var prop = type.GetProperty("ComponentMarks");
            Assert.NotNull(prop);
            Assert.Equal(typeof(ComponentMarksDto), prop.PropertyType);
        }
    }
}
