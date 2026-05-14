using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;
using SchoolManagementSystem.Constants;
using SchoolManagementSystem.Services.Interfaces.Infrastructure;

namespace SchoolManagementSystem.Controllers.Employee;

[Authorize]
public class EmployeeDocumentController : Controller
{
    private readonly IEmployeeDocumentRepository _docRepo;
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _fileStorage;

    public EmployeeDocumentController(IEmployeeDocumentRepository docRepo, IUnitOfWork uow, IFileStorageService fileStorage)
    {
        _docRepo = docRepo;
        _uow = uow;
        _fileStorage = fileStorage;
    }

    [RequirePermission(Permissions.Employee.View)]
    public async Task<IActionResult> Index(long employeeId)
    {
        var docs = await _docRepo.GetByEmployeeIdAsync(employeeId);
        ViewBag.EmployeeId = employeeId;
        return View(docs);
    }

    [HttpPost]
    [RequirePermission(Permissions.Employee.Update)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(long employeeId, string documentType, IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            var filePath = await _fileStorage.SaveAsync(file, AppConstants.FileUpload.DocumentFolder + "/" + employeeId);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var doc = new EmployeeDocument
            {
                EmployeeId = employeeId,
                DocumentType = documentType,
                FilePath = filePath,
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

    [RequirePermission(Permissions.Employee.View)]
    public async Task<IActionResult> Download(long id)
    {
        var doc = await _docRepo.FirstOrDefaultAsync(d => d.Id == id);
        if (doc == null) return NotFound();

        string filePath = _fileStorage.GetAbsolutePath(doc.FilePath);
        if (!System.IO.File.Exists(filePath)) return NotFound();

        return File(System.IO.File.ReadAllBytes(filePath), "application/octet-stream", doc.OriginalFileName);
    }
}
