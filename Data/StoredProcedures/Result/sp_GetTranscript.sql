CREATE OR ALTER PROCEDURE [dbo].[sp_GetTranscript]
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

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
        (SELECT TOP 1 Name FROM AcademicYears WHERE IsActive = 1) AS CurrentAcademicYear
FROM Students s WITH(NOLOCK)
INNER JOIN Classes cl WITH(NOLOCK) ON s.ClassId = cl.Id
LEFT JOIN Sections sec WITH(NOLOCK) ON s.SectionId = sec.Id
LEFT JOIN StudentGroups sg WITH(NOLOCK) ON s.StudentGroupId = sg.Id
    WHERE s.Id = @StudentId;

    -- All exam results across all academic years
    SELECT 
        ay.Id AS AcademicYearId,
        ay.Name AS AcademicYearName,
        e.Id AS ExamId,
        e.Name AS ExamName,
        CASE e.Term
            WHEN 1 THEN 'First Terminal'
            WHEN 2 THEN 'Half Yearly'
            WHEN 3 THEN 'Second Terminal'
            WHEN 4 THEN 'Annual'
            WHEN 5 THEN 'Final'
            WHEN 6 THEN 'Pre Test'
            WHEN 7 THEN 'Test'
            WHEN 8 THEN 'Other'
            ELSE CAST(e.Term AS NVARCHAR)
        END AS Term,
        e.StartsOn,
        e.EndsOn,
        ser.TotalMarks,
        ser.TotalFullMarks,
        ser.Gpa,
        ser.Grade,
        ser.Position,
        ser.ClassPosition,
        ser.IsPassed,
        ser.FailedSubjectCount,
        ser.PassedSubjectCount
FROM StudentExamResults ser WITH(NOLOCK)
INNER JOIN Exams e WITH(NOLOCK) ON ser.ExamId = e.Id
INNER JOIN AcademicYears ay WITH(NOLOCK) ON e.AcademicYearId = ay.Id
    WHERE ser.StudentId = @StudentId
      AND ser.IsDeleted = 0
      AND e.IsDeleted = 0
      AND ser.Status IN (5, 6) -- Published (5) or Locked (6) per ResultWorkflowStatus enum
    ORDER BY ay.Id, e.EndsOn;

    -- All subject results (filtered by student's religion & group)
    SELECT 
        e.Id AS ExamId,
        e.Name AS ExamName,
        ssr.SubjectId,
        sub.Name AS SubjectName,
        sub.Code AS SubjectCode,
        ssr.MarksObtained,
        ssr.FullMarks,
        ssr.PassMarks,
        ssr.Grade,
        ssr.GradePoint,
        CAST(ssr.IsPassed AS BIT) AS IsPassed
FROM StudentSubjectResults ssr WITH(NOLOCK)
INNER JOIN Exams e WITH(NOLOCK) ON ssr.ExamId = e.Id
INNER JOIN Subjects sub WITH(NOLOCK) ON ssr.SubjectId = sub.Id
INNER JOIN Students s WITH(NOLOCK) ON s.Id = @StudentId
LEFT JOIN ClassSubjects cs WITH(NOLOCK) ON cs.SchoolClassId = s.ClassId AND cs.SubjectId = ssr.SubjectId AND cs.IsDeleted = 0
    WHERE ssr.StudentId = @StudentId
      AND ssr.IsDeleted = 0
      AND e.IsDeleted = 0
      AND (
          -- Non-religion subjects
          (sub.ReligionType IS NULL)
          OR
          -- Only the student's assigned religion subject
          (sub.ReligionType IS NOT NULL AND s.AssignedReligionSubjectId = ssr.SubjectId)
      )
    ORDER BY e.EndsOn DESC, sub.DisplayOrder;

    -- Overall stats
    SELECT
        COUNT(DISTINCT ser.ExamId) AS TotalExamsTaken,
        COUNT(DISTINCT e.AcademicYearId) AS TotalAcademicYears,
        ROUND(AVG(ser.Gpa), 2) AS AverageGPA,
        MAX(ser.Gpa) AS BestGPA,
        COUNT(CASE WHEN ser.IsPassed = 1 THEN 1 END) AS PassedExams,
        COUNT(CASE WHEN ser.IsPassed = 0 THEN 1 END) AS FailedExams
FROM StudentExamResults ser WITH(NOLOCK)
INNER JOIN Exams e WITH(NOLOCK) ON ser.ExamId = e.Id
    WHERE ser.StudentId = @StudentId
      AND ser.IsDeleted = 0
      AND e.IsDeleted = 0
      AND ser.Status IN (5, 6); -- Published (5) or Locked (6) per ResultWorkflowStatus enum
END;
GO