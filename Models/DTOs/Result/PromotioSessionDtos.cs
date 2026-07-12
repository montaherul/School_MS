using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Result;

public class PromotioSessionListItemDto
{
    public int Id { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public string AcademicYearName { get; set; } = string.Empty;
    public DateTime PromotionDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalRecords { get; set; }
}

public class PromotioSessionUpsertDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Session name is required")]
    [MaxLength(200)]
    public string SessionName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Academic year is required")]
    public int AcademicYearId { get; set; }

    [Required(ErrorMessage = "Promotion date is required")]
    public DateTime PromotionDate { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Remarks { get; set; }
}

public class PromotioCandidateDto
{
    public int StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public decimal AttendancePercentage { get; set; }
    public int TotalFailedSubjects { get; set; }
    public bool IsPassed { get; set; }
    public string EligibilityStatus { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
}

public class PromotioResult
{
    public int SessionId { get; set; }
    public int FromClassId { get; set; }
    public int ToClassId { get; set; }
    public int TotalCandidates { get; set; }
    public int PromotedCount { get; set; }
    public int FailedCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = [];
}

public class PromotioDashboardDto
{
    public int TotalStudents { get; set; }
    public int EligibleForPromotion { get; set; }
    public int FailedStudents { get; set; }
    public int AlreadyPromoted { get; set; }
    public int TotalPromotions { get; set; }
    public int DraftSessions { get; set; }
    public int CompletedSessions { get; set; }
}

public class ClassProgressionRuleDto
{
    public int Id { get; set; }
    public int FromClassId { get; set; }
    public string FromClassName { get; set; } = string.Empty;
    public int ToClassId { get; set; }
    public string ToClassName { get; set; } = string.Empty;
    public string ProgressionType { get; set; } = "Normal";
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}

public class ClassProgressionRuleUpsertDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "From class is required")]
    public int FromClassId { get; set; }

    [Required(ErrorMessage = "To class is required")]
    public int ToClassId { get; set; }

    [MaxLength(20)]
    public string ProgressionType { get; set; } = "Normal";
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}
