using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class AcademicDocumentController : Controller
{
    private readonly SchoolDbContext _db;
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeService _employeeService;
    private readonly IWebHostEnvironment _env;

    public AcademicDocumentController(SchoolDbContext db, IUnitOfWork uow, IEmployeeService employeeService, IWebHostEnvironment env)
    {
        _db = db;
        _uow = uow;
        _employeeService = employeeService;
        _env = env;
    }

    [RequirePermission("Academic.View")]
    public async Task<IActionResult> Index(int? classId, int? subjectId)
    {
        var query = _db.AcademicDocuments
            .Include(d => d.Class)
            .Include(d => d.Subject)
            .Include(d => d.UploadedBy)
            .Where(d => !d.IsDeleted);

        if (classId.HasValue) query = query.Where(d => d.ClassId == classId.Value);
        if (subjectId.HasValue) query = query.Where(d => d.SubjectId == subjectId.Value);

        var docs = await query.OrderByDescending(d => d.CreatedAt).ToListAsync();
        return View(docs);
    }

    [HttpPost]
    [RequirePermission("Academic.Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(AcademicDocument model, IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var employeeId = await _employeeService.GetEmployeeIdByUserIdAsync(userId);
            if (!employeeId.HasValue) return Forbid();

            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "academic", model.ClassId.ToString());
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            model.FilePath = "/uploads/academic/" + model.ClassId + "/" + fileName;
            model.UploadedByEmployeeId = employeeId.Value;
            model.CreatedBy = User.Identity!.Name!;
            model.CreatedAt = DateTime.UtcNow;

            await _db.AcademicDocuments.AddAsync(model);
            await _uow.SaveChangesAsync();
            TempData["SuccessMessage"] = "Academic document uploaded successfully.";
        }
        return RedirectToAction(nameof(Index), new { classId = model.ClassId, subjectId = model.SubjectId });
    }
}
