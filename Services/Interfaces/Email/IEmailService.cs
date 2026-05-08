namespace SchoolManagementSystem.Services.Interfaces.Email;

public interface IEmailService
{
    Task SendStudentActivationAsync(string toEmail, string applicantName, string UserName,string token, CancellationToken cancellationToken = default);
    Task SendAdmissionReceivedAsync(string toEmail, string applicantName, string applicationNo, CancellationToken cancellationToken = default);
    Task SendTeacherAccountAsync(string toEmail, string teacherName, string userName, string password, CancellationToken cancellationToken = default);
}
