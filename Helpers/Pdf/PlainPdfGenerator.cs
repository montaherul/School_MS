using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using SchoolManagementSystem.Helpers;
using SchoolManagementSystem.Helpers.Common;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using System.IO;
namespace SchoolManagementSystem.Helpers.Pdf;

public class PlainPdfGenerator : IPdfGenerator
{
    public byte[] GenerateSchoolReportCard(
        StudentExamResult result,
        List<MarkEntry> marks)
    {
        using var stream = new MemoryStream();

        using var writer = new PdfWriter(stream);

        using var pdf = new PdfDocument(writer);

        var document = new Document(pdf, PageSize.A4);

        document.SetMargins(20, 20, 20, 20);

        PdfFont bold = PdfFontFactory.CreateFont(
            iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

        PdfFont normal = PdfFontFactory.CreateFont(
            iText.IO.Font.Constants.StandardFonts.HELVETICA);
        document.Add(
          new Paragraph("SCHOOL MANAGEMENT SYSTEM")
              .SetFont(bold)
              .SetFontSize(22)
              .SetTextAlignment(TextAlignment.CENTER)
              .SetFontColor(ColorConstants.BLUE)
      );

        document.Add(
            new Paragraph("Academic Report Card")
                .SetFont(bold)
                .SetFontSize(16)
                .SetTextAlignment(TextAlignment.CENTER)
        );

        document.Add(new Paragraph("\n"));
        var infoTable = new Table(UnitValue.CreatePercentArray(new float[] { 2, 4 }))
            .UseAllAvailableWidth();

        infoTable.AddCell(GetLabelCell("Student Name"));
        infoTable.AddCell(GetValueCell(result.Student.FullName));

        infoTable.AddCell(GetLabelCell("Student ID"));
        infoTable.AddCell(GetValueCell(result.Student.StudentNo));

        infoTable.AddCell(GetLabelCell("Exam"));
        infoTable.AddCell(GetValueCell(result.Exam.Name));

        infoTable.AddCell(GetLabelCell("GPA"));
        infoTable.AddCell(GetValueCell(result.Gpa.ToString("F2")));

        infoTable.AddCell(GetLabelCell("Total Marks"));
        infoTable.AddCell(GetValueCell(result.TotalMarks.ToString("F2")));

        infoTable.AddCell(GetLabelCell("Position"));
        infoTable.AddCell(GetValueCell(result.Position.ToString()));

        document.Add(infoTable);

        document.Add(new Paragraph("\n"));
        var table = new Table(UnitValue.CreatePercentArray(new float[]
       {
            4, 2, 2, 2, 2
       }))
       .UseAllAvailableWidth();

        table.AddHeaderCell(GetHeaderCell("Subject"));
        table.AddHeaderCell(GetHeaderCell("Marks"));
        table.AddHeaderCell(GetHeaderCell("Grade"));
        table.AddHeaderCell(GetHeaderCell("Grade Point"));
        table.AddHeaderCell(GetHeaderCell("Status"));

        foreach (var mark in marks)
        {
            var subjectName = mark.Subject.IsReligionSubject &&
                              !string.IsNullOrEmpty(mark.Subject.ReligionType)
                ? ReligionHelper.GetReligionSubjectName(
                    mark.Subject.ReligionType)
                : mark.Subject.Name;

            table.AddCell(GetBodyCell(subjectName));

            table.AddCell(GetBodyCell(
                $"{mark.MarksObtained} / 100"));

            table.AddCell(GetBodyCell(mark.Grade ?? "N/A"));

            table.AddCell(GetBodyCell(
                (mark.GradePoint ?? 0).ToString("F2")));
            table.AddCell(GetBodyCell(
         mark.MarksObtained >= 33
             ? "PASSED"
             : "FAILED"));
        }

        document.Add(table);

        document.Add(new Paragraph("\n"));

        // FINAL RESULT SUMMARY

        var finalBox = new Table(1)
            .UseAllAvailableWidth();

        string status = result.Gpa > 0
            ? "PROMOTED"
            : "FAILED";

        finalBox.AddCell(
            new Cell()
                .Add(new Paragraph(
                    $"Final GPA: {result.Gpa:F2} | Status: {status}"))
                .SetFont(bold)
                .SetFontSize(14)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetPadding(10)
        );
        document.Add(finalBox);

        document.Add(new Paragraph("\n\n"));

        // SIGNATURE AREA

        var signTable = new Table(UnitValue.CreatePercentArray(new float[]
        {
            1,1
        }))
        .UseAllAvailableWidth();

        signTable.AddCell(
            new Cell()
                .Add(new Paragraph("Class Teacher"))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorderTop(new SolidBorder(1))
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetPaddingTop(20)
        );

        signTable.AddCell(
            new Cell()
                .Add(new Paragraph("Principal"))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorderTop(new SolidBorder(1))
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetPaddingTop(20)
        );

        document.Add(signTable);

        document.Close();

        return stream.ToArray();
    }

    private Cell GetHeaderCell(string text)
    {
        return new Cell()
            .Add(new Paragraph(text)
            .SetBold()
            .SetFontColor(ColorConstants.WHITE))
            .SetBackgroundColor(ColorConstants.BLUE)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetPadding(5);
    }

    private Cell GetBodyCell(string text)
    {
        return new Cell()
            .Add(new Paragraph(text))
            .SetTextAlignment(TextAlignment.CENTER)
            .SetPadding(5);
    }

    private Cell GetLabelCell(string text)
    {
        return new Cell()
            .Add(new Paragraph(text).SetBold())
            .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
            .SetPadding(5);
    }
    private Cell GetValueCell(string text)
    {
        return new Cell()
            .Add(new Paragraph(text))
            .SetPadding(5);
    }
}