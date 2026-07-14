using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class PromotioSessionService : IPromotioSessionService
{
    private readonly IUnitOfWork _uow;
    private readonly IPromotioSessionRepository _sessionRepo;
    private readonly IClassProgressionRuleRepository _progressionRuleRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PromotioSessionService(
        IUnitOfWork uow,
        IPromotioSessionRepository sessionRepo,
        IClassProgressionRuleRepository progressionRuleRepo,
        IHttpContextAccessor httpContextAccessor)
    {
        _uow = uow;
        _sessionRepo = sessionRepo;
        _progressionRuleRepo = progressionRuleRepo;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<PromotioSessionListItemDto>> GetPagedAsync(int page, int size, string? search, string? status, CancellationToken ct = default)
    {
        var items = await _sessionRepo.GetPagedSessionsAsync(page, size, search, status, ct);
        var totalItems = items.Count > 0 ? items[0].TotalRecords : 0;
        return new PagedResult<PromotioSessionListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = size,
            TotalItems = totalItems
        };
    }

    public async Task<PromotioSessionUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default)
    {
        var entity = await _sessionRepo.GetByIdAsync(id, ct);
        if (entity == null) return null;
        return new PromotioSessionUpsertDto
        {
            Id = entity.Id,
            SessionName = entity.SessionName,
            AcademicYearId = entity.AcademicYearId,
            PromotionDate = entity.PromotionDate,
            Remarks = entity.Remarks
        };
    }

    public async Task<PromotioSession?> GetSessionWithAcademicYearAsync(int id, CancellationToken ct = default)
    {
        return await _uow.Repository<PromotioSession>().Query()
            .Include(s => s.AcademicYear)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);
    }

    public async Task<int> CreateAsync(PromotioSessionUpsertDto dto, string userId, CancellationToken ct = default)
    {
        var entity = new PromotioSession
        {
            AcademicYearId = dto.AcademicYearId,
            SessionName = dto.SessionName,
            PromotionDate = dto.PromotionDate,
            Remarks = dto.Remarks,
            Status = "Draft",
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        await _sessionRepo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        await AuditLogAsync("PromotionSession", "Create", $"Created promotion session '{entity.SessionName}' (ID: {entity.Id})", ct);
        return entity.Id;
    }

    public async Task UpdateAsync(PromotioSessionUpsertDto dto, string userId, CancellationToken ct = default)
    {
        var entity = await _sessionRepo.GetByIdAsync(dto.Id!.Value, ct);
        if (entity == null) throw new ArgumentException("Session not found");
        if (entity.Status != "Draft") throw new InvalidOperationException("Only draft sessions can be edited");

        entity.SessionName = dto.SessionName;
        entity.AcademicYearId = dto.AcademicYearId;
        entity.PromotionDate = dto.PromotionDate;
        entity.Remarks = dto.Remarks;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        _sessionRepo.Update(entity);
        await _uow.SaveChangesAsync(ct);
        await AuditLogAsync("PromotionSession", "Update", $"Updated promotion session '{entity.SessionName}' (ID: {entity.Id})", ct);
    }

    public async Task DeleteAsync(int id, string userId, CancellationToken ct = default)
    {
        var entity = await _sessionRepo.GetByIdAsync(id, ct);
        if (entity == null) throw new ArgumentException("Session not found");
        entity.IsDeleted = true;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        _sessionRepo.Update(entity);
        await _uow.SaveChangesAsync(ct);
        await AuditLogAsync("PromotionSession", "Delete", $"Deleted promotion session '{entity.SessionName}' (ID: {entity.Id})", ct);
    }

    public async Task<List<PromotioCandidateDto>> GetCandidatesAsync(int classId, int academicYearId, string? search, CancellationToken ct = default)
    {
        var sql = "EXEC sp_GetPromotionCandidates @p0, @p1, @p2, @p3, @p4";
        return await _sessionRepo.ExecuteStoredProcAsync<PromotioCandidateDto>(
            sql, classId, academicYearId, 1.00m, 60.00m, search ?? (object)DBNull.Value);
    }

    public async Task<PromotioResult> BulkPromoteAsync(int sessionId, int fromClassId, int toClassId, int academicYearId, string userId, CancellationToken ct = default)
    {
        var result = new PromotioResult
        {
            SessionId = sessionId,
            FromClassId = fromClassId,
            ToClassId = toClassId
        };

        var session = await _sessionRepo.GetByIdAsync(sessionId, ct);
        if (session == null) throw new ArgumentException("Session not found");
        if (session.Status != "Draft") throw new InvalidOperationException("Only draft sessions can execute promotions");

        var toClass = await _uow.Repository<SchoolClass>().GetByIdAsync(toClassId);
        if (toClass == null) throw new ArgumentException("Target class not found");

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var candidates = await _uow.Repository<FinalResult>().Query()
                .Where(f => f.AcademicYearId == academicYearId && f.PromotionStatus == PromotionStatus.Pending && f.IsPassed
                    && _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query().Any(s => s.Id == f.StudentId && s.ClassId == fromClassId && !s.IsDeleted && s.Status == StudentStatus.Active))
                .Include(f => f.Student).ThenInclude(s => s.Section)
                .ToListAsync(ct);

            var students = candidates.Select(c => c.Student).Distinct().ToList();
            result.TotalCandidates = students.Count;

            foreach (var student in students)
            {
                try
                {
                    var finalResult = candidates.FirstOrDefault(c => c.StudentId == student.Id);
                    if (finalResult == null)
                    {
                        result.ErrorCount++;
                        result.Errors.Add($"Student {student.FullName} has no final result");
                        continue;
                    }

                    var newSection = await _uow.Repository<Section>()
                        .FirstOrDefaultAsync(s => s.SchoolClassId == toClassId && !s.IsDeleted, ct);

                    int? newGroupId = null;
                    if (toClass.IsGroupBased && student.StudentGroupId.HasValue)
                    {
                        newGroupId = student.StudentGroupId.Value;
                    }

                    var history = new PromotionHistory
                    {
                        StudentId = student.Id,
                        FromClassId = fromClassId,
                        ToClassId = toClassId,
                        AcademicYearId = academicYearId,
                        PromotioSessionId = sessionId,
                        Status = PromotionStatus.Promoted,
                        PromotedAt = DateTime.UtcNow,
                        PromotedByUserId = int.TryParse(userId, out var uid) ? uid : null,
                        NewSectionId = newSection?.Id,
                        NewGroupId = newGroupId,
                        Remarks = "Bulk promoted via session"
                    };
                    await _uow.Repository<PromotionHistory>().AddAsync(history, ct);

                    student.ClassId = toClassId;
                    if (newSection != null)
                        student.SectionId = newSection.Id;
                    if (newGroupId.HasValue)
                        student.StudentGroupId = newGroupId;
                    _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Update(student);

                    finalResult.PromotionStatus = PromotionStatus.Promoted;
                    finalResult.PromotioSessionId = sessionId;
                    finalResult.PromotionRemarks = "Promoted via bulk session";
                    _uow.Repository<FinalResult>().Update(finalResult);

                    result.PromotedCount++;
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.Errors.Add($"Student {student.FullName}: {ex.Message}");
                }
            }

            session.ExecutedByUserId = int.TryParse(userId, out var execUid) ? execUid : null;
            session.ExecutedAt = DateTime.UtcNow;
            session.UpdatedBy = userId;
            session.UpdatedAt = DateTime.UtcNow;
            _sessionRepo.Update(session);

            await _uow.SaveChangesAsync(ct);
        });

        await AuditLogAsync("PromotionSession", "BulkPromote", $"Bulk promoted {result.PromotedCount} students (session ID: {sessionId})", ct);
        return result;
    }

    public async Task<List<ClassProgressionRuleDto>> GetProgressionRulesAsync(CancellationToken ct = default)
    {
        var rules = await _progressionRuleRepo.QueryNoTracking()
            .Include(r => r.FromClass)
            .Include(r => r.ToClass)
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.DisplayOrder)
            .ToListAsync(ct);

        return rules.Select(r => new ClassProgressionRuleDto
        {
            Id = r.Id,
            FromClassId = r.FromClassId,
            FromClassName = r.FromClass?.Name ?? "",
            ToClassId = r.ToClassId,
            ToClassName = r.ToClass?.Name ?? "",
            ProgressionType = r.ProgressionType,
            IsActive = r.IsActive,
            DisplayOrder = r.DisplayOrder
        }).ToList();
    }

    public async Task UpdateProgressionRulesAsync(List<ClassProgressionRuleUpsertDto> rules, string userId, CancellationToken ct = default)
    {
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var existing = await _progressionRuleRepo.ListAsync(r => !r.IsDeleted, ct);
            foreach (var old in existing)
            {
                old.IsDeleted = true;
                old.UpdatedBy = userId;
                old.UpdatedAt = DateTime.UtcNow;
                _progressionRuleRepo.Update(old);
            }

            foreach (var dto in rules)
            {
                var entity = new ClassProgressionRule
                {
                    FromClassId = dto.FromClassId,
                    ToClassId = dto.ToClassId,
                    ProgressionType = dto.ProgressionType,
                    IsActive = dto.IsActive,
                    DisplayOrder = dto.DisplayOrder,
                    CreatedBy = userId
                };
                await _progressionRuleRepo.AddAsync(entity, ct);
            }

            await _uow.SaveChangesAsync(ct);
        });

        await AuditLogAsync("ClassProgressionRule", "Update", $"Updated {rules.Count} progression rules", ct);
    }

    public async Task RollbackSessionAsync(int sessionId, string userId, CancellationToken ct = default)
    {
        var session = await _sessionRepo.GetByIdAsync(sessionId, ct);
        if (session == null) throw new ArgumentException("Session not found");
        if (session.Status != "Draft") throw new InvalidOperationException("Only draft sessions can be rolled back");

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var histories = await _uow.Repository<PromotionHistory>().Query()
                .Where(h => h.PromotioSessionId == sessionId && !h.IsDeleted)
                .ToListAsync(ct);

            foreach (var h in histories)
            {
                var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().GetByIdAsync(h.StudentId, ct);
                if (student != null)
                {
                    student.ClassId = h.FromClassId;
                    student.SectionId = h.NewSectionId ?? student.SectionId;
                    if (h.NewGroupId.HasValue)
                        student.StudentGroupId = null;
                    _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Update(student);
                }

                var finalResult = await _uow.Repository<FinalResult>()
                    .FirstOrDefaultAsync(f => f.StudentId == h.StudentId && f.AcademicYearId == h.AcademicYearId, ct);
                if (finalResult != null)
                {
                    finalResult.PromotionStatus = PromotionStatus.Pending;
                    finalResult.PromotioSessionId = null;
                    finalResult.PromotionRemarks = $"Rolled back (session: {session.SessionName})";
                    _uow.Repository<FinalResult>().Update(finalResult);
                }

                h.IsDeleted = true;
                h.UpdatedBy = userId;
                h.UpdatedAt = DateTime.UtcNow;
                _uow.Repository<PromotionHistory>().Update(h);
            }

            session.UpdatedBy = userId;
            session.UpdatedAt = DateTime.UtcNow;
            _sessionRepo.Update(session);
            await _uow.SaveChangesAsync(ct);
        });

        await AuditLogAsync("PromotionSession", "Rollback", $"Rolled back session '{session.SessionName}' (ID: {sessionId})", ct);
    }

    public async Task ApproveSessionAsync(int sessionId, string userId, CancellationToken ct = default)
    {
        var session = await _sessionRepo.GetByIdAsync(sessionId, ct);
        if (session == null) throw new ArgumentException("Session not found");
        if (session.Status != "Draft") throw new InvalidOperationException("Only draft sessions can be approved");

        session.Status = "Approved";
        session.ApprovedByUserId = int.TryParse(userId, out var uid) ? uid : null;
        session.ApprovedAt = DateTime.UtcNow;
        session.UpdatedBy = userId;
        session.UpdatedAt = DateTime.UtcNow;
        _sessionRepo.Update(session);
        await _uow.SaveChangesAsync(ct);
        await AuditLogAsync("PromotionSession", "Approve", $"Approved promotion session '{session.SessionName}' (ID: {sessionId})", ct);
    }

    public async Task<List<PromotioRegisterDto>> GetPromotioRegisterAsync(int sessionId, CancellationToken ct = default)
    {
        var histories = await _uow.Repository<PromotionHistory>().QueryNoTracking()
            .Include(h => h.Student)
            .Include(h => h.FromClass)
            .Include(h => h.ToClass)
            .Where(h => h.PromotioSessionId == sessionId && !h.IsDeleted)
            .OrderBy(h => h.PromotedAt)
            .ToListAsync(ct);

        return histories.Select(h => new PromotioRegisterDto
        {
            Id = h.Id,
            StudentId = h.StudentId,
            StudentName = h.Student?.FullName ?? "",
            StudentNo = h.Student?.StudentNo ?? "",
            FromClassName = h.FromClass?.Name ?? "",
            ToClassName = h.ToClass?.Name ?? "",
            NewRollNumber = h.NewRollNumber,
            Status = h.Status.ToString(),
            PromotedAt = h.PromotedAt,
            Remarks = h.Remarks
        }).ToList();
    }

    public async Task<List<FailedStudentDto>> GetFailedStudentsAsync(int academicYearId, int? classId, CancellationToken ct = default)
    {
        var query = _uow.Repository<FinalResult>().QueryNoTracking()
            .Include(f => f.Student).ThenInclude(s => s.Class)
            .Include(f => f.Student).ThenInclude(s => s.Section)
            .Where(f => f.AcademicYearId == academicYearId && !f.IsPassed && !f.IsDeleted);

        if (classId.HasValue)
            query = query.Where(f => f.SchoolClassId == classId.Value);

        var results = await query.OrderByDescending(f => f.TotalFailedSubjects).ToListAsync(ct);

        return results.Select(f => new FailedStudentDto
        {
            StudentId = f.StudentId,
            StudentName = f.Student?.FullName ?? "",
            StudentNo = f.Student?.StudentNo ?? "",
            ClassName = f.Student?.Class?.Name ?? "",
            SectionName = f.Student?.Section?.Name ?? "",
            GPA = f.FinalGpa,
            TotalFailedSubjects = f.TotalFailedSubjects,
            AttendancePercentage = f.AttendancePercentage
        }).ToList();
    }

    public async Task<PromotioDashboardDto> GetDashboardAsync(int academicYearId, CancellationToken ct = default)
    {
        var result = await _sessionRepo.ExecuteStoredProcAsync<PromotioDashboardDto>(
            "EXEC sp_GetPromotionDashboard @p0", academicYearId);
        return result.Count > 0 ? result[0] : new PromotioDashboardDto();
    }

    private async Task AuditLogAsync(string module, string action, string details, CancellationToken ct = default)
    {
        var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        var audit = new AuditLog
        {
            Module = module,
            Action = action,
            Details = details,
            CreatedBy = username,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.Repository<AuditLog>().AddAsync(audit, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
