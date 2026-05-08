using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Helpers.Pdf;

public interface IPdfGenerator
{
   // byte[] GenerateReportCard(string title, IReadOnlyDictionary<string, string> fields);
    byte[] GenerateSchoolReportCard( StudentExamResult result,List<MarkEntry> marks);
}
