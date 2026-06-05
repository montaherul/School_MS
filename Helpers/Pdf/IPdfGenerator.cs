using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Website;
using System.IO;

namespace SchoolManagementSystem.Helpers.Pdf;

public interface IPdfGenerator
{
   // byte[] GenerateReportCard(string title, IReadOnlyDictionary<string, string> fields);
    byte[] GenerateSchoolReportCard( StudentExamResult result,List<MarkEntry> marks,SchoolSetting school);
    byte[] GenerateStudentIdCard(StudentUpsertDto student, SchoolSetting school);
    byte[] GenerateEmployeeIdCard(SchoolManagementSystem.Models.DTOs.Employee.EmployeeDetailsDto employee, SchoolManagementSystem.Models.Entities.Website.SchoolSetting schoolSetting);
}
