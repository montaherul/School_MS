using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Attendance;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SchoolManagementSystem.Services.Implementations.Attendance
{
    public class LeaveService : ILeaveService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILeaveApplicationRepository _leaveRepo;
        private readonly ILeaveTypeRepository _typeRepo;
        private readonly IAttendanceLogRepository _auditLog;
        private readonly IEmployeeAttendanceRepository _employeeAttendanceRepo;
        private readonly ICalendarGenerationService _calendarGen;

        public LeaveService(IUnitOfWork uow,ILeaveApplicationRepository leaveRepo,ILeaveTypeRepository typeRepo,IAttendanceLogRepository auditLog,IEmployeeAttendanceRepository employeeAttendanceRepo, ICalendarGenerationService calendarGen)
        {
            _uow = uow;
            _leaveRepo = leaveRepo;
            _typeRepo = typeRepo;
            _auditLog = auditLog;
            _employeeAttendanceRepo = employeeAttendanceRepo;
            _calendarGen = calendarGen;
        }

        public async Task<int> ApplyLeaveAsync(
            LeaveApplyVm vm,
            int employeeId,
            string attachmentPath,
            CancellationToken ct = default)
        {
            // Date validation
            if (vm.FromDate > vm.ToDate)
                throw new InvalidOperationException(
                    "From Date cannot be greater than To Date.");

            var totalDays =
                (vm.ToDate.Date - vm.FromDate.Date).Days + 1;

            if (totalDays <= 0)
                throw new InvalidOperationException(
                    "Invalid leave duration.");

            // Leave Type Validation
            var leaveType =
                await _typeRepo.GetByIdAsync(vm.LeaveTypeId, ct);

            if (leaveType == null)
                throw new InvalidOperationException(
                    "Invalid Leave Type.");

            // Overlapping Leave Validation
            var overlap =
       await _leaveRepo.HasOverlappingLeaveAsync(
           employeeId,
           vm.FromDate.Date,
           vm.ToDate.Date,
           ct);

            if (overlap)
                throw new InvalidOperationException(
                    "You already have a leave application during this period.");

            // Leave Balance Validation
            var remainingDays =await GetLeaveBalanceAsync( employeeId,vm.LeaveTypeId,ct);

            if (totalDays > remainingDays)
                throw new InvalidOperationException(
                    $"Only {remainingDays} leave days remaining.");

            // Holiday/weekend/exam overlap check
            var warnings = new List<string>();
            for (var d = vm.FromDate.Date; d <= vm.ToDate.Date; d = d.AddDays(1))
            {
                var dateOnly = DateOnly.FromDateTime(d);
                var calEntry = await _uow.Repository<AcademicCalendar>().Query()
                    .FirstOrDefaultAsync(c => c.Date == dateOnly && !c.IsDeleted, ct);

                if (calEntry?.IsHoliday == true)
                    warnings.Add($"{d:dd MMM} is a holiday ({calEntry.Title}).");
                else if (calEntry?.IsExamDay == true)
                    warnings.Add($"{d:dd MMM} is an exam day.");
            }

            if (warnings.Any())
            {
                // Store warnings in a data bag that the caller can inspect
            }

            // Create Leave Application
            var entity = new LeaveApplication
            {
                EmployeeId = employeeId,
                LeaveTypeId = vm.LeaveTypeId,
                FromDate = vm.FromDate.Date,
                ToDate = vm.ToDate.Date,
                TotalDays = totalDays,
                Reason = vm.Reason,
                AttachmentPath = attachmentPath,
                ApprovalStatus = LeaveStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _leaveRepo.AddAsync(entity, ct);

            await _uow.SaveChangesAsync(ct);

            await _auditLog.AddAsync(
                new AttendanceLog
                {
                    UserId = employeeId.ToString(),
                    Action = "Applied Leave",
                    EntityName = nameof(LeaveApplication),
                    EntityId = entity.Id
                },
                ct);

            await _uow.SaveChangesAsync(ct);

            return entity.Id;
        }

        public async Task ApproveLeaveAsync(int id, string approvedBy, string? remarks, CancellationToken ct = default)
        {
            var entity = await _leaveRepo.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException("Leave application not found.");

            if (entity.ApprovalStatus != LeaveStatus.Pending)
            {
                throw new InvalidOperationException("Leave already processed.");
            }

            entity.ApprovalStatus = LeaveStatus.Approved;
            entity.ApprovedBy = approvedBy;
            entity.ApprovedAt = DateTime.UtcNow;
            entity.Remarks = remarks;

            for (
                var date = entity.FromDate.Date;
                date <= entity.ToDate.Date;
                date = date.AddDays(1))
            {
                var attendanceExists =
                    await _employeeAttendanceRepo
                        .IsAttendanceExistsAsync(
                            entity.EmployeeId,
                            date,
                            ct);

                if (!attendanceExists)
                {
                    await _employeeAttendanceRepo.AddAsync(
                        new EmployeeAttendance
                        {
                            EmployeeId = entity.EmployeeId,
                            AttendanceDate = date,
                            Status = AttendanceStatus.Leave,
                            Remarks = $"Auto generated from Leave #{entity.Id}"
                        },
                        ct);
                }
            }

            _leaveRepo.Update(entity);

            await _uow.SaveChangesAsync(ct);

            await _auditLog.AddAsync(
                new AttendanceLog
                {
                    UserId = approvedBy,
                    Action = "Approved Leave",
                    EntityName = nameof(LeaveApplication),
                    EntityId = id
                },
                ct);

            await _uow.SaveChangesAsync(ct);
        }

        public async Task RejectLeaveAsync(int id, string rejectedBy, string? remarks, CancellationToken ct = default)
        {
            var entity = await _leaveRepo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Leave application not found.");
           
            if (entity.ApprovalStatus != LeaveStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Leave already processed.");
            }
            entity.ApprovalStatus = LeaveStatus.Rejected;
            entity.ApprovedBy = rejectedBy;
            entity.ApprovedAt = DateTime.UtcNow;
            entity.Remarks = remarks;
            
            _leaveRepo.Update(entity);
            await _uow.SaveChangesAsync(ct);
            await _auditLog.AddAsync(new AttendanceLog { UserId = rejectedBy, Action = "Rejected Leave", EntityName = "LeaveApplication", EntityId = id }, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task CancelLeaveAsync(int id, int employeeId, CancellationToken ct = default)
        {
            var entity = await _leaveRepo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Leave application not found.");
            if (entity.EmployeeId != employeeId) throw new UnauthorizedAccessException("Not authorized to cancel this leave.");
            if (entity.ApprovalStatus != LeaveStatus.Pending) throw new InvalidOperationException("Can only cancel pending leave.");
            
            _leaveRepo.Remove(entity);
            await _uow.SaveChangesAsync(ct);
            await _auditLog.AddAsync(new AttendanceLog { UserId = employeeId.ToString(), Action = "Cancelled Leave", EntityName = "LeaveApplication", EntityId = id }, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private IQueryable<LeaveApplicationDto> MapToDto(IQueryable<LeaveApplication> query)
        {
            return query.Select(l => new LeaveApplicationDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                EmployeeName = l.Employee!.FullName,
                LeaveTypeId = l.LeaveTypeId,
                LeaveTypeName = l.LeaveType!.Name,
                FromDate = l.FromDate,
                ToDate = l.ToDate,
                TotalDays = l.TotalDays,
                Reason = l.Reason,
                AttachmentPath = l.AttachmentPath,
                ApprovalStatus = l.ApprovalStatus.ToString(),
                ApprovedBy = l.ApprovedBy,
                ApprovedAt = l.ApprovedAt,
                Remarks = l.Remarks,
                CreatedAt = l.CreatedAt
            });
        }

        public async Task<(List<LeaveApplicationDto> Data, int TotalRecords)> GetMyLeavesAsync(int employeeId, int page, int size, CancellationToken ct = default)
        {
            var query = _leaveRepo.Query().Where(l => l.EmployeeId == employeeId);
            var total = await query.CountAsync(ct);
            var items = await MapToDto(query).OrderByDescending(l => l.CreatedAt).Skip((page - 1) * size).Take(size).ToListAsync(ct);
            return (items, total);
        }

        public async Task<(List<LeaveApplicationDto> Data, int TotalRecords)> GetPendingLeavesAsync(int page, int size, CancellationToken ct = default)
        {
            var query = _leaveRepo.Query().Where(l => l.ApprovalStatus == LeaveStatus.Pending);
            var total = await query.CountAsync(ct);
            var items = await MapToDto(query).OrderByDescending(l => l.CreatedAt).Skip((page - 1) * size).Take(size).ToListAsync(ct);
            return (items, total);
        }

        public async Task<(List<LeaveApplicationDto> Data, int TotalRecords)> GetAllLeavesAsync(int page, int size, string? status, CancellationToken ct = default)
        {
            var query = _leaveRepo.Query();
            
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, out var statusEnum))
            {
                query = query.Where(l => l.ApprovalStatus == statusEnum);
            }

            var total = await query.CountAsync(ct);
            var items = await MapToDto(query).OrderByDescending(l => l.CreatedAt).Skip((page - 1) * size).Take(size).ToListAsync(ct);
            return (items, total);
        }

        public async Task<List<LeaveTypeDto>> GetActiveLeaveTypesAsync(CancellationToken ct = default)
        {
            return await _typeRepo.Query().Where(t => t.IsActive).Select(t => new LeaveTypeDto
            {
                Id = t.Id,
                Name = t.Name,
                MaxDays = t.MaxDays,
                IsPaid = t.IsPaid,
                IsActive = t.IsActive
            }).ToListAsync(ct);
        }
        public async Task<int> GetLeaveBalanceAsync(int employeeId,int leaveTypeId,CancellationToken ct = default)
        {
            var leaveType = await _typeRepo.GetByIdAsync(leaveTypeId, ct);

            if (leaveType == null)
                return 0;

            var usedDays = await _leaveRepo.Query()
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.LeaveTypeId == leaveTypeId &&
                    x.ApprovalStatus == LeaveStatus.Approved &&
                    x.FromDate.Year == DateTime.UtcNow.Year)
                .SumAsync(x => x.TotalDays, ct);

            return leaveType.MaxDays - usedDays;
        }
    }
}
