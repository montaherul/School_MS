namespace SchoolManagementSystem.Models.DTOs.Employee;

public class EmployeeDto
{
    public long Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? BloodGroup { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public string? PresentVillage { get; set; }
    public string? PresentPostOffice { get; set; }
    public string? PresentThana { get; set; }
    public string? PresentDistrict { get; set; }
    public string? PermanentVillage { get; set; }
    public string? PermanentPostOffice { get; set; }
    public string? PermanentThana { get; set; }
    public string? PermanentDistrict { get; set; }
    public DateTime JoiningDate { get; set; }
    public decimal Salary { get; set; }
    public string? PhotoPath { get; set; }
    public bool IsActive { get; set; }
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public long DesignationId { get; set; }
    public string DesignationName { get; set; } = string.Empty;
}

public class EmployeeListItemDto
{
    public long Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string DesignationName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? PhotoPath { get; set; }
}

public class DepartmentDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class DesignationDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class EmployeeAttendanceDto
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public DateTime AttendanceDate { get; set; }
    public SchoolManagementSystem.Models.Enums.AttendanceStatus Status { get; set; } = SchoolManagementSystem.Models.Enums.AttendanceStatus.Present;
    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
    public string? Remarks { get; set; }
}

public class EmployeeAttendanceSummaryDto
{
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalLate { get; set; }
    public int TotalLeave { get; set; }
    public double AttendancePercentage { get; set; }
    public DateTime? LastAttendanceDate { get; set; }
}

public class LeaveTypeDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DefaultDaysPerYear { get; set; }
    public bool IsPaid { get; set; }
    public string? ColorCode { get; set; }
    public bool IsActive { get; set; }
}

public class EmployeeLeaveDto
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public long LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public string Reason { get; set; } = string.Empty;
    public SchoolManagementSystem.Models.Enums.LeaveStatus Status { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EmployeeLeaveSummaryDto
{
    public int TotalLeaveTaken { get; set; }
    public int RemainingBalance { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedLeaves { get; set; }
    public int RejectedLeaves { get; set; }
    public List<LeaveBalanceDto> Balances { get; set; } = new();
}

public class LeaveBalanceDto
{
    public string LeaveTypeName { get; set; } = string.Empty;
    public int Allowed { get; set; }
    public int Taken { get; set; }
    public int Remaining => Allowed - Taken;
}

public class SalaryStructureDto
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public decimal HouseRent { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal OtherAllowance { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal ProvidentFund { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public bool IsActive { get; set; }
}

public class EmployeePayrollDto
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string DesignationName { get; set; } = string.Empty;
    public int PayrollMonth { get; set; }
    public int PayrollYear { get; set; }
    public int WorkingDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LeaveDays { get; set; }
    public int PaidLeaveDays { get; set; }
    public int UnpaidLeaveDays { get; set; }
    public int LateDays { get; set; }
    public double OvertimeHours { get; set; }
    public decimal BonusAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }
    public SchoolManagementSystem.Models.Enums.PayrollPaymentStatus PaymentStatus { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? Remarks { get; set; }
    public string? GeneratedByName { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class PayrollSummaryDto
{
    public decimal TotalExpense { get; set; }
    public int TotalPaid { get; set; }
    public int TotalPending { get; set; }
    public decimal AverageSalary { get; set; }
}
