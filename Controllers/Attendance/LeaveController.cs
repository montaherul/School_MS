using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models.ViewModels.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;

namespace SchoolManagementSystem.Controllers.Attendance
{
    [Authorize]
    public class LeaveController : Controller
    {
        private readonly ILeaveService _service;
        private readonly IWebHostEnvironment _env;

        public LeaveController(ILeaveService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            return View();
        }

        public async Task<IActionResult> Apply(CancellationToken ct)
        {
            var vm = new LeaveApplyVm();
            var types = await _service.GetActiveLeaveTypesAsync(ct);
            vm.LeaveTypes = new SelectList(types, "Id", "Name");
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(LeaveApplyVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var types = await _service.GetActiveLeaveTypesAsync(ct);
                vm.LeaveTypes = new SelectList(types, "Id", "Name");
                return View(vm);
            }

            try
            {
                string attachmentPath = string.Empty;
                if (vm.Attachment != null && vm.Attachment.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "leaves");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(vm.Attachment.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await vm.Attachment.CopyToAsync(fileStream, ct);
                    }
                    attachmentPath = "/uploads/leaves/" + uniqueFileName;
                }

                // In a real scenario, we'd get the actual EmployeeId from the logged-in User's claims. 
                // Using 1 as a placeholder for now since we are just scaffolding.
                int employeeId = 1; 
                
                await _service.ApplyLeaveAsync(vm, employeeId, attachmentPath, ct);
                TempData["SuccessMessage"] = "Leave application submitted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var types = await _service.GetActiveLeaveTypesAsync(ct);
                vm.LeaveTypes = new SelectList(types, "Id", "Name");
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingData(int page = 1, int size = 10, CancellationToken ct = default)
        {
            var result = await _service.GetPendingLeavesAsync(page, size, ct);
            return Json(new { data = result.Data, last_page = Math.Ceiling((double)result.TotalRecords / size), total_records = result.TotalRecords });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id, string remarks, CancellationToken ct)
        {
            try
            {
                await _service.ApproveLeaveAsync(id, User.Identity?.Name ?? "Admin", remarks, ct);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id, string remarks, CancellationToken ct)
        {
            try
            {
                await _service.RejectLeaveAsync(id, User.Identity?.Name ?? "Admin", remarks, ct);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
