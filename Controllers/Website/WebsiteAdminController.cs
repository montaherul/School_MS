using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Helpers.Files;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Services.Interfaces.Website;

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
        _fileService = fileService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    // ── School Settings ──
    [HttpGet("Settings")]
    public async Task<IActionResult> Settings(CancellationToken ct)
    {
        var settings = await _settingsService.GetSettingsAsync(ct);
        return View(settings);
    }

    [HttpPost("Settings")]
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
    public async Task<IActionResult> SlidersList(CancellationToken ct)
    {
        var list = await _sliderService.GetAllSlidersAsync(ct);
        return Json(list);
    }

    [HttpGet("Sliders/CreateEdit/{id?}")]
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SliderDelete(int id, CancellationToken ct)
    {
        await _sliderService.DeleteSliderAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Notices ──
    [HttpGet("Notices/List")]
    public async Task<IActionResult> NoticesList(string? search, int page = 1, int size = 10, CancellationToken ct = default)
    {
        if (size <= 0) size = 10;
        var (notices, total) = await _noticeService.GetPagedNoticesAsync(search, page, size, ct);
        return Json(new { data = notices, last_page = (int)Math.Ceiling((double)total / size), total = total });
    }

    [HttpGet("Notices/CreateEdit/{id?}")]
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NoticeDelete(int id, CancellationToken ct)
    {
        await _noticeService.DeleteNoticeAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Events ──
    [HttpGet("Events/List")]
    public async Task<IActionResult> EventsList(CancellationToken ct)
    {
        var events = await _eventService.GetAllEventsAsync(ct);
        return Json(events);
    }

    [HttpGet("Events/CreateEdit/{id?}")]
    public async Task<IActionResult> EventCreateEdit(int? id, CancellationToken ct)
    {
        if (id.HasValue && id > 0)
        {
            var ev = await _eventService.GetEventByIdAsync(id.Value, ct);
            if (ev == null) return NotFound();
            return View(ev);
        }
        return View(new Event { EventDate = DateTime.Today.AddDays(7) });
    }

    [HttpPost("Events/CreateEdit/{id?}")]
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EventDelete(int id, CancellationToken ct)
    {
        await _eventService.DeleteEventAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Galleries ──
    [HttpGet("Galleries/List")]
    public async Task<IActionResult> GalleriesList(CancellationToken ct)
    {
        var list = await _galleryService.GetAllAlbumsAsync(ct);
        return Json(list);
    }

    [HttpGet("Galleries/CreateEdit/{id?}")]
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GalleryDelete(int id, CancellationToken ct)
    {
        await _galleryService.DeleteAlbumAsync(id, ct);
        return Json(new { success = true });
    }

    [HttpPost("Galleries/UploadImages/{galleryId}")]
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GalleryDeleteImage(int id, CancellationToken ct)
    {
        await _galleryService.DeleteImageAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Contact Messages ──
    [HttpGet("Messages/List")]
    public async Task<IActionResult> MessagesList(CancellationToken ct)
    {
        var list = await _contactService.GetMessagesAsync(ct);
        return Json(list);
    }

    [HttpPost("Messages/MarkRead/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MessageMarkRead(int id, CancellationToken ct)
    {
        await _contactService.MarkAsReadAsync(id, ct);
        return Json(new { success = true });
    }

    [HttpPost("Messages/Delete/{id}")]
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
        if (id.HasValue && id > 0)
        {
            var tpl = await _emailTemplateService.GetTemplateByIdAsync(id.Value, ct);
            if (tpl == null) return NotFound();
            return View(tpl);
        }
        return View(new EmailTemplate());
    }

    [HttpPost("EmailTemplates/CreateEdit/{id?}")]
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EmailTemplateDelete(int id, CancellationToken ct)
    {
        await _emailTemplateService.DeleteTemplateAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Website Pages ──
    [HttpGet("Pages/List")]
    public async Task<IActionResult> PagesList(CancellationToken ct)
    {
        var list = await _pageService.GetAllPagesAsync(ct);
        return Json(list);
    }

    [HttpGet("Pages/CreateEdit/{id?}")]
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PageDelete(int id, CancellationToken ct)
    {
        await _pageService.DeletePageAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Admission Fee Structures ──
    [HttpGet("AdmissionFees/List")]
    public async Task<IActionResult> AdmissionFeesList(CancellationToken ct)
    {
        var list = await _feeService.GetAllAsync(ct);
        return Json(list);
    }

    [HttpGet("AdmissionFees/CreateEdit/{id?}")]
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdmissionFeeCreateEdit(int? id, AdmissionFeeStructure model, CancellationToken ct)
    {
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdmissionFeeDelete(int id, CancellationToken ct)
    {
        await _feeService.DeleteAsync(id, ct);
        return Json(new { success = true });
    }

    // ── Announcements ──
    [HttpGet("Announcements/List")]
    public async Task<IActionResult> AnnouncementsList(CancellationToken ct)
    {
        var list = await _announcementService.GetAllAnnouncementsAsync(ct);
        return Json(list);
    }

    [HttpGet("Announcements/CreateEdit/{id?}")]
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
