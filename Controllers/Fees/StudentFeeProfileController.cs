using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using StudentEntity = SchoolManagementSystem.Models.Entities.Student.Student;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Helpers.Pdf;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
[Route("StudentFeeProfile")]
public class StudentFeeProfileController : Controller
{
    private readonly IStudentFeeProfileService _profileService;
    private readonly IFeeSecurityService _security;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPdfGenerator _pdfGenerator;

    public StudentFeeProfileController(
        IStudentFeeProfileService profileService,
        IFeeSecurityService security,
        IUnitOfWork unitOfWork,
        IPdfGenerator pdfGenerator)
    {
        _profileService = profileService;
        _security = security;
        _unitOfWork = unitOfWork;
        _pdfGenerator = pdfGenerator;
    }

    [RequirePermission("StudentFeeProfile.View")]
    public IActionResult Index()
    {
        return View("~/Views/Fee/StudentFeeProfile/Index.cshtml");
    }

    [HttpGet("{studentId}")]
    [RequirePermission("StudentFeeProfile.View")]
    public async Task<IActionResult> Profile(int studentId, int? academicYearId = null)
    {
        if (!_security.CanAccessStudentData(User, studentId))
            return Forbid();

        var profile = await _profileService.GetProfileAsync(studentId, academicYearId);
        if (profile.StudentId == 0)
            return NotFound();

        return View("~/Views/Fee/StudentFeeProfile/Profile.cshtml", profile);
    }

    [HttpPost("Search")]
    [RequirePermission("StudentFeeProfile.View")]
    public async Task<IActionResult> Search(string? term = null)
    {
        var students = await _unitOfWork.Repository<StudentEntity>().Query()
            .Where(s => !s.IsDeleted && (
                string.IsNullOrEmpty(term) ||
                s.FullName.Contains(term) ||
                s.StudentNo.Contains(term)))
            .Select(s => new
            {
                s.Id,
                s.FullName,
                s.StudentNo,
                ClassName = s.Class.Name,
                SectionName = s.Section.Name
            })
            .Take(20)
            .ToListAsync();

        return Json(students);
    }

    [HttpGet("Export/{studentId}")]
    [RequirePermission("StudentFeeProfile.View")]
    public async Task<IActionResult> Export(int studentId, int? academicYearId = null)
    {
        if (!_security.CanAccessStudentData(User, studentId))
            return Forbid();

        var profile = await _profileService.GetProfileAsync(studentId, academicYearId);
        if (profile.StudentId == 0)
            return NotFound();

        var html = BuildExportHtml(profile);
        var bytes = _pdfGenerator.GenerateFromHtml(html);
        return File(bytes, "application/pdf", $"FeeProfile_{studentId}.pdf");
    }

    private static string BuildExportHtml(StudentFeeProfileDto profile)
    {
        return $@"
<html><head>
<meta charset='utf-8'/>
<style>
body{{font-family:Arial,sans-serif;margin:20px;}}
h1{{font-size:18px;margin-bottom:5px;}}
table{{width:100%;border-collapse:collapse;margin-bottom:15px;}}
th,td{{border:1px solid #ccc;padding:6px 8px;text-align:left;font-size:12px;}}
th{{background:#f0f0f0;}}
.text-right{{text-align:right;}}
.badge{{display:inline-block;padding:2px 8px;border-radius:10px;font-size:11px;background:#eee;}}
.badge-success{{background:#d4edda;}}
.badge-warning{{background:#fff3cd;}}
.badge-danger{{background:#f8d7da;}}
.badge-info{{background:#d1ecf1;}}
</style></head><body>
<h1>Student Fee Profile</h1>
<p>{profile.StudentName} ({profile.StudentCode}) - {profile.ClassName} - {profile.SectionName}</p>
<table>
<tr><th>Total Assigned</th><th>Total Paid</th><th>Total Due</th><th>Total Discount</th><th>Total Waiver</th><th>Late Fee</th></tr>
<tr>
<td class='text-right'>{profile.TotalAssigned:N2}</td>
<td class='text-right'>{profile.TotalPaid:N2}</td>
<td class='text-right'>{profile.TotalDue:N2}</td>
<td class='text-right'>{profile.TotalDiscount:N2}</td>
<td class='text-right'>{profile.TotalWaiver:N2}</td>
<td class='text-right'>{profile.TotalLateFee:N2}</td>
</tr>
</table>
<h3>Invoices ({profile.InvoiceCount})</h3>
<table>
<tr><th>Invoice No</th><th>Due Date</th><th>Total</th><th>Paid</th><th>Discount</th><th>Late Fee</th><th>Status</th></tr>
{string.Join("", profile.Invoices.Select(i => $"<tr><td>{i.InvoiceNo}</td><td>{i.DueDate}</td><td class='text-right'>{i.TotalAmount:N2}</td><td class='text-right'>{i.PaidAmount:N2}</td><td class='text-right'>{i.DiscountAmount:N2}</td><td class='text-right'>{i.LateFee:N2}</td><td>{GetStatusText(i.Status)}</td></tr>"))}
</table>
<h3>Payments ({profile.PaymentCount})</h3>
<table>
<tr><th>Date</th><th>Invoice</th><th>Amount</th><th>Method</th><th>Reference</th></tr>
{string.Join("", profile.Payments.Select(p => $"<tr><td>{p.PaidAt:yyyy-MM-dd}</td><td>{p.InvoiceNo}</td><td class='text-right'>{p.Amount:N2}</td><td>{p.Method}</td><td>{p.ReferenceNo}</td></tr>"))}
</table>
</body></html>";
    }

    private static string GetStatusText(int status)
    {
        return status switch
        {
            1 => "Draft",
            2 => "Partial",
            3 => "Paid",
            4 => "Waived",
            5 => "Issued",
            6 => "Cancelled",
            7 => "Refunded",
            _ => "Draft"
        };
    }
}
