using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Controllers.Attendance
{
    [Authorize]
    public class AttendanceSettingController : Controller
    {
        private readonly IAttendanceSettingService _service;

        public AttendanceSettingController(IAttendanceSettingService service)
        {
            _service = service;
        }

        private bool CanManage() =>
            User.IsInRole("Super Admin") || User.IsInRole("Admin") || User.IsInRole("Principal");

        // GET: /AttendanceSetting
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var setting = await _service.GetOrCreateDefaultAsync(ct);
            return View(setting);
        }

        // GET: /AttendanceSetting/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(CancellationToken ct)
        {
            if (!CanManage()) return Forbid();
            var setting = await _service.GetOrCreateDefaultAsync(ct);
            return View(setting);
        }

        // POST: /AttendanceSetting/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AttendanceSetting model, CancellationToken ct)
        {
            if (!CanManage()) return Forbid();

            if (!ModelState.IsValid)
                return View(model);

            await _service.UpdateAsync(model, User.Identity?.Name ?? "system", ct);
            TempData["Success"] = "Attendance settings saved successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
