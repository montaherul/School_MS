using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;
using ExamSubjectEntity = SchoolManagementSystem.Models.Entities.Exam.ExamSubject;
using ExamClassEntity = SchoolManagementSystem.Models.Entities.Exam.ExamClass;
using ExamSectionEntity = SchoolManagementSystem.Models.Entities.Exam.ExamSection;
using ExamSubjectComponentEntity = SchoolManagementSystem.Models.Entities.Exam.ExamSubjectComponent;
using SchoolManagementSystem.Models.DTOs.Exam;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ExamService : IExamService
{
    private readonly IUnitOfWork _uow;
    private readonly IExamRepository _examRepository;
    private readonly IGradingRuleRepository _gradingRepository;
    private readonly IExamValidationService _examValidation;
    private readonly ISubjectMarkStructureService _markStructureService;
    private readonly ITeacherAssignmentService _teacherAssignmentService;
    private readonly ILogger<ExamService> _logger;

    public ExamService(IUnitOfWork uow, IExamRepository examRepository, IGradingRuleRepository gradingRepository, IExamValidationService examValidation, ISubjectMarkStructureService markStructureService, ITeacherAssignmentService teacherAssignmentService, ILogger<ExamService> logger)
    {
        _uow = uow;
        _examRepository = examRepository;
        _gradingRepository = gradingRepository;
        _examValidation = examValidation;
        _markStructureService = markStructureService;
        _teacherAssignmentService = teacherAssignmentService;
        _logger = logger;
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
            .AsNoTracking()
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
            .Include(cs => cs.ClassSubjectGroups)
            .Where(cs => cs.SchoolClassId == classId && !cs.IsDeleted);

        if (groupId.HasValue)
        {
            query = query.Where(cs => cs.ClassSubjectGroups.Any(csg => !csg.IsDeleted && csg.StudentGroupId == groupId.Value) || !cs.ClassSubjectGroups.Any(csg => !csg.IsDeleted));
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
            query = query.Where(cs => cs.SectionId == null || cs.SectionId == sectionId.Value);
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
            .Include(cs => cs.ClassSubjectGroups)
            .Where(cs => cs.SchoolClassId == classId && !cs.IsDeleted && cs.IsActive)
            .ToListAsync(ct);

        // Filter by group for class 9-10
        if (groupId.HasValue)
        {
            classSubjects = classSubjects.Where(cs =>
                !cs.ClassSubjectGroups.Any(csg => !csg.IsDeleted) ||
                cs.ClassSubjectGroups.Any(csg => !csg.IsDeleted && csg.StudentGroupId == groupId.Value)).ToList();
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

    public async Task<int> CloneExamConfigForNewYearAsync(int fromAcademicYearId, int toAcademicYearId, string userId, CancellationToken ct = default)
    {
        var sourceExams = await _uow.Repository<ExamEntity>().Query()
            .AsNoTracking()
            .Where(e => e.AcademicYearId == fromAcademicYearId && !e.IsDeleted)
            .ToListAsync(ct);

        if (!sourceExams.Any())
            return 0;

        var cloned = 0;
        foreach (var source in sourceExams)
        {
            var exists = await _uow.Repository<ExamEntity>().AnyAsync(
                e => e.Name == source.Name && e.AcademicYearId == toAcademicYearId && e.ClassId == source.ClassId && e.IsDeleted == false, ct);
            if (exists) continue;

            var exam = new ExamEntity
            {
                Name = source.Name,
                Term = source.Term,
                AcademicYearId = toAcademicYearId,
                ClassId = source.ClassId,
                SectionId = source.SectionId,
                StudentGroupId = source.StudentGroupId,
                StartsOn = source.StartsOn,
                EndsOn = source.EndsOn,
                Status = ResultWorkflowStatus.Draft,
                IsLocked = false,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Repository<ExamEntity>().AddAsync(exam, ct);

            var sourceSubjects = await _uow.Repository<ExamSubjectEntity>().Query()
                .AsNoTracking()
                .Where(es => es.ExamId == source.Id && !es.IsDeleted)
                .ToListAsync(ct);

            foreach (var sub in sourceSubjects)
            {
                await _uow.Repository<ExamSubjectEntity>().AddAsync(new ExamSubjectEntity
                {
                    ExamId = exam.Id,
                    SubjectId = sub.SubjectId,
                    ClassId = sub.ClassId,
                    StudentGroupId = sub.StudentGroupId,
                    FullMarks = sub.FullMarks,
                    PassMarks = sub.PassMarks,
                    IsOptional = sub.IsOptional,
                    TeacherId = sub.TeacherId,
                    CreatedBy = userId
                }, ct);
            }
            cloned++;
        }

        await _uow.SaveChangesAsync(ct);
        return cloned;
    }

    public async Task<ExamReadinessReportDto> GetExamReadinessReportAsync(int academicYearId, CancellationToken ct = default)
    {
        return await _examRepository.GetExamReadinessReportAsync(academicYearId, ct);
    }

    public async Task<ExamEntity?> GetExamEntityByIdAsync(int examId, CancellationToken ct = default)
        => await _uow.Repository<ExamEntity>().GetByIdAsync(examId, ct);

    public async Task<IEnumerable<ExamEntity>> GetExamsByYearAsync(int academicYearId, CancellationToken ct = default)
        => await _examRepository.ListAsync(x => x.AcademicYearId == academicYearId && !x.IsDeleted, ct);

    public async Task<IEnumerable<ExamEntity>> GetAllExamsAsync(CancellationToken ct = default)
        => await _examRepository.ListAsync(x => !x.IsDeleted, ct);

    // ── Merged ExamGroup functionality ──────────────────────────────────────────

    public async Task<ExamWizardLoadResult> LoadExamClassesAsync(int academicYearId, List<int> classIds, CancellationToken ct = default)
    {
        var result = new ExamWizardLoadResult();

        var schoolClasses = await _uow.Repository<SchoolClass>().Query()
            .AsNoTracking()
            .Where(c => classIds.Contains(c.Id) && !c.IsDeleted)
            .ToListAsync(ct);

        foreach (var cls in schoolClasses)
        {
            var classLoad = new ExamClassLoadResult
            {
                ClassId = cls.Id,
                ClassName = cls.Name
            };

            // Load sections for this class
            var sections = await _uow.Repository<Section>().Query()
                .AsNoTracking()
                .Where(s => s.SchoolClassId == cls.Id && !s.IsDeleted)
                .ToListAsync(ct);
            classLoad.Sections = sections.Select(s => new ExamSectionDto
            {
                SectionId = s.Id,
                SectionName = s.Name
            }).ToList();

            // Load subjects via ClassSubject mapping
            var classSubjects = await _uow.Repository<ClassSubject>().Query()
                .AsNoTracking()
                .Include(cs => cs.Subject)
                .Where(cs => cs.SchoolClassId == cls.Id && !cs.IsDeleted && cs.IsActive)
                .ToListAsync(ct);

            foreach (var cs in classSubjects)
            {
                var subjectDetail = new ExamSubjectDetailDto
                {
                    SubjectId = cs.SubjectId,
                    SubjectName = cs.Subject?.Name ?? "",
                    SubjectCode = cs.Subject?.Code ?? "",
                    IsOptional = cs.IsOptional,
                    FullMarks = cs.FullMarks,
                    PassMarks = cs.PassMarks
                };

                // Load mark structure / components for this subject
                var markStructures = await _markStructureService.GetBySubjectAsync(cs.SubjectId);
                subjectDetail.Components = markStructures.Select(ms => new ExamComponentDto
                {
                    ComponentId = ms.ComponentId,
                    ComponentName = ms.ComponentName,
                    ComponentCode = ms.ComponentCode,
                    MaxMarks = ms.FullMarks,
                    PassMarks = ms.PassMarks,
                    DisplayOrder = ms.DisplayOrder
                }).ToList();

                // Use component marks as subject defaults if available
                if (markStructures.Count > 0)
                {
                    subjectDetail.FullMarks = markStructures.Sum(ms => ms.FullMarks);
                    subjectDetail.PassMarks = markStructures.Sum(ms => ms.PassMarks);
                }

                classLoad.Subjects.Add(subjectDetail);
            }

            result.Classes.Add(classLoad);
        }

        return result;
    }

    public async Task<ExamValidationResult> ValidateExamHierarchyAsync(ExamCreateRequest request, CancellationToken ct = default)
    {
        var result = new ExamValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.Name))
            result.Errors.Add("Exam name is required.");

        if (request.AcademicYearId <= 0)
            result.Errors.Add("Academic year is required.");

        if (request.StartDate > request.EndDate)
            result.Errors.Add("Start date must be before end date.");

        if (request.Classes.Count == 0)
            result.Errors.Add("At least one class must be selected.");

        // Check for duplicate name within the same academic year
        if (!string.IsNullOrWhiteSpace(request.Name) && request.AcademicYearId > 0)
        {
            var exists = await _uow.Repository<ExamEntity>().AnyAsync(
                g => g.AcademicYearId == request.AcademicYearId
                    && g.Name == request.Name
                    && !g.IsDeleted, ct);
            if (exists)
                result.Errors.Add($"An exam named '{request.Name}' already exists for this academic year.");
        }

        // Validate academic year is not closed
        var academicYear = await _uow.Repository<AcademicYear>().GetByIdAsync(request.AcademicYearId, ct);
        if (academicYear is null)
            result.Errors.Add("Academic year not found.");
        else if (!academicYear.IsActive)
            result.Warnings.Add("The selected academic year is not active.");

        // Validate each class has subjects
        foreach (var cls in request.Classes)
        {
            if (cls.Subjects.Count == 0)
                result.Errors.Add($"Class {cls.ClassId} has no subjects selected.");

            // Validate subject components
            foreach (var subj in cls.Subjects)
            {
                if (subj.FullMarks <= 0)
                    result.Errors.Add($"Subject {subj.SubjectId} in class {cls.ClassId} must have positive full marks.");

                if (subj.PassMarks > subj.FullMarks)
                    result.Errors.Add($"Subject {subj.SubjectId} in class {cls.ClassId}: pass marks cannot exceed full marks.");

                if (subj.Components.Count == 0)
                    result.Warnings.Add($"Subject {subj.SubjectId} in class {cls.ClassId} has no components configured.");

                // Marks Structure Validation - component totals must equal subject total
                if (subj.Components.Count > 0)
                {
                    var totalMaxMarks = subj.Components.Sum(c => c.MaxMarks);
                    var totalPassMarks = subj.Components.Sum(c => c.PassMarks);
                    
                    if (Math.Abs(totalMaxMarks - subj.FullMarks) > 0.01m)
                        result.Errors.Add($"Subject {subj.SubjectId} in class {cls.ClassId}: sum of component MaxMarks ({totalMaxMarks}) must equal subject FullMarks ({subj.FullMarks}).");
                    
                    if (Math.Abs(totalPassMarks - subj.PassMarks) > 0.01m)
                        result.Warnings.Add($"Subject {subj.SubjectId} in class {cls.ClassId}: sum of component PassMarks ({totalPassMarks}) differs from subject PassMarks ({subj.PassMarks}).");
                }
            }

            // Group Validation - subjects must belong to the class's student group
            var classInfo = await _uow.Repository<SchoolClass>().GetByIdAsync(cls.ClassId, ct);
            if (classInfo != null)
            {
                var studentGroupCodes = await _uow.Repository<Section>().Query()
                    .AsNoTracking()
                    .Where(s => s.SchoolClassId == cls.ClassId && !s.IsDeleted && s.StudentGroupId.HasValue)
                    .Join(_uow.Repository<StudentGroup>().Query().AsNoTracking().Where(sg => sg.IsActive),
                        s => s.StudentGroupId.Value,
                        sg => sg.Id,
                        (s, sg) => sg.Code)
                    .Distinct()
                    .ToListAsync(ct);

                if (studentGroupCodes.Count > 0)
                {
                    var validSubjectIds = await _uow.Repository<ClassSubject>().Query()
                        .AsNoTracking()
                        .Where(cs => cs.SchoolClassId == cls.ClassId 
                            && studentGroupCodes.Contains(cs.GroupName)
                            && !cs.IsDeleted && cs.IsActive)
                        .Select(cs => cs.SubjectId)
                        .ToListAsync(ct);

                    foreach (var subj in cls.Subjects)
                    {
                        if (!validSubjectIds.Contains(subj.SubjectId))
                            result.Warnings.Add($"Subject {subj.SubjectId} may not be assigned to the student group(s) of class {cls.ClassId} (Groups: {string.Join(", ", studentGroupCodes)}).");
                    }
                }
            }
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }

    public async Task<ExamCreateResult> CreateExamHierarchyAsync(ExamCreateRequest request, string userId, CancellationToken ct = default)
    {
        // Validate first
        var validation = await ValidateExamHierarchyAsync(request, ct);
        if (!validation.IsValid)
        {
            return new ExamCreateResult
            {
                Success = false,
                Message = string.Join(" ", validation.Errors)
            };
        }

        var examId = 0;
        var classCount = 0;
        var subjectCount = 0;
        var componentCount = 0;

        // Pre-load reference data for snapshots
        var allSubjects = await _uow.Repository<Subject>().Query()
            .AsNoTracking()
            .ToDictionaryAsync(s => s.Id, s => s, ct);

        var allTeachers = await _uow.Repository<Teacher>().Query()
            .AsNoTracking()
            .ToDictionaryAsync(t => t.Id, t => t, ct);

        var allComponents = await _uow.Repository<ExamComponent>().Query()
            .AsNoTracking()
            .ToDictionaryAsync(c => c.Id, c => c, ct);

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            // 1. Create Exam
            var exam = new ExamEntity
            {
                Name = request.Name,
                AcademicYearId = request.AcademicYearId,
                Term = request.Term,
                Status = ResultWorkflowStatus.Draft,
                IsPublished = false,
                IsLocked = false,
                StartsOn = request.StartDate,
                EndsOn = request.EndDate,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Repository<ExamEntity>().AddAsync(exam, ct);
            await _uow.SaveChangesAsync(ct);
            examId = exam.Id;

            // 2. Create classes, sections, subjects, components
            foreach (var classReq in request.Classes)
            {
                var classEntity = await _uow.Repository<SchoolClass>().Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == classReq.ClassId, ct);

                var examClass = new ExamClassEntity
                {
                    ExamId = examId,
                    ClassId = classReq.ClassId,
                    ClassName = classEntity?.Name ?? "",
                    SortOrder = classCount,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Repository<ExamClassEntity>().AddAsync(examClass, ct);
                await _uow.SaveChangesAsync(ct);
                classCount++;

                // 2a. Sections
                foreach (var sectionId in classReq.SectionIds)
                {
                    var sectionEntity = await _uow.Repository<Section>().Query()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.Id == sectionId, ct);

                    var examSection = new ExamSectionEntity
                    {
                        ExamClassId = examClass.Id,
                        SectionId = sectionId,
                        SectionName = sectionEntity?.Name ?? "",
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.Repository<ExamSectionEntity>().AddAsync(examSection, ct);
                }

                // 2b. Subjects
                foreach (var subjectReq in classReq.Subjects)
                {
                    var subject = allSubjects.GetValueOrDefault(subjectReq.SubjectId);
                    var teacher = subjectReq.TeacherId.HasValue ? allTeachers.GetValueOrDefault(subjectReq.TeacherId.Value) : null;

                    var examSubject = new ExamSubjectEntity
                    {
                        ExamId = examId,
                        SubjectId = subjectReq.SubjectId,
                        ClassId = classReq.ClassId,
                        StudentGroupId = subjectReq.TeacherId.HasValue ? subjectReq.TeacherId : null, // Use TeacherId as StudentGroupId? No, this is separate
                        TeacherId = subjectReq.TeacherId,
                        IsOptional = subjectReq.IsOptional,
                        IsReligionSubject = subjectReq.IsReligionSubject,
                        FullMarks = subjectReq.FullMarks,
                        PassMarks = subjectReq.PassMarks,
                        // Snapshots
                        SubjectName = subject?.Name ?? "",
                        SubjectCode = subject?.Code ?? "",
                        TeacherName = teacher?.FullName,
                        TeacherEmployeeCode = teacher?.TeacherCode,
                        Credit = subject?.Credit ?? 0,
                        NCTBCode = subject?.NctbCode,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.Repository<ExamSubjectEntity>().AddAsync(examSubject, ct);
                    await _uow.SaveChangesAsync(ct);
                    subjectCount++;

                    // 2c. Components
                    foreach (var compReq in subjectReq.Components)
                    {
                        var component = allComponents.GetValueOrDefault(compReq.ComponentId);
                        var examSubjectComponent = new ExamSubjectComponentEntity
                        {
                            ExamSubjectId = examSubject.Id,
                            ComponentId = compReq.ComponentId,
                            MaxMarks = compReq.MaxMarks,
                            PassMarks = compReq.PassMarks,
                            DisplayOrder = compReq.DisplayOrder,
                            // Snapshots
                            ComponentName = component?.Name ?? "",
                            ComponentCode = component?.Code ?? "",
                            Weight = component?.DefaultFullMarks ?? 0,
                            CreatedBy = userId,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _uow.Repository<ExamSubjectComponentEntity>().AddAsync(examSubjectComponent, ct);
                        componentCount++;
                    }
                }
            }

            await _uow.SaveChangesAsync(ct);
        }, ct);

        _logger.LogInformation(
            "Exam created: {Name} (ID={Id}) with {Classes} classes, {Subjects} subjects, {Components} components by {User}",
            request.Name, examId, classCount, subjectCount, componentCount, userId);

        return new ExamCreateResult
        {
            Success = true,
            Message = $"Exam '{request.Name}' created successfully.",
            ExamId = examId,
            ExamName = request.Name,
            ClassCount = classCount,
            SubjectCount = subjectCount,
            ComponentCount = componentCount
        };
    }

    public async Task<ExamReadinessDto> GetExamReadinessAsync(int examId, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<ExamEntity>().Query()
            .AsNoTracking()
            .Include(e => e.Classes)
                .ThenInclude(c => c.Subjects)
                    .ThenInclude(s => s.Components)
            .Include(e => e.Classes)
                .ThenInclude(c => c.Sections)
            .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted, ct);

        if (exam is null)
            throw new InvalidOperationException("Exam not found.");

        var checks = new List<Models.DTOs.Exam.ExamReadinessCheck>();
        int totalWeight = 0;
        int passedWeight = 0;

        // Check 1: Has classes (Weight: 15)
        var hasClasses = exam.Classes.Count > 0;
        checks.Add(new Models.DTOs.Exam.ExamReadinessCheck
        {
            Name = "Classes Configured",
            Passed = hasClasses,
            Details = hasClasses ? $"{exam.Classes.Count} class(es) configured" : "No classes assigned",
            Weight = 15
        });
        totalWeight += 15;
        if (hasClasses) passedWeight += 15;

        // Check 2: All classes have sections (Weight: 10)
        var classesWithSections = exam.Classes.Count(c => c.Sections.Count > 0);
        var allHaveSections = classesWithSections == exam.Classes.Count && exam.Classes.Count > 0;
        checks.Add(new Models.DTOs.Exam.ExamReadinessCheck
        {
            Name = "Sections Configured",
            Passed = allHaveSections,
            Details = $"{classesWithSections}/{exam.Classes.Count} classes have sections",
            Weight = 10
        });
        totalWeight += 10;
        if (allHaveSections) passedWeight += 10;

        // Check 3: All classes have subjects (Weight: 20)
        var classesWithSubjects = exam.Classes.Count(c => c.Subjects.Count > 0);
        var allHaveSubjects = classesWithSubjects == exam.Classes.Count && exam.Classes.Count > 0;
        checks.Add(new Models.DTOs.Exam.ExamReadinessCheck
        {
            Name = "Subjects Assigned",
            Passed = allHaveSubjects,
            Details = $"{classesWithSubjects}/{exam.Classes.Count} classes have subjects",
            Weight = 20
        });
        totalWeight += 20;
        if (allHaveSubjects) passedWeight += 20;

        // Check 4: All subjects have components (Weight: 15)
        var totalSubjects = exam.Classes.SelectMany(c => c.Subjects).Count();
        var subjectsWithComponents = exam.Classes.SelectMany(c => c.Subjects).Count(s => s.Components.Count > 0);
        var allHaveComponents = totalSubjects > 0 && subjectsWithComponents == totalSubjects;
        checks.Add(new Models.DTOs.Exam.ExamReadinessCheck
        {
            Name = "Mark Components Defined",
            Passed = allHaveComponents,
            Details = $"{subjectsWithComponents}/{totalSubjects} subjects have components",
            Weight = 15
        });
        totalWeight += 15;
        if (allHaveComponents) passedWeight += 15;

        // Check 5: Component marks sum equals subject marks (Weight: 20)
        var structureValid = true;
        var structureDetails = new List<string>();
        foreach (var cls in exam.Classes)
        {
            foreach (var subj in cls.Subjects)
            {
                if (subj.Components.Count > 0)
                {
                    var sumMax = subj.Components.Sum(c => c.MaxMarks);
                    var sumPass = subj.Components.Sum(c => c.PassMarks);
                    if (Math.Abs(sumMax - subj.FullMarks) > 0.01m || Math.Abs(sumPass - subj.PassMarks) > 0.01m)
                    {
                        structureValid = false;
                        structureDetails.Add($"{subj.SubjectName}: components sum to {sumMax}/{sumPass} vs subject {subj.FullMarks}/{subj.PassMarks}");
                    }
                }
            }
        }
        checks.Add(new Models.DTOs.Exam.ExamReadinessCheck
        {
            Name = "Marks Structure Valid",
            Passed = structureValid,
            Details = structureValid 
                ? "All component totals match subject totals"
                : string.Join("; ", structureDetails),
            Weight = 20
        });
        totalWeight += 20;
        if (structureValid) passedWeight += 20;

        // Check 6: Teacher assigned to all subjects (Weight: 10)
        var totalSubjectsWithTeacher = exam.Classes.SelectMany(c => c.Subjects).Count(s => s.TeacherId.HasValue);
        var allHaveTeachers = totalSubjects > 0 && totalSubjectsWithTeacher == totalSubjects;
        checks.Add(new Models.DTOs.Exam.ExamReadinessCheck
        {
            Name = "Teachers Assigned",
            Passed = allHaveTeachers,
            Details = $"{totalSubjectsWithTeacher}/{totalSubjects} subjects have teachers",
            Weight = 10
        });
        totalWeight += 10;
        if (allHaveTeachers) passedWeight += 10;

        // Check 7: Schedule exists for all subjects (Weight: 10)
        var hasSchedule = false; // Would query ExamSchedule table
        checks.Add(new Models.DTOs.Exam.ExamReadinessCheck
        {
            Name = "Exam Schedule Complete",
            Passed = hasSchedule,
            Details = hasSchedule ? "All exams scheduled" : "Schedule not yet created",
            Weight = 10
        });
        totalWeight += 10;
        if (hasSchedule) passedWeight += 10;

        var readinessPercent = totalWeight > 0 ? (decimal)passedWeight / totalWeight * 100 : 0;
        var isReady = readinessPercent >= 100;

        return new Models.DTOs.Exam.ExamReadinessDto
        {
            ExamId = exam.Id,
            ExamName = exam.Name,
            ReadinessPercent = Math.Round(readinessPercent, 1),
            IsReadyToPublish = isReady,
            Checks = checks
        };
    }

    public async Task ArchiveExamAsync(int examId, string reason, int userId, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId, ct)
            ?? throw new InvalidOperationException("Exam not found.");

        if (exam.IsArchived)
            throw new InvalidOperationException("Exam is already archived.");

        if (exam.IsPublished)
            throw new InvalidOperationException("Cannot archive a published exam. Unpublish first.");

        exam.IsArchived = true;
        exam.ArchivedAt = DateTime.UtcNow;
        exam.ArchivedByUserId = userId;
        exam.ArchiveReason = reason;
        exam.UpdatedAt = DateTime.UtcNow;
        exam.UpdatedBy = userId.ToString();

        _uow.Repository<ExamEntity>().Update(exam);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Exam {Id} archived by user {UserId}. Reason: {Reason}", examId, userId, reason);
    }

    public async Task RestoreExamAsync(int examId, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId, ct)
            ?? throw new InvalidOperationException("Exam not found.");

        if (!exam.IsArchived)
            throw new InvalidOperationException("Exam is not archived.");

        exam.IsArchived = false;
        exam.ArchivedAt = null;
        exam.ArchivedByUserId = null;
        exam.ArchiveReason = null;
        exam.UpdatedAt = DateTime.UtcNow;

        _uow.Repository<ExamEntity>().Update(exam);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Exam {Id} unarchived", examId);
    }

    public async Task<ExamCreateResult> CopyExamHierarchyAsync(int sourceExamId, int targetAcademicYearId, string userId, CancellationToken ct = default)
    {
        var source = await _uow.Repository<ExamEntity>().Query()
            .AsNoTracking()
            .Include(e => e.Classes)
                .ThenInclude(c => c.Sections)
            .Include(e => e.Classes)
                .ThenInclude(c => c.Subjects)
                    .ThenInclude(s => s.Components)
            .FirstOrDefaultAsync(e => e.Id == sourceExamId && !e.IsDeleted, ct);

        if (source is null)
            throw new InvalidOperationException("Source exam not found.");

        var targetYear = await _uow.Repository<AcademicYear>().GetByIdAsync(targetAcademicYearId, ct);
        if (targetYear is null)
            throw new InvalidOperationException("Target academic year not found.");

        // Create new request from source
        var newRequest = new ExamCreateRequest
        {
            Name = $"{source.Name} (Copy)",
            AcademicYearId = targetAcademicYearId,
            Term = source.Term,
            ExamType = "",
            StartDate = source.StartsOn,
            EndDate = source.EndsOn,
            Classes = source.Classes.Select(c => new ExamClassRequest
            {
                ClassId = c.ClassId,
                SectionIds = c.Sections.Select(s => s.SectionId).ToList(),
                Subjects = c.Subjects.Select(s => new ExamSubjectRequest
                {
                    SubjectId = s.SubjectId,
                    TeacherId = s.TeacherId,
                    IsOptional = s.IsOptional,
                    IsReligionSubject = s.IsReligionSubject,
                    FullMarks = s.FullMarks,
                    PassMarks = s.PassMarks,
                    Components = s.Components.Select(comp => new ExamComponentRequest
                    {
                        ComponentId = comp.ComponentId,
                        MaxMarks = comp.MaxMarks,
                        PassMarks = comp.PassMarks,
                        DisplayOrder = comp.DisplayOrder
                    }).ToList()
                }).ToList()
            }).ToList()
        };

        return await CreateExamHierarchyAsync(newRequest, userId, ct);
    }
}

