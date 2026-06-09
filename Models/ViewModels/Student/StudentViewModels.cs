using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Student;

namespace SchoolManagementSystem.Models.ViewModels.Student;

public class StudentIndexViewModel
{
    public List<SchoolClassListItemDto> Classes { get; set; } = new();
}

public class IdCardPrintViewModel
{
    public List<StudentUpsertDto> Students { get; set; } = [];
    public bool IsBulk => Students.Count > 1;

    public string SchoolLogoPath { get; set; } = string.Empty;
    public string SchoolNameEn { get; set; } = string.Empty;
    public string SchoolNameBn { get; set; } = string.Empty;
    public string SchoolEIIN { get; set; } = string.Empty;
    public string SchoolWebsite { get; set; } = string.Empty;
    public string SchoolAddress { get; set; } = string.Empty;
    public string SchoolPhone { get; set; } = string.Empty;
    public string SchoolEmail { get; set; } = string.Empty;
    public string PrincipalName { get; set; } = string.Empty;
    public string PrincipalSignaturePath { get; set; } = string.Empty;
}

public class IdCardBulkFilterViewModel
{
    public List<SchoolClassListItemDto> Classes { get; set; } = [];
    public int? ClassId { get; set; }
    public int? GroupId { get; set; }
    public int? SectionId { get; set; }
}
