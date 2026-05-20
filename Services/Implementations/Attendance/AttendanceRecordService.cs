using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;

namespace SchoolManagementSystem.Services.Implementations.Attendance;

public class AttendanceRecordService : IAttendanceRecordService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAttendanceRepository _attendanceRepository;

<<<<<<< HEAD
    public AttendanceRecordService(IUnitOfWork unitOfWork, IAttendanceRepository attendanceRepository) 
    { 
        _unitOfWork = unitOfWork;
        _attendanceRepository = attendanceRepository;
    }

    public async Task<PagedResult<AttendanceRecordListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? studentId = null, CancellationToken cancellationToken = default)
=======
    public AttendanceRecordService(IUnitOfWork unitOfWork, IAttendanceRepository attendanceRepository)
>>>>>>> d8b24e6 (attendece and website curtomize)
    {
        _unitOfWork            = unitOfWork;
        _attendanceRepository  = attendanceRepository;
    }

    public async Task<PagedResult<AttendanceRecordListItemDto>> GetPagedAsync(
        int page, int pageSize, string? search,
        int? studentId       = null,
        int? classId         = null,
        int? sectionId       = null,
        DateOnly? attendanceDate = null,
        CancellationToken cancellationToken = default)
    {
        page     = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);

<<<<<<< HEAD
        var (items, totalCount) = await _attendanceRepository.GetListByStoredProcedureAsync(page, pageSize, search, studentId ?? 0, cancellationToken);
=======
        var (items, totalCount) = await _attendanceRepository.GetListByStoredProcedureAsync(
            page, pageSize, search,
            studentId ?? 0,
            classId   ?? 0,
            sectionId ?? 0,
            attendanceDate,
            cancellationToken);
>>>>>>> d8b24e6 (attendece and website curtomize)

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
<<<<<<< HEAD
        var entity = await _unitOfWork.Repository<AttendanceRecord>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new AttendanceRecordUpsertDto { Id = entity.Id, StudentId = entity.StudentId, SchoolClassId = entity.SchoolClassId, SectionId = entity.SectionId, Status = entity.Status, Remarks = entity.Remarks ?? string.Empty };
=======
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
            Remarks       = entity.Remarks ?? string.Empty
        };
>>>>>>> d8b24e6 (attendece and website curtomize)
    }

    public async Task<int> CreateAsync(AttendanceRecordUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
<<<<<<< HEAD
        var entity = new AttendanceRecord { CreatedBy = createdBy, StudentId = dto.StudentId, SchoolClassId = dto.SchoolClassId, SectionId = dto.SectionId, Status = dto.Status, Remarks = dto.Remarks };
=======
        var entity = new AttendanceRecord
        {
            CreatedBy     = createdBy,
            StudentId     = dto.StudentId,
            SchoolClassId = dto.SchoolClassId,
            SectionId     = dto.SectionId,
            Status        = dto.Status,
            Remarks       = dto.Remarks,
            AttendanceDate = DateOnly.FromDateTime(DateTime.Today)
        };
>>>>>>> d8b24e6 (attendece and website curtomize)
        await _unitOfWork.Repository<AttendanceRecord>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(AttendanceRecordUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
<<<<<<< HEAD
        var entity = await _unitOfWork.Repository<AttendanceRecord>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("AttendanceRecord not found.");
        entity.StudentId = dto.StudentId;
        entity.SchoolClassId = dto.SchoolClassId;
        entity.SectionId = dto.SectionId;
        entity.Status = dto.Status;
        entity.Remarks = dto.Remarks;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
=======
        var entity = await _unitOfWork.Repository<AttendanceRecord>()
            .FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("AttendanceRecord not found.");

        entity.StudentId     = dto.StudentId;
        entity.SchoolClassId = dto.SchoolClassId;
        entity.SectionId     = dto.SectionId;
        entity.Status        = dto.Status;
        entity.Remarks       = dto.Remarks;
        entity.UpdatedBy     = updatedBy;
        entity.UpdatedAt     = DateTime.UtcNow;

>>>>>>> d8b24e6 (attendece and website curtomize)
        _unitOfWork.Repository<AttendanceRecord>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
<<<<<<< HEAD
        var entity = await _unitOfWork.Repository<AttendanceRecord>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("AttendanceRecord not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
=======
        var entity = await _unitOfWork.Repository<AttendanceRecord>()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("AttendanceRecord not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

>>>>>>> d8b24e6 (attendece and website curtomize)
        _unitOfWork.Repository<AttendanceRecord>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
<<<<<<< HEAD


=======
>>>>>>> d8b24e6 (attendece and website curtomize)
