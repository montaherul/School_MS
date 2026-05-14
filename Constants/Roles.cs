namespace SchoolManagementSystem.Constants;

/// <summary>
/// Centralized role name constants. Use these to avoid magic strings in authorization logic.
/// </summary>
public static class Roles
{
    public const string SuperAdmin   = "SuperAdmin";
    public const string Admin        = "Admin";
    public const string Principal    = "Principal";
    public const string HRManager    = "HRManager";
    public const string Accountant   = "Accountant";
    public const string Teacher      = "Teacher";
    public const string Librarian    = "Librarian";
    public const string Staff        = "Staff";
    public const string Student      = "Student";
    public const string Parent       = "Parent";

    /// <summary>Academic staff roles — teachers and above.</summary>
    public static readonly string[] AcademicStaff = [Teacher, Principal];

    /// <summary>HR-management capable roles.</summary>
    public static readonly string[] HRRoles = [Admin, SuperAdmin, HRManager, Principal];

    /// <summary>Finance management capable roles.</summary>
    public static readonly string[] FinanceRoles = [Admin, SuperAdmin, Accountant];

    /// <summary>All administrator-tier roles.</summary>
    public static readonly string[] AdminRoles = [Admin, SuperAdmin];

    /// <summary>Roles that can see school-wide dashboards.</summary>
    public static readonly string[] ManagementRoles = [Admin, SuperAdmin, Principal, HRManager, Accountant];
}
