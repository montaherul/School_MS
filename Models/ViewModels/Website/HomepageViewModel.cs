using System.Collections.Generic;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Models.ViewModels.Website;

public class HomepageViewModel
{
    public SchoolSetting Settings { get; set; } = new();
    public IReadOnlyList<Slider> Sliders { get; set; } = [];
    public IReadOnlyList<Notice> LatestNotices { get; set; } = [];
    public IReadOnlyList<Event> UpcomingEvents { get; set; } = [];
    public IReadOnlyList<Gallery> Albums { get; set; } = [];
    public IReadOnlyList<Announcement> Announcements { get; set; } = [];
    public IReadOnlyList<AcademicCalendar> UpcomingCalendarEvents { get; set; } = [];
    public IReadOnlyList<AdmissionFeeStructure> AdmissionFees { get; set; } = [];
    
    // School Statistics
    public int StudentCount { get; set; }
    public int TeacherCount { get; set; }
    public int EmployeeCount { get; set; }
    public int ClassCount { get; set; }
}
