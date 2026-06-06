namespace SchoolManagementSystem.Models.DTOs.Academic
{
    public class AcademicCalendarEventDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public string EventType { get; set; } = "";

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}
