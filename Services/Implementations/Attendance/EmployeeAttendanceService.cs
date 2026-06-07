using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using EmployeeEntity = SchoolManagementSystem.Models.Entities.Employee.Employee;

namespace SchoolManagementSystem.Services.Implementations.Attendance
{
    public class EmployeeAttendanceService : IEmployeeAttendanceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmployeeAttendanceRepository _repo;
        private readonly IAttendanceLogRepository _auditLog;
        private readonly IAttendanceNotificationService _notificationService;
        private readonly IAttendanceValidationService _validationService;
        private readonly IAttendancePercentageService _percentageService;

        public EmployeeAttendanceService(
            IUnitOfWork uow,
            IEmployeeAttendanceRepository repo,
            IAttendanceLogRepository auditLog,
            IAttendanceNotificationService notificationService,
            IAttendanceValidationService validationService,
            IAttendancePercentageService percentageService)
        {
            _uow = uow;
            _repo = repo;
            _auditLog = auditLog;
            _notificationService = notificationService;
            _validationService = validationService;
            _percentageService = percentageService;
        }

        public async Task<int> CheckInAsync(int employeeId, DateTime date, TimeSpan time, string recordedBy, CancellationToken ct = default)
        {
            var validationError = await _validationService.ValidateAttendanceDateAsync(DateOnly.FromDateTime(date), ct);
            if (validationError != null)
                throw new InvalidOperationException(validationError);

            var entity = await _repo.Query().FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.AttendanceDate == date.Date && !a.IsDeleted, ct);
            if (entity != null) throw new InvalidOperationException("Check-in already exists for today.");
            var settings = await GetAttendanceSettingsAsync(ct);

            entity = new EmployeeAttendance
            {
                EmployeeId = employeeId,
                AttendanceDate = date.Date,
                CheckInTime = time,
                Status = ResolveStatusFromSettings(SchoolManagementSystem.Models.Enums.AttendanceStatus.Present, time, settings),
                CreatedBy = recordedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            await _auditLog.AddAsync(new AttendanceLog { UserId = recordedBy, Action = "Employee Check-In", EntityName = "EmployeeAttendance", EntityId = entity.Id }, ct);
            await _uow.SaveChangesAsync(ct);
            
            return entity.Id;
        }

        public async Task<int> CheckOutAsync(int employeeId, DateTime date, TimeSpan time, string recordedBy, CancellationToken ct = default)
        {
            var entity = await _repo.Query().FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.AttendanceDate == date.Date && !a.IsDeleted, ct);
            if (entity == null) throw new InvalidOperationException("No check-in found for today.");

            entity.CheckOutTime = time;
            entity.UpdatedBy = recordedBy;
            entity.UpdatedAt = DateTime.UtcNow;
            _repo.Update(entity);
            await _auditLog.AddAsync(new AttendanceLog { UserId = recordedBy, Action = "Employee Check-Out", EntityName = "EmployeeAttendance", EntityId = entity.Id }, ct);
            await _uow.SaveChangesAsync(ct);
            
            return entity.Id;
        }

        public async Task<int> MarkStatusAsync(int employeeId, DateTime date, SchoolManagementSystem.Models.Enums.AttendanceStatus status, string? remarks, string recordedBy, CancellationToken ct = default)
        {
            var validationError = await _validationService.ValidateAttendanceDateAsync(DateOnly.FromDateTime(date), ct);
            if (validationError != null)
                throw new InvalidOperationException(validationError);

            if (await _repo.IsAttendanceExistsAsync(employeeId, date, ct))
                throw new InvalidOperationException("Attendance already exists for this date.");

            var entity = new EmployeeAttendance
            {
                EmployeeId = employeeId,
                AttendanceDate = date.Date,
                Status = status,
                Remarks = remarks,
                CreatedBy = recordedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            await _auditLog.AddAsync(new AttendanceLog { UserId = recordedBy, Action = "Marked Employee Attendance", EntityName = "EmployeeAttendance", EntityId = entity.Id }, ct);
            if (status == AttendanceStatus.Late)
            {
                await _notificationService.SendLateEmployeeNotificationsAsync(new[] { employeeId }, DateOnly.FromDateTime(date), recordedBy, ct);
            }
            await _uow.SaveChangesAsync(ct);
            
            return entity.Id;
        }

        public async Task UpdateAttendanceAsync(int id, SchoolManagementSystem.Models.Enums.AttendanceStatus status, TimeSpan? checkIn, TimeSpan? checkOut, string? remarks, string updatedBy, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Attendance record not found.");
            
            entity.Status = status;
            entity.CheckInTime = checkIn;
            entity.CheckOutTime = checkOut;
            entity.Remarks = remarks;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
            
            _repo.Update(entity);
            await _auditLog.AddAsync(new AttendanceLog { UserId = updatedBy, Action = "Updated Employee Attendance", EntityName = "EmployeeAttendance", EntityId = id }, ct);
            if (status == AttendanceStatus.Late)
            {
                await _notificationService.SendLateEmployeeNotificationsAsync(new[] { entity.EmployeeId }, DateOnly.FromDateTime(entity.AttendanceDate), updatedBy, ct);
            }
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAttendanceAsync(int id, string deletedBy, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Attendance record not found.");
            
            _repo.Remove(entity);
            await _uow.SaveChangesAsync(ct);
            await _auditLog.AddAsync(new AttendanceLog { UserId = deletedBy, Action = "Deleted Employee Attendance", EntityName = "EmployeeAttendance", EntityId = id }, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<bool> BulkMarkAsync(EmployeeAttendanceBulkDto dto, string recordedBy, CancellationToken ct = default)
        {
            return await SaveAttendanceAsync(dto, recordedBy, ct);
        }

        public async Task<bool> SaveAttendanceAsync(EmployeeAttendanceBulkDto dto, string recordedBy, CancellationToken ct = default)
        {
            var date = dto.AttendanceDate.Date;
            var validationError = await _validationService.ValidateAttendanceDateAsync(DateOnly.FromDateTime(date), ct);
            if (validationError != null)
                throw new InvalidOperationException(validationError);

            if (dto.Attendances.Count == 0)
                throw new InvalidOperationException("No attendance rows were submitted.");

            var duplicateEmployeeIds = dto.Attendances
                .GroupBy(a => a.EmployeeId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToArray();

            if (duplicateEmployeeIds.Length > 0)
                throw new InvalidOperationException("Duplicate employee attendance rows were submitted.");

            var employeeIds = dto.Attendances.Select(a => a.EmployeeId).ToArray();
            var activeEmployeeIds = await _uow.Repository<EmployeeEntity>().Query()
                .Where(e => employeeIds.Contains(e.Id) && e.Status == "Active" && !e.IsDeleted)
                .Select(e => e.Id)
                .ToArrayAsync(ct);

            if (activeEmployeeIds.Length != employeeIds.Length)
                throw new InvalidOperationException("One or more selected employees are inactive or invalid.");

            var existingRecords = await _repo.Query()
                .Where(a => a.AttendanceDate == date && employeeIds.Contains(a.EmployeeId) && !a.IsDeleted)
                .ToListAsync(ct);
            var settings = await GetAttendanceSettingsAsync(ct);

            var existingDict = existingRecords.ToDictionary(a => a.EmployeeId);
            var created = 0;
            var updated = 0;
            var lateEmployeeIds = new List<int>();

            foreach (var item in dto.Attendances)
            {
                var status = ResolveStatus(item, settings);
                ValidateTimes(item.CheckInTime, item.CheckOutTime);

                if (existingDict.TryGetValue(item.EmployeeId, out var att))
                {
                    att.Status = status;
                    att.CheckInTime = item.CheckInTime;
                    att.CheckOutTime = item.CheckOutTime;
                    att.Remarks = item.Remarks;
                    att.UpdatedBy = recordedBy;
                    att.UpdatedAt = DateTime.UtcNow;
                    _repo.Update(att);
                    updated++;
                }
                else
                {
                    await _repo.AddAsync(new EmployeeAttendance
                    {
                        EmployeeId = item.EmployeeId,
                        AttendanceDate = date,
                        Status = status,
                        CheckInTime = item.CheckInTime,
                        CheckOutTime = item.CheckOutTime,
                        Remarks = item.Remarks,
                        CreatedBy = recordedBy,
                        CreatedAt = DateTime.UtcNow
                    }, ct);
                    created++;
                }

                if (status == AttendanceStatus.Late)
                {
                    lateEmployeeIds.Add(item.EmployeeId);
                }
            }
            
            await _auditLog.AddAsync(new AttendanceLog
            {
                UserId = recordedBy,
                Action = $"Saved Employee Attendance: {created} created, {updated} updated",
                EntityName = "EmployeeAttendance",
                EntityId = 0
            }, ct);
            if (lateEmployeeIds.Any())
            {
                await _notificationService.SendLateEmployeeNotificationsAsync(lateEmployeeIds, DateOnly.FromDateTime(date), recordedBy, ct);
            }
            await _uow.SaveChangesAsync(ct);
            
            return true;
        }

        public async Task<(List<EmployeeAttendanceDto> Data, int TotalRecords, EmployeeAttendanceSummaryDto Summary)> LoadAttendanceAsync(
            EmployeeAttendanceFilterDto filter,
            int page,
            int size,
            CancellationToken ct = default)
        {
            page = Math.Max(page, 1);
            size = Math.Clamp(size, 5, 10000);
            filter.AttendanceDate = filter.AttendanceDate.Date;

            var (items, totalRecords) = await _repo.GetAttendanceGridAsync(filter, page, size, ct);
            var summary = await _repo.GetAttendanceSummaryAsync(filter, ct);

            return (items, totalRecords, summary);
        }

        public async Task<(List<EmployeeAttendanceDto> Data, int TotalRecords)> GetPagedAsync(int page, int size, DateTime? date, CancellationToken ct = default)
        {
            var filter = new EmployeeAttendanceFilterDto
            {
                AttendanceDate = date ?? DateTime.Today,
                Page = page,
                PageSize = size
            };

            return await _repo.GetAttendanceGridAsync(filter, page, size, ct);
        }

        public async Task<List<EmployeeAttendanceDto>> GetAttendanceHistoryAsync(int employeeId, int year, int month, CancellationToken ct = default)
        {
            return await _repo.GetEmployeeHistoryAsync(employeeId, year, month, ct);
        }

        public async Task<EmployeeAttendanceMonthlySummaryDto> GetMonthlySummaryAsync(int employeeId, int year, int month, CancellationToken ct = default)
        {
            var employee = await _uow.Repository<EmployeeEntity>().Query()
                .AsNoTracking()
                .Where(e => e.Id == employeeId && !e.IsDeleted)
                .Select(e => new { e.Id, e.EmployeeCode, e.FullName })
                .FirstOrDefaultAsync(ct)
                ?? throw new KeyNotFoundException("Employee not found.");

            var records = await _repo.Query()
                .AsNoTracking()
                .Where(a => a.EmployeeId == employeeId && a.AttendanceDate.Year == year && a.AttendanceDate.Month == month)
                .ToListAsync(ct);

            var present = records.Count(a => a.Status == AttendanceStatus.Present);
            var late = records.Count(a => a.Status == AttendanceStatus.Late);
            var recordedDays = records.Count;
            var percentage = recordedDays == 0 ? 0 : Math.Round(((double)(present + late) / recordedDays) * 100, 2);

            return new EmployeeAttendanceMonthlySummaryDto
            {
                EmployeeId = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                EmployeeName = employee.FullName,
                Year = year,
                Month = month,
                RecordedDays = recordedDays,
                Present = present,
                Absent = records.Count(a => a.Status == AttendanceStatus.Absent),
                Late = late,
                Leave = records.Count(a => a.Status == AttendanceStatus.Leave),
                AttendancePercentage = percentage
            };
        }

        public async Task<double> GetAttendancePercentageAsync(int employeeId, int year, int month, CancellationToken ct = default)
        {
            return await _percentageService.GetEmployeeAttendancePercentageAsync(employeeId, year, month, ct);
        }

        private async Task<AttendanceSetting> GetAttendanceSettingsAsync(CancellationToken ct)
        {
            return await _uow.Repository<AttendanceSetting>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(ct) ?? new AttendanceSetting();
        }

        private static AttendanceStatus ResolveStatus(EmployeeAttendanceItemDto item, AttendanceSetting settings)
        {
            if (!string.IsNullOrWhiteSpace(item.StatusName) &&
                Enum.TryParse<AttendanceStatus>(item.StatusName.Trim(), true, out var statusFromName))
            {
                return ResolveStatusFromSettings(statusFromName, item.CheckInTime, settings);
            }

            if (Enum.IsDefined(typeof(AttendanceStatus), item.Status))
            {
                return ResolveStatusFromSettings(item.Status, item.CheckInTime, settings);
            }

            throw new InvalidOperationException("Invalid attendance status.");
        }

        private static AttendanceStatus ResolveStatusFromSettings(AttendanceStatus requestedStatus, TimeSpan? checkInTime, AttendanceSetting settings)
        {
            if (requestedStatus != AttendanceStatus.Present || !checkInTime.HasValue)
            {
                return requestedStatus;
            }

            var checkInTimeOnly = TimeOnly.FromTimeSpan(checkInTime.Value);
            var lateAt = settings.SchoolStartTime.AddMinutes(settings.LateAfterMinutes);
            if (checkInTimeOnly > lateAt)
            {
                return AttendanceStatus.Late;
            }

            return requestedStatus;
        }

        private static void ValidateTimes(TimeSpan? checkIn, TimeSpan? checkOut)
        {
            if (checkIn.HasValue && (checkIn.Value < TimeSpan.Zero || checkIn.Value >= TimeSpan.FromDays(1)))
                throw new InvalidOperationException("Invalid check-in time.");

            if (checkOut.HasValue && (checkOut.Value < TimeSpan.Zero || checkOut.Value >= TimeSpan.FromDays(1)))
                throw new InvalidOperationException("Invalid check-out time.");

            if (checkIn.HasValue && checkOut.HasValue && checkOut.Value <= checkIn.Value)
                throw new InvalidOperationException("Check-out time must be later than check-in time.");
        }
    }
}
