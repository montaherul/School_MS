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

        public EmployeeAttendanceService(IUnitOfWork uow, IEmployeeAttendanceRepository repo, IAttendanceLogRepository auditLog)
        {
            _uow = uow;
            _repo = repo;
            _auditLog = auditLog;
        }

        public async Task<int> CheckInAsync(int employeeId, DateTime date, TimeSpan time, string recordedBy, CancellationToken ct = default)
        {
            var entity = await _repo.Query().FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.AttendanceDate == date.Date && !a.IsDeleted, ct);
            if (entity != null) throw new InvalidOperationException("Check-in already exists for today.");

            entity = new EmployeeAttendance
            {
                EmployeeId = employeeId,
                AttendanceDate = date.Date,
                CheckInTime = time,
                Status = SchoolManagementSystem.Models.Enums.AttendanceStatus.Present,
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
                .Where(a => a.AttendanceDate == date && employeeIds.Contains(a.EmployeeId))
                .ToListAsync(ct);

            var existingDict = existingRecords.ToDictionary(a => a.EmployeeId);
            var created = 0;
            var updated = 0;

            foreach (var item in dto.Attendances)
            {
                var status = ResolveStatus(item);
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
            }
            
            await _auditLog.AddAsync(new AttendanceLog
            {
                UserId = recordedBy,
                Action = $"Saved Employee Attendance: {created} created, {updated} updated",
                EntityName = "EmployeeAttendance",
                EntityId = 0
            }, ct);
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
            size = Math.Clamp(size, 5, 100);
            filter.AttendanceDate = filter.AttendanceDate.Date;

            var (items, totalRecords) = await _repo.GetAttendanceGridAsync(filter, page, size, ct);
            var summary = await _repo.GetAttendanceSummaryAsync(filter, ct);

            return (items, totalRecords, summary);
        }

        public async Task<(List<EmployeeAttendanceDto> Data, int TotalRecords)> GetPagedAsync(int page, int size, DateTime? date, CancellationToken ct = default)
        {
            var searchDate = date ?? DateTime.Today;

            var allEmployees = await _uow.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().Query()
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Where(e => e.Status == "Active" && !e.IsDeleted)
                .ToListAsync(ct);

            var existingAttendances = await _repo.Query()
                .Where(a => a.AttendanceDate == searchDate.Date)
                .ToListAsync(ct);

            var list = new List<EmployeeAttendanceDto>();
            foreach (var emp in allEmployees)
            {
                var att = existingAttendances.FirstOrDefault(a => a.EmployeeId == emp.Id);
                if (att != null)
                {
                    list.Add(new EmployeeAttendanceDto
                    {
                        Id = att.Id,
                        EmployeeId = emp.Id,
                        EmployeeCode = emp.EmployeeCode,
                        EmployeeName = emp.FullName,
                        Department = emp.Department != null ? emp.Department.Name : string.Empty,
                        Designation = emp.Designation != null ? emp.Designation.Name : string.Empty,
                        EmployeeType = emp.EmployeeType,
                        IsTeachingStaff = emp.IsTeachingStaff,
                        AttendanceDate = searchDate.Date,
                        CheckInTime = att.CheckInTime,
                        CheckOutTime = att.CheckOutTime,
                        Status = att.Status,
                        StatusName = att.Status.ToString(),
                        Remarks = att.Remarks
                    });
                }
                else
                {
                    list.Add(new EmployeeAttendanceDto
                    {
                        Id = 0,
                        EmployeeId = emp.Id,
                        EmployeeCode = emp.EmployeeCode,
                        EmployeeName = emp.FullName,
                        Department = emp.Department != null ? emp.Department.Name : string.Empty,
                        Designation = emp.Designation != null ? emp.Designation.Name : string.Empty,
                        EmployeeType = emp.EmployeeType,
                        IsTeachingStaff = emp.IsTeachingStaff,
                        AttendanceDate = searchDate.Date,
                        CheckInTime = null,
                        CheckOutTime = null,
                        Status = SchoolManagementSystem.Models.Enums.AttendanceStatus.Present,
                        StatusName = SchoolManagementSystem.Models.Enums.AttendanceStatus.Present.ToString(),
                        Remarks = string.Empty
                    });
                }
            }

            var totalCount = list.Count;
            var pagedData = list.OrderBy(e => e.EmployeeName)
                                .Skip((page - 1) * size).Take(size).ToList();

            return (pagedData, totalCount);
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
            var records = await _repo.Query().Where(a => a.EmployeeId == employeeId && a.AttendanceDate.Year == year && a.AttendanceDate.Month == month).ToListAsync(ct);
            if (!records.Any()) return 0;

            int totalDays = records.Count;
            int presentDays = records.Count(a => a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Present || a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Late);

            return Math.Round(((double)presentDays / totalDays) * 100, 2);
        }

        private static AttendanceStatus ResolveStatus(EmployeeAttendanceItemDto item)
        {
            if (!string.IsNullOrWhiteSpace(item.StatusName) &&
                Enum.TryParse<AttendanceStatus>(item.StatusName.Trim(), true, out var statusFromName))
            {
                return statusFromName;
            }

            if (Enum.IsDefined(typeof(AttendanceStatus), item.Status))
            {
                return item.Status;
            }

            throw new InvalidOperationException("Invalid attendance status.");
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
