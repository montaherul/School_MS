using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Helpers.Reports
{
    /// <summary>
    /// Minimal XLSX writer that produces Microsoft Excel-compatible Office Open XML
    /// SpreadsheetML files without an external dependency. The output is a real .xlsx
    /// archive containing the standard sheet/workbook parts.
    /// </summary>
    public static class SimpleExcelWriter
    {
        public static byte[] WriteWorkbook(string sheetName, IEnumerable<string[]> rows)
        {
            if (string.IsNullOrWhiteSpace(sheetName))
            {
                sheetName = "Sheet1";
            }

            using var ms = new MemoryStream();
            using (var archive = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Create, true))
            {
                WriteEntry(archive, "[Content_Types].xml", GetContentTypesXml());
                WriteEntry(archive, "_rels/.rels", GetRootRelsXml());
                WriteEntry(archive, "xl/workbook.xml", GetWorkbookXml(sheetName));
                WriteEntry(archive, "xl/_rels/workbook.xml.rels", GetWorkbookRelsXml());
                WriteEntry(archive, "xl/worksheets/sheet1.xml", GetSheetXml(rows));
                WriteEntry(archive, "xl/styles.xml", GetStylesXml());
                WriteEntry(archive, "xl/sharedStrings.xml", GetSharedStringsXml(rows));
            }
            return ms.ToArray();
        }

        private static void WriteEntry(System.IO.Compression.ZipArchive archive, string entryName, string content)
        {
            var entry = archive.CreateEntry(entryName, System.IO.Compression.CompressionLevel.Optimal);
            using var stream = entry.Open();
            var bytes = Encoding.UTF8.GetBytes(content);
            stream.Write(bytes, 0, bytes.Length);
        }

        private static string Escape(string value)
        {
            if (value == null) return string.Empty;
            return value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

        private static string GetContentTypesXml()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                   "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">" +
                   "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
                   "<Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
                   "<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>" +
                   "<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>" +
                   "<Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/>" +
                   "<Override PartName=\"/xl/sharedStrings.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml\"/>" +
                   "</Types>";
        }

        private static string GetRootRelsXml()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                   "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                   "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/>" +
                   "</Relationships>";
        }

        private static string GetWorkbookXml(string sheetName)
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                   "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                   "<sheets>" +
                   $"<sheet name=\"{Escape(sheetName)}\" sheetId=\"1\" r:id=\"rId1\"/>" +
                   "</sheets>" +
                   "</workbook>";
        }

        private static string GetWorkbookRelsXml()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                   "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                   "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/>" +
                   "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>" +
                   "<Relationship Id=\"rId3\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings\" Target=\"sharedStrings.xml\"/>" +
                   "</Relationships>";
        }

        private static string GetStylesXml()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                   "<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">" +
                   "<fonts count=\"1\"><font><sz val=\"11\"/><name val=\"Calibri\"/></font></fonts>" +
                   "<fills count=\"1\"><fill><patternFill patternType=\"none\"/></fill></fills>" +
                   "<borders count=\"1\"><border/></borders>" +
                   "<cellStyleXfs count=\"1\"><xf/></cellStyleXfs>" +
                   "<cellXfs count=\"1\"><xf/></cellXfs>" +
                   "</styleSheet>";
        }

        private static string GetSharedStringsXml(IEnumerable<string[]> rows)
        {
            var unique = new Dictionary<string, int>();
            int counter = 0;
            var list = new List<string>();

            foreach (var row in rows)
            {
                if (row == null) continue;
                foreach (var cell in row)
                {
                    var text = cell ?? string.Empty;
                    if (!unique.ContainsKey(text))
                    {
                        unique[text] = counter++;
                        list.Add(text);
                    }
                }
            }

            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append($"<sst xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" count=\"{list.Count}\" uniqueCount=\"{list.Count}\">");
            foreach (var s in list)
            {
                sb.Append($"<si><t xml:space=\"preserve\">{Escape(s)}</t></si>");
            }
            sb.Append("</sst>");
            return sb.ToString();
        }

        private static string GetSheetXml(IEnumerable<string[]> rows)
        {
            var shared = new Dictionary<string, int>();
            int counter = 0;
            foreach (var row in rows)
            {
                if (row == null) continue;
                foreach (var cell in row)
                {
                    var text = cell ?? string.Empty;
                    if (!shared.ContainsKey(text))
                    {
                        shared[text] = counter++;
                    }
                }
            }

            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");
            sb.Append("<sheetData>");

            int rowIndex = 1;
            foreach (var row in rows)
            {
                if (row == null) continue;
                sb.Append($"<row r=\"{rowIndex}\">");
                int colIndex = 1;
                foreach (var cell in row)
                {
                    var text = cell ?? string.Empty;
                    var colLetter = ColumnLetter(colIndex);
                    var ssIndex = shared[text];
                    sb.Append($"<c r=\"{colLetter}{rowIndex}\" t=\"s\"><v>{ssIndex}</v></c>");
                    colIndex++;
                }
                sb.Append("</row>");
                rowIndex++;
            }
            sb.Append("</sheetData></worksheet>");
            return sb.ToString();
        }

        private static string ColumnLetter(int colIndex)
        {
            string result = string.Empty;
            while (colIndex > 0)
            {
                int rem = (colIndex - 1) % 26;
                result = (char)('A' + rem) + result;
                colIndex = (colIndex - 1) / 26;
            }
            return result;
        }
    }
}
