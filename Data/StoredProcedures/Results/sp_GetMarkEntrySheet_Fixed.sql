CREATE OR ALTER PROCEDURE [dbo].[sp_GetMarkEntrySheet]
    @ExamId INT,
    @ClassId INT,
    @SectionId INT,
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.Id as StudentId,
        s.FullName as StudentName,
        s.StudentNo,
        s.RollNumber,
        m.MarksObtained,
        m.Grade,
        m.IsLocked

FROM Students s WITH(NOLOCK)

INNER JOIN Subjects sub WITH(NOLOCK)
        ON sub.Id = @SubjectId

LEFT JOIN Marks m WITH(NOLOCK) 
        ON s.Id = m.StudentId 
        AND m.ExamId = @ExamId 
        AND m.SubjectId = @SubjectId
        AND m.IsDeleted = 0

    WHERE 
        s.ClassId = @ClassId
        AND s.SectionId = @SectionId
        AND s.Status = 1
        AND s.IsDeleted = 0

        AND
        (
            sub.ReligionType IS NULL
            OR s.Religion = sub.ReligionType
        )

    ORDER BY s.RollNumber;
END;
GO