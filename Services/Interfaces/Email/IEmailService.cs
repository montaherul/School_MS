namespace SchoolManagementSystem.Services.Interfaces.Email;

public interface IEmailService
{
    Task SendStudentActivationAsync(string toEmail, string token, CancellationToken cancellationToken = default);
}

