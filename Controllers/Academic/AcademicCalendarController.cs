using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Controllers.Academic
{
    [Authorize]
    public class AcademicCalendarController : Controller
    {
        private readonly IAcademicCalendarService _service;
        private readonly IUnitOfWork _uow;

        public AcademicCalendarController(
            IAcademicCalendarService service,
            IUnitOfWork uow)
        {
            _service = service;
            _uow = uow;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(
            DateTime start,
            DateTime end,
            CancellationToken ct = default)
        {
            // If parameters are missing, default to current month
            if (start == default) start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            if (end == default) end = start.AddMonths(1).AddDays(-1);

            var days = await _service.GetCalendarDaysAsync(start, end, ct);
            
            // Map to the format suitable for Calendar UI/Tabulator
            var events = new List<object>();
            foreach (var day in days)
            {
                events.Add(new
                {
                    id = day.Id,
                    title = day.Title,
                    description = day.Description,
                    date = day.Date.ToString("yyyy-MM-dd"),
                    isHoliday = day.IsHoliday,
                    isWorkingDay = day.IsWorkingDay,
                    isExamDay = day.IsExamDay,
                    isEventDay = day.IsEventDay,
                    remarks = day.Remarks,
                    holidayType = day.HolidayType
                });
            }

            return Json(events);
        }

        private bool CanManageCalendar()
        {
            return User.IsInRole("Super Admin") || User.IsInRole("Admin") || User.IsInRole("Principal") || User.IsInRole("Assistant Head");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!CanManageCalendar()) return Forbid();

            var activeYear = await _uow.Repository<AcademicYear>().Query()
                .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted);

            ViewBag.AcademicYears = await _uow.Repository<AcademicYear>().Query()
                .Where(y => !y.IsDeleted)
                .ToListAsync();

            return View(new AcademicCalendar
            {
                Date = DateOnly.FromDateTime(DateTime.Today),
                AcademicYearId = activeYear?.Id ?? 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AcademicCalendar entity, CancellationToken ct)
        {
            if (!CanManageCalendar()) return Forbid();

            if (!ModelState.IsValid)
            {
                ViewBag.AcademicYears = await _uow.Repository<AcademicYear>().Query()
                    .Where(y => !y.IsDeleted)
                    .ToListAsync(ct);
                return View(entity);
            }

            entity.CreatedBy = User.Identity?.Name ?? "system";
            entity.CreatedAt = DateTime.UtcNow;

            await _service.CreateAsync(entity, ct);
            TempData["Success"] = "Calendar entry created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            if (!CanManageCalendar()) return Forbid();

            var entity = await _service.GetByIdAsync(id, ct);
            if (entity == null) return NotFound();

            ViewBag.AcademicYears = await _uow.Repository<AcademicYear>().Query()
                .Where(y => !y.IsDeleted)
                .ToListAsync(ct);

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AcademicCalendar entity, CancellationToken ct)
        {
            if (!CanManageCalendar()) return Forbid();

            if (!ModelState.IsValid)
            {
                ViewBag.AcademicYears = await _uow.Repository<AcademicYear>().Query()
                    .Where(y => !y.IsDeleted)
                    .ToListAsync(ct);
                return View(entity);
            }

            var existing = await _service.GetByIdAsync(entity.Id, ct);
            if (existing == null) return NotFound();

            existing.Date = entity.Date;
            existing.Title = entity.Title;
            existing.Description = entity.Description;
            existing.IsHoliday = entity.IsHoliday;
            existing.IsWorkingDay = entity.IsWorkingDay;
            existing.IsExamDay = entity.IsExamDay;
            existing.IsEventDay = entity.IsEventDay;
            existing.Remarks = entity.Remarks;
            existing.HolidayType = entity.HolidayType;
            existing.AcademicYearId = entity.AcademicYearId;
            existing.IsActive = entity.IsActive;
            existing.UpdatedBy = User.Identity?.Name ?? "system";
            existing.UpdatedAt = DateTime.UtcNow;

            await _service.UpdateAsync(existing, ct);
            TempData["Success"] = "Calendar entry updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            if (!CanManageCalendar()) return Forbid();

            await _service.DeleteAsync(id, ct);
            return Json(new { success = true, message = "Calendar entry deleted successfully." });
        }
    }
}