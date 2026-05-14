using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using SchoolManagementSystem.Helpers;

using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using System.IO;
using SchoolManagementSystem.Models.DTOs.Student;
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
            var subjectName = mark.Subject.Name;

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

    public byte[] GenerateStudentIdCard(StudentUpsertDto student)
    {
        using var stream = new MemoryStream();
        using var writer = new PdfWriter(stream);
        using var pdf = new PdfDocument(writer);
        
        // Custom ID Card Size (CR80 equivalent in points: ~243 x 153)
        var pageSize = new PageSize(243, 153);
        var document = new Document(pdf, pageSize);
        document.SetMargins(10, 10, 10, 10);

        PdfFont bold = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
        PdfFont normal = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

        // Background Color / Border
        var borderTable = new Table(1).SetWidth(UnitValue.CreatePercentValue(100));
        borderTable.SetBorder(new SolidBorder(ColorConstants.BLUE, 2));
        
        // Header
        var header = new Cell().Add(new Paragraph("SCHOOL MS")
            .SetFont(bold).SetFontSize(10).SetFontColor(ColorConstants.BLUE)
            .SetTextAlignment(TextAlignment.CENTER))
            .SetBorder(Border.NO_BORDER).SetPadding(2);
        borderTable.AddCell(header);

        // Content Table (Photo | Info)
        var contentTable = new Table(UnitValue.CreatePercentArray(new float[] { 3, 7 })).UseAllAvailableWidth();
        
        // Photo Placeholder
        var photoCell = new Cell().Add(new Paragraph("PHOTO")
            .SetFontSize(8).SetTextAlignment(TextAlignment.CENTER))
            .SetHeight(50).SetVerticalAlignment(VerticalAlignment.MIDDLE)
            .SetBorder(new SolidBorder(ColorConstants.GRAY, 1));
        contentTable.AddCell(photoCell);

        // Info
        var infoCell = new Cell().SetPaddingLeft(5).SetBorder(Border.NO_BORDER);
        infoCell.Add(new Paragraph(student.FullName).SetFont(bold).SetFontSize(10));
        infoCell.Add(new Paragraph($"ID: {student.StudentNo}").SetFontSize(8));
        infoCell.Add(new Paragraph($"Class: {student.ClassId}").SetFontSize(7));
        infoCell.Add(new Paragraph($"Roll: {student.RollNumber}").SetFontSize(7));
        contentTable.AddCell(infoCell);

        borderTable.AddCell(new Cell().Add(contentTable).SetBorder(Border.NO_BORDER));

        document.Add(borderTable);
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