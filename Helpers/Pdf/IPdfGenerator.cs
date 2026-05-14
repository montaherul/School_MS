using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Result;
using System.IO;

namespace SchoolManagementSystem.Helpers.Pdf;

public interface IPdfGenerator
{
   // byte[] GenerateReportCard(string title, IReadOnlyDictionary<string, string> fields);
    byte[] GenerateSchoolReportCard( StudentExamResult result,List<MarkEntry> marks);
    byte[] GenerateStudentIdCard(StudentUpsertDto student);
}
