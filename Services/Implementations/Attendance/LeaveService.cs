using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Models.ViewModels.Attendance;

namespace SchoolManagementSystem.Services.Implementations.Attendance
{
    public class LeaveService : ILeaveService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILeaveApplicationRepository _leaveRepo;
        private readonly ILeaveTypeRepository _typeRepo;
        private readonly IAttendanceLogRepository _auditLog;

        public LeaveService(IUnitOfWork uow, ILeaveApplicationRepository leaveRepo, ILeaveTypeRepository typeRepo, IAttendanceLogRepository auditLog)
        {
            _uow = uow;
            _leaveRepo = leaveRepo;
            _typeRepo = typeRepo;
            _auditLog = auditLog;
        }

        public async Task<int> ApplyLeaveAsync(LeaveApplyVm vm, int employeeId, string attachmentPath, CancellationToken ct = default)
        {
            var totalDays = (int)(vm.ToDate - vm.FromDate).TotalDays + 1;
            if (totalDays <= 0) throw new InvalidOperationException("Invalid date range.");

            var entity = new LeaveApplication
            {
                EmployeeId = employeeId,
                LeaveTypeId = vm.LeaveTypeId,
                FromDate = vm.FromDate,
                ToDate = vm.ToDate,
                TotalDays = totalDays,
                Reason = vm.Reason,
                AttachmentPath = attachmentPath,
                ApprovalStatus = LeaveApplication.ApprovalStatusEnum.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _leaveRepo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            await _auditLog.AddAsync(new AttendanceLog { UserId = employeeId.ToString(), Action = "Applied for Leave", EntityName = "LeaveApplication", EntityId = entity.Id }, ct);
            await _uow.SaveChangesAsync(ct);
            
            return entity.Id;
        }

        public async Task ApproveLeaveAsync(int id, string approvedBy, string? remarks, CancellationToken ct = default)
        {
            var entity = await _leaveRepo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Leave application not found.");
            
            entity.ApprovalStatus = LeaveApplication.ApprovalStatusEnum.Approved;
            entity.ApprovedBy = approvedBy;
            entity.ApprovedAt = DateTime.UtcNow;
            entity.Remarks = remarks;
            
            _leaveRepo.Update(entity);
            await _uow.SaveChangesAsync(ct);
            await _auditLog.AddAsync(new AttendanceLog { UserId = approvedBy, Action = "Approved Leave", EntityName = "LeaveApplication", EntityId = id }, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task RejectLeaveAsync(int id, string rejectedBy, string? remarks, CancellationToken ct = default)
        {
            var entity = await _leaveRepo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Leave application not found.");
            
            entity.ApprovalStatus = LeaveApplication.ApprovalStatusEnum.Rejected;
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
            if (entity.ApprovalStatus != LeaveApplication.ApprovalStatusEnum.Pending) throw new InvalidOperationException("Can only cancel pending leave.");
            
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
            var query = _leaveRepo.Query().Where(l => l.ApprovalStatus == LeaveApplication.ApprovalStatusEnum.Pending);
            var total = await query.CountAsync(ct);
            var items = await MapToDto(query).OrderByDescending(l => l.CreatedAt).Skip((page - 1) * size).Take(size).ToListAsync(ct);
            return (items, total);
        }

        public async Task<(List<LeaveApplicationDto> Data, int TotalRecords)> GetAllLeavesAsync(int page, int size, string? status, CancellationToken ct = default)
        {
            var query = _leaveRepo.Query();
            
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveApplication.ApprovalStatusEnum>(status, out var statusEnum))
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
    }
}
