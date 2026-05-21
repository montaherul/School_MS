using SchoolManagementSystem.Models.Entities.Assignment;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Services.Interfaces.Assignment;

public interface IAssignmentService : IBaseService<AssignmentTask>
{
    Task<IQueryable<AssignmentTask>> ApplySecurityFiltersAsync(IQueryable<AssignmentTask> query, int userId, bool isStudent, bool isTeacher, bool isAdmin, CancellationToken ct = default);
}

