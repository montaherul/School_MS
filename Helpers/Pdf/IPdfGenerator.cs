using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.ViewModels.Student;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Website;

namespace SchoolManagementSystem.Helpers.Pdf;

public interface IPdfGenerator
{
    byte[] GenerateSchoolReportCard(StudentExamResult result, List<MarkEntry> marks, SchoolSetting school);
    byte[] GenerateStudentIdCardPdf(IdCardPrintViewModel model);
    byte[] GenerateEmployeeIdCardPdf(EmployeeIdCardPrintViewModel model);
    byte[] GenerateTranscript(StudentTranscriptDto transcript);
    byte[] GenerateFromHtml(string html);
}
