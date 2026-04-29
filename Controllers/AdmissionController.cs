using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace SchoolManagementSystem.Controllers.Admission;

[Authorize]
public class AdmissionController : Controller
{
    private readonly IAdmissionService _admissionService;
    private readonly SchoolDbContext _db;

    public AdmissionController(IAdmissionService admissionService, SchoolDbContext db)
    {
        _admissionService = admissionService;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.Classes = await _db.Classes.Select(c => new { c.Id, c.Name }).ToListAsync();
        return View();
    }

    [HttpGet]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, int? classId = null)
    {
        var query = _db.Admissions.Where(a => !a.IsDeleted);

        if (classId.HasValue && classId.Value > 0)
        {
            query = query.Where(a => a.AppliedClassId == classId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(a => a.ApplicantName.Contains(search) || a.ApplicationNo.Contains(search) || a.FatherOrGuardianMobileNo.Contains(search));
        }

        var totalItems = await query.CountAsync();
        var items = await query.OrderByDescending(a => a.Id)
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
    public async Task<IActionResult> GetClassSections(int classId)
    {
        var sections = await _db.Sections
                                .Where(s => s.SchoolClassId == classId)
                                .Select(s => new { s.Id, s.Name })
                                .ToListAsync();
        return Json(sections);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSectionAjax(int schoolClassId, string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Section name cannot be empty." });

            var existing = await _db.Sections.AnyAsync(s => s.SchoolClassId == schoolClassId && s.Name.ToLower() == name.ToLower());
            if (existing) return Json(new { success = false, message = "A section with this name already exists for this class." });

            var section = new SchoolManagementSystem.Models.Entities.Academic.Section
            {
                SchoolClassId = schoolClassId,
                Name = name.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System"
            };

            await _db.Sections.AddAsync(section);
            await _db.SaveChangesAsync();

            return Json(new { success = true, id = section.Id, name = section.Name });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult CreateEdit(int? id)
    {
        ViewBag.Classes = _db.Classes.Select(c => new { c.Id, c.Name }).ToList();

        if (id.HasValue && id > 0)
        {
            var application = _db.Admissions.FirstOrDefault(a => a.Id == id.Value && !a.IsDeleted);
            if (application == null) return NotFound();

            var dto = new AdmissionCreateDto
            {
                ApplicantName = application.ApplicantName,
                ApplicantNameBangla = application.ApplicantNameBangla,
                DateOfBirth = application.DateOfBirth,
                Gender = application.Gender,

                FatherName = application.FatherName,
                FatherOccupation = application.FatherOccupation,
                MotherName = application.MotherName,
                MotherOccupation = application.MotherOccupation,
                GuardianName = application.GuardianName,
                GuardianOccupation = application.GuardianOccupation,

                ApplicantMobileNumber = application.ApplicantMobileNumber,
                AlternativeNumber = application.AlternativeNumber,
                FatherOrGuardianMobileNo = application.FatherOrGuardianMobileNo,
                ApplicantEmail = application.ApplicantEmail,

                Nationality = application.Nationality,
                Country = application.Country,
                MaritalStatus = application.MaritalStatus,
                Religion = application.Religion,
                BloodGroup = application.BloodGroup,

                PassportNo = application.PassportNo,
                NationalIdNo = application.NationalIdNo,
                BirthCertificateNo = application.BirthCertificateNo,

                PaymentMethod = application.PaymentMethod,
                TransactionDetails = application.TransactionDetails,

                PresentVillage = application.PresentVillage,
                PresentPostOffice = application.PresentPostOffice,
                PresentThana = application.PresentThana,
                PresentDistrict = application.PresentDistrict,

                PermanentVillage = application.PermanentVillage,
                PermanentPostOffice = application.PermanentPostOffice,
                PermanentThana = application.PermanentThana,
                PermanentDistrict = application.PermanentDistrict,



                AppliedClassId = application.AppliedClassId,
                ProfilePicturePath = application.ProfilePicturePath
            };
            ViewBag.IsEdit = true;
            ViewBag.Id = id.Value;
            return View(dto);
        }

        ViewBag.IsEdit = false;
        return View(new AdmissionCreateDto { DateOfBirth = DateTime.Today.AddYears(-6) });
    }

    [HttpPost]
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

        if (id.HasValue && id > 0)
        {
            // 🔥 GET ENTITY FIRST
            var application = await _db.Admissions
                .FirstOrDefaultAsync(a => a.Id == id.Value && !a.IsDeleted);

            if (application == null) return NotFound();

            // 🔥 IMAGE UPDATE + DELETE OLD
            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/admissions");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // 🔴 DELETE OLD
                if (!string.IsNullOrEmpty(application.ProfilePicturePath))
                {
                    var oldPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        application.ProfilePicturePath.TrimStart('/')
                    );

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // 🔵 SAVE NEW
                var fileName = Guid.NewGuid() + Path.GetExtension(model.ProfilePicture.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(stream);
                }

                application.ProfilePicturePath = "/uploads/admissions/" + fileName;
            }

            // 🔧 UPDATE DATA
            application.ApplicantName = model.ApplicantName.Trim();
            application.ApplicantNameBangla = model.ApplicantNameBangla?.Trim();
            application.DateOfBirth = model.DateOfBirth;
            application.Gender = model.Gender.Trim();

            application.FatherName = model.FatherName.Trim();
            application.FatherOccupation = model.FatherOccupation?.Trim();
            application.MotherName = model.MotherName.Trim();
            application.MotherOccupation = model.MotherOccupation?.Trim();
            application.GuardianName = model.GuardianName?.Trim();
            application.GuardianOccupation = model.GuardianOccupation?.Trim();

            application.ApplicantMobileNumber = model.ApplicantMobileNumber.Trim();
            application.AlternativeNumber = model.AlternativeNumber?.Trim();
            application.FatherOrGuardianMobileNo = model.FatherOrGuardianMobileNo.Trim();
            application.ApplicantEmail = model.ApplicantEmail?.Trim();

            application.Nationality = model.Nationality.Trim();
            application.Country = model.Country.Trim();
            application.MaritalStatus = model.MaritalStatus.Trim();
            application.Religion = model.Religion.Trim();
            application.BloodGroup = model.BloodGroup?.Trim();

            application.PassportNo = model.PassportNo?.Trim();
            application.NationalIdNo = model.NationalIdNo?.Trim();
            application.BirthCertificateNo = model.BirthCertificateNo?.Trim();

            application.PresentVillage = model.PresentVillage?.Trim();
            application.PresentPostOffice = model.PresentPostOffice?.Trim();
            application.PresentThana = model.PresentThana?.Trim();
            application.PresentDistrict = model.PresentDistrict?.Trim();

            application.PermanentVillage = model.PermanentVillage?.Trim();
            application.PermanentPostOffice = model.PermanentPostOffice?.Trim();
            application.PermanentThana = model.PermanentThana?.Trim();
            application.PermanentDistrict = model.PermanentDistrict?.Trim();

            application.PaymentMethod = model.PaymentMethod?.Trim();
            application.TransactionDetails = model.TransactionDetails?.Trim();

            application.AppliedClassId = model.AppliedClassId;
            application.UpdatedBy = userId;
            application.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Admission modified successfully.";
        }
        else
        {
            // CREATE
            await _admissionService.SubmitAsync(model, userId);
            TempData["SuccessMessage"] = "Admission application submitted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(AdmissionCreateDto model, int? id) => CreateEdit(id, model);

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var application = await _db.Admissions
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

        if (application == null) return NotFound();

        return View(application);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var application = await _db.Admissions
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

        if (application == null) return NotFound();

        return View(application);
    }

    [HttpPost]
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, int sectionId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var studentId = await _admissionService.ApproveAndConvertAsync(id, sectionId, userId);
            return Json(new { success = true, message = $"Application converted to Student successfully (Student ID: {studentId})." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
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
}
