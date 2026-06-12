using DinkToPdf;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using SchoolManagementSystem.Helpers;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Website;
using Path = System.IO.Path;

namespace SchoolManagementSystem.Helpers.Pdf;

public class PlainPdfGenerator : IPdfGenerator
{
    private static readonly object _syncLock = new();

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

        PdfFont bold = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
        PdfFont normal = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

        document.Add(new Paragraph($"{school.SchoolName}")
            .SetFont(bold).SetFontSize(22)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontColor(ColorConstants.BLUE));

        document.Add(new Paragraph("Academic Report Card")
            .SetFont(bold).SetFontSize(16)
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

        var table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 2, 2, 2, 2 })).UseAllAvailableWidth();
        table.AddHeaderCell(GetHeaderCell("Subject"));
        table.AddHeaderCell(GetHeaderCell("Marks"));
        table.AddHeaderCell(GetHeaderCell("Grade"));
        table.AddHeaderCell(GetHeaderCell("Grade Point"));
        table.AddHeaderCell(GetHeaderCell("Status"));

        foreach (var mark in marks)
        {
            var fullMarks = mark.Subject?.DefaultFullMarks ?? 100;
            var passMarks = mark.Subject?.DefaultPassMarks ?? 33;
            table.AddCell(GetBodyCell(mark.Subject.Name));
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
            .SetFont(bold).SetFontSize(14)
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
    //  STUDENT ID CARD  — HTML to PDF via DinkToPdf
    // ─────────────────────────────────────────────────────────────
    public byte[] GenerateStudentIdCardFromHtml(string html)
    {
        return ConvertHtmlToPdf(html, singleCard: true);
    }

    public byte[] GenerateBulkStudentIdCardPdfFromHtml(string html)
    {
        return ConvertHtmlToPdf(html, singleCard: false);
    }

    private byte[] ConvertHtmlToPdf(string html, bool singleCard)
    {
        lock (_syncLock)
        {
            var converter = new SynchronizedConverter(new PdfTools());
            var doc = new HtmlToPdfDocument
            {
                GlobalSettings =
                {
                    PaperSize = singleCard
                        ? new PechkinPaperSize("85.6mm", "53.98mm")
                        : new PechkinPaperSize("210mm", "297mm"),
                    Orientation = DinkToPdf.Orientation.Landscape,
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
                        FooterSettings = { HtmUrl = string.Empty, FontSize = 0 }
                    }
                }
            };

            return converter.Convert(doc);
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  EMPLOYEE ID CARD  — HTML to PDF via DinkToPdf
    // ─────────────────────────────────────────────────────────────
    public byte[] GenerateEmployeeIdCardFromHtml(string html)
    {
        return ConvertHtmlToPdf(html, singleCard: true);
    }

    public byte[] GenerateBulkEmployeeIdCardPdfFromHtml(string html)
    {
        return ConvertHtmlToPdf(html, singleCard: false);
    }

    public byte[] GenerateFromHtml(string html)
    {
        return ConvertHtmlToPdf(html, singleCard: false);
    }

    // ─────────────────────────────────────────────────────────────
    //  EMPLOYEE ID CARD  (iText7 – legacy, kept for compatibility)
    // ─────────────────────────────────────────────────────────────
    public byte[] GenerateEmployeeIdCard(EmployeeDetailsDto employee, SchoolSetting schoolSetting)
    {
        using var stream = new MemoryStream();
        using var writer = new PdfWriter(stream);
        using var pdf = new PdfDocument(writer);

        // Portrait CR80 card
        float cardW = 153f, cardH = 243f;
        var pageSize = new PageSize(cardW, cardH);

        // Determine theme colour from designation
        var themeColor = GetThemeColor(employee.Designation);

        // ── FRONT ───────────────────────────────────────────────
        // Must add the page first; GetFirstPage() fails on an empty document
        pdf.AddNewPage(pageSize);
        DrawEmployeeFront(pdf, pageSize, employee, schoolSetting, themeColor);

        // ── BACK ────────────────────────────────────────────────
        pdf.AddNewPage(pageSize);
        DrawEmployeeBack(pdf, pageSize, employee, schoolSetting, themeColor);

        pdf.Close();
        return stream.ToArray();
    }

    // ─────────────────────────────────────────────────────────────
    //  TRANSCRIPT
    // ─────────────────────────────────────────────────────────────
    public byte[] GenerateTranscript(StudentTranscriptDto transcript)
    {
        using var stream = new MemoryStream();
        using var writer = new PdfWriter(stream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf, PageSize.A4.Rotate());
        document.SetMargins(20, 20, 20, 20);

        PdfFont bold = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
        PdfFont normal = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

        document.Add(new Paragraph(transcript.SchoolName)
            .SetFont(bold).SetFontSize(22)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontColor(ColorConstants.BLUE));

        document.Add(new Paragraph("Academic Transcript")
            .SetFont(bold).SetFontSize(16)
            .SetTextAlignment(TextAlignment.CENTER));

        document.Add(new Paragraph($"Academic Year: {transcript.AcademicYear}")
            .SetFont(normal).SetFontSize(12)
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
                .SetFont(bold).SetFontSize(13)
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
            .SetFont(bold).SetFontSize(13)
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
            .SetFont(bold).SetFontSize(14)
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

    // ── Front drawing ──────────────────────────────────────────────
    private void DrawEmployeeFront(
        PdfDocument pdf, PageSize ps,
        EmployeeDetailsDto employee, SchoolSetting schoolSetting,
        DeviceRgb themeColor)
    {
        // Card: w=153, h=243 (all Y = distance from BOTTOM)
        // Dark wave zone: top of card → y≈107
        //   "International" label  → y=227  (h-16)
        //   School name            → y=202  (h-41, up to 2 lines, font 8)
        //   Photo centre           → y=148, radius=30  (top=178, bottom=118 – inside dark zone)
        //   Wave bottom            → y≈107
        // Light zone: y=107 → 0
        //   Name                   → y=88
        //   Designation            → y=76
        //   Address row            → y=58
        //   Phone row              → y=44
        //   Email row              → y=30
        //   Bottom margin          → y=18

        var pc = new PdfCanvas(pdf.GetPage(1));
        float w = ps.GetWidth(), h = ps.GetHeight();

        var bgColor = new DeviceRgb(0xC5, 0xCC, 0xF5);

        // 1. Light background
        pc.SetFillColor(bgColor).Rectangle(0, 0, w, h).Fill();

        // 2. Dark wavy top section
        float waveL = 107f, waveR = 112f, waveCtrlY = 85f;
        pc.SetFillColor(themeColor);
        pc.MoveTo(0, h)
          .LineTo(w, h)
          .LineTo(w, waveR)
          .CurveTo(w * 0.70f, waveCtrlY, w * 0.30f, waveCtrlY, 0, waveL)
          .ClosePath()
          .Fill();

        // 3. Decorative dots inside dark zone (right side, y between 193 and 142)
        var dotColor = Lighten(themeColor, 45);
        pc.SetFillColor(dotColor);
        (float dx, float dy)[] dots =
        {
            (w * 0.78f, h - 22f),
            (w * 0.88f, h - 38f),
            (w * 0.72f, h - 45f),
            (w * 0.84f, h - 58f),
            (w * 0.90f, h - 68f),
        };
        foreach (var (dx, dy) in dots)
            pc.Circle(dx, dy, 3f).Fill();

        // 4. Tick/dash marks – bottom-left of dark zone (y 111–125)
        pc.SetFillColor(dotColor);
        float[] tickYs = { 125f, 118f, 111f };
        foreach (var ty in tickYs)
        {
            pc.Rectangle(8f, ty, 10f, 2f).Fill();
            pc.Rectangle(12f, ty - 4f, 7f, 2f).Fill();
        }

        // 5. Circular photo (centre at 76.5, 148; radius 30)
        float circR = 30f, circX = w / 2f, circY = 148f;
        pc.SetFillColor(ColorConstants.WHITE).Circle(circX, circY, circR + 3.5f).Fill();

        bool photoDrawn = false;
        var relPath = employee.ProfilePicturePath;
        var fullPath = string.IsNullOrEmpty(relPath)
            ? ""
            : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relPath.TrimStart('/'));

        if (!string.IsNullOrEmpty(fullPath) && File.Exists(fullPath))
        {
            try
            {
                pc.SaveState();
                double k = 0.5522847498, r = circR, cx = circX, cy = circY;
                pc.MoveTo(cx + r, cy)
                  .CurveTo(cx + r, cy + k * r, cx + k * r, cy + r, cx, cy + r)
                  .CurveTo(cx - k * r, cy + r, cx - r, cy + k * r, cx - r, cy)
                  .CurveTo(cx - r, cy - k * r, cx - k * r, cy - r, cx, cy - r)
                  .CurveTo(cx + k * r, cy - r, cx + r, cy - k * r, cx + r, cy)
                  .ClosePath().Clip().EndPath();
                pc.AddImageFittedIntoRectangle(
                    ImageDataFactory.Create(fullPath),
                    new Rectangle(circX - circR, circY - circR, circR * 2, circR * 2), false);
                pc.RestoreState();
                photoDrawn = true;
            }
            catch { }
        }
        if (!photoDrawn)
        {
            pc.SetFillColor(new DeviceRgb(0xBB, 0xBB, 0xBB)).Circle(circX, circY, circR).Fill();
        }

        // 6. Text layer
        PdfFont bold = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
        PdfFont normal = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);
        var lay = new Canvas(pc, ps);

        // School name – constrained width so it wraps and stays above photo
        var schoolName = schoolSetting?.SchoolName?.ToUpper() ?? "SCHOOL MS";
        lay.Add(new Paragraph(schoolName)
            .SetFont(bold).SetFontSize(8f).SetFontColor(ColorConstants.WHITE)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFixedPosition(6f, h - 42f, w - 12f).SetMargin(0));

        // Photo placeholder text
        if (!photoDrawn)
        {
            lay.Add(new Paragraph("PHOTO")
                .SetFont(normal).SetFontSize(7f).SetFontColor(ColorConstants.GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFixedPosition(circX - circR, circY - 4f, circR * 2).SetMargin(0));
        }

        // Name
        lay.Add(new Paragraph(employee.FullName)
            .SetFont(bold).SetFontSize(9f).SetFontColor(ColorConstants.WHITE)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFixedPosition(6f, 88f, w - 12f).SetMargin(0));

        // Designation
        lay.Add(new Paragraph(employee.Designation ?? "")
            .SetFont(normal).SetFontSize(7.5f).SetFontColor(Lighten(themeColor, 35))
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFixedPosition(6f, 76f, w - 12f).SetMargin(0));

        var rowColor = new DeviceRgb(0x22, 0x22, 0x44);

        lay.Add(new Paragraph($"Blood Group: {employee.BloodGroup ?? "N/A"}")
            .SetFont(normal)
            .SetFontSize(6f)
            .SetFontColor(rowColor)
            .SetFixedPosition(14f, 58f, w - 28f)
            .SetMargin(0));

        lay.Add(new Paragraph($" Phone: {employee.Phone ?? schoolSetting?.Phone ?? "+00 000 000 000"}")
            .SetFont(normal)
            .SetFontSize(6f)
            .SetFontColor(rowColor)
            .SetFixedPosition(14f, 44f, w - 28f)
            .SetMargin(0));

        lay.Add(new Paragraph($" Email: {employee.Email ?? "info@school.edu"}")
            .SetFont(normal)
            .SetFontSize(6f)
            .SetFontColor(rowColor)
            .SetFixedPosition(14f, 30f, w - 28f)
            .SetMargin(0));
        lay.Close();
    }

    // ── Back drawing ───────────────────────────────────────────────
    private void DrawEmployeeBack(
        PdfDocument pdf, PageSize ps,
        EmployeeDetailsDto employee, SchoolSetting schoolSetting,
        DeviceRgb themeColor)
    {
        // Back layout (Y from bottom, card h=243):
        //  Top dark wave band     → y = 243 down to ~200
        //    "International"      → y = 227
        //    School name          → y = 202 (2 lines, font 8)
        //  Light middle zone      → y = 200 down to ~43
        //    JOIN                 → y = 178
        //    EXPIRED              → y = 164
        //    Terms text           → y = 145 (2 lines)
        //    School address block → y = 118 (3 lines)
        //    QR code              → y = 52, centred, size=60
        //  Bottom dark wave band  → y = 43 down to 0
        //    Cross decorations    → inside bottom band

        var pc = new PdfCanvas(pdf.GetPage(2));
        float w = ps.GetWidth(), h = ps.GetHeight();

        var bgColor = new DeviceRgb(0xC5, 0xCC, 0xF5);
        pc.SetFillColor(bgColor).Rectangle(0, 0, w, h).Fill();

        // Top wavy band (h → ~200)
        pc.SetFillColor(themeColor);
        pc.MoveTo(0, h).LineTo(w, h).LineTo(w, h - 40f)
          .CurveTo(w * 0.65f, h - 20f, w * 0.35f, h - 55f, 0, h - 36f)
          .ClosePath().Fill();

        // Bottom wavy band (0 → ~43)
        pc.SetFillColor(themeColor);
        pc.MoveTo(0, 0).LineTo(w, 0).LineTo(w, 32f)
          .CurveTo(w * 0.65f, 50f, w * 0.35f, 16f, 0, 36f)
          .ClosePath().Fill();

        // Cross decoration bottom-right (inside bottom band, y 15-50)
        var crossColor = Lighten(themeColor, 55);
        pc.SetStrokeColor(crossColor).SetLineWidth(1.5f);
        pc.MoveTo(w - 20f, 14f).LineTo(w - 20f, 38f).Stroke();
        pc.MoveTo(w - 32f, 26f).LineTo(w - 8f, 26f).Stroke();
        pc.MoveTo(w - 12f, 8f).LineTo(w - 12f, 22f).Stroke();

        PdfFont bold = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
        PdfFont normal = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);
        var lay = new Canvas(pc, ps);

        // Top band text
       

        lay.Add(new Paragraph(schoolSetting?.SchoolName?.ToUpper() ?? "SCHOOL MS")
            .SetFont(bold).SetFontSize(8f).SetFontColor(ColorConstants.WHITE)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFixedPosition(6f, h - 20f, w - 12f).SetMargin(0));

        // JOIN / EXPIRED
        lay.Add(new Paragraph("JOIN :  " +
            (employee.CardIssueDate.HasValue
             ? employee.CardIssueDate.Value.ToString("MM/dd/yy")
                    : "MM/DD/YY"))
            .SetFont(bold).SetFontSize(7.5f).SetFontColor(themeColor)
            .SetFixedPosition(14f, 178f, w - 28f).SetMargin(0));

        lay.Add(new Paragraph("EXPIRED :  " +
                (employee.CardExpiryDate.HasValue
                    ? employee.CardExpiryDate.Value.ToString("MM/dd/yy")
                    : "MM/DD/YY"))
            .SetFont(bold).SetFontSize(7.5f).SetFontColor(themeColor)
            .SetFixedPosition(14f, 163f, w - 28f).SetMargin(0));

        // Terms
        lay.Add(new Paragraph(
                "This card is the property of the school. If found, " +
                "please return to the address below.")
            .SetFont(normal).SetFontSize(5.5f).SetFontColor(new DeviceRgb(0x33, 0x33, 0x44))
            .SetFixedPosition(14f, 140f, w - 28f).SetMargin(0));

        // Address block
        lay.Add(new Paragraph(
                $"{schoolSetting?.SchoolName ?? "School Name"}\n" +
                $"Address: {schoolSetting?.Address ?? ""}\n" +
                $"Phone: {schoolSetting?.Phone ?? ""}")
            .SetFont(bold).SetFontSize(5.5f).SetFontColor(new DeviceRgb(0x22, 0x22, 0x44))
            .SetFixedPosition(14f, 110f, w - 28f).SetMargin(0));

        // QR code – centred, y=52, size=60×60
        var verificationUrl = $"{schoolSetting?.Website?.TrimEnd('/')}/Employee/Verify/{employee.Id}";
        var qrData = $"ID:{employee.Id}|Code:{employee.EmployeeCode}|Name:{employee.FullName}" +
                     $"|Designation:{employee.Designation}|Verify:{verificationUrl}";
        var qrBytes = GetQrCodeBytes(qrData);
        float qrSize = 60f;
        float qrX = (w - qrSize) / 2f;
        float qrY = 47f;

        if (qrBytes != null)
        {
            try
            {
                lay.Add(new Image(ImageDataFactory.Create(qrBytes))
                    .ScaleAbsolute(qrSize, qrSize)
                    .SetFixedPosition(qrX, qrY));
            }
            catch { DrawQrPlaceholder(pc, qrX, qrY, qrSize); }
        }
        else
        {
            DrawQrPlaceholder(pc, qrX, qrY, qrSize);
        }

        lay.Close();
    }

    // ── Helpers ────────────────────────────────────────────────────

    private static void DrawQrPlaceholder(PdfCanvas cv, float x, float y, float size)
    {
        cv.SetFillColor(ColorConstants.WHITE)
          .Rectangle(x, y, size, size).Fill();
        cv.SetFillColor(ColorConstants.LIGHT_GRAY)
          .Rectangle(x + 4, y + 4, size - 8, size - 8).Fill();
    }

    private static DeviceRgb GetThemeColor(string? designation)
    {
        var des = (designation ?? "").ToLowerInvariant();
        if (des.Contains("principal"))
        {
            if (des.Contains("vice")) return new DeviceRgb(192, 192, 192);
            return new DeviceRgb(218, 165, 32);
        }
        if (des.Contains("assistant head")) return new DeviceRgb(0, 0, 139);
        if (des.Contains("senior teacher")) return new DeviceRgb(30, 144, 255);
        if (des.Contains("teacher")) return new DeviceRgb(0x2B, 0x2F, 0x8F); // default indigo
        if (des.Contains("accountant")) return new DeviceRgb(34, 139, 34);
        if (des.Contains("librarian")) return new DeviceRgb(128, 0, 128);
        if (des.Contains("lab assistant")) return new DeviceRgb(255, 140, 0);
        if (des.Contains("driver")) return new DeviceRgb(139, 69, 19);
        if (des.Contains("guard")) return new DeviceRgb(40, 40, 40);
        if (des.Contains("staff")) return new DeviceRgb(128, 128, 128);
        return new DeviceRgb(0x2B, 0x2F, 0x8F); // default
    }

    /// <summary>Returns a lighter version of the color by adding an offset to each channel.</summary>
    private static DeviceRgb Lighten(DeviceRgb color, int offset)
    {
        // DeviceRgb stores channels 0-1 as floats
        float[] cv = color.GetColorValue();
        float r = Math.Min(1f, cv[0] + offset / 255f);
        float g = Math.Min(1f, cv[1] + offset / 255f);
        float b = Math.Min(1f, cv[2] + offset / 255f);
        return new DeviceRgb(r, g, b);
    }

    private static byte[]? GetQrCodeBytes(string data)
    {
        try
        {
            return IdCardQRHelper.GenerateQrCodePng(data);
        }
        catch { return null; }
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
