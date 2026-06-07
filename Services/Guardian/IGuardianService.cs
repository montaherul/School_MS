using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.DTOs.Guardian;
using SchoolManagementSystem.Models.Entities.Admission;

namespace SchoolManagementSystem.Services.Guardian;

public interface IGuardianService
{
    Task<(IEnumerable<GuardianListItemDto> Items, int TotalCount)> GetGuardianListAsync(string? searchTerm, string? status, int pageNumber, int pageSize);
    Task<GuardianDetailsDto?> GetGuardianByIdAsync(int id);
    Task<int> CreateGuardianAsync(GuardianUpsertDto dto);
    Task UpdateGuardianAsync(GuardianUpsertDto dto);
    Task DeleteGuardianAsync(int id);
    Task SetGuardianStatusAsync(int id, bool active);
    Task LinkStudentAsync(int guardianId, int studentId, string relation);
    Task<GuardianDashboardDataDto> GetDashboardAsync(int guardianId);
    Task<GuardianDashboardDataDto> GetDashboardByUserIdAsync(int userId);

    // PHASE 6/7: Onboarding from Admission approval
    /// <summary>
    /// Idempotently find or create a Guardian from admission data.
    /// Searches by GuardianEmail first, then by GuardianMobileNumber.
    /// Does NOT create a user; the caller is responsible for invoking <see cref="EnsureGuardianUserAsync"/>.
    /// </summary>
    Task<Models.Entities.Guardian.Guardian> EnsureGuardianFromAdmissionAsync(AdmissionApplication application, CancellationToken ct = default);

    /// <summary>
    /// Create (or reuse) a Guardian portal user with username pattern gdn-{GuardianCode}.
    /// Returns the activation token if a NEW user was created (empty string if user already existed).
    /// </summary>
    Task<string> EnsureGuardianUserAsync(int guardianId, CancellationToken ct = default);

    // Profile update
    Task UpdateGuardianProfileAsync(int userId, GuardianProfileUpdateDto dto, CancellationToken ct = default);

    // Notifications
    Task CreateNotificationAsync(int guardianId, string title, string message, string? category = null, CancellationToken ct = default);
    Task CreateAttendanceNotificationAsync(int studentId, string studentName, string status, DateTime date, CancellationToken ct = default);
    Task CreateFeeDueNotificationAsync(int studentId, string studentName, decimal amount, CancellationToken ct = default);
    Task CreateResultPublishedNotificationAsync(int studentId, string studentName, string examName, CancellationToken ct = default);

    // PHASE 22: Security helper - returns true if the user has access to the student.
    Task<bool> UserHasAccessToStudentAsync(int userId, int studentId, CancellationToken ct = default);

    // PHASE 10b / 11-17: Per-child data
    Task<List<GuardianChildCardDto>> GetChildrenByUserIdAsync(int userId, CancellationToken ct = default);
    Task<GuardianChildDetailDto?> GetChildDetailAsync(int userId, int studentId, CancellationToken ct = default);
    Task<List<StudentAttendanceDto>> GetChildAttendanceAsync(int userId, int studentId, DateTime? from, DateTime? to, CancellationToken ct = default);
}

