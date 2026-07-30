using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IBillingService
{
    Task<BillingCategoryInfoDto> GetCategoryInfoAsync(string categoryName);
    Task<List<BillingStudentDto>> SearchStudentsAsync(string? term);
    Task<string> CreateBillingInvoiceAsync(int studentId, string categoryName, List<BillingItemDto> items, DateOnly dueDate, string? remarks, string createdBy);
}

public class BillingCategoryInfoDto
{
    public int? CategoryId { get; set; }
    public List<BillingFeeTypeDto> FeeTypes { get; set; } = [];
}

public class BillingFeeTypeDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int DisplayOrder { get; set; }
}

public class BillingStudentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string StudentNo { get; set; } = "";
}


