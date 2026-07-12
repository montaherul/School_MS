using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Result;

public class ExamWizardStateDto
{
    public int Step { get; set; } = 1;
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = "";
    public ExamTerm Term { get; set; }
    public string TermName { get; set; } = "";
    public string ExamType { get; set; } = "";
    public List<int> SelectedClassIds { get; set; } = [];
    public List<string> SelectedClassNames { get; set; } = [];
    public List<ExamWizardSubjectDto> Subjects { get; set; } = [];
    public int? SourceExamId { get; set; }
    public string? SourceExamName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class ExamWizardSubjectDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = "";
    public string SubjectNameBn { get; set; } = "";
    public string SubjectCode { get; set; } = "";
    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
    public bool IsOptional { get; set; }
    public int? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = "";
    public List<ExamWizardComponentDto> Components { get; set; } = [];
}

public class ExamWizardComponentDto
{
    public int ComponentId { get; set; }
    public string ComponentName { get; set; } = "";
    public decimal FullMarks { get; set; }
    public decimal PassMarks { get; set; }
    public int DisplayOrder { get; set; }
}

public class ExamWizardCreateRequest
{
    public int AcademicYearId { get; set; }
    public ExamTerm Term { get; set; }
    public string ExamType { get; set; } = "";
    public List<int> SelectedClassIds { get; set; } = [];
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? SourceExamId { get; set; }
    public List<ExamWizardSubjectDto> Subjects { get; set; } = [];
}

public class ExamWizardResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public List<int> CreatedExamIds { get; set; } = [];
    public List<string> CreatedExamNames { get; set; } = [];
    public int SubjectCount { get; set; }
    public int TeacherAssignmentCount { get; set; }
}

/// <summary>
/// NCTB standard template definition for Bangladesh national curriculum subject/component/mark configurations.
/// </summary>
public class NctbTemplateDto
{
    public string GroupName { get; set; } = "";
    public string GroupCode { get; set; } = "";
    public List<NctbSubjectTemplateDto> Subjects { get; set; } = [];
}

public class NctbSubjectTemplateDto
{
    public string SubjectName { get; set; } = "";
    public string SubjectNameBn { get; set; } = "";
    public string SubjectCode { get; set; } = "";
    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
    public bool IsMandatory { get; set; } = true;
    public bool IsOptional { get; set; }
    public bool HasPractical { get; set; }
    public List<NctbComponentTemplateDto> Components { get; set; } = [];
}

public class NctbComponentTemplateDto
{
    public string ComponentName { get; set; } = "";
    public string ComponentCode { get; set; } = "";
    public decimal FullMarks { get; set; }
    public decimal PassMarks { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPractical { get; set; }
}

/// <summary>
/// Saved exam template for reuse across terms/years.
/// </summary>
public class ExamTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = "";
    public ExamTerm Term { get; set; }
    public string TermName { get; set; } = "";
    public string ExamType { get; set; } = "";
    public List<int> SelectedClassIds { get; set; } = [];
    public List<ExamWizardSubjectDto> Subjects { get; set; } = [];
    public string CreatedBy { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class ExamTemplateListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string TermName { get; set; } = "";
    public string ExamType { get; set; } = "";
    public string ClassCount { get; set; } = "";
    public int SubjectCount { get; set; }
    public string CreatedBy { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class SaveTemplateRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int AcademicYearId { get; set; }
    public ExamTerm Term { get; set; }
    public string ExamType { get; set; } = "";
    public List<int> SelectedClassIds { get; set; } = [];
    public List<ExamWizardSubjectDto> Subjects { get; set; } = [];
}

public class LoadNctbTemplateRequest
{
    public int AcademicYearId { get; set; }
    public int ClassId { get; set; }
    public ExamTerm Term { get; set; }
}

public class TemplateListRequest
{
    public int? AcademicYearId { get; set; }
    public ExamTerm? Term { get; set; }
}
