using System;
using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Academic
{
    public class AcademicCalendar : BaseEntity
    {
        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }

        public DateOnly Date { get; set; }

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsHoliday { get; set; }

        public bool IsWorkingDay { get; set; } = true;

        public bool IsExamDay { get; set; }

        public bool IsEventDay { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [MaxLength(100)]
        public string? HolidayType { get; set; }

        public bool IsActive { get; set; } = true;
    }
}