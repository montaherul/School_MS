namespace SchoolManagementSystem.Models.ViewModels.Exam;

public class SubjectMarkStructureBulkViewModel
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public List<SubjectMarkStructureItemViewModel> Components { get; set; } = [];
}

public class SubjectMarkStructureItemViewModel
{
    public int ComponentId { get; set; }
    public string ComponentName { get; set; } = string.Empty;
    public string ComponentCode { get; set; } = string.Empty;
    public decimal FullMarks { get; set; }
    public decimal PassMarks { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsEnabled { get; set; }
}
