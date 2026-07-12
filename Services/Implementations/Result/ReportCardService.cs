using System.IO.Compression;
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
    private readonly IViewRendererService _viewRenderer;


    public ReportCardService(
        IUnitOfWork uow,
        IPdfGenerator pdfGenerator,
        IStudentExamResultRepository examResultRepository,
        IMarkEntryRepository markEntryRepository,
        ISchoolSettingRepository schoolSettingRepository,
        IStudentSubjectFilterService subjectFilter,
        IViewRendererService viewRenderer
        )
    {
        _uow = uow;
        _pdfGenerator = pdfGenerator;
        _examResultRepository = examResultRepository;
        _markEntryRepository = markEntryRepository;
        _schoolSettingRepository = schoolSettingRepository;
        _subjectFilter = subjectFilter;
        _viewRenderer = viewRenderer;
    }

    public async Task<bool> IsResultBlockedForStudentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        if (await IsResultBlockedAsync(cancellationToken))
            return await HasFeeDueAsync(studentId, cancellationToken);
        return false;
    }

    public async Task<byte[]?> GenerateReportCardPdfAsync(int examId, int studentId, CancellationToken ct = default)
        => await GenerateReportCardPdfAsync(examId, studentId, false, ct);

    public async Task<byte[]?> GenerateReportCardPdfAsync(int examId, int studentId, bool isAdmin, CancellationToken ct = default)
    {
        if (await IsResultBlockedForStudentAsync(studentId, ct))
            return null;

        var query = _examResultRepository.QueryNoTracking()
            .Include(r => r.Student)
            .Include(r => r.Exam)
            .Where(r => r.ExamId == examId && r.StudentId == studentId && !r.IsDeleted);

        if (!isAdmin)
            query = query.Where(r => r.Status == ResultWorkflowStatus.Published || r.Status == ResultWorkflowStatus.Locked);

        var result = await query.FirstOrDefaultAsync(ct);

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

        var marks = await _markEntryRepository.QueryNoTracking()
            .Include(m => m.Subject)
            .Where(m => m.ExamId == examId && m.StudentId == studentId && !m.IsDeleted)
            .ToListAsync(ct);

        // Filter marks to only show valid subjects for this student
        if (validSubjectIds.Count > 0)
            marks = marks.Where(m => validSubjectIds.Contains(m.SubjectId)).ToList();

        var school = await _schoolSettingRepository.Query().AsNoTracking().FirstOrDefaultAsync(ct);
 
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
        var setting = await _schoolSettingRepository.Query().AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        if (setting == null) return false;
        return !setting.AllowResultWithDue;
    }

    public async Task<int> GetReportCardCountAsync(int examId, int? classId, int? sectionId, CancellationToken ct = default)
    {
        var query = _examResultRepository.QueryNoTracking()
            .Where(r => r.ExamId == examId && !r.IsDeleted);

        if (classId.HasValue)
            query = query.Where(r => r.Student.ClassId == classId.Value);

        if (sectionId.HasValue && sectionId > 0)
            query = query.Where(r => r.Student.SectionId == sectionId.Value);

        return await query.Select(r => r.StudentId).Distinct().CountAsync(ct);
    }

    public async Task<byte[]> GenerateBulkReportCardsAsync(int examId, int? classId, int? sectionId, string format, CancellationToken ct = default)
    {
        var studentIds = await GetStudentIdsAsync(examId, classId, sectionId, ct);
        if (studentIds.Count == 0) return [];

        if (string.Equals(format, "zip", StringComparison.OrdinalIgnoreCase))
            return await GenerateZipAsync(examId, studentIds, ct);

        return await GenerateCombinedPdfAsync(examId, studentIds, ct);
    }

    public async Task<int> AddToPrintQueueAsync(int examId, int? classId, int? sectionId, int totalItems, string requestedBy, CancellationToken ct = default)
    {
        var item = new ReportCardPrintQueueItem
        {
            ExamId = examId,
            ClassId = classId,
            SectionId = sectionId,
            RequestedBy = requestedBy,
            RequestedAt = DateTime.UtcNow,
            Status = ReportCardPrintStatus.Pending,
            TotalItems = totalItems
        };

        await _uow.Repository<ReportCardPrintQueueItem>().AddAsync(item, ct);
        await _uow.SaveChangesAsync(ct);
        return item.Id;
    }

    private async Task<List<int>> GetStudentIdsAsync(int examId, int? classId, int? sectionId, CancellationToken ct)
    {
        var baseQuery = _examResultRepository.QueryNoTracking()
            .Where(r => r.ExamId == examId && !r.IsDeleted);

        if (classId.HasValue)
            baseQuery = baseQuery.Where(r => r.Student.ClassId == classId.Value);

        if (sectionId.HasValue && sectionId > 0)
            baseQuery = baseQuery.Where(r => r.Student.SectionId == sectionId.Value);

        return await baseQuery
            .Select(r => r.StudentId)
            .Distinct()
            .ToListAsync(ct);
    }

    private async Task<byte[]> GenerateCombinedPdfAsync(int examId, List<int> studentIds, CancellationToken ct)
    {
        var htmlParts = new List<string>(studentIds.Count);

        foreach (var studentId in studentIds)
        {
            ct.ThrowIfCancellationRequested();

            if (await IsResultBlockedForStudentAsync(studentId, ct))
                continue;

            var dto = await _examResultRepository.GetReportCardAsync(examId, studentId, ct);
            if (dto == null) continue;

            var html = await _viewRenderer.RenderToStringAsync(
                "~/Views/ReportCard/BangladeshFormat.cshtml", dto);

            htmlParts.Add(html);
        }

        if (htmlParts.Count == 0)
            return [];

        var combinedHtml = string.Join(
            "<div style=\"page-break-after: always;\"></div>",
            htmlParts);

        return _pdfGenerator.GenerateFromHtml(combinedHtml);
    }

    private async Task<byte[]> GenerateZipAsync(int examId, List<int> studentIds, CancellationToken ct)
    {
        using var ms = new MemoryStream();

        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
        {
            foreach (var studentId in studentIds)
            {
                ct.ThrowIfCancellationRequested();

                if (await IsResultBlockedForStudentAsync(studentId, ct))
                    continue;

                var pdfBytes = await GenerateReportCardPdfAsync(examId, studentId, true, ct);
                if (pdfBytes == null) continue;

                var entryName = $"ReportCard_Student_{studentId}_Exam_{examId}.pdf";
                var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
                await using var entryStream = entry.Open();
                await entryStream.WriteAsync(pdfBytes, ct);
            }
        }

        return ms.ToArray();
    }

    private string GenerateReportCardHash(int examId, int studentId, decimal totalMarks, string grade, decimal gpa)
    {
        var raw = $"{examId}|{studentId}|{totalMarks}|{grade}|{gpa}|SchoolManagementSystem-Secret-2026";
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes).ToLowerInvariant()[..16];
    }
}

