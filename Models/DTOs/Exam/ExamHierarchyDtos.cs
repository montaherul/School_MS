using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Exam;

public class ExamClassLoadResult
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = "";
    public List<ExamSectionDto> Sections { get; set; } = [];
    public List<ExamSubjectDetailDto> Subjects { get; set; } = [];
}

public class ExamSectionDto
{
    public int SectionId { get; set; }
    public string SectionName { get; set; } = "";
}

public class ExamSubjectDetailDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = "";
    public string SubjectNameBn { get; set; } = "";
    public string SubjectCode { get; set; } = "";
    public int? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public string? TeacherEmployeeCode { get; set; }
    public bool IsOptional { get; set; }
    public bool IsReligionSubject { get; set; }
    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
    public decimal Credit { get; set; }
    public string? NCTBCode { get; set; }
    public List<ExamComponentDto> Components { get; set; } = [];
}

public class ExamComponentDto
{
    public int ComponentId { get; set; }
    public string ComponentName { get; set; } = "";
    public string ComponentCode { get; set; } = "";
    public decimal MaxMarks { get; set; }
    public decimal PassMarks { get; set; }
    public int DisplayOrder { get; set; }
}

public class ExamWizardLoadResult
{
    public List<ExamClassLoadResult> Classes { get; set; } = [];
}

public class ExamCreateRequest
{
    public string Name { get; set; } = "";
    public int AcademicYearId { get; set; }
    public ExamTerm Term { get; set; }
    public string ExamType { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<ExamClassRequest> Classes { get; set; } = [];
}

public class ExamClassRequest
{
    public int ClassId { get; set; }
    public List<int> SectionIds { get; set; } = [];
    public List<ExamSubjectRequest> Subjects { get; set; } = [];
}

public class ExamSubjectRequest
{
    public int SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public bool IsOptional { get; set; }
    public bool IsReligionSubject { get; set; }
    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
    public List<ExamComponentRequest> Components { get; set; } = [];
}

public class ExamComponentRequest
{
    public int ComponentId { get; set; }
    public decimal MaxMarks { get; set; }
    public decimal PassMarks { get; set; }
    public int DisplayOrder { get; set; }
}

public class ExamValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}

public class ExamCreateResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public int ExamId { get; set; }
    public string ExamName { get; set; } = "";
    public int ClassCount { get; set; }
    public int SubjectCount { get; set; }
    public int ComponentCount { get; set; }
}

public class ExamReadinessCheck
{
    public string Name { get; set; } = "";
    public bool Passed { get; set; }
    public string Details { get; set; } = "";
    public int Weight { get; set; }
}

public class ExamReadinessDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = "";
    public decimal ReadinessPercent { get; set; }
    public bool IsReadyToPublish { get; set; }
    public List<ExamReadinessCheck> Checks { get; set; } = [];
}

public class ExamArchiveRequest
{
    public string Reason { get; set; } = "";
}

public class ExamCopyRequest
{
    public int SourceExamId { get; set; }
    public int TargetAcademicYearId { get; set; }
}