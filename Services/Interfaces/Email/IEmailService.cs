namespace SchoolManagementSystem.Services.Interfaces.Email;

public interface IEmailService
{
    Task SendStudentActivationAsync(string toEmail, string applicantName, string UserName,string token, CancellationToken cancellationToken = default);
    Task SendAdmissionReceivedAsync(string toEmail, string applicantName, string applicationNo, CancellationToken cancellationToken = default);
    Task SendTeacherAccountAsync(string toEmail, string teacherName, string userName, string password, CancellationToken cancellationToken = default);
    Task SendEmployeeAccountAsync(string toEmail, string employeeName, string userName, string password, CancellationToken cancellationToken = default);
    Task SendEmployeeInvitationAsync(string toEmail, string employeeName, string invitationToken, DateTime expiresAt, CancellationToken cancellationToken = default);
    Task SendPasswordResetAsync(string toEmail, string userName, string otp, CancellationToken cancellationToken = default);
    Task SendAttendanceNotificationAsync(string toEmail, string studentName, string rollNumber, string className, string sectionName, DateOnly attendanceDate, string schoolName, CancellationToken cancellationToken = default);
    Task SendGuardianActivationAsync(string toEmail, string guardianName, string userName, string token, string activationBaseUrl, CancellationToken cancellationToken = default);
    Task SendWelcomeEmailAsync(string toEmail, string studentName, string userName, int studentId, string className, string sectionName, string portalUrl, CancellationToken cancellationToken = default);
}

