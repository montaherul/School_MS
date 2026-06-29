using System.Text;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Services.Interfaces.Admissions;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public class AdmissionPdfReportService : IAdmissionPdfReportService
{
    private readonly IAdmissionReportService _admissionReportService;
    private readonly IPdfGenerator _pdfGenerator;

    public AdmissionPdfReportService(
        IAdmissionReportService admissionReportService,
        IPdfGenerator pdfGenerator)
    {
        _admissionReportService = admissionReportService;
        _pdfGenerator = pdfGenerator;
    }

    public async Task<byte[]> GenerateRegisterReportPdfAsync(
        DateTime? fromDate, DateTime? toDate, int? classId, int? statusId)
    {
        var request = new AdmissionReportRequest
        {
            DateFrom = fromDate,
            DateTo = toDate,
            ClassId = classId,
            Status = statusId
        };

        var report = await _admissionReportService.GetRegisterReportAsync(request);

        var sb = new StringBuilder();
        sb.AppendLine("""
            <!DOCTYPE html>
            <html>
            <head>
            <meta charset="utf-8">
            <style>
                body { font-family: 'Segoe UI', Arial, sans-serif; margin: 20px; color: #1a1a2e; }
                h1 { text-align: center; color: #1B4D8C; font-size: 20px; margin-bottom: 5px; }
                .subtitle { text-align: center; color: #6B7280; font-size: 12px; margin-bottom: 20px; }
                table { width: 100%; border-collapse: collapse; font-size: 10px; }
                th { background: #1B4D8C; color: #fff; padding: 6px 4px; text-align: center; font-weight: 600; }
                td { padding: 4px; border-bottom: 1px solid #E5E7EB; text-align: center; }
                tr:nth-child(even) { background: #F9FAFB; }
                .footer { text-align: center; color: #9CA3AF; font-size: 9px; margin-top: 15px; }
            </style>
            </head>
            <body>
            """);

        sb.AppendLine($"<h1>Admission Register</h1>");
        sb.AppendLine($"<p class=\"subtitle\">Generated: {DateTime.Now:dd-MMM-yyyy HH:mm}</p>");
        sb.AppendLine("<table>");
        sb.AppendLine("<thead><tr>");
        sb.AppendLine("<th>#</th><th>App No</th><th>Applicant Name</th><th>Name (Bangla)</th>");
        sb.AppendLine("<th>DOB</th><th>Gender</th><th>Father</th><th>Mother</th>");
        sb.AppendLine("<th>Mobile</th><th>Religion</th><th>Class</th><th>Status</th><th>Submitted</th>");
        sb.AppendLine("</tr></thead><tbody>");

        foreach (var row in report.Rows)
        {
            sb.AppendLine("<tr>");
            sb.AppendLine($"<td>{row.SerialNo}</td>");
            sb.AppendLine($"<td>{EscapeHtml(row.ApplicationNo)}</td>");
            sb.AppendLine($"<td>{EscapeHtml(row.ApplicantName)}</td>");
            sb.AppendLine($"<td>{EscapeHtml(row.NameBangla)}</td>");
            sb.AppendLine($"<td>{row.DateOfBirth:dd-MMM-yyyy}</td>");
            sb.AppendLine($"<td>{EscapeHtml(row.Gender)}</td>");
            sb.AppendLine($"<td>{EscapeHtml(row.FatherName)}</td>");
            sb.AppendLine($"<td>{EscapeHtml(row.MotherName)}</td>");
            sb.AppendLine($"<td>{EscapeHtml(row.Mobile)}</td>");
            sb.AppendLine($"<td>{EscapeHtml(row.Religion)}</td>");
            sb.AppendLine($"<td>{EscapeHtml(row.AppliedClass)}</td>");
            sb.AppendLine($"<td>{EscapeHtml(row.Status)}</td>");
            sb.AppendLine($"<td>{row.SubmittedAt:dd-MMM-yyyy}</td>");
            sb.AppendLine("</tr>");
        }

        sb.AppendLine("</tbody></table>");
        sb.AppendLine($"<p class=\"footer\">Total Records: {report.TotalRecords}</p>");
        sb.AppendLine("</body></html>");

        return _pdfGenerator.GenerateFromHtml(sb.ToString());
    }

    public async Task<byte[]> GenerateConversionFunnelPdfAsync(int? academicYearId)
    {
        var funnel = await _admissionReportService.GetConversionFunnelAsync(null, null);

        var maxVal = Math.Max(funnel.TotalApplications, 1);
        var barMaxWidth = 80;

        var sb = new StringBuilder();
        sb.AppendLine("""
            <!DOCTYPE html>
            <html>
            <head>
            <meta charset="utf-8">
            <style>
                body { font-family: 'Segoe UI', Arial, sans-serif; margin: 20px; color: #1a1a2e; }
                h1 { text-align: center; color: #1B4D8C; font-size: 20px; margin-bottom: 5px; }
                .subtitle { text-align: center; color: #6B7280; font-size: 12px; margin-bottom: 25px; }
                .funnel { display: flex; flex-direction: column; align-items: center; gap: 6px; }
                .bar-row { display: flex; align-items: center; justify-content: center; width: 100%; gap: 10px; }
                .bar-label { width: 140px; text-align: right; font-size: 11px; font-weight: 600; color: #374151; }
                .bar-track { height: 28px; background: #DBEAFE; border-radius: 4px; display: flex; align-items: center; justify-content: center; transition: width 0.3s; min-width: 40px; }
                .bar-value { font-size: 11px; font-weight: 700; color: #1E3A5F; }
                .bar-pct { width: 50px; text-align: left; font-size: 10px; color: #6B7280; }
                .rate-box { text-align: center; margin-top: 25px; padding: 15px; background: #F0FDF4; border: 1px solid #86EFAC; border-radius: 8px; }
                .rate-box .rate-value { font-size: 28px; font-weight: 700; color: #166534; }
                .rate-box .rate-label { font-size: 11px; color: #166534; }
                .footer { text-align: center; color: #9CA3AF; font-size: 9px; margin-top: 20px; }
            </style>
            </head>
            <body>
            """);

        sb.AppendLine("<h1>Admission Conversion Funnel</h1>");
        sb.AppendLine($"<p class=\"subtitle\">Generated: {DateTime.Now:dd-MMM-yyyy HH:mm}</p>");
        sb.AppendLine("<div class=\"funnel\">");

        var stages = new[]
        {
            ("Total Applications", funnel.TotalApplications, "#1B4D8C"),
            ("Document Verified", funnel.DocumentVerified, "#2563EB"),
            ("Interview Completed", funnel.InterviewCompleted, "#7C3AED"),
            ("Fee Paid", funnel.FeePaid, "#D97706"),
            ("Approved", funnel.Approved, "#059669"),
            ("Converted", funnel.Converted, "#166534"),
        };

        foreach (var (label, value, color) in stages)
        {
            var pct = maxVal > 0 ? (double)value / maxVal : 0;
            var width = Math.Max(20, pct * barMaxWidth);
            sb.AppendLine("<div class=\"bar-row\">");
            sb.AppendLine($"<div class=\"bar-label\">{EscapeHtml(label)}</div>");
            sb.AppendLine($"<div class=\"bar-track\" style=\"width:{width}%;background:{color}\"><span class=\"bar-value\">{value}</span></div>");
            var stagePct = maxVal > 0 ? (value * 100.0 / maxVal) : 0;
            sb.AppendLine($"<div class=\"bar-pct\">{stagePct:F1}%</div>");
            sb.AppendLine("</div>");
        }

        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"rate-box\">");
        sb.AppendLine($"<div class=\"rate-value\">{funnel.ConversionRate:F1}%</div>");
        sb.AppendLine("<div class=\"rate-label\">Overall Conversion Rate</div>");
        sb.AppendLine("</div>");
        sb.AppendLine("</body></html>");

        return _pdfGenerator.GenerateFromHtml(sb.ToString());
    }

    private static string EscapeHtml(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return System.Net.WebUtility.HtmlEncode(value);
    }
}
