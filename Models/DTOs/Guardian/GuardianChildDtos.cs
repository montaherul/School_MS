namespace SchoolManagementSystem.Models.DTOs.Guardian;

public class GuardianChildCardDto
{
    public int StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
    public string ProfilePicturePath { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public bool IsPrimaryGuardian { get; set; }
    public double AttendancePercentage { get; set; }
    public int UnreadNotificationCount { get; set; }
    public decimal OutstandingFees { get; set; }
}

public class GuardianChildDetailDto
{
    public int StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public string ProfilePicturePath { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public bool IsPrimaryGuardian { get; set; }
    public int TotalAttendanceDays { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public int LeaveCount { get; set; }
    public double AttendancePercentage { get; set; }
    public decimal OutstandingFees { get; set; }
    public decimal? LatestGPA { get; set; }
}
