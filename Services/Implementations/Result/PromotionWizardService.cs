using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Result;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class PromotionWizardService : IPromotionWizardService
{
    private readonly IPromotionPolicyService _promotionPolicyService;
    private readonly IRollGenerationService _rollGenerationService;
    private readonly IClassSubjectMappingService _classSubjectMappingService;
    private readonly IAcademicYearService _academicYearService;

    public PromotionWizardService(
        IPromotionPolicyService promotionPolicyService,
        IRollGenerationService rollGenerationService,
        IClassSubjectMappingService classSubjectMappingService,
        IAcademicYearService academicYearService)
    {
        _promotionPolicyService = promotionPolicyService;
        _rollGenerationService = rollGenerationService;
        _classSubjectMappingService = classSubjectMappingService;
        _academicYearService = academicYearService;
    }

    public async Task<PromotionWizardPreviewDto> GetPreviewAsync(
        int fromAcademicYearId, int fromClassId, int toClassId, int? examId, CancellationToken ct = default)
    {
        var evaluations = await _promotionPolicyService.EvaluateClassPromotionAsync(fromClassId, fromAcademicYearId, ct);

        var preview = new PromotionWizardPreviewDto
        {
            TotalStudents = evaluations.Count
        };

        foreach (var eval in evaluations)
        {
            var studentDto = new PromotionWizardStudentDto
            {
                StudentId = eval.StudentId,
                StudentName = eval.StudentName,
                GPA = eval.FinalGpa,
                AttendancePercent = (double)eval.AttendancePercentage,
                Reason = eval.Reason
            };

            switch (eval.Status)
            {
                case PromotionStatus.Promoted:
                    studentDto.Status = "Eligible";
                    preview.EligibleCount++;
                    break;
                case PromotionStatus.Repeat:
                    studentDto.Status = "Conditional";
                    preview.ConditionalCount++;
                    break;
                case PromotionStatus.Failed:
                    studentDto.Status = "Failed";
                    preview.FailedCount++;
                    break;
                default:
                    studentDto.Status = "Pending";
                    preview.InactiveCount++;
                    break;
            }

            preview.Students.Add(studentDto);
        }

        // Populate roll preview: sort eligible+conditional by GPA desc, assign roll 1,2,3...
        var rollCandidates = preview.Students
            .Where(s => s.Status == "Eligible" || s.Status == "Conditional")
            .OrderByDescending(s => s.GPA)
            .ToList();

        var rollConfig = await _rollGenerationService.GetConfigAsync(fromAcademicYearId, toClassId, ct);
        var strategy = rollConfig?.Strategy ?? Models.Enums.RollGenerationStrategy.MeritBased;
        preview.RollStrategy = strategy.ToString();

        int rank = 1;
        foreach (var candidate in rollCandidates)
        {
            preview.RollPreview.Add(new RollPreviewItem
            {
                Rank = rank,
                StudentId = candidate.StudentId,
                StudentName = candidate.StudentName,
                Gpa = candidate.GPA,
                ProposedRoll = rank,
                Strategy = preview.RollStrategy
            });
            rank++;
        }

        // Populate subject preview from ClassSubjectMapping for the target class
        var subjectResult = await _classSubjectMappingService.GetPagedAsync(
            1, 100, toClassId, null, null, ct);
        foreach (var item in subjectResult.Items)
        {
            preview.SubjectPreview.Add(new SubjectPreviewItem
            {
                SubjectId = item.SubjectId,
                SubjectName = item.SubjectNameEn,
                SubjectCode = item.SubjectCode,
                ClassName = item.SchoolClassName,
                IsMandatory = item.IsMandatory
            });
        }

        return preview;
    }

    public async Task<PromotionWizardExecuteResult> ExecuteAsync(
        PromotionWizardExecuteRequest request, int userId, CancellationToken ct = default)
    {
        var result = new PromotionWizardExecuteResult();

        try
        {
            var executionResult = await _promotionPolicyService.ExecutePromotionAsync(
                request.FromClassId, request.FromAcademicYearId, userId, ct);

            result.PromotedCount = executionResult.PromotedCount;
            result.RepeatCount = executionResult.RepeatCount;
            result.FailedCount = executionResult.FailedCount;

            if (executionResult.Errors.Count > 0)
            {
                result.Warnings.AddRange(executionResult.Errors);
            }

            if (request.AutoGenerateRoll && executionResult.PromotedCount > 0)
            {
                try
                {
                    var rollResults = await _rollGenerationService.GenerateRollsAsync(
                        request.ToAcademicYearId, request.ToClassId, ct);
                    result.RollCount = rollResults.Count;
                }
                catch (Exception ex)
                {
                    result.Warnings.Add($"Roll generation failed: {ex.Message}");
                }
            }

            if (request.AutoAssignSubjects)
            {
                try
                {
                    await _classSubjectMappingService.SeedMappingsAsync(ct);
                }
                catch (Exception ex)
                {
                    result.Warnings.Add($"Subject mapping seeding failed: {ex.Message}");
                }
            }

            result.Success = true;
            result.Message = $"Promotion completed. Promoted: {result.PromotedCount}, Repeat: {result.RepeatCount}, Failed: {result.FailedCount}";
            if (result.RollCount > 0)
                result.Message += $", Rolls generated: {result.RollCount}";
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = $"Promotion execution failed: {ex.Message}";
        }

        return result;
    }
}
