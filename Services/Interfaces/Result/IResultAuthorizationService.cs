using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IResultAuthorizationService
{
    Task<bool> IsAuthorizedToEnterMarksAsync(int teacherId, int subjectId, int classId, int sectionId, int academicYearId, CancellationToken ct = default);
}
