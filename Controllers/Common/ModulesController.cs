using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;

namespace SchoolManagementSystem.Controllers.Common;

public class ModulesController : Controller
{
    private static readonly HashSet<string> KnownModules = new(StringComparer.OrdinalIgnoreCase)
    {
        "Admissions", "Students", "Academics", "Attendance", "Exams", "Results", "Assignments", "Fees",
        "Communication", "Library", "Transport", "Health", "Reports", "Users", "Roles", "Notifications", "System"
    };

    private readonly SchoolDbContext _db;

    public ModulesController(SchoolDbContext db)
    {
        _db = db;
    }

    [Route("Modules/{id?}")]
    public IActionResult Index(string id = "Students")
    {
        if (!KnownModules.Contains(id))
        {
            return NotFound();
        }

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

        return Json(new
        {
            last_page = (int)Math.Ceiling(count / (double)size),
            data = rows
        });
    }

    private IQueryable<object> QueryFor(string id, string? search)
    {
        var term = search?.Trim();

        return id.ToLowerInvariant() switch
        {
            "admissions" => _db.Admissions
                .Where(x => term == null || x.ApplicationNo.Contains(term) || x.ApplicantName.Contains(term))
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new { x.ApplicationNo, x.ApplicantName, ClassId = x.AppliedClassId, Status = x.Status.ToString(), x.AdmissionFee }),
            "students" => _db.Students
                .Where(x => term == null || x.StudentNo.Contains(term) || x.FullName.Contains(term))
                .OrderBy(x => x.ClassId).ThenBy(x => x.RollNumber)
                .Select(x => new { x.StudentNo, x.FullName, x.ClassId, x.SectionId, x.RollNumber, Status = x.Status.ToString() }),
            "academics" => _db.Subjects
                .Where(x => term == null || x.Code.Contains(term) || x.Name.Contains(term))
                .OrderBy(x => x.Code)
                .Select(x => new { x.Code, x.Name, Module = "Subject" }),
            "attendance" => _db.Attendance
                .OrderByDescending(x => x.AttendanceDate)
                .Select(x => new { x.AttendanceDate, x.StudentId, x.SchoolClassId, x.SectionId, Status = x.Status.ToString(), x.PeriodNo }),
            "exams" => _db.Exams
                .Where(x => term == null || x.Name.Contains(term))
                .OrderByDescending(x => x.StartsOn)
                .Select(x => new { x.Name, x.AcademicYearId, x.StartsOn, x.EndsOn }),
            "results" => _db.Marks
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new { x.ExamId, x.StudentId, x.SubjectId, x.MarksObtained, Status = x.Status.ToString() }),
            "assignments" => _db.Assignments
                .Where(x => term == null || x.Title.Contains(term))
                .OrderByDescending(x => x.Deadline)
                .Select(x => new { x.Title, x.SchoolClassId, x.SectionId, x.SubjectId, x.Deadline, Status = x.Status.ToString() }),
            "fees" => _db.FeeInvoices
                .Where(x => term == null || x.InvoiceNo.Contains(term))
                .OrderByDescending(x => x.DueDate)
                .Select(x => new { x.InvoiceNo, x.StudentId, x.DueDate, x.TotalAmount, x.PaidAmount, Status = x.Status.ToString() }),
            "communication" => _db.Notices
                .Where(x => term == null || x.Title.Contains(term))
                .OrderByDescending(x => x.PublishAt)
                .Select(x => new { x.Title, x.AudienceRole, x.PublishAt }),
            "library" => _db.Books
                .Where(x => term == null || x.AccessionNo.Contains(term) || x.Title.Contains(term))
                .OrderBy(x => x.Title)
                .Select(x => new { x.AccessionNo, x.Title, x.Author, x.TotalCopies, x.AvailableCopies }),
            "transport" => _db.TransportRoutes
                .Where(x => term == null || x.Name.Contains(term))
                .OrderBy(x => x.Name)
                .Select(x => new { x.Name, x.PickupDropSchedule }),
            "health" => _db.MedicalRecords
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new { x.StudentId, x.BloodGroup, x.EmergencyContactName, x.EmergencyContactPhone }),
            "reports" => _db.AuditLogs
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new { x.Module, x.Action, x.UserId, x.CreatedAt }),
            "users" => _db.Users
                .Where(x => term == null || x.UserName.Contains(term) || x.Email.Contains(term))
                .OrderBy(x => x.UserName)
                .Select(x => new { x.UserName, x.Email, x.PhoneNumber, Status = x.Status.ToString(), x.LastLoginAt }),
            "roles" => _db.Roles
                .Where(x => term == null || x.Name.Contains(term))
                .OrderBy(x => x.Name)
                .Select(x => new { x.Name, x.Description }),
            "notifications" => _db.Notifications
                .Where(x => term == null || x.Title.Contains(term))
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new { x.Title, Channel = x.Channel.ToString(), x.IsRead, x.SentAt }),
            "system" => _db.SchoolProfiles
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
