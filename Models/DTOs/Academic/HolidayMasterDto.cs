namespace SchoolManagementSystem.Models.DTOs.Academic;

public class HolidayMasterDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameBn { get; set; }
    public string HolidayType { get; set; } = string.Empty;
    public DateOnly HolidayDate { get; set; }
    public bool IsRecurring { get; set; }
    public string? Religion { get; set; }
    public string CountryCode { get; set; } = "BD";
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class HolidayMasterUpsertDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameBn { get; set; }
    public string HolidayType { get; set; } = string.Empty;
    public DateOnly HolidayDate { get; set; }
    public bool IsRecurring { get; set; }
    public string? Religion { get; set; }
    public string CountryCode { get; set; } = "BD";
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
