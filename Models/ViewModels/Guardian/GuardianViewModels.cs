namespace SchoolManagementSystem.Models.ViewModels.Guardian;

public class GuardianProfileViewModel
{
    public SchoolManagementSystem.Models.Entities.Guardian.Guardian Guardian { get; set; } = null!;
    public int ChildrenCount { get; set; }
}
