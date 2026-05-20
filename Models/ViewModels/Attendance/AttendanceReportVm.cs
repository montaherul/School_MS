using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolManagementSystem.Models.ViewModels.Attendance
{
    public class AttendanceReportVm
    {
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }
        public int? StudentId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? ReportType { get; set; } // Daily, Monthly, DateRange
        
        public IEnumerable<SelectListItem> Classes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Sections { get; set; } = new List<SelectListItem>();
    }
}
