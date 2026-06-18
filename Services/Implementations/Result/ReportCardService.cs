using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ReportCardService : IReportCardService
{
    private readonly IUnitOfWork _uow;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IStudentExamResultRepository _examResultRepository;
    private readonly IMarkEntryRepository _markEntryRepository;
    private readonly ISchoolSettingRepository _schoolSettingRepository;
    private readonly IStudentSubjectFilterService _subjectFilter;


    public ReportCardService(
        IUnitOfWork uow,
        IPdfGenerator pdfGenerator,
        IStudentExamResultRepository examResultRepository,
        IMarkEntryRepository markEntryRepository,
        ISchoolSettingRepository schoolSettingRepository,
        IStudentSubjectFilterService subjectFilter
        )
    {
        _uow = uow;
        _pdfGenerator = pdfGenerator;
        _examResultRepository = examResultRepository;
        _markEntryRepository = markEntryRepository;
        _schoolSettingRepository = schoolSettingRepository;
        _subjectFilter = subjectFilter;
    }

    public async Task<bool> IsResultBlockedForStudentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        if (await IsResultBlockedAsync(cancellationToken))
            return await HasFeeDueAsync(studentId, cancellationToken);
        return false;
    }

    public async Task<byte[]?> GenerateReportCardPdfAsync(int examId, int studentId, CancellationToken ct = default)
    {
        if (await IsResultBlockedForStudentAsync(studentId, ct))
            return null;

        var result = await _examResultRepository.Query()
            .Include(r => r.Student)
            .Include(r => r.Exam)
            .FirstOrDefaultAsync(r => r.ExamId == examId && r.StudentId == studentId && !r.IsDeleted
                && (r.Status == ResultWorkflowStatus.Published || r.Status == ResultWorkflowStatus.Locked), ct);

        if (result == null) return null;

        // Get the full student entity for subject filtering
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking()
            .Where(s => s.Id == studentId)
            .FirstOrDefaultAsync(ct);

        // Get valid subject IDs for this student based on curriculum
        var validSubjectIds = student != null
            ? await _subjectFilter.GetValidSubjectIdsForStudentAsync(student, ct)
            : new HashSet<int>();

        var marks = await _markEntryRepository.Query()
            .Include(m => m.Subject)
            .Where(m => m.ExamId == examId && m.StudentId == studentId && !m.IsDeleted)
            .ToListAsync(ct);

        // Filter marks to only show valid subjects for this student
        if (validSubjectIds.Count > 0)
            marks = marks.Where(m => validSubjectIds.Contains(m.SubjectId)).ToList();

        var school = await _schoolSettingRepository.Query().FirstOrDefaultAsync(ct);
 
        return _pdfGenerator.GenerateSchoolReportCard(result, marks, school);
    }

    private async Task<bool> HasFeeDueAsync(int studentId, CancellationToken cancellationToken)
    {
        return await _uow.Repository<FeeInvoice>().AnyAsync(
            x => x.StudentId == studentId && !x.IsDeleted && x.Status != PaymentStatus.Paid && x.Status != PaymentStatus.Waived,
            cancellationToken);
    }

    private async Task<bool> IsResultBlockedAsync(CancellationToken cancellationToken)
    {
        var setting = await _schoolSettingRepository.Query().FirstOrDefaultAsync(cancellationToken);
        if (setting == null) return false;
        return !setting.AllowResultWithDue;
    }

    private string GenerateReportCardHash(int examId, int studentId, decimal totalMarks, string grade, decimal gpa)
    {
        var raw = $"{examId}|{studentId}|{totalMarks}|{grade}|{gpa}|SchoolManagementSystem-Secret-2026";
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes).ToLowerInvariant()[..16];
    }
}

