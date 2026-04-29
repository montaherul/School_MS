namespace SchoolManagementSystem.Helpers.Pdf;

public interface IPdfGenerator
{
    byte[] GenerateReportCard(string title, IReadOnlyDictionary<string, string> fields);
}
