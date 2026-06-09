using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Website;

namespace SchoolManagementSystem.Helpers.Pdf;

public interface IPdfGenerator
{
    byte[] GenerateSchoolReportCard(StudentExamResult result, List<MarkEntry> marks, SchoolSetting school);
    byte[] GenerateStudentIdCardFromHtml(string html);
    byte[] GenerateBulkStudentIdCardPdfFromHtml(string html);
    byte[] GenerateEmployeeIdCard(SchoolManagementSystem.Models.DTOs.Employee.EmployeeDetailsDto employee, SchoolSetting schoolSetting);
    byte[] GenerateEmployeeIdCardFromHtml(string html);
    byte[] GenerateBulkEmployeeIdCardPdfFromHtml(string html);
    byte[] GenerateTranscript(SchoolManagementSystem.Models.DTOs.Result.StudentTranscriptDto transcript);
}
