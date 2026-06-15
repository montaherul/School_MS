using DinkToPdf;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using SchoolManagementSystem.Helpers;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Models.ViewModels.Student;
using Microsoft.AspNetCore.Hosting;
using iText.IO.Image;

namespace SchoolManagementSystem.Helpers.Pdf;

public class PlainPdfGenerator : IPdfGenerator
{
    private readonly IWebHostEnvironment _env;
    private readonly IViewRendererService _viewRenderer;

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

    public PlainPdfGenerator(IWebHostEnvironment env, IViewRendererService viewRenderer)
    {
        _env = env;
        _viewRenderer = viewRenderer;
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

        var rawHtml = _viewRenderer.RenderToStringAsync("~/Views/Student/PrintIdCard.cshtml", model)
            .GetAwaiter().GetResult();

        var html = PrepareHtmlForPdf(rawHtml);
        var debugDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "IdCardDebug");
        System.IO.Directory.CreateDirectory(debugDir);
        System.IO.File.WriteAllText(System.IO.Path.Combine(debugDir, "student-card-debug.html"), html);
        return GenerateFromHtml(html);
    }

    // ─────────────────────────────────────────────────────────────
    //  EMPLOYEE ID CARD — HTML + wkhtmltopdf
    // ─────────────────────────────────────────────────────────────
    public byte[] GenerateEmployeeIdCardPdf(EmployeeIdCardPrintViewModel model)
    {
        var baseUrl = $"file:///{_env.WebRootPath.Replace('\\', '/').TrimEnd('/')}/";
        model.BaseUrl = baseUrl;

        var rawHtml = _viewRenderer.RenderToStringAsync("~/Views/Employee/PrintIdCard.cshtml", model)
            .GetAwaiter().GetResult();

        var html = PrepareHtmlForPdf(rawHtml);
        var debugDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "IdCardDebug");
        System.IO.Directory.CreateDirectory(debugDir);
        System.IO.File.WriteAllText(System.IO.Path.Combine(debugDir, "employee-card-debug.html"), html);
        return GenerateFromHtml(html);
    }

    private string PrepareHtmlForPdf(string rawHtml)
    {
        var cssPath = System.IO.Path.Combine(_env.WebRootPath, "css", "idcard-print.css");
        var css = System.IO.File.ReadAllText(cssPath);

        // 1. Replace external CSS link with inline <style>
        var linkPattern = "<link[^>]*href=\"/css/idcard-print\\.css\"[^>]*>";
        var inlineStyle = $"<style>\n{css}\n</style>";
        var html = System.Text.RegularExpressions.Regex.Replace(rawHtml, linkPattern, inlineStyle);

        // 2. Replace relative src paths (starting with /) with absolute file:// paths
        var wwwrootUrl = $"file:///{_env.WebRootPath.Replace('\\', '/').TrimEnd('/')}";
        html = System.Text.RegularExpressions.Regex.Replace(
            html,
            "(src|href)=\"(/)",
            $"$1=\"{wwwrootUrl}$2"
        );

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
    //  STUDENT FRONT — modern enterprise design
    // ─────────────────────────────────────────────────────────────
    private void DrawStudentFront(PdfDocument pdf, PdfPage page, StudentUpsertDto student,
        IdCardPrintViewModel model, string? photoPath, string? logoPath, string? signPath)
    {
        var canvas = new PdfCanvas(page);
        var ps = page.GetPageSize();
        float W = ps.GetWidth();
        float H = ps.GetHeight();

        float margin = 2f * MM;
        float bodyW = W - 2f * margin;

        // ── Section heights ──
        float headerH = 14f * MM;      // top
        float footerH = 7f * MM;       // bottom
        float bodyH = H - headerH - footerH;

        float headerBot = H - headerH; // bottom edge of header

        // ══════════════════════════════════════════════
        //  HEADER
        // ══════════════════════════════════════════════
        canvas.SetFillColor(Primary);
        canvas.Rectangle(0, headerBot, W, headerH);
        canvas.Fill();

        // — Logo (clipped circle) —
        float logoSize = 9f * MM;
        float logoX = margin;
        float logoY = headerBot + (headerH - logoSize) / 2f;

        canvas.SaveState();
        canvas.Circle(logoX + logoSize / 2f, logoY + logoSize / 2f, logoSize / 2f);
        canvas.Clip();
        canvas.EndPath();

        if (logoPath != null && File.Exists(logoPath))
        {
            try
            {
                var logoData = ImageDataFactory.Create(logoPath);
                var logoImg = new Image(logoData);
                logoImg.ScaleToFit(logoSize, logoSize);
                logoImg.SetFixedPosition(logoX, logoY);
                new Canvas(canvas, ps).Add(logoImg);
            }
            catch { DrawPlaceholderCircle(canvas, logoX, logoY, logoSize, White); }
        }
        else { DrawPlaceholderCircle(canvas, logoX, logoY, logoSize, White); }
        canvas.RestoreState();

        // — Academic year (top-right) —
        {
            float ayX = W - margin - 22f * MM;
            var c = new Canvas(canvas, new Rectangle(ayX, headerBot + 1f * MM, 22f * MM, 3f * MM));
            c.Add(new Paragraph(model.AcademicYear)
                .SetFont(_bold).SetFontSize(5f).SetFontColor(Gold)
                .SetTextAlignment(TextAlignment.RIGHT));
            c.Close();
        }

        // — School name —
        float tx = logoX + logoSize + 2f * MM;
        float tw = W - tx - margin;

        {
            var c = new Canvas(canvas, new Rectangle(tx, headerBot + 5.5f * MM, tw - 24f * MM, 4.5f * MM));
            c.Add(new Paragraph(model.SchoolNameEn)
                .SetFont(_bold).SetFontSize(7.5f).SetFontColor(White));
            c.Close();
        }

        // — EIIN & Website —
        {
            var c = new Canvas(canvas, new Rectangle(tx, headerBot + 2.8f * MM, tw, 3f * MM));
            c.Add(new Paragraph($"EIIN: {model.SchoolEIIN}  |  {model.SchoolWebsite}")
                .SetFont(_normal).SetFontSize(4.5f).SetFontColor(White).SetOpacity(0.85f));
            c.Close();
        }

        // — Motto —
        {
            var c = new Canvas(canvas, new Rectangle(tx, headerBot + 0.3f * MM, tw, 3f * MM));
            c.Add(new Paragraph(!string.IsNullOrEmpty(model.SchoolMotto) ? $"\"{model.SchoolMotto}\"" : "")
                .SetFont(_normal).SetFontSize(4.5f).SetFontColor(GoldLight).SetItalic());
            c.Close();
        }

        // ══════════════════════════════════════════════
        //  BODY
        // ══════════════════════════════════════════════
        float bodyTop = headerBot;

        // — Photo —
        float photoW = 20f * MM;
        float photoH = 25f * MM;
        float photoX = margin;
        float photoY = footerH + (bodyH - photoH) / 2f;

        // Photo frame background
        canvas.SetFillColor(new DeviceRgb(0xF3, 0xF4, 0xF6));
        canvas.RoundRectangle(photoX, photoY, photoW, photoH, 1.5f * MM);
        canvas.Fill();
        canvas.SetStrokeColor(Primary);
        canvas.SetLineWidth(0.6f);
        canvas.RoundRectangle(photoX, photoY, photoW, photoH, 1.5f * MM);
        canvas.Stroke();

        if (photoPath != null && File.Exists(photoPath))
        {
            try
            {
                var photoData = ImageDataFactory.Create(photoPath);
                var photoImg = new Image(photoData);
                photoImg.ScaleToFit(photoW - 2f * MM, photoH - 2f * MM);
                float imgX = photoX + (photoW - photoImg.GetImageScaledWidth()) / 2f;
                float imgY = photoY + (photoH - photoImg.GetImageScaledHeight()) / 2f;
                photoImg.SetFixedPosition(imgX, imgY);
                new Canvas(canvas, ps).Add(photoImg);
            }
            catch { }
        }

        // — Info fields —
        float infoX = photoX + photoW + 2.5f * MM;
        float infoW = W - infoX - margin;
        float labelW = 11f * MM;
        float lineH = 3.2f * MM;

        var infoLines = new (string Label, string Value)[]
        {
            ("Name", student.FullName),
            ("ID No", student.StudentNo ?? "---"),
            ("Roll", student.RollNumber.ToString("D3")),
            ("Class", !string.IsNullOrEmpty(student.ClassName) ? student.ClassName : $"Class {student.ClassId}"),
            ("Section", !string.IsNullOrEmpty(student.SectionName) ? student.SectionName : "---"),
        };

        // Add group only for class 9-10
        if (student.ClassId >= 9 && !string.IsNullOrEmpty(student.GroupName))
        {
            infoLines = [.. infoLines, ("Group", student.GroupName)];
        }

        infoLines = [.. infoLines,
            ("Blood", !string.IsNullOrEmpty(student.BloodGroup) ? student.BloodGroup : "N/A"),
            ("Gender", student.Gender),
        ];

        float startY = bodyTop - 0.8f * MM;

        for (int i = 0; i < infoLines.Length; i++)
        {
            float y = startY - i * lineH;
            bool isName = i == 0;

            {
                var c = new Canvas(canvas, new Rectangle(infoX, y - lineH, labelW, lineH));
                c.Add(new Paragraph(infoLines[i].Label)
                    .SetFont(_bold).SetFontSize(5f).SetFontColor(MutedText));
                c.Close();
            }
            {
                var c = new Canvas(canvas, new Rectangle(infoX + labelW, y - lineH, infoW - labelW, lineH));
                c.Add(new Paragraph(infoLines[i].Value)
                    .SetFont(_bold).SetFontSize(6f)
                    .SetFontColor(isName ? Primary : DarkText));
                c.Close();
            }
        }

        // — Divider line between body and footer —
        canvas.SetStrokeColor(Gold);
        canvas.SetLineWidth(0.5f);
        canvas.MoveTo(margin, footerH + 0.5f * MM);
        canvas.LineTo(W - margin, footerH + 0.5f * MM);
        canvas.Stroke();

        // ══════════════════════════════════════════════
        //  FOOTER
        // ══════════════════════════════════════════════
        canvas.SetFillColor(Primary);
        canvas.Rectangle(0, 0, W, footerH);
        canvas.Fill();

        // — Issue / Expiry (left) —
        {
            float x = margin;
            float y = 0.7f * MM;
            var c = new Canvas(canvas, new Rectangle(x, y, 22f * MM, footerH - 1.4f * MM));
            c.Add(new Paragraph($"Issued: {DateTime.Today:dd MMM yyyy}    Expires: {DateTime.Today.AddYears(1):dd MMM yyyy}")
                .SetFont(_normal).SetFontSize(4.5f).SetFontColor(White).SetOpacity(0.9f));
            c.Close();
        }

        // — Principal signature (right) —
        {
            float sigX = W - margin - 26f * MM;
            float sigY = 0.3f * MM;
            var c = new Canvas(canvas, new Rectangle(sigX, sigY, 26f * MM, footerH - 0.6f * MM));
            c.Add(new Paragraph(model.PrincipalName ?? "Principal")
                .SetFont(_normal).SetFontSize(5f).SetFontColor(White)
                .SetTextAlignment(TextAlignment.CENTER));
            c.Close();

            canvas.SetStrokeColor(Gold);
            canvas.SetLineWidth(0.4f);
            float lineX1 = sigX + 2f * MM;
            float lineX2 = sigX + 24f * MM;
            float lineY = sigY + 4.5f * MM;
            canvas.MoveTo(lineX1, lineY);
            canvas.LineTo(lineX2, lineY);
            canvas.Stroke();
        }

        // — School seal (far right) —
        if (!string.IsNullOrEmpty(model.SchoolSealPath) && File.Exists(model.SchoolSealPath))
        {
            try
            {
                float sealSize = 5f * MM;
                float sealX = W - margin - sealSize;
                float sealY = (footerH - sealSize) / 2f;
                var sealData = ImageDataFactory.Create(model.SchoolSealPath);
                var sealImg = new Image(sealData);
                sealImg.ScaleToFit(sealSize, sealSize);
                sealImg.SetFixedPosition(sealX, sealY);
                new Canvas(canvas, ps).Add(sealImg);
            }
            catch { }
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  STUDENT BACK — guardian, address, QR
    // ─────────────────────────────────────────────────────────────
    private void DrawStudentBack(PdfDocument pdf, PdfPage page, StudentUpsertDto student,
        IdCardPrintViewModel model, byte[] qrBytes)
    {
        var canvas = new PdfCanvas(page);
        var ps = page.GetPageSize();
        float W = ps.GetWidth();
        float H = ps.GetHeight();

        float margin = 2f * MM;

        float headerH = 9f * MM;
        float footerH = 6.5f * MM;
        float bodyH = H - headerH - footerH;
        float headerBot = H - headerH;

        // ══════════════════════════════════════════════
        //  HEADER
        // ══════════════════════════════════════════════
        canvas.SetFillColor(Primary);
        canvas.Rectangle(0, headerBot, W, headerH);
        canvas.Fill();

        {
            var c = new Canvas(canvas, new Rectangle(0, headerBot, W, headerH));
            c.Add(new Paragraph("IMPORTANT INFORMATION")
                .SetFont(_bold).SetFontSize(7f).SetFontColor(White)
                .SetTextAlignment(TextAlignment.CENTER));
            c.Close();
        }

        // ══════════════════════════════════════════════
        //  BODY — left: guardian & address, right: QR
        // ══════════════════════════════════════════════
        float qrSize = 13f * MM;
        float qrX = W - margin - qrSize;
        float qrY = headerBot - qrSize - 2f * MM;

        float infoW = qrX - margin - 1.5f * MM;
        float infoX = margin;
        float lineH = 2.8f * MM;
        float labelW = 10f * MM;
        float bodyY = headerBot - 1.2f * MM;

        var guardianMobile = student.GuardianMobileNumber ?? student.FatherOrGuardianMobileNo;
        var address = string.Join(", ",
            new[] { student.PresentVillage, student.PresentPostOffice, student.PresentThana, student.PresentDistrict }
                .Where(a => !string.IsNullOrEmpty(a)));

        // — Section: GUARDIAN —
        {
            var c = new Canvas(canvas, new Rectangle(infoX, bodyY - lineH, infoW, lineH));
            c.Add(new Paragraph("GUARDIAN INFORMATION")
                .SetFont(_bold).SetFontSize(5.5f).SetFontColor(Primary).SetCharacterSpacing(0.5f));
            c.Close();
            canvas.SetStrokeColor(Gold);
            canvas.SetLineWidth(0.3f);
            canvas.MoveTo(infoX, bodyY - 0.8f * MM);
            canvas.LineTo(infoX + infoW, bodyY - 0.8f * MM);
            canvas.Stroke();
        }
        bodyY -= 2f * MM;

        var backLines = new (string Label, string Value)[]
        {
            ("Father", student.FatherName),
            ("Mother", student.MotherName),
            ("Guardian", student.GuardianName ?? "---"),
            ("Phone", !string.IsNullOrEmpty(guardianMobile) ? guardianMobile : "N/A"),
        };

        foreach (var (label, value) in backLines)
        {
            {
                var c = new Canvas(canvas, new Rectangle(infoX, bodyY - lineH, labelW, lineH));
                c.Add(new Paragraph(label).SetFont(_bold).SetFontSize(5f).SetFontColor(MutedText));
                c.Close();
            }
            {
                var c = new Canvas(canvas, new Rectangle(infoX + labelW, bodyY - lineH, infoW - labelW, lineH));
                c.Add(new Paragraph(value).SetFont(_bold).SetFontSize(5.5f).SetFontColor(DarkText));
                c.Close();
            }
            bodyY -= lineH;
        }

        // — Section: ADDRESS —
        bodyY -= 0.5f * MM;
        {
            var c = new Canvas(canvas, new Rectangle(infoX, bodyY - lineH, infoW, lineH));
            c.Add(new Paragraph("ADDRESS")
                .SetFont(_bold).SetFontSize(5.5f).SetFontColor(Primary).SetCharacterSpacing(0.5f));
            c.Close();
            canvas.SetStrokeColor(Gold);
            canvas.SetLineWidth(0.3f);
            canvas.MoveTo(infoX, bodyY - 0.8f * MM);
            canvas.LineTo(infoX + infoW, bodyY - 0.8f * MM);
            canvas.Stroke();
        }
        bodyY -= 2f * MM;

        {
            var c = new Canvas(canvas, new Rectangle(infoX, bodyY - 2f * lineH, infoW, 2f * lineH));
            c.Add(new Paragraph(!string.IsNullOrEmpty(address) ? address : "N/A")
                .SetFont(_normal).SetFontSize(5f).SetFontColor(DarkText));
            c.Close();
        }

        // — QR Code (right side) —
        canvas.SetStrokeColor(Primary);
        canvas.SetLineWidth(0.5f);
        canvas.RoundRectangle(qrX, qrY, qrSize, qrSize, 1f * MM);
        canvas.Stroke();

        if (qrBytes != null)
        {
            try
            {
                var qrData = ImageDataFactory.Create(qrBytes);
                var qrImg = new Image(qrData);
                float qrPad = 1f * MM;
                qrImg.ScaleToFit(qrSize - 2f * qrPad, qrSize - 2f * qrPad);
                qrImg.SetFixedPosition(qrX + qrPad, qrY + qrPad);
                new Canvas(canvas, ps).Add(qrImg);
            }
            catch { }
        }

        {
            var c = new Canvas(canvas, new Rectangle(qrX, qrY - 2.5f * MM, qrSize, 2.5f * MM));
            c.Add(new Paragraph("Student ID").SetFont(_normal).SetFontSize(5f).SetFontColor(MutedText)
                .SetTextAlignment(TextAlignment.CENTER));
            c.Close();
        }

        // ══════════════════════════════════════════════
        //  FOOTER
        // ══════════════════════════════════════════════
        canvas.SetFillColor(Primary);
        canvas.Rectangle(0, 0, W, footerH);
        canvas.Fill();

        {
            var footerParts = new[]
            {
                $"{model.SchoolNameEn}",
                $"{model.SchoolPhone}  |  {model.SchoolEmail}",
                model.SchoolAddress,
                model.FooterText
            };
            var footerText = string.Join("\n", footerParts.Where(p => !string.IsNullOrEmpty(p)));

            var c = new Canvas(canvas, new Rectangle(margin, 0.3f * MM, W - 2f * margin, footerH - 0.6f * MM));
            c.Add(new Paragraph(footerText)
                .SetFont(_normal).SetFontSize(4.5f).SetFontColor(White).SetOpacity(0.9f)
                .SetTextAlignment(TextAlignment.CENTER));
            c.Close();
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  EMPLOYEE FRONT — modern enterprise design
    // ─────────────────────────────────────────────────────────────
    private void DrawEmployeeFront(PdfDocument pdf, PdfPage page, EmployeeDetailsDto employee,
        EmployeeIdCardPrintViewModel model, string? photoPath, string? logoPath, string? signPath)
    {
        var themeColor = GetPdfThemeColor(employee.Designation);
        var canvas = new PdfCanvas(page);
        var ps = page.GetPageSize();
        float W = ps.GetWidth();
        float H = ps.GetHeight();

        float margin = 2f * MM;
        float bodyW = W - 2f * margin;

        float headerH = 14f * MM;
        float footerH = 7f * MM;
        float bodyH = H - headerH - footerH;
        float headerBot = H - headerH;

        // ══════════════════════════════════════════════
        //  HEADER
        // ══════════════════════════════════════════════
        canvas.SetFillColor(themeColor);
        canvas.Rectangle(0, headerBot, W, headerH);
        canvas.Fill();

        // — Logo (clipped circle) —
        float logoSize = 9f * MM;
        float logoX = margin;
        float logoY = headerBot + (headerH - logoSize) / 2f;

        canvas.SaveState();
        canvas.Circle(logoX + logoSize / 2f, logoY + logoSize / 2f, logoSize / 2f);
        canvas.Clip();
        canvas.EndPath();

        if (logoPath != null && File.Exists(logoPath))
        {
            try
            {
                var logoData = ImageDataFactory.Create(logoPath);
                var logoImg = new Image(logoData);
                logoImg.ScaleToFit(logoSize, logoSize);
                logoImg.SetFixedPosition(logoX, logoY);
                new Canvas(canvas, ps).Add(logoImg);
            }
            catch { DrawPlaceholderCircle(canvas, logoX, logoY, logoSize, White); }
        }
        else { DrawPlaceholderCircle(canvas, logoX, logoY, logoSize, White); }
        canvas.RestoreState();

        // — Academic year (top-right) —
        {
            float ayX = W - margin - 22f * MM;
            var c = new Canvas(canvas, new Rectangle(ayX, headerBot + 1f * MM, 22f * MM, 3f * MM));
            c.Add(new Paragraph(model.AcademicYear)
                .SetFont(_bold).SetFontSize(5f).SetFontColor(Gold)
                .SetTextAlignment(TextAlignment.RIGHT));
            c.Close();
        }

        // — School name —
        float tx = logoX + logoSize + 2f * MM;
        float tw = W - tx - margin;

        {
            var c = new Canvas(canvas, new Rectangle(tx, headerBot + 5.5f * MM, tw - 24f * MM, 4.5f * MM));
            c.Add(new Paragraph(model.SchoolNameEn)
                .SetFont(_bold).SetFontSize(7.5f).SetFontColor(White));
            c.Close();
        }

        // — EIIN & Website —
        {
            var c = new Canvas(canvas, new Rectangle(tx, headerBot + 2.8f * MM, tw, 3f * MM));
            c.Add(new Paragraph($"EIIN: {model.SchoolEIIN}  |  {model.SchoolWebsite}")
                .SetFont(_normal).SetFontSize(4.5f).SetFontColor(White).SetOpacity(0.85f));
            c.Close();
        }

        // — Motto —
        {
            var c = new Canvas(canvas, new Rectangle(tx, headerBot + 0.3f * MM, tw, 3f * MM));
            c.Add(new Paragraph(!string.IsNullOrEmpty(model.SchoolMotto) ? $"\"{model.SchoolMotto}\"" : "")
                .SetFont(_normal).SetFontSize(4.5f).SetFontColor(GoldLight).SetItalic());
            c.Close();
        }

        // ══════════════════════════════════════════════
        //  BODY
        // ══════════════════════════════════════════════
        float bodyTop = headerBot;

        // — Photo —
        float photoW = 20f * MM;
        float photoH = 25f * MM;
        float photoX = margin;
        float photoY = footerH + (bodyH - photoH) / 2f;

        canvas.SetFillColor(new DeviceRgb(0xF3, 0xF4, 0xF6));
        canvas.RoundRectangle(photoX, photoY, photoW, photoH, 1.5f * MM);
        canvas.Fill();
        canvas.SetStrokeColor(themeColor);
        canvas.SetLineWidth(0.6f);
        canvas.RoundRectangle(photoX, photoY, photoW, photoH, 1.5f * MM);
        canvas.Stroke();

        if (photoPath != null && File.Exists(photoPath))
        {
            try
            {
                var photoData = ImageDataFactory.Create(photoPath);
                var photoImg = new Image(photoData);
                photoImg.ScaleToFit(photoW - 2f * MM, photoH - 2f * MM);
                float imgX = photoX + (photoW - photoImg.GetImageScaledWidth()) / 2f;
                float imgY = photoY + (photoH - photoImg.GetImageScaledHeight()) / 2f;
                photoImg.SetFixedPosition(imgX, imgY);
                new Canvas(canvas, ps).Add(photoImg);
            }
            catch { }
        }

        // — Info fields —
        float infoX = photoX + photoW + 2.5f * MM;
        float infoW = W - infoX - margin;
        float labelW = 11f * MM;
        float lineH = 3.2f * MM;

        var infoLines = new (string Label, string Value)[]
        {
            ("Name", employee.FullName),
            ("Code", employee.EmployeeCode),
            ("Designation", employee.Designation),
            ("Department", employee.Department),
            ("Joining", employee.JoiningDate.ToString("dd MMM yyyy")),
            ("Blood", !string.IsNullOrEmpty(employee.BloodGroup) ? employee.BloodGroup : "N/A"),
            ("Mobile", employee.Phone),
            ("Card No", employee.EmployeeCardNumber ?? "---"),
        };

        float startY = bodyTop - 0.8f * MM;

        for (int i = 0; i < infoLines.Length; i++)
        {
            float y = startY - i * lineH;
            bool isName = i == 0;

            {
                var c = new Canvas(canvas, new Rectangle(infoX, y - lineH, labelW, lineH));
                c.Add(new Paragraph(infoLines[i].Label)
                    .SetFont(_bold).SetFontSize(5f).SetFontColor(MutedText));
                c.Close();
            }
            {
                var c = new Canvas(canvas, new Rectangle(infoX + labelW, y - lineH, infoW - labelW, lineH));
                c.Add(new Paragraph(infoLines[i].Value)
                    .SetFont(_bold).SetFontSize(6f)
                    .SetFontColor(isName ? themeColor : DarkText));
                c.Close();
            }
        }

        // — Divider —
        canvas.SetStrokeColor(Gold);
        canvas.SetLineWidth(0.5f);
        canvas.MoveTo(margin, footerH + 0.5f * MM);
        canvas.LineTo(W - margin, footerH + 0.5f * MM);
        canvas.Stroke();

        // ══════════════════════════════════════════════
        //  FOOTER
        // ══════════════════════════════════════════════
        canvas.SetFillColor(themeColor);
        canvas.Rectangle(0, 0, W, footerH);
        canvas.Fill();

        // — Issue / Expiry —
        {
            float x = margin;
            float y = 0.7f * MM;
            var issueDate = employee.CardIssueDate ?? DateTime.Today;
            var expiryDate = employee.CardExpiryDate ?? DateTime.Today.AddYears(2);
            var c = new Canvas(canvas, new Rectangle(x, y, 22f * MM, footerH - 1.4f * MM));
            c.Add(new Paragraph($"Issued: {issueDate:dd MMM yyyy}    Expires: {expiryDate:dd MMM yyyy}")
                .SetFont(_normal).SetFontSize(4.5f).SetFontColor(White).SetOpacity(0.9f));
            c.Close();
        }

        // — Principal signature —
        {
            float sigX = W - margin - 26f * MM;
            float sigY = 0.3f * MM;
            var c = new Canvas(canvas, new Rectangle(sigX, sigY, 26f * MM, footerH - 0.6f * MM));
            c.Add(new Paragraph(model.PrincipalName ?? "Principal")
                .SetFont(_normal).SetFontSize(5f).SetFontColor(White)
                .SetTextAlignment(TextAlignment.CENTER));
            c.Close();

            canvas.SetStrokeColor(Gold);
            canvas.SetLineWidth(0.4f);
            float lineX1 = sigX + 2f * MM;
            float lineX2 = sigX + 24f * MM;
            float lineY = sigY + 4.5f * MM;
            canvas.MoveTo(lineX1, lineY);
            canvas.LineTo(lineX2, lineY);
            canvas.Stroke();
        }

        // — School seal —
        if (!string.IsNullOrEmpty(model.SchoolSealPath) && File.Exists(model.SchoolSealPath))
        {
            try
            {
                float sealSize = 5f * MM;
                float sealX = W - margin - sealSize;
                float sealY = (footerH - sealSize) / 2f;
                var sealData = ImageDataFactory.Create(model.SchoolSealPath);
                var sealImg = new Image(sealData);
                sealImg.ScaleToFit(sealSize, sealSize);
                sealImg.SetFixedPosition(sealX, sealY);
                new Canvas(canvas, ps).Add(sealImg);
            }
            catch { }
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  EMPLOYEE BACK — emergency, NID, QR
    // ─────────────────────────────────────────────────────────────
    private void DrawEmployeeBack(PdfDocument pdf, PdfPage page, EmployeeDetailsDto employee,
        EmployeeIdCardPrintViewModel model, byte[] qrBytes)
    {
        var themeColor = GetPdfThemeColor(employee.Designation);
        var canvas = new PdfCanvas(page);
        var ps = page.GetPageSize();
        float W = ps.GetWidth();
        float H = ps.GetHeight();

        float margin = 2f * MM;

        float headerH = 9f * MM;
        float footerH = 6.5f * MM;
        float bodyH = H - headerH - footerH;
        float headerBot = H - headerH;

        // ══════════════════════════════════════════════
        //  HEADER
        // ══════════════════════════════════════════════
        canvas.SetFillColor(themeColor);
        canvas.Rectangle(0, headerBot, W, headerH);
        canvas.Fill();

        {
            var c = new Canvas(canvas, new Rectangle(0, headerBot, W, headerH));
            c.Add(new Paragraph("EMPLOYEE INFORMATION")
                .SetFont(_bold).SetFontSize(7f).SetFontColor(White)
                .SetTextAlignment(TextAlignment.CENTER));
            c.Close();
        }

        // ══════════════════════════════════════════════
        //  BODY
        // ══════════════════════════════════════════════
        float qrSize = 13f * MM;
        float qrX = W - margin - qrSize;
        float qrY = headerBot - qrSize - 2f * MM;

        float infoW = qrX - margin - 1.5f * MM;
        float infoX = margin;
        float lineH = 2.8f * MM;
        float labelW = 10f * MM;
        float bodyY = headerBot - 1.2f * MM;

        // — Section: CONTACT & PERSONAL —
        {
            var c = new Canvas(canvas, new Rectangle(infoX, bodyY - lineH, infoW, lineH));
            c.Add(new Paragraph("CONTACT & PERSONAL")
                .SetFont(_bold).SetFontSize(5.5f).SetFontColor(themeColor).SetCharacterSpacing(0.5f));
            c.Close();
            canvas.SetStrokeColor(Gold);
            canvas.SetLineWidth(0.3f);
            canvas.MoveTo(infoX, bodyY - 0.8f * MM);
            canvas.LineTo(infoX + infoW, bodyY - 0.8f * MM);
            canvas.Stroke();
        }
        bodyY -= 2f * MM;

        var backLines = new (string Label, string Value)[]
        {
            ("Emergency", !string.IsNullOrEmpty(employee.EmergencyContactPhone)
                ? $"{employee.EmergencyContactName ?? ""} {employee.EmergencyContactPhone}".Trim() : "N/A"),
            ("Address", !string.IsNullOrEmpty(employee.PresentAddress) ? employee.PresentAddress : "N/A"),
            ("Email", employee.Email ?? "N/A"),
            ("Department", employee.Department),
        };

        foreach (var (label, value) in backLines)
        {
            {
                var c = new Canvas(canvas, new Rectangle(infoX, bodyY - lineH, labelW, lineH));
                c.Add(new Paragraph(label).SetFont(_bold).SetFontSize(5f).SetFontColor(MutedText));
                c.Close();
            }
            {
                var c = new Canvas(canvas, new Rectangle(infoX + labelW, bodyY - lineH, infoW - labelW, lineH));
                c.Add(new Paragraph(value).SetFont(_bold).SetFontSize(5.5f).SetFontColor(DarkText));
                c.Close();
            }
            bodyY -= lineH;
        }

        // — Section: IDENTIFICATION —
        bodyY -= 0.5f * MM;
        {
            var c = new Canvas(canvas, new Rectangle(infoX, bodyY - lineH, infoW, lineH));
            c.Add(new Paragraph("IDENTIFICATION")
                .SetFont(_bold).SetFontSize(5.5f).SetFontColor(themeColor).SetCharacterSpacing(0.5f));
            c.Close();
            canvas.SetStrokeColor(Gold);
            canvas.SetLineWidth(0.3f);
            canvas.MoveTo(infoX, bodyY - 0.8f * MM);
            canvas.LineTo(infoX + infoW, bodyY - 0.8f * MM);
            canvas.Stroke();
        }
        bodyY -= 2f * MM;

        var idLines = new (string Label, string Value)[]
        {
            ("National ID", !string.IsNullOrEmpty(employee.NIDNumber) ? employee.NIDNumber : "N/A"),
            ("Joining", employee.JoiningDate.ToString("dd MMM yyyy")),
            ("Valid Until", employee.CardExpiryDate?.ToString("dd MMM yyyy") ?? "N/A"),
        };

        foreach (var (label, value) in idLines)
        {
            {
                var c = new Canvas(canvas, new Rectangle(infoX, bodyY - lineH, labelW, lineH));
                c.Add(new Paragraph(label).SetFont(_bold).SetFontSize(5f).SetFontColor(MutedText));
                c.Close();
            }
            {
                var c = new Canvas(canvas, new Rectangle(infoX + labelW, bodyY - lineH, infoW - labelW, lineH));
                c.Add(new Paragraph(value).SetFont(_bold).SetFontSize(5.5f).SetFontColor(DarkText));
                c.Close();
            }
            bodyY -= lineH;
        }

        // — QR Code —
        canvas.SetStrokeColor(themeColor);
        canvas.SetLineWidth(0.5f);
        canvas.RoundRectangle(qrX, qrY, qrSize, qrSize, 1f * MM);
        canvas.Stroke();

        if (qrBytes != null)
        {
            try
            {
                var qrData = ImageDataFactory.Create(qrBytes);
                var qrImg = new Image(qrData);
                float qrPad = 1f * MM;
                qrImg.ScaleToFit(qrSize - 2f * qrPad, qrSize - 2f * qrPad);
                qrImg.SetFixedPosition(qrX + qrPad, qrY + qrPad);
                new Canvas(canvas, ps).Add(qrImg);
            }
            catch { }
        }

        {
            var c = new Canvas(canvas, new Rectangle(qrX, qrY - 2.5f * MM, qrSize, 2.5f * MM));
            c.Add(new Paragraph("Employee ID").SetFont(_normal).SetFontSize(5f).SetFontColor(MutedText)
                .SetTextAlignment(TextAlignment.CENTER));
            c.Close();
        }

        // ══════════════════════════════════════════════
        //  FOOTER
        // ══════════════════════════════════════════════
        canvas.SetFillColor(themeColor);
        canvas.Rectangle(0, 0, W, footerH);
        canvas.Fill();

        {
            var footerParts = new[]
            {
                $"{model.SchoolNameEn}",
                $"{model.SchoolPhone}  |  {model.SchoolEmail}",
                model.SchoolAddress,
                model.FooterText
            };
            var footerText = string.Join("\n", footerParts.Where(p => !string.IsNullOrEmpty(p)));

            var c = new Canvas(canvas, new Rectangle(margin, 0.3f * MM, W - 2f * margin, footerH - 0.6f * MM));
            c.Add(new Paragraph(footerText)
                .SetFont(_normal).SetFontSize(4.5f).SetFontColor(White).SetOpacity(0.9f)
                .SetTextAlignment(TextAlignment.CENTER));
            c.Close();
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  HTML → PDF via DinkToPdf (admit cards, etc.)
    // ─────────────────────────────────────────────────────────────
    public byte[] GenerateFromHtml(string html)
    {
        var converter = new SynchronizedConverter(new PdfTools());
        var doc = new HtmlToPdfDocument
        {
            GlobalSettings =
            {
                PaperSize = new PechkinPaperSize("210mm", "297mm"),
                Orientation = DinkToPdf.Orientation.Portrait,
                Margins = new MarginSettings { Top = 0, Right = 0, Bottom = 0, Left = 0 },
            },
            Objects =
            {
                new ObjectSettings
                {
                    HtmlContent = html,
                    PagesCount = true,
                    WebSettings = new WebSettings
                    {
                        DefaultEncoding = "utf-8",
                        LoadImages = true,
                        EnableIntelligentShrinking = false,
                        PrintMediaType = true,
                    },
                    LoadSettings = new LoadSettings
                    {
                        BlockLocalFileAccess = false,
                    },
                    FooterSettings = { HtmUrl = string.Empty, FontSize = 0 }
                }
            }
        };

        return converter.Convert(doc);
    }

    // ─────────────────────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────────────────────
    private void EnsureFonts()
    {
        _bold = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
        _normal = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);
    }

    private string? ResolvePath(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return null;
        var path = relativePath.TrimStart('~', '/', '\\').Replace('/', System.IO.Path.DirectorySeparatorChar);
        return System.IO.Path.Combine(_env.WebRootPath, path);
    }

    private static void DrawPlaceholderCircle(PdfCanvas canvas, float x, float y, float size, Color color)
    {
        canvas.SetFillColor(color);
        canvas.Circle(x + size / 2f, y + size / 2f, size / 2f);
        canvas.Fill();
    }

    private static Color GetPdfThemeColor(string? designation)
    {
        if (string.IsNullOrWhiteSpace(designation))
            return Primary;
        var des = designation.Trim().ToLowerInvariant();
        return des switch
        {
            string d when d.Contains("principal") => new DeviceRgb(0xDA, 0xA5, 0x20),
            string d when d.Contains("vice principal") => new DeviceRgb(0xC0, 0xC0, 0xC0),
            string d when d.Contains("teacher") => Primary,
            string d when d.Contains("accountant") => new DeviceRgb(0x2E, 0x8B, 0x57),
            string d when d.Contains("librarian") => new DeviceRgb(0x80, 0x00, 0x80),
            string d when d.Contains("staff") => new DeviceRgb(0x80, 0x80, 0x80),
            string d when d.Contains("assistant head") => new DeviceRgb(0x00, 0x00, 0x8B),
            string d when d.Contains("senior teacher") => new DeviceRgb(0x1E, 0x90, 0xFF),
            string d when d.Contains("lab assistant") => new DeviceRgb(0xFF, 0x8C, 0x00),
            string d when d.Contains("driver") => new DeviceRgb(0x8B, 0x45, 0x13),
            string d when d.Contains("guard") => new DeviceRgb(0x28, 0x28, 0x28),
            _ => Primary,
        };
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
}
