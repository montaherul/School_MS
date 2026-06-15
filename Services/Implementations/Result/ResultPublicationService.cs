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

public class ResultPublicationService : IResultPublicationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMeritCalculationService _meritCalculationService;
    private readonly IExamRepository _examRepository;
    private readonly IMarkEntryRepository _markEntryRepository;
    private readonly IResultPublicationRepository _resultPublicationRepository;
    private readonly IStudentSubjectResultRepository _subjectResultRepository;
    private readonly IStudentExamResultRepository _examResultRepository;
    private readonly IGradingRuleRepository _gradingRuleRepository;
    private readonly IGradeCalculator _gradeCalculator;
    private readonly IComponentAggregator _componentAggregator;
    private readonly IPassFailPolicy _passFailPolicy;

    public ResultPublicationService(
        IUnitOfWork uow,
        IMeritCalculationService meritCalculationService,
        IExamRepository examRepository,
        IMarkEntryRepository markEntryRepository,
        IResultPublicationRepository resultPublicationRepository,
        IStudentSubjectResultRepository subjectResultRepository,
        IStudentExamResultRepository examResultRepository,
        IGradingRuleRepository gradingRuleRepository,
        IGradeCalculator gradeCalculator,
        IComponentAggregator componentAggregator,
        IPassFailPolicy passFailPolicy)
    {
        _uow = uow;
        _meritCalculationService = meritCalculationService;
        _examRepository = examRepository;
        _markEntryRepository = markEntryRepository;
        _resultPublicationRepository = resultPublicationRepository;
        _subjectResultRepository = subjectResultRepository;
        _examResultRepository = examResultRepository;
        _gradingRuleRepository = gradingRuleRepository;
        _gradeCalculator = gradeCalculator;
        _componentAggregator = componentAggregator;
        _passFailPolicy = passFailPolicy;
    }

    public async Task SubmitExamResultsAsync(int examId, int classId)
    {
        var marks = await _markEntryRepository.Query()
            .Where(x => x.ExamId == examId && x.Student.ClassId == classId)
            .ToListAsync();

        foreach (var mark in marks)
        {
            if (mark.Status != ResultWorkflowStatus.Draft)
                throw new InvalidOperationException($"Mark entry for StudentId {mark.StudentId} is not in Draft status (current: {mark.Status})");
            mark.Status = ResultWorkflowStatus.Submitted;
            _markEntryRepository.Update(mark);
        }

        await _uow.SaveChangesAsync();
    }

    public async Task ApproveExamResultsAsync(int examId)
    {
        var marks = await _markEntryRepository.ListAsync(x => x.ExamId == examId);

        foreach (var mark in marks)
        {
            if (mark.Status != ResultWorkflowStatus.Submitted && mark.Status != ResultWorkflowStatus.Reviewed)
                throw new InvalidOperationException($"Mark entry for StudentId {mark.StudentId} must be Submitted or Reviewed before approval (current: {mark.Status})");
            mark.Status = ResultWorkflowStatus.Approved;
            _markEntryRepository.Update(mark);
        }

        await _uow.SaveChangesAsync();
    }

    public async Task PublishResultsAsync(ResultPublishDto dto)
    {
        var exam = await _examRepository.GetByIdAsync(dto.ExamId);
        if (exam == null) return;

        if (exam.Status == ResultWorkflowStatus.Published)
            throw new InvalidOperationException("Exam results are already published");

        if (exam.Status == ResultWorkflowStatus.Locked)
            throw new InvalidOperationException("Exam results are locked and cannot be published");

        // Get Marks and preload needed data
        var marks = await _markEntryRepository.Query()
            .Include(x => x.Subject)
            .Where(x => x.ExamId == dto.ExamId)
            .ToListAsync();

        var gradingRules = await _gradingRuleRepository.ListAsync();
        var examSubjects = await _uow.Repository<ExamSubject>().Query()
            .Where(es => es.ExamId == dto.ExamId)
            .ToDictionaryAsync(es => es.SubjectId);

        // Preload ClassSubject mappings for class-specific pass/full marks
        var classIds = marks.Where(m => m.ClassId > 0).Select(m => m.ClassId).Distinct().ToList();
        var subjectIds = marks.Select(m => m.SubjectId).Distinct().ToList();
        var classSubjects = await _uow.Repository<ClassSubject>().Query()
            .Where(cs => classIds.Contains(cs.SchoolClassId) && subjectIds.Contains(cs.SubjectId) && !cs.IsDeleted && cs.IsActive)
            .ToListAsync();
        var classSubjectLookup = classSubjects
            .GroupBy(cs => (cs.SchoolClassId, cs.SubjectId))
            .ToDictionary(g => g.Key, g => g.First());

        // Update Mark Status + Create StudentSubjectResults using GradeCalculator
        foreach (var mark in marks)
        {
            // Lock Result
            if (dto.LockResults) mark.IsLocked = true;

            // Publish Status
            if (mark.Status == ResultWorkflowStatus.Published)
                continue;

            mark.Status = ResultWorkflowStatus.Published;
            _markEntryRepository.Update(mark);

            var cs = classSubjectLookup.GetValueOrDefault((mark.ClassId, mark.SubjectId));
            examSubjects.TryGetValue(mark.SubjectId, out var examSubject);

            // Prevent Duplicate Result Insert
            bool exists = await _subjectResultRepository.AnyAsync(x =>
                x.StudentId == mark.StudentId &&
                x.ExamId == mark.ExamId &&
                x.SubjectId == mark.SubjectId);

            if (!exists)
            {
                decimal totalMarks = _componentAggregator.AggregateAll(mark);
                var (grade, gradePoint) = _gradeCalculator.CalculateGrade(totalMarks, gradingRules);
                bool isPassed = totalMarks >= (examSubject?.PassMarks ?? cs?.PassMarks ?? 33);

                var subjectResult = new StudentSubjectResult
                {
                    ExamId = mark.ExamId,
                    StudentId = mark.StudentId,
                    SubjectId = mark.SubjectId,
                    AcademicYearId = mark.AcademicYearId,
                    ClassId = mark.ClassId,
                    SectionId = mark.SectionId,
                    StudentGroupId = null,
                    IsOptionalSubject = cs?.IsOptional ?? false,
                    IsReligionSubject = cs?.IsReligionSubject ?? false,
                    MarksObtained = totalMarks,
                    FullMarks = examSubject?.FullMarks ?? cs?.FullMarks ?? 100,
                    PassMarks = examSubject?.PassMarks ?? cs?.PassMarks ?? 33,
                    Grade = grade ?? "F",
                    GradePoint = gradePoint ?? 0,
                    IsPassed = isPassed,
                    CalculatedAt = DateTime.Now
                };
                await _subjectResultRepository.AddAsync(subjectResult);
            }
        }

        // Calculate Ranking / GPA / Position after subject results are created
        await _meritCalculationService.RecalculateMeritPositionsAsync(dto.ExamId);

        // Update StudentExamResults with PublishedAt and Published status
        var publishedAt = DateTime.UtcNow;
        var studentExamResults = await _examResultRepository.Query()
            .Where(r => r.ExamId == dto.ExamId && !r.IsDeleted)
            .ToListAsync();

        foreach (var ser in studentExamResults)
        {
            ser.PublishedAt = publishedAt;
            ser.Status = ResultWorkflowStatus.Published;
            _examResultRepository.Update(ser);
        }

        // Update Exam Status
        exam.Status = ResultWorkflowStatus.Published;
        _examRepository.Update(exam);

        // Result Publication Record
        var publication = await _resultPublicationRepository.FirstOrDefaultAsync(p => p.ExamId == dto.ExamId && !p.IsDeleted);

        if (publication == null)
        {
            publication = new ResultPublication
            {
                ExamId = dto.ExamId,
                Status = ResultWorkflowStatus.Published,
                PublishedAt = DateTime.UtcNow,
                IsLocked = dto.LockResults,
                LockedAt = dto.LockResults ? DateTime.UtcNow : null,
                CreatedAt = DateTime.UtcNow
            };
            await _resultPublicationRepository.AddAsync(publication);
        }
        else
        {
            publication.Status = ResultWorkflowStatus.Published;
            publication.PublishedAt = DateTime.UtcNow;
            publication.IsLocked = dto.LockResults;
            if (dto.LockResults) publication.LockedAt = DateTime.UtcNow;
            _resultPublicationRepository.Update(publication);
        }

        await _uow.SaveChangesAsync();
    }

    public async Task ReviewExamResultsAsync(int examId, int reviewerUserId)
    {
        var marks = await _markEntryRepository.ListAsync(x => x.ExamId == examId);
        foreach (var mark in marks)
        {
            if (mark.Status != ResultWorkflowStatus.Submitted)
                throw new InvalidOperationException($"Mark entry for StudentId {mark.StudentId} must be Submitted before review (current: {mark.Status})");
            mark.Status = ResultWorkflowStatus.Reviewed;
            _markEntryRepository.Update(mark);
        }
        await _uow.SaveChangesAsync();
    }

    public async Task ApproveReviewedResultsAsync(int examId, int approverUserId)
    {
        var marks = await _markEntryRepository.ListAsync(x => x.ExamId == examId);
        foreach (var mark in marks)
        {
            if (mark.Status != ResultWorkflowStatus.Reviewed)
                throw new InvalidOperationException($"Mark entry for StudentId {mark.StudentId} must be Reviewed before approval (current: {mark.Status})");
            mark.Status = ResultWorkflowStatus.Approved;
            _markEntryRepository.Update(mark);
        }
        await _uow.SaveChangesAsync();
    }

    public async Task UnpublishResultsAsync(int examId)
    {
        var exam = await _examRepository.GetByIdAsync(examId);
        if (exam == null) return;

        if (exam.Status != ResultWorkflowStatus.Published && exam.Status != ResultWorkflowStatus.Unpublished)
            throw new InvalidOperationException($"Exam must be Published or Unpublished to unpublish (current: {exam.Status})");
        
        exam.Status = ResultWorkflowStatus.Unpublished;
        _examRepository.Update(exam);
        
        var publication = await _resultPublicationRepository.FirstOrDefaultAsync(p => p.ExamId == examId && !p.IsDeleted);
        if (publication != null)
        {
            publication.Status = ResultWorkflowStatus.Unpublished;
            _resultPublicationRepository.Update(publication);
        }
        
        await _uow.SaveChangesAsync();
    }

    public async Task RepublishResultsAsync(int examId)
    {
        var exam = await _examRepository.GetByIdAsync(examId);
        if (exam == null) return;
        
        var dto = new ResultPublishDto { ExamId = examId, LockResults = true };
        await PublishResultsAsync(dto);
    }

    public async Task<ResultPublicationDto> GetPublicationStatusAsync(int examId)
    {
        var publication = await _resultPublicationRepository.FirstOrDefaultAsync(p => p.ExamId == examId && !p.IsDeleted);
        if (publication == null) return new ResultPublicationDto { ExamName = "Not Published" };
        
        return new ResultPublicationDto
        {
            Id = publication.Id,
            ExamName = publication.Exam?.Name ?? "",
            PublishedAt = publication.PublishedAt ?? publication.CreatedAt,
            IsPublished = publication.Status == ResultWorkflowStatus.Published
        };
    }

    public async Task<IEnumerable<ResultPublicationDto>> GetResultPublicationsAsync()
    {
        var activeYear = await _uow.Repository<AcademicYear>().FirstOrDefaultAsync(x => x.IsActive);
        if (activeYear == null) return Enumerable.Empty<ResultPublicationDto>();

        var pubs = await _resultPublicationRepository.Query()
            .Include(rp => rp.Exam)
            .Where(rp => rp.Exam.AcademicYearId == activeYear.Id && !rp.IsDeleted)
            .OrderByDescending(rp => rp.CreatedAt)
            .ToListAsync();

        return pubs.Select(p => new ResultPublicationDto
        {
            Id = p.Id,
            ExamName = p.Exam.Name,
            PublishedAt = p.CreatedAt,
            IsPublished = p.Status == ResultWorkflowStatus.Published
        });
    }

    public async Task<StudentPortalResultDto> GetStudentResultsAsync(int studentId)
    {
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().GetByIdAsync(studentId);

        // Exam Results
        var examResults = await _examResultRepository.Query()
            .Include(x => x.Exam)
            .Where(x =>
                x.StudentId == studentId &&
                (
                    x.Status == ResultWorkflowStatus.Published ||
                    x.Exam.Status == ResultWorkflowStatus.Published ||
                    x.Status == ResultWorkflowStatus.Locked
                ))
            .ToListAsync();

        // Subject Results
        var subjectResults = await _subjectResultRepository.Query()
            .Include(x => x.Subject)
            .Include(x => x.Exam)
            .Where(x => x.StudentId == studentId)
            .ToListAsync();

        var result = new StudentPortalResultDto
        {
            StudentId = studentId,
            StudentName = student?.FullName ?? "",
            ExamResults = examResults.Select(r => new StudentExamResultDto
            {
                ExamId = r.ExamId,
                ExamName = r.Exam?.Name ?? "Exam",
                Term = r.Exam?.Term ?? ExamTerm.Other,
                Gpa = r.Gpa,
                Grade = r.Grade,
                TotalMarks = r.TotalMarks,
                TotalFullMarks = r.TotalFullMarks,
                Position = r.Position,
                ClassPosition = r.ClassPosition,
                GroupPosition = r.GroupPosition,
                IsPassed = r.IsPassed,
                FailedSubjectCount = r.FailedSubjectCount,
                PassedSubjectCount = r.PassedSubjectCount,
                PublishedAt = r.PublishedAt,
                Status = r.Status,
                Subjects = subjectResults
                    .Where(s => s.ExamId == r.ExamId)
                    .Select(s => new StudentSubjectResultDto
                    {
                        SubjectId = s.SubjectId,
                        SubjectName = s.Subject?.Name ?? "",
                        SubjectNameBn = s.Subject?.NameBn ?? "",
                        SubjectGroup = s.Subject?.SubjectGroup ?? "",
                        MarksObtained = s.MarksObtained,
                        FullMarks = s.FullMarks,
                        PassMarks = s.PassMarks,
                        Grade = s.Grade,
                        GradePoint = s.GradePoint,
                        IsPassed = s.IsPassed,
                        ObtainedMarks = s.MarksObtained,
                        GPA = s.GradePoint
                    })
                    .ToList()
            }).ToList()
        };

        return result;
    }

    public async Task<IEnumerable<StudentExamResultDto>> GetAllResultsAsync(int? examId, int? classId, string? status)
    {
        var query = _examResultRepository.Query()
            .Include(r => r.Student)
            .ThenInclude(s => s.Class)
            .Where(r => !r.IsDeleted);

        if (examId.HasValue) query = query.Where(r => r.ExamId == examId.Value);
        if (classId.HasValue) query = query.Where(r => r.Student.ClassId == classId.Value);

        if (!string.IsNullOrEmpty(status))
        {
            var statusFilter = status.ToLower() switch
            {
                "published" => ResultWorkflowStatus.Published,
                "approved" => ResultWorkflowStatus.Approved,
                "reviewed" => ResultWorkflowStatus.Reviewed,
                "submitted" => ResultWorkflowStatus.Submitted,
                "draft" => ResultWorkflowStatus.Draft,
                "locked" => ResultWorkflowStatus.Locked,
                "unpublished" => ResultWorkflowStatus.Unpublished,
                _ => (ResultWorkflowStatus?)null
            };
            if (statusFilter.HasValue) query = query.Where(r => r.Status == statusFilter.Value);
        }

        var results = await query.OrderByDescending(r => r.CreatedAt).Take(1000).ToListAsync();
        
        return results.Select(r => new StudentExamResultDto
        {
            ExamId = r.ExamId,
            ExamName = r.Exam?.Name ?? "",
            TotalMarks = r.TotalMarks,
            TotalFullMarks = r.TotalFullMarks,
            Gpa = r.Gpa,
            Grade = r.Grade,
            Position = r.Position,
            ClassPosition = r.ClassPosition,
            GroupPosition = r.GroupPosition,
            IsPassed = r.IsPassed,
            FailedSubjectCount = r.FailedSubjectCount,
            PassedSubjectCount = r.PassedSubjectCount,
            PublishedAt = r.PublishedAt,
            Status = r.Status
        });
    }
}
