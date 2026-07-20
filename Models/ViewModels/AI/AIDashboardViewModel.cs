using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Models.ViewModels.AI;

public class AIDashboardViewModel
{
    public AIDashboardStatsDto Stats { get; set; } = new();
    public List<AIRequestChartPoint> RequestsPerHour { get; set; } = [];
    public List<AICostChartPoint> DailyCost { get; set; } = [];
    public List<TopUserDto> TopUsers { get; set; } = [];
    public List<TopSubjectDto> TopSubjects { get; set; } = [];
    public List<TopPromptDto> TopPrompts { get; set; } = [];
}
