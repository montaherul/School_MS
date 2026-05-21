using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Assignment;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Models.Entities.Library;
using SchoolManagementSystem.Models.Entities.Transport;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Health;
using SchoolManagementSystem.Models.Entities.Notification;
using SchoolManagementSystem.Models.Entities.System;
using StudentEntity = SchoolManagementSystem.Models.Entities.Student.Student;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;
using MedicalRecordEntity = SchoolManagementSystem.Models.Entities.Health.MedicalRecord;
using NotificationMessageEntity = SchoolManagementSystem.Models.Entities.Notification.NotificationMessage;

namespace SchoolManagementSystem.Controllers.Common;

public class ModulesController : Controller
{
    private static readonly HashSet<string> KnownModules = new(StringComparer.OrdinalIgnoreCase)
    {
        "Admissions", "Students", "Academics", "Attendance", "Exams", "Results", "Assignments", "Fees",
        "Communication", "Library", "Transport", "Health", "Reports", "Users", "Roles", "Notifications", "System"
    };

    private readonly IUnitOfWork _unitOfWork;

    public ModulesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [Route("Modules/{id?}")]
    public IActionResult Index(string id = "Students")
    {
        if (!KnownModules.Contains(id)) return NotFound();
        ViewData["Module"] = id;
        ViewData["Columns"] = ColumnsFor(id);
        return View();
    }

    [HttpGet("Modules/Data")]
    public async Task<IActionResult> Data(string id, int page = 1, int size = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        size = Math.Clamp(size, 5, 100);

        var query = QueryFor(id, search);
        var count = await query.CountAsync(cancellationToken);
        var rows = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);

        return Json(new { last_page = (int)Math.Ceiling(count / (double)size), data = rows });
    }

    private IQueryable<object> QueryFor(string id, string? search)
    {
        var term = search?.Trim();
        return id.ToLowerInvariant() switch
        {
            "admissions" => _unitOfWork.Repository<AdmissionApplication>().Query()
                .Where(x => term == null || x.ApplicationNo.Contains(term) || x.ApplicantName.Contains(term))
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new { x.ApplicationNo, x.ApplicantName, ClassId = x.AppliedClassId, Status = x.Status.ToString(), x.AdmissionFee }),
            "students" => _unitOfWork.Repository<StudentEntity>().Query()
                .Where(x => term == null || x.StudentNo.Contains(term) || x.FullName.Contains(term))
                .OrderBy(x => x.ClassId).ThenBy(x => x.RollNumber)
                .Select(x => new { x.StudentNo, x.FullName, x.ClassId, x.SectionId, x.RollNumber, Status = x.Status.ToString() }),
            "academics" => _unitOfWork.Repository<Subject>().Query()
                .Where(x => term == null || x.Code.Contains(term) || x.Name.Contains(term))
                .OrderBy(x => x.Code)
                .Select(x => new { x.Code, x.Name, Module = "Subject" }),
            "attendance" => _unitOfWork.Repository<AttendanceRecord>().Query()
                .OrderByDescending(x => x.AttendanceDate)
                .Select(x => new { x.AttendanceDate, x.StudentId, x.SchoolClassId, x.SectionId, Status = x.Status.ToString(), x.PeriodNo }),
            "exams" => _unitOfWork.Repository<ExamEntity>().Query()
                .Where(x => term == null || x.Name.Contains(term))
                .OrderByDescending(x => x.StartsOn)
                .Select(x => new { x.Name, x.AcademicYearId, x.StartsOn, x.EndsOn }),
            "results" => _unitOfWork.Repository<MarkEntry>().Query()
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new { x.ExamId, x.StudentId, x.SubjectId, x.MarksObtained, Status = x.Status.ToString() }),
            "assignments" => _unitOfWork.Repository<AssignmentTask>().Query()
                .Where(x => term == null || x.Title.Contains(term))
                .OrderByDescending(x => x.Deadline)
                .Select(x => new { x.Title, x.SchoolClassId, x.SectionId, x.SubjectId, x.Deadline, Status = x.Status.ToString() }),
            "fees" => _unitOfWork.Repository<FeeInvoice>().Query()
                .Where(x => term == null || x.InvoiceNo.Contains(term))
                .OrderByDescending(x => x.DueDate)
                .Select(x => new { x.InvoiceNo, x.StudentId, x.DueDate, x.TotalAmount, x.PaidAmount, Status = x.Status.ToString() }),
            "communication" => _unitOfWork.Repository<Notice>().Query()
                .Where(x => term == null || x.Title.Contains(term))
                .OrderByDescending(x => x.PublishAt)
                .Select(x => new { x.Title, x.AudienceRole, x.PublishAt }),
            "library" => _unitOfWork.Repository<Book>().Query()
                .Where(x => term == null || x.AccessionNo.Contains(term) || x.Title.Contains(term))
                .OrderBy(x => x.Title)
                .Select(x => new { x.AccessionNo, x.Title, x.Author, x.TotalCopies, x.AvailableCopies }),
            "transport" => _unitOfWork.Repository<TransportRoute>().Query()
                .Where(x => term == null || x.Name.Contains(term))
                .OrderBy(x => x.Name)
                .Select(x => new { x.Name, x.PickupDropSchedule }),
            "health" => _unitOfWork.Repository<MedicalRecordEntity>().Query()
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new { x.StudentId, x.BloodGroup, x.EmergencyContactName, x.EmergencyContactPhone }),
            "reports" => _unitOfWork.Repository<AuditLog>().Query()
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new { x.Module, x.Action, x.UserId, x.CreatedAt }),
            "users" => _unitOfWork.Repository<ApplicationUser>().Query()
                .Where(x => term == null || x.UserName.Contains(term) || x.Email.Contains(term))
                .OrderBy(x => x.UserName)
                .Select(x => new { x.UserName, x.Email, x.PhoneNumber, Status = x.Status.ToString(), x.LastLoginAt }),
            "roles" => _unitOfWork.Repository<Role>().Query()
                .Where(x => term == null || x.Name.Contains(term))
                .OrderBy(x => x.Name)
                .Select(x => new { x.Name, x.Description }),
            "notifications" => _unitOfWork.Repository<NotificationMessageEntity>().Query()
                .Where(x => term == null || x.Title.Contains(term))
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new { x.Title, Channel = x.Channel.ToString(), x.IsRead, x.SentAt }),
            "system" => _unitOfWork.Repository<SchoolProfile>().Query()
                .OrderBy(x => x.Name)
                .Select(x => new { x.Name, x.Address, x.Phone, x.Email }),
            _ => Enumerable.Empty<object>().AsQueryable()
        };
    }

    private static string ColumnsFor(string id)
    {
        var fields = id.ToLowerInvariant() switch
        {
            "admissions" => new[] { "applicationNo", "applicantName", "classId", "status", "admissionFee" },
            "students" => new[] { "studentNo", "fullName", "classId", "sectionId", "rollNumber", "status" },
            "academics" => new[] { "code", "name", "module" },
            "attendance" => new[] { "attendanceDate", "studentId", "schoolClassId", "sectionId", "status", "periodNo" },
            "exams" => new[] { "name", "academicYearId", "startsOn", "endsOn" },
            "results" => new[] { "examId", "studentId", "subjectId", "marksObtained", "status" },
            "assignments" => new[] { "title", "schoolClassId", "sectionId", "subjectId", "deadline", "status" },
            "fees" => new[] { "invoiceNo", "studentId", "dueDate", "totalAmount", "paidAmount", "status" },
            "communication" => new[] { "title", "audienceRole", "publishAt" },
            "library" => new[] { "accessionNo", "title", "author", "totalCopies", "availableCopies" },
            "transport" => new[] { "name", "pickupDropSchedule" },
            "health" => new[] { "studentId", "bloodGroup", "emergencyContactName", "emergencyContactPhone" },
            "reports" => new[] { "module", "action", "userId", "createdAt" },
            "users" => new[] { "userName", "email", "phoneNumber", "status", "lastLoginAt" },
            "roles" => new[] { "name", "description" },
            "notifications" => new[] { "title", "channel", "isRead", "sentAt" },
            "system" => new[] { "name", "address", "phone", "email" },
            _ => Array.Empty<string>()
        };
        return string.Join(",", fields);
    }
}
