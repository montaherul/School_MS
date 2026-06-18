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
    private readonly ISchoolClassService _classService;
    private readonly ISectionService _sectionService;

    public AdmissionController(
        IAdmissionService admissionService,
        ISchoolClassService classService,
        ISectionService sectionService)
    {
        _admissionService = admissionService;
        _classService = classService;
        _sectionService = sectionService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Apply(CancellationToken ct)
    {
        ViewBag.Classes = await _admissionService.GetAvailableClassesAsync(ct);
        ViewBag.StudentGroups = await _admissionService.GetActiveStudentGroupsAsync(ct);
        return View(new AdmissionCreateDto { DateOfBirth = DateTime.Today.AddYears(-6) });
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
            return View(model);
        }

        try
        {
            // Anonymous applications are attributed to "Public_Applicant" or "System"
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Public_Applicant";
            string applicationNo = await _admissionService.SubmitAsync(model, userId, ct);
            ViewBag.ApplicationNo = applicationNo;
            return View("ApplySuccess");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Classes = await _admissionService.GetAvailableClassesAsync(ct);
            ViewBag.StudentGroups = await _admissionService.GetActiveStudentGroupsAsync(ct);
            return View(model);
        }
    }

    [RequirePermission("Admission.View")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, int? classId = null, string? status = null, CancellationToken ct = default)
    {
        bool isAjax = Request.Headers["Accept"].ToString().Contains("application/json") || Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Query.ContainsKey("page");

        if (isAjax)
        {
            int? statusValue = !string.IsNullOrEmpty(status) ? (int)Enum.Parse(typeof(AdmissionStatus), status) : null;

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
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("Admission.Create")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve([FromBody] AdmissionApproveRequest request, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _admissionService.ApproveAndConvertAsync(request.Id, request.SectionId, userId, ct);
            return Json(new { success = true, message = "Application converted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Admission.Approve")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _admissionService.RejectAsync(id, userId, ct);
            return Json(new { success = true, message = "Application rejected successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSectionAjax(int schoolClassId, string name, int? parentSectionId = null, CancellationToken ct = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            int id = await _sectionService.CreateAjaxAsync(schoolClassId, name.Trim(), parentSectionId, userId, ct);
            return Json(new { success = true, id = id, name = name.Trim() });
        }
        catch (Exception ex)
        {
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

