using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeReportService : IFeeReportService
{
    private readonly IFeeReportRepository _repository;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IUnitOfWork _uow;

    public FeeReportService(IFeeReportRepository repository, IPdfGenerator pdfGenerator, IUnitOfWork uow)
    {
        _repository = repository;
        _pdfGenerator = pdfGenerator;
        _uow = uow;
    }

    public async Task<PagedResult<StudentLedgerReportDto>> GetStudentLedgerReportAsync(int studentId, int page, int pageSize)
    {
        var (items, total) = await _repository.GetStudentLedgerReportAsync(studentId, Math.Max(page, 1), Math.Clamp(pageSize, 5, 100));
        return new PagedResult<StudentLedgerReportDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<PagedResult<DailyCollectionReportDto>> GetDailyCollectionReportAsync(DateOnly date, int page, int pageSize)
    {
        var (items, total) = await _repository.GetDailyCollectionReportAsync(date, Math.Max(page, 1), Math.Clamp(pageSize, 5, 100));
        return new PagedResult<DailyCollectionReportDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<PagedResult<MonthlyCollectionReportDto>> GetMonthlyCollectionReportAsync(int year, int page, int pageSize)
    {
        var (items, total) = await _repository.GetMonthlyCollectionReportAsync(year, Math.Max(page, 1), Math.Clamp(pageSize, 5, 100));
        return new PagedResult<MonthlyCollectionReportDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<PagedResult<DueReportDto>> GetDueReportAsync(int page, int pageSize, int classId = 0)
    {
        var (items, total) = await _repository.GetDueReportAsync(Math.Max(page, 1), Math.Clamp(pageSize, 5, 100), classId);
        return new PagedResult<DueReportDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<PagedResult<DiscountReportDto>> GetDiscountReportAsync(int page, int pageSize)
    {
        var (items, total) = await _repository.GetDiscountReportAsync(Math.Max(page, 1), Math.Clamp(pageSize, 5, 100));
        return new PagedResult<DiscountReportDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<PagedResult<WaiverReportDto>> GetWaiverReportAsync(int page, int pageSize)
    {
        var (items, total) = await _repository.GetWaiverReportAsync(Math.Max(page, 1), Math.Clamp(pageSize, 5, 100));
        return new PagedResult<WaiverReportDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<PagedResult<RefundReportDto>> GetRefundReportAsync(int page, int pageSize)
    {
        var (items, total) = await _repository.GetRefundReportAsync(Math.Max(page, 1), Math.Clamp(pageSize, 5, 100));
        return new PagedResult<RefundReportDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<PagedResult<ClassCollectionSummaryDto>> GetClassCollectionSummaryAsync(int academicYearId, int page, int pageSize)
    {
        var (items, total) = await _repository.GetClassCollectionSummaryAsync(academicYearId, Math.Max(page, 1), Math.Clamp(pageSize, 5, 100));
        return new PagedResult<ClassCollectionSummaryDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<CashBookResultDto> GetCashBookAsync(DateOnly fromDate, DateOnly toDate, int? academicYearId = null)
    {
        return await _repository.GetCashBookAsync(fromDate, toDate, academicYearId);
    }

    public async Task<List<AcademicYearOptionDto>> GetAcademicYearOptionsAsync()
    {
        return await _uow.Repository<AcademicYear>().Query()
            .Where(y => !y.IsDeleted)
            .OrderByDescending(y => y.StartsOn)
            .Select(y => new AcademicYearOptionDto
            {
                Id = y.Id,
                Name = y.Name,
                StartsOn = DateOnly.FromDateTime(y.StartsOn)
            })
            .ToListAsync();
    }

    public async Task<byte[]> ExportToExcelAsync<T>(List<T> data, string reportName)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(reportName.Length > 31 ? reportName[..31] : reportName);
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

    public async Task<byte[]> ExportToPdfAsync<T>(List<T> data, string reportName, string htmlTemplate)
    {
        var html = BuildReportHtml(data, reportName);
        return await Task.FromResult(_pdfGenerator.GenerateFromHtml(html));
    }

    private static string BuildReportHtml<T>(List<T> data, string title)
    {
        var props = typeof(T).GetProperties().Where(p => p.Name != "TotalRecords").ToArray();
        var sb = new System.Text.StringBuilder();
        sb.Append("<html><head><style>");
        sb.Append("body{font-family:Arial,sans-serif;padding:30px;color:#333}");
        sb.Append("h2{text-align:center;color:#1a56db;margin-bottom:20px}");
        sb.Append("table{width:100%;border-collapse:collapse;font-size:12px}");
        sb.Append("th{background:#1a56db;color:#fff;padding:8px 10px;text-align:left;font-weight:600}");
        sb.Append("td{padding:6px 10px;border-bottom:1px solid #e2e8f0}");
        sb.Append("tr:nth-child(even){background:#f8fafc}");
        sb.Append("</style></head><body>");
        sb.Append($"<h2>{System.Net.WebUtility.HtmlEncode(title)}</h2>");
        sb.Append("<table><thead><tr>");
        foreach (var p in props)
            sb.Append($"<th>{System.Net.WebUtility.HtmlEncode(p.Name)}</th>");
        sb.Append("</tr></thead><tbody>");
        foreach (var item in data)
        {
            sb.Append("<tr>");
            foreach (var p in props)
            {
                var val = p.GetValue(item)?.ToString() ?? "";
                sb.Append($"<td>{System.Net.WebUtility.HtmlEncode(val)}</td>");
            }
            sb.Append("</tr>");
        }
        sb.Append("</tbody></table></body></html>");
        return sb.ToString();
    }
}
