using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Attendance
{
    public class AttendancePercentageService : IAttendancePercentageService
    {
        private readonly IUnitOfWork _uow;
        private readonly IAttendanceSettingService _settingService;
        private readonly IAttendanceValidationService _validationService;

        public AttendancePercentageService(
            IUnitOfWork uow,
            IAttendanceSettingService settingService,
            IAttendanceValidationService validationService)
        {
            _uow = uow;
            _settingService = settingService;
            _validationService = validationService;
        }

        public async Task<double> GetStudentAttendancePercentageAsync(int studentId, int year, int month, CancellationToken ct = default)
        {
            var stats = await GetStudentAttendanceStatsAsync(studentId, year, month, ct);
            return stats.WorkingDays == 0 ? 0 : stats.AttendancePercentage;
        }

        public async Task<StudentAttendanceStatsDto> GetStudentAttendanceStatsAsync(int studentId, int year, int month, CancellationToken ct = default)
        {
            var setting = await _settingService.GetOrCreateDefaultAsync(ct);
            var startDate = new DateOnly(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (endDate > today) endDate = today;

            var workingDays = 0;
            for (var d = startDate; d <= endDate; d = d.AddDays(1))
            {
                if (await _validationService.IsWorkingDayAsync(d, ct))
                {
                    workingDays++;
                }
            }

            var records = await _uow.Repository<AttendanceRecord>().Query()
                .AsNoTracking()
                .Where(a => a.StudentId == studentId
                    && a.AttendanceDate >= startDate
                    && a.AttendanceDate <= endDate
                    && !a.IsDeleted)
                .ToListAsync(ct);

            int present = 0, late = 0, absent = 0, leave = 0;
            foreach (var r in records)
            {
                switch (r.Status)
                {
                    case AttendanceStatus.Present: present++; break;
                    case AttendanceStatus.Late: late++; break;
                    case AttendanceStatus.Absent: absent++; break;
                    case AttendanceStatus.Leave: leave++; break;
                }
            }

            int countedPresent = present;
            if (setting.CountLateAsPresent) countedPresent += late;
            if (setting.CountLeaveAsPresent) countedPresent += leave;

            double percentage = workingDays == 0 ? 0 : Math.Round((double)countedPresent / workingDays * 100, 2);

            return new StudentAttendanceStatsDto
            {
                StudentId = studentId,
                Year = year,
                Month = month,
                WorkingDays = workingDays,
                RecordedDays = records.Count,
                Present = present,
                Late = late,
                Absent = absent,
                Leave = leave,
                CountedAsPresent = countedPresent,
                AttendancePercentage = percentage
            };
        }

        public async Task<double> GetEmployeeAttendancePercentageAsync(int employeeId, int year, int month, CancellationToken ct = default)
        {
            var stats = await GetEmployeeAttendanceStatsAsync(employeeId, year, month, ct);
            return stats.WorkingDays == 0 ? 0 : stats.AttendancePercentage;
        }

        public async Task<EmployeeAttendanceStatsDto> GetEmployeeAttendanceStatsAsync(int employeeId, int year, int month, CancellationToken ct = default)
        {
            var setting = await _settingService.GetOrCreateDefaultAsync(ct);
            var startDate = new DateOnly(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (endDate > today) endDate = today;

            var workingDays = 0;
            for (var d = startDate; d <= endDate; d = d.AddDays(1))
            {
                if (await _validationService.IsWorkingDayAsync(d, ct))
                {
                    workingDays++;
                }
            }

            var records = await _uow.Repository<EmployeeAttendance>().Query()
                .AsNoTracking()
                .Where(a => a.EmployeeId == employeeId
                    && a.AttendanceDate >= startDate.ToDateTime(TimeOnly.MinValue)
                    && a.AttendanceDate <= endDate.ToDateTime(TimeOnly.MaxValue)
                    && !a.IsDeleted)
                .ToListAsync(ct);

            int present = 0, late = 0, absent = 0, leave = 0;
            foreach (var r in records)
            {
                switch (r.Status)
                {
                    case AttendanceStatus.Present: present++; break;
                    case AttendanceStatus.Late: late++; break;
                    case AttendanceStatus.Absent: absent++; break;
                    case AttendanceStatus.Leave: leave++; break;
                }
            }

            int countedPresent = present;
            if (setting.CountLateAsPresent) countedPresent += late;
            if (setting.CountLeaveAsPresent) countedPresent += leave;

            double percentage = workingDays == 0 ? 0 : Math.Round((double)countedPresent / workingDays * 100, 2);

            return new EmployeeAttendanceStatsDto
            {
                EmployeeId = employeeId,
                Year = year,
                Month = month,
                WorkingDays = workingDays,
                RecordedDays = records.Count,
                Present = present,
                Late = late,
                Absent = absent,
                Leave = leave,
                CountedAsPresent = countedPresent,
                AttendancePercentage = percentage
            };
        }
    }
}
