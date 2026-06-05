using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;

namespace SchoolManagementSystem.Services.Implementations.Attendance;

public class AttendanceRecordService : IAttendanceRecordService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IAttendanceAuthorizationService _authorizationService;

    public AttendanceRecordService(
        IUnitOfWork unitOfWork,
        IAttendanceRepository attendanceRepository,
        IAttendanceAuthorizationService authorizationService)
    {
        _unitOfWork            = unitOfWork;
        _attendanceRepository  = attendanceRepository;
        _authorizationService  = authorizationService;
    }

    public async Task<PagedResult<AttendanceRecordListItemDto>> GetPagedAsync(
        int page, int pageSize, string? search,
        int? studentId       = null,
        int? classId         = null,
        int? sectionId       = null,
        int? studentGroupId  = null,
        DateOnly? attendanceDate = null,
        CancellationToken cancellationToken = default)
    {
        page     = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);

        var (items, totalCount) = await _attendanceRepository.GetListByStoredProcedureAsync(
            page, pageSize, search,
            studentId      ?? 0,
            classId        ?? 0,
            sectionId      ?? 0,
            studentGroupId ?? 0,
            0,
            attendanceDate,
            null,
            null,
            cancellationToken);

        return new PagedResult<AttendanceRecordListItemDto>
        {
            Items      = items,
            Page       = page,
            PageSize   = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<AttendanceRecordUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<AttendanceRecord>()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (entity is null) return null;

        return new AttendanceRecordUpsertDto
        {
            Id            = entity.Id,
            StudentId     = entity.StudentId,
            SchoolClassId = entity.SchoolClassId,
            SectionId     = entity.SectionId,
            Status        = entity.Status,
            Remarks       = entity.Remarks ?? string.Empty,
            AttendanceDate = entity.AttendanceDate
        };
    }

    public async Task<int> CreateAsync(AttendanceRecordUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<AttendanceRecord>();
        var attendanceDate = dto.AttendanceDate == default ? DateOnly.FromDateTime(DateTime.Today) : dto.AttendanceDate;

        var studentGroupId = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
            .Query()
            .Where(s => s.Id == dto.StudentId && !s.IsDeleted)
            .Select(s => s.StudentGroupId)
            .FirstOrDefaultAsync(cancellationToken);

        await _authorizationService.EnsureCurrentUserCanManageAttendanceAsync(dto.SchoolClassId, dto.SectionId, studentGroupId, 0, cancellationToken);
        await EnsureSessionWritableAsync(dto.SchoolClassId, dto.SectionId, studentGroupId, attendanceDate, cancellationToken);
        await ValidateRosterAsync(dto.StudentId, dto.SchoolClassId, dto.SectionId, studentGroupId, cancellationToken);

        var duplicate = await repo.Query().AnyAsync(
            x => x.StudentId == dto.StudentId && x.AttendanceDate == attendanceDate && !x.IsDeleted,
            cancellationToken);

        if (duplicate)
            throw new InvalidOperationException("Attendance already exists for this student on the selected date.");

        var entity = new AttendanceRecord
        {
            CreatedBy     = createdBy,
            StudentId     = dto.StudentId,
            SchoolClassId = dto.SchoolClassId,
            SectionId     = dto.SectionId,
            Status        = dto.Status,
            Remarks       = dto.Remarks,
            AttendanceDate = attendanceDate
        };
        await repo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(AttendanceRecordUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<AttendanceRecord>();
        var entity = await repo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("AttendanceRecord not found.");

        var attendanceDate = dto.AttendanceDate == default ? entity.AttendanceDate : dto.AttendanceDate;

        var studentGroupId = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
            .Query()
            .Where(s => s.Id == dto.StudentId && !s.IsDeleted)
            .Select(s => s.StudentGroupId)
            .FirstOrDefaultAsync(cancellationToken);

        await _authorizationService.EnsureCurrentUserCanManageAttendanceAsync(dto.SchoolClassId, dto.SectionId, studentGroupId, 0, cancellationToken);
        await EnsureSessionWritableAsync(dto.SchoolClassId, dto.SectionId, studentGroupId, attendanceDate, cancellationToken);
        await ValidateRosterAsync(dto.StudentId, dto.SchoolClassId, dto.SectionId, studentGroupId, cancellationToken);

        if (entity.StudentId != dto.StudentId || entity.AttendanceDate != attendanceDate)
        {
            var duplicate = await repo.Query().AnyAsync(
                x => x.Id != dto.Id && x.StudentId == dto.StudentId && x.AttendanceDate == attendanceDate && !x.IsDeleted,
                cancellationToken);
            if (duplicate)
                throw new InvalidOperationException("Attendance already exists for this student on the selected date.");
        }

        entity.StudentId     = dto.StudentId;
        entity.SchoolClassId = dto.SchoolClassId;
        entity.SectionId     = dto.SectionId;
        entity.Status        = dto.Status;
        entity.Remarks       = dto.Remarks;
        entity.AttendanceDate = attendanceDate;
        entity.UpdatedBy     = updatedBy;
        entity.UpdatedAt     = DateTime.UtcNow;

        repo.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<AttendanceRecord>()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("AttendanceRecord not found.");

        var studentGroupId = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
            .Query()
            .Where(s => s.Id == entity.StudentId && !s.IsDeleted)
            .Select(s => s.StudentGroupId)
            .FirstOrDefaultAsync(cancellationToken);

        await EnsureSessionWritableAsync(entity.SchoolClassId, entity.SectionId, studentGroupId, entity.AttendanceDate, cancellationToken);
        await _authorizationService.EnsureCurrentUserCanManageAttendanceAsync(entity.SchoolClassId, entity.SectionId, studentGroupId, 0, cancellationToken);

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<AttendanceRecord>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureSessionWritableAsync(int classId, int sectionId, int? studentGroupId, DateOnly date, CancellationToken ct)
    {
        var sessionRepo = _unitOfWork.Repository<AttendanceSession>();
        var query = sessionRepo.Query()
            .Where(s => s.SchoolClassId == classId && s.SectionId == sectionId && s.AttendanceDate == date && !s.IsDeleted);

        query = studentGroupId.HasValue
            ? query.Where(s => s.StudentGroupId == studentGroupId)
            : query.Where(s => s.StudentGroupId == null);

        var session = await query.FirstOrDefaultAsync(ct);
        if (session == null) return;

        if (session.Status == AttendanceSessionStatus.Locked || session.Status == AttendanceSessionStatus.Approved)
            throw new InvalidOperationException("Attendance session is locked and cannot be modified.");
    }

    private async Task ValidateRosterAsync(int studentId, int classId, int sectionId, int? studentGroupId, CancellationToken ct)
    {
        var student = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted, ct)
            ?? throw new InvalidOperationException("Student not found.");

        if (student.ClassId != classId || student.SectionId != sectionId || student.StudentGroupId != studentGroupId)
        {
            throw new InvalidOperationException("Student is not assigned to the selected class, section and group.");
        }
    }
}
