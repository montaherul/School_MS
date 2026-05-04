using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers;

public class AdmissionController : Controller
{
    private readonly IAdmissionService _admissionService;
    private readonly SchoolDbContext _db;

    public AdmissionController(IAdmissionService admissionService, SchoolDbContext db)
    {
        _admissionService = admissionService;
        _db = db;
    }

    // ── Public Access: Application ──────────────────────────────────────────

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Apply()
    {
        ViewBag.Classes = _db.Classes
            .Where(c => c.Name != "Class Ten")
            .Select(c => new { c.Id, c.Name })
            .ToList();
        return View(new AdmissionCreateDto { DateOfBirth = DateTime.Today.AddYears(-6) });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(AdmissionCreateDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Classes = _db.Classes.Select(c => new { c.Id, c.Name }).ToList();
            return View(model);
        }

        try
        {
            var applicationNo = await _admissionService.SubmitAsync(model, "Public");
            TempData["SuccessMessage"] = $"Your application ({applicationNo}) has been submitted successfully. A confirmation email has been sent.";
            return RedirectToAction(nameof(ApplySuccess), new { applicationNo });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Classes = _db.Classes.Select(c => new { c.Id, c.Name }).ToList();
            return View(model);
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ApplySuccess(string applicationNo)
    {
        ViewBag.ApplicationNo = applicationNo;
        return View();
    }

    // ── Admin Access: Management ─────────────────────────────────────────────

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin")]
    public async Task<IActionResult> Index()
    {
        ViewBag.Classes = await _db.Classes
            .Where(c => c.Name != "Class Ten")
            .Select(c => new { c.Id, c.Name })
            .ToListAsync();
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, int? classId = null)
    {
        var query = _db.Admissions.Where(a => !a.IsDeleted);

        if (classId.HasValue && classId.Value > 0)
            query = query.Where(a => a.AppliedClassId == classId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a =>
                a.ApplicantName.Contains(search) ||
                a.ApplicationNo.Contains(search) ||
                a.FatherOrGuardianMobileNo.Contains(search));

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(a => new
            {
                a.Id,
                a.ApplicationNo,
                a.ApplicantName,
                a.DateOfBirth,
                a.Gender,
                a.FatherOrGuardianMobileNo,
                a.AppliedClassId,
                Status = a.Status.ToString(),
                a.FatherName,
                a.FatherOccupation,
                a.MotherName,
                a.MotherOccupation,
                a.GuardianName,
                a.GuardianOccupation,
                a.ApplicantMobileNumber,
                a.AlternativeNumber,
                a.ApplicantEmail,
                a.Nationality,
                a.Religion,
                a.BloodGroup,
                a.NationalIdNo,
                a.BirthCertificateNo,
                a.PassportNo,
                a.PaymentMethod,
                a.TransactionDetails,
                a.PresentVillage,
                a.PresentPostOffice,
                a.PresentThana,
                a.PresentDistrict,
                a.PermanentVillage,
                a.PermanentPostOffice,
                a.PermanentThana,
                a.PermanentDistrict,
                a.ProfilePicturePath
            })
            .ToListAsync();

        return Json(new { data = items, last_page = Math.Ceiling((double)totalItems / size) });
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin")]
    public IActionResult CreateEdit(int? id)
    {
        ViewBag.Classes = _db.Classes
            .Where(c => c.Name != "Class Ten")
            .Select(c => new { c.Id, c.Name })
            .ToList();

        if (id.HasValue && id > 0)
        {
            var application = _db.Admissions.FirstOrDefault(a => a.Id == id.Value && !a.IsDeleted);
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
    [Authorize(Roles = "Admin,Super Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(int? id, AdmissionCreateDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Classes = _db.Classes.Select(c => new { c.Id, c.Name }).ToList();
            ViewBag.IsEdit = id.HasValue && id > 0;
            ViewBag.Id = id;
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            if (id.HasValue && id > 0)
            {
                await _admissionService.UpdateAsync(id.Value, model, userId);
                TempData["SuccessMessage"] = "Admission modified successfully.";
            }
            else
            {
                await _admissionService.SubmitAsync(model, userId);
                TempData["SuccessMessage"] = "Admission application submitted successfully.";
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Classes = _db.Classes.Select(c => new { c.Id, c.Name }).ToList();
            ViewBag.IsEdit = id.HasValue && id > 0;
            ViewBag.Id = id;
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(AdmissionCreateDto model, int? id) => CreateEdit(id, model);

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin")]
    public async Task<IActionResult> Details(int id)
    {
        var application = await _db.Admissions
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

        if (application == null) return NotFound();
        return View(application);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var application = await _db.Admissions
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

        if (application == null) return NotFound();
        return View(application);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var application = await _db.Admissions.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (application == null) return NotFound();

        application.IsDeleted = true;
        application.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        application.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Admission application deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve([FromBody] AdmissionApproveRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _admissionService.ApproveAndConvertAsync(request.Id, request.SectionId, userId);
            return Json(new { success = true, message = "Application converted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _admissionService.RejectAsync(id, userId);
            return Json(new { success = true, message = "Application rejected successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin")]
    public async Task<IActionResult> GetClassSections(int classId)
    {
        var sections = await _db.Sections
            .Where(s => s.SchoolClassId == classId)
            .Select(s => new { s.Id, s.Name })
            .ToListAsync();
        return Json(sections);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSectionAjax(int schoolClassId, string name)
    {
        try
        {
            var section = new SchoolManagementSystem.Models.Entities.Academic.Section
            {
                SchoolClassId = schoolClassId,
                Name = name.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System"
            };
            _db.Sections.Add(section);
            await _db.SaveChangesAsync();
            return Json(new { success = true, id = section.Id, name = section.Name });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private static AdmissionCreateDto MapToDto(AdmissionApplication a) => new()
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
        PassportNo = a.PassportNo,
        NationalIdNo = a.NationalIdNo,
        BirthCertificateNo = a.BirthCertificateNo,
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
        ProfilePicturePath = a.ProfilePicturePath
    };
}