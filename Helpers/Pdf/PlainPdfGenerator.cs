using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using SchoolManagementSystem.Helpers;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Models.ViewModels.Student;
using Microsoft.AspNetCore.Hosting;

namespace SchoolManagementSystem.Helpers.Pdf;

public class PlainPdfGenerator : IPdfGenerator
{
    private readonly IWebHostEnvironment _env;
    private readonly IViewRendererService _viewRenderer;
    private readonly PlaywrightPdfEngine _playwright;

    private const float MM = 2.83465f;
    private const float CARD_W = 85.6f * MM;
    private const float CARD_H = 53.98f * MM;

    private static readonly Color Primary = new DeviceRgb(0x1B, 0x4D, 0x8C);
    private static readonly Color Gold = new DeviceRgb(0xC5, 0xA5, 0x5A);
    private static readonly Color DarkText = new DeviceRgb(0x1A, 0x1A, 0x2E);
    private static readonly Color MutedText = new DeviceRgb(0x6B, 0x72, 0x80);
    private static readonly Color GoldLight = new DeviceRgb(0xE8, 0xD5, 0xA3);
    private static readonly Color BorderColor = new DeviceRgb(0xD1, 0xD5, 0xDB);
    private static readonly Color White = DeviceRgb.WHITE;

    private PdfFont _bold = null!;
    private PdfFont _normal = null!;

    public PlainPdfGenerator(IWebHostEnvironment env, IViewRendererService viewRenderer, PlaywrightPdfEngine playwright)
    {
        _env = env;
        _viewRenderer = viewRenderer;
        _playwright = playwright;
    }

    // ─────────────────────────────────────────────────────────────
    //  REPORT CARD  (unchanged)
    // ─────────────────────────────────────────────────────────────
    public byte[] GenerateSchoolReportCard(
        StudentExamResult result,
        List<MarkEntry> marks, SchoolSetting school)
    {
        using var stream = new MemoryStream();
        using var writer = new PdfWriter(stream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf, PageSize.A4);
        document.SetMargins(20, 20, 20, 20);

        EnsureFonts();

        document.Add(new Paragraph($"{school.SchoolName}")
            .SetFont(_bold).SetFontSize(22)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontColor(ColorConstants.BLUE));

        document.Add(new Paragraph("Academic Report Card")
            .SetFont(_bold).SetFontSize(16)
            .SetTextAlignment(TextAlignment.CENTER));

        document.Add(new Paragraph("\n"));

        var infoTable = new Table(UnitValue.CreatePercentArray(new float[] { 2, 4 })).UseAllAvailableWidth();
        infoTable.AddCell(GetLabelCell("Student Name")); infoTable.AddCell(GetValueCell(result.Student.FullName));
        infoTable.AddCell(GetLabelCell("Student ID")); infoTable.AddCell(GetValueCell(result.Student.StudentNo));
        infoTable.AddCell(GetLabelCell("Exam")); infoTable.AddCell(GetValueCell(result.Exam.Name));
        infoTable.AddCell(GetLabelCell("GPA")); infoTable.AddCell(GetValueCell(result.Gpa.ToString("F2")));
        infoTable.AddCell(GetLabelCell("Total Marks")); infoTable.AddCell(GetValueCell(result.TotalMarks.ToString("F2")));
        infoTable.AddCell(GetLabelCell("Position")); infoTable.AddCell(GetValueCell(result.Position.ToString()));
        document.Add(infoTable);

        document.Add(new Paragraph("\n"));

        var allComponentCodes = marks
            .SelectMany(m => SchoolManagementSystem.Services.Implementations.Result.ComponentFieldMapper.FromEntity(m).Keys)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c)
            .ToList();

        var colWidths = new List<float> { 4 };
        foreach (var _ in allComponentCodes) colWidths.Add(2);
        colWidths.AddRange(new float[] { 2, 2, 2 });

        var table = new Table(UnitValue.CreatePercentArray(colWidths.ToArray())).UseAllAvailableWidth();
        table.AddHeaderCell(GetHeaderCell("Subject"));
        foreach (var code in allComponentCodes)
            table.AddHeaderCell(GetHeaderCell(code));
        table.AddHeaderCell(GetHeaderCell("Total"));
        table.AddHeaderCell(GetHeaderCell("Grade"));
        table.AddHeaderCell(GetHeaderCell("GP"));
        table.AddHeaderCell(GetHeaderCell("Status"));

        foreach (var mark in marks)
        {
            var componentMarks = SchoolManagementSystem.Services.Implementations.Result.ComponentFieldMapper.FromEntity(mark);
            var fullMarks = mark.Subject?.DefaultFullMarks ?? 100;
            var passMarks = mark.Subject?.DefaultPassMarks ?? 33;
            table.AddCell(GetBodyCell(mark.Subject?.Name ?? ""));
            foreach (var code in allComponentCodes)
            {
                var val = componentMarks[code];
                table.AddCell(GetBodyCell(val.HasValue ? val.Value.ToString("F0") : "—"));
            }
            table.AddCell(GetBodyCell($"{mark.MarksObtained} / {fullMarks}"));
            table.AddCell(GetBodyCell(mark.Grade ?? "N/A"));
            table.AddCell(GetBodyCell((mark.GradePoint ?? 0).ToString("F2")));
            table.AddCell(GetBodyCell(mark.MarksObtained >= passMarks ? "PASSED" : "FAILED"));
        }
        document.Add(table);

        document.Add(new Paragraph("\n"));

        var finalBox = new Table(1).UseAllAvailableWidth();
        string status = result.Gpa > 0 ? "PROMOTED" : "FAILED";
        finalBox.AddCell(new Cell()
            .Add(new Paragraph($"Final GPA: {result.Gpa:F2} | Status: {status}"))
            .SetFont(_bold).SetFontSize(14)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
            .SetPadding(10));
        document.Add(finalBox);

        document.Add(new Paragraph("\n\n"));

        var signTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 })).UseAllAvailableWidth();
        signTable.AddCell(new Cell()
            .Add(new Paragraph("Class Teacher"))
            .SetTextAlignment(TextAlignment.CENTER)
            .SetBorderTop(new SolidBorder(1))
            .SetBorderBottom(Border.NO_BORDER).SetBorderLeft(Border.NO_BORDER).SetBorderRight(Border.NO_BORDER)
            .SetPaddingTop(20));
        signTable.AddCell(new Cell()
            .Add(new Paragraph("Principal"))
            .SetTextAlignment(TextAlignment.CENTER)
            .SetBorderTop(new SolidBorder(1))
            .SetBorderBottom(Border.NO_BORDER).SetBorderLeft(Border.NO_BORDER).SetBorderRight(Border.NO_BORDER)
            .SetPaddingTop(20));
        document.Add(signTable);

        document.Close();
        return stream.ToArray();
    }

    // ─────────────────────────────────────────────────────────────
    //  STUDENT ID CARD — HTML + wkhtmltopdf
    // ─────────────────────────────────────────────────────────────
    public byte[] GenerateStudentIdCardPdf(IdCardPrintViewModel model)
    {
        var baseUrl = $"file:///{_env.WebRootPath.Replace('\\', '/').TrimEnd('/')}/";
        model.BaseUrl = baseUrl;

        var rawHtml = Task.Run(() => _viewRenderer.RenderToStringAsync("~/Views/Student/PrintIdCard.cshtml", model))
            .GetAwaiter().GetResult();

        var html = PrepareHtmlForPdf(rawHtml);
        SaveDebugHtml(html, "student-card-debug.html");
        return GenerateIdCardPdf(html, model.IsBulk);
    }

    // ─────────────────────────────────────────────────────────────
    //  EMPLOYEE ID CARD — HTML + wkhtmltopdf
    // ─────────────────────────────────────────────────────────────
    public byte[] GenerateEmployeeIdCardPdf(EmployeeIdCardPrintViewModel model)
    {
        var baseUrl = $"file:///{_env.WebRootPath.Replace('\\', '/').TrimEnd('/')}/";
        model.BaseUrl = baseUrl;

        var rawHtml = Task.Run(() => _viewRenderer.RenderToStringAsync("~/Views/Employee/PrintIdCard.cshtml", model))
            .GetAwaiter().GetResult();

        var html = PrepareHtmlForPdf(rawHtml);
        SaveDebugHtml(html, "employee-card-debug.html");
        return GenerateIdCardPdf(html, model.IsBulk);
    }

    // ─────────────────────────────────────────────────────────────
    //  ID CARD PDF — Playwright Chromium (modern CSS support)
    //  Single:  Exact landscape CR80 PVC page
    //  Bulk:    A4 Landscape, six landscape card pairs per page
    // ─────────────────────────────────────────────────────────────

    private byte[] GenerateIdCardPdf(string html, bool isBulk)
    {
        return _playwright.Convert(html, isBulk);
    }

    private string PrepareHtmlForPdf(string rawHtml)
    {
        var cssPath = System.IO.Path.Combine(_env.WebRootPath, "css", "idcard-print.css");
        var css = System.IO.File.ReadAllText(cssPath);

        // 1. Inline the CSS
        var linkPattern = @"<link[^>]*href=""[^""]*idcard-print\.css""[^>]*/?>"; 
        var inlineStyle = $"<style>\n{css}\n</style>";
        var html = System.Text.RegularExpressions.Regex.Replace(rawHtml, linkPattern, inlineStyle);

        // 2. Convert ALL relative paths to absolute file:// paths
        var wwwRoot = _env.WebRootPath.Replace('\\', '/').TrimEnd('/');
        
        // Fix src="/..." attributes
        html = System.Text.RegularExpressions.Regex.Replace(html, @"src=""(/[^""]+)""", m =>
        {
            var path = m.Groups[1].Value;
            return $"src=\"file:///{wwwRoot}{path}\"";
        });
        
        // Fix href="/..." attributes (but not # or http links)
        html = System.Text.RegularExpressions.Regex.Replace(html, @"href=""(/[^""#http][^""]*)""", m =>
        {
            var path = m.Groups[1].Value;
            return $"href=\"file:///{wwwRoot}{path}\"";
        });

        // 3. Set base tag for Chromium to resolve relative paths
        var baseTag = $"<base href=\"file:///{wwwRoot}/\">";
        html = System.Text.RegularExpressions.Regex.Replace(html, @"<base[^>]*/?>", baseTag);

        return html;
    }

    // ─────────────────────────────────────────────────────────────
    //  TRANSCRIPT
    // ─────────────────────────────────────────────────────────────
    public byte[] GenerateTranscript(SchoolManagementSystem.Models.DTOs.Result.StudentTranscriptDto transcript)
    {
        using var stream = new MemoryStream();
        using var writer = new PdfWriter(stream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf, PageSize.A4.Rotate());
        document.SetMargins(20, 20, 20, 20);

        EnsureFonts();

        document.Add(new Paragraph(transcript.SchoolName)
            .SetFont(_bold).SetFontSize(22)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontColor(ColorConstants.BLUE));

        document.Add(new Paragraph("Academic Transcript")
            .SetFont(_bold).SetFontSize(16)
            .SetTextAlignment(TextAlignment.CENTER));

        document.Add(new Paragraph($"Academic Year: {transcript.AcademicYear}")
            .SetFont(_normal).SetFontSize(12)
            .SetTextAlignment(TextAlignment.CENTER));

        document.Add(new Paragraph("\n"));

        var infoTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3, 1, 3 })).UseAllAvailableWidth();
        infoTable.AddCell(GetLabelCell("Student Name")); infoTable.AddCell(GetValueCell(transcript.StudentName));
        infoTable.AddCell(GetLabelCell("Roll No")); infoTable.AddCell(GetValueCell(transcript.RollNumber.ToString()));
        infoTable.AddCell(GetLabelCell("Father's Name")); infoTable.AddCell(GetValueCell(transcript.FatherName));
        infoTable.AddCell(GetLabelCell("Reg. No")); infoTable.AddCell(GetValueCell(transcript.RegistrationNumber));
        infoTable.AddCell(GetLabelCell("Mother's Name")); infoTable.AddCell(GetValueCell(transcript.MotherName));
        infoTable.AddCell(GetLabelCell("DOB")); infoTable.AddCell(GetValueCell(transcript.DateOfBirth.ToString("dd-MM-yyyy")));
        document.Add(infoTable);

        document.Add(new Paragraph("\n"));

        foreach (var exam in transcript.ExamResults)
        {
            document.Add(new Paragraph(exam.ExamName)
                .SetFont(_bold).SetFontSize(13)
                .SetFontColor(ColorConstants.DARK_GRAY));

            var examTable = new Table(UnitValue.CreatePercentArray(new float[] { 4, 2, 2, 2, 2 })).UseAllAvailableWidth();
            examTable.AddHeaderCell(GetHeaderCell("Subject"));
            examTable.AddHeaderCell(GetHeaderCell("Marks"));
            examTable.AddHeaderCell(GetHeaderCell("Grade"));
            examTable.AddHeaderCell(GetHeaderCell("GPA"));
            examTable.AddHeaderCell(GetHeaderCell("Status"));

            foreach (var subj in exam.Subjects)
            {
                examTable.AddCell(GetBodyCell(subj.SubjectName));
                examTable.AddCell(GetBodyCell($"{subj.MarksObtained} / {subj.FullMarks}"));
                examTable.AddCell(GetBodyCell(subj.Grade));
                examTable.AddCell(GetBodyCell(subj.GradePoint.ToString("F2")));
                examTable.AddCell(GetBodyCell(subj.IsPassed ? "PASS" : "FAIL"));
            }

            examTable.AddCell(GetHeaderCell("Total"));
            examTable.AddCell(GetHeaderCell($"{exam.TotalMarks} / {exam.TotalFullMarks}"));
            examTable.AddCell(GetHeaderCell(exam.Grade));
            examTable.AddCell(GetHeaderCell(exam.Gpa.ToString("F2")));
            examTable.AddCell(GetHeaderCell(exam.IsPassed ? "PASSED" : "FAILED"));

            document.Add(examTable);
            document.Add(new Paragraph("\n"));
        }

        document.Add(new Paragraph("Subject-Wise Summary (All Terms)")
            .SetFont(_bold).SetFontSize(13)
            .SetFontColor(ColorConstants.DARK_GRAY));

        var summaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 4, 2, 2, 2, 2 })).UseAllAvailableWidth();
        summaryTable.AddHeaderCell(GetHeaderCell("Subject"));
        summaryTable.AddHeaderCell(GetHeaderCell("Avg Marks"));
        summaryTable.AddHeaderCell(GetHeaderCell("Grade"));
        summaryTable.AddHeaderCell(GetHeaderCell("GPA"));
        summaryTable.AddHeaderCell(GetHeaderCell("Status"));

        foreach (var subj in transcript.SubjectWiseResults)
        {
            summaryTable.AddCell(GetBodyCell(subj.SubjectName));
            summaryTable.AddCell(GetBodyCell(subj.TotalMarks.ToString("F2")));
            summaryTable.AddCell(GetBodyCell(subj.Grade));
            summaryTable.AddCell(GetBodyCell(subj.GradePoint.ToString("F2")));
            summaryTable.AddCell(GetBodyCell(subj.IsPassed ? "PASS" : "FAIL"));
        }
        document.Add(summaryTable);

        document.Add(new Paragraph("\n"));

        var finalBox = new Table(1).UseAllAvailableWidth();
        string status = transcript.FinalGPA > 0 ? "PROMOTED" : "FAILED";
        finalBox.AddCell(new Cell()
            .Add(new Paragraph($"Final GPA: {transcript.FinalGPA:F2} | Grade: {transcript.FinalGrade} | Position: {transcript.MeritPosition} | Status: {status}"))
            .SetFont(_bold).SetFontSize(14)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
            .SetPadding(10));
        document.Add(finalBox);

        document.Add(new Paragraph("\n\n"));

        var signTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 })).UseAllAvailableWidth();
        signTable.AddCell(new Cell()
            .Add(new Paragraph("Class Teacher"))
            .SetTextAlignment(TextAlignment.CENTER)
            .SetBorderTop(new SolidBorder(1))
            .SetBorderBottom(Border.NO_BORDER).SetBorderLeft(Border.NO_BORDER).SetBorderRight(Border.NO_BORDER)
            .SetPaddingTop(20));
        signTable.AddCell(new Cell()
            .Add(new Paragraph("Principal"))
            .SetTextAlignment(TextAlignment.CENTER)
            .SetBorderTop(new SolidBorder(1))
            .SetBorderBottom(Border.NO_BORDER).SetBorderLeft(Border.NO_BORDER).SetBorderRight(Border.NO_BORDER)
            .SetPaddingTop(20));
        document.Add(signTable);

        document.Close();
        return stream.ToArray();
    }

    // ─────────────────────────────────────────────────────────────
    //  HTML → PDF via Playwright (admit cards, etc.)
    // ─────────────────────────────────────────────────────────────
    public byte[] GenerateFromHtml(string html)
    {
        return _playwright.Convert(html, false);
    }

    // ─────────────────────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────────────────────
    private void EnsureFonts()
    {
        _bold = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
        _normal = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);
    }

    // ── Report-card helpers ────────────────────────────────────────

    private Cell GetHeaderCell(string text) =>
        new Cell()
            .Add(new Paragraph(text).SetBold().SetFontColor(ColorConstants.WHITE))
            .SetBackgroundColor(ColorConstants.BLUE)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetPadding(5);

    private Cell GetBodyCell(string text) =>
        new Cell()
            .Add(new Paragraph(text))
            .SetTextAlignment(TextAlignment.CENTER)
            .SetPadding(5);

    private Cell GetLabelCell(string text) =>
        new Cell()
            .Add(new Paragraph(text).SetBold())
            .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
            .SetPadding(5);

    private Cell GetValueCell(string text) =>
        new Cell()
            .Add(new Paragraph(text))
            .SetPadding(5);

    private void SaveDebugHtml(string html, string fileName)
    {
        var debugDir = System.IO.Path.Combine(_env.WebRootPath, "debug");
        System.IO.Directory.CreateDirectory(debugDir);
        var debugPath = System.IO.Path.Combine(debugDir, fileName);
        System.IO.File.WriteAllText(debugPath, html);
    }
}
