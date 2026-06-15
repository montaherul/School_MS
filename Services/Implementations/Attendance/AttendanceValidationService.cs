using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Implementations.Attendance
{
    /// <summary>
    /// Validates attendance dates against the academic calendar and attendance settings.
    /// Used by StudentAttendanceService and EmployeeAttendanceService before saving records.
    /// </summary>
    public class AttendanceValidationService : IAttendanceValidationService
    {
        private readonly IAcademicCalendarRepository _calendarRepo;
        private readonly IAttendanceSettingRepository _settingRepo;
        private readonly IUnitOfWork _uow;

        public AttendanceValidationService(
            IAcademicCalendarRepository calendarRepo,
            IAttendanceSettingRepository settingRepo,
            IUnitOfWork uow)
        {
            _calendarRepo = calendarRepo;
            _settingRepo = settingRepo;
            _uow = uow;
        }

        public async Task<bool> IsHolidayAsync(DateOnly date, CancellationToken ct = default)
        {
            var entry = await _calendarRepo.Query()
                .FirstOrDefaultAsync(c => c.Date == date && c.IsHoliday && !c.IsDeleted, ct);
            return entry != null;
        }

        public async Task<bool> IsWorkingDayAsync(DateOnly date, CancellationToken ct = default)
        {
            // 1. Check if there's an explicit calendar entry marking it a non-working day
            var calEntry = await _calendarRepo.Query()
                .FirstOrDefaultAsync(c => c.Date == date && !c.IsDeleted, ct);

            if (calEntry != null)
                return calEntry.IsWorkingDay;

            // 2. Fall back to attendance settings for the day-of-week rule
            var setting = await _settingRepo.Query()
                .FirstOrDefaultAsync(s => s.IsActive && !s.IsDeleted, ct);

            if (setting == null)
                return true; // default: allow attendance

            var dayAbbrev = date.DayOfWeek switch
            {
                DayOfWeek.Sunday    => "Sun",
                DayOfWeek.Monday    => "Mon",
                DayOfWeek.Tuesday   => "Tue",
                DayOfWeek.Wednesday => "Wed",
                DayOfWeek.Thursday  => "Thu",
                DayOfWeek.Friday    => "Fri",
                DayOfWeek.Saturday  => "Sat",
                _                   => string.Empty
            };

            return setting.WorkingDays.Contains(dayAbbrev, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> IsWithinRevisionWindowAsync(DateOnly attendanceDate, CancellationToken ct = default)
        {
            var setting = await _settingRepo.Query()
                .FirstOrDefaultAsync(s => s.IsActive && !s.IsDeleted, ct);

            int windowHours = setting?.AttendanceLockAfterHours ?? 24;
            var cutoff = DateTime.UtcNow.AddHours(-windowHours);
            var attendanceDateUtc = attendanceDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            return attendanceDateUtc >= cutoff;
        }

        public async Task<bool> IsExamDayAsync(DateOnly date, CancellationToken ct = default)
        {
            var entry = await _calendarRepo.Query()
                .FirstOrDefaultAsync(c => c.Date == date && c.IsExamDay && !c.IsDeleted, ct);
            return entry != null;
        }

        public async Task<string?> ValidateAttendanceDateAsync(DateOnly date, CancellationToken ct = default)
        {
            if (await IsHolidayAsync(date, ct))
                return $"{date:dd MMM yyyy} is a holiday. Attendance cannot be saved on holidays.";

            if (!await IsWorkingDayAsync(date, ct))
                return $"{date:dd MMM yyyy} is not a configured working day.";

            if (!await IsWithinRevisionWindowAsync(date, ct))
                return $"The revision window has closed for {date:dd MMM yyyy}. Contact an administrator to unlock.";

            return null; // valid
        }
    }
}
