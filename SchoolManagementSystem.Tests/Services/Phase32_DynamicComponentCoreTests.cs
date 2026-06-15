using Xunit;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Services.Implementations.Result;
namespace SchoolManagementSystem.Tests.Services;

public class Phase32_DynamicComponentCoreTests
{
    [Fact(DisplayName = "1. ComponentMarksDto stores and retrieves values by code")]
    public void ComponentMarks_StoresAndRetrieves()
    {
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 75;
        marks["MCQ"] = 20;
        Assert.Equal(75, marks["WRITTEN"]);
        Assert.Equal(20, marks["MCQ"]);
        Assert.Null(marks["NONEXISTENT"]);
    }

    [Fact(DisplayName = "2. ComponentMarksDto case-insensitive key lookup")]
    public void ComponentMarks_CaseInsensitive()
    {
        var marks = new ComponentMarksDto();
        marks["written"] = 80;
        Assert.Equal(80, marks["WRITTEN"]);
        Assert.Equal(80, marks["Written"]);
    }

    [Fact(DisplayName = "3. ComponentMarksDto.ContainsKey works correctly")]
    public void ComponentMarks_ContainsKey()
    {
        var marks = new ComponentMarksDto();
        marks["CQ"] = 50;
        Assert.True(marks.ContainsKey("CQ"));
        Assert.False(marks.ContainsKey("UNKNOWN"));
    }

    [Fact(DisplayName = "4. ComponentMarksDto.Count tracks entries")]
    public void ComponentMarks_Count()
    {
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 75;
        marks["MCQ"] = 20;
        Assert.Equal(2, marks.Count);
    }

    [Fact(DisplayName = "5. ComponentMarksDto.Keys and Values enumeration")]
    public void ComponentMarks_KeysAndValues()
    {
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 75;
        marks["MCQ"] = 20;
        Assert.Contains("WRITTEN", marks.Keys);
        Assert.Contains(20m, marks.Values);
    }

    [Fact(DisplayName = "6. ComponentMarksDto.Clear removes all entries")]
    public void ComponentMarks_Clear()
    {
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 75;
        marks.Clear();
        Assert.Equal(0, marks.Count);
    }

    [Fact(DisplayName = "7. ComponentMarksDto.FromDictionary creates from existing dict")]
    public void ComponentMarks_FromDictionary()
    {
        var source = new Dictionary<string, decimal?> { ["WRITTEN"] = 75, ["MCQ"] = 20 };
        var marks = ComponentMarksDto.FromDictionary(source);
        Assert.Equal(75, marks["WRITTEN"]);
        Assert.Equal(20, marks["MCQ"]);
    }

    [Fact(DisplayName = "8. ComponentMarksDto.ToDictionary round-trips")]
    public void ComponentMarks_ToDictionary()
    {
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 75;
        var dict = marks.ToDictionary();
        Assert.Equal(75, dict["WRITTEN"]);
    }

    [Fact(DisplayName = "9. ComponentFieldMapper.FromEntity extracts all 12 standard fields")]
    public void FieldMapper_FromEntity_ExtractsStandardFields()
    {
        var entity = new MarkEntry
        {
            WrittenMarks = 75,
            MCQMarks = 20,
            CQMarks = 0,
            PracticalMarks = 10,
            VivaMarks = 5,
            LabMarks = 3,
            OralMarks = 2,
            AssignmentMarks = 8,
            ContinuousAssessmentMarks = 15,
            CompetencyMarks = 12,
            BehaviourMarks = 10,
            ParticipationMarks = 8
        };
        var marks = ComponentFieldMapper.FromEntity(entity);
        Assert.Equal(75, marks["WRITTEN"]);
        Assert.Equal(20, marks["MCQ"]);
        Assert.Equal(0, marks["CQ"]);
        Assert.Equal(10, marks["PRACTICAL"]);
        Assert.Equal(5, marks["VIVA"]);
        Assert.Equal(3, marks["LAB"]);
        Assert.Equal(2, marks["ORAL"]);
        Assert.Equal(8, marks["ASSIGNMENT"]);
        Assert.Equal(15, marks["CONTINUOUS_ASSESSMENT"]);
        Assert.Equal(12, marks["COMPETENCY"]);
        Assert.Equal(10, marks["BEHAVIOUR"]);
        Assert.Equal(8, marks["PARTICIPATION"]);
    }

    [Fact(DisplayName = "10. ComponentFieldMapper.FromEntity includes dynamic ComponentValues JSON")]
    public void FieldMapper_FromEntity_IncludesDynamicValues()
    {
        var entity = new MarkEntry
        {
            WrittenMarks = 75,
            ComponentValues = """{"PROJECT":40,"ATTENDANCE":10}"""
        };
        var marks = ComponentFieldMapper.FromEntity(entity);
        Assert.Equal(75, marks["WRITTEN"]);
        Assert.Equal(40, marks["PROJECT"]);
        Assert.Equal(10, marks["ATTENDANCE"]);
    }

    [Fact(DisplayName = "11. ComponentFieldMapper.ApplyToEntity sets standard fields and returns dynamic JSON")]
    public void FieldMapper_ApplyToEntity()
    {
        var entity = new MarkEntry();
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 75;
        marks["PROJECT"] = 40;
        var json = ComponentFieldMapper.ApplyToEntity(marks, entity);
        Assert.Equal(75, entity.WrittenMarks);
        Assert.Null(entity.MCQMarks);
        Assert.NotNull(json);
        Assert.Contains("PROJECT", json);
    }

    [Fact(DisplayName = "12. ComponentFieldMapper.ApplyToEntity all 12 standard fields")]
    public void FieldMapper_ApplyToEntity_AllStandard()
    {
        var entity = new MarkEntry();
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 75;
        marks["MCQ"] = 20;
        marks["CQ"] = 50;
        marks["PRACTICAL"] = 10;
        marks["VIVA"] = 5;
        marks["LAB"] = 3;
        marks["ORAL"] = 2;
        marks["ASSIGNMENT"] = 8;
        marks["CONTINUOUS_ASSESSMENT"] = 15;
        marks["COMPETENCY"] = 12;
        marks["BEHAVIOUR"] = 10;
        marks["PARTICIPATION"] = 8;
        ComponentFieldMapper.ApplyToEntity(marks, entity);
        Assert.Equal(75, entity.WrittenMarks);
        Assert.Equal(20, entity.MCQMarks);
        Assert.Equal(50, entity.CQMarks);
        Assert.Equal(10, entity.PracticalMarks);
        Assert.Equal(5, entity.VivaMarks);
        Assert.Equal(3, entity.LabMarks);
        Assert.Equal(2, entity.OralMarks);
        Assert.Equal(8, entity.AssignmentMarks);
        Assert.Equal(15, entity.ContinuousAssessmentMarks);
        Assert.Equal(12, entity.CompetencyMarks);
        Assert.Equal(10, entity.BehaviourMarks);
        Assert.Equal(8, entity.ParticipationMarks);
    }

    [Fact(DisplayName = "13. ComponentFieldMapper.SerializeDynamicComponents serializes only non-standard fields")]
    public void FieldMapper_SerializeDynamicComponents()
    {
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 75;
        marks["PROJECT"] = 40;
        marks["ATTENDANCE"] = 10;
        var json = ComponentFieldMapper.SerializeDynamicComponents(marks);
        Assert.NotNull(json);
        Assert.DoesNotContain("WRITTEN", json);
        Assert.Contains("PROJECT", json);
        Assert.Contains("ATTENDANCE", json);
    }

    [Fact(DisplayName = "14. ComponentFieldMapper.ComputeTotalFromDto sums configured components")]
    public void FieldMapper_ComputeTotalFromDto()
    {
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 50;
        marks["MCQ"] = 25;
        marks["PRACTICAL"] = 10;
        var components = new[] { ("WRITTEN", "Written"), ("MCQ", "MCQ"), ("PRACTICAL", "Practical") };
        var total = ComponentFieldMapper.ComputeTotalFromDto(marks, components);
        Assert.Equal(85, total);
    }

    [Fact(DisplayName = "15. ComponentFieldMapper.IsStandardField returns true for mapped codes")]
    public void FieldMapper_IsStandardField()
    {
        Assert.True(ComponentFieldMapper.IsStandardField("WRITTEN"));
        Assert.True(ComponentFieldMapper.IsStandardField("MCQ"));
        Assert.False(ComponentFieldMapper.IsStandardField("PROJECT"));
        Assert.False(ComponentFieldMapper.IsStandardField("ATTENDANCE"));
    }
}
