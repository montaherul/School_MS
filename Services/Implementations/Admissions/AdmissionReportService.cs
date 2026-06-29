using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public class AdmissionReportService : IAdmissionReportService
{
    private readonly IAdmissionRepository _admissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdmissionReportService> _logger;

    public AdmissionReportService(
        IAdmissionRepository admissionRepository,
        IUnitOfWork unitOfWork,
        ILogger<AdmissionReportService> logger)
    {
        _admissionRepository = admissionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AdmissionRegisterReportDto> GetRegisterReportAsync(AdmissionReportRequest request, CancellationToken ct = default)
    {
        var query = _admissionRepository.Query().AsNoTracking().Where(a => !a.IsDeleted);

        if (request.DateFrom.HasValue)
            query = query.Where(a => a.CreatedAt >= request.DateFrom.Value);
        if (request.DateTo.HasValue)
            query = query.Where(a => a.CreatedAt <= request.DateTo.Value.AddDays(1));
        if (request.ClassId.HasValue && request.ClassId.Value > 0)
            query = query.Where(a => a.AppliedClassId == request.ClassId.Value);
        if (request.Status.HasValue)
            query = query.Where(a => a.Status == (AdmissionStatus)request.Status.Value);
        if (!string.IsNullOrEmpty(request.Gender))
            query = query.Where(a => a.Gender == request.Gender);
        if (!string.IsNullOrEmpty(request.Religion))
            query = query.Where(a => a.Religion == request.Religion);
        if (!string.IsNullOrEmpty(request.District))
            query = query.Where(a => a.PresentDistrict == request.District || a.PermanentDistrict == request.District);

        var apps = await query.OrderByDescending(a => a.CreatedAt).ToListAsync(ct);

        var rows = apps.Select((a, i) => new AdmissionRegisterRow
        {
            SerialNo = i + 1,
            ApplicationNo = a.ApplicationNo,
            ApplicantName = a.ApplicantName,
            NameBangla = a.ApplicantNameBangla,
            DateOfBirth = a.DateOfBirth,
            Gender = a.Gender,
            FatherName = a.FatherName,
            MotherName = a.MotherName,
            Mobile = a.ApplicantMobileNumber ?? string.Empty,
            Religion = a.Religion,
            AppliedClass = a.AppliedClassId.ToString(),
            Status = a.Status.ToString(),
            SubmittedAt = a.CreatedAt
        }).ToList();

        return new AdmissionRegisterReportDto { Title = "Admission Register", Rows = rows };
    }

    public async Task<byte[]> ExportRegisterToExcelAsync(AdmissionReportRequest request, CancellationToken ct = default)
    {
        var report = await GetRegisterReportAsync(request, ct);
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var ws = workbook.Worksheets.Add("Admission Register");

        ws.Cell(1, 1).Value = "#";
        ws.Cell(1, 2).Value = "App No";
        ws.Cell(1, 3).Value = "Applicant Name";
        ws.Cell(1, 4).Value = "Name (Bangla)";
        ws.Cell(1, 5).Value = "DOB";
        ws.Cell(1, 6).Value = "Gender";
        ws.Cell(1, 7).Value = "Father";
        ws.Cell(1, 8).Value = "Mother";
        ws.Cell(1, 9).Value = "Mobile";
        ws.Cell(1, 10).Value = "Religion";
        ws.Cell(1, 11).Value = "Class";
        ws.Cell(1, 12).Value = "Status";
        ws.Cell(1, 13).Value = "Submitted";

        var header = ws.Range(1, 1, 1, 13);
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromArgb(15, 118, 110);
        header.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;

        int row = 2;
        foreach (var r in report.Rows)
        {
            ws.Cell(row, 1).Value = r.SerialNo;
            ws.Cell(row, 2).Value = r.ApplicationNo;
            ws.Cell(row, 3).Value = r.ApplicantName;
            ws.Cell(row, 4).Value = r.NameBangla ?? string.Empty;
            ws.Cell(row, 5).Value = r.DateOfBirth.ToString("dd-MMM-yyyy");
            ws.Cell(row, 6).Value = r.Gender;
            ws.Cell(row, 7).Value = r.FatherName;
            ws.Cell(row, 8).Value = r.MotherName;
            ws.Cell(row, 9).Value = r.Mobile;
            ws.Cell(row, 10).Value = r.Religion;
            ws.Cell(row, 11).Value = r.AppliedClass;
            ws.Cell(row, 12).Value = r.Status;
            ws.Cell(row, 13).Value = r.SubmittedAt.ToString("dd-MMM-yyyy");
            row++;
        }

        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> ExportRegisterToPdfAsync(AdmissionReportRequest request, CancellationToken ct = default)
    {
        var report = await GetRegisterReportAsync(request, ct);
        _logger.LogInformation("PDF export requested for {Count} rows", report.TotalRecords);
        return Array.Empty<byte>();
    }

    public async Task<ConversionFunnelDto> GetConversionFunnelAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        var query = _admissionRepository.Query().AsNoTracking().Where(a => !a.IsDeleted);

        if (dateFrom.HasValue) query = query.Where(a => a.CreatedAt >= dateFrom.Value);
        if (dateTo.HasValue) query = query.Where(a => a.CreatedAt <= dateTo.Value.AddDays(1));

        var all = await query.ToListAsync(ct);

        return new ConversionFunnelDto
        {
            TotalApplications = all.Count,
            DocumentVerified = all.Count(a => a.AllDocumentsVerified),
            InterviewCompleted = all.Count(a => a.Status != AdmissionStatus.Pending),
            FeePaid = all.Count(a => a.AdmissionFeePaid),
            Approved = all.Count(a => a.Status == AdmissionStatus.Approved),
            Converted = all.Count(a => a.Status == AdmissionStatus.Converted),
            ConversionRate = all.Count > 0 ? (double)all.Count(a => a.Status == AdmissionStatus.Converted) / all.Count * 100 : 0
        };
    }

    public async Task<CollectionReportDto> GetCollectionReportAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        var payments = await _unitOfWork.Repository<Payment>().Query().AsNoTracking()
            .Where(p => !p.IsDeleted && p.Remarks != null && p.Remarks.StartsWith("ADM-"))
            .ToListAsync(ct);

        if (dateFrom.HasValue) payments = payments.Where(p => p.PaidAt >= dateFrom.Value).ToList();
        if (dateTo.HasValue) payments = payments.Where(p => p.PaidAt <= dateTo.Value.AddDays(1)).ToList();

        return new CollectionReportDto
        {
            TotalCollected = payments.Sum(p => p.Amount),
            TotalPayments = payments.Count,
            Details = payments.Select(p => new CollectionRow
            {
                Date = p.PaidAt,
                ApplicationNo = p.Remarks?.Replace("ADM-", "") ?? "",
                ApplicantName = "",
                Amount = p.Amount,
                PaymentMethod = p.Method.ToString()
            }).ToList()
        };
    }
}
