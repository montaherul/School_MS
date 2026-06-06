using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;

namespace SchoolManagementSystem.Repositories.Implementations.Academic;

public class AcademicCalendarRepository : BaseRepository<AcademicCalendar>,IAcademicCalendarRepository
{
    public AcademicCalendarRepository(SchoolDbContext context) : base(context)
    {
    }
}