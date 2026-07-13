using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Models.ViewModels.Fees;

public class FeeStructureWizardViewModel
{
    public FeeStructureWizardDto Wizard { get; set; } = new();
    public List<SelectListItem> AcademicYears { get; set; } = [];
    public List<SelectListItem> SchoolClasses { get; set; } = [];
    public List<SelectListItem> Sections { get; set; } = [];
    public List<SelectListItem> StudentGroups { get; set; } = [];
    public List<SelectListItem> FeeCategories { get; set; } = [];
}
