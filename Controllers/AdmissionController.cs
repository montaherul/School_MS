using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using System;
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
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, int? classId = null, string? status = null)
    {
        // Detect AJAX/Tabulator requests: check headers OR presence of pagination query params
        bool isAjax = Request.Headers["Accept"].ToString().Contains("application/json")
                    || Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                    || Request.Query.ContainsKey("page");

        if (isAjax)
        {
            // ✅ ADD THIS BLOCK HERE
            int? statusValue = null;

            if (!string.IsNullOrEmpty(status))
            {
                statusValue = (int)Enum.Parse(typeof(AdmissionStatus), status);
            }

            var (items, totalRecords, counts) = await _admissionService.GetListByStoredProcedureAsync(
                pageNumber: Math.Max(page, 1),
                pageSize: Math.Clamp(pageSize, 5, 100),
                searchTerm: search,
                classId: classId ?? 0,
                cancellationToken: HttpContext.RequestAborted,
                status: statusValue   // ✅ PASS HERE
            );

            return Json(new 
            { 
                data = items.Select(a => new
                {
                    a.Id,
                    a.ApplicationNo,
                    a.ApplicantName,
                    age = a.Age,
                    a.DateOfBirth,
                    a.Gender,
                    a.AppliedClassId,
                    a.ApplicantMobileNumber,
                    a.ApplicantEmail,
                    a.ClassName,
                    a.Status,
                    statusBadgeClass = a.StatusBadgeClass,
                    a.CreatedBy,
                    createdAtFormatted = a.CreatedAtFormatted,
                    a.DaysApplied,
                    a.ProfilePicturePath
                }),
                last_page = Math.Ceiling((double)totalRecords / Math.Max(pageSize, 1)),
                total_records = totalRecords,
                counts = counts
            });
        }

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
        // Load all sections for this class, including parent info
        var allSections = await _db.Sections
            .Where(s => s.SchoolClassId == classId && !s.IsDeleted)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Capacity,
                s.ParentSectionId,
                ParentName = _db.Sections
                    .Where(p => p.Id == s.ParentSectionId)
                    .Select(p => p.Name)
                    .FirstOrDefault(),
                StudentCount = _db.Students.Count(st =>
                    st.SectionId == s.Id && !st.IsDeleted &&
                    st.Status == SchoolManagementSystem.Models.Enums.StudentStatus.Active)
            })
            .ToListAsync();

        // Leaf sections = those WITH a parent (sub-sections), OR those with no children (flat sections like Class 1-8)
        var parentIds = allSections.Where(s => s.ParentSectionId == null)
                                   .Select(s => s.Id)
                                   .ToHashSet();
        var hasChildren = allSections.Any(s => s.ParentSectionId != null);

        List<object> result;
        if (hasChildren)
        {
            // Class 9/10 style: return only leaf sub-sections with groupName
            result = allSections
                .Where(s => s.ParentSectionId != null)
                .Select(s => (object)new
                {
                    id = s.Id,
                    name = s.Name,
                    displayName = $"{s.Name} ({s.StudentCount}/{s.Capacity}){(s.StudentCount >= s.Capacity ? " - FULL" : "")}",
                    groupName = s.ParentName ?? "",
                    parentSectionId = s.ParentSectionId,
                    studentCount = s.StudentCount,
                    capacity = s.Capacity,
                    isFull = s.StudentCount >= s.Capacity
                })
                .ToList();
        }
        else
        {
            // Class 1-8 style: return flat sections
            result = allSections
                .Select(s => (object)new
                {
                    id = s.Id,
                    name = s.Name,
                    displayName = $"{s.Name} ({s.StudentCount}/{s.Capacity}){(s.StudentCount >= s.Capacity ? " - FULL" : "")}",
                    groupName = "",
                    studentCount = s.StudentCount,
                    capacity = s.Capacity,
                    isFull = s.StudentCount >= s.Capacity
                })
                .ToList();
        }

        return Json(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSectionAjax(int schoolClassId, string name, int? parentSectionId = null)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var section = new SchoolManagementSystem.Models.Entities.Academic.Section
            {
                SchoolClassId = schoolClassId,
                Name = name.Trim(),
                ParentSectionId = parentSectionId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };
            _db.Sections.Add(section);
            await _db.SaveChangesAsync();

            // If this is a sub-section, copy subjects from the parent group (if any)
            if (parentSectionId.HasValue)
            {
                var parentSubjects = await _db.ClassSubjects
                    .Where(cs => cs.SectionId == parentSectionId.Value && !cs.IsDeleted)
                    .ToListAsync();

                if (parentSubjects.Any())
                {
                    var newSubjects = parentSubjects.Select(ps => new ClassSubject
                    {
                        SchoolClassId = ps.SchoolClassId,
                        SubjectId = ps.SubjectId,
                        SectionId = section.Id, // Link to new section
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId
                    }).ToList();

                    _db.ClassSubjects.AddRange(newSubjects);
                    await _db.SaveChangesAsync();
                }
            }

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