using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Employee;

public class EmployeeSalaryDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }

    [Required, Display(Name = "Basic Salary")]
    public decimal BasicSalary { get; set; }

    [Display(Name = "House Rent")]
    public decimal HouseRent { get; set; }

    [Display(Name = "Medical Allowance")]
    public decimal MedicalAllowance { get; set; }

    [Display(Name = "Transport Allowance")]
    public decimal TransportAllowance { get; set; }

    [Display(Name = "Other Allowance")]
    public decimal OtherAllowance { get; set; }

    [Display(Name = "Deduction")]
    public decimal Deduction { get; set; }

    [Display(Name = "Total Salary")]
    public decimal TotalSalary { get; set; }

    [Required, Display(Name = "Effective From")]
    public DateTime EffectiveFrom { get; set; } = DateTime.Today;
}
