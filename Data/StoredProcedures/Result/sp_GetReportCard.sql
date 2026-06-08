CREATE OR ALTER PROCEDURE [dbo].[sp_GetReportCard]
    @ExamId INT,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- School info
    SELECT TOP 1
        SchoolName,
        Address AS SchoolAddress,
        EIIN,
        LogoPath AS SchoolLogoPath
    FROM SchoolSettings
    WHERE IsDeleted = 0;

    -- Student info
    SELECT 
        s.Id AS StudentId,
        s.FullName AS StudentName,
        s.StudentNo,
        s.RollNumber,
        s.DateOfBirth,
        s.FatherName,
        s.MotherName,
        s.ClassId,
        cl.Name AS ClassName,
        s.SectionId,
        sec.Name AS SectionName,
        s.StudentGroupId,
        sg.Name AS GroupName,
        s.ProfilePicturePath AS PhotoPath
    FROM Students s
    INNER JOIN Classes cl ON s.ClassId = cl.Id
    LEFT JOIN Sections sec ON s.SectionId = sec.Id
    LEFT JOIN StudentGroups sg ON s.StudentGroupId = sg.Id
    WHERE s.Id = @StudentId;

    -- Exam info
    SELECT 
        e.Id AS ExamId,
        e.Name AS ExamName,
        e.Term,
        e.StartsOn,
        e.EndsOn,
        e.AcademicYearId,
        ay.Name AS AcademicYearName
    FROM Exams e
    INNER JOIN AcademicYears ay ON e.AcademicYearId = ay.Id
    WHERE e.Id = @ExamId;

    -- Subject-wise marks and grades (filtered by student's religion & group)
    SELECT 
        ssr.SubjectId,
        sub.Name AS SubjectName,
        sub.Code AS SubjectCode,
        ssr.FullMarks,
        ssr.PassMarks,
        ssr.MarksObtained,
        ssr.Grade,
        ssr.GradePoint,
        CAST(ssr.IsPassed AS BIT) AS IsPassed,
        ssr.IsOptionalSubject,
        ssr.IsReligionSubject,
        es.WrittenMarks,
        es.MCQMarks,
        es.PracticalMarks,
        es.VivaMarks,
        es.LabMarks,
        es.OralMarks,
        es.AssignmentMarks,
        es.ContinuousAssessmentMarks,
        m.WrittenMarks AS MarksWritten,
        m.MCQMarks AS MarksMCQ,
        m.PracticalMarks AS MarksPractical,
        m.VivaMarks AS MarksViva,
        m.LabMarks AS MarksLab,
        m.OralMarks AS MarksOral,
        m.AssignmentMarks AS MarksAssignment,
        m.ContinuousAssessmentMarks AS MarksContinuousAssessment,
        m.ComponentValues
    FROM StudentSubjectResults ssr
    INNER JOIN Subjects sub ON ssr.SubjectId = sub.Id
    INNER JOIN Students s ON s.Id = @StudentId
    LEFT JOIN ExamSubjects es ON es.ExamId = @ExamId AND es.SubjectId = ssr.SubjectId
    LEFT JOIN Marks m ON m.ExamId = @ExamId AND m.StudentId = @StudentId AND m.SubjectId = ssr.SubjectId AND m.IsDeleted = 0
    WHERE ssr.ExamId = @ExamId AND ssr.StudentId = @StudentId AND ssr.IsDeleted = 0
      AND (
          -- Include non-religion subjects
          (sub.ReligionType IS NULL)
          OR
          -- Include only the student's assigned religion subject
          (sub.ReligionType IS NOT NULL AND s.AssignedReligionSubjectId = ssr.SubjectId)
      )
    ORDER BY sub.DisplayOrder;

    -- Overall result
    SELECT 
        ser.TotalMarks,
        ser.TotalFullMarks,
        ser.Gpa,
        ser.Grade,
        ser.Position,
        ser.ClassPosition,
        ser.GroupPosition,
        CAST(ser.IsPassed AS BIT) AS IsPassed,
        ser.FailedSubjectCount,
        ser.PassedSubjectCount,
        ser.Status,
        ser.PublishedAt
    FROM StudentExamResults ser
    WHERE ser.ExamId = @ExamId AND ser.StudentId = @StudentId AND ser.IsDeleted = 0;
END;
GO