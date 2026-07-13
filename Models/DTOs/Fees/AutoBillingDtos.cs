namespace SchoolManagementSystem.Models.DTOs.Fees;

public class AutoBillingResultDto
{
    public int InvoicesGenerated { get; set; }
    public int StudentsBilled { get; set; }
    public decimal TotalAmount { get; set; }
    public string? ErrorMessage { get; set; }
    public bool Success => string.IsNullOrEmpty(ErrorMessage);
}

public class AutoAssignmentResultDto
{
    public int AssignmentsCreated { get; set; }
    public string? ErrorMessage { get; set; }
    public bool Success => string.IsNullOrEmpty(ErrorMessage);
}

public class FeeMigrationResultDto
{
    public int OldAssignmentsDeactivated { get; set; }
    public int NewAssignmentsCreated { get; set; }
    public string? ErrorMessage { get; set; }
    public bool Success => string.IsNullOrEmpty(ErrorMessage);
}

public class FeeCopyResultDto
{
    public int StructuresCopied { get; set; }
    public string? ErrorMessage { get; set; }
    public bool Success => string.IsNullOrEmpty(ErrorMessage);
}
