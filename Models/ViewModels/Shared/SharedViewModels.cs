namespace SchoolManagementSystem.Models.ViewModels.Shared;

public class StatCardModel
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = "bi-info-circle";
    public string? Color { get; set; }
    public string? Subtitle { get; set; }
    public double? TrendPercent { get; set; }
    public string? LinkUrl { get; set; }
}

public class EmptyStateModel
{
    public string Title { get; set; } = "No data found";
    public string Description { get; set; } = "There are no records to display at the moment.";
    public string Icon { get; set; } = "bi-folder-x";
    public string? ActionText { get; set; }
    public string? ActionUrl { get; set; }
}
