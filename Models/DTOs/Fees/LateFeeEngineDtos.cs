namespace SchoolManagementSystem.Models.DTOs.Fees;

public class LateFeeEngineResultDto
{
    public int InvoicesProcessed { get; set; }
    public decimal TotalLateFeeApplied { get; set; }
    public List<string> Errors { get; set; } = [];
}
