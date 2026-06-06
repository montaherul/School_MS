using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;

namespace SchoolManagementSystem.Repositories.Implementations.Academic;

public class AcademicCalendarEventRepository: BaseRepository<AcademicCalendarEvent>,
      IAcademicCalendarEventRepository
{
    public AcademicCalendarEventRepository(SchoolDbContext context) : base(context)
    {
    }
}