using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using System.Text.Json;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Models.ViewModels.Exam;

public static class ExamViewModelMapper
{
    public static AcademicYearOptionViewModel ToOption(AcademicYear year) => new()
    {
        Id = year.Id,
        Name = year.Name,
        IsActive = year.IsActive
    };

    public static ExamFilterOptionViewModel ToFilterOption(StudentGroup group) => new()
    {
        Id = group.Id,
        Name = group.Name
    };

    public static ExamFilterOptionViewModel ToClassFilterOption(SchoolClass cls) => new()
    {
        Id = cls.Id,
        Name = cls.Name
    };

    public static ExamFilterOptionViewModel ToSectionFilterOption(Section section) => new()
    {
        Id = section.Id,
        Name = section.Name
    };

    public static ExamListDto ToListDto(ExamEntity exam, string academicYearName = "") => new()
    {
        Id = exam.Id,
        Name = exam.Name,
        Term = exam.Term,
        StartsOn = exam.StartsOn,
        EndsOn = exam.EndsOn,
        Status = exam.Status,
        AcademicYearId = exam.AcademicYearId,
        AcademicYearName = academicYearName,
        StudentGroupId = exam.StudentGroupId,
        IsLocked = exam.IsLocked,
        CreatedAt = exam.CreatedAt
    };

    public static ExamUpsertDto ToUpsertDto(ExamEntity exam) => new()
    {
        Id = exam.Id,
        Name = exam.Name,
        Term = exam.Term,
        AcademicYearId = exam.AcademicYearId,
        ClassId = exam.ClassId,
        SectionId = exam.SectionId,
        StudentGroupId = exam.StudentGroupId,
        StartsOn = exam.StartsOn,
        EndsOn = exam.EndsOn,
        Status = exam.Status,
        IsLocked = exam.IsLocked
    };

    public static ExamCreateEditViewModel ToCreateEditViewModel(ExamUpsertDto exam, bool isEdit)
    {
        var wizardPayload = new
        {
            id = exam.Id,
            name = exam.Name,
            term = exam.Term.ToString(),
            academicYearId = exam.AcademicYearId,
            classId = exam.ClassId,
            sectionId = exam.SectionId,
            startsOn = exam.StartsOn.ToString("yyyy-MM-dd"),
            endsOn = exam.EndsOn.ToString("yyyy-MM-dd"),
            description = "",
            status = exam.Status.ToString(),
            selectedClasses = exam.ClassId > 0 ? new[] { exam.ClassId } : Array.Empty<int>(),
            selectedSections = exam.SectionId.HasValue ? new[] { exam.SectionId.Value } : Array.Empty<int>(),
            selectedGroups = exam.StudentGroupId.HasValue ? new[] { exam.StudentGroupId.Value } : Array.Empty<int>(),
            selectedSubjects = Array.Empty<int>(),
            subjectMarks = new object()
        };

        return new ExamCreateEditViewModel
        {
            IsEdit = isEdit,
            ExamId = exam.Id,
            Exam = exam,
            ExamDataJson = JsonSerializer.Serialize(wizardPayload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        };
    }
}
