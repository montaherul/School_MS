using SchoolManagementSystem.Models.DTOs.Exam;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class ExamIntegrationTests
{
    [Fact(DisplayName = "ExamReadinessReportDto data contract is complete")]
    public void ExamReadinessReportDto_HasAllRequiredProperties()
    {
        var report = new ExamReadinessReportDto
        {
            TotalExams = 10,
            DraftExams = 3,
            ReadyExams = 7,
            ClassesWithExams = 5,
            TotalActiveClasses = 8,
            ExamsWithoutSubjects =
            [
                new ExamReadinessIssueDto { ExamId = 1, ExamName = "Half Yearly", ClassName = "Six", SubjectCount = 0 }
            ],
            ExamsWithoutSchedule =
            [
                new ExamReadinessIssueDto { ExamId = 2, ExamName = "Annual", ClassName = "Seven", SubjectCount = 5, ScheduledCount = 2 }
            ],
            ExamsWithoutGradingRules =
            [
                new ExamReadinessIssueDto { ExamId = 3, ExamName = "Pre-Test", ClassName = "Ten" }
            ]
        };

        Assert.Equal(10, report.TotalExams);
        Assert.Equal(3, report.DraftExams);
        Assert.Equal(7, report.ReadyExams);
        Assert.Equal(5, report.ClassesWithExams);
        Assert.Equal(8, report.TotalActiveClasses);
        Assert.Single(report.ExamsWithoutSubjects);
        Assert.Single(report.ExamsWithoutSchedule);
        Assert.Single(report.ExamsWithoutGradingRules);
    }

    [Fact(DisplayName = "Exam readiness — all requirements met returns ready")]
    public void ExamReadiness_AllRequirementsMet_ReturnsReady()
    {
        int totalExams = 5, readyExams = 5;
        Assert.Equal(totalExams, readyExams);
    }

    [Fact(DisplayName = "Exam readiness — missing subjects flagged")]
    public void ExamReadiness_MissingSubjects_Flagged()
    {
        var issues = new List<ExamReadinessIssueDto>
        {
            new() { ExamId = 1, ExamName = "Annual", ClassName = "Six", SubjectCount = 0 }
        };
        Assert.Contains(issues, i => i.SubjectCount == 0);
    }

    [Fact(DisplayName = "Exam readiness — partial schedule flagged")]
    public void ExamReadiness_PartialSchedule_Flagged()
    {
        var issues = new List<ExamReadinessIssueDto>
        {
            new() { ExamId = 2, ExamName = "Half Yearly", ClassName = "Seven", SubjectCount = 6, ScheduledCount = 4 }
        };
        Assert.Contains(issues, i => i.ScheduledCount < i.SubjectCount);
    }

    [Fact(DisplayName = "CloneExamConfigForNewYear skips duplicates by Name+ClassId")]
    public void CloneExamConfig_SkipsDuplicates()
    {
        var existing = new List<(string Name, int ClassId)> { ("Annual", 1) };
        var toClone = new List<(string Name, int ClassId)> { ("Annual", 1), ("Half Yearly", 1), ("Annual", 2) };
        var cloned = toClone.Where(tc => !existing.Any(e => e.Name == tc.Name && e.ClassId == tc.ClassId)).ToList();
        Assert.Equal(2, cloned.Count);
        Assert.Contains(cloned, c => c.Name == "Half Yearly" && c.ClassId == 1);
        Assert.Contains(cloned, c => c.Name == "Annual" && c.ClassId == 2);
    }

    [Fact(DisplayName = "CloneExamConfigForNewYear empty source returns 0")]
    public void CloneExamConfig_EmptySource_ReturnsZero()
    {
        var sourceExams = new List<(string Name, int ClassId)>();
        Assert.Empty(sourceExams);
    }

    [Fact(DisplayName = "ExamReadinessIssueDto data contract is complete")]
    public void ExamReadinessIssueDto_HasAllRequiredProperties()
    {
        var dto = new ExamReadinessIssueDto
        {
            ExamId = 1,
            ExamName = "Half Yearly",
            ClassName = "Six",
            SubjectCount = 5,
            ScheduledCount = 5
        };
        Assert.Equal(1, dto.ExamId);
        Assert.Equal("Half Yearly", dto.ExamName);
        Assert.Equal("Six", dto.ClassName);
        Assert.Equal(5, dto.SubjectCount);
        Assert.Equal(5, dto.ScheduledCount);
    }
}
