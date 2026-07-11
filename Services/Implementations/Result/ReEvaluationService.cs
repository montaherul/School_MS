using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ReEvaluationService : IReEvaluationService
{
    private readonly IUnitOfWork _uow;
    private readonly IReEvaluationRequestRepository _reEvaluationRequestRepository;
    private readonly IMarkEntryRepository _markEntryRepository;
    private readonly IStudentSubjectResultRepository _subjectResultRepository;
    private readonly IGradeCalculator _gradeCalculator;
    private readonly IGradingRuleRepository _gradingRuleRepository;
    private readonly IStudentExamResultRepository _examResultRepository;
    private readonly IMeritCalculationService _meritCalculationService;
    private readonly IPassFailPolicy _passFailPolicy;
    private readonly IResultCalculationService _resultCalculationService;

    public ReEvaluationService(
        IUnitOfWork uow,
        IReEvaluationRequestRepository reEvaluationRequestRepository,
        IMarkEntryRepository markEntryRepository,
        IStudentSubjectResultRepository subjectResultRepository,
        IGradeCalculator gradeCalculator,
        IGradingRuleRepository gradingRuleRepository,
        IStudentExamResultRepository examResultRepository,
        IMeritCalculationService meritCalculationService,
        IPassFailPolicy passFailPolicy,
        IResultCalculationService resultCalculationService)
    {
        _uow = uow;
        _reEvaluationRequestRepository = reEvaluationRequestRepository;
        _markEntryRepository = markEntryRepository;
        _subjectResultRepository = subjectResultRepository;
        _gradeCalculator = gradeCalculator;
        _gradingRuleRepository = gradingRuleRepository;
        _examResultRepository = examResultRepository;
        _meritCalculationService = meritCalculationService;
        _passFailPolicy = passFailPolicy;
        _resultCalculationService = resultCalculationService;
    }

    public async Task RequestReEvaluationAsync(ReEvaluationRequestDto dto, int requestedByUserId)
    {
        var mark = await _markEntryRepository.Query()
            .FirstOrDefaultAsync(m => m.ExamId == dto.ExamId && m.StudentId == dto.StudentId && m.SubjectId == dto.SubjectId);

        if (mark == null) throw new InvalidOperationException("Marks not found");

        var existingReq = await _reEvaluationRequestRepository.Query()
            .FirstOrDefaultAsync(r => r.ExamId == dto.ExamId && r.StudentId == dto.StudentId && r.SubjectId == dto.SubjectId && r.Status == ReEvaluationStatus.Requested);

        if (existingReq != null) throw new InvalidOperationException("A pending re-evaluation request already exists.");

        var req = new ReEvaluationRequest
        {
            ExamId = dto.ExamId,
            StudentId = dto.StudentId,
            SubjectId = dto.SubjectId,
            RequestedByUserId = requestedByUserId,
            RequestReason = dto.Reason,
            Notes = dto.Notes,
            Status = ReEvaluationStatus.Requested,
            OldMarks = mark.MarksObtained,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = requestedByUserId.ToString()
        };
        await _reEvaluationRequestRepository.AddAsync(req);
        await _uow.SaveChangesAsync();
    }

    public async Task ProcessReEvaluationAsync(ReEvaluationProcessDto dto, int adminId)
    {
        var req = await _reEvaluationRequestRepository.GetByIdAsync(dto.RequestId);
        if (req == null || req.Status != ReEvaluationStatus.Requested) return;

        req.Status = dto.Approved ? ReEvaluationStatus.Revised : ReEvaluationStatus.Rejected;
        req.ApprovedByUserId = adminId;
        req.ApprovedAt = DateTime.UtcNow;
        req.Notes = dto.AdminNotes;

        if (dto.Approved && dto.NewMarks.HasValue)
        {
            req.NewMarks = dto.NewMarks.Value;

            // Update the mark entry with new marks
            var mark = await _markEntryRepository.Query().FirstOrDefaultAsync(m => m.ExamId == req.ExamId && m.StudentId == req.StudentId && m.SubjectId == req.SubjectId);
            if (mark != null)
            {
                mark.MarksObtained = dto.NewMarks.Value;
                _markEntryRepository.Update(mark);
            }

            // Recalculate subject-level result (Grade, GradePoint, IsPassed)
            var subResult = await _subjectResultRepository.Query().FirstOrDefaultAsync(sr => sr.ExamId == req.ExamId && sr.StudentId == req.StudentId && sr.SubjectId == req.SubjectId);
            if (subResult != null)
            {
                subResult.MarksObtained = dto.NewMarks.Value;
                var gradingRules = await _gradingRuleRepository.ListAsync();
                var (grade, gradePoint) = _gradeCalculator.CalculateGrade(dto.NewMarks.Value, gradingRules);
                subResult.Grade = grade ?? "F";
                subResult.GradePoint = gradePoint ?? 0;
                subResult.IsPassed = dto.NewMarks.Value >= subResult.PassMarks;
                _subjectResultRepository.Update(subResult);
            }

            // Recalculate aggregate StudentExamResult (GPA, Grade, IsPassed, FailedSubjectCount, PassedSubjectCount)
            var allSubjectResults = await _subjectResultRepository.Query()
                .Where(sr => sr.ExamId == req.ExamId && sr.StudentId == req.StudentId)
                .ToListAsync();

            var examResult = await _examResultRepository.Query()
                .FirstOrDefaultAsync(er => er.ExamId == req.ExamId && er.StudentId == req.StudentId);

            if (examResult != null && allSubjectResults.Any())
            {
                var gpa = await _resultCalculationService.CalculateGpaAsync(allSubjectResults);
                var totalMarks = allSubjectResults.Sum(sr => sr.MarksObtained);
                var passedCount = allSubjectResults.Count(r => r.IsPassed);
                var failedCount = allSubjectResults.Count(r => !r.IsPassed);
                var exam = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().GetByIdAsync(req.ExamId);
                int academicYearId = exam?.AcademicYearId ?? 0;
                var setting = await _uow.Repository<ResultSetting>().Query()
                    .Where(rs => rs.AcademicYearId == academicYearId && rs.IsActive && !rs.IsDeleted)
                    .FirstOrDefaultAsync() ?? new ResultSetting();
                var (isPassed, _) = _passFailPolicy.DeterminePassFailStatus(allSubjectResults, setting);

                examResult.Gpa = gpa;
                examResult.Grade = _gradeCalculator.GetOverallGrade(gpa);
                examResult.TotalMarks = totalMarks;
                examResult.FailedSubjectCount = failedCount;
                examResult.PassedSubjectCount = passedCount;
                examResult.IsPassed = isPassed;
                _examResultRepository.Update(examResult);
            }

            // Recalculate merit positions for the exam
            await _meritCalculationService.RecalculateMeritPositionsAsync(req.ExamId);
        }

        _reEvaluationRequestRepository.Update(req);
        await _uow.SaveChangesAsync();
    }

    public async Task<ReEvaluationDashboardDto> GetReEvaluationDashboardAsync()
    {
        var requests = await _reEvaluationRequestRepository.QueryNoTracking()
            .Include(r => r.Exam)
            .Include(r => r.Student)
            .Include(r => r.Subject)
            .Where(r => !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        static ReEvaluationRequestItemDto ToItem(ReEvaluationRequest r) => new()
        {
            Id = r.Id,
            StudentId = r.StudentId,
            SubjectId = r.SubjectId,
            ExamId = r.ExamId,
            ExamName = r.Exam.Name,
            StudentName = r.Student.FullName,
            SubjectName = r.Subject.Name,
            OldMarks = r.OldMarks,
            NewMarks = r.NewMarks,
            Status = r.Status,
            Notes = r.Notes,
            CreatedAt = r.CreatedAt
        };

        return new ReEvaluationDashboardDto
        {
            PendingRequests = requests.Where(r => r.Status == ReEvaluationStatus.Requested).Select(ToItem).ToList(),
            CompletedRequests = requests.Where(r => r.Status != ReEvaluationStatus.Requested).Select(ToItem).ToList()
        };
    }
}
