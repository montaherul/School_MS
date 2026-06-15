using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using System.Security.Claims;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.DTOs.Identity;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Models.ViewModels.IdCard;
using SchoolManagementSystem.Models.ViewModels.Student;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.Services.Interfaces.Identity;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Website;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using EmpEntity = SchoolManagementSystem.Models.Entities.Employee.Employee;

namespace SchoolManagementSystem.Controllers;

[Authorize]
public class IdCardController : Controller
{
    private readonly IIdCardService _idCardService;
    private readonly IStudentService _studentService;
    private readonly IEmployeeService _employeeService;
    private readonly ISectionService _sectionService;
    private readonly IDepartmentService _departmentService;
    private readonly IDesignationService _designationService;
    private readonly ISchoolWebsiteService _websiteService;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IUnitOfWork _uow;

    public IdCardController(
        IIdCardService idCardService,
        IStudentService studentService,
        IEmployeeService employeeService,
        ISectionService sectionService,
        IDepartmentService departmentService,
        IDesignationService designationService,
        ISchoolWebsiteService websiteService,
        IPdfGenerator pdfGenerator,
        IUnitOfWork uow)
    {
        _idCardService = idCardService;
        _studentService = studentService;
        _employeeService = employeeService;
        _sectionService = sectionService;
        _departmentService = departmentService;
        _designationService = designationService;
        _websiteService = websiteService;
        _pdfGenerator = pdfGenerator;
        _uow = uow;
    }

    // ──────────────────────────────────────────────
    //  STUDENT ID CARDS
    // ──────────────────────────────────────────────

    [RequirePermission("IdCard.View")]
    public async Task<IActionResult> Students(CancellationToken ct)
    {
        var classes = (await _sectionService.GetAvailableClassesAsync(ct))
            .Cast<dynamic>()
            .Select(c => new SchoolClassListItemDto
            {
                Id = (int)c.Id,
                Name = (string)c.Name
            })
            .ToList();

        var groups = await _sectionService.GetStudentGroupsByClassIdAsync(0, ct);

        var model = new IdCardStudentListViewModel
        {
            Classes = classes,
            Groups = [.. groups]
        };

        return View(model);
    }

    [HttpGet]
    [RequirePermission("IdCard.View")]
    public async Task<IActionResult> StudentsData(
        int page = 1, int pageSize = 10,
        string? search = null,
        int? classId = null, int? sectionId = null, int? groupId = null,
        string? status = null, string? gender = null,
        DateTime? admissionFrom = null, DateTime? admissionTo = null,
        CancellationToken ct = default)
    {
        var (items, totalRecords) = await _idCardService.GetStudentIdCardListAsync(
            page, pageSize, search,
            classId, sectionId, groupId,
            status, gender,
            admissionFrom, admissionTo,
            ct);

        var data = items.Select(s => new
        {
            s.Id,
            studentCode = s.StudentCode,
            studentName = s.StudentName,
            s.Gender,
            s.RollNumber,
            s.Phone,
            s.Email,
            s.PhotoPath,
            s.Status,
            s.ClassName,
            s.SectionName,
            s.GroupName,
            s.GuardianName,
            admissionDate = s.AdmissionDate,
            hasPhoto = !string.IsNullOrEmpty(s.PhotoPath)
        });

        return Json(new
        {
            data,
            last_page = (int)Math.Ceiling((double)totalRecords / pageSize),
            total_records = totalRecords
        });
    }

    [HttpGet]
    [RequirePermission("IdCard.View")]
    public async Task<IActionResult> DownloadStudentCardPdf(int id, CancellationToken ct)
    {
        var bulkData = await _idCardService.GetStudentIdCardBulkDataAsync(id.ToString(), ct);
        var dto = bulkData.FirstOrDefault();
        if (dto == null) return NotFound();

        var student = MapToStudentUpsertDto(dto);
        var school = await _websiteService.GetSettingsAsync(ct);
        var academicYear = await GetActiveAcademicYearAsync(ct);
        var viewModel = BuildStudentCardViewModel([student], school, academicYear);
        var pdfBytes = _pdfGenerator.GenerateStudentIdCardPdf(viewModel);
        return File(pdfBytes, "application/pdf", $"ID_Card_{dto.StudentNo}.pdf");
    }

    [HttpGet]
    [RequirePermission("IdCard.View")]
    public async Task<IActionResult> DownloadBulkStudentCardPdf(string? ids, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(ids)) return NotFound();

        var bulkData = await _idCardService.GetStudentIdCardBulkDataAsync(ids, ct);
        if (bulkData.Count == 0) return NotFound();

        var students = bulkData.Select(MapToStudentUpsertDto).ToList();
        var school = await _websiteService.GetSettingsAsync(ct);
        var academicYear = await GetActiveAcademicYearAsync(ct);
        var viewModel = BuildStudentCardViewModel(students, school, academicYear);
        viewModel.IsBulk = true;
        var pdfBytes = _pdfGenerator.GenerateStudentIdCardPdf(viewModel);
        return File(pdfBytes, "application/pdf", $"Bulk_ID_Cards_{DateTime.Today:yyyyMMdd}.pdf");
    }

    [HttpGet]
    [RequirePermission("IdCard.View")]
    public async Task<IActionResult> DownloadAllFilteredStudentCardPdf(
        string? search = null,
        int? classId = null, int? sectionId = null, int? groupId = null,
        string? status = null, string? gender = null,
        DateTime? admissionFrom = null, DateTime? admissionTo = null,
        CancellationToken ct = default)
    {
        var (items, _) = await _idCardService.GetStudentIdCardListAsync(
            1, int.MaxValue, search,
            classId, sectionId, groupId,
            status, gender,
            admissionFrom, admissionTo,
            ct);

        var ids = string.Join(",", items.Select(i => i.Id));
        if (string.IsNullOrEmpty(ids)) return NotFound();

        var bulkData = await _idCardService.GetStudentIdCardBulkDataAsync(ids, ct);
        if (bulkData.Count == 0) return NotFound();

        var students = bulkData.Select(MapToStudentUpsertDto).ToList();
        var school = await _websiteService.GetSettingsAsync(ct);
        var academicYear = await GetActiveAcademicYearAsync(ct);
        var viewModel = BuildStudentCardViewModel(students, school, academicYear);
        viewModel.IsBulk = true;
        var pdfBytes = _pdfGenerator.GenerateStudentIdCardPdf(viewModel);
        return File(pdfBytes, "application/pdf", $"Bulk_ID_Cards_{DateTime.Today:yyyyMMdd}.pdf");
    }

    // ──────────────────────────────────────────────
    //  EMPLOYEE ID CARDS
    // ──────────────────────────────────────────────

    [RequirePermission("IdCard.View")]
    public async Task<IActionResult> Employees(CancellationToken ct)
    {
        var departments = await _departmentService.GetAllAsync(ct);
        var designations = await _designationService.GetAllAsync(ct);

        var model = new IdCardEmployeeListViewModel
        {
            Departments = [.. departments],
            Designations = [.. designations]
        };

        return View(model);
    }

    [HttpGet]
    [RequirePermission("IdCard.View")]
    public async Task<IActionResult> EmployeesData(
        int page = 1, int pageSize = 10,
        string? search = null,
        int? departmentId = null, int? designationId = null,
        string? status = null, string? employmentType = null,
        DateTime? joiningFrom = null, DateTime? joiningTo = null,
        CancellationToken ct = default)
    {
        var (items, totalRecords) = await _idCardService.GetEmployeeIdCardListAsync(
            page, pageSize, search,
            departmentId, designationId,
            status, employmentType,
            joiningFrom, joiningTo,
            ct);

        var data = items.Select(e => new
        {
            e.Id,
            e.EmployeeCode,
            employeeName = e.EmployeeName,
            e.Phone,
            e.Email,
            e.Status,
            e.IsTeachingStaff,
            e.EmploymentType,
            e.JoiningDate,
            photoPath = e.PhotoPath,
            e.DesignationName,
            e.DepartmentName,
            e.EmployeeCardNumber,
            e.CardIssueDate,
            e.CardExpiryDate,
            e.CardPrintedAt,
            e.CardVersion,
            hasPhoto = !string.IsNullOrEmpty(e.PhotoPath),
            staffTypeLabel = e.IsTeachingStaff ? "Teaching" : "Non-Teaching"
        });

        return Json(new
        {
            data,
            last_page = (int)Math.Ceiling((double)totalRecords / pageSize),
            total_records = totalRecords
        });
    }

    [HttpGet]
    [RequirePermission("IdCard.View")]
    public async Task<IActionResult> DownloadEmployeeCardPdf(int id, CancellationToken ct)
    {
        var bulkData = await _idCardService.GetEmployeeIdCardBulkDataAsync(id.ToString(), ct);
        var dto = bulkData.FirstOrDefault();
        if (dto == null) return NotFound();

        var employee = MapToEmployeeDetailsDto(dto);
        await InitializeEmployeeCardFieldsAsync(employee, id, ct);

        var schoolSetting = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(ct)
            ?? new SchoolSetting { SchoolName = "School Management ERP" };

        var academicYear = await GetActiveAcademicYearAsync(ct);
        var viewModel = BuildEmployeeCardViewModel([employee], schoolSetting, academicYear);
        var pdfBytes = _pdfGenerator.GenerateEmployeeIdCardPdf(viewModel);

        var empEntity = await _uow.Repository<EmpEntity>().GetByIdAsync(id, ct);
        if (empEntity != null && empEntity.CardPrintedAt == null)
        {
            empEntity.CardPrintedAt = DateTime.UtcNow;
            await _uow.SaveChangesAsync(ct);
        }

        await LogAuditAsync("ID Card Downloaded", $"Employee ID Card downloaded for: {employee.FullName}", ct);

        return File(pdfBytes, "application/pdf", $"ID_Card_{employee.EmployeeCode}.pdf");
    }

    [HttpGet]
    [RequirePermission("IdCard.View")]
    public async Task<IActionResult> DownloadBulkEmployeeCardPdf(string? ids, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(ids)) return NotFound();

        var bulkData = await _idCardService.GetEmployeeIdCardBulkDataAsync(ids, ct);
        if (bulkData.Count == 0) return NotFound();

        var employees = new List<EmployeeDetailsDto>();
        foreach (var bulk in bulkData)
        {
            var emp = MapToEmployeeDetailsDto(bulk);
            await InitializeEmployeeCardFieldsAsync(emp, emp.Id, ct);
            employees.Add(emp);
        }

        var schoolSetting = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(ct)
            ?? new SchoolSetting { SchoolName = "School Management ERP" };

        var academicYear = await GetActiveAcademicYearAsync(ct);
        var viewModel = BuildEmployeeCardViewModel(employees, schoolSetting, academicYear);
        viewModel.IsBulk = true;
        var pdfBytes = _pdfGenerator.GenerateEmployeeIdCardPdf(viewModel);

        await LogAuditAsync("Bulk ID Cards Downloaded", $"Bulk downloaded ID Cards for {employees.Count} employees", ct);

        return File(pdfBytes, "application/pdf", $"Bulk_Employee_ID_Cards_{DateTime.Today:yyyyMMdd}.pdf");
    }

    [HttpGet]
    [RequirePermission("IdCard.View")]
    public async Task<IActionResult> DownloadAllFilteredEmployeeCardPdf(
        string? search = null,
        int? departmentId = null, int? designationId = null,
        string? status = null, string? employmentType = null,
        DateTime? joiningFrom = null, DateTime? joiningTo = null,
        CancellationToken ct = default)
    {
        var (items, _) = await _idCardService.GetEmployeeIdCardListAsync(
            1, int.MaxValue, search,
            departmentId, designationId,
            status, employmentType,
            joiningFrom, joiningTo,
            ct);

        var ids = string.Join(",", items.Select(i => i.Id));
        if (string.IsNullOrEmpty(ids)) return NotFound();

        var bulkData = await _idCardService.GetEmployeeIdCardBulkDataAsync(ids, ct);
        if (bulkData.Count == 0) return NotFound();

        var employees = new List<EmployeeDetailsDto>();
        foreach (var bulk in bulkData)
        {
            var emp = MapToEmployeeDetailsDto(bulk);
            await InitializeEmployeeCardFieldsAsync(emp, emp.Id, ct);
            employees.Add(emp);
        }

        var schoolSetting = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(ct)
            ?? new SchoolSetting { SchoolName = "School Management ERP" };

        var academicYear = await GetActiveAcademicYearAsync(ct);
        var viewModel = BuildEmployeeCardViewModel(employees, schoolSetting, academicYear);
        viewModel.IsBulk = true;
        var pdfBytes = _pdfGenerator.GenerateEmployeeIdCardPdf(viewModel);

        await LogAuditAsync("Bulk ID Cards Downloaded", $"Bulk downloaded ID Cards for {employees.Count} employees", ct);

        return File(pdfBytes, "application/pdf", $"Bulk_Employee_ID_Cards_{DateTime.Today:yyyyMMdd}.pdf");
    }

    // ──────────────────────────────────────────────
    //  HELPERS
    // ──────────────────────────────────────────────

    private async Task<string> GetActiveAcademicYearAsync(CancellationToken ct)
    {
        var year = await _uow.Repository<AcademicYear>().Query()
            .Where(y => y.IsActive)
            .Select(y => y.Name)
            .FirstOrDefaultAsync(ct);
        return year ?? $"Academic Year {DateTime.Today.Year}";
    }

    private IdCardPrintViewModel BuildStudentCardViewModel(
        List<SchoolManagementSystem.Models.DTOs.Student.StudentUpsertDto> students,
        SchoolSetting school, string academicYear)
    {
        return new IdCardPrintViewModel
        {
            Students = students,
            SchoolLogoPath = school.LogoPath ?? "",
            SchoolSealPath = school.PrincipalSignaturePath ?? "",
            SchoolNameEn = school.SchoolName,
            SchoolNameBn = school.BanglaName ?? "",
            SchoolEIIN = school.EIIN,
            SchoolWebsite = school.Website,
            SchoolMotto = school.SchoolMotto ?? "",
            AcademicYear = academicYear,
            SchoolAddress = school.Address,
            SchoolPhone = school.Phone,
            SchoolEmail = school.Email,
            PrincipalName = school.PrincipalName ?? "",
            PrincipalSignaturePath = school.PrincipalSignaturePath ?? "",
            FooterText = school.FooterText
        };
    }

    private EmployeeIdCardPrintViewModel BuildEmployeeCardViewModel(
        List<EmployeeDetailsDto> employees,
        SchoolSetting school, string academicYear)
    {
        return new EmployeeIdCardPrintViewModel
        {
            Employees = employees,
            SchoolLogoPath = school.LogoPath ?? "",
            SchoolSealPath = school.PrincipalSignaturePath ?? "",
            SchoolNameEn = school.SchoolName,
            SchoolEIIN = school.EIIN,
            SchoolWebsite = school.Website,
            SchoolMotto = school.SchoolMotto ?? "",
            AcademicYear = academicYear,
            SchoolAddress = school.Address,
            SchoolPhone = school.Phone,
            SchoolEmail = school.Email,
            PrincipalName = school.PrincipalName ?? "",
            PrincipalSignaturePath = school.PrincipalSignaturePath ?? "",
            FooterText = school.FooterText
        };
    }

    private async Task InitializeEmployeeCardFieldsAsync(EmployeeDetailsDto employee, int id, CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(employee.EmployeeCardNumber)) return;

        var employeeEntity = await _uow.Repository<EmpEntity>().GetByIdAsync(id, ct);
        if (employeeEntity == null) return;

        employeeEntity.EmployeeCardNumber = $"EMP-{DateTime.Today.Year}-{id:D6}";
        employeeEntity.CardIssueDate = DateTime.Today;
        employeeEntity.CardExpiryDate = new DateTime(DateTime.Today.Year + 2, 12, 31);
        employeeEntity.CardVersion = 1;
        employeeEntity.QRVerificationCode = Guid.NewGuid().ToString("N")[..10].ToUpper();
        await _uow.SaveChangesAsync(ct);

        employee.EmployeeCardNumber = employeeEntity.EmployeeCardNumber;
        employee.CardIssueDate = employeeEntity.CardIssueDate;
        employee.CardExpiryDate = employeeEntity.CardExpiryDate;
        employee.CardVersion = employeeEntity.CardVersion;
        employee.QRVerificationCode = employeeEntity.QRVerificationCode;
    }

    private async Task LogAuditAsync(string action, string details, CancellationToken ct)
    {
        try
        {
            var userIdStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            int? userId = null;
            if (int.TryParse(userIdStr, out var parsedId))
                userId = parsedId;

            var log = new AuditLog
            {
                UserId = userId,
                Module = "IdCardManagement",
                Action = action,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                Details = details,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Repository<AuditLog>().AddAsync(log, ct);
            await _uow.SaveChangesAsync(ct);
        }
        catch
        {
            // Ignore audit log errors
        }
    }

    private static SchoolManagementSystem.Models.DTOs.Student.StudentUpsertDto MapToStudentUpsertDto(
        SchoolManagementSystem.Models.DTOs.Identity.StudentIdCardBulkDto dto)
    {
        return new SchoolManagementSystem.Models.DTOs.Student.StudentUpsertDto
        {
            Id = dto.Id,
            StudentNo = dto.StudentNo,
            FullName = dto.FullName,
            FullNameBangla = dto.FullNameBangla,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            FatherName = dto.FatherName,
            FatherOccupation = dto.FatherOccupation,
            MotherName = dto.MotherName,
            MotherOccupation = dto.MotherOccupation,
            GuardianName = dto.GuardianName,
            FatherOrGuardianMobileNo = dto.GuardianMobileNumber ?? "",
            MobileNumber = dto.MobileNumber,
            EmailAddress = dto.EmailAddress,
            BloodGroup = dto.BloodGroup,
            Religion = dto.Religion,
            ClassId = dto.ClassId,
            SectionId = dto.SectionId,
            StudentGroupId = dto.StudentGroupId,
            RollNumber = dto.RollNumber,
            PresentVillage = dto.PresentVillage,
            PresentPostOffice = dto.PresentPostOffice,
            PresentThana = dto.PresentThana,
            PresentDistrict = dto.PresentDistrict,
            ProfilePicturePath = dto.ProfilePicturePath,
            ClassName = dto.ClassName,
            SectionName = dto.SectionName,
            GroupName = dto.GroupName
        };
    }

    private static EmployeeDetailsDto MapToEmployeeDetailsDto(
        SchoolManagementSystem.Models.DTOs.Identity.EmployeeIdCardBulkDto dto)
    {
        return new EmployeeDetailsDto
        {
            Id = dto.Id,
            EmployeeCode = dto.EmployeeCode,
            FullName = dto.FullName,
            FatherName = dto.FatherName,
            MotherName = dto.MotherName,
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            BloodGroup = dto.BloodGroup,
            NIDNumber = dto.NIDNumber,
            Phone = dto.Phone,
            Email = dto.Email,
            PresentAddress = dto.PresentAddress,
            JoiningDate = dto.JoiningDate,
            Department = dto.Department,
            Designation = dto.Designation,
            EmployeeType = dto.EmployeeType,
            Status = dto.Status,
            ProfilePicturePath = dto.ProfilePicturePath,
            EmergencyContactName = dto.EmergencyContactName,
            EmergencyContactPhone = dto.EmergencyContactPhone,
            EmployeeCardNumber = dto.EmployeeCardNumber,
            CardIssueDate = dto.CardIssueDate,
            CardExpiryDate = dto.CardExpiryDate,
            CardVersion = dto.CardVersion,
            QRVerificationCode = dto.QRVerificationCode
        };
    }
}
