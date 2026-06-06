using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Implementations.Attendance
{
    public class AttendanceSettingService : IAttendanceSettingService
    {
        private readonly IAttendanceSettingRepository _repo;
        private readonly IUnitOfWork _uow;

        public AttendanceSettingService(IAttendanceSettingRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<AttendanceSetting?> GetCurrentAsync(CancellationToken ct = default)
        {
            return await _repo.Query()
                .FirstOrDefaultAsync(s => s.IsActive && !s.IsDeleted, ct);
        }

        public async Task<AttendanceSetting> GetOrCreateDefaultAsync(CancellationToken ct = default)
        {
            var existing = await GetCurrentAsync(ct);
            if (existing != null) return existing;

            var defaults = new AttendanceSetting
            {
                SchoolStartTime   = new TimeOnly(8, 0),
                LateAfterMinutes  = 15,
                HalfDayAfterMinutes = 240,
                RevisionWindowHours = 24,
                AttendanceLockAfterHours = 24,
                CountLateAsPresent = true,
                CountLeaveAsPresent = false,
                AutoAbsentEnabled = true,
                AutoAbsentTime = new TimeOnly(17, 0),
                WorkingDays = "Sun,Mon,Tue,Wed,Thu",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system"
            };

            await _repo.AddAsync(defaults, ct);
            await _uow.SaveChangesAsync(ct);
            return defaults;
        }

        public async Task UpdateAsync(AttendanceSetting setting, string updatedBy, CancellationToken ct = default)
        {
            var existing = await _repo.Query()
                .FirstOrDefaultAsync(s => s.Id == setting.Id && !s.IsDeleted, ct);

            if (existing == null) return;

            existing.SchoolStartTime         = setting.SchoolStartTime;
            existing.LateAfterMinutes        = setting.LateAfterMinutes;
            existing.HalfDayAfterMinutes     = setting.HalfDayAfterMinutes;
            existing.RevisionWindowHours     = setting.RevisionWindowHours;
            existing.AttendanceLockAfterHours = setting.AttendanceLockAfterHours;
            existing.CountLateAsPresent      = setting.CountLateAsPresent;
            existing.CountLeaveAsPresent     = setting.CountLeaveAsPresent;
            existing.AutoAbsentEnabled       = setting.AutoAbsentEnabled;
            existing.AutoAbsentTime          = setting.AutoAbsentTime;
            existing.WorkingDays             = setting.WorkingDays;
            existing.IsActive                = setting.IsActive;
            existing.UpdatedAt               = DateTime.UtcNow;
            existing.UpdatedBy               = updatedBy;

            _repo.Update(existing);
            await _uow.SaveChangesAsync(ct);
        }
    }
}
