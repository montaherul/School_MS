using SchoolManagementSystem.Models.ViewModels.Auth;
using System.Security.Claims;

namespace SchoolManagementSystem.Services.Interfaces.Auth;

public interface IAuthService
{
    Task<(bool success, string? message, ClaimsIdentity? identity)> LoginAsync(LoginViewModel model, CancellationToken ct = default);
    Task RecordLoginSessionAsync(int userId, string sessionId, string? ipAddress, string? userAgent, CancellationToken ct = default);
    Task RecordLogoutSessionAsync(string sessionId, CancellationToken ct = default);
    Task<(bool success, string message)> ForgotPasswordAsync(ForgotPasswordViewModel model, CancellationToken ct = default);
    Task<(bool success, string message)> VerifyOtpAsync(VerifyOtpViewModel model, CancellationToken ct = default);
    Task<(bool success, string message)> ResetPasswordAsync(ResetPasswordViewModel model, CancellationToken ct = default);
    Task<(bool success, string message)> ActivateAccountAsync(SetPasswordViewModel model, CancellationToken ct = default);
    Task<bool> IsActivationTokenValidAsync(string token, CancellationToken ct = default);
}

