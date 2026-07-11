using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class TransferService : ITransferService
{
    private readonly IUnitOfWork _uow;

    public TransferService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<TransferCertificateListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var query = _uow.Repository<TransferCertificate>().Query().AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(x => x.CertificateNo.ToLower().Contains(term)
                || x.NewSchoolName.ToLower().Contains(term));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.IssueDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TransferCertificateListItemDto
            {
                Id = x.Id,
                CertificateNo = x.CertificateNo,
                StudentId = x.StudentId,
                OldClassId = x.OldClassId,
                OldSectionId = x.OldSectionId,
                NewSchoolName = x.NewSchoolName,
                IssueDate = x.IssueDate,
                Reason = x.Reason,
                IsActive = x.IsActive,
                TotalRecords = total
            })
            .ToListAsync(ct);

        return items;
    }

    public async Task<TransferCertificateUpsertDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<TransferCertificate>()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity is null) return null;

        return new TransferCertificateUpsertDto
        {
            Id = entity.Id,
            StudentId = entity.StudentId,
            OldClassId = entity.OldClassId,
            OldSectionId = entity.OldSectionId,
            NewSchoolName = entity.NewSchoolName,
            CertificateNo = entity.CertificateNo,
            IssueDate = entity.IssueDate,
            Reason = entity.Reason,
            IsActive = entity.IsActive
        };
    }

    public async Task<int> CreateAsync(TransferCertificateUpsertDto dto, string createdBy, CancellationToken ct = default)
    {
        var entity = new TransferCertificate
        {
            StudentId = dto.StudentId,
            OldClassId = dto.OldClassId,
            OldSectionId = dto.OldSectionId,
            NewSchoolName = dto.NewSchoolName,
            CertificateNo = dto.CertificateNo,
            IssueDate = dto.IssueDate,
            Reason = dto.Reason,
            IsActive = dto.IsActive,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<TransferCertificate>().AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(TransferCertificateUpsertDto dto, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<TransferCertificate>()
            .FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Transfer certificate not found.");

        entity.OldClassId = dto.OldClassId;
        entity.OldSectionId = dto.OldSectionId;
        entity.NewSchoolName = dto.NewSchoolName;
        entity.CertificateNo = dto.CertificateNo;
        entity.IssueDate = dto.IssueDate;
        entity.Reason = dto.Reason;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<TransferCertificate>()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Transfer certificate not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<int> ProcessTransferAsync(TransferCertificateUpsertDto dto, string createdBy, CancellationToken ct = default)
    {
        int certificateId = 0;

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            // 1. Create TransferCertificate
            var entity = new TransferCertificate
            {
                StudentId = dto.StudentId,
                OldClassId = dto.OldClassId,
                OldSectionId = dto.OldSectionId,
                NewSchoolName = dto.NewSchoolName,
                CertificateNo = dto.CertificateNo,
                IssueDate = dto.IssueDate,
                Reason = dto.Reason,
                IsActive = dto.IsActive,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Repository<TransferCertificate>().AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            certificateId = entity.Id;

            // 2. Update Student
            var now = DateTime.UtcNow;
            await _uow.Repository<Student>().Query()
                .Where(s => s.Id == dto.StudentId && !s.IsDeleted)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.ClassId, dto.NewClassId)
                    .SetProperty(x => x.SectionId, dto.NewSectionId)
                    .SetProperty(x => x.StudentGroupId, dto.NewStudentGroupId)
                    .SetProperty(x => x.Status, StudentStatus.Transferred)
                    .SetProperty(x => x.UpdatedBy, createdBy)
                    .SetProperty(x => x.UpdatedAt, now), ct);

            // 3. Resolve active academic year before cascade
            var activeYear = await _uow.Repository<AcademicYear>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct)
                ?? throw new InvalidOperationException("No active academic year found. Cannot complete transfer.");

            // 4. Update AttendanceRecord (no academic year filter - entity doesn't have AcademicYearId)
            await _uow.Repository<AttendanceRecord>().Query()
                .Where(ar => ar.StudentId == dto.StudentId)
                .ExecuteUpdateAsync(ar => ar
                    .SetProperty(x => x.SchoolClassId, dto.NewClassId)
                    .SetProperty(x => x.SectionId, dto.NewSectionId), ct);

            // 5. Cascade: update StudentExamResult and StudentSubjectResult (scoped to active academic year)
            var studentExamResultRepo = _uow.Repository<StudentExamResult>();
            var studentSubjectResultRepo = _uow.Repository<StudentSubjectResult>();

            await studentExamResultRepo.Query()
                .Where(r => r.StudentId == dto.StudentId && r.AcademicYearId == activeYear.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.ClassId, dto.NewClassId)
                    .SetProperty(x => x.SectionId, dto.NewSectionId)
                    .SetProperty(x => x.StudentGroupId, dto.NewStudentGroupId), ct);

            await studentSubjectResultRepo.Query()
                .Where(r => r.StudentId == dto.StudentId && r.AcademicYearId == activeYear.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.ClassId, dto.NewClassId)
                    .SetProperty(x => x.SectionId, dto.NewSectionId)
                    .SetProperty(x => x.StudentGroupId, dto.NewStudentGroupId), ct);

            // 6. Update StudentGroupAssignment — soft-delete existing (scoped to active academic year)
            await _uow.Repository<StudentGroupAssignment>().Query()
                .Where(sga => sga.StudentId == dto.StudentId && sga.AcademicYearId == activeYear.Id && !sga.IsDeleted)
                .ExecuteUpdateAsync(sga => sga
                    .SetProperty(x => x.IsDeleted, true)
                    .SetProperty(x => x.UpdatedBy, createdBy)
                    .SetProperty(x => x.UpdatedAt, now), ct);

            // 7. Create new StudentGroupAssignment if group specified
            if (dto.NewStudentGroupId.HasValue)
            {
                var newAssignment = new StudentGroupAssignment
                {
                    StudentId = dto.StudentId,
                    StudentGroupId = dto.NewStudentGroupId.Value,
                    SchoolClassId = dto.NewClassId,
                    AcademicYearId = activeYear.Id,
                    AssignedDate = DateTime.Now,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Repository<StudentGroupAssignment>().AddAsync(newAssignment, ct);
            }
        }, ct);

        return certificateId;
    }
}
