using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Helpers.Files;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Services.Interfaces.Website;
using SchoolManagementSystem.Models.Entities.Website;
using System.Collections.Generic;

namespace SchoolManagementSystem.Controllers.Website;

[Authorize]
[Route("Admin/Website")]
public class WebsiteAdminController : Controller
{
    private readonly ISchoolWebsiteService _settingsService;
    private readonly ISliderService _sliderService;
    private readonly INoticeService _noticeService;
    private readonly IEventService _eventService;
    private readonly IGalleryService _galleryService;
    private readonly IWebsitePageService _pageService;
    private readonly IContactMessageService _contactService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IAdmissionFeeStructureService _feeService;
    private readonly IAnnouncementService _announcementService;
    private readonly IEventNotificationService _eventNotificationService;
    private readonly IEventCategoryService _eventCategoryService;
    private readonly IFileStorageService _fileService;

    public WebsiteAdminController(
        ISchoolWebsiteService settingsService,
        ISliderService sliderService,
        INoticeService noticeService,
        IEventService eventService,
        IGalleryService galleryService,
        IWebsitePageService pageService,
        IContactMessageService contactService,
        IEmailTemplateService emailTemplateService,
        IAdmissionFeeStructureService feeService,
        IAnnouncementService announcementService,
        IEventNotificationService eventNotificationService,
        IEventCategoryService eventCategoryService,
        IFileStorageService fileService)
    {
        _settingsService = settingsService;
        _sliderService = sliderService;
        _noticeService = noticeService;
        _eventService = eventService;
        _galleryService = galleryService;
        _pageService = pageService;
        _contactService = contactService;
        _emailTemplateService = emailTemplateService;
        _feeService = feeService;
        _announcementService = announcementService;
        _eventNotificationService = eventNotificationService;
        _eventCategoryService = eventCategoryService;
        _fileService = fileService;
    }

    [HttpGet("")]
    [RequirePermission("Website.View")]
    public IActionResult Index()
    {
        return View();
    }

    // ── School Settings ──
    [HttpGet("Settings")]
    [RequirePermission("Website.View")]
    public async Task<IActionResult> Settings(CancellationToken ct)
    {
        var settings = await _settingsService.GetSettingsAsync(ct);
        return View(settings);
    }

    [HttpPost("Settings")]
    [RequirePermission("Website.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(SchoolSetting model, IFormFile? logoFile, IFormFile? faviconFile, IFormFile? principalFile, IFormFile? signatureFile, IFormFile? loginLogoFile, IFormFile? footerLogoFile, IFormFile? bannerFile, IFormFile? ogImageFile, IFormFile? admissionCircularFile, IFormFile? admissionFormFile, IFormFile? admissionOgImageFile, CancellationToken ct)
    {
        if (logoFile != null && ValidateFile(logoFile, [".jpg", ".jpeg", ".png"]))
        {
            model.LogoPath = await _fileService.SaveAsync(logoFile, "settings", ct);
        }
        if (faviconFile != null && ValidateFile(faviconFile, [".png", ".ico"]))
        {
            model.FaviconPath = await _fileService.SaveAsync(faviconFile, "settings", ct);
        }
        if (principalFile != null && ValidateFile(principalFile, [".jpg", ".jpeg", ".png"]))
        {
            model.PrincipalImagePath = await _fileService.SaveAsync(principalFile, "settings", ct);
        }
        if (signatureFile != null && ValidateFile(signatureFile, [".jpg", ".jpeg", ".png"]))
        {
            model.PrincipalSignaturePath = await _fileService.SaveAsync(signatureFile, "settings", ct);
        }
        if (loginLogoFile != null && ValidateFile(loginLogoFile, [".jpg", ".jpeg", ".png"]))
        {
            model.LoginLogoPath = await _fileService.SaveAsync(loginLogoFile, "settings", ct);
        }
        if (footerLogoFile != null && ValidateFile(footerLogoFile, [".jpg", ".jpeg", ".png"]))
        {
            model.FooterLogoPath = await _fileService.SaveAsync(footerLogoFile, "settings", ct);
        }
        if (bannerFile != null && ValidateFile(bannerFile, [".jpg", ".jpeg", ".png"]))
        {
            model.WebsiteBannerPath = await _fileService.SaveAsync(bannerFile, "settings", ct);
        }
        if (ogImageFile != null && ValidateFile(ogImageFile, [".jpg", ".jpeg", ".png"]))
        {
            model.OgImagePath = await _fileService.SaveAsync(ogImageFile, "settings", ct);
        }
        if (admissionCircularFile != null && ValidateFile(admissionCircularFile, [".pdf"]))
        {
            model.AdmissionCircularPath = await _fileService.SaveAsync(admissionCircularFile, "admissions", ct);
        }
        if (admissionFormFile != null && ValidateFile(admissionFormFile, [".pdf"]))
        {
            model.AdmissionFormPath = await _fileService.SaveAsync(admissionFormFile, "admissions", ct);
        }
        if (admissionOgImageFile != null && ValidateFile(admissionOgImageFile, [".jpg", ".jpeg", ".png"]))
        {
            model.AdmissionOgImagePath = await _fileService.SaveAsync(admissionOgImageFile, "settings", ct);
        }

        if (model.AdmissionOpenDate.HasValue && model.AdmissionCloseDate.HasValue && model.AdmissionCloseDate < model.AdmissionOpenDate)
        {
            TempData["ErrorMessage"] = "Admission close date must be after the open date.";
            return RedirectToAction(nameof(Settings));
        }

        await _settingsService.UpdateSettingsAsync(model, ct);
        TempData["SuccessMessage"] = "School settings updated successfully.";
        return RedirectToAction(nameof(Settings));
    }

    // ── Hero Sliders ──
    [HttpGet("Sliders/List")]
    [RequirePermission("Website.View")]
    public async Task<IActionResult> SlidersList(CancellationToken ct)
    {
        var list = await _sliderService.GetAllSlidersAsync(ct);
        return Json(list);
    }

    [HttpGet("Sliders/CreateEdit/{id?}")]
    [RequirePermission("Website.View")]
    public async Task<IActionResult> SliderCreateEdit(int? id, CancellationToken ct)
    {
        if (id.HasValue && id > 0)
        {
            var slider = await _sliderService.GetSliderByIdAsync(id.Value, ct);
            if (slider == null) return NotFound();
            return View(slider);
        }
        return View(new Slider());
    }

    [HttpPost("Sliders/CreateEdit/{id?}")]
    [RequirePermission("Website.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SliderCreateEdit(int? id, Slider model, IFormFile? imageFile, CancellationToken ct)
    {
        if (imageFile != null && ValidateFile(imageFile, [".jpg", ".jpeg", ".png"]))
        {
            model.ImagePath = await _fileService.SaveAsync(imageFile, "sliders", ct);
        }
        else if (id == null && imageFile == null)
        {
            ModelState.AddModelError("ImagePath", "Please upload a slider image.");
            return View(model);
        }

        if (id.HasValue && id > 0)
        {
            model.Id = id.Value;
            await _sliderService.UpdateSliderAsync(model, ct);
            TempData["SuccessMessage"] = "Slider updated successfully.";
        }
        else
        {
            await _sliderService.CreateSliderAsync(model, ct);
            TempData["SuccessMessage"] = "Slider created successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Sliders/Delete/{id}")]
    [RequirePermission("Website.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SliderDelete(int id, CancellationToken ct)
    {
        await _sliderService.DeleteSliderAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Notices ──
    [HttpGet("Notices/List")]
    [RequirePermission("Website.Notices")]
    public async Task<IActionResult> NoticesList(string? search, int page = 1, int size = 10, CancellationToken ct = default)
    {
        if (size <= 0) size = 10;
        var (notices, total) = await _noticeService.GetPagedNoticesAsync(search, page, size, ct);
        return Json(new { data = notices, last_page = (int)Math.Ceiling((double)total / size), total = total });
    }

    [HttpGet("Notices/CreateEdit/{id?}")]
    [RequirePermission("Website.Notices")]
    public async Task<IActionResult> NoticeCreateEdit(int? id, CancellationToken ct)
    {
        if (id.HasValue && id > 0)
        {
            var notice = await _noticeService.GetNoticeByIdAsync(id.Value, ct);
            if (notice == null) return NotFound();
            return View(notice);
        }
        return View(new Notice { PublishAt = DateTime.Today });
    }

    [HttpPost("Notices/CreateEdit/{id?}")]
    [RequirePermission("Website.Notices")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NoticeCreateEdit(int? id, Notice model, IFormFile? attachmentFile, CancellationToken ct)
    {
        if (attachmentFile != null && ValidateFile(attachmentFile, [".jpg", ".jpeg", ".png", ".pdf", ".docx", ".doc"]))
        {
            model.AttachmentPath = await _fileService.SaveAsync(attachmentFile, "notices", ct);
        }

        if (id.HasValue && id > 0)
        {
            model.Id = id.Value;
            await _noticeService.UpdateNoticeAsync(model, ct);
            TempData["SuccessMessage"] = "Notice updated successfully.";
        }
        else
        {
            await _noticeService.CreateNoticeAsync(model, ct);
            TempData["SuccessMessage"] = "Notice published successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Notices/Delete/{id}")]
    [RequirePermission("Website.Notices")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NoticeDelete(int id, CancellationToken ct)
    {
        await _noticeService.DeleteNoticeAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Events ──
    [HttpGet("Events/List")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> EventsList(CancellationToken ct)
    {
        var events = await _eventService.GetAllEventsAsync(ct);
        return Json(events);
    }

    [HttpGet("Events/CreateEdit/{id?}")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> EventCreateEdit(int? id, CancellationToken ct)
    {
        ViewBag.Categories = await _eventCategoryService.GetActiveCategoriesAsync(ct);
        if (id.HasValue && id > 0)
        {
            var ev = await _eventService.GetEventByIdAsync(id.Value, ct);
            if (ev == null) return NotFound();
            return View(ev);
        }
        return View(new Event { EventDate = DateTime.Today.AddDays(7) });
    }

    [HttpPost("Events/CreateEdit/{id?}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EventCreateEdit(int? id, Event model, IFormFile? coverFile, CancellationToken ct)
    {
        if (coverFile != null && ValidateFile(coverFile, [".jpg", ".jpeg", ".png"]))
        {
            model.CoverImagePath = await _fileService.SaveAsync(coverFile, "events", ct);
        }

        if (id.HasValue && id > 0)
        {
            model.Id = id.Value;
            await _eventService.UpdateEventAsync(model, ct);
            TempData["SuccessMessage"] = "Event updated successfully.";
        }
        else
        {
            await _eventService.CreateEventAsync(model, ct);
            TempData["SuccessMessage"] = "Event published successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Events/Delete/{id}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EventDelete(int id, CancellationToken ct)
    {
        await _eventService.DeleteEventAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Event Categories ──
    [HttpGet("EventCategories/List")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> EventCategoriesList(CancellationToken ct)
    {
        var list = await _eventCategoryService.GetAllCategoriesAsync(ct);
        return Json(list);
    }

    [HttpGet("EventCategories/CreateEdit/{id?}")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> EventCategoryCreateEdit(int? id, CancellationToken ct)
    {
        if (id.HasValue && id > 0)
        {
            var cat = await _eventCategoryService.GetCategoryByIdAsync(id.Value, ct);
            if (cat == null) return NotFound();
            return View(cat);
        }
        return View(new EventCategory());
    }

    [HttpPost("EventCategories/CreateEdit/{id?}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EventCategoryCreateEdit(int? id, EventCategory model, CancellationToken ct)
    {
        if (id.HasValue && id > 0)
        {
            model.Id = id.Value;
            await _eventCategoryService.UpdateCategoryAsync(model, ct);
            TempData["SuccessMessage"] = "Event category updated successfully.";
        }
        else
        {
            await _eventCategoryService.CreateCategoryAsync(model, ct);
            TempData["SuccessMessage"] = "Event category created successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("EventCategories/Delete/{id}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EventCategoryDelete(int id, CancellationToken ct)
    {
        await _eventCategoryService.DeleteCategoryAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Galleries ──
    [HttpGet("Galleries/List")]
    [RequirePermission("Website.Gallery")]
    public async Task<IActionResult> GalleriesList(CancellationToken ct)
    {
        var list = await _galleryService.GetAllAlbumsAsync(ct);
        return Json(list);
    }

    [HttpGet("Galleries/CreateEdit/{id?}")]
    [RequirePermission("Website.Gallery")]
    public async Task<IActionResult> GalleryCreateEdit(int? id, CancellationToken ct)
    {
        if (id.HasValue && id > 0)
        {
            var album = await _galleryService.GetAlbumWithImagesAsync(id.Value, ct);
            if (album == null) return NotFound();
            return View(album);
        }
        return View(new Gallery());
    }

    [HttpPost("Galleries/CreateEdit/{id?}")]
    [RequirePermission("Website.Gallery")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GalleryCreateEdit(int? id, Gallery model, IFormFile? coverFile, CancellationToken ct)
    {
        if (coverFile != null && ValidateFile(coverFile, [".jpg", ".jpeg", ".png"]))
        {
            model.CoverImagePath = await _fileService.SaveAsync(coverFile, "gallery", ct);
        }

        if (id.HasValue && id > 0)
        {
            model.Id = id.Value;
            await _galleryService.UpdateAlbumAsync(model, ct);
            TempData["SuccessMessage"] = "Album updated successfully.";
        }
        else
        {
            await _galleryService.CreateAlbumAsync(model, ct);
            TempData["SuccessMessage"] = "Album created successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Galleries/Delete/{id}")]
    [RequirePermission("Website.Gallery")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GalleryDelete(int id, CancellationToken ct)
    {
        await _galleryService.DeleteAlbumAsync(id, ct);
        return Json(new { success = true });
    }

    [HttpPost("Galleries/UploadImages/{galleryId}")]
    [RequirePermission("Website.Gallery")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GalleryUploadImages(int galleryId, IFormFile[] files, CancellationToken ct)
    {
        foreach (var file in files)
        {
            if (ValidateFile(file, [".jpg", ".jpeg", ".png"]))
            {
                var path = await _fileService.SaveAsync(file, "gallery", ct);
                var img = new GalleryImage
                {
                    GalleryId = galleryId,
                    ImagePath = path,
                    Caption = Path.GetFileNameWithoutExtension(file.FileName)
                };
                await _galleryService.AddImageToAlbumAsync(img, ct);
            }
        }
        TempData["SuccessMessage"] = "Images uploaded successfully.";
        return RedirectToAction(nameof(GalleryCreateEdit), new { id = galleryId });
    }

    [HttpPost("Galleries/DeleteImage/{id}")]
    [RequirePermission("Website.Gallery")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GalleryDeleteImage(int id, CancellationToken ct)
    {
        await _galleryService.DeleteImageAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Contact Messages ──
    [HttpGet("Messages/List")]
    [RequirePermission("Website.View")]
    public async Task<IActionResult> MessagesList(CancellationToken ct)
    {
        var list = await _contactService.GetMessagesAsync(ct);
        return Json(list);
    }

    [HttpGet("Messages/Details/{id}")]
    [RequirePermission("Website.View")]
    public async Task<IActionResult> MessageDetails(int id, CancellationToken ct)
    {
        var message = await _contactService.GetMessageByIdAsync(id, ct);
        if (message == null) return NotFound();
        return View(message);
    }

    [HttpPost("Messages/MarkRead/{id}")]
    [RequirePermission("Website.View")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MessageMarkRead(int id, CancellationToken ct)
    {
        await _contactService.MarkAsReadAsync(id, ct);
        return Json(new { success = true });
    }

    [HttpPost("Messages/Delete/{id}")]
    [RequirePermission("Website.View")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MessageDelete(int id, CancellationToken ct)
    {
        await _contactService.DeleteMessageAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Email Templates ──
    [HttpGet("EmailTemplates/List")]
    public async Task<IActionResult> EmailTemplatesList(CancellationToken ct)
    {
        var list = await _emailTemplateService.GetTemplatesAsync(ct);
        return Json(list);
    }

    [HttpGet("EmailTemplates/CreateEdit/{id?}")]
    public async Task<IActionResult> EmailTemplateCreateEdit(int? id, CancellationToken ct)
    {
        ViewBag.EmailTemplateCategories = GetEmailTemplateCategories();
        if (id.HasValue && id > 0)
        {
            var tpl = await _emailTemplateService.GetTemplateByIdAsync(id.Value, ct);
            if (tpl == null) return NotFound();
            return View(tpl);
        }
        return View(new EmailTemplate());
    }

    public static List<string> GetEmailTemplateCategories()
    {
        return new List<string>
        {
            "HR",
            "Security", 
            "Attendance",
            "General"
        };
    }

    [HttpPost("EmailTemplates/CreateEdit/{id?}")]
    [RequirePermission("Website.EmailTemplates")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EmailTemplateCreateEdit(int? id, EmailTemplate model, CancellationToken ct)
    {
        if (id.HasValue && id > 0)
        {
            model.Id = id.Value;
            await _emailTemplateService.UpdateTemplateAsync(model, ct);
            TempData["SuccessMessage"] = "Email template updated successfully.";
        }
        else
        {
            await _emailTemplateService.CreateTemplateAsync(model, ct);
            TempData["SuccessMessage"] = "Email template created successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("EmailTemplates/Delete/{id}")]
    [RequirePermission("Website.EmailTemplates")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EmailTemplateDelete(int id, CancellationToken ct)
    {
        await _emailTemplateService.DeleteTemplateAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Website Pages ──
    [HttpGet("Pages/List")]
    [RequirePermission("Website.Pages")]
    public async Task<IActionResult> PagesList(CancellationToken ct)
    {
        var list = await _pageService.GetAllPagesAsync(ct);
        return Json(list);
    }

    [HttpGet("Pages/CreateEdit/{id?}")]
    [RequirePermission("Website.Pages")]
    public async Task<IActionResult> PageCreateEdit(int? id, CancellationToken ct)
    {
        if (id.HasValue && id > 0)
        {
            var page = await _pageService.GetPageByIdAsync(id.Value, ct);
            if (page == null) return NotFound();
            return View(page);
        }
        return View(new WebsitePage());
    }

    [HttpPost("Pages/CreateEdit/{id?}")]
    [RequirePermission("Website.Pages")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PageCreateEdit(int? id, WebsitePage model, CancellationToken ct)
    {
        if (id.HasValue && id > 0)
        {
            model.Id = id.Value;
            await _pageService.UpdatePageAsync(model, ct);
            TempData["SuccessMessage"] = "Page updated successfully.";
        }
        else
        {
            await _pageService.CreatePageAsync(model, ct);
            TempData["SuccessMessage"] = "Page created successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Pages/Delete/{id}")]
    [RequirePermission("Website.Pages")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PageDelete(int id, CancellationToken ct)
    {
        await _pageService.DeletePageAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Admission Fee Structures ──
    [HttpGet("AdmissionFees/List")]
    [RequirePermission("Website.AdmissionFees")]
    public async Task<IActionResult> AdmissionFeesList(CancellationToken ct)
    {
        var list = await _feeService.GetAllAsync(ct);
        return Json(list);
    }

    [HttpGet("AdmissionFees/CreateEdit/{id?}")]
    [RequirePermission("Website.AdmissionFees")]
    public async Task<IActionResult> AdmissionFeeCreateEdit(int? id, CancellationToken ct)
    {
        if (id.HasValue && id > 0)
        {
            var fee = await _feeService.GetByIdAsync(id.Value, ct);
            if (fee == null) return NotFound();
            return View(fee);
        }
        return View(new AdmissionFeeStructure());
    }

    [HttpPost("AdmissionFees/CreateEdit/{id?}")]
    [RequirePermission("Website.AdmissionFees")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdmissionFeeCreateEdit(int? id, AdmissionFeeStructure model, CancellationToken ct)
    {
        var existing = await _feeService.GetAllAsync(ct);
        var duplicate = existing.FirstOrDefault(f => f.SchoolClassId == model.SchoolClassId && f.Id != (id ?? 0));
        if (duplicate != null)
        {
            TempData["ErrorMessage"] = $"Fee structure for class '{duplicate.ClassName}' (ID: {model.SchoolClassId}) already exists.";
            return RedirectToAction(nameof(Index));
        }

        if (id.HasValue && id > 0)
        {
            model.Id = id.Value;
            await _feeService.UpdateAsync(model, ct);
            TempData["SuccessMessage"] = "Fee structure updated successfully.";
        }
        else
        {
            await _feeService.CreateAsync(model, ct);
            TempData["SuccessMessage"] = "Fee structure created successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("AdmissionFees/Delete/{id}")]
    [RequirePermission("Website.AdmissionFees")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdmissionFeeDelete(int id, CancellationToken ct)
    {
        await _feeService.DeleteAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Event Notifications ──
    [HttpGet("Events/Notifications")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> EventNotifications(CancellationToken ct)
    {
        var notifications = await _eventNotificationService.GetAllNotificationsAsync(ct);
        return Json(notifications);
    }

    [HttpGet("Events/Notifications/Dashboard")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> EventNotificationDashboard(CancellationToken ct)
    {
        var dashboard = await _eventNotificationService.GetDashboardAsync(ct);
        return Json(dashboard);
    }

    [HttpGet("Events/Notifications/Recent")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> RecentNotifications(int count = 10, CancellationToken ct = default)
    {
        var list = await _eventNotificationService.GetRecentNotificationsAsync(count, ct);
        return Json(list);
    }

    [HttpPost("Events/Notifications/Send/{eventId}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendEventNotification(int eventId, [FromForm] EventScope scope,
        int? classId, int? sectionId, int? groupId, string? studentIds, string? guardianIds,
        bool notifyGuardians = true, bool notifyStudents = false, bool primaryGuardianOnly = true,
        int? templateId = null, CancellationToken ct = default)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        int? userId = userIdStr != null && int.TryParse(userIdStr, out var uid) ? uid : null;

        var notification = await _eventNotificationService.CreateNotificationAsync(eventId, scope,
            classId, sectionId, groupId, studentIds, guardianIds,
            notifyGuardians, notifyStudents, primaryGuardianOnly,
            templateId, userId, ct);

        await _eventNotificationService.QueueNotificationAsync(notification.Id, ct);

        var settings = await _settingsService.GetSettingsAsync(ct);
        if (settings.SendImmediately)
        {
            await _eventNotificationService.SendNotificationAsync(notification.Id, ct);
        }

        TempData["SuccessMessage"] = "Event notification sent successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Events/Notifications/Resend/{notificationId}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendEventNotification(int notificationId, CancellationToken ct)
    {
        await _eventNotificationService.ResendNotificationAsync(notificationId, ct);
        TempData["SuccessMessage"] = "Failed notifications rescheduled for retry.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Events/Notifications/Preview/{notificationId}")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> PreviewEventNotification(int notificationId, CancellationToken ct)
    {
        var html = await _eventNotificationService.PreviewEmailAsync(notificationId, ct);
        return Content(html, "text/html");
    }

    [HttpGet("Events/Notifications/{notificationId}/Recipients")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> NotificationRecipients(int notificationId, CancellationToken ct)
    {
        var recipients = await _eventNotificationService.GetRecipientsAsync(notificationId, ct);
        return Json(recipients);
    }

    [HttpGet("Events/Notifications/{notificationId}")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> NotificationDetail(int notificationId, CancellationToken ct)
    {
        var notification = await _eventNotificationService.GetNotificationAsync(notificationId, ct);
        if (notification == null) return NotFound();
        return Json(notification);
    }

    // ── Event Notification Analytics ──
    [HttpGet("Events/Notifications/Analytics/{notificationId}")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> NotificationAnalytics(int notificationId, CancellationToken ct)
    {
        var analytics = await _eventNotificationService.GetAnalyticsAsync(notificationId, ct);
        return Json(analytics);
    }

    // ── Guardian Notification Preferences ──
    [HttpGet("Events/Notifications/Preferences/{guardianId}")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> GuardianNotificationPreferences(int guardianId, CancellationToken ct)
    {
        var pref = await _eventNotificationService.GetGuardianPreferenceAsync(guardianId, ct);
        return Json(pref != null ? pref : (object)new { });
    }

    [HttpPost("Events/Notifications/Preferences/{guardianId}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetGuardianNotificationPreferences(int guardianId,
        [FromForm] Models.Entities.Website.GuardainNotificationPreference model, CancellationToken ct)
    {
        await _eventNotificationService.SetGuardianPreferenceAsync(guardianId, model, ct);
        return Json(new { success = true });
    }

    [HttpPost("Events/Notifications/VerifyEmail/{guardianId}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyGuardianEmail(int guardianId, [FromForm] string email, CancellationToken ct)
    {
        await _eventNotificationService.VerifyGuardianEmailAsync(guardianId, email, ct);
        return Json(new { success = true });
    }

    // ── Event Notification Attachments ──
    [HttpGet("Events/Notifications/{notificationId}/Attachments")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> GetAttachments(int notificationId, CancellationToken ct)
    {
        var list = await _eventNotificationService.GetAttachmentsAsync(notificationId, ct);
        return Json(list);
    }

    [HttpPost("Events/Notifications/{notificationId}/Attachments")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadAttachment(int notificationId, IFormFile file, string? description = null, bool isInline = false, CancellationToken ct = default)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        if (file.Length > 10 * 1024 * 1024)
            return BadRequest("File too large (max 10MB).");

        var path = await _fileService.SaveAsync(file, "notifications", ct);
        await _eventNotificationService.AddAttachmentAsync(notificationId,
            file.FileName, path, file.ContentType, file.Length, description, isInline, ct);

        return Json(new { success = true, path });
    }

    [HttpPost("Events/Notifications/Attachments/Delete/{attachmentId}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAttachment(int attachmentId, CancellationToken ct)
    {
        await _eventNotificationService.RemoveAttachmentAsync(attachmentId, ct);
        return Json(new { success = true });
    }

    // ── Scheduled Notifications ──
    [HttpPost("Events/Notifications/Schedule/{notificationId}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ScheduleNotification(int notificationId, [FromForm] DateTime scheduledAt, CancellationToken ct)
    {
        await _eventNotificationService.ScheduleNotificationAsync(notificationId, scheduledAt, ct);
        TempData["SuccessMessage"] = "Notification scheduled successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Event Reminder Configs ──
    [HttpGet("Events/{eventId}/Reminders")]
    [RequirePermission("Website.Events")]
    public async Task<IActionResult> GetReminderConfigs(int eventId, CancellationToken ct)
    {
        var list = await _eventNotificationService.GetReminderConfigsAsync(eventId, ct);
        return Json(list);
    }

    [HttpPost("Events/{eventId}/Reminders/Create")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateReminderConfig(int eventId,
        [FromForm] int reminderValue, [FromForm] ReminderUnit reminderUnit, CancellationToken ct)
    {
        var config = await _eventNotificationService.CreateReminderConfigAsync(eventId, reminderValue, reminderUnit, ct);
        return Json(new { success = true, configId = config.Id });
    }

    [HttpPost("Events/Reminders/Update")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateReminderConfig([FromForm] ReminderConfig model, CancellationToken ct)
    {
        await _eventNotificationService.UpdateReminderConfigAsync(model, ct);
        return Json(new { success = true });
    }

    [HttpPost("Events/Reminders/Delete/{configId}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReminderConfig(int configId, CancellationToken ct)
    {
        await _eventNotificationService.DeleteReminderConfigAsync(configId, ct);
        return Json(new { success = true });
    }

    // ── Event Approval Workflow ──
    [HttpPost("Events/SubmitForApproval/{id}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitEventForApproval(int id, CancellationToken ct)
    {
        await _eventService.SubmitForApprovalAsync(id, ct);
        TempData["SuccessMessage"] = "Event submitted for approval.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Events/Approve/{id}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveEvent(int id, CancellationToken ct)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        int userId = userIdStr != null && int.TryParse(userIdStr, out var uid) ? uid : 0;
        await _eventService.ApproveEventAsync(id, userId, ct);
        TempData["SuccessMessage"] = "Event approved successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Events/Reject/{id}")]
    [RequirePermission("Website.Events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectEvent(int id, [FromForm] string reason, CancellationToken ct)
    {
        await _eventService.RejectEventAsync(id, reason, ct);
        TempData["SuccessMessage"] = "Event rejected.";
        return RedirectToAction(nameof(Index));
    }

    // ── Announcements ──
    [HttpGet("Announcements/List")]
    [RequirePermission("Website.View")]
    public async Task<IActionResult> AnnouncementsList(CancellationToken ct)
    {
        var list = await _announcementService.GetAllAnnouncementsAsync(ct);
        return Json(list);
    }

    [HttpGet("Announcements/CreateEdit/{id?}")]
    [RequirePermission("Website.Edit")]
    public async Task<IActionResult> AnnouncementCreateEdit(int? id, CancellationToken ct)
    {
        if (id.HasValue && id > 0)
        {
            var item = await _announcementService.GetAnnouncementByIdAsync(id.Value, ct);
            if (item == null) return NotFound();
            return View(item);
        }
        return View(new Announcement());
    }

    [HttpPost("Announcements/CreateEdit/{id?}")]
    [RequirePermission("Website.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AnnouncementCreateEdit(int? id, Announcement model, CancellationToken ct)
    {
        if (id.HasValue && id > 0)
        {
            model.Id = id.Value;
            await _announcementService.UpdateAnnouncementAsync(model, ct);
            TempData["SuccessMessage"] = "Announcement updated successfully.";
        }
        else
        {
            await _announcementService.CreateAnnouncementAsync(model, ct);
            TempData["SuccessMessage"] = "Announcement created successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Announcements/Delete/{id}")]
    [RequirePermission("Website.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AnnouncementDelete(int id, CancellationToken ct)
    {
        await _announcementService.DeleteAnnouncementAsync(id, ct);
        return Json(new { success = true });
    }

    // Helper file validator
    private static bool ValidateFile(IFormFile file, string[] allowedExtensions)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext)) return false;
        if (file.Length > 5 * 1024 * 1024) return false; // 5MB limit
        return true;
    }
}
