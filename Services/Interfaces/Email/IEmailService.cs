namespace SchoolManagementSystem.Services.Interfaces.Email;

public interface IEmailService
{
    Task SendStudentActivationAsync(string toEmail, string applicantName, string UserName,string token, CancellationToken cancellationToken = default);
    Task SendAdmissionReceivedAsync(string toEmail, string applicantName, string applicationNo, CancellationToken cancellationToken = default);
    Task SendTeacherAccountAsync(string toEmail, string teacherName, string userName, string password, CancellationToken cancellationToken = default);
    Task SendEmployeeAccountAsync(string toEmail, string employeeName, string userName, string password, CancellationToken cancellationToken = default);
    Task SendEmployeeInvitationAsync(string toEmail, string employeeName, string invitationToken, DateTime expiresAt, CancellationToken cancellationToken = default);
}

