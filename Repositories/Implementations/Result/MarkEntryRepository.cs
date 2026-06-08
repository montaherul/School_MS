using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using System.Data;
using System.Data.Common;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class MarkEntryRepository : BaseRepository<MarkEntry>, IMarkEntryRepository
{
    public MarkEntryRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<MarkEntrySheetDto>> GetMarkEntrySheetAsync(int examId, int classId, int sectionId, int subjectId, CancellationToken ct)
    {
        return await _db.Students.AsNoTracking()
            .Where(s => s.ClassId == classId && s.SectionId == sectionId && !s.IsDeleted)
            .GroupJoin(
                _db.Marks.Where(m => m.ExamId == examId && m.SubjectId == subjectId),
                s => s.Id,
                m => m.StudentId,
                (s, marks) => new { Student = s, Marks = marks })
            .Select(x => new MarkEntrySheetDto
            {
                StudentId = x.Student.Id,
                StudentNo = x.Student.StudentNo,
                StudentName = x.Student.FullName,
                RollNumber = x.Student.RollNumber,
                MarksObtained = x.Marks.Select(m => m.MarksObtained).FirstOrDefault(),
                Grade = x.Marks.Select(m => m.Grade).FirstOrDefault(),
                IsLocked = x.Marks.Select(m => m.IsLocked).FirstOrDefault(),
                WrittenMarks = x.Marks.Select(m => m.WrittenMarks).FirstOrDefault(),
                MCQMarks = x.Marks.Select(m => m.MCQMarks).FirstOrDefault(),
                CQMarks = x.Marks.Select(m => m.CQMarks).FirstOrDefault(),
                PracticalMarks = x.Marks.Select(m => m.PracticalMarks).FirstOrDefault(),
                VivaMarks = x.Marks.Select(m => m.VivaMarks).FirstOrDefault(),
                LabMarks = x.Marks.Select(m => m.LabMarks).FirstOrDefault(),
                OralMarks = x.Marks.Select(m => m.OralMarks).FirstOrDefault(),
                AssignmentMarks = x.Marks.Select(m => m.AssignmentMarks).FirstOrDefault(),
                ContinuousAssessmentMarks = x.Marks.Select(m => m.ContinuousAssessmentMarks).FirstOrDefault(),
                CompetencyMarks = x.Marks.Select(m => m.CompetencyMarks).FirstOrDefault(),
                BehaviourMarks = x.Marks.Select(m => m.BehaviourMarks).FirstOrDefault(),
                ParticipationMarks = x.Marks.Select(m => m.ParticipationMarks).FirstOrDefault(),
                ComponentValues = x.Marks.Select(m => m.ComponentValues).FirstOrDefault(),
                EnteredByTeacherId = x.Marks.Select(m => m.EnteredByTeacherId).FirstOrDefault()
            })
            .OrderBy(x => x.RollNumber)
            .ToListAsync(ct);
    }

    public async Task<List<MarksEntryStudentDto>> GetMarksEntryListAsync(int examId, int classId, int sectionId, int subjectId, CancellationToken ct)
    {
        var result = new List<MarksEntryStudentDto>();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetMarksEntryList]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@ExamId", examId);
        AddParameter(command, "@ClassId", classId);
        AddParameter(command, "@SectionId", sectionId);
        AddParameter(command, "@SubjectId", subjectId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            result.Add(new MarksEntryStudentDto
            {
                StudentId = GetInt32(reader, "StudentId"),
                StudentName = GetString(reader, "StudentName"),
                StudentNo = GetString(reader, "StudentNo"),
                RollNumber = GetString(reader, "RollNumber"),
                ClassId = GetInt32(reader, "ClassId"),
                SectionId = GetInt32(reader, "SectionId"),
                ClassName = GetString(reader, "ClassName"),
                SectionName = GetString(reader, "SectionName"),
                MarkId = GetNullableInt32(reader, "MarkId"),
                MarksObtained = GetNullableDecimal(reader, "MarksObtained"),
                WrittenMarks = GetNullableDecimal(reader, "WrittenMarks"),
                MCQMarks = GetNullableDecimal(reader, "MCQMarks"),
                CQMarks = GetNullableDecimal(reader, "CQMarks"),
                PracticalMarks = GetNullableDecimal(reader, "PracticalMarks"),
                AssignmentMarks = GetNullableDecimal(reader, "AssignmentMarks"),
                VivaMarks = GetNullableDecimal(reader, "VivaMarks"),
                LabMarks = GetNullableDecimal(reader, "LabMarks"),
                ContinuousAssessmentMarks = GetNullableDecimal(reader, "ContinuousAssessmentMarks"),
                OralMarks = GetNullableDecimal(reader, "OralMarks"),
                CompetencyMarks = GetNullableDecimal(reader, "CompetencyMarks"),
                BehaviourMarks = GetNullableDecimal(reader, "BehaviourMarks"),
                ParticipationMarks = GetNullableDecimal(reader, "ParticipationMarks"),
                ComponentValues = GetNullableString(reader, "ComponentValues"),
                Grade = GetNullableString(reader, "Grade"),
                GradePoint = GetNullableDecimal(reader, "GradePoint"),
                IsLocked = GetNullableBoolean(reader, "IsLocked"),
                MarkStatus = GetNullableInt32(reader, "MarkStatus"),
                HasEntry = GetBoolean(reader, "HasEntry")
            });
        }
        return result;
    }
}
