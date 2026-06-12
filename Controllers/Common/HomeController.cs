using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.ViewModels.Website;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Models.Entities.Website;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagementSystem.Controllers.Common;

public class HomeController : Controller
{
    private readonly ISchoolWebsiteService _websiteService;
    private readonly ISliderService _sliderService;
    private readonly INoticeService _noticeService;
    private readonly IEventService _eventService;
    private readonly IGalleryService _galleryService;
    private readonly IContactMessageService _contactService;
    private readonly IAnnouncementService _announcementService;
    private readonly IAcademicCalendarService _calendarService;
    private readonly IAdmissionFeeStructureService _admissionFeeService;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IUnitOfWork _uow;

    public HomeController(
        ISchoolWebsiteService websiteService,
        ISliderService sliderService,
        INoticeService noticeService,
        IEventService eventService,
        IGalleryService galleryService,
        IContactMessageService contactService,
        IAnnouncementService announcementService,
        IAcademicCalendarService calendarService,
        IAdmissionFeeStructureService admissionFeeService,
        IEmailSender emailSender,
        IEmailTemplateService emailTemplateService,
        IUnitOfWork uow)
    {
        _websiteService = websiteService;
        _sliderService = sliderService;
        _noticeService = noticeService;
        _eventService = eventService;
        _galleryService = galleryService;
        _contactService = contactService;
        _announcementService = announcementService;
        _calendarService = calendarService;
        _admissionFeeService = admissionFeeService;
        _emailSender = emailSender;
        _emailTemplateService = emailTemplateService;
        _uow = uow;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var settings = await _websiteService.GetSettingsAsync(ct);
        var sliders = await _sliderService.GetActiveSlidersAsync(ct);
        var notices = await _noticeService.GetLatestNoticesAsync(5, ct);
        var events = await _eventService.GetUpcomingEventsAsync(3, ct);
        var albums = await _galleryService.GetAllAlbumsAsync(ct);
        var announcements = await _announcementService.GetActiveAnnouncementsAsync(ct);

        // Academic Calendar - next 5 upcoming events
        var today = DateOnly.FromDateTime(DateTime.Today);
        var calDays = await _calendarService.GetCalendarDaysAsync(
            today.ToDateTime(TimeOnly.MinValue),
            today.AddDays(90).ToDateTime(TimeOnly.MaxValue), ct);
        var calendarEvents = calDays
            .Where(d => d.IsActive && d.Date >= today && (d.IsHoliday || d.IsExamDay || d.IsEventDay))
            .OrderBy(d => d.Date)
            .Take(5)
            .ToList();

        // Admission Fee Structures - show first 6 classes
        var admissionFees = await _admissionFeeService.GetAllAsync(ct);

        // Fetch counts for the stats block
        int studentCount = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().CountAsync(s => !s.IsDeleted, ct);
        int employeeCount = await _uow.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().CountAsync(e => !e.IsDeleted, ct);
        int teacherCount = await _uow.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().CountAsync(e => !e.IsDeleted && e.IsTeachingStaff, ct);
        int classCount = await _uow.Repository<SchoolClass>().CountAsync(null, ct);

        var viewModel = new HomepageViewModel
        {
            Settings = settings,
            Sliders = sliders.OrderBy(s => s.DisplayOrder).ToList(),
            LatestNotices = notices,
            UpcomingEvents = events,
            Albums = albums.Take(3).ToList(),
            Announcements = announcements,
            UpcomingCalendarEvents = calendarEvents,
            AdmissionFees = admissionFees.OrderBy(f => f.DisplayOrder).Take(6).ToList(),
            StudentCount = studentCount > 0 ? studentCount : 350,
            EmployeeCount = employeeCount > 0 ? employeeCount : 25,
            TeacherCount = teacherCount > 0 ? teacherCount : 18,
            ClassCount = classCount > 0 ? classCount : 10
        };

        return View(viewModel);
    }

    [HttpGet("/about")]
    public async Task<IActionResult> About(CancellationToken ct)
    {
        var settings = await _websiteService.GetSettingsAsync(ct);
        return View(settings);
    }

    [HttpGet("/principal-message")]
    public async Task<IActionResult> PrincipalMessage(CancellationToken ct)
    {
        var settings = await _websiteService.GetSettingsAsync(ct);
        return View(settings);
    }

    [HttpGet("/mission-vision")]
    public async Task<IActionResult> MissionVision(CancellationToken ct)
    {
        var settings = await _websiteService.GetSettingsAsync(ct);
        return View(settings);
    }

    [HttpGet("/contact")]
    public async Task<IActionResult> Contact(CancellationToken ct)
    {
        var settings = await _websiteService.GetSettingsAsync(ct);
        return View(settings);
    }

    [HttpGet("/admission-info")]
    public async Task<IActionResult> Admission(CancellationToken ct)
    {
        var settings = await _websiteService.GetSettingsAsync(ct);
        var feeStructure = await _uow.Repository<AdmissionFeeStructure>()
            .ListAsync(f => f.IsActive && !f.IsDeleted, ct);
        ViewBag.FeeStructure = feeStructure.OrderBy(f => f.DisplayOrder).ToList();
        return View(settings);
    }

    [HttpPost("/contact/send")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ContactSend(ContactMessage model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill in all required fields.";
            return RedirectToAction(nameof(Contact));
        }

        await _contactService.SaveMessageAsync(model, ct);

        // Send email notification to school admin using EmailTemplate system
        try
        {
            var settings = await _websiteService.GetSettingsAsync(ct);
            if (!string.IsNullOrEmpty(settings.Email))
            {
                var placeholders = new Dictionary<string, string>
                {
                    { "SchoolName", settings.SchoolName },
                    { "Name", model.Name },
                    { "Email", model.Email },
                    { "Phone", model.Phone ?? "N/A" },
                    { "Subject", model.Subject },
                    { "Message", model.Message },
                    { "Timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
                };

                var subject = await _emailTemplateService.RenderTemplateSubjectAsync("ContactNotification", placeholders, ct);
                var body = await _emailTemplateService.RenderTemplateAsync("ContactNotification", placeholders, ct);

                if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(body))
                {
                    await _emailSender.SendAsync(settings.Email, subject, body, ct);
                }
                else
                {
                    // Fallback to hardcoded template if template not found
                    var emailBody = $@"
<h2>New Contact Form Message</h2>
<p><strong>From:</strong> {model.Name} ({model.Email})</p>
<p><strong>Phone:</strong> {model.Phone ?? "N/A"}</p>
<p><strong>Subject:</strong> {model.Subject}</p>
<p><strong>Message:</strong></p>
<p>{model.Message}</p>
<hr/>
<p><small>Sent via {settings.SchoolName} contact form</small></p>";
                    await _emailSender.SendAsync(settings.Email, $"New Contact Message: {model.Subject}", emailBody, ct);
                }
            }
        }
        catch
        {
            // Email failure should not block the user experience
        }

        TempData["Success"] = "Your message has been received! Our support desk will reply soon.";
        return RedirectToAction(nameof(Contact));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
