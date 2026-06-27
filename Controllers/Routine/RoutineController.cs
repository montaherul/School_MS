using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Routine;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Routine;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Services.Implementations.Routine;
using SchoolManagementSystem.Services.Interfaces.Routine;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Routine;

[Authorize]
public class RoutineController : Controller
{
    private readonly IRoutinePeriodService _periodService;
    private readonly IRoomService _roomService;
    private readonly ISubjectRequirementService _requirementService;
    private readonly IWorkingDayService _workingDayService;
    private readonly ITeacherAvailabilityService _availabilityService;
    private readonly IRoutineEntryService _entryService;
    private readonly IRoutineGenerationService _generationService;
    private readonly IRoutineVersionService _versionService;
    private readonly IRoutineEngineService _engineService;
    private readonly ISubstituteService _substituteService;
    private readonly IViewRendererService _viewRenderer;
    private readonly PlaywrightPdfEngine _playwright;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RoutineGenerationQueue _generationQueue;
    private readonly IMemoryCache _cache;
    private const string RoutineSettingsCacheKey = "RoutineSettings";

    public RoutineController(
        IRoutinePeriodService periodService,
        IRoomService roomService,
        ISubjectRequirementService requirementService,
        IWorkingDayService workingDayService,
        ITeacherAvailabilityService availabilityService,
        IRoutineEntryService entryService,
        IRoutineGenerationService generationService,
        IRoutineVersionService versionService,
        IRoutineEngineService engineService,
        ISubstituteService substituteService,
        IViewRendererService viewRenderer,
        PlaywrightPdfEngine playwright,
        IUnitOfWork unitOfWork,
        RoutineGenerationQueue generationQueue,
        IMemoryCache cache)
    {
        _periodService = periodService;
        _roomService = roomService;
        _requirementService = requirementService;
        _workingDayService = workingDayService;
        _availabilityService = availabilityService;
        _entryService = entryService;
        _generationService = generationService;
        _versionService = versionService;
        _engineService = engineService;
        _substituteService = substituteService;
        _viewRenderer = viewRenderer;
        _playwright = playwright;
        _unitOfWork = unitOfWork;
        _generationQueue = generationQueue;
        _cache = cache;
    }

    // ── Dashboard ────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        var currentYear = await _engineService.GetCurrentAcademicYearAsync(ct);
        var yearId = currentYear?.Id ?? 0;
        var data = await _engineService.GetDashboardAsync(yearId, ct);

        ViewBag.TotalTeachers = data.TotalTeachers;
        ViewBag.TotalRooms = data.TotalRooms;
        ViewBag.TotalClasses = data.TotalClasses;
        ViewBag.TotalSubjects = data.TotalSubjects;
        ViewBag.TotalEntries = data.TotalEntries;
        ViewBag.TotalConflicts = data.TotalConflicts;
        ViewBag.AcademicYearId = yearId;
        ViewBag.AcademicYearName = currentYear?.Name ?? "All Years";

        if (data.LastGenerationId.HasValue)
        {
            var lastGen = await _engineService.GetGenerationByIdAsync(data.LastGenerationId.Value, ct);
            ViewBag.LastGeneration = new
            {
                Status = data.LastGenerationStatus,
                GeneratedAt = lastGen?.CompletedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A",
                EntryCount = lastGen?.SuccessfulAssignments ?? 0
            };
        }

        if (data.PublishedVersionId.HasValue)
        {
            ViewBag.PublishedVersion = new
            {
                Name = data.PublishedVersionName ?? "Published",
                EntryCount = data.TotalEntries
            };
        }

        return View();
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetDashboardData(int academicYearId, CancellationToken ct)
    {
        var data = await _engineService.GetDashboardAsync(academicYearId, ct);
        return Json(data);
    }

    // ── Period Management ────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public IActionResult Periods()
    {
        return View();
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetPeriods(int page = 1, int size = 10, string? search = null, CancellationToken ct = default)
    {
        var result = await _periodService.GetPagedAsync(page, size, search, ct);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetActivePeriods(CancellationToken ct = default)
    {
        var periods = await _periodService.GetActivePeriodsAsync(ct);
        return Json(periods);
    }

    [HttpGet]
    [RequirePermission("Routine.Create")]
    public async Task<IActionResult> CreateEditPeriod(int? id, CancellationToken ct = default)
    {
        if (id.HasValue && id > 0)
        {
            var dto = await _periodService.GetForEditAsync(id.Value, ct);
            if (dto == null) return NotFound();
            return View(dto);
        }
        return View(new RoutinePeriodUpsertDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Create")]
    public async Task<IActionResult> CreateEditPeriod(RoutinePeriodUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            if (dto.Id > 0)
            {
                await _periodService.UpdateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Period updated successfully.";
            }
            else
            {
                await _periodService.CreateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Period created successfully.";
            }
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Periods));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Delete")]
    public async Task<IActionResult> DeletePeriod(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        try
        {
            await _periodService.DeleteAsync(id, userId, ct);
            TempData["SuccessMessage"] = "Period deleted successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Periods));
    }

    // ── Room Management ──────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public IActionResult Rooms()
    {
        return View();
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetRooms(int page = 1, int size = 10, string? search = null, CancellationToken ct = default)
    {
        var result = await _roomService.GetPagedAsync(page, size, search, ct);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    [RequirePermission("Routine.Create")]
    public async Task<IActionResult> CreateEditRoom(int? id, CancellationToken ct = default)
    {
        ViewBag.RoomTypes = await _roomService.GetRoomTypesAsync();
        if (id.HasValue && id > 0)
        {
            var dto = await _roomService.GetForEditAsync(id.Value, ct);
            if (dto == null) return NotFound();
            return View(dto);
        }
        return View(new RoomUpsertDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Create")]
    public async Task<IActionResult> CreateEditRoom(RoomUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            if (dto.Id > 0)
            {
                await _roomService.UpdateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Room updated successfully.";
            }
            else
            {
                await _roomService.CreateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Room created successfully.";
            }
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Rooms));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Delete")]
    public async Task<IActionResult> DeleteRoom(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        try
        {
            await _roomService.DeleteAsync(id, userId, ct);
            TempData["SuccessMessage"] = "Room deleted successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Rooms));
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetActiveRooms(CancellationToken ct = default)
    {
        var rooms = await _roomService.GetActiveRoomsAsync(ct);
        return Json(rooms);
    }

    // ── Subject Requirements ─────────────────────────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public IActionResult SubjectRequirements()
    {
        return View();
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetSubjectRequirements(int page = 1, int size = 10, string? search = null, CancellationToken ct = default)
    {
        var result = await _requirementService.GetPagedAsync(page, size, search, ct);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    [RequirePermission("Routine.Create")]
    public async Task<IActionResult> CreateEditSubjectRequirement(int? id, CancellationToken ct = default)
    {
        await PopulateLookupViewBags(ct);
        if (id.HasValue && id > 0)
        {
            var dto = await _requirementService.GetForEditAsync(id.Value, ct);
            if (dto == null) return NotFound();
            return View(dto);
        }
        return View(new SubjectRequirementUpsertDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Create")]
    public async Task<IActionResult> CreateEditSubjectRequirement(SubjectRequirementUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            if (dto.Id > 0)
            {
                await _requirementService.UpdateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Subject requirement updated successfully.";
            }
            else
            {
                await _requirementService.CreateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Subject requirement created successfully.";
            }
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(SubjectRequirements));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Delete")]
    public async Task<IActionResult> DeleteSubjectRequirement(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        try
        {
            await _requirementService.DeleteAsync(id, userId, ct);
            TempData["SuccessMessage"] = "Subject requirement deleted successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(SubjectRequirements));
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetRequirementsForClass(int classId, int? sectionId, int? groupId, CancellationToken ct = default)
    {
        var requirements = await _requirementService.GetByClassAsync(classId, sectionId, groupId, ct);
        return Json(requirements);
    }

    // ── Working Days ─────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public IActionResult WorkingDays()
    {
        return View();
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetWorkingDays(int academicYearId, CancellationToken ct = default)
    {
        var items = await _workingDayService.GetByAcademicYearAsync(academicYearId, ct);
        return Json(new { data = items });
    }

    [HttpGet]
    [RequirePermission("Routine.Create")]
    public async Task<IActionResult> CreateEditWorkingDay(int? id, CancellationToken ct = default)
    {
        ViewBag.AcademicYears = await _engineService.GetAcademicYearItemsAsync(ct);

        if (id.HasValue && id > 0)
        {
            var dto = await _workingDayService.GetForEditAsync(id.Value, ct);
            if (dto == null) return NotFound();
            return View(dto);
        }
        return View(new WorkingDayUpsertDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Create")]
    public async Task<IActionResult> CreateEditWorkingDay(WorkingDayUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            if (dto.Id > 0)
            {
                await _workingDayService.UpdateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Working day updated successfully.";
            }
            else
            {
                await _workingDayService.CreateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Working day created successfully.";
            }
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(WorkingDays));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Delete")]
    public async Task<IActionResult> DeleteWorkingDay(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        try
        {
            await _workingDayService.DeleteAsync(id, userId, ct);
            TempData["SuccessMessage"] = "Working day deleted successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(WorkingDays));
    }

    // ── Teacher Availability ─────────────────────────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public IActionResult TeacherAvailabilities()
    {
        return View();
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetTeacherAvailabilities(int page = 1, int size = 50, string? search = null, CancellationToken ct = default)
    {
        var result = await _availabilityService.GetPagedAsync(page, size, search, ct);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    [RequirePermission("Routine.Create")]
    public async Task<IActionResult> CreateEditTeacherAvailability(int? id, CancellationToken ct = default)
    {
        await PopulateLookupViewBags(ct);
        if (id.HasValue && id > 0)
        {
            var dto = await _availabilityService.GetForEditAsync(id.Value, ct);
            if (dto == null) return NotFound();
            return View(dto);
        }
        return View(new TeacherAvailabilityUpsertDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Create")]
    public async Task<IActionResult> CreateEditTeacherAvailability(TeacherAvailabilityUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            if (dto.Id > 0)
            {
                await _availabilityService.UpdateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Teacher availability updated successfully.";
            }
            else
            {
                await _availabilityService.CreateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Teacher availability created successfully.";
            }
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(TeacherAvailabilities));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Delete")]
    public async Task<IActionResult> DeleteTeacherAvailability(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        try
        {
            await _availabilityService.DeleteAsync(id, userId, ct);
            TempData["SuccessMessage"] = "Teacher availability deleted successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(TeacherAvailabilities));
    }

    // ── Main Timetable Grid ──────────────────────────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public IActionResult Timetable()
    {
        return View();
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetTimetableEntries(
        int academicYearId,
        int page = 1, int size = 50,
        int? classId = null, int? sectionId = null,
        int? groupId = null, int? teacherId = null,
        int? roomId = null, CancellationToken ct = default)
    {
        var result = await _entryService.GetGridAsync(academicYearId, classId, sectionId, groupId, teacherId, roomId, page, size, ct);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize), total_records = result.TotalItems });
    }

    [HttpPost]
    [RequirePermission("Routine.Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEntry([FromBody] RoutineEntryUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Invalid data." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _entryService.CreateAsync(dto, userId);
            });

            return Json(new { success = true, message = "Entry created successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Routine.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateEntry([FromBody] UpdateEntryRequestDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid || dto.Id <= 0)
            return Json(new { success = false, message = "Invalid data." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _entryService.UpdateEntryAsync(dto.Id, dto.RoomId, dto.RoutinePeriodId, dto.DayNumber, userId);
            });

            return Json(new { success = true, message = "Entry updated successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Routine.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SwapEntries([FromBody] SwapEntriesDto dto, CancellationToken ct = default)
    {
        if (dto.EntryId1 <= 0 || dto.EntryId2 <= 0)
            return Json(new { success = false, message = "Invalid entry IDs." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _entryService.SwapEntriesAsync(dto.EntryId1, dto.EntryId2, userId);
            });

            return Json(new { success = true, message = "Entries swapped successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Routine.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MoveEntry([FromBody] MoveEntryDto dto, CancellationToken ct = default)
    {
        if (dto.EntryId <= 0)
            return Json(new { success = false, message = "Invalid entry ID." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _entryService.MoveEntryAsync(dto.EntryId, dto.TargetPeriodId, dto.TargetDayNumber, userId);
            });

            return Json(new { success = true, message = "Entry moved successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Routine.Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkDeleteEntries([FromBody] BulkDeleteRequestDto dto, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        var results = new List<string>();

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var id in dto.Ids)
                {
                    await _entryService.DeleteAsync(id, userId);
                    results.Add($"Entry {id} deleted.");
                }
            });

            return Json(new { success = true, message = $"{dto.Ids.Count} entries deleted successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Routine.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkUpdateEntries([FromBody] BulkUpdateRequestDto dto, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var id in dto.Ids)
                {
                    await _entryService.UpdateEntryAsync(id, dto.RoomId, dto.RoutinePeriodId, dto.DayNumber, userId);
                }
            });

            return Json(new { success = true, message = $"{dto.Ids.Count} entries updated." });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Routine.Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEntry(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _entryService.DeleteAsync(id, userId);
            });

            return Json(new { success = true, message = "Entry deleted successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> ValidateEntry(
        int academicYearId, int classId, int dayNumber,
        int routinePeriodId, int roomId, int teacherId,
        int? sectionId, int? groupId, int? subjectId,
        int? id = null, CancellationToken ct = default)
    {
        var dto = new RoutineEntryUpsertDto
        {
            Id = id ?? 0,
            AcademicYearId = academicYearId,
            ClassId = classId,
            SectionId = sectionId,
            GroupId = groupId,
            SubjectId = subjectId ?? 0,
            TeacherId = teacherId,
            RoomId = roomId,
            RoutinePeriodId = routinePeriodId,
            DayNumber = dayNumber
        };

        var isValid = await _entryService.ValidateEntryAsync(dto, ct);
        return Json(new { isValid });
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetEntry(int id, CancellationToken ct)
    {
        var entry = await _entryService.GetForEditAsync(id, ct);
        if (entry == null)
            return Json(new { success = false, message = "Entry not found." });

        return Json(new { success = true, data = entry });
    }

    // ── Auto Generation ──────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Routine.Generate")]
    public async Task<IActionResult> Generation(CancellationToken ct = default)
    {
        ViewBag.AcademicYears = await _engineService.GetAcademicYearItemsAsync(ct);
        return View();
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetGenerations(int page = 1, int size = 50, string? search = null, CancellationToken ct = default)
    {
        var result = await _generationService.GetPagedAsync(page, size, search, ct);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpPost]
    [RequirePermission("Routine.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FullUpdateEntry([FromBody] RoutineEntryUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid || dto.Id <= 0)
            return Json(new { success = false, message = "Invalid data." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _entryService.UpdateAsync(dto, userId, ct);
            });

            return Json(new { success = true, message = "Entry updated successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Routine.Edit")]
    [ValidateAntiForgeryToken]
    public IActionResult Generate(int academicYearId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        _generationQueue.Enqueue(academicYearId, userId);
        TempData["SuccessMessage"] = "Routine generation has been queued for background processing.";
        return Json(new { success = true, message = "Routine generation has been queued." });
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetGenerationConflicts(int id, CancellationToken ct)
    {
        var conflicts = await _generationService.GetConflictsAsync(id, ct);
        return Json(conflicts);
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public IActionResult Conflicts()
    {
        return View();
    }

    // ── Substitute Management ────────────────────────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public IActionResult SubstituteAssignments()
    {
        return View();
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetSubstituteAssignments(int page = 1, int size = 10, string? search = null, CancellationToken ct = default)
    {
        var result = await _substituteService.GetPagedAsync(page, size, search, ct);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    [RequirePermission("Routine.Create")]
    public async Task<IActionResult> CreateEditSubstituteAssignment(int? id, CancellationToken ct = default)
    {
        if (id.HasValue && id > 0)
        {
            var dto = await _substituteService.GetForEditAsync(id.Value, ct);
            if (dto == null) return NotFound();
            return View(dto);
        }

        await PopulateSubstituteViewBags(ct);
        return View(new SubstituteAssignmentUpsertDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Create")]
    public async Task<IActionResult> CreateEditSubstituteAssignment(SubstituteAssignmentUpsertDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSubstituteViewBags(ct);
            return View(dto);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            if (dto.Id > 0)
            {
                await _substituteService.UpdateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Substitute assignment updated successfully.";
            }
            else
            {
                await _substituteService.CreateAsync(dto, userId, ct);
                TempData["SuccessMessage"] = "Substitute assignment created successfully.";
            }
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(SubstituteAssignments));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Delete")]
    public async Task<IActionResult> DeleteSubstituteAssignment(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        try
        {
            await _substituteService.DeleteAsync(id, userId, ct);
            TempData["SuccessMessage"] = "Substitute assignment deleted successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(SubstituteAssignments));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Edit")]
    public async Task<IActionResult> ApproveSubstituteAssignment(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        try
        {
            await _substituteService.ApproveAsync(id, userId, ct);
            TempData["SuccessMessage"] = "Substitute assignment approved successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(SubstituteAssignments));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Edit")]
    public async Task<IActionResult> DeclineSubstituteAssignment(int id, string reason, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        try
        {
            await _substituteService.DeclineAsync(id, reason, ct);
            TempData["SuccessMessage"] = "Substitute assignment declined.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(SubstituteAssignments));
    }

    private async Task PopulateLookupViewBags(CancellationToken ct)
    {
        ViewBag.Teachers = await _engineService.GetTeacherLookupAsync(ct);
        ViewBag.AcademicYears = await _engineService.GetAcademicYearItemsAsync(ct);
        ViewBag.Classes = await _engineService.GetClassItemsAsync(ct);
        ViewBag.Subjects = await _engineService.GetSubjectLookupAsync(ct);
        ViewBag.Periods = await _engineService.GetPeriodLookupAsync(ct);
        ViewBag.RoutineEntries = await _engineService.GetRoutineEntryLookupAsync(ct);
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetTeachers(CancellationToken ct)
    {
        var teachers = await _engineService.GetTeacherLookupAsync(ct);
        return Json(teachers);
    }

    private async Task PopulateSubstituteViewBags(CancellationToken ct)
    {
        ViewBag.RoutineEntries = await _engineService.GetRoutineEntryLookupAsync(ct);
        ViewBag.Teachers = await _engineService.GetTeacherLookupAsync(ct);
    }

    // ── Version Management ───────────────────────────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public IActionResult Versions()
    {
        return View();
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetVersions(int page = 1, int size = 50, string? search = null, CancellationToken ct = default)
    {
        var result = await _versionService.GetPagedAsync(page, size, search, ct);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpPost]
    [RequirePermission("Routine.Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVersion([FromBody] RoutineVersionUpsertDto dto, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            var versionId = await _versionService.CreateAsync(dto, userId);
            TempData["SuccessMessage"] = "Version created successfully.";
            return Json(new { success = true, versionId });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Routine.Publish")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PublishVersion(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            var result = await _versionService.PublishAsync(id, userId);
            if (result == null)
                return Json(new { success = false, message = "Version not found." });

            TempData["SuccessMessage"] = "Version published successfully.";
            return Json(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Routine.Publish")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveVersion(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            var result = await _versionService.ApproveAsync(id, userId);
            if (result == null)
                return Json(new { success = false, message = "Version not found." });

            TempData["SuccessMessage"] = "Version approved successfully.";
            return Json(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [RequirePermission("Routine.Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ArchiveVersion(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        try
        {
            await _versionService.ArchiveAsync(id, userId);
            return Json(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ── Settings ─────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public IActionResult Settings()
    {
        var data = GetRoutineSettings();
        return View(data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Routine.Manage")]
    public IActionResult Settings(int maxTeacherPeriodsPerDay, int maxTeacherPeriodsPerWeek, bool autoPublishAfterGeneration, bool enableConflictDetection, string generationAlgorithmVersion, int workingDaysPerWeek)
    {
        var data = new RoutineSettingsViewModel
        {
            MaxTeacherPeriodsPerDay = maxTeacherPeriodsPerDay,
            MaxTeacherPeriodsPerWeek = maxTeacherPeriodsPerWeek,
            AutoPublishAfterGeneration = autoPublishAfterGeneration,
            EnableConflictDetection = enableConflictDetection,
            GenerationAlgorithmVersion = generationAlgorithmVersion,
            WorkingDaysPerWeek = workingDaysPerWeek
        };

        _cache.Set(RoutineSettingsCacheKey, data, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24)));

        TempData["SuccessMessage"] = "Routine settings saved successfully.";
        return RedirectToAction(nameof(Settings));
    }

    private RoutineSettingsViewModel GetRoutineSettings()
    {
        if (_cache.TryGetValue<RoutineSettingsViewModel>(RoutineSettingsCacheKey, out var cached))
            return cached!;

        var defaults = new RoutineSettingsViewModel();
        _cache.Set(RoutineSettingsCacheKey, defaults, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24)));
        return defaults;
    }

    // ── Analytics ────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> Analytics(int academicYearId, CancellationToken ct)
    {
        var data = await _engineService.GetAnalyticsAsync(academicYearId, ct);
        return View(data);
    }

    // ── Views for Students / Teachers / Guardians ────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> StudentView(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var student = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.Section)
            .Include(s => s.StudentGroup)
            .FirstOrDefaultAsync(s => s.UserId == userId, ct);

        if (student == null)
            return View(new RoutineStudentViewModel());

        var currentYear = await _unitOfWork.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .Where(y => !y.IsDeleted && y.IsActive)
            .OrderByDescending(y => y.StartsOn)
            .FirstOrDefaultAsync(ct);

        var academicYearId = currentYear?.Id ?? 0;
        var entries = await _entryService.GetGridAsync(
            academicYearId, student.ClassId, student.SectionId, student.StudentGroupId, null, null, 1, 500, ct);
        var periods = await _periodService.GetActivePeriodsAsync(ct);
        var dayNames = new[] { "sat", "sun", "mon", "tue", "wed", "thu", "fri" };

        var grid = periods.Select(p => new Dictionary<string, object?>
        {
            ["periodName"] = p.Name,
            ["sat"] = (string?)null,
            ["sun"] = (string?)null,
            ["mon"] = (string?)null,
            ["tue"] = (string?)null,
            ["wed"] = (string?)null,
            ["thu"] = (string?)null,
            ["fri"] = (string?)null
        }).ToList();

        foreach (var entry in entries.Items)
        {
            var row = grid.FirstOrDefault(r => (string?)r["periodName"] == entry.PeriodName);
            if (row != null && entry.DayNumber >= 1 && entry.DayNumber <= 7)
            {
                row[dayNames[entry.DayNumber - 1]] = $"{entry.SubjectName}<br><small>{entry.TeacherName}<br>{entry.RoomNo}</small>";
            }
        }

        var todayDayNumber = ((int)DateTime.Today.DayOfWeek + 1) % 7 + 1;
        var todayEntries = entries.Items.Where(e => e.DayNumber == todayDayNumber).ToList();

        var model = new RoutineStudentViewModel
        {
            ClassName = student.Class?.Name ?? string.Empty,
            SectionName = student.Section?.Name,
            GroupName = student.StudentGroup?.Name,
            WeeklyGrid = grid.Cast<object>().ToList(),
            Statistics = new List<StatisticItem>
            {
                new() { IconClass = "fas fa-book", Icon = "📚", Value = entries.Items.Select(e => e.SubjectName).Distinct().Count(), Label = "Subjects" },
                new() { IconClass = "fas fa-clock", Icon = "⏰", Value = entries.Items.Count, Label = "Total Periods" },
                new() { IconClass = "fas fa-calendar-day", Icon = "📅", Value = todayEntries.Count, Label = "Today's Classes" }
            },
            TodayClasses = todayEntries.Select(e => new TodayStudentClassDto
            {
                PeriodName = e.PeriodName,
                SubjectName = e.SubjectName,
                TeacherName = e.TeacherName,
                RoomNo = e.RoomNo,
                StartTime = periods.FirstOrDefault(p => p.Name == e.PeriodName)?.StartTime ?? string.Empty,
                EndTime = periods.FirstOrDefault(p => p.Name == e.PeriodName)?.EndTime ?? string.Empty
            }).ToList()
        };

        return View(model);
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> TeacherView(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _unitOfWork.Repository<ApplicationUser>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user?.EmployeeId == null)
            return View(new RoutineTeacherViewModel());

        var teacher = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Teachers.Teacher>().Query()
            .AsNoTracking()
            .Include(t => t.Employee)
            .FirstOrDefaultAsync(t => t.EmployeeId == user.EmployeeId, ct);

        if (teacher == null)
            return View(new RoutineTeacherViewModel());

        var currentYear = await _unitOfWork.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .Where(y => !y.IsDeleted && y.IsActive)
            .OrderByDescending(y => y.StartsOn)
            .FirstOrDefaultAsync(ct);

        var academicYearId = currentYear?.Id ?? 0;
        var entries = await _entryService.GetGridAsync(
            academicYearId, null, null, null, teacher.Id, null, 1, 500, ct);
        var periods = await _periodService.GetActivePeriodsAsync(ct);
        var dayNames = new[] { "sat", "sun", "mon", "tue", "wed", "thu", "fri" };

        var grid = periods.Select(p => new Dictionary<string, object?>
        {
            ["periodName"] = p.Name,
            ["sat"] = (string?)null,
            ["sun"] = (string?)null,
            ["mon"] = (string?)null,
            ["tue"] = (string?)null,
            ["wed"] = (string?)null,
            ["thu"] = (string?)null,
            ["fri"] = (string?)null
        }).ToList();

        foreach (var entry in entries.Items)
        {
            var row = grid.FirstOrDefault(r => (string?)r["periodName"] == entry.PeriodName);
            if (row != null && entry.DayNumber >= 1 && entry.DayNumber <= 7)
            {
                row[dayNames[entry.DayNumber - 1]] = $"{entry.SubjectName}<br><small>{entry.ClassName} - {entry.SectionName}<br>{entry.RoomNo}</small>";
            }
        }

        var todayDayNumber = ((int)DateTime.Today.DayOfWeek + 1) % 7 + 1;
        var todayEntries = entries.Items.Where(e => e.DayNumber == todayDayNumber).ToList();

        var model = new RoutineTeacherViewModel
        {
            TeacherName = teacher.Employee?.FullName ?? string.Empty,
            TotalSubjects = entries.Items.Select(e => e.SubjectName).Distinct().Count(),
            TotalPeriodsPerWeek = entries.Items.Count,
            TotalClasses = entries.Items.Select(e => new { e.ClassId, e.SectionId }).Distinct().Count(),
            TotalWorkingDays = entries.Items.Select(e => e.DayNumber).Distinct().Count(),
            WeeklyGrid = grid.Cast<object>().ToList(),
            TodayClasses = todayEntries.Select(e => new TodayClassDto
            {
                PeriodName = e.PeriodName,
                ClassName = e.ClassName,
                SectionName = e.SectionName ?? string.Empty,
                SubjectName = e.SubjectName,
                RoomNo = e.RoomNo,
                StartTime = periods.FirstOrDefault(p => p.Name == e.PeriodName)?.StartTime ?? string.Empty,
                EndTime = periods.FirstOrDefault(p => p.Name == e.PeriodName)?.EndTime ?? string.Empty
            }).ToList()
        };

        return View(model);
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> ClassView(CancellationToken ct)
    {
        var academicYears = await _unitOfWork.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .Where(y => !y.IsDeleted)
            .OrderByDescending(y => y.StartsOn)
            .Select(y => new AcademicYearItem { Id = y.Id, Name = y.Name, IsActive = y.IsActive })
            .ToListAsync(ct);

        var classes = await _unitOfWork.Repository<SchoolClass>().Query()
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .Select(c => new ClassItem { Id = c.Id, Name = c.Name })
            .ToListAsync(ct);

        return View(new RoutineClassViewModel { AcademicYears = academicYears, Classes = classes });
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> RoomView(CancellationToken ct)
    {
        var rooms = await _unitOfWork.Repository<Room>().Query()
            .AsNoTracking()
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.RoomNo)
            .Select(r => new RoomItem { Id = r.Id, RoomNo = r.RoomNo, Name = r.Name })
            .ToListAsync(ct);

        return View(new RoutineRoomViewModel { Rooms = rooms });
    }

    // ── JSON Endpoints for Read-Only Views ────────────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetConflicts(int page = 1, int size = 10, bool? unresolvedOnly = null, CancellationToken ct = default)
    {
        var result = await _engineService.GetConflictsPagedAsync(page, size, unresolvedOnly, ct);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetSectionsByClass(int classId, CancellationToken ct)
    {
        var sections = await _unitOfWork.Repository<Section>().Query()
            .AsNoTracking()
            .Where(s => s.SchoolClassId == classId && !s.IsDeleted)
            .OrderBy(s => s.Name)
            .Select(s => new SectionItem { Id = s.Id, Name = s.Name })
            .ToListAsync(ct);
        return Json(sections);
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetGroupsByClass(int classId, CancellationToken ct)

    {
        var sectionIds = await _unitOfWork.Repository<Section>().Query()
            .AsNoTracking()
            .Where(s => s.SchoolClassId == classId && s.StudentGroupId != null && !s.IsDeleted)
            .Select(s => s.StudentGroupId!.Value)
            .Distinct()
            .ToListAsync(ct);

        var groups = await _unitOfWork.Repository<StudentGroup>().Query()
            .AsNoTracking()
            .Where(g => sectionIds.Contains(g.Id) && !g.IsDeleted)
            .OrderBy(g => g.DisplayOrder)
            .Select(g => new { g.Id, g.Name })
            .ToListAsync(ct);
        return Json(groups);
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetClassRoutine(int academicYearId, int classId, int? sectionId, int? groupId, CancellationToken ct)
    {
        var entries = await _entryService.GetGridAsync(academicYearId, classId, sectionId, groupId, null, null, 1, 500, ct);
        var periods = await _periodService.GetActivePeriodsAsync(ct);
        var dayNames = new[] { "sat", "sun", "mon", "tue", "wed", "thu", "fri" };

        var grid = periods.Select(p => new Dictionary<string, object?>
        {
            ["periodName"] = p.Name,
            ["sat"] = (string?)null,
            ["sun"] = (string?)null,
            ["mon"] = (string?)null,
            ["tue"] = (string?)null,
            ["wed"] = (string?)null,
            ["thu"] = (string?)null,
            ["fri"] = (string?)null
        }).ToList();

        foreach (var entry in entries.Items)
        {
            var row = grid.FirstOrDefault(r => (string?)r["periodName"] == entry.PeriodName);
            if (row != null && entry.DayNumber >= 1 && entry.DayNumber <= 7)
            {
                row[dayNames[entry.DayNumber - 1]] = $"{entry.SubjectName}<br><small>{entry.TeacherName}<br>{entry.RoomNo}</small>";
            }
        }

        return Json(grid);
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetRoomSchedule(int roomId, int? dayNumber, CancellationToken ct)
    {
        var room = await _roomService.GetForEditAsync(roomId, ct);
        var currentYear = await _unitOfWork.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .Where(y => !y.IsDeleted && y.IsActive)
            .OrderByDescending(y => y.StartsOn)
            .FirstOrDefaultAsync(ct);

        var academicYearId = currentYear?.Id ?? 0;
        var entries = await _entryService.GetGridAsync(academicYearId, null, null, null, null, roomId, 1, 500, ct);

        var activePeriods = await _periodService.GetActivePeriodsAsync(ct);
        var periodTimeMap = activePeriods.ToDictionary(p => p.Id, p => $"{p.StartTime} - {p.EndTime}");

        var dayNames = new[] { "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
        var dayMap = new Dictionary<int, string> { { 1, "Saturday" }, { 2, "Sunday" }, { 3, "Monday" }, { 4, "Tuesday" }, { 5, "Wednesday" }, { 6, "Thursday" }, { 7, "Friday" } };

        var schedules = entries.Items
            .Where(e => dayNumber == null || e.DayNumber == dayNumber.Value)
            .Select(e => new
            {
                dayName = e.DayName ?? (dayMap.ContainsKey(e.DayNumber) ? dayMap[e.DayNumber] : ""),
                periodName = e.PeriodName,
                className = e.ClassName,
                sectionName = e.SectionName ?? "",
                subjectName = e.SubjectName,
                teacherName = e.TeacherName,
                time = e.RoutinePeriodId > 0 && periodTimeMap.ContainsKey(e.RoutinePeriodId) ? periodTimeMap[e.RoutinePeriodId] : "",
                room = e.RoomNo
            })
            .OrderBy(e => e.dayName)
            .ToList();

        return Json(new
        {
            roomInfo = new
            {
                roomNo = room?.RoomNo ?? "",
                name = room?.Name ?? "",
                capacity = room?.Capacity ?? 0
            },
            schedules
        });
    }

    // ── Exports ──────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> ExportPdf(int academicYearId, int? classId, int? sectionId, int? groupId, int? teacherId, CancellationToken ct)
    {
        var entries = await _entryService.GetGridAsync(academicYearId, classId, sectionId, groupId, teacherId, null, 1, 5000, ct);
        var periods = await _periodService.GetActivePeriodsAsync(ct);
        var viewModel = new RoutinePrintViewModel
        {
            Entries = entries.Items.ToList(),
            Periods = periods
        };
        var html = await _viewRenderer.RenderToStringAsync("~/Views/Routine/_RoutinePrint.cshtml", viewModel);
        var pdf = _playwright.Convert(html, false);
        return File(pdf, "application/pdf", "Routine.pdf");
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> ExportExcel(int academicYearId, int? classId, int? sectionId, int? groupId, int? teacherId, CancellationToken ct)
    {
        var entries = await _entryService.GetGridAsync(academicYearId, classId, sectionId, groupId, teacherId, null, 1, 5000, ct);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Routine");
        ws.Cell(1, 1).Value = "Day";
        ws.Cell(1, 2).Value = "Period";
        ws.Cell(1, 3).Value = "Class";
        ws.Cell(1, 4).Value = "Section";
        ws.Cell(1, 5).Value = "Group";
        ws.Cell(1, 6).Value = "Subject";
        ws.Cell(1, 7).Value = "Teacher";
        ws.Cell(1, 8).Value = "Room";
        ws.Cell(1, 9).Value = "Type";

        var header = ws.Range(1, 1, 1, 9);
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = XLColor.FromArgb(0x1a, 0x73, 0xe8);
        header.Style.Font.FontColor = XLColor.White;

        int row = 2;
        foreach (var e in entries.Items)
        {
            ws.Cell(row, 1).Value = e.DayName;
            ws.Cell(row, 2).Value = e.PeriodName;
            ws.Cell(row, 3).Value = e.ClassName;
            ws.Cell(row, 4).Value = e.SectionName ?? "";
            ws.Cell(row, 5).Value = e.GroupName ?? "";
            ws.Cell(row, 6).Value = e.SubjectName;
            ws.Cell(row, 7).Value = e.TeacherName;
            ws.Cell(row, 8).Value = e.RoomNo;
            ws.Cell(row, 9).Value = e.IsLab ? "Lab" : "Theory";
            row++;
        }

        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Routine.xlsx");
    }

    private static string GetConflictDayName(int dayNumber) => dayNumber switch
    {
        1 => "Saturday",
        2 => "Sunday",
        3 => "Monday",
        4 => "Tuesday",
        5 => "Wednesday",
        6 => "Thursday",
        7 => "Friday",
        _ => "Unknown"
    };
}

// ── Inline DTOs ─────────────────────────────────────────────

public class SwapEntriesDto
{
    [Required]
    public int EntryId1 { get; set; }

    [Required]
    public int EntryId2 { get; set; }
}

public class MoveEntryDto
{
    [Required]
    public int EntryId { get; set; }

    public int TargetPeriodId { get; set; }

    public int TargetDayNumber { get; set; }
}

public class UpdateEntryRequestDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Required]
    public int RoutinePeriodId { get; set; }

    public int DayNumber { get; set; }
}

public class BulkDeleteRequestDto
{
    [Required]
    public List<int> Ids { get; set; } = [];
}

public class BulkUpdateRequestDto
{
    [Required]
    public List<int> Ids { get; set; } = [];

    public int RoomId { get; set; }

    public int RoutinePeriodId { get; set; }

    public int DayNumber { get; set; }
}
