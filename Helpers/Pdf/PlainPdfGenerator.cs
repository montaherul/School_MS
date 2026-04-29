using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace SchoolManagementSystem.Helpers.Pdf;

public class PlainPdfGenerator : IPdfGenerator
{
    public byte[] GenerateReportCard(string title, IReadOnlyDictionary<string, string> fields)
    {
        using var stream = new MemoryStream();
        using var writer = new PdfWriter(stream);
        using var pdf = new PdfDocument(writer);
        using var document = new Document(pdf);

        document.Add(new Paragraph(title)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(18)
            .SetBold());

        var table = new Table(UnitValue.CreatePercentArray([35f, 65f]))
            .UseAllAvailableWidth();

        foreach (var field in fields)
        {
            table.AddCell(new Cell().Add(new Paragraph(field.Key).SetBold()));
            table.AddCell(new Cell().Add(new Paragraph(field.Value)));
        }

        document.Add(table);
        document.Close();
        return stream.ToArray();
    }
}
