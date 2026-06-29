using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Admission;

public class ConversionRequest
{
    public int ApplicationId { get; set; }
    public int SectionId { get; set; }
    public int? StudentGroupId { get; set; }
    public bool CreateGuardian { get; set; } = true;
    public bool CreateUser { get; set; } = true;
    public bool GenerateIdCard { get; set; } = true;
    public bool SendWelcomeEmail { get; set; } = true;
}

public class ConversionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int? StudentId { get; set; }
    public int? UserId { get; set; }
    public int? GuardianId { get; set; }
    public string? StudentNo { get; set; }
    public string? RollNumber { get; set; }
    public string? UserName { get; set; }
    public string? GuardianCode { get; set; }
    public List<ConversionStepResult> Steps { get; set; } = new();
}

public class ConversionStepResult
{
    public string StepName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Message { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}

public class RollGenerationRequest
{
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public int? StudentGroupId { get; set; }
}

public class SectionAllocationRequest
{
    public int StudentId { get; set; }
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public int? StudentGroupId { get; set; }
}
