using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Teacher;

public class TeacherAttendanceDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public string? TeacherNo { get; set; }
    
    [Required]
    public DateTime AttendanceDate { get; set; }
    
    [Required]
    public string Status { get; set; } = "Present"; // Present, Absent, Late, Half-Day, OnLeave
    
    public string? Remarks { get; set; }
}

public class TeacherLeaveDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    
    [Required]
    public string LeaveType { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required, MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Pending";
    public string? ApproverRemarks { get; set; }
}

public class TeacherPayrollDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    
    [Required]
    public DateTime MonthYear { get; set; }
    
    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary { get; set; }
    
    public string Status { get; set; } = "Unpaid";
}
