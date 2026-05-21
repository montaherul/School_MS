using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolManagementSystem.Models.ViewModels.Attendance
{
    public class LeaveApplyVm
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }

        [Required]
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; } = DateTime.Today;

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Supporting Document (Optional)")]
        public IFormFile? Attachment { get; set; }

        public string? ExistingAttachmentPath { get; set; }

        public IEnumerable<SelectListItem> LeaveTypes { get; set; } = new List<SelectListItem>();
    }
}
