using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using SchoolManagementSystem.Helpers;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using System.IO;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.Entities.Website;
using Path = System.IO.Path;

namespace SchoolManagementSystem.Helpers.Pdf;

public class PlainPdfGenerator : IPdfGenerator
{
    // ─────────────────────────────────────────────────────────────
    //  REPORT CARD  (unchanged)
    // ─────────────────────────────────────────────────────────────
    public byte[] GenerateSchoolReportCard(
        StudentExamResult result,
        List<MarkEntry> marks,SchoolSetting school)
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
            table.AddCell(GetBodyCell(mark.Subject.Name));
            table.AddCell(GetBodyCell($"{mark.MarksObtained} / 100"));
            table.AddCell(GetBodyCell(mark.Grade ?? "N/A"));
            table.AddCell(GetBodyCell((mark.GradePoint ?? 0).ToString("F2")));
            table.AddCell(GetBodyCell(mark.MarksObtained >= 33 ? "PASSED" : "FAILED"));
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
    //  STUDENT ID CARD  (new design)
    // ─────────────────────────────────────────────────────────────
    public byte[] GenerateStudentIdCard(StudentUpsertDto student, SchoolSetting school)
    {
        using var stream = new MemoryStream();
        using var writer = new PdfWriter(stream);
        using var pdf = new PdfDocument(writer);

        // CR80 card: ~243 x 153 pt  (portrait: swap → 153 w × 243 h)
        // The reference image shows a PORTRAIT card
        float cardW = 153f, cardH = 243f;
        var pageSize = new PageSize(cardW, cardH);

        // ── FRONT PAGE ──────────────────────────────────────────
        // Must add the page before drawing on it
        pdf.AddNewPage(pageSize);
        DrawStudentFront(pdf, pageSize, student, school);

        // ── BACK PAGE ───────────────────────────────────────────
        pdf.AddNewPage(pageSize);
        DrawStudentBack(pdf, pageSize, student, school);

        pdf.Close();
        return stream.ToArray();
    }

    private void DrawStudentFront(PdfDocument pdf, PageSize ps, StudentUpsertDto student, SchoolSetting school)
    {
        var page = pdf.GetFirstPage();
        var canvas = new PdfCanvas(page);
        float w = ps.GetWidth(), h = ps.GetHeight();

        // Dark blue background (top 55 %)
        var darkBlue = new DeviceRgb(0x2B, 0x2F, 0x8F);   // #2B2F8F
        var lightBlue = new DeviceRgb(0xB8, 0xC4, 0xF5);   // #B8C4F5

        // Full card background – light blue
        canvas.SetFillColor(lightBlue)
              .Rectangle(0, 0, w, h)
              .Fill();

        // Dark blue wavy top section (top ~55% of card)
        // Approximated with a rounded rectangle + bezier wave
        float waveY = h * 0.44f;
        canvas.SetFillColor(darkBlue);
        canvas.MoveTo(0, h)
              .LineTo(w, h)
              .LineTo(w, waveY + 10)
              .CurveTo(w * 0.75f, waveY - 18, w * 0.25f, waveY + 20, 0, waveY + 8)
              .ClosePath()
              .Fill();

        // Decorative dots (light, scattered) – front top-right area
        canvas.SetFillColor(new DeviceRgb(0x6B, 0x74, 0xD0));
        float[] dotXs = { w * 0.72f, w * 0.80f, w * 0.88f, w * 0.76f, w * 0.68f };
        float[] dotYs = { h * 0.78f, h * 0.72f, h * 0.82f, h * 0.90f, h * 0.65f };
        for (int i = 0; i < dotXs.Length; i++)
            canvas.Circle(dotXs[i], dotYs[i], 2.5f).Fill();

        // Small tick marks (bottom left, light blue on dark)
        canvas.SetFillColor(new DeviceRgb(0x9B, 0xA5, 0xE8));
        float[] tickX = { 12, 18, 8 };
        float[] tickY = { h * 0.52f, h * 0.47f, h * 0.44f };
        for (int i = 0; i < tickX.Length; i++)
        {
            canvas.Rectangle(tickX[i], tickY[i], 6, 1.5f).Fill();
            canvas.Rectangle(tickX[i] + 2, tickY[i] - 3, 6, 1.5f).Fill();
        }

        // Circular photo frame
        float circleR = 30f;
        float circleX = w / 2f;
        float circleY = h * 0.60f;

        // White ring
        canvas.SetFillColor(ColorConstants.WHITE)
              .Circle(circleX, circleY, circleR + 3)
              .Fill();

        // Photo or placeholder inside circle
        // We clip to circle for the photo
        canvas.SaveState();
        canvas.SetFillColor(new DeviceRgb(0xCC, 0xCC, 0xCC));
        canvas.Circle(circleX, circleY, circleR).Fill();
        // (Actual image clipping requires iText canvas.Clip() + image paint – placeholder shown)
        canvas.RestoreState();

        // Text area (below wave)
        PdfFont bold = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
        PdfFont normal = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

        var layout = new Canvas(canvas, ps);

        // Name
        layout.Add(new Paragraph(student.FullName)
            .SetFont(bold).SetFontSize(9f)
            .SetFontColor(darkBlue)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFixedPosition(0, h * 0.335f, w));

        // Designation / class label
        layout.Add(new Paragraph($"Class {student.ClassId}")
            .SetFont(normal).SetFontSize(7f)
            .SetFontColor(new DeviceRgb(0x55, 0x5E, 0xCC))
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFixedPosition(0, h * 0.305f, w));

        // Icon row: location, phone, email
        float iconRowY = h * 0.21f;
        float lineH = 14f;

        // Row 1 – ID No
        layout.Add(new Paragraph($"\u25CF  {student.StudentNo}")
            .SetFont(normal).SetFontSize(6.5f)
            .SetFontColor(new DeviceRgb(0x44, 0x44, 0x44))
            .SetFixedPosition(20, iconRowY + lineH * 2, w - 40));

        // Row 2 – Roll
        layout.Add(new Paragraph($"\u260E  Roll: {student.RollNumber}")
            .SetFont(normal).SetFontSize(6.5f)
            .SetFontColor(new DeviceRgb(0x44, 0x44, 0x44))
            .SetFixedPosition(20, iconRowY + lineH, w - 40));

        // Row 3 – placeholder email / contact
        layout.Add(new Paragraph($"\u2709  {school.Email}")
            .SetFont(normal).SetFontSize(6.5f)
            .SetFontColor(new DeviceRgb(0x44, 0x44, 0x44))
            .SetFixedPosition(20, iconRowY, w - 40));

        layout.Close();

        // School name top-centre — reuse same page canvas
        var topLayout = new Canvas(new PdfCanvas(pdf.GetPage(1)), ps);
        topLayout.Add(new Paragraph($"{school.SchoolName}")
            .SetFont(bold).SetFontSize(8f)
            .SetFontColor(ColorConstants.WHITE)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFixedPosition(0, h - 26, w));
        topLayout.Close();
    }

    private void DrawStudentBack(PdfDocument pdf, PageSize ps, StudentUpsertDto student, SchoolSetting school)
    {
        var page = pdf.GetPage(2);
        var canvas = new PdfCanvas(page);
        float w = ps.GetWidth(), h = ps.GetHeight();

        var darkBlue = new DeviceRgb(0x2B, 0x2F, 0x8F);
        var lightBlue = new DeviceRgb(0xB8, 0xC4, 0xF5);

        // Background
        canvas.SetFillColor(lightBlue)
              .Rectangle(0, 0, w, h).Fill();

        // Small dark-blue wave at top
        canvas.SetFillColor(darkBlue);
        canvas.MoveTo(0, h)
              .LineTo(w, h)
              .LineTo(w, h - 40)
              .CurveTo(w * 0.65f, h - 22, w * 0.35f, h - 54, 0, h - 36)
              .ClosePath().Fill();

        // Bottom wave
        canvas.SetFillColor(darkBlue);
        canvas.MoveTo(0, 0)
              .LineTo(w, 0)
              .LineTo(w, 28)
              .CurveTo(w * 0.65f, 46, w * 0.35f, 14, 0, 32)
              .ClosePath().Fill();

        // Decorative cross lines bottom-right
        canvas.SetStrokeColor(new DeviceRgb(0x6B, 0x74, 0xD0));
        canvas.SetLineWidth(1.2f);
        canvas.MoveTo(w - 22, 28).LineTo(w - 22, 50).Stroke();
        canvas.MoveTo(w - 30, 38).LineTo(w - 8, 38).Stroke();
        canvas.MoveTo(w - 14, 20).LineTo(w - 14, 36).Stroke();

        // Book icon (open book shape) – top centre
        PdfFont bold = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
        PdfFont normal = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

        var layout = new Canvas(canvas, ps);
        layout.Add(new Paragraph($"{school.SchoolName}")
            .SetFont(bold).SetFontSize(10f)
            .SetFontColor(ColorConstants.WHITE)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFixedPosition(0, h - 30, w));

        // Join / Expired dates
        float midY = h * 0.58f;
        layout.Add(new Paragraph("JOIN :  MM/DD/YY")
            .SetFont(bold).SetFontSize(7f)
            .SetFontColor(darkBlue)
            .SetFixedPosition(14, midY, w - 28));

        layout.Add(new Paragraph("EXPIRED :  MM/DD/YY")
            .SetFont(bold).SetFontSize(7f)
            .SetFontColor(darkBlue)
            .SetFixedPosition(14, midY - 13, w - 28));

        // Terms text
        layout.Add(new Paragraph(
                "This card is the property of the institution. " +
                "If found, please return to the school office.")
            .SetFont(normal).SetFontSize(5.5f)
            .SetFontColor(new DeviceRgb(0x44, 0x44, 0x44))
            .SetFixedPosition(14, midY - 40, w - 28));

        // QR Code
        var qrData = $"ID:{student.StudentNo}|Name:{student.FullName}|Class:{student.ClassId}|Roll:{student.RollNumber}";
        var qrBytes = GetQrCodeBytes(qrData);
        if (qrBytes != null)
        {
            try
            {
                var qrImg = new Image(ImageDataFactory.Create(qrBytes))
                    .ScaleAbsolute(52, 52)
                    .SetFixedPosition((w - 52) / 2f, 32);
                layout.Add(qrImg);
            }
            catch { /* skip */ }
        }

        layout.Close();
    }

    // ─────────────────────────────────────────────────────────────
    //  EMPLOYEE ID CARD  (new design – matches screenshot exactly)
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
            using var client = new System.Net.Http.HttpClient();
            var url = $"https://api.qrserver.com/v1/create-qr-code/?size=150x150&data={Uri.EscapeDataString(data)}";
            return client.GetByteArrayAsync(url).GetAwaiter().GetResult();
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
