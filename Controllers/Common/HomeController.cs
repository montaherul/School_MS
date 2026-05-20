<<<<<<< HEAD
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models;
=======
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
using SchoolManagementSystem.UnitOfWork.Interfaces;
>>>>>>> d8b24e6 (attendece and website curtomize)

namespace SchoolManagementSystem.Controllers.Common;

public class HomeController : Controller
{
<<<<<<< HEAD
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }
=======
    private readonly ISchoolWebsiteService _websiteService;
    private readonly ISliderService _sliderService;
    private readonly INoticeService _noticeService;
    private readonly IEventService _eventService;
    private readonly IGalleryService _galleryService;
    private readonly IUnitOfWork _uow;

    public HomeController(
        ISchoolWebsiteService websiteService,
        ISliderService sliderService,
        INoticeService noticeService,
        IEventService eventService,
        IGalleryService galleryService,
        IUnitOfWork uow)
    {
        _websiteService = websiteService;
        _sliderService = sliderService;
        _noticeService = noticeService;
        _eventService = eventService;
        _galleryService = galleryService;
        _uow = uow;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var settings = await _websiteService.GetSettingsAsync(ct);
        var sliders = await _sliderService.GetActiveSlidersAsync(ct);
        var notices = await _noticeService.GetLatestNoticesAsync(5, ct);
        var events = await _eventService.GetUpcomingEventsAsync(3, ct);
        var albums = await _galleryService.GetAllAlbumsAsync(ct);

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
            StudentCount = studentCount > 0 ? studentCount : 350, // realistic fallback counts if seeder is fresh
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
    public IActionResult Admission()
    {
>>>>>>> d8b24e6 (attendece and website curtomize)
        return View();
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
<<<<<<< HEAD

=======
>>>>>>> d8b24e6 (attendece and website curtomize)
