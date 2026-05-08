using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Data;
using SchoolManagementSystem.Helpers.Common;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ResultService : IResultService
{
    private readonly IUnitOfWork _uow;
    private readonly SchoolDbContext _db;

    public ResultService(IUnitOfWork uow, SchoolDbContext db)
    {
        _uow = uow;
        _db = db;
    }

    public async Task<IEnumerable<ExamUpsertDto>> GetExamsAsync(int academicYearId)
    {
        var exams = new List<ExamUpsertDto>();
        using (var command = _db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetExamsForAdmin";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@AcademicYearId", academicYearId));

            await _db.Database.OpenConnectionAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    exams.Add(new ExamUpsertDto
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Term = (ExamTerm)reader.GetInt32(2),
                        StartsOn = DateOnly.FromDateTime(reader.GetDateTime(3)),
                        EndsOn = DateOnly.FromDateTime(reader.GetDateTime(4)),
                        Status = (ResultWorkflowStatus)reader.GetInt32(5)
                    });
                }
            }
            await _db.Database.CloseConnectionAsync();
        }
        return exams;
    }

    public async Task CreateExamAsync(ExamUpsertDto dto)
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
        await _uow.SaveChangesAsync();
    }

    public async Task<MarkEntryViewModel> GetMarkEntryDataAsync(int examId, int subjectId, int classId, int sectionId)
    {
        var exam = await _uow.Repository<Exam>().GetByIdAsync(examId);
        var subject = await _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.Subject>().GetByIdAsync(subjectId);
        var schoolClass = await _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.SchoolClass>().GetByIdAsync(classId);
        var section = await _db.Sections.FindAsync(sectionId);

        var students = new List<StudentMarkViewModel>();
        
        using (var command = _db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetMarkEntrySheet";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@ExamId", examId));
            command.Parameters.Add(new SqlParameter("@ClassId", classId));
            command.Parameters.Add(new SqlParameter("@SectionId", sectionId));
            command.Parameters.Add(new SqlParameter("@SubjectId", subjectId));

            await _db.Database.OpenConnectionAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    students.Add(new StudentMarkViewModel
                    {
                        StudentId = reader.GetInt32(0),
                        StudentName = reader.GetString(1),
                        StudentNo = reader.GetString(2),
                        RollNumber = reader.GetInt32(3),
                        MarksObtained = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                        Grade = reader.IsDBNull(5) ? null : reader.GetString(5),
                        IsLocked = reader.IsDBNull(6) ? false : reader.GetBoolean(6)
                    });
                }
            }
            await _db.Database.CloseConnectionAsync();
        }

        return new MarkEntryViewModel
        {
            ExamId = examId,
            ExamName = exam?.Name ?? "",
            SubjectId = subjectId,
            SubjectName = subject?.IsReligionSubject == true && !string.IsNullOrEmpty(subject?.ReligionType)
                ? ReligionHelper.GetReligionSubjectName(subject.ReligionType)
                : subject?.Name ?? "",
            ClassId = classId,
            ClassName = section != null ? $"{schoolClass?.Name} - {section.Name}" : (schoolClass?.Name ?? ""),
            Students = students
        };
    }

    public async Task SubmitMarksBatchAsync(MarkBatchDto dto)
    {
        var gradingRules = await _uow.Repository<GradingRule>().ListAsync();

        foreach (var markDto in dto.Marks)
        {
            var existingMark = (await _uow.Repository<MarkEntry>()
                .ListAsync(x => x.ExamId == dto.ExamId && x.StudentId == markDto.StudentId && x.SubjectId == dto.SubjectId))
                .FirstOrDefault();

            if (existingMark != null && existingMark.IsLocked) continue;

            var (grade, gp) = CalculateGrade(markDto.MarksObtained, gradingRules);

            if (existingMark == null)
            {
                var newMark = new MarkEntry
                {
                    ExamId = dto.ExamId,
                    StudentId = markDto.StudentId,
                    SubjectId = dto.SubjectId,
                    MarksObtained = markDto.MarksObtained,
                    Grade = grade,
                    GradePoint = gp,
                    EnteredByTeacherId = dto.TeacherId,
                    Status = ResultWorkflowStatus.Submitted
                };
                await _uow.Repository<MarkEntry>().AddAsync(newMark);
            }
            else
            {
                // Audit Log
                if (existingMark.MarksObtained != markDto.MarksObtained)
                {
                    var audit = new ResultAuditLog
                    {
                        ExamId = dto.ExamId,
                        StudentId = markDto.StudentId,
                        SubjectId = dto.SubjectId,
                        OldMarks = existingMark.MarksObtained,
                        NewMarks = markDto.MarksObtained,
                        ChangedByUserId = dto.TeacherId,
                        Reason = "Teacher update"
                    };
                    await _uow.Repository<ResultAuditLog>().AddAsync(audit);
                }

                existingMark.MarksObtained = markDto.MarksObtained;
                existingMark.Grade = grade;
                existingMark.GradePoint = gp;
                existingMark.EnteredByTeacherId = dto.TeacherId;
                existingMark.Status = ResultWorkflowStatus.Submitted;
                _uow.Repository<MarkEntry>().Update(existingMark);
            }
        }

        await _uow.SaveChangesAsync();
    }

    public async Task SubmitExamResultsAsync(int examId, int classId)
    {
        var marks = await _uow.Repository<MarkEntry>()
            .ListAsync(x => x.ExamId == examId && x.Student.ClassId == classId);

        foreach (var mark in marks)
        {
            mark.Status = ResultWorkflowStatus.Submitted;
            _uow.Repository<MarkEntry>().Update(mark);
        }

        await _uow.SaveChangesAsync();
    }

    public async Task ApproveExamResultsAsync(int examId)
    {
        var marks = await _uow.Repository<MarkEntry>().ListAsync(x => x.ExamId == examId);

        foreach (var mark in marks)
        {
            mark.Status = ResultWorkflowStatus.Approved;
            _uow.Repository<MarkEntry>().Update(mark);
        }

        await _uow.SaveChangesAsync();
    }

    public async Task PublishResultsAsync(ResultPublishDto dto)
    {
        var exam = await _uow.Repository<Exam>()
            .GetByIdAsync(dto.ExamId);

        if (exam == null)
            return;

        // Calculate Ranking / GPA / Position
        await CalculateRankingAsync(dto.ExamId);

        // Get Marks
        var marks = await _db.Marks
            .Include(x => x.Subject)
            .Where(x => x.ExamId == dto.ExamId)
            .ToListAsync();

        // Update Mark Status + Create StudentSubjectResults
        foreach (var mark in marks)
        {
            // Lock Result
            if (dto.LockResults)
                mark.IsLocked = true;

            // Publish Status
            mark.Status = ResultWorkflowStatus.Published;

            _db.Marks.Update(mark);

            // Prevent Duplicate Result Insert
            bool exists = await _db.StudentSubjectResults
                .AnyAsync(x =>
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

                    IsPassed = mark.MarksObtained >=
                               mark.Subject.DefaultPassMarks,

                    FullMarks = mark.Subject.DefaultFullMarks,

                    PassMarks = mark.Subject.DefaultPassMarks,

                    CreatedAt = DateTime.UtcNow,

                    CreatedBy = "system",

                    IsDeleted = false
                };

                await _db.StudentSubjectResults
                    .AddAsync(subjectResult);
            }
        }

        // Update Exam Status
        exam.Status = ResultWorkflowStatus.Published;

        _uow.Repository<Exam>().Update(exam);

        // Result Publication Record
        var publication = await _db.ResultPublications
            .FirstOrDefaultAsync(p =>
                p.ExamId == dto.ExamId &&
                !p.IsDeleted);

        if (publication == null)
        {
            publication = new ResultPublication
            {
                ExamId = dto.ExamId,

                Status = ResultWorkflowStatus.Published,

                PublishedAt = DateTime.UtcNow,

                IsLocked = dto.LockResults,

                LockedAt = dto.LockResults
                    ? DateTime.UtcNow
                    : null,

                CreatedAt = DateTime.UtcNow
            };

            await _db.ResultPublications
                .AddAsync(publication);
        }
        else
        {
            publication.Status = ResultWorkflowStatus.Published;

            publication.PublishedAt = DateTime.UtcNow;

            publication.IsLocked = dto.LockResults;

            if (dto.LockResults)
                publication.LockedAt = DateTime.UtcNow;

            _db.ResultPublications.Update(publication);
        }

        await _uow.SaveChangesAsync();
    }

    public async Task CalculateRankingAsync(int examId)
    {
        await _db.Database.ExecuteSqlRawAsync("EXEC sp_CalculateExamRanking @ExamId", new SqlParameter("@ExamId", examId));
    }

    public async Task<Models.ViewModels.Result.StudentPortalResultViewModel>
      GetStudentResultsAsync(int studentId)
    {
        var student = await _uow
            .Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
            .GetByIdAsync(studentId);

        // Exam Results
        var examResults = await _db.StudentExamResults
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
        var subjectResults = await _db.StudentSubjectResults
            .Include(x => x.Subject)
            .Include(x => x.Exam)
            .Where(x => x.StudentId == studentId)
            .ToListAsync();

        var viewModel = new Models.ViewModels.Result.StudentPortalResultViewModel
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

                // SUBJECT LIST
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

    public async Task RequestReEvaluationAsync(ReEvaluationRequestDto dto, int requestedByUserId)
    {
        var existingMark = (await _uow.Repository<MarkEntry>()
            .ListAsync(x => x.ExamId == dto.ExamId && x.StudentId == dto.StudentId && x.SubjectId == dto.SubjectId))
            .FirstOrDefault();

        if (existingMark == null) throw new Exception("Mark record not found");

        var request = new ReEvaluationRequest
        {
            ExamId = dto.ExamId,
            StudentId = dto.StudentId,
            SubjectId = dto.SubjectId,
            RequestedByUserId = requestedByUserId,
            OldMarks = existingMark.MarksObtained,
            Notes = dto.Notes,
            Status = ReEvaluationStatus.Requested
        };

        await _uow.Repository<ReEvaluationRequest>().AddAsync(request);
        await _uow.SaveChangesAsync();
    }

    public async Task ProcessReEvaluationAsync(ReEvaluationProcessDto dto, int adminId)
    {
        var request = await _uow.Repository<ReEvaluationRequest>().GetByIdAsync(dto.RequestId);
        if (request == null) return;

        if (dto.Approved && dto.NewMarks.HasValue)
        {
            request.Status = ReEvaluationStatus.Approved;
            request.NewMarks = dto.NewMarks;
            
            var mark = (await _uow.Repository<MarkEntry>()
                .ListAsync(x => x.ExamId == request.ExamId && x.StudentId == request.StudentId && x.SubjectId == request.SubjectId))
                .FirstOrDefault();

            if (mark != null)
            {
                // Audit Log
                var audit = new ResultAuditLog
                {
                    ExamId = request.ExamId,
                    StudentId = request.StudentId,
                    SubjectId = request.SubjectId,
                    OldMarks = mark.MarksObtained,
                    NewMarks = dto.NewMarks.Value,
                    ChangedByUserId = adminId,
                    Reason = $"Re-evaluation: {dto.AdminNotes}"
                };
                await _uow.Repository<ResultAuditLog>().AddAsync(audit);

                mark.MarksObtained = dto.NewMarks.Value;
                mark.IsLocked = false; // Temporarily unlock to update
                
                var gradingRules = await _uow.Repository<GradingRule>().ListAsync();
                var (grade, gp) = CalculateGrade(mark.MarksObtained, gradingRules);
                mark.Grade = grade;
                mark.GradePoint = gp;
                
                _uow.Repository<MarkEntry>().Update(mark);
            }
        }
        else
        {
            request.Status = ReEvaluationStatus.Rejected;
        }

        request.Notes += $" | Admin Note: {dto.AdminNotes}";
        _uow.Repository<ReEvaluationRequest>().Update(request);
        await _uow.SaveChangesAsync();
        
        if (dto.Approved)
        {
            await CalculateRankingAsync(request.ExamId);
        }
    }

    public async Task<ReEvaluationDashboardViewModel> GetReEvaluationDashboardAsync()
    {
        var requests = await _uow.Repository<ReEvaluationRequest>().ListAsync();
        
        var viewModel = new ReEvaluationDashboardViewModel
        {
            PendingRequests = requests.Where(x => x.Status == ReEvaluationStatus.Requested)
                .Select(MapToViewModel).ToList(),
            CompletedRequests = requests.Where(x => x.Status != ReEvaluationStatus.Requested)
                .Select(MapToViewModel).ToList()
        };

        return viewModel;
    }

    private ReEvaluationRequestViewModel MapToViewModel(ReEvaluationRequest r)
    {
        return new ReEvaluationRequestViewModel
        {
            Id = r.Id,
            StudentId = r.StudentId,
            StudentName = r.Student?.FullName ?? "Unknown",
            SubjectId = r.SubjectId,
            SubjectName = r.Subject?.IsReligionSubject == true && !string.IsNullOrEmpty(r.Subject?.ReligionType)
                ? ReligionHelper.GetReligionSubjectName(r.Subject.ReligionType)
                : r.Subject?.Name ?? "Unknown",
            ExamId = r.ExamId,
            ExamName = r.Exam?.Name ?? "Unknown",
            OldMarks = r.OldMarks,
            NewMarks = r.NewMarks,
            Status = r.Status,
            Notes = r.Notes,
            CreatedAt = r.CreatedAt
        };
    }

    public async Task<ResultSummaryDto> GetClassPerformanceAsync(int examId, int classId)
    {
        var results = await _uow.Repository<StudentExamResult>()
            .ListAsync(x => x.ExamId == examId && x.Student.ClassId == classId);

        var dto = new ResultSummaryDto
        {
            ExamId = examId,
            ClassId = classId,
            TotalStudents = results.Count,
            PassedStudents = results.Count(x => x.IsPassed),
            FailedStudents = results.Count(x => !x.IsPassed),
            ClassAverageGpa = results.Any() ? results.Average(x => x.Gpa) : 0,
            TopPerformers = results.OrderBy(x => x.Position).Take(5).Select(r => new StudentExamResultDto
            {
                ExamId = r.ExamId,
                ExamName = r.Exam?.Name ?? "",
                Gpa = r.Gpa,
                TotalMarks = r.TotalMarks,
                Position = r.Position
            }).ToList()
        };

        return dto;
    }

    public async Task<IEnumerable<GradingRuleUpsertDto>> GetGradingRulesAsync()
    {
        var rules = await _uow.Repository<GradingRule>().ListAsync();
        return rules.Select(x => new GradingRuleUpsertDto
        {
            Id = x.Id,
            Grade = x.Grade,
            MinMarks = x.MinMarks,
            MaxMarks = x.MaxMarks,
            GradePoint = x.GradePoint
        });
    }

    public async Task UpsertGradingRuleAsync(GradingRuleUpsertDto dto)
    {
        if (dto.Id.HasValue)
        {
            var rule = await _uow.Repository<GradingRule>().GetByIdAsync(dto.Id.Value);
            if (rule != null)
            {
                rule.Grade = dto.Grade;
                rule.MinMarks = dto.MinMarks;
                rule.MaxMarks = dto.MaxMarks;
                rule.GradePoint = dto.GradePoint;
                _uow.Repository<GradingRule>().Update(rule);
            }
        }
        else
        {
            var rule = new GradingRule
            {
                Grade = dto.Grade,
                MinMarks = dto.MinMarks,
                MaxMarks = dto.MaxMarks,
                GradePoint = dto.GradePoint
            };
            await _uow.Repository<GradingRule>().AddAsync(rule);
        }
        await _uow.SaveChangesAsync();
    }

    private (string Grade, decimal GP) CalculateGrade(decimal marks, IEnumerable<GradingRule> rules)
    {
        var rule = rules.FirstOrDefault(x => marks >= x.MinMarks && marks <= x.MaxMarks);
        return rule != null ? (rule.Grade, rule.GradePoint) : ("F", 0);
    }
}
