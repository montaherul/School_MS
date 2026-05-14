using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Employee;

[Authorize]
public class EmployeeDocumentController : Controller
{
    private readonly IEmployeeDocumentRepository _docRepo;
    private readonly IUnitOfWork _uow;
    private readonly IWebHostEnvironment _env;

    public EmployeeDocumentController(IEmployeeDocumentRepository docRepo, IUnitOfWork uow, IWebHostEnvironment env)
    {
        _docRepo = docRepo;
        _uow = uow;
        _env = env;
    }

    [RequirePermission("Employee.View")]
    public async Task<IActionResult> Index(long employeeId)
    {
        var docs = await _docRepo.GetByEmployeeIdAsync(employeeId);
        ViewBag.EmployeeId = employeeId;
        return View(docs);
    }

    [HttpPost]
    [RequirePermission("Employee.Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(long employeeId, string documentType, IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "documents", employeeId.ToString());
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var doc = new EmployeeDocument
            {
                EmployeeId = employeeId,
                DocumentType = documentType,
                FilePath = "/uploads/documents/" + employeeId + "/" + fileName,
                OriginalFileName = file.FileName,
                UploadedById = userId,
                UploadedAt = DateTime.UtcNow
            };

            await _docRepo.AddAsync(doc);
            await _uow.SaveChangesAsync();
            TempData["SuccessMessage"] = "Document uploaded successfully.";
        }
        return RedirectToAction(nameof(Index), new { employeeId });
    }

    [RequirePermission("Employee.View")]
    public async Task<IActionResult> Download(long id)
    {
        var doc = await _docRepo.FirstOrDefaultAsync(d => d.Id == id);
        if (doc == null) return NotFound();

        string filePath = Path.Combine(_env.WebRootPath, doc.FilePath.TrimStart('/'));
        if (!System.IO.File.Exists(filePath)) return NotFound();

        return File(System.IO.File.ReadAllBytes(filePath), "application/octet-stream", doc.OriginalFileName);
    }
}
