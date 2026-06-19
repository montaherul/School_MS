using ClosedXML.Excel;
using System.Net;
using System.Text;

namespace SchoolManagementSystem.Helpers.Reports;

public static class FeeListExporter
{
    public static byte[] ExportToExcel<T>(List<T> data, string sheetName)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(sheetName.Length > 31 ? sheetName[..31] : sheetName);
        var properties = typeof(T).GetProperties().Where(p => p.Name != "TotalRecords").ToArray();
        for (int i = 0; i < properties.Length; i++)
            ws.Cell(1, i + 1).Value = properties[i].Name;
        for (int r = 0; r < data.Count; r++)
            for (int c = 0; c < properties.Length; c++)
            {
                var val = properties[c].GetValue(data[r]);
                ws.Cell(r + 2, c + 1).Value = val?.ToString() ?? "";
            }
        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    public static string BuildExportHtml<T>(List<T> data, string title)
    {
        var props = typeof(T).GetProperties().Where(p => p.Name != "TotalRecords").ToArray();
        var sb = new StringBuilder();
        sb.Append("<!DOCTYPE html><html><head><meta charset='utf-8'/><style>");
        sb.Append("body{font-family:Arial,sans-serif;padding:30px;color:#333}");
        sb.Append("h2{text-align:center;color:#1a56db;margin-bottom:20px}");
        sb.Append("table{width:100%;border-collapse:collapse;font-size:12px}");
        sb.Append("th{background:#1a56db;color:#fff;padding:8px 10px;text-align:left;font-weight:600}");
        sb.Append("td{padding:6px 10px;border-bottom:1px solid #e2e8f0}");
        sb.Append("tr:nth-child(even){background:#f8fafc}");
        sb.Append("</style></head><body>");
        sb.Append($"<h2>{WebUtility.HtmlEncode(title)}</h2>");
        sb.Append("<table><thead><tr>");
        foreach (var p in props)
            sb.Append($"<th>{WebUtility.HtmlEncode(p.Name)}</th>");
        sb.Append("</tr></thead><tbody>");
        foreach (var item in data)
        {
            sb.Append("<tr>");
            foreach (var p in props)
            {
                var val = p.GetValue(item)?.ToString() ?? "";
                sb.Append($"<td>{WebUtility.HtmlEncode(val)}</td>");
            }
            sb.Append("</tr>");
        }
        sb.Append("</tbody></table></body></html>");
        return sb.ToString();
    }
}
