using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.ViewModels.Auth;

public class LoginViewModel
{
    [Required]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}

public class ForgotPasswordViewModel
{
    [Required]
    [Display(Name = "Username or Email")]
    public string UserNameOrEmail { get; set; } = string.Empty;
}

public class ResetPasswordViewModel
{
    [Required]
    [Display(Name = "Username or Email")]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(12, MinimumLength = 4, ErrorMessage = "Invalid OTP format.")]
    [Display(Name = "OTP")]
    public string Otp { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(128, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class SetPasswordViewModel
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Password must include uppercase, lowercase, and a digit.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class VerifyOtpViewModel
{
    public string UserNameOrEmail { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
}
