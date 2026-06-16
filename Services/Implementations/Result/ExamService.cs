using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;
using ExamSubjectEntity = SchoolManagementSystem.Models.Entities.Exam.ExamSubject;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ExamService : IExamService
{
    private readonly IUnitOfWork _uow;
    private readonly IExamRepository _examRepository;
    private readonly IGradingRuleRepository _gradingRepository;
    private readonly IExamValidationService _examValidation;

    public ExamService(IUnitOfWork uow, IExamRepository examRepository, IGradingRuleRepository gradingRepository, IExamValidationService examValidation)
    {
        _uow = uow;
        _examRepository = examRepository;
        _gradingRepository = gradingRepository;
        _examValidation = examValidation;
    }

    public async Task<IEnumerable<ExamListDto>> GetExamsAsync(int academicYearId, CancellationToken ct = default)
        => await _examRepository.GetExamsForAdminAsync(academicYearId, ct);

    public async Task<(IEnumerable<ExamListDto> Items, int TotalCount)> GetPagedExamsAsync(
        int academicYearId, string? searchTerm, int? status,
        int pageNumber, int pageSize, string sortColumn, string sortDirection,
        CancellationToken ct = default)
        => await _examRepository.GetExamListAsync(academicYearId, searchTerm, status, pageNumber, pageSize, sortColumn, sortDirection, ct);

    public Task<ExamDetailsDto?> GetExamDetailsAsync(int examId, CancellationToken ct = default)
        => _examRepository.GetExamDetailsAsync(examId, ct);

    public async Task<ExamUpsertDto?> GetExamForEditAsync(int examId, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId, ct);
        if (exam == null) return null;

        var subjects = await _uow.Repository<ExamSubjectEntity>().Query()
            .Where(es => es.ExamId == examId && !es.IsDeleted)
            .Select(es => new SubjectMarkConfigDto
            {
                SubjectId = es.SubjectId,
                FullMarks = es.FullMarks,
                PassMarks = es.PassMarks,
                IsOptional = es.IsOptional
            })
            .ToListAsync(ct);

        return new ExamUpsertDto
        {
            Id = exam.Id,
            Name = exam.Name,
            Term = exam.Term,
            AcademicYearId = exam.AcademicYearId,
            ClassId = exam.ClassId,
            SectionId = exam.SectionId,
            StudentGroupId = exam.StudentGroupId,
            StartsOn = exam.StartsOn,
            EndsOn = exam.EndsOn,
            Status = exam.Status,
            IsLocked = exam.IsLocked,
            Subjects = subjects
        };
    }

    /// <summary>
    /// Create a new exam with subjects and mark configuration
    /// </summary>
    public async Task<object?> CreateExamAsync(ExamUpsertDto dto, CancellationToken ct = default)
    {
        var repo = _uow.Repository<ExamEntity>();
        if (await repo.AnyAsync(e => e.Name == dto.Name && e.AcademicYearId == dto.AcademicYearId && e.ClassId == dto.ClassId && e.StudentGroupId == dto.StudentGroupId, ct))
            throw new InvalidOperationException($"An exam named '{dto.Name}' already exists for this class and academic year.");

        // Validate Bangladesh Group Rules
        await _examValidation.ValidateBangladeshGroupRulesAsync(dto.ClassId, dto.StudentGroupId, ct);

        var exam = new ExamEntity
        {
            Name = dto.Name,
            Term = dto.Term,
            AcademicYearId = dto.AcademicYearId,
            ClassId = dto.ClassId,
            SectionId = dto.SectionId,
            StartsOn = dto.StartsOn,
            EndsOn = dto.EndsOn,
            Status = ResultWorkflowStatus.Draft,
            StudentGroupId = dto.StudentGroupId
        };
        await _uow.Repository<ExamEntity>().AddAsync(exam);
        await _uow.SaveChangesAsync(ct);

        if (dto.Subjects != null && dto.Subjects.Count > 0)
        {
            foreach (var s in dto.Subjects)
            {
                var examSubject = new ExamSubjectEntity
                {
                    ExamId = exam.Id,
                    SubjectId = s.SubjectId,
                    ClassId = dto.ClassId,
                    StudentGroupId = dto.StudentGroupId,
                    FullMarks = s.FullMarks,
                    PassMarks = s.PassMarks,
                    IsOptional = s.IsOptional
                };
                await _uow.Repository<ExamSubjectEntity>().AddAsync(examSubject);
            }
            await _uow.SaveChangesAsync(ct);
        }
        
        return new { exam.Id, exam.Name, exam.Term, exam.Status };
    }

    /// <summary>
    /// Create exams for multiple classes/groups in bulk.
    /// </summary>
    public async Task<List<object?>> CreateExamsBulkAsync(ExamUpsertDto dto, CancellationToken ct = default)
    {
        var results = new List<object?>();
        var classIds = dto.SelectedClassIds ?? [];
        var groupIds = dto.SelectedGroupIds ?? [];
        var sectionIds = dto.SelectedSectionIds ?? [];

        foreach (var classId in classIds)
        {
            var batchGroupIds = groupIds.Count == classIds.Count
                ? new List<int?> { groupIds[classIds.IndexOf(classId)] }
                : new List<int?> { dto.StudentGroupId };

            if (batchGroupIds.Count == 1 && batchGroupIds[0] == null)
                batchGroupIds.Clear();

            foreach (var groupId in batchGroupIds)
            {
                var clone = new ExamUpsertDto
                {
                    Name = dto.Name,
                    Term = dto.Term,
                    AcademicYearId = dto.AcademicYearId,
                    ClassId = classId,
                    SectionId = sectionIds.Count == classIds.Count
                        ? sectionIds[classIds.IndexOf(classId)]
                        : dto.SectionId,
                    StudentGroupId = groupId,
                    StartsOn = dto.StartsOn,
                    EndsOn = dto.EndsOn,
                    Status = dto.Status,
                    Subjects = dto.Subjects?.Select(s => new SubjectMarkConfigDto
                    {
                        SubjectId = s.SubjectId,
                        FullMarks = s.FullMarks,
                        PassMarks = s.PassMarks,
                        IsOptional = s.IsOptional
                    }).ToList()
                };

                var result = await CreateExamAsync(clone, ct);
                results.Add(result);
            }
        }

        return results;
    }

    /// <summary>
    /// Update existing exam details and subjects
    /// </summary>
    public async Task<object?> UpdateExamAsync(int examId, ExamUpsertDto dto, CancellationToken ct = default)
    {
        var repo = _uow.Repository<ExamEntity>();
        if (await repo.AnyAsync(e => e.Name == dto.Name && e.AcademicYearId == dto.AcademicYearId && e.ClassId == dto.ClassId && e.StudentGroupId == dto.StudentGroupId && e.Id != examId, ct))
            throw new InvalidOperationException($"Another exam named '{dto.Name}' already exists for this class and academic year.");

        // Validate Bangladesh Group Rules
        await _examValidation.ValidateBangladeshGroupRulesAsync(dto.ClassId, dto.StudentGroupId, ct);

        var exam = await repo.GetByIdAsync(examId, ct);
        if (exam == null)
            throw new KeyNotFoundException($"Exam with ID {examId} not found");

        exam.Name = dto.Name;
        exam.Term = dto.Term;
        exam.AcademicYearId = dto.AcademicYearId;
        exam.ClassId = dto.ClassId;
        exam.SectionId = dto.SectionId;
        exam.StartsOn = dto.StartsOn;
        exam.EndsOn = dto.EndsOn;
        exam.StudentGroupId = dto.StudentGroupId;

        _uow.Repository<ExamEntity>().Update(exam);

        if (dto.Subjects != null)
        {
            var existingSubjects = await _uow.Repository<ExamSubjectEntity>().Query()
                .Where(es => es.ExamId == examId).ToListAsync(ct);
            foreach (var old in existingSubjects)
                _uow.Repository<ExamSubjectEntity>().Remove(old);

            foreach (var s in dto.Subjects)
            {
                var examSubject = new ExamSubjectEntity
                {
                    ExamId = exam.Id,
                    SubjectId = s.SubjectId,
                    ClassId = exam.ClassId,
                    StudentGroupId = exam.StudentGroupId,
                    FullMarks = s.FullMarks,
                    PassMarks = s.PassMarks,
                    IsOptional = s.IsOptional
                };
                await _uow.Repository<ExamSubjectEntity>().AddAsync(examSubject);
            }
        }

        await _uow.SaveChangesAsync(ct);

        return new { exam.Id, exam.Name, exam.Term, exam.Status };
    }

    /// <summary>
    /// Delete an exam
    /// </summary>
    public async Task DeleteExamAsync(int examId, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId, ct);
        if (exam == null)
            throw new KeyNotFoundException($"Exam with ID {examId} not found");

        _uow.Repository<ExamEntity>().Remove(exam);
        await _uow.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Get exam by ID
    /// </summary>
    public async Task<object?> GetExamByIdAsync(int examId, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId, ct);
        return exam == null ? null : new 
        { 
            exam.Id, 
            exam.Name,
            exam.Term,
            exam.Status,
            exam.StartsOn,
            exam.EndsOn,
            exam.StudentGroupId
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
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId, ct);
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

        _uow.Repository<ExamEntity>().Update(exam);
        await _uow.Repository<ResultLock>().AddAsync(resultLock, ct);
        await _uow.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Unlock an exam to allow result modifications
    /// </summary>
    public async Task UnlockExamAsync(int examId, string? reason = null, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId, ct);
        if (exam == null)
            throw new KeyNotFoundException($"Exam with ID {examId} not found");

        exam.IsLocked = false;
        _uow.Repository<ExamEntity>().Update(exam);
        await _uow.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Get complete exam status including lock status and result publication status
    /// </summary>
    public async Task<object?> GetExamStatusAsync(int examId, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId, ct);
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
    /// Get subjects assigned to a class with Bangladesh group filtering:
    /// Classes 1-8: show only General subjects (SubjectGroup = "" or "General")
    /// Classes 9-10: show subjects matching the student's group (Science/BusinessStudies/Humanities)
    /// </summary>
    public async Task<IEnumerable<object>> GetSubjectsByClassIdAsync(int classId, int? groupId = null, int? sectionId = null, CancellationToken ct = default)
    {
        var query = _uow.Repository<ClassSubject>().Query()
            .AsNoTracking()
            .Include(cs => cs.Subject)
            .Where(cs => cs.SchoolClassId == classId && !cs.IsDeleted);

        if (groupId.HasValue)
        {
            query = query.Where(cs => cs.StudentGroupId == groupId.Value);
        }

        // Filter by SubjectGroup based on Bangladesh curriculum rules
        var schoolClass = await _uow.Repository<SchoolClass>().GetByIdAsync(classId, ct);
        var classNumber = ExtractClassNumberFromName(schoolClass?.Name);
        if (classNumber >= 1 && classNumber <= 8)
        {
            // Classes 1-8: General subjects only
            query = query.Where(cs => cs.Subject != null && (
                cs.Subject.SubjectGroup == "" ||
                cs.Subject.SubjectGroup == "General" ||
                cs.Subject.SubjectGroup == null));
        }
        else if (classNumber >= 9 && classNumber <= 10 && groupId.HasValue)
        {
            // Classes 9-10: subjects matching the selected group
            var group = await _uow.Repository<StudentGroup>().GetByIdAsync(groupId.Value, ct);
            if (group != null)
            {
                var groupName = group.Name;
                query = query.Where(cs => cs.Subject != null && (
                    cs.Subject.SubjectGroup == groupName ||
                    cs.Subject.SubjectGroup == "" ||
                    cs.Subject.SubjectGroup == "Common" ||
                    cs.Subject.SubjectGroup == null));
            }
        }

        if (sectionId.HasValue)
        {
            query = query.Where(cs => cs.SectionId == sectionId.Value);
        }

        return await query.Select(cs => new
            {
                subjectId = cs.SubjectId,
                subjectName = cs.Subject!.Name
            })
            .Distinct()
            .ToListAsync(ct);
    }

    private static int ExtractClassNumberFromName(string? className)
    {
        if (string.IsNullOrEmpty(className)) return 0;
        var digits = new string(className.Where(char.IsDigit).ToArray());
        if (int.TryParse(digits, out var number) && number > 0 && number <= 12)
            return number;
        return 0;
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

    /// <summary>
    /// Dynamically generate exam subjects from ClassSubjectMappings based on NCTB curriculum.
    /// Considers class, group, religion, and optional subject assignments.
    /// </summary>
    public async Task<int> GenerateExamSubjectsFromCurriculumAsync(int examId, int classId, int? groupId = null, CancellationToken ct = default)
    {
        var classSubjects = await _uow.Repository<ClassSubject>().Query()
            .AsNoTracking()
            .Include(cs => cs.Subject)
            .Where(cs => cs.SchoolClassId == classId && !cs.IsDeleted && cs.IsActive)
            .ToListAsync(ct);

        // Filter by group for class 9-10
        if (groupId.HasValue)
        {
            classSubjects = classSubjects.Where(cs =>
                cs.StudentGroupId == null || cs.StudentGroupId == groupId.Value).ToList();
        }

        // Filter by SubjectGroup based on Bangladesh curriculum
        var classNumber = ExtractClassNumberFromName(
            _uow.Repository<SchoolClass>().Query().AsNoTracking()
                .Where(c => c.Id == classId).Select(c => c.Name).FirstOrDefault());
        if (classNumber >= 1 && classNumber <= 8)
        {
            classSubjects = classSubjects.Where(cs => cs.Subject == null ||
                string.IsNullOrEmpty(cs.Subject.SubjectGroup) ||
                cs.Subject.SubjectGroup == "General").ToList();
        }
        else if (classNumber >= 9 && classNumber <= 10 && groupId.HasValue)
        {
            var group = await _uow.Repository<StudentGroup>().GetByIdAsync(groupId.Value, ct);
            if (group != null)
            {
                classSubjects = classSubjects.Where(cs => cs.Subject == null ||
                    cs.Subject.SubjectGroup == group.Name ||
                    cs.Subject.SubjectGroup == "Common" ||
                    string.IsNullOrEmpty(cs.Subject.SubjectGroup)).ToList();
            }
        }

        // Remove religion subjects (they're added per-student based on religion)
        var nonReligionSubjects = classSubjects.Where(cs => !cs.IsReligionSubject).ToList();

        // Generate ExamSubject records scoped to this class/group
        var examSubjects = new List<ExamSubjectEntity>();
        foreach (var cs in nonReligionSubjects)
        {
            examSubjects.Add(new ExamSubjectEntity
            {
                ExamId = examId,
                SubjectId = cs.SubjectId,
                ClassId = classId,
                StudentGroupId = groupId,
                FullMarks = cs.FullMarks,
                PassMarks = cs.PassMarks,
                IsOptional = cs.IsOptional
            });
        }

        // Remove only subjects for THIS class/group, not other classes' subjects
        var existingSubjects = await _uow.Repository<ExamSubjectEntity>().Query()
            .Where(es => es.ExamId == examId && es.ClassId == classId && es.StudentGroupId == groupId).ToListAsync(ct);

        foreach (var old in existingSubjects)
            _uow.Repository<ExamSubjectEntity>().Remove(old);

        await _uow.Repository<ExamSubjectEntity>().AddRangeAsync(examSubjects, ct);
        await _uow.SaveChangesAsync(ct);

        return examSubjects.Count;
    }

    /// <summary>
    /// Generate per-student religion exam subjects for an exam based on student profiles.
    /// </summary>
    public async Task<int> GenerateReligionExamSubjectsAsync(int examId, int classId, CancellationToken ct = default)
    {
        var students = await _uow.Repository<Student>().Query()
            .AsNoTracking()
            .Where(s => s.ClassId == classId && !s.IsDeleted && s.Status == StudentStatus.Active)
            .ToListAsync(ct);

        var religionSubjectIds = students
            .Where(s => s.AssignedReligionSubjectId.HasValue)
            .Select(s => s.AssignedReligionSubjectId!.Value)
            .Distinct()
            .ToList();

        var classSubjects = await _uow.Repository<ClassSubject>().Query()
            .AsNoTracking()
            .Where(cs => cs.SchoolClassId == classId && cs.IsReligionSubject && !cs.IsDeleted && cs.IsActive)
            .ToListAsync(ct);

        var generated = 0;
        foreach (var religionSubjectId in religionSubjectIds)
        {
            var cs = classSubjects.FirstOrDefault(c => c.SubjectId == religionSubjectId);
            if (cs == null) continue;

            var existing = await _uow.Repository<ExamSubjectEntity>().Query()
                .AnyAsync(es => es.ExamId == examId && es.SubjectId == religionSubjectId, ct);

            if (!existing)
            {
                await _uow.Repository<ExamSubjectEntity>().AddAsync(new ExamSubjectEntity
                {
                    ExamId = examId,
                    SubjectId = religionSubjectId,
                    FullMarks = cs.FullMarks,
                    PassMarks = cs.PassMarks,
                    IsOptional = false
                }, ct);
                generated++;
            }
        }

        if (generated > 0)
            await _uow.SaveChangesAsync(ct);

        return generated;
    }

    /// <summary>
    /// Generate per-student optional exam subjects based on student selections.
    /// </summary>
    public async Task<int> GenerateOptionalExamSubjectsAsync(int examId, int classId, CancellationToken ct = default)
    {
        var students = await _uow.Repository<Student>().Query()
            .AsNoTracking()
            .Where(s => s.ClassId == classId && !s.IsDeleted && s.Status == StudentStatus.Active)
            .ToListAsync(ct);

        var optionalSubjectIds = students
            .Where(s => s.OptionalSubjectId.HasValue)
            .Select(s => s.OptionalSubjectId!.Value)
            .Distinct()
            .ToList();

        var classSubjects = await _uow.Repository<ClassSubject>().Query()
            .AsNoTracking()
            .Where(cs => cs.SchoolClassId == classId && cs.IsOptional && !cs.IsDeleted && cs.IsActive)
            .ToListAsync(ct);

        var generated = 0;
        foreach (var optionalSubjectId in optionalSubjectIds)
        {
            var cs = classSubjects.FirstOrDefault(c => c.SubjectId == optionalSubjectId);
            if (cs == null) continue;

            var existing = await _uow.Repository<ExamSubjectEntity>().Query()
                .AnyAsync(es => es.ExamId == examId && es.SubjectId == optionalSubjectId, ct);

            if (!existing)
            {
                await _uow.Repository<ExamSubjectEntity>().AddAsync(new ExamSubjectEntity
                {
                    ExamId = examId,
                    SubjectId = optionalSubjectId,
                    FullMarks = cs.FullMarks,
                    PassMarks = cs.PassMarks,
                    IsOptional = true
                }, ct);
                generated++;
            }
        }

        if (generated > 0)
            await _uow.SaveChangesAsync(ct);

        return generated;
    }
}

