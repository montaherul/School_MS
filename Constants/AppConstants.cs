namespace SchoolManagementSystem.Constants;

/// <summary>
/// Centralized application constants (pagination, file limits, cache keys, etc.)
/// </summary>
public static class AppConstants
{
    // ── Pagination ───────────────────────────────────────────────────────────
    public static class Pagination
    {
        public const int DefaultPageSize = 20;
        public const int MaxPageSize     = 100;
        public const int SmallPageSize   = 10;
        public const int LargePageSize   = 50;
    }

    // ── File Upload ──────────────────────────────────────────────────────────
    public static class FileUpload
    {
        public const long MaxPhotoSizeBytes    = 2 * 1024 * 1024;  // 2 MB
        public const long MaxDocumentSizeBytes = 10 * 1024 * 1024; // 10 MB

        public static readonly string[] AllowedPhotoExtensions    = [".jpg", ".jpeg", ".png", ".webp"];
        public static readonly string[] AllowedDocumentExtensions = [".pdf", ".doc", ".docx", ".xls", ".xlsx"];

        // Upload sub-folders
        public const string EmployeeFolder   = "employees";
        public const string StudentFolder    = "students";
        public const string AdmissionFolder  = "admissions/profiles";
        public const string DocumentFolder   = "documents";
        public const string PayslipFolder    = "payslips";
        public const string AcademicFolder   = "academic";
        public const string ContractFolder   = "contracts";
    }

    // ── Cache Keys ───────────────────────────────────────────────────────────
    public static class CacheKeys
    {
        public const string AdminDashboard     = "dashboard:admin";
        public const string ActiveAcademicYear = "academic:year:active";
        public const string AllRoles           = "roles:all";
        public const string AllPermissions     = "permissions:all";
        public const string AllDepartments     = "departments:all";
        public const string AllDesignations    = "designations:all";
        public const string AllLeaveTypes      = "leavetypes:all";
        public const string HolidaysUpcoming   = "holidays:upcoming";

        public static string EmployeeDashboard(long empId)       => $"dashboard:employee:{empId}";
        public static string NotificationCount(long userId)      => $"notifications:count:{userId}";
        public static string AttendanceSummary(long empId, int month) => $"attendance:summary:{empId}:{month}";
    }

    // ── Cache Duration ───────────────────────────────────────────────────────
    public static class CacheDuration
    {
        public static readonly TimeSpan Short  = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan Medium = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan Long   = TimeSpan.FromHours(1);
        public static readonly TimeSpan Day    = TimeSpan.FromHours(24);
    }

    // ── Date Formats ─────────────────────────────────────────────────────────
    public static class DateFormats
    {
        public const string Display     = "dd MMM yyyy";
        public const string DisplayTime = "dd MMM yyyy HH:mm";
        public const string MonthYear   = "MMMM yyyy";
        public const string Iso         = "yyyy-MM-dd";
    }
}
