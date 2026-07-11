using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Admission;

[Authorize]
public class AdmissionController : Controller
{
    private readonly IAdmissionService _admissionService;
    private readonly ISectionService _sectionService;
    private readonly IAdmissionDashboardService _admissionDashboardService;
    private readonly IAdmissionFinanceService _admissionFinanceService;
    private readonly IDocumentVerificationService _documentVerificationService;
    private readonly IAdmissionReportService _admissionReportService;
    private readonly ILogger<AdmissionController> _logger;

    public AdmissionController(
        IAdmissionService admissionService,
        ISectionService sectionService,
        IAdmissionDashboardService admissionDashboardService,
        IAdmissionFinanceService admissionFinanceService,
        IDocumentVerificationService documentVerificationService,
        IAdmissionReportService admissionReportService,
        ILogger<AdmissionController> logger)
    {
        _admissionService = admissionService;
        _sectionService = sectionService;
        _admissionDashboardService = admissionDashboardService;
        _admissionFinanceService = admissionFinanceService;
        _documentVerificationService = documentVerificationService;
        _admissionReportService = admissionReportService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Apply(CancellationToken ct)
    {
        ViewBag.Classes = await _admissionService.GetAvailableClassesAsync(ct);
        ViewBag.StudentGroups = await _admissionService.GetActiveStudentGroupsAsync(ct);
        ViewBag.GroupStartsFromClassId = await _admissionService.GetGroupStartThresholdAsync(ct);
        return View(new AdmissionCreateDto { DateOfBirth = DateTime.Today.AddYears(-6) });
    }

    [HttpGet("ApplySuccess")]
    [AllowAnonymous]
    public IActionResult ApplySuccess()
    {
        ViewBag.ApplicationNo = TempData["ApplicationNo"] as string;
        if (string.IsNullOrEmpty(ViewBag.ApplicationNo))
            return RedirectToAction("Apply");
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("AdmissionApply")]
    public async Task<IActionResult> Apply(AdmissionCreateDto model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Classes = await _admissionService.GetAvailableClassesAsync(ct);
            ViewBag.StudentGroups = await _admissionService.GetActiveStudentGroupsAsync(ct);
            ViewBag.GroupStartsFromClassId = await _admissionService.GetGroupStartThresholdAsync(ct);
            return View(model);
        }

        try
        {
            // Anonymous applications are attributed to "Public_Applicant" or "System"
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Public_Applicant";
            string applicationNo = await _admissionService.SubmitAsync(model, userId, ct);
            TempData["ApplicationNo"] = applicationNo;
            return RedirectToAction(nameof(ApplySuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Admission Apply failed");
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Classes = await _admissionService.GetAvailableClassesAsync(ct);
            ViewBag.StudentGroups = await _admissionService.GetActiveStudentGroupsAsync(ct);
            return View(model);
        }
    }

    [HttpGet]
    [RequirePermission("Admission.View")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, int? classId = null, string? status = null, CancellationToken ct = default)
    {
        bool isAjax = Request.Headers["Accept"].ToString().Contains("application/json") || Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Query.ContainsKey("page");

        if (isAjax)
        {
            int? statusValue = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<AdmissionStatus>(status, ignoreCase: true, out var parsedStatus))
                statusValue = (int)parsedStatus;

            var (items, totalRecords, counts) = await _admissionService.GetListByStoredProcedureAsync(
                pageNumber: Math.Max(page, 1),
                pageSize: Math.Clamp(pageSize, 5, 100),
                searchTerm: search,
                classId: classId ?? 0,
                cancellationToken: ct,
                status: statusValue
            );

            return Json(new
            {
                data = items,
                last_page = Math.Ceiling((double)totalRecords / Math.Max(pageSize, 1)),
                total_records = totalRecords,
                counts = counts
            });
        }

        ViewBag.Classes = await _admissionService.GetAvailableClassesAsync(ct);
        return View();
    }

    [HttpGet]
    [RequirePermission("Admission.Create")]
    public async Task<IActionResult> CreateEdit(int? id, CancellationToken ct)
    {
        ViewBag.Classes = await _admissionService.GetAvailableClassesAsync(ct);

        if (id.HasValue && id > 0)
        {
            var application = await _admissionService.GetByIdAsync(id.Value, ct);
            if (application == null) return NotFound();

            var dto = MapToDto(application);
            ViewBag.IsEdit = true;
            ViewBag.Id = id.Value;
            return View(dto);
        }

        ViewBag.IsEdit = false;
        return View(new AdmissionCreateDto { DateOfBirth = DateTime.Today.AddYears(-6) });
    }

    [HttpPost]
    [RequirePermission("Admission.Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(int? id, AdmissionCreateDto model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Classes = await _admissionService.GetAvailableClassesAsync(ct);
            ViewBag.IsEdit = id.HasValue && id > 0;
            ViewBag.Id = id;
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            if (id.HasValue && id > 0)
            {
                await _admissionService.UpdateAsync(id.Value, model, userId, ct);
                TempData["SuccessMessage"] = "Admission modified successfully.";
            }
            else
            {
                await _admissionService.SubmitAsync(model, userId, ct);
                TempData["SuccessMessage"] = "Admission application submitted successfully.";
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Classes = await _admissionService.GetAvailableClassesAsync(ct);
            ViewBag.IsEdit = id.HasValue && id > 0;
            ViewBag.Id = id;
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Admission.View")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var application = await _admissionService.GetByIdAsync(id, ct);
        if (application == null) return NotFound();
        var classes = await _admissionService.GetAvailableClassesAsync(ct);
        ViewBag.ClassLookup = classes.ToDictionary(
            c => (int)c.GetType().GetProperty("Id")!.GetValue(c)!,
            c => (string)c.GetType().GetProperty("Name")!.GetValue(c)!);
        return View(application);
    }

    [HttpGet]
    [RequirePermission("Admission.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var application = await _admissionService.GetByIdAsync(id, ct);
        if (application == null) return NotFound();
        return View(application);
    }

    [HttpPost]
    [RequirePermission("Admission.Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        await _admissionService.DeleteAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System", ct);
        TempData["SuccessMessage"] = "Admission application deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [RequirePermission("Admission.Approve")]
    public async Task<IActionResult> Approve([FromBody] AdmissionApproveRequest request, CancellationToken ct)
    {
        if (request is null || request.Id <= 0 || request.SectionId <= 0)
            return Json(new { success = false, message = "Invalid request." });

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _admissionService.ApproveAndConvertAsync(request.Id, request.SectionId, userId, ct);
            TempData["SuccessMessage"] = "Application approved and converted successfully. Invoice generated.";
            return Json(new { success = true, message = "Application converted successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Approval failed for admission {Id}", request?.Id);
            return Json(new { success = false, message = "An unexpected error occurred. Please try again." });
        }
    }

    [HttpPost]
    [RequirePermission("Admission.Approve")]
    public async Task<IActionResult> Reject(int id, [FromForm] string? rejectionReason, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _admissionService.RejectAsync(id, userId, rejectionReason, ct);
            return Json(new { success = true, message = "Application rejected successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rejection failed for admission {Id}", id);
            return Json(new { success = false, message = "An unexpected error occurred. Please try again." });
        }
    }

    [HttpGet]
    [RequirePermission("Admission.View")]
    public async Task<IActionResult> GetClassSections(int classId, CancellationToken ct)
    {
        var result = await _sectionService.GetAdmissionSectionsAsync(classId, ct);
        return Json(result);
    }

    [HttpPost]
    [RequirePermission("Admission.Create")]
    public async Task<IActionResult> CreateSectionAjax(int schoolClassId, string name, int? parentSectionId = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Json(new { success = false, message = "Section name is required." });

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            int id = await _sectionService.CreateAjaxAsync(schoolClassId, name.Trim(), parentSectionId, userId, ct);
            return Json(new { success = true, id = id, name = name.Trim() });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateSection failed for class {ClassId}", schoolClassId);
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }
    }

    [HttpPost]
    [RequirePermission("Admission.Approve")]
    public async Task<IActionResult> BulkApprove([FromBody] BulkAssignRequest request, CancellationToken ct)
    {
        if (request?.Ids == null || !request.Ids.Any())
            return Json(new { success = false, message = "No applications selected." });

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var progress = await _admissionService.BulkApproveAsync(request.Ids, request.SectionId ?? 0, userId, ct);
            return Json(new { success = true, message = $"{progress.Succeeded} approved, {progress.Failed} failed.", errors = progress.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk approve failed");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Admission.Approve")]
    public async Task<IActionResult> BulkReject([FromBody] BulkIdsRequest request, CancellationToken ct)
    {
        if (request?.Ids == null || !request.Ids.Any())
            return Json(new { success = false, message = "No applications selected." });

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var progress = await _admissionService.BulkRejectAsync(request.Ids, userId, null, ct);
            return Json(new { success = true, message = $"{progress.Succeeded} rejected, {progress.Failed} failed." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk reject failed");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Admission.Delete")]
    public async Task<IActionResult> BulkDelete([FromBody] BulkIdsRequest request, CancellationToken ct)
    {
        if (request?.Ids == null || !request.Ids.Any())
            return Json(new { success = false, message = "No applications selected." });

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var progress = await _admissionService.BulkDeleteAsync(request.Ids, userId, ct);
            return Json(new { success = true, message = $"{progress.Succeeded} deleted, {progress.Failed} failed." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk delete failed");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Admission.Delete")]
    public async Task<IActionResult> BulkRestore([FromBody] BulkIdsRequest request, CancellationToken ct)
    {
        if (request?.Ids == null || !request.Ids.Any())
            return Json(new { success = false, message = "No applications selected." });

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var progress = await _admissionService.BulkRestoreAsync(request.Ids, userId, ct);
            return Json(new { success = true, message = $"{progress.Succeeded} restored, {progress.Failed} failed." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk restore failed");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Admission.Export")]
    public async Task<IActionResult> BulkExportExcel([FromBody] BulkIdsRequest? request, CancellationToken ct)
    {
        try
        {
            var data = await _admissionService.BulkExportExcelAsync(request?.Ids, ct);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"admissions_bulk_{DateTime.UtcNow:yyyyMMdd}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk export failed");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [RequirePermission("Admission.View")]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        return View();
    }

    [HttpGet]
    [RequirePermission("Admission.View")]
    public async Task<IActionResult> GetDashboardData(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        try
        {
            var data = await _admissionDashboardService.GetDashboardAsync(dateFrom, dateTo, ct);
            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load admission dashboard data");
            return Json(new { success = false, message = "Failed to load dashboard data." });
        }
    }

    [HttpGet]
    [RequirePermission("Admission.View")]
    public async Task<IActionResult> Documents(int id, CancellationToken ct)
    {
        var application = await _admissionService.GetByIdAsync(id, ct);
        if (application == null) return NotFound();

        var docs = await _documentVerificationService.GetDocumentsByApplicationAsync(id, ct);
        ViewBag.Application = application;
        return View(docs);
    }

    [HttpPost]
    [RequirePermission("Admission.Verify")]
    public async Task<IActionResult> VerifyDocument([FromBody] DocumentVerificationRequest request, CancellationToken ct)
    {
        if (request == null || request.DocumentId <= 0)
            return Json(new { success = false, message = "Invalid request." });

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var doc = await _documentVerificationService.VerifyDocumentAsync(request.DocumentId, request.Status, userId, request.Remarks, ct);
            return Json(new { success = true, message = $"Document marked as {request.Status}.", data = doc });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Admission.Verify")]
    public async Task<IActionResult> UploadDocument(int applicationId, [FromForm] string documentType, [FromForm] IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return Json(new { success = false, message = "No file provided." });

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var doc = await _documentVerificationService.UploadDocumentAsync(applicationId, documentType, file, userId, ct);
            return Json(new { success = true, message = "Document uploaded.", data = doc });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Admission.Verify")]
    public async Task<IActionResult> RequestReUpload(int documentId, string? remarks, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _documentVerificationService.RequestReUploadAsync(documentId, userId, remarks, ct);
            return Json(new { success = true, message = "Re-upload requested." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("Finance")]
    [RequirePermission("Admission.Finance")]
    public async Task<IActionResult> Finance(CancellationToken ct)
    {
        var summaries = await _admissionFinanceService.GetAllFeeSummariesAsync(ct);
        return View(summaries);
    }

    [HttpGet("Finance/{id:int}")]
    [RequirePermission("Admission.Finance")]
    public async Task<IActionResult> FinanceDetail(int id, CancellationToken ct)
    {
        var application = await _admissionService.GetByIdAsync(id, ct);
        if (application == null) return NotFound();

        var summary = await _admissionFinanceService.GetFeeSummaryAsync(id, ct);
        ViewBag.Application = application;
        return View(summary);
    }

    [HttpPost]
    [RequirePermission("Admission.Finance")]
    public async Task<IActionResult> RecordPayment([FromBody] AdmissionFeePaymentRequest request, CancellationToken ct)
    {
        if (request == null || request.ApplicationId <= 0)
            return Json(new { success = false, message = "Invalid request." });

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var payment = await _admissionFinanceService.RecordPaymentAsync(request, userId, ct);
            return Json(new { success = true, message = "Payment recorded successfully.", data = payment });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Admission.Finance")]
    public async Task<IActionResult> ApplyScholarship(int applicationId, decimal percentage, string? description, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _admissionFinanceService.ApplyScholarshipAsync(applicationId, percentage, description, userId, ct);
            return Json(new { success = true, message = $"Scholarship of {percentage}% applied." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Admission.Finance")]
    public async Task<IActionResult> ProcessRefund(int applicationId, decimal amount, string reason, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _admissionFinanceService.ProcessRefundAsync(applicationId, amount, reason, userId, ct);
            return Json(new { success = true, message = "Refund processed successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [RequirePermission("Admission.View")]
    public async Task<IActionResult> Reports(CancellationToken ct)
    {
        ViewBag.Classes = await _admissionService.GetAvailableClassesAsync(ct);
        return View();
    }

    [HttpGet]
    [RequirePermission("Admission.View")]
    public async Task<IActionResult> RegisterReport(CancellationToken ct)
    {
        ViewBag.Classes = await _admissionService.GetAvailableClassesAsync(ct);
        return View();
    }

    [HttpGet]
    [RequirePermission("Admission.View")]
    public async Task<IActionResult> Analytics(CancellationToken ct)
    {
        ViewBag.Classes = await _admissionService.GetAvailableClassesAsync(ct);
        return View();
    }

    [HttpPost]
    [RequirePermission("Admission.Export")]
    public async Task<IActionResult> GetRegisterReport([FromBody] AdmissionReportRequest request, CancellationToken ct)
    {
        try
        {
            var report = await _admissionDashboardService.GetRegisterReportAsync(request, ct);
            return Json(new { success = true, data = report });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate register report");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Admission.Export")]
    public async Task<IActionResult> ExportReportExcel([FromBody] AdmissionReportRequest request, CancellationToken ct)
    {
        try
        {
            var data = await _admissionReportService.ExportRegisterToExcelAsync(request, ct);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"admission_register_{DateTime.UtcNow:yyyyMMdd}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export report");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [RequirePermission("Admission.View")]
    public async Task<IActionResult> GetConversionFunnel(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        try
        {
            var funnel = await _admissionDashboardService.GetConversionFunnelAsync(dateFrom, dateTo, ct);
            return Json(new { success = true, data = funnel });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get conversion funnel");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [RequirePermission("Admission.View")]
    public async Task<IActionResult> GetTrendAnalysis(DateTime? dateFrom = null, DateTime? dateTo = null, string? groupBy = "Month", CancellationToken ct = default)
    {
        try
        {
            var data = await _admissionDashboardService.GetTrendAnalysisAsync(dateFrom, dateTo, groupBy, ct);
            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get trend analysis");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [RequirePermission("Admission.View")]
    public async Task<IActionResult> GetClassDemand(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        try
        {
            var data = await _admissionDashboardService.GetClassDemandAsync(dateFrom, dateTo, ct);
            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get class demand");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [RequirePermission("Admission.View")]
    public async Task<IActionResult> GetRevenueReport(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        try
        {
            var data = await _admissionDashboardService.GetRevenueReportAsync(dateFrom, dateTo, ct);
            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get revenue report");
            return Json(new { success = false, message = ex.Message });
        }
    }

    private static AdmissionCreateDto MapToDto(SchoolManagementSystem.Models.Entities.Admission.AdmissionApplication a) => new()
    {
        ApplicantName = a.ApplicantName,
        ApplicantNameBangla = a.ApplicantNameBangla,
        DateOfBirth = a.DateOfBirth,
        Gender = a.Gender,
        FatherName = a.FatherName,
        FatherOccupation = a.FatherOccupation,
        MotherName = a.MotherName,
        MotherOccupation = a.MotherOccupation,
        GuardianName = a.GuardianName,
        GuardianOccupation = a.GuardianOccupation,
        ApplicantMobileNumber = a.ApplicantMobileNumber,
        AlternativeNumber = a.AlternativeNumber,
        FatherOrGuardianMobileNo = a.FatherOrGuardianMobileNo,
        ApplicantEmail = a.ApplicantEmail,
        Nationality = a.Nationality,
        Country = a.Country,
        MaritalStatus = a.MaritalStatus,
        Religion = a.Religion,
        BloodGroup = a.BloodGroup,
        BirthCertificateNo = a.BirthCertificateNo,
        BirthCertificatePath = a.BirthCertificatePath,
        PaymentSlipPath = a.PaymentSlipPath,
        PaymentMethod = a.PaymentMethod,
        TransactionDetails = a.TransactionDetails,
        PresentVillage = a.PresentVillage,
        PresentPostOffice = a.PresentPostOffice,
        PresentThana = a.PresentThana,
        PresentDistrict = a.PresentDistrict,
        PermanentVillage = a.PermanentVillage,
        PermanentPostOffice = a.PermanentPostOffice,
        PermanentThana = a.PermanentThana,
        PermanentDistrict = a.PermanentDistrict,
        AppliedClassId = a.AppliedClassId,
        ProfilePicturePath = a.ProfilePicturePath,
        GuardianPhotoPath = a.GuardianPhoto
    };
}

