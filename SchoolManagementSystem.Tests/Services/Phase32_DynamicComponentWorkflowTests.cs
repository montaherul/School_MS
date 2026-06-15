using Xunit;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Result;
using SchoolManagementSystem.Services.Implementations.Result;
namespace SchoolManagementSystem.Tests.Services;

public class Phase32_DynamicComponentWorkflowTests
{
    [Fact(DisplayName = "16. MarkEntryDto stores dynamic component via ComponentMarks")]
    public void MarkEntryDto_StoresDynamicComponent()
    {
        var dto = new MarkEntryDto
        {
            ExamId = 1,
            StudentId = 1,
            SubjectId = 1,
            TeacherId = 1
        };
        dto.ComponentMarks["WRITTEN"] = 75;
        dto.ComponentMarks["PROJECT"] = 40;
        Assert.Equal(75, dto.ComponentMarks["WRITTEN"]);
        Assert.Equal(40, dto.ComponentMarks["PROJECT"]);
    }

    [Fact(DisplayName = "17. StudentMarkDataDto stores dynamic components")]
    public void StudentMarkDataDto_StoresDynamicComponents()
    {
        var dto = new StudentMarkDataDto
        {
            StudentId = 1,
            StudentName = "Test Student"
        };
        dto.ComponentMarks["WRITTEN"] = 70;
        dto.ComponentMarks["MCQ"] = 20;
        Assert.Equal(70, dto.ComponentMarks["WRITTEN"]);
    }

    [Fact(DisplayName = "18. MarkEntrySheetDto round-trips ComponentMarks")]
    public void MarkEntrySheetDto_RoundTrips()
    {
        var sheet = new MarkEntrySheetDto
        {
            StudentId = 1,
            StudentName = "Test",
            RollNumber = 101
        };
        sheet.ComponentMarks["WRITTEN"] = 80;
        sheet.ComponentMarks["VIVA"] = 10;
        Assert.Equal(80, sheet.ComponentMarks["WRITTEN"]);
        Assert.Equal(10, sheet.ComponentMarks["VIVA"]);
    }

    [Fact(DisplayName = "19. MarksEntryStudentDto stores dynamic components")]
    public void MarksEntryStudentDto_StoresDynamicComponents()
    {
        var dto = new MarksEntryStudentDto
        {
            StudentId = 1,
            StudentName = "Test"
        };
        dto.ComponentMarks["CQ"] = 45;
        dto.ComponentMarks["ASSIGNMENT"] = 15;
        Assert.Equal(45, dto.ComponentMarks["CQ"]);
        Assert.Equal(15, dto.ComponentMarks["ASSIGNMENT"]);
    }

    [Fact(DisplayName = "20. MarksEntryExistingDto stores component marks")]
    public void MarksEntryExistingDto_StoresComponents()
    {
        var dto = new MarksEntryExistingDto
        {
            StudentId = 1,
            MarksObtained = 85
        };
        dto.ComponentMarks["WRITTEN"] = 60;
        dto.ComponentMarks["MCQ"] = 25;
        Assert.Equal(60, dto.ComponentMarks["WRITTEN"]);
        Assert.Equal(25, dto.ComponentMarks["MCQ"]);
    }

    [Fact(DisplayName = "21. TeacherExportRowDto stores component marks")]
    public void TeacherExportRowDto_StoresComponents()
    {
        var dto = new TeacherExportRowDto
        {
            StudentName = "Test",
            MarksObtained = 85
        };
        dto.ComponentMarks["WRITTEN"] = 60;
        dto.ComponentMarks["PRACTICAL"] = 25;
        Assert.Equal(60, dto.ComponentMarks["WRITTEN"]);
        Assert.Equal(25, dto.ComponentMarks["PRACTICAL"]);
    }

    [Fact(DisplayName = "22. ReportCardSubjectDto stores component marks")]
    public void ReportCardSubjectDto_StoresComponents()
    {
        var dto = new ReportCardSubjectDto
        {
            SubjectId = 1,
            SubjectName = "Bangla",
            MarksObtained = 85
        };
        dto.ComponentMarks["WRITTEN"] = 50;
        dto.ComponentMarks["MCQ"] = 25;
        dto.ComponentMarks["CQ"] = 10;
        Assert.Equal(50, dto.ComponentMarks["WRITTEN"]);
        Assert.Equal(25, dto.ComponentMarks["MCQ"]);
        Assert.Equal(10, dto.ComponentMarks["CQ"]);
    }

    [Fact(DisplayName = "23. StudentMarkViewModel stores component marks")]
    public void StudentMarkViewModel_StoresComponents()
    {
        var vm = new StudentMarkViewModel
        {
            StudentId = 1,
            StudentName = "Test"
        };
        vm.ComponentMarks["WRITTEN"] = 70;
        vm.ComponentMarks["LAB"] = 15;
        Assert.Equal(70, vm.ComponentMarks["WRITTEN"]);
        Assert.Equal(15, vm.ComponentMarks["LAB"]);
    }

    [Fact(DisplayName = "24. ComponentFieldMapper.GetDtoValue extracts from ComponentMarksDto")]
    public void FieldMapper_GetDtoValue_FromComponentMarks()
    {
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 80;
        var val = ComponentFieldMapper.GetDtoValue(marks, "WRITTEN");
        Assert.Equal(80, val);
    }

    [Fact(DisplayName = "25. ComponentFieldMapper.GetDtoValue extracts from MarkEntryDto")]
    public void FieldMapper_GetDtoValue_FromMarkEntryDto()
    {
        var dto = new MarkEntryDto { ExamId = 1 };
        dto.ComponentMarks["MCQ"] = 25;
        var val = ComponentFieldMapper.GetDtoValue(dto, "MCQ");
        Assert.Equal(25, val);
    }

    [Fact(DisplayName = "26. ComponentFieldMapper.ExtractConfiguredComponents extracts only specified codes")]
    public void FieldMapper_ExtractConfiguredComponents()
    {
        var entry = new MarkEntry
        {
            WrittenMarks = 75,
            MCQMarks = 20,
            PracticalMarks = 10
        };
        var components = new[] { ("WRITTEN", "Written"), ("PRACTICAL", "Practical") };
        var extracted = ComponentFieldMapper.ExtractConfiguredComponents(entry, components);
        Assert.Equal(75, extracted["WRITTEN"]);
        Assert.Equal(10, extracted["PRACTICAL"]);
        Assert.Null(extracted["MCQ"]); // Not in configured components
    }

    [Fact(DisplayName = "27. ComponentFieldMapper.ApplyToEntity returns null when no dynamic components")]
    public void FieldMapper_ApplyToEntity_NoDynamic()
    {
        var entity = new MarkEntry();
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 75;
        marks["MCQ"] = 20;
        var json = ComponentFieldMapper.ApplyToEntity(marks, entity);
        Assert.Null(json);
        Assert.Equal(75, entity.WrittenMarks);
        Assert.Equal(20, entity.MCQMarks);
    }

    [Fact(DisplayName = "28. ComponentFieldMapper.ComputeTotalFromDto with null components returns 0")]
    public void FieldMapper_ComputeTotalFromDto_NullComponents()
    {
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 75;
        var total = ComponentFieldMapper.ComputeTotalFromDto(marks, null);
        Assert.Equal(0, total);
    }

    [Fact(DisplayName = "29. ComponentFieldMapper.GetCodeToColumnMap returns all 12 standard mappings")]
    public void FieldMapper_GetCodeToColumnMap()
    {
        var map = ComponentFieldMapper.GetCodeToColumnMap();
        Assert.Equal(12, map.Count);
        Assert.Equal("WrittenMarks", map["WRITTEN"]);
        Assert.Equal("MCQMarks", map["MCQ"]);
        Assert.Equal("ParticipationMarks", map["PARTICIPATION"]);
    }

    [Fact(DisplayName = "30. ComponentMarksDto enumerates key-value pairs")]
    public void ComponentMarks_Enumerates()
    {
        var marks = new ComponentMarksDto();
        marks["WRITTEN"] = 75;
        marks["MCQ"] = 20;
        int count = 0;
        foreach (var kvp in marks)
        {
            count++;
            Assert.NotNull(kvp.Key);
        }
        Assert.Equal(2, count);
    }
}
