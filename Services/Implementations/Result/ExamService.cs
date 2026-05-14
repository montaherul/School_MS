using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ExamService : IExamService
{
    private readonly IUnitOfWork _uow;
    private readonly IExamRepository _examRepository;
    private readonly IGradingRuleRepository _gradingRepository;

    public ExamService(IUnitOfWork uow, IExamRepository examRepository, IGradingRuleRepository gradingRepository)
    {
        _uow = uow;
        _examRepository = examRepository;
        _gradingRepository = gradingRepository;
    }

    /// <summary>
    /// Get all exams for admin view, optionally filtered by academic year
    /// </summary>
    public async Task<IEnumerable<ExamUpsertDto>> GetExamsAsync(int academicYearId)
    {
        return await _examRepository.GetExamsForAdminAsync(academicYearId, default);
    }

    /// <summary>
    /// Create a new exam
    /// </summary>
    public async Task<object?> CreateExamAsync(ExamUpsertDto dto, CancellationToken ct = default)
    {
        var exam = new Exam
        {
            Name = dto.Name,
            Term = dto.Term,
            AcademicYearId = dto.AcademicYearId,
            StartsOn = dto.StartsOn,
            EndsOn = dto.EndsOn,
            Status = ResultWorkflowStatus.Draft
        };
        await _uow.Repository<Exam>().AddAsync(exam);
        await _uow.SaveChangesAsync(ct);
        
        return new { exam.Id, exam.Name, exam.Term, exam.Status };
    }

    /// <summary>
    /// Update existing exam details
    /// </summary>
    public async Task<object?> UpdateExamAsync(int examId, ExamUpsertDto dto, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<Exam>().GetByIdAsync(examId, ct);
        if (exam == null)
            throw new KeyNotFoundException($"Exam with ID {examId} not found");

        exam.Name = dto.Name;
        exam.Term = dto.Term;
        exam.StartsOn = dto.StartsOn;
        exam.EndsOn = dto.EndsOn;

        _uow.Repository<Exam>().Update(exam);
        await _uow.SaveChangesAsync(ct);

        return new { exam.Id, exam.Name, exam.Term, exam.Status };
    }

    /// <summary>
    /// Delete an exam
    /// </summary>
    public async Task DeleteExamAsync(int examId, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<Exam>().GetByIdAsync(examId, ct);
        if (exam == null)
            throw new KeyNotFoundException($"Exam with ID {examId} not found");

        _uow.Repository<Exam>().Remove(exam);
        await _uow.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Get exam by ID
    /// </summary>
    public async Task<object?> GetExamByIdAsync(int examId, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<Exam>().GetByIdAsync(examId, ct);
        return exam == null ? null : new 
        { 
            exam.Id, 
            exam.Name,
            exam.Term,
            exam.Status,
            exam.StartsOn,
            exam.EndsOn
        };
    }

    /// <summary>
    /// Get all grading rules
    /// </summary>
    public async Task<IEnumerable<GradingRuleUpsertDto>> GetGradingRulesAsync(CancellationToken ct = default)
    {
        var rules = await _gradingRepository.ListAsync(null, ct);
        return rules.Select(x => new GradingRuleUpsertDto
        {
            Id = x.Id,
            Grade = x.Grade,
            MinMarks = x.MinMarks,
            MaxMarks = x.MaxMarks,
            GradePoint = x.GradePoint
        });
    }

    /// <summary>
    /// Create or update a grading rule
    /// </summary>
    public async Task<object?> UpsertGradingRuleAsync(GradingRuleUpsertDto dto, CancellationToken ct = default)
    {
        if (dto.Id.HasValue)
        {
            var rule = await _gradingRepository.GetByIdAsync(dto.Id.Value, ct);
            if (rule != null)
            {
                rule.Grade = dto.Grade;
                rule.MinMarks = dto.MinMarks;
                rule.MaxMarks = dto.MaxMarks;
                rule.GradePoint = dto.GradePoint;
                _gradingRepository.Update(rule);
                await _uow.SaveChangesAsync(ct);
                return new { rule.Id, rule.Grade, rule.GradePoint };
            }
        }

        var newRule = new GradingRule
        {
            Grade = dto.Grade,
            MinMarks = dto.MinMarks,
            MaxMarks = dto.MaxMarks,
            GradePoint = dto.GradePoint
        };
        await _gradingRepository.AddAsync(newRule, ct);
        await _uow.SaveChangesAsync(ct);

        return new { newRule.Id, newRule.Grade, newRule.GradePoint };
    }

    /// <summary>
    /// Delete a grading rule
    /// </summary>
    public async Task DeleteGradingRuleAsync(int ruleId, CancellationToken ct = default)
    {
        var rule = await _gradingRepository.GetByIdAsync(ruleId, ct);
        if (rule == null)
            throw new KeyNotFoundException($"Grading rule with ID {ruleId} not found");

        _gradingRepository.Remove(rule);
        await _uow.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Lock an exam to prevent result modifications
    /// </summary>
    public async Task LockExamAsync(int examId, int userId, string? reason = null, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<Exam>().GetByIdAsync(examId, ct);
        if (exam == null)
            throw new KeyNotFoundException($"Exam with ID {examId} not found");

        exam.IsLocked = true;

        var resultLock = new ResultLock
        {
            ExamId = examId,
            LockedByUserId = userId,
            LockedAt = DateTime.Now,
            Reason = reason
        };

        _uow.Repository<Exam>().Update(exam);
        await _uow.Repository<ResultLock>().AddAsync(resultLock, ct);
        await _uow.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Unlock an exam to allow result modifications
    /// </summary>
    public async Task UnlockExamAsync(int examId, string? reason = null, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<Exam>().GetByIdAsync(examId, ct);
        if (exam == null)
            throw new KeyNotFoundException($"Exam with ID {examId} not found");

        exam.IsLocked = false;
        _uow.Repository<Exam>().Update(exam);
        await _uow.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Get complete exam status including lock status and result publication status
    /// </summary>
    public async Task<object?> GetExamStatusAsync(int examId, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<Exam>().GetByIdAsync(examId, ct);
        if (exam == null)
            return null;

        var resultPub = await _uow.Repository<ResultPublication>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(rp => rp.ExamId == examId, ct);

        return new
        {
            exam.Id,
            exam.Name,
            exam.Status,
            exam.IsLocked,
            PublicationStatus = resultPub?.Status,
            PublishedAt = resultPub?.PublishedAt
        };
    }

    /// <summary>
    /// Get all active subjects
    /// </summary>
    public async Task<IEnumerable<object>> GetSubjectsAsync(CancellationToken ct = default)
    {
        return await _uow.Repository<Subject>().Query()
            .AsNoTracking()
            .Where(s => !s.IsDeleted)
            .Select(s => new { s.Id, s.Name, s.Code })
            .ToListAsync(ct);
    }

    /// <summary>
    /// Get subjects assigned to a class
    /// </summary>
    public async Task<IEnumerable<object>> GetSubjectsByClassIdAsync(int classId, CancellationToken ct = default)
    {
        return await _uow.Repository<ClassSubject>().Query()
            .AsNoTracking()
            .Include(cs => cs.Subject)
            .Where(cs => cs.SchoolClassId == classId && !cs.IsDeleted)
            .Select(cs => new
            {
                subjectId = cs.SubjectId,
                subjectName = cs.Subject!.Name
            })
            .Distinct()
            .ToListAsync(ct);
    }

    /// <summary>
    /// Get all school classes
    /// </summary>
    public async Task<IEnumerable<object>> GetClassesAsync(CancellationToken ct = default)
    {
        return await _uow.Repository<SchoolClass>().Query()
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .Select(c => new { c.Id, c.Name })
            .ToListAsync(ct);
    }

    /// <summary>
    /// Get sections, optionally filtered by class
    /// </summary>
    public async Task<IEnumerable<object>> GetSectionsAsync(int? classId = null, CancellationToken ct = default)
    {
        var query = _uow.Repository<Section>().Query().AsNoTracking().Where(s => !s.IsDeleted);
        if (classId.HasValue) query = query.Where(s => s.SchoolClassId == classId.Value);
        return await query.Select(s => new { s.Id, s.Name }).ToListAsync(ct);
    }
}

