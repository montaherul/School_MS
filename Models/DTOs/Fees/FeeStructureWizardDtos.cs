namespace SchoolManagementSystem.Models.DTOs.Fees;

public class FeeStructureWizardDto
{
    public int Step { get; set; } = 1;
    public int AcademicYearId { get; set; }
    public int SchoolClassId { get; set; }
    public int? SectionId { get; set; }
    public int? StudentGroupId { get; set; }
    public List<FeeHeadItemDto> FeeHeads { get; set; } = [];
    public List<FeeDiscountItemDto> Discounts { get; set; } = [];
    public List<FeeFineRuleItemDto> FineRules { get; set; } = [];
    public bool IsActive { get; set; } = true;
}

public class FeeHeadItemDto
{
    public int FeeCategoryId { get; set; }
    public string FeeName { get; set; } = "";
    public decimal Amount { get; set; }
    public int Frequency { get; set; } = 1;
    public int? DueDay { get; set; }
    public bool IsRecurring { get; set; } = true;
}

public class FeeDiscountItemDto
{
    public string Name { get; set; } = "";
    public int DiscountType { get; set; } = 1;
    public decimal Value { get; set; }
    public int? FeeCategoryId { get; set; }
    public int? FeeStructureId { get; set; }
}

public class FeeFineRuleItemDto
{
    public string Name { get; set; } = "";
    public int GraceDays { get; set; }
    public decimal FinePerDay { get; set; }
}
