using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using StudentPortalResultViewModel = SchoolManagementSystem.Models.ViewModels.Result.StudentPortalResultViewModel;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ResultPublicationService : IResultPublicationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMeritCalculationService _meritCalculationService;
    private readonly IResultCalculationService _resultCalculationService;
    private readonly IExamRepository _examRepository;
    private readonly IMarkEntryRepository _markEntryRepository;
    private readonly IResultPublicationRepository _resultPublicationRepository;
    private readonly IStudentSubjectResultRepository _subjectResultRepository;
    private readonly IStudentExamResultRepository _examResultRepository;

    public ResultPublicationService(
        IUnitOfWork uow,
        IMeritCalculationService meritCalculationService,
        IResultCalculationService resultCalculationService,
        IExamRepository examRepository,
        IMarkEntryRepository markEntryRepository,
        IResultPublicationRepository resultPublicationRepository,
        IStudentSubjectResultRepository subjectResultRepository,
        IStudentExamResultRepository examResultRepository)
    {
        _uow = uow;
        _meritCalculationService = meritCalculationService;
        _resultCalculationService = resultCalculationService;
        _examRepository = examRepository;
        _markEntryRepository = markEntryRepository;
        _resultPublicationRepository = resultPublicationRepository;
        _subjectResultRepository = subjectResultRepository;
        _examResultRepository = examResultRepository;
    }

    public async Task SubmitExamResultsAsync(int examId, int classId)
    {
        var marks = await _markEntryRepository.Query()
            .Where(x => x.ExamId == examId && x.Student.ClassId == classId)
            .ToListAsync();

        foreach (var mark in marks)
        {
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
            mark.Status = ResultWorkflowStatus.Approved;
            _markEntryRepository.Update(mark);
        }

        await _uow.SaveChangesAsync();
    }

    public async Task PublishResultsAsync(ResultPublishDto dto)
    {
        var exam = await _examRepository.GetByIdAsync(dto.ExamId);
        if (exam == null) return;

        // Calculate Ranking / GPA / Position
        await _meritCalculationService.RecalculateMeritPositionsAsync(dto.ExamId);

        // Get Marks
        var marks = await _markEntryRepository.Query()
            .Include(x => x.Subject)
            .Where(x => x.ExamId == dto.ExamId)
            .ToListAsync();

        // Update Mark Status + Create StudentSubjectResults
        foreach (var mark in marks)
        {
            // Lock Result
            if (dto.LockResults) mark.IsLocked = true;

            // Publish Status
            mark.Status = ResultWorkflowStatus.Published;
            _markEntryRepository.Update(mark);

            // Prevent Duplicate Result Insert
            bool exists = await _subjectResultRepository.AnyAsync(x =>
                x.StudentId == mark.StudentId &&
                x.ExamId == mark.ExamId &&
                x.SubjectId == mark.SubjectId);

            if (!exists)
            {
                var subjectResult = new StudentSubjectResult
                {
                    ExamId = mark.ExamId,
                    StudentId = mark.StudentId,
                    SubjectId = mark.SubjectId,
                    MarksObtained = mark.MarksObtained,
                    Grade = mark.Grade ?? "",
                    GradePoint = mark.GradePoint ?? 0,
                    IsPassed = mark.MarksObtained >= mark.Subject.DefaultPassMarks,
                    FullMarks = mark.Subject.DefaultFullMarks,
                    PassMarks = mark.Subject.DefaultPassMarks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    IsDeleted = false
                };
                await _subjectResultRepository.AddAsync(subjectResult);
            }
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

    public async Task<StudentPortalResultViewModel> GetStudentResultsAsync(int studentId)
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

        var viewModel = new StudentPortalResultViewModel
        {
            StudentId = studentId,
            StudentName = student?.FullName ?? "",
            ExamResults = examResults.Select(r => new StudentExamResultDto
            {
                ExamId = r.ExamId,
                ExamName = r.Exam?.Name ?? "Exam",
                Gpa = r.Gpa,
                TotalMarks = r.TotalMarks,
                Position = r.Position,
                Status = r.Status,
                Subjects = subjectResults
                    .Where(s => s.ExamId == r.ExamId)
                    .Select(s => new StudentSubjectResultDto
                    {
                        SubjectName = s.Subject.Name,
                        ObtainedMarks = s.MarksObtained,
                        FullMarks = s.FullMarks,
                        Grade = s.Grade,
                        GPA = s.GradePoint,
                        IsPassed = s.IsPassed
                    })
                    .ToList()
            }).ToList()
        };

        return viewModel;
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
                "draft" => ResultWorkflowStatus.Draft,
                _ => (ResultWorkflowStatus?)null
            };
            if (statusFilter.HasValue) query = query.Where(r => r.Status == statusFilter.Value);
        }

        var results = await query.OrderByDescending(r => r.CreatedAt).Take(1000).ToListAsync();
        
        return results.Select(r => new StudentExamResultDto
        {
            ExamId = r.ExamId,
            TotalMarks = r.TotalMarks,
            Gpa = r.Gpa,
            Grade = r.Grade,
            Position = r.Position,
            IsPassed = r.IsPassed
        });
    }

    public async Task RecalculateResultsAsync(int examId)
    {
        await _resultCalculationService.CalculateExamResultsAsync(examId);
    }

    public async Task RecalculateMeritPositionsAsync(int examId)
    {
        await _meritCalculationService.RecalculateMeritPositionsAsync(examId);
    }
}

