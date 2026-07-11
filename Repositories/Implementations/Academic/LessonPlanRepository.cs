using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;

namespace SchoolManagementSystem.Repositories.Implementations.Academic;

public class LessonPlanRepository : BaseRepository<LessonPlan>, ILessonPlanRepository
{
    public LessonPlanRepository(SchoolDbContext db) : base(db) { }
}
