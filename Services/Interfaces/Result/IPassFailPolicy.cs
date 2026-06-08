using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IPassFailPolicy
{
    (bool IsPassed, int FailedSubjectCount) DeterminePassFailStatus(
        IEnumerable<StudentSubjectResult> subjectResults, ResultSetting setting);
}
