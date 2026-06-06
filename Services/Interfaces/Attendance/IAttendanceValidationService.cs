using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Interfaces.Attendance
{
    /// <summary>
    /// Central validation engine used by Student and Employee attendance services.
    /// Checks calendar holidays, working-day rules, and revision-window locks.
    /// </summary>
    public interface IAttendanceValidationService
    {
        /// <summary>Returns true when the given date is a configured holiday.</summary>
        Task<bool> IsHolidayAsync(DateOnly date, CancellationToken ct = default);

        /// <summary>Returns true when the given date is a configured working day.</summary>
        Task<bool> IsWorkingDayAsync(DateOnly date, CancellationToken ct = default);

        /// <summary>Returns true when attendance can still be edited (within the revision window).</summary>
        Task<bool> IsWithinRevisionWindowAsync(DateOnly attendanceDate, CancellationToken ct = default);

        /// <summary>Returns a human-readable reason when attendance cannot be saved, or null when valid.</summary>
        Task<string?> ValidateAttendanceDateAsync(DateOnly date, CancellationToken ct = default);
    }
}
