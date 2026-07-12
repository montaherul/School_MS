using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using ClassSubject = SchoolManagementSystem.Models.Entities.Academic.ClassSubject;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;
using ExamSubjectEntity = SchoolManagementSystem.Models.Entities.Exam.ExamSubject;
using ExamTemplateEntity = SchoolManagementSystem.Models.Entities.Exam.ExamTemplate;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ExamWizardService : IExamWizardService
{
    private readonly IUnitOfWork _uow;
    private readonly IExamService _examService;
    private readonly IExamValidationService _examValidation;
    private readonly ISubjectMarkStructureService _markStructureService;
    private readonly IExamComponentService _componentService;
    private readonly ITeacherAssignmentService _teacherAssignmentService;
    private readonly IExamWizardRepository _wizardRepository;

    public ExamWizardService(
        IUnitOfWork uow,
        IExamService examService,
        IExamValidationService examValidation,
        ISubjectMarkStructureService markStructureService,
        IExamComponentService componentService,
        ITeacherAssignmentService teacherAssignmentService,
        IExamWizardRepository wizardRepository)
    {
        _uow = uow;
        _examService = examService;
        _examValidation = examValidation;
        _markStructureService = markStructureService;
        _componentService = componentService;
        _teacherAssignmentService = teacherAssignmentService;
        _wizardRepository = wizardRepository;
    }

    // ──────────────────────────────────────────────
    // SP-Based Methods (New Architecture)
    // ──────────────────────────────────────────────

    public async Task<ExamCreationPreviewDto> GetExamCreationPreviewAsync(int academicYearId, List<int> classIds, CancellationToken ct = default)
    {
        return await _wizardRepository.GetExamCreationPreviewAsync(academicYearId, classIds, ct);
    }

    public async Task<ExamClassHierarchyDto> GetExamClassHierarchyAsync(int academicYearId, List<int> classIds, CancellationToken ct = default)
    {
        return await _wizardRepository.GetExamClassHierarchyAsync(academicYearId, classIds, ct);
    }

    public async Task<List<ExamTeacherAssignmentDto>> GetExamTeacherAssignmentsAsync(int academicYearId, List<int> classIds, CancellationToken ct = default)
    {
        return await _wizardRepository.GetExamTeacherAssignmentsAsync(academicYearId, classIds, ct);
    }

    public async Task<ExamValidationResultDto> ValidateExamCreationAsync(int academicYearId, string examName, ExamTerm term, List<int> classIds, DateOnly startDate, DateOnly endDate, CancellationToken ct = default)
    {
        return await _wizardRepository.GetExamValidationAsync(academicYearId, examName, (int)term, classIds, startDate, endDate, ct);
    }

    public async Task<ExamCreateResultDto> CreateExamHierarchyAsync(ExamCreateHierarchyRequest request, string userId, CancellationToken ct = default)
    {
        request.UserId = userId;
        return await _wizardRepository.CreateExamHierarchyAsync(request, ct);
    }

    public async Task<ExamCreationReadinessDto> GetExamReadinessAsync(int academicYearId, List<int> classIds, CancellationToken ct = default)
    {
        return await _wizardRepository.GetExamReadinessAsync(academicYearId, classIds, ct);
    }

    public async Task<ExamStatisticsDto> GetExamStatisticsAsync(int academicYearId, List<int> classIds, CancellationToken ct = default)
    {
        return await _wizardRepository.GetExamStatisticsAsync(academicYearId, classIds, ct);
    }

    public async Task<ExamScheduleResultDto> GenerateExamScheduleAsync(int examId, DateOnly startDate, DateOnly endDate, string userId, CancellationToken ct = default)
    {
        return await _wizardRepository.GenerateExamScheduleAsync(examId, startDate, endDate, userId, ct);
    }

    public async Task<List<ExamConflictDto>> GetExamConflictsAsync(int examId, CancellationToken ct = default)
    {
        return await _wizardRepository.GetExamConflictsAsync(examId, ct);
    }

    // ──────────────────────────────────────────────
    // Fix Issues Methods
    // ──────────────────────────────────────────────

    public async Task<ExamFixResultDto> AssignTeacherToExamSubjectAsync(int academicYearId, int subjectId, int classId, int? sectionId, int? studentGroupId, int teacherId, string userId, CancellationToken ct = default)
    {
        return await _wizardRepository.AssignTeacherToExamSubjectAsync(academicYearId, subjectId, classId, sectionId, studentGroupId, teacherId, userId, ct);
    }

    public async Task<ExamFixResultDto> ConfigureExamSubjectComponentsAsync(int examSubjectId, string componentsJson, string userId, CancellationToken ct = default)
    {
        return await _wizardRepository.ConfigureExamSubjectComponentsAsync(examSubjectId, componentsJson, userId, ct);
    }

    public async Task<ExamFixResultDto> AddSectionsToClassAsync(int classId, string sectionNamesJson, int? studentGroupId, string userId, CancellationToken ct = default)
    {
        return await _wizardRepository.AddSectionsToClassAsync(classId, sectionNamesJson, studentGroupId, userId, ct);
    }

    public async Task<ExamFixResultDto> MapSubjectToClassAsync(int subjectId, int classId, int? studentGroupId, decimal fullMarks, decimal passMarks, bool isOptional, int displayOrder, string userId, CancellationToken ct = default)
    {
        return await _wizardRepository.MapSubjectToClassAsync(subjectId, classId, studentGroupId, fullMarks, passMarks, isOptional, displayOrder, userId, ct);
    }

    public async Task<ExamFixResultDto> ConfigureSubjectMarkStructureAsync(int subjectId, int? classId, int? studentGroupId, string componentsJson, string userId, CancellationToken ct = default)
    {
        return await _wizardRepository.ConfigureSubjectMarkStructureAsync(subjectId, classId, studentGroupId, componentsJson, userId, ct);
    }

    public async Task<ExamPublishReadinessDto> CheckExamPublishReadinessAsync(int examId, CancellationToken ct = default)
    {
        return await _wizardRepository.CheckExamPublishReadinessAsync(examId, ct);
    }

    // ──────────────────────────────────────────────
    // Legacy Methods (Backward Compatibility)
    // ──────────────────────────────────────────────

    public async Task<List<ExamWizardSubjectDto>> LoadSubjectsAsync(int academicYearId, List<int> classIds, ExamTerm term, CancellationToken ct = default)
    {
        var result = new List<ExamWizardSubjectDto>();
        var classSubjects = await _uow.Repository<ClassSubject>().Query()
            .AsNoTracking()
            .Include(cs => cs.Subject)
            .Where(cs => classIds.Contains(cs.SchoolClassId) && !cs.IsDeleted && cs.IsActive)
            .ToListAsync(ct);

        var subjectIds = classSubjects.Select(cs => cs.SubjectId).Distinct().ToList();
        var teacherAssignments = await GetTeacherAssignmentsAsync(academicYearId, classIds, subjectIds, ct);

        var subjectMarkStructures = await _uow.Repository<SubjectMarkStructure>().Query()
            .AsNoTracking()
            .Include(sms => sms.Component)
            .Where(sms => subjectIds.Contains(sms.SubjectId ?? 0) && sms.IsActive)
            .OrderBy(sms => sms.DisplayOrder)
            .ToListAsync(ct);

        var componentsBySubject = subjectMarkStructures
            .GroupBy(sms => sms.SubjectId ?? 0)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var cs in classSubjects)
        {
            var subject = cs.Subject;
            if (subject == null) continue;

            var components = componentsBySubject.GetValueOrDefault(subject.Id, []);
            teacherAssignments.TryGetValue(subject.Id, out var teacherId);

            result.Add(new ExamWizardSubjectDto
            {
                SubjectId = subject.Id,
                SubjectName = subject.Name,
                SubjectNameBn = subject.NameBn ?? "",
                SubjectCode = subject.Code ?? "",
                FullMarks = cs.FullMarks,
                PassMarks = cs.PassMarks,
                IsOptional = cs.IsOptional,
                TeacherId = teacherId,
                ClassId = cs.SchoolClassId,
                ClassName = cs.SchoolClass?.Name ?? "",
                Components = components.Select(c => new ExamWizardComponentDto
                {
                    ComponentId = c.ComponentId,
                    ComponentName = c.Component?.Name ?? "",
                    FullMarks = c.FullMarks,
                    PassMarks = c.PassMarks,
                    DisplayOrder = c.DisplayOrder
                }).ToList()
            });
        }

        return result;
    }

    public async Task<ExamWizardStateDto?> LoadPreviousExamTemplateAsync(int academicYearId, ExamTerm term, CancellationToken ct = default)
    {
        var repo = _uow.Repository<ExamEntity>();
        var previousExam = await repo.Query()
            .AsNoTracking()
            .Where(e => e.Term == term && e.AcademicYearId < academicYearId && !e.IsDeleted)
            .OrderByDescending(e => e.AcademicYearId)
            .ThenByDescending(e => e.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (previousExam == null) return null;

        return await LoadExamByIdAsync(previousExam.Id, ct);
    }

    public async Task<ExamWizardStateDto?> LoadExamByIdAsync(int examId, CancellationToken ct = default)
    {
        var repo = _uow.Repository<ExamEntity>();
        var exam = await repo.Query()
            .AsNoTracking()
            .Include(e => e.ExamSubjects).ThenInclude(es => es.Subject)
            .Include(e => e.ExamSubjects).ThenInclude(es => es.Teacher)
            .Include(e => e.ExamSubjects).ThenInclude(es => es.Class)
            .Include(e => e.ExamSchedules)
            .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted, ct);

        if (exam == null) return null;

        var classIds = exam.ExamSubjects
            .Select(es => es.ClassId)
            .Distinct()
            .ToList();

        var classNames = await _uow.Repository<SchoolClass>().Query()
            .AsNoTracking()
            .Where(c => classIds.Contains(c.Id))
            .Select(c => c.Name)
            .ToListAsync(ct);

        var subjects = new List<ExamWizardSubjectDto>();
        foreach (var es in exam.ExamSubjects.Where(es => !es.IsDeleted))
        {
            var components = await GetComponentsForSubjectAsync(es.SubjectId, es.ClassId, ct);

            subjects.Add(new ExamWizardSubjectDto
            {
                SubjectId = es.SubjectId,
                SubjectName = es.Subject?.Name ?? "",
                SubjectNameBn = es.Subject?.NameBn ?? "",
                SubjectCode = es.Subject?.Code ?? "",
                FullMarks = es.FullMarks,
                PassMarks = es.PassMarks,
                IsOptional = es.IsOptional,
                TeacherId = es.TeacherId,
                TeacherName = es.Teacher?.FullName ?? "",
                ClassId = es.ClassId,
                ClassName = es.Class?.Name ?? "",
                Components = components
            });
        }

        return new ExamWizardStateDto
        {
            Step = 2,
            AcademicYearId = exam.AcademicYearId,
            AcademicYearName = "",
            Term = exam.Term,
            TermName = exam.Term.ToString(),
            ExamType = "",
            SelectedClassIds = classIds,
            SelectedClassNames = classNames,
            Subjects = subjects,
            SourceExamId = exam.Id,
            SourceExamName = exam.Name,
            StartDate = exam.StartsOn.ToDateTime(TimeOnly.MinValue),
            EndDate = exam.EndsOn.ToDateTime(TimeOnly.MinValue)
        };
    }

    public async Task<ExamWizardResultDto> CreateExamsFromWizardAsync(ExamWizardCreateRequest request, string userId, CancellationToken ct = default)
    {
        var result = new ExamWizardResultDto();
        var classIds = request.SelectedClassIds;
        var termName = request.Term.ToString();
        var examType = string.IsNullOrWhiteSpace(request.ExamType) ? termName : request.ExamType;
        var startDate = request.StartDate.HasValue ? DateOnly.FromDateTime(request.StartDate.Value) : DateOnly.MinValue;
        var endDate = request.EndDate.HasValue ? DateOnly.FromDateTime(request.EndDate.Value) : DateOnly.MinValue;

        var subjectsByClass = request.Subjects.GroupBy(s => s.ClassId).ToDictionary(g => g.Key, g => g.ToList());
        var createdExamIds = new List<int>();
        var createdExamNames = new List<string>();
        var totalSubjectCount = 0;
        var totalTeacherAssignments = 0;

        foreach (var classId in classIds)
        {
            var yearName = await _uow.Repository<AcademicYear>().Query()
                .AsNoTracking()
                .Where(y => y.Id == request.AcademicYearId)
                .Select(y => y.Name)
                .FirstOrDefaultAsync(ct);

            var className = await _uow.Repository<SchoolClass>().Query()
                .AsNoTracking()
                .Where(c => c.Id == classId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync(ct);

            var examName = $"{examType} {className} ({yearName})";

            var dto = new ExamUpsertDto
            {
                Name = examName,
                Term = request.Term,
                AcademicYearId = request.AcademicYearId,
                ClassId = classId,
                StartsOn = startDate,
                EndsOn = endDate,
                Status = ResultWorkflowStatus.Draft,
                Subjects = subjectsByClass.GetValueOrDefault(classId, []).Select(s => new SubjectMarkConfigDto
                {
                    SubjectId = s.SubjectId,
                    FullMarks = s.FullMarks,
                    PassMarks = s.PassMarks,
                    IsOptional = s.IsOptional
                }).ToList()
            };

            var examResult = await _examService.CreateExamAsync(dto, ct);
            if (examResult != null)
            {
                var examId = (int)examResult.GetType().GetProperty("Id")?.GetValue(examResult, null)!;
                createdExamIds.Add(examId);
                createdExamNames.Add(examName);
                totalSubjectCount += dto.Subjects.Count;

                var classSubjects = subjectsByClass.GetValueOrDefault(classId, []);
                foreach (var subj in classSubjects.Where(s => s.TeacherId.HasValue))
                {
                    var examSubjectRepo = _uow.Repository<ExamSubjectEntity>();
                    var examSubject = await examSubjectRepo.FirstOrDefaultAsync(
                        es => es.ExamId == examId && es.SubjectId == subj.SubjectId && !es.IsDeleted, ct);
                    if (examSubject != null)
                    {
                        examSubject.TeacherId = subj.TeacherId;
                        examSubjectRepo.Update(examSubject);
                        totalTeacherAssignments++;
                    }
                }
            }
        }

        await _uow.SaveChangesAsync(ct);

        result.Success = true;
        result.Message = $"Created {createdExamIds.Count} exam(s) with {totalSubjectCount} subject(s) and {totalTeacherAssignments} teacher assignment(s).";
        result.CreatedExamIds = createdExamIds;
        result.CreatedExamNames = createdExamNames;
        result.SubjectCount = totalSubjectCount;
        result.TeacherAssignmentCount = totalTeacherAssignments;
        return result;
    }

    public async Task<List<ExamWizardComponentDto>> GetComponentsForSubjectAsync(int subjectId, int? classId, CancellationToken ct = default)
    {
        var query = _uow.Repository<SubjectMarkStructure>().Query()
            .AsNoTracking()
            .Include(sms => sms.Component)
            .Where(sms => sms.SubjectId == subjectId && sms.IsActive);

        if (classId.HasValue)
            query = query.Where(sms => sms.ClassId == null || sms.ClassId == classId.Value);

        var structures = await query
            .OrderBy(sms => sms.DisplayOrder)
            .ToListAsync(ct);

        return structures.Select(s => new ExamWizardComponentDto
        {
            ComponentId = s.ComponentId,
            ComponentName = s.Component?.Name ?? "",
            FullMarks = s.FullMarks,
            PassMarks = s.PassMarks,
            DisplayOrder = s.DisplayOrder
        }).ToList();
    }

    public async Task<Dictionary<int, int?>> GetTeacherAssignmentsAsync(int academicYearId, List<int> classIds, List<int> subjectIds, CancellationToken ct = default)
    {
        var assignments = await _uow.Repository<TeacherSubjectAssignment>().Query()
            .AsNoTracking()
            .Where(tsa => tsa.AcademicYearId == academicYearId
                && classIds.Contains(tsa.ClassId)
                && subjectIds.Contains(tsa.SubjectId)
                && tsa.IsActive)
            .Select(tsa => new { tsa.SubjectId, tsa.TeacherId })
            .ToListAsync(ct);

        return assignments
            .GroupBy(a => a.SubjectId)
            .ToDictionary(g => g.Key, g => (int?)g.First().TeacherId);
    }

    // ──────────────────────────────────────────────
    // NCTB Template Loading
    // ──────────────────────────────────────────────

    public async Task<ExamWizardStateDto?> LoadNctbTemplateAsync(int academicYearId, int classId, ExamTerm term, CancellationToken ct = default)
    {
        var year = await _uow.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .Where(y => y.Id == academicYearId)
            .FirstOrDefaultAsync(ct);

        if (year == null) return null;

        var schoolClass = await _uow.Repository<SchoolClass>().Query()
            .AsNoTracking()
            .Where(c => c.Id == classId)
            .FirstOrDefaultAsync(ct);

        if (schoolClass == null) return null;

        var templates = GetNctbTemplates(classId);
        if (templates.Count == 0) return null;

        // For group-based classes (9-10), return all groups; otherwise return general template
        var subjects = new List<ExamWizardSubjectDto>();
        foreach (var template in templates)
        {
            foreach (var nctbSubj in template.Subjects)
            {
                var components = nctbSubj.Components.Select(c => new ExamWizardComponentDto
                {
                    ComponentId = 0,
                    ComponentName = c.ComponentName,
                    FullMarks = c.FullMarks,
                    PassMarks = c.PassMarks,
                    DisplayOrder = c.DisplayOrder
                }).ToList();

                subjects.Add(new ExamWizardSubjectDto
                {
                    SubjectId = 0,
                    SubjectName = nctbSubj.SubjectName,
                    SubjectNameBn = nctbSubj.SubjectNameBn,
                    SubjectCode = nctbSubj.SubjectCode,
                    FullMarks = nctbSubj.FullMarks,
                    PassMarks = nctbSubj.PassMarks,
                    IsOptional = nctbSubj.IsOptional,
                    TeacherId = null,
                    ClassId = classId,
                    ClassName = schoolClass.Name,
                    Components = components
                });
            }
        }

        return new ExamWizardStateDto
        {
            Step = 2,
            AcademicYearId = academicYearId,
            AcademicYearName = year.Name,
            Term = term,
            TermName = term.ToString(),
            ExamType = term.ToString(),
            SelectedClassIds = [classId],
            SelectedClassNames = [schoolClass.Name],
            Subjects = subjects,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(14)
        };
    }

    private static List<NctbTemplateDto> GetNctbTemplates(int classId)
    {
        // NCTB standard configurations based on Bangladesh national curriculum
        // Science group (classes 9-10)
        var science = new NctbTemplateDto
        {
            GroupName = "Science",
            GroupCode = "SCIENCE",
            Subjects =
            [
                new() { SubjectName = "Bangla", SubjectNameBn = "বাংলা", SubjectCode = "101", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "English", SubjectNameBn = "ইংরেজি", SubjectCode = "107", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Mathematics", SubjectNameBn = "গণিত", SubjectCode = "109", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Physics", SubjectNameBn = "পদার্থবিজ্ঞান", SubjectCode = "136", FullMarks = 100, PassMarks = 33, HasPractical = true, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }, new() { ComponentName = "Practical", ComponentCode = "PRACTICAL", FullMarks = 25, PassMarks = 10, DisplayOrder = 3, IsPractical = true }] },
                new() { SubjectName = "Chemistry", SubjectNameBn = "রসায়ন", SubjectCode = "137", FullMarks = 100, PassMarks = 33, HasPractical = true, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }, new() { ComponentName = "Practical", ComponentCode = "PRACTICAL", FullMarks = 25, PassMarks = 10, DisplayOrder = 3, IsPractical = true }] },
                new() { SubjectName = "Biology", SubjectNameBn = "জীববিজ্ঞান", SubjectCode = "138", FullMarks = 100, PassMarks = 33, HasPractical = true, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }, new() { ComponentName = "Practical", ComponentCode = "PRACTICAL", FullMarks = 25, PassMarks = 10, DisplayOrder = 3, IsPractical = true }] },
                new() { SubjectName = "Higher Math", SubjectNameBn = "উচ্চতর গণিত", SubjectCode = "140", FullMarks = 100, PassMarks = 33, IsOptional = true, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Information & Communication Technology", SubjectNameBn = "তথ্য ও যোগাযোগ প্রযুক্তি", SubjectCode = "139", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 50, PassMarks = 17, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 25, PassMarks = 8, DisplayOrder = 2 }, new() { ComponentName = "Practical", ComponentCode = "PRACTICAL", FullMarks = 25, PassMarks = 8, DisplayOrder = 3, IsPractical = true }] },
                new() { SubjectName = "Islam & Moral Education", SubjectNameBn = "ইসলাম ও নৈতিক শিক্ষা", SubjectCode = "111", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Bangladesh & Global Studies", SubjectNameBn = "বাংলাদেশ ও বিশ্বপরিচয়", SubjectCode = "150", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Physical Education & Health", SubjectNameBn = "শারীরিক শিক্ষা ও স্বাস্থ্য", SubjectCode = "190", FullMarks = 50, PassMarks = 17, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 25, PassMarks = 8, DisplayOrder = 1 }, new() { ComponentName = "Practical", ComponentCode = "PRACTICAL", FullMarks = 25, PassMarks = 9, DisplayOrder = 2, IsPractical = true }] }
            ]
        };

        // Business Studies group (classes 9-10)
        var businessStudies = new NctbTemplateDto
        {
            GroupName = "Business Studies",
            GroupCode = "BUSINESS",
            Subjects =
            [
                new() { SubjectName = "Bangla", SubjectNameBn = "বাংলা", SubjectCode = "101", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "English", SubjectNameBn = "ইংরেজি", SubjectCode = "107", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Mathematics", SubjectNameBn = "গণিত", SubjectCode = "109", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Accounting", SubjectNameBn = "হিসাববিজ্ঞান", SubjectCode = "142", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Finance & Banking", SubjectNameBn = "ফাইন্যান্স ও ব্যাংকিং", SubjectCode = "143", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Business Entrepreneurship", SubjectNameBn = "ব্যবসায় উদ্যোগ", SubjectCode = "144", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Economics", SubjectNameBn = "অর্থনীতি", SubjectCode = "141", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Information & Communication Technology", SubjectNameBn = "তথ্য ও যোগাযোগ প্রযুক্তি", SubjectCode = "139", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 50, PassMarks = 17, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 25, PassMarks = 8, DisplayOrder = 2 }, new() { ComponentName = "Practical", ComponentCode = "PRACTICAL", FullMarks = 25, PassMarks = 8, DisplayOrder = 3, IsPractical = true }] },
                new() { SubjectName = "Islam & Moral Education", SubjectNameBn = "ইসলাম ও নৈতিক শিক্ষা", SubjectCode = "111", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Bangladesh & Global Studies", SubjectNameBn = "বাংলাদেশ ও বিশ্বপরিচয়", SubjectCode = "150", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Physical Education & Health", SubjectNameBn = "শারীরিক শিক্ষা ও স্বাস্থ্য", SubjectCode = "190", FullMarks = 50, PassMarks = 17, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 25, PassMarks = 8, DisplayOrder = 1 }, new() { ComponentName = "Practical", ComponentCode = "PRACTICAL", FullMarks = 25, PassMarks = 9, DisplayOrder = 2, IsPractical = true }] }
            ]
        };

        // Humanities group (classes 9-10)
        var humanities = new NctbTemplateDto
        {
            GroupName = "Humanities",
            GroupCode = "HUMANITIES",
            Subjects =
            [
                new() { SubjectName = "Bangla", SubjectNameBn = "বাংলা", SubjectCode = "101", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "English", SubjectNameBn = "ইংরেজি", SubjectCode = "107", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Mathematics", SubjectNameBn = "গণিত", SubjectCode = "109", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "History", SubjectNameBn = "ইতিহাস", SubjectCode = "145", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Geography", SubjectNameBn = "ভূগোল", SubjectCode = "146", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Civics & Citizenship", SubjectNameBn = "পৌরনীতি ও নাগরিকতা", SubjectCode = "147", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Economics", SubjectNameBn = "অর্থনীতি", SubjectCode = "141", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Information & Communication Technology", SubjectNameBn = "তথ্য ও যোগাযোগ প্রযুক্তি", SubjectCode = "139", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 50, PassMarks = 17, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 25, PassMarks = 8, DisplayOrder = 2 }, new() { ComponentName = "Practical", ComponentCode = "PRACTICAL", FullMarks = 25, PassMarks = 8, DisplayOrder = 3, IsPractical = true }] },
                new() { SubjectName = "Islam & Moral Education", SubjectNameBn = "ইসলাম ও নৈতিক শিক্ষা", SubjectCode = "111", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Bangladesh & Global Studies", SubjectNameBn = "বাংলাদেশ ও বিশ্বপরিচয়", SubjectCode = "150", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Physical Education & Health", SubjectNameBn = "শারীরিক শিক্ষা ও স্বাস্থ্য", SubjectCode = "190", FullMarks = 50, PassMarks = 17, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 25, PassMarks = 8, DisplayOrder = 1 }, new() { ComponentName = "Practical", ComponentCode = "PRACTICAL", FullMarks = 25, PassMarks = 9, DisplayOrder = 2, IsPractical = true }] }
            ]
        };

        // General template for classes 1-8 (no group differentiation)
        var primaryGeneral = new NctbTemplateDto
        {
            GroupName = "General",
            GroupCode = "GENERAL",
            Subjects =
            [
                new() { SubjectName = "Bangla", SubjectNameBn = "বাংলা", SubjectCode = "101", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "English", SubjectNameBn = "ইংরেজি", SubjectCode = "107", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Mathematics", SubjectNameBn = "গণিত", SubjectCode = "109", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Science", SubjectNameBn = "বিজ্ঞান", SubjectCode = "160", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Bangladesh & Global Studies", SubjectNameBn = "বাংলাদেশ ও বিশ্বপরিচয়", SubjectCode = "150", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Islam & Moral Education", SubjectNameBn = "ইসলাম ও নৈতিক শিক্ষা", SubjectCode = "111", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 70, PassMarks = 23, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 30, PassMarks = 10, DisplayOrder = 2 }] },
                new() { SubjectName = "Information & Communication Technology", SubjectNameBn = "তথ্য ও যোগাযোগ প্রযুক্তি", SubjectCode = "139", FullMarks = 100, PassMarks = 33, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 50, PassMarks = 17, DisplayOrder = 1 }, new() { ComponentName = "MCQ", ComponentCode = "MCQ", FullMarks = 25, PassMarks = 8, DisplayOrder = 2 }, new() { ComponentName = "Practical", ComponentCode = "PRACTICAL", FullMarks = 25, PassMarks = 8, DisplayOrder = 3, IsPractical = true }] },
                new() { SubjectName = "Physical Education & Health", SubjectNameBn = "শারীরিক শিক্ষা ও স্বাস্থ্য", SubjectCode = "190", FullMarks = 50, PassMarks = 17, Components = [new() { ComponentName = "Written", ComponentCode = "WRITTEN", FullMarks = 25, PassMarks = 8, DisplayOrder = 1 }, new() { ComponentName = "Practical", ComponentCode = "PRACTICAL", FullMarks = 25, PassMarks = 9, DisplayOrder = 2, IsPractical = true }] },
                new() { SubjectName = "Arts & Crafts", SubjectNameBn = "চারু ও কারুকলা", SubjectCode = "180", FullMarks = 50, PassMarks = 17, Components = [new() { ComponentName = "Practical", ComponentCode = "PRACTICAL", FullMarks = 50, PassMarks = 17, DisplayOrder = 1, IsPractical = true }] }
            ]
        };

        var templates = new List<NctbTemplateDto>();

        if (classId >= 9)
        {
            if (classId <= 10)
            {
                templates.Add(science);
                templates.Add(businessStudies);
                templates.Add(humanities);
            }
        }
        else
        {
            templates.Add(primaryGeneral);
        }

        return templates;
    }

    // ──────────────────────────────────────────────
    // Template Persistence
    // ──────────────────────────────────────────────

    public async Task<ExamTemplateDto> SaveTemplateAsync(SaveTemplateRequest request, string userId, CancellationToken ct = default)
    {
        var classIdsJson = JsonSerializer.Serialize(request.SelectedClassIds);
        var subjectsJson = JsonSerializer.Serialize(request.Subjects);

        var entity = new ExamTemplateEntity
        {
            Name = request.Name,
            Description = request.Description,
            AcademicYearId = request.AcademicYearId,
            Term = request.Term,
            ExamType = request.ExamType,
            ClassIdsJson = classIdsJson,
            TemplateDataJson = subjectsJson,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var repo = _uow.Repository<ExamTemplateEntity>();
        await repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(entity);
    }

    public async Task<ExamWizardStateDto?> LoadTemplateAsync(int templateId, CancellationToken ct = default)
    {
        var repo = _uow.Repository<ExamTemplateEntity>();
        var entity = await repo.Query()
            .AsNoTracking()
            .Where(t => t.Id == templateId && !t.IsDeleted && t.IsActive)
            .FirstOrDefaultAsync(ct);

        if (entity == null) return null;

        var year = await _uow.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .Where(y => y.Id == entity.AcademicYearId)
            .FirstOrDefaultAsync(ct);

        var classIds = JsonSerializer.Deserialize<List<int>>(entity.ClassIdsJson) ?? [];
        var classNames = await _uow.Repository<SchoolClass>().Query()
            .AsNoTracking()
            .Where(c => classIds.Contains(c.Id))
            .Select(c => c.Name)
            .ToListAsync(ct);

        var subjects = JsonSerializer.Deserialize<List<ExamWizardSubjectDto>>(entity.TemplateDataJson) ?? [];

        return new ExamWizardStateDto
        {
            Step = 2,
            AcademicYearId = entity.AcademicYearId,
            AcademicYearName = year?.Name ?? "",
            Term = entity.Term,
            TermName = entity.Term.ToString(),
            ExamType = entity.ExamType,
            SelectedClassIds = classIds,
            SelectedClassNames = classNames,
            Subjects = subjects,
            SourceExamName = $"Template: {entity.Name}"
        };
    }

    public async Task<List<ExamTemplateListItemDto>> ListTemplatesAsync(int? academicYearId, ExamTerm? term, CancellationToken ct = default)
    {
        var query = _uow.Repository<ExamTemplateEntity>().Query()
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.IsActive);

        if (academicYearId.HasValue)
            query = query.Where(t => t.AcademicYearId == academicYearId.Value);

        if (term.HasValue)
            query = query.Where(t => t.Term == term.Value);

        var entities = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

        return entities.Select(e =>
        {
            var classIds = JsonSerializer.Deserialize<List<int>>(e.ClassIdsJson) ?? [];
            var subjects = JsonSerializer.Deserialize<List<ExamWizardSubjectDto>>(e.TemplateDataJson) ?? [];
            return new ExamTemplateListItemDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                TermName = e.Term.ToString(),
                ExamType = e.ExamType,
                ClassCount = $"{classIds.Count} class(es)",
                SubjectCount = subjects.Count,
                CreatedBy = e.CreatedBy,
                CreatedAt = e.CreatedAt
            };
        }).ToList();
    }

    public async Task<bool> DeleteTemplateAsync(int templateId, CancellationToken ct = default)
    {
        var repo = _uow.Repository<ExamTemplateEntity>();
        var entity = await repo.FirstOrDefaultAsync(t => t.Id == templateId && !t.IsDeleted, ct);
        if (entity == null) return false;

        entity.IsDeleted = true;
        repo.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    private static ExamTemplateDto MapToDto(ExamTemplateEntity entity)
    {
        var classIds = JsonSerializer.Deserialize<List<int>>(entity.ClassIdsJson) ?? [];
        var subjects = JsonSerializer.Deserialize<List<ExamWizardSubjectDto>>(entity.TemplateDataJson) ?? [];
        return new ExamTemplateDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            AcademicYearId = entity.AcademicYearId,
            Term = entity.Term,
            TermName = entity.Term.ToString(),
            ExamType = entity.ExamType,
            SelectedClassIds = classIds,
            Subjects = subjects,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt
        };
    }
}