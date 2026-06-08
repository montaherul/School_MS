using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class PassFailPolicy : IPassFailPolicy
{
    public (bool IsPassed, int FailedSubjectCount) DeterminePassFailStatus(
        IEnumerable<StudentSubjectResult> subjectResults, ResultSetting setting)
    {
        int failedSubjects = subjectResults.Count(r => !r.IsPassed);
        var failedMandatory = subjectResults.Count(r => !r.IsPassed && !r.IsOptionalSubject);
        var failedOptional = subjectResults.Count(r => !r.IsPassed && r.IsOptionalSubject);

        bool isPassed;
        if (setting.FailSubjectMode == FailSubjectMode.StrictFail)
        {
            isPassed = failedMandatory == 0;
        }
        else if (setting.FailSubjectMode == FailSubjectMode.ExcludeFail)
        {
            isPassed = failedMandatory <= setting.MaxFailedCompulsoryAllowed;
        }
        else
        {
            isPassed = failedMandatory == 0;
        }

        return (isPassed, failedSubjects);
    }
}
